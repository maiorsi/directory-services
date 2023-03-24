using DirectoryServices.Impl;
using DirectoryServices.Interfaces;
using DirectoryServices.Settings;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Logging;

using Moq;

namespace DirectoryServices.Tests;

public class IntegrationTests : IAsyncLifetime, IDisposable
{
    private const ushort LDAP_PORT = 389;
    private const ushort HOST_LDAP_PORT = 10389;

    private const string USER_QUERY = "(cn=*)";
    private const string SAMPLE_USER_GUID = "18d0e2f5-deb8-4e0c-a483-1313d60863fc";

    private readonly CancellationTokenSource _cts = new(TimeSpan.FromMinutes(1));
    private readonly IContainer _ldapContainer;
    private readonly DirectorySettings _directorySettings = new DirectorySettings
    {
        BindDistinguishedName = "cn=admin,dc=planetexpress,dc=com",
        BindPassword = "GoodNewsEveryone",
        DefaultLdapUsersSearchBase = "ou=people,dc=planetexpress,dc=com",
        DefaultLdapGroupsSearchBase = "ou=people,dc=planetexpress,dc=com",
        DistinguishedNameEscapeCharactersString = "\\,:\\5C\\2C|\\*:\\5C\\2A|\\(:\\5C\\28|\\):\\5C\\29|\\\\:\\5C\\5C",
        LdapConnectionFallback = "ldap://127.0.0.1:389",
        LdapConnectionOverride = "",
        LdapConnectionServer = "127.0.0.1",
        LdapConnectionPort = 389,
        LdapFollowReferrals = true,
        LdapKerberosBind = false,
        LdapSecure = false,
        PageSize = -1,
        Threads = 4
    };

    public IntegrationTests()
    {
        _ldapContainer = new ContainerBuilder()
            .WithImage("ghcr.io/maiorsi/ldap-test-server:0.2")
            .WithImagePullPolicy((_) => true)
            .WithExposedPort(LDAP_PORT)
            .WithPortBinding(LDAP_PORT, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy(5))
            .Build();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _ldapContainer.StartAsync(_cts.Token).ConfigureAwait(false);
    }

    [Fact]
    public void TestSearchUsersByLdapQuery()
    {
        var hostPort = _ldapContainer.GetMappedPublicPort(LDAP_PORT);

        _directorySettings.LdapConnectionPort = hostPort;

        var mockLogger = Mock.Of<ILogger<NovellDirectoryService>>();

        var directoryService = new NovellDirectoryService(mockLogger, _directorySettings);

        var users = directoryService.SearchUsersByLdapQuery(USER_QUERY, 0, 10);

        // There are 9 users in the test ldap directory
        Assert.Equivalent(9, users.Count);
    }

    [Fact]
    public void SearchUserByGuid()
    {
        var hostPort = _ldapContainer.GetMappedPublicPort(LDAP_PORT);

        _directorySettings.LdapConnectionPort = hostPort;

        var mockLogger = Mock.Of<ILogger<NovellDirectoryService>>();

        var directoryService = new NovellDirectoryService(mockLogger, _directorySettings);

        var user = directoryService.SearchUserByGuid(SAMPLE_USER_GUID);

        Console.Out.WriteLine(user);
    }
}