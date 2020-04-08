namespace Miki.Discord.Common
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using Miki.Discord.Common.Packets;

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
        public string Status { get; set; }
    }

    [DataContract]
    public class DiscordStatus
    {
        [JsonPropertyName("since")]
        [DataMember(Name = "since", Order = 1)]
        public int? Since { get; set; }

        [JsonPropertyName("game")]
        [DataMember(Name = "game", Order = 2)]
        public DiscordActivity Game { get; set; }

        [JsonPropertyName("status")]
        [DataMember(Name = "status", Order = 3)]
        public string Status { get; set; }

        [JsonPropertyName("afk")]
        [DataMember(Name = "afk", Order = 4)]
        public bool IsAFK { get; set; }
    }

    [DataContract]
    public class DiscordActivity
    {
        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [DataMember(Name = "type", Order = 2)]
        public ActivityType Type { get; set; }

        [JsonPropertyName("url")]
        [DataMember(Name = "url", Order = 3)]
        public string Url { get; set; }

        [JsonPropertyName("timestamps")]
        [DataMember(Name = "timestamps", Order = 4)]
        public TimeStampsObject Timestamps { get; set; }

        [JsonPropertyName("application_id")]
        [DataMember(Name = "application_id", Order = 5)]
        public ulong? ApplicationId { get; set; }

        [JsonPropertyName("state")]
        [DataMember(Name = "state", Order = 6)]
        public string State { get; set; }

        [JsonPropertyName("details")]
        [DataMember(Name = "details", Order = 7)]
        public string Details { get; set; }

        [JsonPropertyName("party")]
        [DataMember(Name = "party", Order = 8)]
        public RichPresenceParty Party { get; set; }

        [JsonPropertyName("assets")]
        [DataMember(Name = "assets", Order = 9)]
        public RichPresenceAssets Assets { get; set; }
    }

    [DataContract]
    public class RichPresenceParty
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public string Id { get; set; }

        [JsonPropertyName("size")]
        [DataMember(Name = "size", Order = 2)]
        public int[] Size { get; set; }
    }

    [DataContract]
    public class RichPresenceAssets
    {
        [JsonPropertyName("large_image")]
        [DataMember(Name = "large_image", Order = 1)]
        public string LargeImage { get; set; }

        [JsonPropertyName("large_text")]
        [DataMember(Name = "large_text", Order = 2)]
        public string LargeText { get; set; }

        [JsonPropertyName("small_image")]
        [DataMember(Name = "small_image", Order = 3)]
        public string SmallImage { get; set; }

        [JsonPropertyName("small_text")]
        [DataMember(Name = "small_text", Order = 4)]
        public string SmallText { get; set; }
    }

    [DataContract]
    public class TimeStampsObject
    {
        [JsonPropertyName("start")]
        [DataMember(Name = "start", Order = 1)]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        [DataMember(Name = "end", Order = 2)]
        public long End { get; set; }
    }

    public enum UserStatus
    {
        ONLINE,
        IDLE,
        DND,
        OFFLINE
    }
}