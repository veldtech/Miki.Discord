using Miki.Discord.Common.Packets;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    /// <summary>
    /// Discord Emoji payload.
    /// </summary>
    [DataContract]
    public class DiscordEmoji
    {
        /// <summary>
        /// Empty constructor for modern builder pattern.
        /// </summary>
        public DiscordEmoji() { }

        /// <summary>
        /// Creates an unicode emoji.
        /// <code>new DiscordEmoji("🚀");</code> 
        /// </summary>
        public DiscordEmoji(string unicode)
        {
            Name = unicode;
        }

        /// <summary>
        /// The Discord ID belonging to this emoji, if this value is set it means it's a custom Discord 
        /// emoji.
        /// </summary>
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong? Id { get; set; }

        /// <summary>
        /// Emoji name or unicode.
        /// </summary>
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

        /// <summary>
        /// Checks if the emoji is animated.
        /// </summary>
        [JsonPropertyName("animated")]
        [DataMember(Name = "animated", Order = 7)]
        public bool? Animated { get; set; }

        /// <summary>
        /// Parses an discord emoji from either a mention.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="emoji"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out DiscordEmoji emoji)
        {
            if(Mention.TryParse(text, out Mention mention))
            {
                if(mention.Type == MentionType.EMOJI
                    || mention.Type == MentionType.ANIMATED_EMOJI)
                {
                    emoji = new DiscordEmoji
                    {
                        Id = mention.Id,
                        Name = mention.Data,
                        Animated = mention.Type == MentionType.ANIMATED_EMOJI
                    };
                    return true;
                }
            }

            emoji = null;
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if(Id.HasValue)
            {
                return $"<{(Animated ?? false ? "a" : "")}:{Name}:{Id}>";
            }
            return Name;
        }
    }
}