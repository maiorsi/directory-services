[![Build & Release](https://github.com/maiorsi/directory-services/actions/workflows/workflow.yml/badge.svg)](https://github.com/maiorsi/directory-services/actions/workflows/workflow.yml)

# Directory Services

DotNet Directory Services Library

This library provides a convenience wrapper of LDAP queries from C# including the following:
* Search by SID (objectSid - S-1-5...)
* Search By Guid (objectGUID)
* Active Directory Implementation (windows deployment/usage only as it relies on [System.DirectoryServices](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices?view=dotnet-plat-ext-7.0))
* [Novell LDAP Implementation](https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard) (cross platform)

## Installation

Use the DotNet CLI package manager [dotnet](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build the project.

```bash
dotnet restore
dotnet build
```

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

// OR
services.AddSingleton<IDirectoyService, ActiveDirectoryService>();

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
        var userBySid = _directoryService.SearchUserBySid("S-1-5....");
        var userByGuid = _directoryService.SearchUserByGuid("12345-aedgf234...");
    }
}
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
