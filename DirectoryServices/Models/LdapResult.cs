// <copyright file="LdapResult.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

using Novell.Directory.Ldap;

namespace DirectoryServices.Models
{
    public class LdapResult
    {
        public LdapEntry? LdapEntry { get; set; }
        public Dictionary<string, Group> Ancestors { get; set; } = new Dictionary<string, Group>();
        public Dictionary<string, User> Members { get; set; } = new Dictionary<string, User>();
    }
}