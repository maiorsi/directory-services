// <copyright file="TestActiveDirectoryHelper.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections;
using System.Runtime.InteropServices;

using DirectoryServices.Helpers;
using DirectoryServices.Models;

using Microsoft.Extensions.Logging;

using Moq;

namespace DirectoryServices.Tests;

public class TestActiveDirectoryHelper
{
    [SkippableFact]
    public void TestFromDirectoryEntry()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

        var mockLogger = Mock.Of<ILogger<ActiveDirectoryHelper>>();
        var activeDirectoryHelper = new ActiveDirectoryHelper(mockLogger);

        var mockDirectoryEntry = Mock.Of<DirectoryEntryAdapter>();
        mockDirectoryEntry.Properties = new Hashtable
            {
                { "name", "Test User 1"},
                { "distinguishedName", "CN=Test\\ User\\ 1"},
                { "objectSid", "AQQAAAAAAAUVAAAAwdFKz9WmazDFb3Y8"},
                { "sAMAccountName", "testuser1"}
            };

        var expectedUser = new User
        {
            Name = "Test User 1",
            DistinguishedName = "CN=Test\\ User\\ 1",
            Sid = "S-1-5-21-3477787073-812361429-1014394821",
            Username = "testuser1"
        };

        var user = activeDirectoryHelper.FromDirectoryEntry(mockDirectoryEntry, false);

        Assert.Equivalent(expectedUser, user);
    }
}