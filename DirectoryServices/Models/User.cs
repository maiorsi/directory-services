// <copyright file="User.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

namespace DirectoryServices.Models
{
    public class User : LdapEntity
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public ActiveDirectoryUserAccountControl UserAccountControl { get; set; }
        public Dictionary<string, Group> Groups { get; set; } = new Dictionary<string, Group>();
    }
}