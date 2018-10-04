using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordChannel : ISnowflake
	{
		bool IsNsfw { get; }

		string Name { get; }

		Task DeleteAsync();
		Task ModifyAsync(object todo);
	}

	public interface IDiscordGuildChannel : IDiscordChannel
	{
		ulong GuildId { get; }

		ChannelType Type { get; }

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetUserAsync(ulong id);
		Task<IDiscordGuild> GetGuildAsync();
	}
}
