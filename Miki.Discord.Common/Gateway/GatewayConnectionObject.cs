using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway
{
	public class GatewayConnectionPacket
	{
		[DataMember(Name = "url")]
		public string Url;

		[DataMember(Name = "shards")]
		public int ShardCount;

		[DataMember(Name = "session_start_limit")]
		public GatewaySessionLimitsPacket SessionLimit;
	}

	public class GatewaySessionLimitsPacket
	{
		[DataMember(Name = "total")]
		public int Total;

		[DataMember(Name = "remaining")]
		public int Remaining;

		[DataMember(Name = "reset_after")]
		public int ResetAfter;
	}
}