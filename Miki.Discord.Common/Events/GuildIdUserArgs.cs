using Miki.Discord.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Events
{
    public class GuildIdUserArgs
    {
		[JsonProperty("user")]
		public DiscordUserPacket user;

		[JsonProperty("guild_id")]
		public ulong guildId;
	}
}
