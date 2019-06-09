using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    /// <summary>
    /// The root of the Guild Packet.
    /// This class is used internally and should not be used. Use <see cref="DiscordGuildPacket"/> instead.
    /// </summary>
    [DataContract]
    public class DiscordGuildPacketRoot
    {
        [DataMember(Name = "id", Order = 1)]
        public ulong Id;

        [DataMember(Name = "name", Order = 2)]
        public string Name;

        [DataMember(Name = "icon", Order = 3)]
        public string Icon;

        [DataMember(Name = "splash", Order = 4)]
        public string Splash;

        [DataMember(Name = "owner_id", Order = 5)]
        public ulong OwnerId;

        [DataMember(Name = "region", Order = 6)]
        public string Region;

        [DataMember(Name = "afk_channel_id", Order = 7)]
        public ulong? AfkChannelId;

        [DataMember(Name = "afk_timeout", Order = 8)]
        public int? AfkTimeout;

        [DataMember(Name = "embed_enabled", Order = 9)]
        public bool EmbedEnabled;

        [DataMember(Name = "embed_channel_id", Order = 10)]
        public ulong? EmbedChannelId;

        [DataMember(Name = "verification_level", Order = 11)]
        public int VerificationLevel;

        [DataMember(Name = "default_message_notifications", Order = 12)]
        public int DefaultMessageNotifications;

        [DataMember(Name = "explicit_content_filter", Order = 13)]
        public int ExplicitContentFilter;

        [DataMember(Name = "features", Order = 16)]
        public List<string> Features;

        [DataMember(Name = "mfa_level", Order = 17)]
        public int MFALevel;

        [DataMember(Name = "application_id", Order = 18)]
        public ulong? ApplicationId;

        [DataMember(Name = "widget_enabled", Order = 19)]
        public bool? WidgetEnabled;

        [DataMember(Name = "widget_channel_id", Order = 20)]
        public ulong? WidgetChannelId;

        [DataMember(Name = "system_channel_id", Order = 21)]
        public ulong? SystemChannelId;

        public long CreatedAt;

        [DataMember(Name = "joined_at", Order = 22)]
        internal string _createdAt
        {
            get
            {
                return new DateTime(CreatedAt).ToString("o");
            }

            set
            {
                var d = DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
                CreatedAt = d.Ticks;
            }
        }

        [DataMember(Name = "large", Order = 23)]
        public bool? IsLarge;

        [DataMember(Name = "unavailable", Order = 24)]
        public bool? Unavailable;

        [DataMember(Name = "member_count", Order = 25)]
        public int? MemberCount;

        [DataMember(Name = "owner", Order = 26)]
        public bool? IsOwner;

        [DataMember(Name = "permissions", Order = 27)]
        public int? Permissions;

        [DataMember(Name = "premium_tier", Order = 28)]
        public int PremiumTier;

        [DataMember(Name = "premium_subscription_count", Order = 29)]
        public int PremiumSubscriberCount;

        public void OverwriteContext(DiscordGuildPacketRoot guild)
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