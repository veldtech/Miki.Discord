﻿using Miki.Discord.Common;
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

		public DiscordChannel()
		{
		}

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
