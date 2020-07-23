using System.Runtime.Serialization;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common.Events
{
    [DataContract]
    public class GuildIdUserArgs
    {
        [DataMember(Name = "user")]
        public DiscordUserPacket User { get; set; }

        [DataMember(Name = "guild_id")]
        public ulong GuildId { get; set; }
    }
}