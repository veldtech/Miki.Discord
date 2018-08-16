using Miki.Discord.Rest.Entities;
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

		Task<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null);
		Task<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content = null, bool isTTs = false, DiscordEmbed embed = null);
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
