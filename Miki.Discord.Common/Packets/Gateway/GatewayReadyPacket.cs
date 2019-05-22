using Miki.Discord.Common.Packets;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Gateway.Packets
{
    [DataContract]
    public class GatewayReadyPacket
	{
        [DataMember(Name = "v")]
        public int ProtocolVersion;

        [DataMember(Name = "user")]
        public DiscordUserPacket CurrentUser;

        [DataMember(Name = "private_channels")]
        public DiscordChannelPacket[] PrivateChannels;

        [DataMember(Name = "guilds")]
        public DiscordGuildPacket[] Guilds;

        [DataMember(Name = "session_id")]
        public string SessionId;

        [DataMember(Name = "_trace")]
        public string[] TraceGuilds;

        [DataMember(Name = "shard")]
        public int[] Shard;

		public int CurrentShard
			=> Shard[0];

		public int TotalShards
			=> Shard[1];
	}
}