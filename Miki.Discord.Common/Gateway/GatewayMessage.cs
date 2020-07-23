using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common.Gateway
{
    /// <summary>
    /// A message payload wrapping events received from the Discord gateway.
    /// </summary>
    [DataContract]
    public struct GatewayMessage
    {
        /// <summary>
        /// Gateway message type. Can be instructions for the gateway to follow, or events.
        /// </summary>
        [JsonPropertyName("op")]
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode? OpCode { get; set; }

        /// <summary>
        /// Data modelled for each <see cref="OpCode"/>.
        /// </summary>
        [JsonPropertyName("d")]
        [DataMember(Name = "d", Order = 2)]
        public object Data { get; set; }

        /// <summary>
        /// Sequence number, should increase linearly.
        /// </summary>
        [JsonPropertyName("s")]
        [DataMember(Name = "s", Order = 3)]
        public int? SequenceNumber { get; set; }

        /// <summary>
        /// If <see cref="OpCode"/> is <see cref="GatewayOpcode.Dispatch"/>, an event name is attached
        /// for the user to parse <see cref="Data"/> with.
        /// </summary>
        [JsonPropertyName("t")]
        [DataMember(Name = "t", Order = 4)]
        public string EventName { get; set; }
    }
}