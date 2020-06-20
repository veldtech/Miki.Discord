namespace Miki.Discord.Common.Gateway
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class GatewayHelloPacket
    {
        [JsonPropertyName("heartbeat_interval")]
        [DataMember(Name = "heartbeat_interval")]
        public int HeartbeatInterval { get; set; }

        [JsonPropertyName("_trace")]
        [DataMember(Name = "_trace")]
        public string[] TraceServers { get; set; }
    }
}