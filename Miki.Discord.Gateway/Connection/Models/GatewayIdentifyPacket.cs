
namespace Miki.Discord.Common.Gateway
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class GatewayIdentifyPacket
    {
        [JsonPropertyName("token")]
        [DataMember(Name = "token")]
        public string Token { get; set; }

        [JsonPropertyName("properties")]
        [DataMember(Name = "properties")]
        public GatewayIdentifyConnectionProperties ConnectionProperties { get; set; } 
            = new GatewayIdentifyConnectionProperties();

        [JsonPropertyName("compress")]
        [DataMember(Name = "compress")] 
        public bool Compressed { get; set; }

        [JsonPropertyName("large_threshold")]
        [DataMember(Name = "large_threshold")] 
        public int LargeThreshold { get; set; }

        [JsonPropertyName("presence")]
        [DataMember(Name = "presence")] 
        public DiscordStatus Presence { get; set; }
        
        [JsonPropertyName("shard")]
        [DataMember(Name = "shard")] 
        public int[] Shard { get; set; }

        [JsonPropertyName("intents")]
        [DataMember(Name = "shard")]
        public int Intent { get; set; }
    }

    [DataContract]
    public class GatewayIdentifyConnectionProperties
    {
        [JsonPropertyName("$os")]
        [DataMember(Name = "$os")] 
        public string OperatingSystem { get; set; } 
            = Environment.OSVersion.ToString();

        [JsonPropertyName("$browser")]
        [DataMember(Name = "$browser")] 
        public string Browser { get; set; } = "Miki.Discord";

        [JsonPropertyName("$device")]
        [DataMember(Name = "$device")] 
        public string Device { get; set; } = "Miki.Discord";
    }
}