// <copyright file="BaseDirectoryService.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

using DirectoryServices.Models;
using DirectoryServices.Settings;

namespace DirectoryServices.Interfaces
{
    public abstract class BaseDirectoryService : IDirectoryService
    {
        internal readonly string LDAP_MEMBER = "member:1.2.840.113556.1.4.1941:={0}";
        internal readonly string LDAP_MEMBER_OF = "memberOf:1.2.840.113556.1.4.1941:={0}";
        internal readonly string LDAP_SID = "objectSID={0}";
        internal readonly int PAGE_ONE = 1;
        internal readonly int SINGLE_LIMIT = 2;
        internal readonly int AD_LIMIT = 1000;

        internal readonly string[] _userProperties = new string[]
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

        internal readonly string[] _groupProperties = new string[]
        {
            "distinguishedName",
            "objectGUID",
            "objectSID",
            "member",
            "memberOf",
            "name"
        };

        public abstract IReadOnlyCollection<Group> GetAncestors(string distinguishedName, string? ancestorSearchBase = null);
        public abstract IReadOnlyCollection<User> GetMembers(string distinguishedName, string? memberSearchBase = null);
        public abstract Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false);
        public abstract IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false);
        public abstract User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false);
        public abstract IReadOnlyCollection<User> SearchUsersByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false);

        internal string SanitiseDistinguishedName(string? distinguishedName, DirectorySettings directorySettings)
        {
            if (string.IsNullOrEmpty(distinguishedName))
            {
                return string.Empty;
            }

            foreach (var keyPair in directorySettings.DistinguishedNameEscapeCharacters)
            {
                distinguishedName = distinguishedName.Replace(keyPair.Key, keyPair.Value);
            }

            return distinguishedName.Trim();
        }
    }
}