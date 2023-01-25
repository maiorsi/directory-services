// <copyright file="ActiveDirectoryHelper.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;

using DirectoryServices.Models;

using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("DirectoryServices.Tests")]
namespace DirectoryServices.Helpers
{
    public class ActiveDirectoryHelper
    {
        private readonly ILogger<ActiveDirectoryHelper> _logger;

        public ActiveDirectoryHelper(ILogger<ActiveDirectoryHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal User FromDirectoryEntry(DirectoryEntry directoryEntry, bool includeAncestors)
        {
            _logger.LogTrace("User instantiation - {name}", directoryEntry.Name);

            User user = new User();

            try
            {
                user.Sid = new SecurityIdentifier((byte[])(directoryEntry.Properties["objectSid"].Value ?? 0), 0).Value;
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing objectSid for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Guid = new Guid((byte[])(directoryEntry.Properties["objectSid"].Value ?? 0)).ToString();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing guid for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.DistinguishedName = (string?)(directoryEntry.Properties["distinguishedName"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing dn for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Username = (string?)(directoryEntry.Properties["sAMAccountName"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing username for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Email = (string?)(directoryEntry.Properties["mail"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing mail for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Name = (string?)(directoryEntry.Properties["name"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing name for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Phone = (string?)(directoryEntry.Properties["telephoneNumber"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing phone for user {user} {exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.UserAccountControl = (ActiveDirectoryUserAccountControl)(directoryEntry.Properties["userAccountControl"]?.Value ?? 0);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing account control for user {user} {exception}", directoryEntry.Name, exception);
            }

            if (includeAncestors)
            {
                try
                {
                    directoryEntry.RefreshCache(new string[] { "tokenGroups" });

                    var securityIdentifiers = ((object[]?)directoryEntry.Properties["tokenGroups"]?.Value).Select(_ =>
                    {
                        return new SecurityIdentifier(_ as byte[], 0);
                    });

                    user.Groups = securityIdentifiers.ToDictionary(_ => _.Value, _ => new Group { Sid = _.Value, Name = _.Translate(typeof(NTAccount)).ToString() });
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Exception occurred parsing token groups for user {user} {exception}", directoryEntry.Name, exception);
                }
            }

            return user;
        }

        internal Group FromDirectoryEntryGroup(DirectoryEntry directoryEntry)
        {
            _logger.LogTrace("Group instantiation - {name}", directoryEntry.Name);

            Group group = new Group();

            try
            {
                group.Sid = new SecurityIdentifier((byte[])(directoryEntry.Properties["objectSid"].Value ?? 0), 0).Value;
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing objectSid for group {group}{exception}", directoryEntry.Name, exception);
            }

            try
            {
                group.Guid = new Guid((byte[])(directoryEntry.Properties["objectSid"].Value ?? 0)).ToString();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing guid for group {group}{exception}", directoryEntry.Name, exception);
            }

            try
            {
                group.DistinguishedName = (string?)(directoryEntry.Properties["distinguishedName"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing dn for group {group}{exception}", directoryEntry.Name, exception);
            }

            try
            {
                group.Name = (string?)(directoryEntry.Properties["name"]?.Value);
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing name for group {group}{exception}", directoryEntry.Name, exception);
            }

            return group;
        }
    }
}