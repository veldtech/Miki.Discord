using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordTextChannel : IDiscordChannel
	{
		Task DeleteMessagesAsync(params ulong[] id);

		Task<IDiscordMessage[]> GetMessagesAsync(ulong id, int amount = 100, GetMessageType type = GetMessageType.Before);
		Task<IDiscordMessage> GetMessageAsync(ulong id);

		Task<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null);
		Task<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content = null, bool isTTs = false, DiscordEmbed embed = null);

		Task TriggerTypingAsync();
	}

	public enum GetMessageType
	{
		Around, Before, After
	}
}
