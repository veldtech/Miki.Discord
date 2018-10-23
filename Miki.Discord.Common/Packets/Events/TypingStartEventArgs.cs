using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets.Events
{
	public class TypingStartEventArgs
	{
		[JsonProperty("channel_id")]
		public ulong channelId;

		[JsonProperty("guild_id")]
		public ulong guildId;

		[JsonProperty("member")]
		public DiscordGuildMemberPacket member;
	}
}
