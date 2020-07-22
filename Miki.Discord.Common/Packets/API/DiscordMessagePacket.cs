using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common.Packets.API
{
    [Serializable]
    [DataContract]
    public class DiscordMessagePacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id")]
        public ulong Id { get; set; }

        [JsonPropertyName("channel_id")]
        [DataMember(Name = "channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id")]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("author")]
        [DataMember(Name = "author")]
        public DiscordUserPacket Author { get; set; }

        [JsonPropertyName("member")]
        [DataMember(Name = "member")]
        public DiscordGuildMemberPacket Member { get; set; }

        [JsonPropertyName("type")]
        [DataMember(Name = "type")]
        public DiscordMessageType Type { get; set; }

        [JsonPropertyName("content")]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [JsonPropertyName("timestamp")]
        [DataMember(Name = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("tts")]
        [DataMember(Name = "tts")]
        public bool IsTTS { get; set; }

        [JsonPropertyName("mention_everyone")]
        [DataMember(Name = "mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonPropertyName("mentions")]
        [DataMember(Name = "mentions")]
        public List<DiscordUserPacket> Mentions { get; set; }

        [JsonPropertyName("attachments")]
        [DataMember(Name = "attachments")]
        public List<DiscordAttachmentPacket> Attachments { get; set; }
    }

    /// <summary>
    /// Type of message received from the Discord API.
    /// </summary>
    public enum DiscordMessageType
    {
        /// <summary>
        /// Default text message from a user.
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// Recipent added to a DM group.
        /// </summary>
        RECIPIENT_ADD,
        /// <summary>
        /// Recipent removed from a DM group.
        /// </summary>
        RECIPIENT_REMOVE,
        /// <summary>
        /// Receiving a voice call request from another user.
        /// </summary>
        CALL,
        /// <summary>
        /// DM channel name change event
        /// </summary>
        CHANNEL_NAME_CHANGE,
        /// <summary>
        /// DM channel icon change event
        /// </summary>
        CHANNEL_ICON_CHANGE,
        /// <summary>
        /// Message has been pinned in a channel
        /// </summary>
        CHANNEL_PINNED_MESSAGE,
        /// <summary>
        /// A guild member joined, and the discord guild has default join events turned on.
        /// </summary>
        GUILD_MEMBER_JOIN,
        USER_PREMIUM_GUILD_SUBSCRIPTION,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_1,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_2,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_3
    }
}