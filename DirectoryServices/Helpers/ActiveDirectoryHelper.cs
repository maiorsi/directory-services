using System;
using System.DirectoryServices;
using System.Security.Principal;

using DirectoryServices.Models;
using DirectoryServices.Settings;

using Microsoft.Extensions.Logging;

namespace DirectoryServices.Helpers
{
    public class ActiveDirectoryHelper
    {
        private readonly ILogger<ActiveDirectoryHelper> _logger;

        private readonly DirectorySettings _directorySettings;

        public ActiveDirectoryHelper(ILogger<ActiveDirectoryHelper> logger, DirectorySettings directorySettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _directorySettings = directorySettings;
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
                _logger.LogWarning("Exception occurred parsing objectSid for user {user}{exception}", directoryEntry.Name, exception);
            }

            try
            {
                user.Guid = new Guid((byte[])(directoryEntry.Properties["objectSid"].Value ?? 0)).ToString();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Exception occurred parsing guid for user {user}{exception}", directoryEntry.Name, exception);
            }

            return user;
        }
    }
}