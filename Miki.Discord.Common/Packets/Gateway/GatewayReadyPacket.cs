using Miki.Discord.Common.Packets;
using Newtonsoft.Json;

namespace Miki.Discord.Common.Gateway.Packets
{
	public class GatewayReadyPacket
	{
		[JsonProperty("v")]
		public int ProtocolVersion;

		[JsonProperty("user")]
		public DiscordUserPacket CurrentUser;

		[JsonProperty("private_channels")]
		public DiscordChannelPacket[] PrivateChannels;

		[JsonProperty("guilds")]
		public DiscordGuildPacket[] Guilds;

		[JsonProperty("session_id")]
		public string SessionId;

		[JsonProperty("_trace")]
		public string[] TraceGuilds;

		[JsonProperty("shard")]
		public int[] Shard;

		public int CurrentShard
			=> Shard[0];

		public int TotalShards
			=> Shard[1];
	}
}