using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway.Packets
{
	[DataContract]
	public class GatewayHelloPacket
	{
		[DataMember(Name = "heartbeat_interval")]
		public int HeartbeatInterval;

		[DataMember(Name = "_trace")]
		public string[] TraceServers;
	}
}