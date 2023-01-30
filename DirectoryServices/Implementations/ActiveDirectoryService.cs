// <copyright file="ActiveDirectoryService.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

using DirectoryServices.Exceptions;
using DirectoryServices.Helpers;
using DirectoryServices.Interfaces;
using DirectoryServices.Models;
using DirectoryServices.Settings;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryServices.Impl
{
    public class ActiveDirectoryService : IDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;

        private readonly ActiveDirectoryHelper _activeDirectoryHelper;
        private readonly string? _ldapConnectionString;
        private readonly DirectorySettings _directorySettings;

        private readonly string LDAP_MEMBER = "member:1.2.840.113556.1.4.1941:={0}";
        private readonly string LDAP_MEMBER_OF = "memberOf:1.2.840.113556.1.4.1941:={0}";
        private readonly string LDAP_SID = "objectSID={0}";
        private readonly int PAGE_ONE = 1;
        private readonly int SINGLE_LIMIT = 2;
        private readonly int AD_LIMIT = 1000;

        private readonly string[] _userProperties = new string[]
        {
            "displayName",
            "distinguishedName",
            "givenName",
            "objectGUID",
            "objectSID",
            "mail",
            "member",
            "memberOf",
            "name",
            "sAMAccountName",
            "sn",
            "telephoneNumber",
            "userPrincipalName",
            "userAccountControl"
        };

        private readonly string[] _groupProperties = new string[]
        {
            "distinguishedName",
            "objectGUID",
            "objectSID",
            "member",
            "memberOf",
            "name"
        };

        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger, IOptions<DirectorySettings> directorySettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _directorySettings = directorySettings.Value ?? throw new ArgumentNullException(nameof(directorySettings));

            _activeDirectoryHelper = new ActiveDirectoryHelper(logger);

            // Obtain forest information
            try
            {
                var forest = Forest.GetCurrentForest();

                foreach (var domain in forest.Domains)
                {
                    _logger.LogDebug("Found domain: '{domain}'", domain);

                    using var directoryEntry = new DirectoryEntry($"LDAP://{domain}/RootDSE");

                    foreach (PropertyValueCollection property in directoryEntry.Properties)
                    {
                        _logger.LogTrace("Directory entry property: '{name}' -> '{value}'", property.PropertyName, property.Value);
                    }

                    var defaultNamingContext = directoryEntry.Properties["defaultNamingContext"].Value?.ToString();
                    var rootDomainNamingContext = directoryEntry.Properties["rootDomainNamingContext"].Value?.ToString();

                    if (defaultNamingContext != rootDomainNamingContext)
                    {
                        _logger.LogDebug("Setting LDAP connection string to 'LDAP://{connection}'", defaultNamingContext);
                        _ldapConnectionString = $"LDAP://{defaultNamingContext}";
                    }
                }

                if (string.IsNullOrEmpty(_ldapConnectionString))
                {
                    _logger.LogWarning("Failed to find a valid ldap connection string. Falling back to '{ldap}'.", _directorySettings.LdapConnectionFallback, exception);
                    _ldapConnectionString = _directorySettings.LdapConnectionFallback;
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to find a valid ldap connection string. Falling back to '{ldap}'.", _directorySettings.LdapConnectionFallback, exception);
                _ldapConnectionString = _directorySettings.LdapConnectionFallback;
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Group> GetAncestors(string distinguishedName, string? ancestorSearchBase = null)
        {
            var sanitisedDN = sanitisedDistinguishedName(distinguishedName);

            var query = string.Format(LDAP_MEMBER, sanitisedDN);

            return GetGroups(query, PAGE_ONE, AD_LIMIT, ancestorSearchBase);
        }

        public IReadOnlyCollection<User> GetMembers(string distinguishedName, string? memberSearchBase = null)
        {
            var sanitisedDN = sanitisedDistinguishedName(distinguishedName);

            var query = string.Format(LDAP_MEMBER_OF, sanitisedDN);

            return GetUsers(query, PAGE_ONE, AD_LIMIT, memberSearchBase);
        }

        public Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_SID, sid);

            var groups = GetGroups(query, PAGE_ONE, SINGLE_LIMIT, groupSearchBase: null, includeMembers, includeAncestors);

            if (groups.Count == 1)
            {
                return groups.First();
            }
            else if (groups.Count > 1)
            {
                throw new MultipleResultsException($"Multiple results returned for query: '{query}'!");
            }
            else
            {
                throw new NoResultsException($"No results returned for query: '{query}'!");
            }
        }

        public IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            return GetGroups(ldapQuery, page, pageSize, searchBase, includeMembers, includeAncestors);
        }

        public User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_SID, sid);

            var users = GetUsers(query, PAGE_ONE, SINGLE_LIMIT, memberSearchBase: null, includeAncestors);

            if (users.Count == 1)
            {
                return users.First();
            }
            else if (users.Count > 1)
            {
                throw new MultipleResultsException($"Multiple results returned for query: '{query}'!");
            }
            else
            {
                throw new NoResultsException($"No results returned for query: '{query}'!");
            }
        }

        public IReadOnlyCollection<User> SearchUsersByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            return GetUsers(ldapQuery, page, pageSize, searchBase, includeAncestors);
        }

        private IReadOnlyCollection<Group> GetGroups(string query, int page, int pageSize, string? groupSearchBase = null, bool? includeMembers = false, bool? includeAncestors = false)
        {
            SearchResultCollection? results = QueryActiveDirectory(query, page, pageSize, _groupProperties, groupSearchBase);

            if (results == null)
            {
                return new List<Group>().AsReadOnly();
            }

            return AsGroups(results);
        }

        private IReadOnlyCollection<Group> AsGroups(SearchResultCollection results)
        {
            return results.Cast<SearchResult>().Select(_ => _activeDirectoryHelper.FromDirectoryEntryGroup(_.GetDirectoryEntry())).ToList().AsReadOnly();
        }

        private IReadOnlyCollection<User> GetUsers(string query, int page, int pageSize, string? memberSearchBase = null, bool? includeAncestors = false)
        {
            SearchResultCollection? directoryUsers = QueryActiveDirectory(query, page, pageSize, _userProperties, memberSearchBase);

            if (directoryUsers == null)
            {
                return new List<User>().AsReadOnly();
            }

            return AsUsers(directoryUsers, includeAncestors);
        }

        private IReadOnlyCollection<User> AsUsers(SearchResultCollection results, bool? includeAncestors = false)
        {
            return results.Cast<SearchResult>().Select(_ => _activeDirectoryHelper.FromDirectoryEntry(_.GetDirectoryEntry(), includeAncestors)).ToList().AsReadOnly();
        }

        private SearchResultCollection? QueryActiveDirectory(string query, int page, int pageSize, string[] propertiesToLoad, string? searchBase = null)
        {
            var rootEntry = string.IsNullOrEmpty(searchBase) ? _ldapConnectionString : string.Format(":DAP://{0}", searchBase);

            var index = (page * pageSize) + 1;
            var before = 0;
            var after = pageSize - 1;

            using var directoryEntry = new DirectoryEntry(rootEntry);

            directoryEntry.AuthenticationType = AuthenticationTypes.Secure |
                AuthenticationTypes.Sealing |
                AuthenticationTypes.ReadonlyServer |
                AuthenticationTypes.Signing;

            using var search = new DirectorySearcher(directoryEntry);

            search.SearchRoot = directoryEntry;
            search.PropertiesToLoad.AddRange(propertiesToLoad);
            search.Filter = query;
            search.Sort = new SortOption("cn", SortDirection.Ascending);
            search.VirtualListView = new DirectoryVirtualListView(before, after, index);
            search.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResultCollection results = search.FindAll();

                return results;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Active Directory Service Search Error", null);
            }

            return null;
        }

        private object sanitisedDistinguishedName(string distinguishedName)
        {
            foreach (var keyPair in _directorySettings.DistinguishedNameEscapeCharacters)
            {
                distinguishedName = distinguishedName.Replace(keyPair.Key, keyPair.Value);
            }

            return distinguishedName.Trim();
        }
    }
}