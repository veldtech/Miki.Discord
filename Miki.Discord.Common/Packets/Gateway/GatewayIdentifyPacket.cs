using Miki.Discord.Common.Packets;
using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway.Packets
{
    [DataContract]
    public class GatewayIdentifyPacket
    {
        [DataMember(Name = "token")]
        public string Token;

        [DataMember(Name = "properties")]
        public GatewayIdentifyConnectionProperties ConnectionProperties = new GatewayIdentifyConnectionProperties();

        [DataMember(Name = "compress")]
        public bool Compressed;

        [DataMember(Name = "large_threshold")]
        public int LargeThreshold;

        [DataMember(Name = "presence")]
        public DiscordStatus Presence;

        [DataMember(Name = "shard")]
        public int[] Shard;
    }

    [DataContract]
    public class GatewayIdentifyConnectionProperties
    {
        [DataMember(Name = "$os")]
        public string OperatingSystem = Environment.OSVersion.ToString();

        [DataMember(Name = "$browser")]
        public string Browser = "Miki.Discord";

        [DataMember(Name = "$device")]
        public string Device = "Miki.Discord";
    }
}