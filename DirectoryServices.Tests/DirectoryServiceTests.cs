// <copyright file="DirectoryServiceTests.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;

using DirectoryServices.Helpers;
using DirectoryServices.Impl;
using DirectoryServices.Models;

using Microsoft.Extensions.Logging;

using Moq;

using Novell.Directory.Ldap;

namespace DirectoryServices.Tests;

public class DirectoryServiceTests
{
    private const string Name = "Test User 1";
    private const string DistinguishedName = "CN=Test\\ User\\ 1";
    private const string BinaryObjectSid = "AQQAAAAAAAUVAAAAwdFKz9WmazDFb3Y8";
    private const string BinaryObjectGuid = "9eLQGLjeDE6kgxMT1ghj+g==";
    private const string Username = "testuser1";
    private const string SidString = "S-1-5-21-3477787073-812361429-1014394821";
    private const string GuidString = "18d0e2f5-deb8-4e0c-a483-1313d60863fa";

    internal class DirectoryEntryAdapter : DirectoryEntry
    {
        public new IDictionary Properties { get; set; } = new Dictionary<string, object>();
    }

    [SkippableFact]
    public void TestFromDirectoryEntry()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

        var mockLogger = Mock.Of<ILogger<ActiveDirectoryService>>();
        var activeDirectoryHelper = new ActiveDirectoryHelper(mockLogger);

        var mockDirectoryEntry = Mock.Of<DirectoryEntryAdapter>();
        mockDirectoryEntry.Properties = GetProperties();

        var user = activeDirectoryHelper.FromDirectoryEntry(mockDirectoryEntry, false);

        Assert.Equivalent(GetExpectedUser(), user);
    }

    [Fact]
    public void TestFromLdapResult()
    {
        var mockLogger = Mock.Of<ILogger<LdapHelper>>();
        var ldapHelper = new LdapHelper(mockLogger);

        var mockLdapEntry = new Mock<LdapEntry>(DistinguishedName, GetAttributeSet()).Object;
        var mockLdapResult = Mock.Of<LdapResult>();

        mockLdapResult.LdapEntry = mockLdapEntry;

        var user = ldapHelper.FromLdapResult(mockLdapResult);

        Assert.Equivalent(GetExpectedUser(), user);
    }

    private User GetExpectedUser()
    {
        return new User
        {
            Name = Name,
            DistinguishedName = DistinguishedName,
            Sid = SidString,
            Guid = GuidString,
            Username = Username
        };
    }

    private Hashtable GetProperties()
    {
        return new Hashtable
            {
                { "name", Name},
                { "distinguishedName", DistinguishedName},
                { "objectSid", BinaryObjectSid},
                { "objectGUID", BinaryObjectGuid},
                { "sAMAccountName", Username}
            };
    }

    private LdapAttributeSet GetAttributeSet()
    {
        var attributeSet = new LdapAttributeSet();

        attributeSet.Add("name", new LdapAttribute("name", Name));
        attributeSet.Add("distinguishedName", new LdapAttribute("distinguishedName", DistinguishedName));
        attributeSet.Add("objectSid", new LdapAttribute("objectSid", Convert.FromBase64String(BinaryObjectSid)));
        attributeSet.Add("objectGUID", new LdapAttribute("objectGUID", Convert.FromBase64String(BinaryObjectGuid)));
        attributeSet.Add("sAMAccountName", new LdapAttribute("sAMAccountName", Username));

        return attributeSet;
    }
}