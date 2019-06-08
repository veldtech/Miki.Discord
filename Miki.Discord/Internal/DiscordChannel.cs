using Miki.Discord.Common;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
	public class DiscordChannel : IDiscordChannel
	{
		protected DiscordChannelPacket _packet;
		protected IDiscordClient _client;

		public DiscordChannel()
		{
		}

		public DiscordChannel(DiscordChannelPacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

		public bool IsNsfw
			=> _packet?.IsNsfw
                .GetValueOrDefault(false) 
                    ?? false;

		public async Task DeleteAsync()
		{
			await _client.ApiClient.DeleteChannelAsync(Id);
		}

		public Task ModifyAsync(object todo)
		{
			throw new NotImplementedException();
		}
	}
}