using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Gateway.Packets
{
    public class GatewayMessage
    {
		[JsonProperty("op")]
		public GatewayOpcode OpCode;

		[JsonProperty("d")]
		public JToken Data;

		[JsonProperty("s")]
		public int? SequenceNumber;

		[JsonProperty("t")]
		public string EventName;
	}
}