using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Miki.Discord.Common
{
    [Serializable]
    [DataContract]
    public sealed class DiscordChannelPacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        [DataMember(Name = "type", Order = 2)]
        public ChannelType Type { get; set; }

        [JsonPropertyName("created_at")]
        [DataMember(Name = "created_at", Order = 3)]
        public long CreatedAt { get; set; }

        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 4)]
        public string Name { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 5)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("position")]
        [DataMember(Name = "position", Order = 6)]
        public int? Position { get; set; }

        [JsonPropertyName("permission_overwrites")]
        [DataMember(Name = "permission_overwrites", Order = 7)]
        public PermissionOverwrite[] PermissionOverwrites { get; set; }

        [JsonPropertyName("parent_id")]
        [DataMember(Name = "parent_id", Order = 8)]
        public ulong? ParentId { get; set; }

        [JsonPropertyName("nsfw")]
        [DataMember(Name = "nsfw", Order = 9)]
        public bool? IsNsfw { get; set; }

        [JsonPropertyName("topic")]
        [DataMember(Name = "topic", Order = 10)]
        public string Topic { get; set; }
    }

    public enum ChannelType
    {
        /// <summary>
        /// A text channel within a Discord server.
        /// </summary>
        GuildText = 0,

        /// <summary>
        /// A Direct Message channel with another user.
        /// </summary>
        DirectText,

        /// <summary>
        /// A voice channel.
        /// </summary>
        GuildVoice,

        /// <summary>
        /// A Group Direct Message channel with multiple users.
        /// </summary>
        GroupDirect,
        
        /// <summary>
        /// A server category
        /// </summary>
        GuildCategory,
        
        /// <summary>
        /// A news channel which allows users to cross-post their message.
        /// </summary>
        GuildNews,

        /// <summary>
        /// A game store channel to sell games on Discord.
        /// </summary>
        GuildStore
    }
}