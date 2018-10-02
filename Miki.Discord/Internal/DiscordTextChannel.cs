using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordGuildTextChannel : DiscordGuildChannel, IDiscordGuildChannel, IDiscordTextChannel
	{
		public DiscordGuildTextChannel(DiscordChannelPacket packet, DiscordClient client)
			: base(packet, client)
		{
		}

		public async Task DeleteMessagesAsync(params ulong[] id)
		{
			if(id.Length == 1)
			{
				await _client.ApiClient.DeleteMessageAsync(Id, id[0]);
			}
			

			throw new NotImplementedException();
		}

		public Task<IDiscordMessage> GetMessageAsync(ulong id)
		{
			throw new NotImplementedException();
		}

		public Task<IDiscordMessage[]> GetMessagesAsync(ulong id, int amount = 100, GetMessageType type = GetMessageType.Before)
		{
			throw new NotImplementedException();
		}

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
			});
		}

		public Task TriggerTypingAsync()
		{
			throw new NotImplementedException();
		}
	}
}
