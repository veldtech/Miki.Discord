using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets
{
    public class DiscordGuildUnavailablePacket
    {
		[JsonProperty("id")]
		public ulong GuildId;

		[JsonProperty("unavailable")]
		public bool IsUnavailable;
    }
}
