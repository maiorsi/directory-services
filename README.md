# directory-services

DotNet Directory Services Library

## Installation

Use the DotNet CLI package manager [dotnet](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build the project.

```bash
dotnet restore
dotnet build
```

## Usage

### App Settings
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

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
