using Newtonsoft.Json;

namespace Miki.Discord.Common.Gateway.Packets
{
	public class GatewayMessage
	{
		[JsonProperty("op")]
		public GatewayOpcode OpCode;

		[JsonProperty("d")]
		public object Data;

		[JsonProperty("s")]
		public int? SequenceNumber;

		[JsonProperty("t")]
		public string EventName;
	}
}