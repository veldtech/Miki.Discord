using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
	public interface IDiscordMessage : ISnowflake
	{
		string Content { get; }

		IDiscordUser Author { get; }

		ulong ChannelId { get; }

		Task CreateReactionAsync(DiscordEmoji emoji);

		Task DeleteReactionAsync(DiscordEmoji emoji);

		Task DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user);

		Task DeleteReactionAsync(DiscordEmoji emoji, ulong userId);

		Task DeleteAllReactionsAsync();

		Task<IDiscordMessage> EditAsync(EditMessageArgs args);

		Task DeleteAsync();

		Task<IDiscordTextChannel> GetChannelAsync();

		Task<IDiscordUser[]> GetReactionsAsync(DiscordEmoji emoji);

		IReadOnlyList<ulong> MentionedUserIds { get; }

		DateTimeOffset Timestamp { get; }
	}
}