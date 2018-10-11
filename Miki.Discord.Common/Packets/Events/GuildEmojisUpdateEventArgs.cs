using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets.Events
{
	public class GuildEmojisUpdateEventArgs
	{
		[JsonProperty("guild_id")]
		public ulong guildId;

		[JsonProperty("emojis")]
		public DiscordEmoji[] emojis;
	}
}
