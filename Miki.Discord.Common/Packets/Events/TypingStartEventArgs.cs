using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets.Events
{
    [DataContract]
    public class TypingStartEventArgs
    {
        [DataMember(Name = "channel_id")]
        public ulong channelId;

        [DataMember(Name = "guild_id")]
        public ulong guildId;

        [DataMember(Name = "member")]
        public DiscordGuildMemberPacket member;
    }
}