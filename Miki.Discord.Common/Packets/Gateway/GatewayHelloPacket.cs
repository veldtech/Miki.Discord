using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Gateway.Packets
{
    public class GatewayHelloPacket
    {
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval;

		[JsonProperty("_trace")]
		public string[] TraceServers;
    }
}
