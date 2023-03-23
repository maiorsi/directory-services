using DirectoryServices.Impl;
using DirectoryServices.Settings;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Logging;

using Moq;

namespace DirectoryServices.Tests;

public class IntegrationTests : IAsyncLifetime, IDisposable
{
    private const ushort LDAP_PORT = 389;
    private const string USER_QUERY = "(cn=*)";

    private readonly CancellationTokenSource _cts = new(TimeSpan.FromMinutes(1));
    private readonly IContainer _ldapContainer;
    private readonly DirectorySettings _directorySettings = new DirectorySettings
    {
        BindDistinguishedName = "cn=admin,dc=planetexpress,dc=com",
        BindPassword = "GoodNewsEveryone",
        DefaultLdapUsersSearchBase = "ou=people,dc=planetexpress,dc=com",
        DefaultLdapGroupsSearchBase = "ou=people,dc=planetexpress,dc=com",
        DistinguishedNameEscapeCharactersString = "\\,:\\5C\\2C|\\*:\\5C\\2A|\\(:\\5C\\28|\\):\\5C\\29|\\\\:\\5C\\5C",
        LdapConnectionFallback = "LDAP://localhost:389",
        LdapConnectionOverride = "",
        LdapConnectionUrl = "localhost:389",
        LdapFollowReferrals = true,
        LdapKerberosBind = false,
        LdapSecure = false,
        PageSize = 1000,
        Threads = 4
    };

    public IntegrationTests()
    {
        _ldapContainer = new ContainerBuilder()
            .WithImage("ghcr.io/maiorsi/ldap-test-server:0.2.0")
            .WithPortBinding(LDAP_PORT, LDAP_PORT)
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
        var mockLogger = Mock.Of<ILogger<NovellDirectoryService>>();
        var directoryService = new NovellDirectoryService(mockLogger, _directorySettings);

        var users = directoryService.SearchUsersByLdapQuery(USER_QUERY, 1, 10);

        Console.Out.WriteLine(users.Count);
    }
}