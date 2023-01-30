// <copyright file="NovellDirectoryService.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

using DirectoryServices.Interfaces;
using DirectoryServices.Models;

namespace DirectoryServices.Impl
{
    public class NovellDirectoryService : IBaseDirectoryService
    {
        public override IReadOnlyCollection<Group> GetAncestors(string distinguishedName, string? ancestorSearchBase = null)
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyCollection<User> GetMembers(string distinguishedName, string? memberSearchBase = null)
        {
            throw new System.NotImplementedException();
        }

        public override Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public override User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyCollection<User> SearchUsersByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }
    }
}