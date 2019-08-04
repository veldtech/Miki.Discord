using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordMessage : IDiscordMessage
	{
		private readonly DiscordMessagePacket _packet;
		private readonly IDiscordClient _client;

		public DiscordMessage(DiscordMessagePacket packet, IDiscordClient client)
		{
			_packet = packet;
			if(_packet.GuildId != null
				&& _packet.Member != null)
			{
				_packet.Member.User = _packet.Author;
				_packet.Member.GuildId = _packet.GuildId.Value;
			}
			_client = client;
		}

		public IReadOnlyList<IDiscordAttachment> Attachments
			=> _packet.Attachments
				.Select(x => new DiscordAttachment(x))
				.ToList();

		public IDiscordUser Author
		{
			get
			{
				if(_packet.Member == null)
				{
					return new DiscordUser(_packet.Author, _client);
				}
				else
				{
					return new DiscordGuildUser(_packet.Member, _client);
				}
			}
		}

		public string Content
			=> _packet.Content;

		public ulong ChannelId
			=> _packet.ChannelId;

		public IReadOnlyList<ulong> MentionedUserIds
			=> _packet.Mentions.Select(x => x.Id)
				.ToList();

		public DateTimeOffset Timestamp
			=> _packet.Timestamp;

		public ulong Id
			=> _packet.Id;

		public DiscordMessageType Type
			=> _packet.Type;

		public async Task<IDiscordMessage> EditAsync(EditMessageArgs args)
			=> await _client.EditMessageAsync(ChannelId, Id, args.Content, args.Embed);

		public async Task DeleteAsync()
			=> await _client.ApiClient.DeleteMessageAsync(_packet.ChannelId, _packet.Id);

		public async Task<IDiscordTextChannel> GetChannelAsync()
		{
			var channel = await _client.GetChannelAsync(_packet.ChannelId, _packet.GuildId);
			return channel as IDiscordTextChannel;
		}

		public async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji)
			=> await _client.GetReactionsAsync(_packet.ChannelId, Id, emoji);

		public async Task CreateReactionAsync(DiscordEmoji emoji)
			=> await _client.ApiClient.CreateReactionAsync(ChannelId, Id, emoji);

		public async Task DeleteReactionAsync(DiscordEmoji emoji)
			=> await _client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji);

		public async Task DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user)
			=> await DeleteReactionAsync(emoji, user.Id);

		public async Task DeleteReactionAsync(DiscordEmoji emoji, ulong userId)
			=> await _client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji, userId);

		public async Task DeleteAllReactionsAsync()
			=> await _client.ApiClient.DeleteReactionsAsync(ChannelId, Id);
	}
}