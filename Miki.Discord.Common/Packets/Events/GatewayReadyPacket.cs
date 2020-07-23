using Miki.Discord.Common.Packets;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common.Gateway
{
    [DataContract]
    public class GatewayReadyPacket
    {
        [JsonPropertyName("v")]
        [DataMember(Name = "v")]
        public int ProtocolVersion { get; set; }

        [JsonPropertyName("user")]
        [DataMember(Name = "user")]
        public DiscordUserPacket CurrentUser { get; set; }

        [JsonPropertyName("private_channels")]
        [DataMember(Name = "private_channels")]
        public DiscordChannelPacket[] PrivateChannels { get; set; }
        
        [JsonPropertyName("guilds")]
        [DataMember(Name = "guilds")]
        public DiscordGuildPacket[] Guilds { get; set; }

        [JsonPropertyName("session_id")]
        [DataMember(Name = "session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("_trace")]
        [DataMember(Name = "_trace")]
        public string[] TraceGuilds { get; set; }

        [JsonPropertyName("shard")]
        [DataMember(Name = "shard")]
        public int[] Shard { get; set; }

        public int CurrentShard
            => Shard[0];

        public int TotalShards
            => Shard[1];
    }
}