using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordRolePacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        [DataMember(Name = "color", Order = 3)]
        public int Color { get; set; }

        [JsonPropertyName("hoist")]
        [DataMember(Name = "hoist", Order = 4)]
        public bool IsHoisted { get; set; }

        [JsonPropertyName("position")]
        [DataMember(Name = "position", Order = 5)]
        public int Position { get; set; }

        [JsonPropertyName("permissions")]
        [DataMember(Name = "permissions", Order = 6)]
        public int Permissions { get; set; }

        [JsonPropertyName("managed")]
        [DataMember(Name = "managed", Order = 7)]
        public bool Managed { get; set; }

        [JsonPropertyName("mentionable")]
        [DataMember(Name = "mentionable", Order = 8)]
        public bool Mentionable { get; set; }
        
        public ulong GuildId { get; set; }
    }
}