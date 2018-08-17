using Miki.Discord.Common;
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
			=> await _client.DeleteMessageAsync(_packet.ChannelId, _packet.Id);

		public async Task<IDiscordChannel> GetChannelAsync()
			=> await _client.GetChannelAsync(_packet.ChannelId);
	}
}
