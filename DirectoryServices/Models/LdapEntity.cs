// <copyright file="LdapEntity.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

namespace DirectoryServices.Models
{
    public abstract class LdapEntity
    {
        public string? Sid { get; set; }
        public string? Guid { get; set; }
        public string? DistinguishedName { get; set; }
        public string? Name { get; set; }
    }
}