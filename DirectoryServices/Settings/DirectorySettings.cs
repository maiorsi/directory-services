// <copyright file="DirectoryServiceSettings.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections.Generic;

namespace DirectoryServices.Settings
{
    public class DirectorySettings
    {
        public string? BindDistinguishedName { get; set; }
        public string? BindPassword { get; set; }
        public string? DefaultLdapUsersSearchBase { get; set; }
        public string? DefaultLdapGroupsSearchBase { get; set; }
        public string? DistinguishedNameEscapeCharactersString { get; set; }
        public Dictionary<string, string> DistinguishedNameEscapeCharacters
        {
            get
            {
                var dnEscapeCharacters = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(DistinguishedNameEscapeCharactersString))
                {
                    return dnEscapeCharacters;
                }

                var escapeCharacterPairs = DistinguishedNameEscapeCharactersString.Split("|");

                foreach (var escapeCharacterPair in escapeCharacterPairs)
                {
                    var escapeCharacterParts = escapeCharacterPair.Split(":");

                    if (escapeCharacterParts.Length == 2)
                    {
                        dnEscapeCharacters.Add(escapeCharacterParts[0], escapeCharacterParts[1]);
                    }
                }

                return dnEscapeCharacters;
            }
        }
        public string? LdapConnectionFallback { get; set; }
        public string? LdapConnectionOverride { get; set; }
        public string? LdapConnectionServer { get; set; }
        public int LdapConnectionPort { get; set; }
        public bool LdapFollowReferrals { get; set; }
        public bool LdapKerberosBind { get; set; }
        public bool LdapSecure { get; set; }
        public int PageSize { get; set; }
        public int Threads { get; set; }
    }
}
