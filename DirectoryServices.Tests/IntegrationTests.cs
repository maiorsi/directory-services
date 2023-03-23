using System.Text.Json;

using DirectoryServices.Impl;
using DirectoryServices.Settings;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace DirectoryServices.Tests;

public class IntegrationTests : IAsyncLifetime, IDisposable
{
    private const ushort LDAP_PORT = 389;
    private const ushort HOST_LDAP_PORT = 10389;

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
        LdapConnectionFallback = "ldap://127.0.0.1:389",
        LdapConnectionOverride = "",
        LdapConnectionUrl = "127.0.0.1",
        LdapFollowReferrals = true,
        LdapKerberosBind = false,
        LdapSecure = false,
        PageSize = 1000,
        Threads = 4
    };

    public IntegrationTests()
    {
        _ldapContainer = new ContainerBuilder()
            .WithImage("ghcr.io/maiorsi/ldap-test-server:0.2")
            .WithImagePullPolicy((_) => true)
            .WithExposedPort(LDAP_PORT)
            .WithPortBinding(LDAP_PORT, LDAP_PORT)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
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
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole(option => option.LogToStandardErrorThreshold = LogLevel.Trace))
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        var logger = factory?.CreateLogger<NovellDirectoryService>();

        var directoryService = new NovellDirectoryService(logger!, _directorySettings);

        var users = directoryService.SearchUsersByLdapQuery(USER_QUERY, 1, 10);

        Console.Out.WriteLine($"Users returned: {users.Count}");
    }
}