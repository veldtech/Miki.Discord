using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordPresencePacket
    {
        [JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 1)]
        public DiscordUserPacket User { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 2)]
        public List<ulong> RoleIds { get; set; }

        [JsonPropertyName("game")]
        [DataMember(Name = "game", Order = 3)]
        public DiscordActivity Game { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 4)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("status")]
        [DataMember(Name = "status", Order = 5)]
        public UserStatus Status { get; set; }
    }
}