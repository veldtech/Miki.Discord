namespace Miki.Discord.Common.Events
{
    using System.Runtime.Serialization;
    using Miki.Discord.Common.Packets;

    [DataContract]
    public class GuildIdUserArgs
    {
        [DataMember(Name = "user")]
        public DiscordUserPacket User { get; set; }

        [DataMember(Name = "guild_id")]
        public ulong GuildId { get; set; }
    }
}