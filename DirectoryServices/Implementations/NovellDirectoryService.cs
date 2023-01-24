using System.Collections.Generic;

using DirectoryServices.Interfaces;
using DirectoryServices.Models;

namespace DirectoryServices.Impl
{
    public class NovellDirectoryService : IDirectoryService
    {
        public IReadOnlyCollection<Group> GetAncestors(string distinguishedName, string? ancestorSearchBase = null)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<User> GetMembers(string distinguishedName, string? memberSearchBase = null)
        {
            throw new System.NotImplementedException();
        }

        public Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<User> SearchUsersByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false)
        {
            throw new System.NotImplementedException();
        }
    }
}