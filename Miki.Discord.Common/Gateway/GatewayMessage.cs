using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway.Packets
{
    [DataContract]
	public struct GatewayMessage
	{
        [DataMember(Name = "op")]
        public GatewayOpcode? OpCode;

        [DataMember(Name = "d")]
		public object Data;

        [DataMember(Name = "s")]
        public int? SequenceNumber;

        [DataMember(Name = "t")]
        public string EventName;
	}
}