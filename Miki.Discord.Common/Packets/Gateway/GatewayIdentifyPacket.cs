using Miki.Discord.Common.Packets;
using Newtonsoft.Json;
using System;

namespace Miki.Discord.Common.Gateway.Packets
{
	public class GatewayIdentifyPacket
	{
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("properties")]
		public GatewayIdentifyConnectionProperties ConnectionProperties = new GatewayIdentifyConnectionProperties();

		[JsonProperty("compress")]
		public bool Compressed;

		[JsonProperty("large_threshold")]
		public int LargeThreshold;

		[JsonProperty("presence")]
		public DiscordStatus Presence;

		[JsonProperty("shard")]
		public int[] Shard;
	}

	public class GatewayIdentifyConnectionProperties
	{
		[JsonProperty("$os")]
		public string OperatingSystem = Environment.OSVersion.ToString();

		[JsonProperty("$browser")]
		public string Browser = "Miki.Discord";

		[JsonProperty("$device")]
		public string Device = "Miki.Discord";
	}
}