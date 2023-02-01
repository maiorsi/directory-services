// <copyright file="DirectoryEntryAdapter.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections;
using System.DirectoryServices;

namespace DirectoryServices.Tests
{
    public class DirectoryEntryAdapter : DirectoryEntry
    {
        public new IDictionary Properties { get; set; } = new Dictionary<string, object>();
    }
}