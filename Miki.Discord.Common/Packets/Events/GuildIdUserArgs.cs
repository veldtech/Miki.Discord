using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets
{
    public class GuildIdUserArgs
    {
		[JsonProperty("user")]
		public DiscordUserPacket user;

		[JsonProperty("guild_id")]
		public ulong guildId;
	}
}
