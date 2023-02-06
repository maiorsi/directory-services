// <copyright file="IDirectoryService.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

using DirectoryServices.Models;

namespace DirectoryServices.Interfaces
{
    /// <summary>
    /// An interface (contract) for implmentations to follow.
    /// </summary>
    public interface IDirectoryService
    {
        /// <summary>
        /// Search LDAP for <c cref="User">Users</c> based on an LDAP query.
        /// </summary>
        /// <param name="ldapQuery">LDAP Query/Filter</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">The number of results to include in a single page of results</param>
        /// <param name="searchBase">Optional: search base dn for searching</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A collection of users</returns>
        IReadOnlyCollection<User> SearchUsersByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? ancestorSearchBase = null, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for <c cref="Group">Groups</c> based on an LDAP query.
        /// </summary>
        /// <param name="ldapQuery">LDAP Query/Filter</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">The number of results to include in a single page of results</param>
        /// <param name="searchBase">Optional: search base dn for searching</param>
        /// <param name="memberSearchBase">Optional: search base dn for searching users</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups/param>
        /// <param name="includeMembers">Optional: Include members with returned objects</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A collection of groups</returns>
        IReadOnlyCollection<Group> SearchGroupsByLdapQuery(string ldapQuery, int page, int pageSize, string? searchBase = null, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for a single user based on a Secure Identifier (SId).
        /// </summary>
        /// <param name="sid">Secure Identifier String</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A single user or null for no user</returns>
        User? SearchUserBySid(string sid, string? ancestorSearchBase = null, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for a single user based on a Guid.
        /// </summary>
        /// <param name="guid">Guid string representation</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A single user or null for no user</returns>
        User? SearchUserByGuid(string guid, string? ancestorSearchBase = null, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for a single group.
        /// based on a Secure Identifier (SId)
        /// </summary>
        /// <param name="sid">Secure Identifier String</param>
        /// <param name="memberSearchBase">Optional: search base dn for searching users</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups</param>
        /// <param name="includeMembers">Optional: Include members with returned objects</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A single group or null for no group</returns>
        Group? SearchGroupBySid(string sid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for a single group.
        /// based on a Guid
        /// </summary>
        /// <param name="guid">Guid string representation</param>
        /// <param name="memberSearchBase">Optional: search base dn for searching users</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups</param>
        /// <param name="includeMembers">Optional: Include members with returned objects</param>
        /// <param name="includeAncestors">Optional: Include ancestor groups with returned objects</param>
        /// <returns>A single group or null for no group</returns>
        Group? SearchGroupByGuid(string guid, string? memberSearchBase = null, string? ancestorSearchBase = null, bool includeMembers = false, bool includeAncestors = false);

        /// <summary>
        /// Search LDAP for all ancestor groups for an LDAP object based on a DN.
        /// </summary>
        /// <param name="distinguishedName">The DN of the LDAP object to search on</param>
        /// <param name="ancestorSearchBase">Optional: search base dn for searching groups/param>
        /// <returns>A collection of groups</returns>
        IReadOnlyCollection<Group> GetAncestors(string distinguishedName, string? ancestorSearchBase = null);

        /// <summary>
        /// Search LDAP for all members for an LDAP object based on a DN.
        /// </summary>
        /// <param name="distinguishedName">The DN of the LDAP object to search on</param>
        /// <param name="memberSearchBase">Optional: search base dn for searching users</param>
        /// <returns>A collection of users</returns>
        IReadOnlyCollection<User> GetMembers(string distinguishedName, string? memberSearchBase = null);
    }
}