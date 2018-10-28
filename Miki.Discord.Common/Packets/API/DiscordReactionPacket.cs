using Newtonsoft.Json;

namespace Miki.Discord.Common
{
	public class DiscordReactionPacket
	{
		[JsonProperty("count")]
		public int Count;

		[JsonProperty("me")]
		public bool Me;

		[JsonProperty("emoji")]
		public DiscordEmoji Emoji;
	}
}