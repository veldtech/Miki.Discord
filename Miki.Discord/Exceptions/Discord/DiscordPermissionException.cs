using Miki.Discord.Common;
using System;

namespace Miki.Discord.Exceptions
{
    public class DiscordPermissionException : Exception
    {
        public DiscordPermissionException(GuildPermission permissions)
            : base($"Could not perform actions as permission(s) {permissions} is required.")
        { }

        public DiscordPermissionException(string message)
            : base(message)
        { }
    }
}