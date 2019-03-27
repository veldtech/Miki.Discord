using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordGuildPacket
	{
        [DataMember(Name = "id")]
        [ProtoMember(1)]
		[Key(0)]
		public ulong Id;
        [DataMember(Name = "name")]
        [ProtoMember(2)]
		[Key(1)]
		public string Name;

        [DataMember(Name = "icon")]
        [ProtoMember(3)]
		[Key(2)]
		public string Icon;

        [DataMember(Name = "splash")]
        [ProtoMember(4)]
		[Key(3)]
		public string Splash;

        [DataMember(Name = "owner_id")]
        [ProtoMember(5)]
		[Key(4)]
		public ulong OwnerId;

        [DataMember(Name = "region")]
        [ProtoMember(6)]
		[Key(5)]
		public string Region;

        [DataMember(Name = "afk_channel_id")]
        [ProtoMember(7)]
		[Key(6)]
		public ulong? AfkChannelId;

        [DataMember(Name = "afk_timeout")]
        [ProtoMember(8)]
		[Key(7)]
		public int? AfkTimeout;

        [DataMember(Name = "embed_enabled")]
        [ProtoMember(9)]
		[Key(8)]
		public bool EmbedEnabled;

        [DataMember(Name = "embed_channel_id")]
        [ProtoMember(10)]
		[Key(9)]
		public ulong? EmbedChannelId;

        [DataMember(Name = "verification_level")]
        [ProtoMember(11)]
		[Key(10)]
		public int VerificationLevel;

        [DataMember(Name = "default_message_notifications")]
        [ProtoMember(12)]
		[Key(11)]
		public int DefaultMessageNotifications;

        [DataMember(Name = "explicit_content_filter")]
        [ProtoMember(13)]
		[Key(12)]
		public int ExplicitContentFilter;

        [DataMember(Name = "roles")]
        [ProtoMember(14)]
		[Key(13)]
		public List<DiscordRolePacket> Roles = new List<DiscordRolePacket>();

        [DataMember(Name = "emojis")]
        [ProtoMember(15)]
		[Key(14)]
		public DiscordEmoji[] Emojis;

        [DataMember(Name = "features")]
        [ProtoMember(16)]
		[Key(15)]
		public List<string> Features;

        [DataMember(Name = "mfa_level")]
        [ProtoMember(17)]
		[Key(16)]
		public int MFALevel;

        [DataMember(Name = "application_id")]
        [ProtoMember(18)]
		[Key(17)]
		public ulong? ApplicationId;

        [DataMember(Name = "widget_enabled")]
        [ProtoMember(19)]
		[Key(18)]
		public bool? WidgetEnabled;

        [DataMember(Name = "widget_channel_id")]
        [ProtoMember(20)]
		[Key(19)]
		public ulong? WidgetChannelId;

        [DataMember(Name = "system_channel_id")]
        [ProtoMember(21)]
		[Key(20)]
		public ulong? SystemChannelId;

		[ProtoMember(22)]
		[Key(21)]
		public long CreatedAt;

        [DataMember(Name = "joined_at")]
        internal string _createdAt
		{
			get
			{
				return new DateTime(CreatedAt).ToString("MM/dd/yyyy HH:mm:ss");
			}

			set
			{
				var d = DateTime.ParseExact(value, "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None);
				CreatedAt = d.Ticks;
			}
		}

        [DataMember(Name = "large")]
        [ProtoMember(23)]
		[Key(22)]
		public bool? IsLarge;

        [DataMember(Name = "unavailable")]
        [ProtoMember(24)]
		[Key(23)]
		public bool? Unavailable;

        [DataMember(Name = "member_count")]
        [ProtoMember(25)]
		[Key(24)]
		public int? MemberCount;

        [DataMember(Name = "voice_states")]
        [IgnoreMember]
		public List<DiscordVoiceStatePacket> VoiceStates;

        [DataMember(Name = "members")]
        [IgnoreMember]
		public List<DiscordGuildMemberPacket> Members = new List<DiscordGuildMemberPacket>();

        [DataMember(Name = "channels")]
        [IgnoreMember]
		public List<DiscordChannelPacket> Channels = new List<DiscordChannelPacket>();

        [DataMember(Name = "presences")]
        //[ProtoMember(27)]
        [IgnoreMember]
		public List<DiscordPresencePacket> Presences = new List<DiscordPresencePacket>();

        [DataMember(Name = "owner")]
        [ProtoMember(26)]
		[Key(25)]
		public bool? IsOwner;

        [DataMember(Name = "permissions")]
        [ProtoMember(27)]
		[Key(26)]
		public int? Permissions;

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
			Roles = guild.Roles;
			Emojis = guild.Emojis;
			MFALevel = guild.MFALevel;
			ApplicationId = guild.ApplicationId;
			WidgetEnabled = guild.WidgetEnabled;
			WidgetChannelId = guild.WidgetChannelId;
			SystemChannelId = guild.SystemChannelId;
		}
	}
}