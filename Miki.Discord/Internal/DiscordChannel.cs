using Miki.Discord.Common;
using Miki.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordChannel : IDiscordChannel
    {
		protected DiscordChannelPacket _packet;
		protected DiscordClient _client;

		public DiscordChannel(DiscordChannelPacket packet, DiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

		public bool IsNsfw
			=> _packet.IsNsfw.Value;

		public async Task<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content, bool isTTS = false, DiscordEmbed embed = null)
			=> await _client.SendFileAsync(Id, file, fileName, new MessageArgs
			{
				content = content,
				tts = isTTS,
				embed = embed
			});

		public async Task<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
		{
			return await _client.SendMessageAsync(Id, new MessageArgs()
			{
				content = content,
				tts = isTTS,
				embed = embed
			}, true);
		}
	}
}
