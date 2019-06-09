using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway.Packets
{
    public interface IGatewayMessage
    {
        GatewayOpcode OpCode { get; }

        int? SequenceNumber { get; }

        string EventName { get; }

        object Data { get; }
    }

    [DataContract]
    public struct GatewayMessage : IGatewayMessage
    {
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode OpCode;

        GatewayOpcode IGatewayMessage.OpCode => OpCode;

        int? IGatewayMessage.SequenceNumber => null;

        string IGatewayMessage.EventName => null;

        object IGatewayMessage.Data => null;
    }

    [DataContract]
	public struct GatewayMessage<T> : IGatewayMessage
    {
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode OpCode;

        [DataMember(Name = "t", Order = 2)]
        public string EventName;

        [DataMember(Name = "s", Order = 3)]
        public int? SequenceNumber;

        [DataMember(Name = "d", Order = 4)]
        public T Data;

        GatewayOpcode IGatewayMessage.OpCode => OpCode;

        int? IGatewayMessage.SequenceNumber => SequenceNumber;

        string IGatewayMessage.EventName => EventName;

        object IGatewayMessage.Data => Data;
    }
}