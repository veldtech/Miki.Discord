using Miki.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
	public interface IDiscordRole : ISnowflake<ulong>
	{
		string Name { get; }
		Color Color { get; }
		int Position { get; }
		GuildPermission Permissions { get; }
	}
}
