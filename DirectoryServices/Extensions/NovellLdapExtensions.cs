// <copyright file="NovellLdapExtensions.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Controls;

namespace DirectoryServices.Extensions
{
    public static class NovellLdapExtensions
    {
        public static LdapSearchConstraints AddPagination(this LdapSearchConstraints constraints, int page, int pageSize)
        {
            var start = (page - 1) * pageSize;

            start++;

            var before = 0;
            var after = pageSize - 1;

            // 0 means we do not know the total
            var count = 0;

            var lvlc = new LdapVirtualListControl(start, before, after, count);
            var sc = new LdapSortControl(new LdapSortKey("cn"), true);

            constraints.SetControls(new LdapControl[] { lvlc, sc });

            return constraints;
        }
    }
}