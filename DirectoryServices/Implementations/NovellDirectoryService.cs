// <copyright file="NovellDirectoryService.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DirectoryServices.Exceptions;
using DirectoryServices.Extensions;
using DirectoryServices.Helpers;
using DirectoryServices.Interfaces;
using DirectoryServices.Models;
using DirectoryServices.Settings;

using Microsoft.Extensions.Logging;

using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Controls;

namespace DirectoryServices.Impl
{
    public class NovellDirectoryService : BaseDirectoryService
    {
        private readonly ILogger<NovellDirectoryService> _logger;
        private readonly DirectorySettings _directorySettings;
        private readonly NovellHelper _ldapHelper;

        public NovellDirectoryService(ILogger<NovellDirectoryService> logger, DirectorySettings directorySettings)
        {
            _logger = logger;
            _directorySettings = directorySettings;
            _ldapHelper = new NovellHelper(logger);

            foreach (var keyPair in directorySettings.DistinguishedNameEscapeCharacters)
            {
                _logger.LogDebug("Escap Chars Entry: '{key}' -> '{value}'", keyPair.Key, keyPair.Value);
            }
        }

        public override IReadOnlyCollection<Group> GetAncestors(string? distinguishedName, string? ancestorSearchBase = null)
        {
            var sanitisedDN = SanitiseDistinguishedName(distinguishedName, _directorySettings);

            var query = string.Format(LDAP_MEMBER, sanitisedDN);

            return GetGroups(query, PAGE_ONE, AD_LIMIT, ancestorSearchBase);
        }

        public override IReadOnlyCollection<User> GetMembers(string? distinguishedName, string? memberSearchBase = null)
        {
            var sanitisedDN = SanitiseDistinguishedName(distinguishedName, _directorySettings);

            var query = string.Format(LDAP_MEMBER_OF, sanitisedDN);

            return GetUsers(query, PAGE_ONE, AD_LIMIT, memberSearchBase);
        }

        public override Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_SID, GetOctetStringRepresentation(SecureIdentifierHelper.Encode(sid)));

            var groups = GetGroups(query, PAGE_ONE, SINGLE_LIMIT, searchBase: null);

