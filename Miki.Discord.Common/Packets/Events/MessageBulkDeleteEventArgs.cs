using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets.Events
{
	public class MessageBulkDeleteEventArgs
	{
		[JsonProperty("guild_id")]
		public ulong guildId;

		[JsonProperty("channel_id")]
		public ulong channelId;

		[JsonProperty("ids")]
		public ulong[] messagesDeleted;
	}
}
