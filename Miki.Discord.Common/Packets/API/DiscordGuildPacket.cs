using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
    /// <summary>
    /// The root of the Guild Packet. This class is used internally and should not be used. Use
    /// <see cref="DiscordGuildPacket"/> instead.
    /// </summary>
    [DataContract]
    public class DiscordGuildPacket
    {
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        [DataMember(Name = "icon", Order = 3)]
        public string Icon { get; set; }

        [JsonPropertyName("splash")]
        [DataMember(Name = "splash", Order = 4)]
        public string Splash { get; set; }

        [JsonPropertyName("owner_id")]
        [DataMember(Name = "owner_id", Order = 5)]
        public ulong OwnerId { get; set; }

        [JsonPropertyName("region")]
        [DataMember(Name = "region", Order = 6)]
        public string Region { get; set; }

        [JsonPropertyName("afk_channel_id")]
        [DataMember(Name = "afk_channel_id", Order = 7)]
        public ulong? AfkChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        [DataMember(Name = "afk_timeout", Order = 8)]
        public int? AfkTimeout { get; set; }

        [JsonPropertyName("embed_enabled")]
        [DataMember(Name = "embed_enabled", Order = 9)]
        public bool EmbedEnabled { get; set; }

        [JsonPropertyName("embed_channel_id")]
        [DataMember(Name = "embed_channel_id", Order = 10)]
        public ulong? EmbedChannelId { get; set; }

        [JsonPropertyName("verification_level")]
        [DataMember(Name = "verification_level", Order = 11)]
        public int VerificationLevel { get; set; }

        [JsonPropertyName("default_message_notifications")]
        [DataMember(Name = "default_message_notifications", Order = 12)]
        public int DefaultMessageNotifications { get; set; }

        [JsonPropertyName("explicit_content_filter")]
        [DataMember(Name = "explicit_content_filter", Order = 13)]
        public int ExplicitContentFilter { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 14)]
        public List<DiscordRolePacket> Roles { get; set; } 
            = new List<DiscordRolePacket>();

        [JsonPropertyName("emojis")]
        [DataMember(Name = "emojis", Order = 15)]
        public DiscordEmoji[] Emojis { get; set; }

        [JsonPropertyName("features")]
        [DataMember(Name = "features", Order = 16)]
        public List<string> Features { get; set; }

        [JsonPropertyName("mfa_level")]
        [DataMember(Name = "mfa_level", Order = 17)]
        public int MFALevel { get; set; }

        [JsonPropertyName("application_id")]
        [DataMember(Name = "application_id", Order = 18)]
        public ulong? ApplicationId { get; set; }

        [JsonPropertyName("widget_enabled")]
        [DataMember(Name = "widget_enabled", Order = 19)]
        public bool? WidgetEnabled { get; set; }

        [JsonPropertyName("widget_channel_id")]
        [DataMember(Name = "widget_channel_id", Order = 20)]
        public ulong? WidgetChannelId { get; set; }

        [JsonPropertyName("system_channel_id")]
        [DataMember(Name = "system_channel_id", Order = 21)]
        public ulong? SystemChannelId { get; set; }

        [JsonPropertyName("joined_at")]
        [DataMember(Name = "joined_at", Order = 22)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("large")]
        [DataMember(Name = "large", Order = 23)]
        public bool? IsLarge { get; set; }

        [JsonPropertyName("unavailable")]
        [DataMember(Name = "unavailable", Order = 24)]
        public bool? Unavailable { get; set; }

        [JsonPropertyName("member_count")]
        [DataMember(Name = "member_count", Order = 25)]
        public int? MemberCount { get; set; }

        [JsonPropertyName("owner")]
        [DataMember(Name = "owner", Order = 26)]
        public bool? IsOwner { get; set; }

        [JsonPropertyName("permissions")]
        [DataMember(Name = "permissions", Order = 27)]
        public int? Permissions { get; set; }

        [JsonPropertyName("premium_tier")]
        [DataMember(Name = "premium_tier", Order = 28)]
        public int? PremiumTier { get; set; }

        [JsonPropertyName("premium_subscription_count")]
        [DataMember(Name = "premium_subscription_count", Order = 29)]
        public int? PremiumSubscriberCount { get; set; }

        [JsonPropertyName("voice_states")]
        [DataMember(Name = "voice_states")]
        public List<DiscordVoiceStatePacket> VoiceStates { get; set; }

        [JsonPropertyName("members")]
        [DataMember(Name = "members")]
        public List<DiscordGuildMemberPacket> Members { get; set; } 
            = new List<DiscordGuildMemberPacket>();

        [JsonPropertyName("channels")]
        [DataMember(Name = "channels")]
        public List<DiscordChannelPacket> Channels { get; set; } 
            = new List<DiscordChannelPacket>();

        [JsonPropertyName("presences")]
        [DataMember(Name = "presences")]
        public List<DiscordPresencePacket> Presences { get; set; } 
            = new List<DiscordPresencePacket>();

        public void OverwriteContext(DiscordGuildPacket guild)
        {
            Name = guild.Name;
            Icon = guild.Icon;
            Splash = guild.Splash;
            OwnerId = guild.OwnerId;
            Region = guild.Region;
            AfkChannelId = guild.AfkChannelId;
            AfkTimeout = guild.AfkTimeout;
            Permissions = guild.Permissions;
            EmbedEnabled = guild.EmbedEnabled;
            EmbedChannelId = guild.EmbedChannelId;
            VerificationLevel = guild.VerificationLevel;
            DefaultMessageNotifications = guild.DefaultMessageNotifications;
            ExplicitContentFilter = guild.ExplicitContentFilter;
            MFALevel = guild.MFALevel;
            ApplicationId = guild.ApplicationId;
            WidgetEnabled = guild.WidgetEnabled;
            WidgetChannelId = guild.WidgetChannelId;
            SystemChannelId = guild.SystemChannelId;
            PremiumTier = guild.PremiumTier;
            PremiumSubscriberCount = guild.PremiumSubscriberCount;
        }
    }
}