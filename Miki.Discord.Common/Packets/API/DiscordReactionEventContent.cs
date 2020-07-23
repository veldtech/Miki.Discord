using System.Runtime.Serialization;
using System.Text.Json.Serialization;

#nullable enable

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordReactionPacket
    {
        /// <summary>
        /// Id of the user reacting.
        /// </summary>
        [DataMember(Name = "user_id", Order = 1)]
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        
        /// <summary>
        /// Channel where the message was posted.
        /// </summary>
        [DataMember(Name = "channel_id", Order = 2)]
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        
        /// <summary>
        /// Message ID of the reaction added.
        /// </summary>
        [DataMember(Name = "message_id", Order = 3)]
        [JsonPropertyName("message_id")]
        public ulong MessageId { get; set; }
        
        /// <summary>
        /// Guild Id if the channel is in a guild.
        /// </summary>
        [DataMember(Name = "guild_id", Order = 4)]
        [JsonPropertyName("guild_id")]
        public ulong? GuildId { get; set; }
        
        /// <summary>
        /// Guild member if the message was in a guild.
        /// </summary>
        [DataMember(Name = "member", Order = 5)]
        [JsonPropertyName("member")]
        public DiscordGuildMemberPacket? Member { get; set; }
        
        /// <summary>
        /// The emoji which was reacted.
        /// </summary>
        [DataMember(Name = "emoji", Order = 6)]
        [JsonPropertyName("emoji")]
        public DiscordEmoji Emoji { get; set; }
    }
}