using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordGuildPacket : DiscordGuildPacketRoot
    {
        [DataMember(Name = "emojis", Order = 15)]
        public DiscordEmoji[] Emojis;

        [DataMember(Name = "roles", Order = 14)]
        public List<DiscordRolePacket> Roles = new List<DiscordRolePacket>();

        [DataMember(Name = "voice_states")]
		public List<DiscordVoiceStatePacket> VoiceStates;

        [DataMember(Name = "members")]
		public List<DiscordGuildMemberPacket> Members = new List<DiscordGuildMemberPacket>();

        [DataMember(Name = "channels")]
		public List<DiscordChannelPacket> Channels = new List<DiscordChannelPacket>();

        [DataMember(Name = "presences")]
		public List<DiscordPresencePacket> Presences = new List<DiscordPresencePacket>();
	}
}