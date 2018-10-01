using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets.API
{
    public class DiscordReactionPacket
    {
		[JsonProperty("count")]
		public int Count;

		[JsonProperty("me")]
		public bool Me;

		[JsonProperty("emoji")]
		public DiscordEmojiPacket Emoji;
    }
}
