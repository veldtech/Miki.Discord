using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordMessage : ISnowflake
    {
		string Content { get; }

		IDiscordUser Author { get; }

		ulong ChannelId { get; }

		Task<IDiscordMessage> EditAsync(EditMessageArgs args);

		Task DeleteAsync();

		Task<IDiscordChannel> GetChannelAsync();

		IReadOnlyList<ulong> MentionedUserIds { get; }

		DateTimeOffset Timestamp { get; }
	}
}
