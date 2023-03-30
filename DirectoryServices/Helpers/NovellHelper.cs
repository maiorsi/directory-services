// <copyright file="NovellHelper.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;
using System.Runtime.CompilerServices;

using DirectoryServices.Impl;
using DirectoryServices.Models;

using Microsoft.Extensions.Logging;

using Novell.Directory.Ldap;

[assembly: InternalsVisibleTo("DirectoryServices.Tests")]
namespace DirectoryServices.Helpers
{
    public class NovellHelper
    {
        private readonly ILogger<NovellDirectoryService> _logger;

        public NovellHelper(ILogger<NovellDirectoryService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal User FromLdapEntry(LdapEntry ldapEntry)
        {
            _logger.LogTrace("User instantiation - {name}", ldapEntry.Dn);

            User user = new User();

            var attributes = ldapEntry?.GetAttributeSet();

            if (attributes != null)
            {
                try
                {
                    if (attributes.ContainsKey("objectSid"))
                    {
                        user.Sid = SecureIdentifierHelper.Decode(attributes["objectSid"]?.ByteValue ?? new byte[0]);
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find objectSid attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing objectSid for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("objectGUID"))
                    {
                        user.Guid = new Guid(attributes["objectGUID"]?.ByteValue ?? new byte[0]).ToString();
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find objectGUID attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing objectGUID for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("sAMAccountName"))
                    {
                        user.Username = attributes["sAMAccountName"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find sAMAccountName attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing sAMAccountName for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("uid"))
                    {
                        user.Uid = attributes["uid"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find uid attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing uid for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("mail"))
                    {
                        user.Email = attributes["mail"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find mail attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing mail for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("distinguishedName"))
                    {
                        user.DistinguishedName = attributes["distinguishedName"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find distinguishedName attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing distinguishedName for user {user} {exception}", ldapEntry?.Dn, exception);
                }


                try
                {
                    if (attributes.ContainsKey("displayName"))
                    {
                        user.DisplayName = attributes["displayName"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find displayName attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing displayName for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("givenName"))
                    {
                        user.FirstName = attributes["givenName"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find givenName attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing givenName for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("sn"))
                    {
                        user.LastName = attributes["sn"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find sn attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing sn for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("name"))
                    {
                        user.Name = attributes["name"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find name attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing name for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("telephoneNumber"))
                    {
                        user.Phone = attributes["telephoneNumber"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find telephoneNumber attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing telephoneNumber for user {user} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("userAccountControl"))
                    {
                        if (int.TryParse(attributes["userAccountControl"]?.StringValue, out int userAccountControl))
                        {
                            user.UserAccountControl = (ActiveDirectoryUserAccountControl)userAccountControl;
                        }
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find userAccountControl attribute for user {user}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing userAccountControl for user {user} {exception}", ldapEntry?.Dn, exception);
                }
            }

            return user;
        }

        internal Group FromLdapResultGroup(LdapEntry ldapEntry)
        {
            _logger.LogTrace("Group instantiation - {name}", ldapEntry?.Dn);

            Group group = new Group();

            var attributes = ldapEntry?.GetAttributeSet();

            if (attributes != null)
            {
                try
                {
                    if (attributes.ContainsKey("objectSid"))
                    {
                        group.Sid = SecureIdentifierHelper.Decode(attributes["objectSid"]?.ByteValue ?? new byte[0]);
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find objectSid attribute for group {group}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing objectSid for group {group} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("objectGUID"))
                    {
                        group.Guid = new Guid(attributes["objectGUID"]?.ByteValue ?? new byte[0]).ToString();
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find objectGUID attribute for group {group}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing objectGUID for group {group} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("distinguishedName"))
                    {
                        group.DistinguishedName = attributes["distinguishedName"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find distinguishedName attribute for group {group}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing distinguishedName for group {group} {exception}", ldapEntry?.Dn, exception);
                }

                try
                {
                    if (attributes.ContainsKey("name"))
                    {
                        group.Name = attributes["name"]?.StringValue;
                    }
                    else
                    {
                        _logger.LogDebug("Failed to find name attribute for group {group}", ldapEntry?.Dn);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing name for group {group} {exception}", ldapEntry?.Dn, exception);
                }
            }

            return group;
        }
    }
}