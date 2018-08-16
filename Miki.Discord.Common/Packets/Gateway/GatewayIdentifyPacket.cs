using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Gateway.Packets
{
	public class GatewayIdentifyPacket
	{
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("properties")]
		public GatewayIdentifyConnectionProperties ConnectionProperties;

		[JsonProperty("compress")]
		public bool Compressed;

		[JsonProperty("large_threshold")]
		public int LargeThreshold;

		[JsonProperty("presence")]
		public DiscordPresence Presence;
	}

	public class GatewayIdentifyConnectionProperties
	{
		[JsonProperty("$os")]
		public string OperatingSystem;

		[JsonProperty("$browser")]
		public string Browser;

		[JsonProperty("$device")]
		public string Device;
	}
}
