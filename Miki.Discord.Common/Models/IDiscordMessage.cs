using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
	public interface IDiscordMessage : ISnowflake
	{
		/// <summary>
		/// All attachments attached to this message.
		/// </summary>
		IReadOnlyList<IDiscordAttachment> Attachments { get; }

		/// <summary>
		/// The creator of the message.
		/// </summary>
		IDiscordUser Author { get; }

		string Content { get; }

		ulong? GuildId { get; }

		/// <summary>
		/// The channel this message was created in.
		/// </summary>
		ulong ChannelId { get; }

		IReadOnlyList<ulong> MentionedUserIds { get; }

		DateTimeOffset Timestamp { get; }

		MessageType Type { get; }

		Task CreateReactionAsync(DiscordEmoji emoji);

		Task DeleteReactionAsync(DiscordEmoji emoji);

		Task DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user);

		Task DeleteReactionAsync(DiscordEmoji emoji, ulong userId);

		Task DeleteAllReactionsAsync();

		Task<IDiscordMessage> EditAsync(EditMessageArgs args);

		/// <summary>
		/// Deletes this message.
		/// </summary>
		Task DeleteAsync();

		Task<IDiscordTextChannel> GetChannelAsync();

		Task<IReadOnlyList<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji);
	}
}