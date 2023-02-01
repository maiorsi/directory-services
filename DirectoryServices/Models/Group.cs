// <copyright file="Group.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

namespace DirectoryServices.Models
{
    public class Group : LdapEntity
    {
        public Dictionary<string?, Group> Ancestors { get; set; } = new Dictionary<string?, Group>();
        public Dictionary<string?, User> Members { get; set; } = new Dictionary<string?, User>();
    }
}