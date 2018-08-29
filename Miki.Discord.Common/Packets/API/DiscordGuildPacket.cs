using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	public class DiscordGuildPacket
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		public ulong Id;

		[JsonProperty("name")]
		[ProtoMember(2)]
		public string Name;

		[JsonProperty("icon")]
		[ProtoMember(3)]
		public string Icon;

		[JsonProperty("splash")]
		[ProtoMember(4)]
		public string Splash;

		[JsonProperty("owner_id")]
		[ProtoMember(5)]
		public ulong OwnerId;

		[JsonProperty("region")]
		[ProtoMember(6)]
		public string Region;

		[JsonProperty("afk_channel_id")]
		[ProtoMember(7)]
		public ulong? AfkChannelId;

		[JsonProperty("afk_timeout")]
		[ProtoMember(8)]
		public int? AfkTimeout;

		[JsonProperty("embed_enabled")]
		[ProtoMember(9)]
		public bool EmbedEnabled;

		[JsonProperty("embed_channel_id")]
		[ProtoMember(10)]
		public ulong? EmbedChannelId;

		[JsonProperty("verification_level")]
		[ProtoMember(11)]
		public int VerificationLevel;

		[JsonProperty("default_message_notifications")]
		[ProtoMember(12)]
		public int DefaultMessageNotifications;

		[JsonProperty("explicit_content_filter")]
		[ProtoMember(13)]
		public int ExplicitContentFilter;

		[JsonProperty("roles")]
		[ProtoMember(14)]
		public List<DiscordRolePacket> Roles = new List<DiscordRolePacket>();

		[JsonProperty("emojis")]
		[ProtoMember(15)]
		public List<DiscordEmojiPacket> Emojis = new List<DiscordEmojiPacket>();

		[JsonProperty("features")]
		[ProtoMember(16)]
		public List<string> Features;

		[JsonProperty("mfa_level")]
		[ProtoMember(17)]
		public int MFALevel;

		[JsonProperty("application_id")]
		[ProtoMember(18)]
		public ulong? ApplicationId;

		[JsonProperty("widget_enabled")]
		[ProtoMember(19)]
		public bool? WidgetEnabled;

		[JsonProperty("widget_channel_id")]
		[ProtoMember(20)]
		public ulong? WidgetChannelId;

		[JsonProperty("system_channel_id")]
		[ProtoMember(21)]
		public ulong? SystemChannelId;

		[ProtoMember(22)]
		public long CreatedAt;

		[JsonProperty("joined_at")]
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

		[JsonProperty("large")]
		[ProtoMember(23)]
		public bool? IsLarge;

		[JsonProperty("unavailable")]
		[ProtoMember(24)]
		public bool? Unavailable;

		[JsonProperty("member_count")]
		[ProtoMember(25)]
		public int? MemberCount;

		[JsonProperty("voice_states")]
		[ProtoMember(26)]
		public List<DiscordVoiceStatePacket> VoiceStates;
		
		[JsonProperty("members")]
		public List<DiscordGuildMemberPacket> Members = new List<DiscordGuildMemberPacket>();

		[JsonProperty("channels")]
		public List<DiscordChannelPacket> Channels = new List<DiscordChannelPacket>();

		[JsonProperty("presences")]
		[ProtoMember(27)]
		public List<DiscordPresencePacket> Presences = new List<DiscordPresencePacket>();

		[JsonProperty("owner")]
		[ProtoMember(28)]
		public bool? IsOwner;

		[JsonProperty("permissions")]
		[ProtoMember(29)]
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
