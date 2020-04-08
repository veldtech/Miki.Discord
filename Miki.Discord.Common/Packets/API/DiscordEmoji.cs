namespace Miki.Discord.Common
{
    using Miki.Discord.Common.Packets;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    public class DiscordEmoji
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong? Id { get; set; }

        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 3)]
        public List<ulong> WhitelistedRoles { get; set; }

        [JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 4)]
        public DiscordUserPacket Creator { get; set; }

        [JsonPropertyName("require_colons")]
        [DataMember(Name = "require_colons", Order = 5)]
        public bool? RequireColons { get; set; }

        [JsonPropertyName("managed")]
        [DataMember(Name = "managed", Order = 6)]
        public bool? Managed { get; set; }

        [JsonPropertyName("animated")]
        [DataMember(Name = "animated", Order = 7)]
        public bool? Animated { get; set; }

        public static bool TryParse(string text, out DiscordEmoji emoji)
        {
            if(Mention.TryParse(text, out Mention mention))
            {
                if(mention.Type == MentionType.EMOJI)
                {
                    emoji = new DiscordEmoji
                    {
                        Id = mention.Id,
                        Name = mention.Data
                    };
                    return true;
                }
            }
            emoji = null;
            return false;
        }

        public override string ToString()
        {
            if(Id.HasValue)
            {
                return $"{Name}:{Id}";
            }
            return Name;
        }
    }
}