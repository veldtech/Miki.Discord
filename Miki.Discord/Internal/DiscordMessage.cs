﻿using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordMessage : IDiscordMessage
    {
		private DiscordMessagePacket _packet;
		private DiscordClient _client;

		public DiscordMessage(DiscordMessagePacket packet, DiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public IDiscordUser Author
			=> new DiscordUser(_packet.Author, _client);

		public string Content
			=> _packet.Content;

		public ulong ChannelId
			=> _packet.ChannelId;

		public IReadOnlyList<ulong> MentionedUserIds
			=> _packet.Mentions.Select(x => x.Id).ToList();

		public DateTimeOffset Timestamp
			=> _packet.Timestamp;

		public ulong Id
			=> _packet.Id;

		public async Task<IDiscordMessage> EditAsync(EditMessageArgs args)
			=> await _client.EditMessageAsync(ChannelId, Id, args.content, args.embed);

		public async Task DeleteAsync()
			=> await _client.ApiClient.DeleteMessageAsync(_packet.ChannelId, _packet.Id);

		public async Task<IDiscordTextChannel> GetChannelAsync()
			=> (await _client.GetChannelAsync(_packet.ChannelId, _packet.GuildId)) as IDiscordTextChannel;

		public async Task<IDiscordUser[]> GetReactionsAsync(DiscordEmoji emoji)
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
