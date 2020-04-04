namespace Miki.Discord.Common
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class DiscordChannelPacket
    {
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [DataMember(Name = "type", Order = 2)]
        public ChannelType Type { get; set; }

        [DataMember(Name = "created_at", Order = 3)]
        public long CreatedAt { get; set; }

        [DataMember(Name = "name", Order = 4)]
        public string Name { get; set; }

        [DataMember(Name = "guild_id", Order = 5)]
        public ulong? GuildId { get; set; }

        [DataMember(Name = "position", Order = 6)]
        public int? Position { get; set; }

        [DataMember(Name = "permission_overwrites", Order = 7)]
        public PermissionOverwrite[] PermissionOverwrites { get; set; }

        [DataMember(Name = "parent_id", Order = 8)]
        public ulong? ParentId { get; set; }

        [DataMember(Name = "nsfw", Order = 9)]
        public bool? IsNsfw { get; set; }

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