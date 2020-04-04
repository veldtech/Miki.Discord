namespace Miki.Discord.Common.Events
{
    using Miki.Discord.Common.Packets;
    using System.Runtime.Serialization;

    [DataContract]
    public class GuildMemberUpdateEventArgs
    {
        [DataMember(Name = "guild_id")]
        public ulong GuildId;

        [DataMember(Name = "roles")]
        public ulong[] RoleIds;

        [DataMember(Name = "user")]
        public DiscordUserPacket User;

        [DataMember(Name = "nick")]
        public string Nickname;
    }
}