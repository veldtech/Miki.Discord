using Newtonsoft.Json;

namespace Miki.Discord.Common.Gateway
{
	public class GatewayConnectionPacket
	{
		[JsonProperty("url")]
		public string Url;

		[JsonProperty("shards")]
		public int ShardCount;

		[JsonProperty("session_start_limit")]
		public GatewaySessionLimitsPacket SessionLimit;
	}

	public class GatewaySessionLimitsPacket
	{
		[JsonProperty("total")]
		public int Total;

		[JsonProperty("remaining")]
		public int Remaining;

		[JsonProperty("reset_after")]
		public int ResetAfter;
	}
}