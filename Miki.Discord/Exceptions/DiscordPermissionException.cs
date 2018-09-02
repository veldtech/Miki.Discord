using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Exceptions
{
	public class DiscordPermissionException : Exception
	{
		GuildPermission _permissions;

		public DiscordPermissionException(GuildPermission permissions)
			: base($"Could not perform actions as permission(s) {permissions} is required.")
		{

		}
		public DiscordPermissionException(string message)
			: base(message)
		{

		}
	}
}
