# directory-services
DotNet Directory Services Library

## Getting Started

### Usage

#### App Settings
```json
{
    "Ldap": {
        "BindDistinguishedName": "DN here",
        "BindPassword": "Password here",
        "DefaultLdapUsersSearchBase": "DN search base here",
        "DefaultLdapGroupsSearchBase": "DN search base here",
        "DistinguishedNameEscapeCharactersString": "\\,:\\5C\\2C|\\*:\\5C\\2A|\\(:\\5C\\28|\\):\\5C\\29|\\\\:\\5C\\5C",
        "Domain": "local",
        "LdapConnectionFallback": "LDAP://ldap.local.domain",
        "LdapConnectionOverride": "",
        "LdapConnectionUrl": "ldap.local.domain",
        "LdapFollowReferrals": true,
        "LdapKerberosBind": false,
        "LdapSecure": false
    }
}
```

#### Dependency Injection
```c#
// Part 1 - Add to services container
services.AddSingleton<IDirectoyService, NovellDirectoryService>();

// Part 2 - Use
public class UsersController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    public UsersController(IDirectoryService directoryService)
    {
        _directorService = directoryService ?? throw new ArgumentNullException(nameof(directorService));
    }

    private TestMethod()
    {
        var user = _directoryService.SearchUserBySid("S-1-5....");
    }
}
```