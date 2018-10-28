using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			if (id.Length == 0)
			{
				throw new ArgumentNullException();
			}

			if (id.Length < 2)
			{
				await _client._apiClient.DeleteMessageAsync(Id, id[0]);
			}

			if (id.Length > 100)
			{
				id = id.Take(100).ToArray();
			}

			await _client._apiClient.DeleteMessagesAsync(Id, id);
		}

		public async Task DeleteMessagesAsync(params IDiscordMessage[] messages)
		{
			await DeleteMessagesAsync(messages.Select(x => x.Id).ToArray());
		}

		public async Task<IDiscordMessage> GetMessageAsync(ulong id)
		{
			return new DiscordMessage(await _client._apiClient.GetMessageAsync(Id, id), _client);
		}

		public async Task<IEnumerable<IDiscordMessage>> GetMessagesAsync(int amount = 100)
		{
			return (await _client._apiClient.GetMessagesAsync(Id, amount))
				.Select(x => new DiscordMessage(x, _client));
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

		public async Task TriggerTypingAsync()
		{
			await _client._apiClient.TriggerTypingAsync(Id);
		}
	}
}