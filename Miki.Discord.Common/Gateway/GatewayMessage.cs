namespace Miki.Discord.Common.Gateway
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct GatewayMessage
    {
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode? OpCode;

        [DataMember(Name = "d", Order = 2)]
        public object Data;

        [DataMember(Name = "s", Order = 3)]
        public int? SequenceNumber;

        [DataMember(Name = "t", Order = 4)]
        public string EventName;
    }
}