            if (groups.Count == 1)
            {
                var group = groups.First();

                if (includeMembers)
                {
                    group.Members = GetMembers(group.DistinguishedName, memberSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                if (includeAncestors)
                {
                    group.Ancestors = GetAncestors(group.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                return group;
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

        public override IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string query, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            var groups = GetGroups(query, page, pageSize, searchBase);

            if (includeMembers)
            {
                Parallel.ForEach(groups, new ParallelOptions { MaxDegreeOfParallelism = _directorySettings.Threads }, group =>
                {
                    group.Members = GetMembers(group.DistinguishedName, memberSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                });
            }

            if (includeAncestors)
            {
                Parallel.ForEach(groups, new ParallelOptions { MaxDegreeOfParallelism = _directorySettings.Threads }, group =>
                {
                    group.Ancestors = GetAncestors(group.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                });
            }

            return groups;
        }

        public override User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_SID, GetOctetStringRepresentation(SecureIdentifierHelper.Encode(sid)));

            var users = GetUsers(query, PAGE_ONE, SINGLE_LIMIT);

            if (users.Count == 1)
            {
                var user = users.First();

                if (includeAncestors)
                {
                    user.Groups = GetAncestors(user.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                return user;
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

        public override IReadOnlyCollection<User> SearchUsersByLdapQuery(string query, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            var users = GetUsers(query, page, pageSize, searchBase);

            if (includeAncestors)
            {
                Parallel.ForEach(users, new ParallelOptions { MaxDegreeOfParallelism = _directorySettings.Threads }, user =>
                {
                    user.Groups = GetAncestors(user.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                });
            }

            return users;
        }

        private IReadOnlyCollection<Group> GetGroups(string query, int page, int pageSize, string? searchBase = null)
        {
            var baseSearch = string.IsNullOrEmpty(searchBase) ? _directorySettings.DefaultLdapGroupsSearchBase : searchBase;

            return AsGroups(QueryLdap(query, page, pageSize, _groupProperties, baseSearch).ToList().AsReadOnly());
        }

        private IReadOnlyCollection<User> GetUsers(string query, int page, int pageSize, string? searchBase = null)
        {
            var baseSearch = string.IsNullOrEmpty(searchBase) ? _directorySettings.DefaultLdapUsersSearchBase : searchBase;

            return AsUsers(QueryLdap(query, page, pageSize, _userProperties, baseSearch).ToList().AsReadOnly());
        }

        private IReadOnlyCollection<Group> AsGroups(IReadOnlyCollection<LdapEntry> results)
        {
            return results.Select(_ => _ldapHelper.FromLdapResultGroup(_)).ToList().AsReadOnly();
        }

        private IReadOnlyCollection<User> AsUsers(IReadOnlyCollection<LdapEntry> results)
        {
            return results.Select(_ => _ldapHelper.FromLdapEntry(_)).ToList().AsReadOnly();
        }

        private IEnumerable<LdapEntry> QueryLdap(string? query, int page, int pageSize, string[] propertiesToLoad, string? searchBase)
        {
            var connection = new LdapConnection();

            _logger.LogDebug("Querying LDAP with query '{query}' and search base '{base}'", query, searchBase);

            var results = GetLdapResults(connection, query, page, pageSize, propertiesToLoad, searchBase);

            while (results.HasMore())
            {
                LdapEntry? ldapEntry = null;

                try
                {
                    var total = GetTotal((LdapSearchResults)results);

                    _logger.LogDebug("Get toal returned '{total}'", total);

                    var result = results.Next();

                    _logger.LogDebug("Found entry '{entry}'", result.Dn);

                    ldapEntry = result;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error encountered querying ldap with query '{query}'", query);
                }

                if (ldapEntry != null)
                {
                    yield return ldapEntry;
                }
            }

            connection.Disconnect();
        }

        private int? GetTotal(LdapSearchResults results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (results.ResponseControls != null && results.ResponseControls.Any())
            {
                foreach (var control in results.ResponseControls)
                {
                    _logger.LogTrace("Found control {control}", control.Id);

                    if (control.Id == "2.16.840.1.113730.3.4.10")
                    {
                        var response = new LdapVirtualListResponse(control.Id, control.Critical, control.GetValue());

                        return response.ContentCount;
                    }
                }
            }

            return null;
        }

        private ILdapSearchResults GetLdapResults(LdapConnection connection, string? query, int page, int pageSize, string[] propertiesToLoad, string? searchBase)
        {
            _logger.LogTrace("GetLdapResults({query},{page},...)", query, page);

            var rootEntry = searchBase;

            connection.SecureSocketLayer = _directorySettings.LdapSecure;
            connection.Connect(_directorySettings.LdapConnectionUrl, _directorySettings.LdapSecure ? LdapConnection.DefaultSslPort : LdapConnection.DefaultPort);

            var ldapSearchConstraints = new LdapSearchConstraints();

            ldapSearchConstraints.ReferralFollowing = _directorySettings.LdapFollowReferrals;
            ldapSearchConstraints.AddPagination(page, pageSize);

            connection.Bind(_directorySettings.BindDistinguishedName, _directorySettings.BindPassword);

            return connection.Search(rootEntry, LdapConnection.ScopeSub, query, propertiesToLoad, false, ldapSearchConstraints);
        }

        public override User? SearchUserByGuid(string guid, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_GUID, GetOctetStringRepresentation(GuidHelper.Encode(guid)));

            var users = GetUsers(query, PAGE_ONE, SINGLE_LIMIT);

            if (users.Count == 1)
            {
                var user = users.First();

                if (includeAncestors)
                {
                    user.Groups = GetAncestors(user.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                return user;
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

        public override Group? SearchGroupByGuid(string guid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            var query = string.Format(LDAP_GUID, GetOctetStringRepresentation(GuidHelper.Encode(guid)));

            var groups = GetGroups(query, PAGE_ONE, SINGLE_LIMIT, searchBase: null);

            if (groups.Count == 1)
            {
                var group = groups.First();

                if (includeMembers)
                {
                    group.Members = GetMembers(group.DistinguishedName, memberSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                if (includeAncestors)
                {
                    group.Ancestors = GetAncestors(group.DistinguishedName, ancestorSearchBase).Select(_ => new { Key = _.Sid, Value = _ }).ToDictionary(_ => _.Key, _ => _.Value);
                }

                return group;
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
    }
}