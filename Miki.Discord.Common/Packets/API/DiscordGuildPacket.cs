using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordGuildPacket
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		[Key(0)]
		public ulong Id;

		[JsonProperty("name")]
		[ProtoMember(2)]
		[Key(1)]
		public string Name;	

		[JsonProperty("icon")]
		[ProtoMember(3)]
		[Key(2)]
		public string Icon;

		[JsonProperty("splash")]
		[ProtoMember(4)]
		[Key(3)]
		public string Splash;

		[JsonProperty("owner_id")]
		[ProtoMember(5)]
		[Key(4)]
		public ulong OwnerId;

		[JsonProperty("region")]
		[ProtoMember(6)]
		[Key(5)]
		public string Region;

		[JsonProperty("afk_channel_id")]
		[ProtoMember(7)]
		[Key(6)]
		public ulong? AfkChannelId;

		[JsonProperty("afk_timeout")]
		[ProtoMember(8)]
		[Key(7)]
		public int? AfkTimeout;

		[JsonProperty("embed_enabled")]
		[ProtoMember(9)]
		[Key(8)]
		public bool EmbedEnabled;

		[JsonProperty("embed_channel_id")]
		[ProtoMember(10)]
		[Key(9)]
		public ulong? EmbedChannelId;

		[JsonProperty("verification_level")]
		[ProtoMember(11)]
		[Key(10)]
		public int VerificationLevel;

		[JsonProperty("default_message_notifications")]
		[ProtoMember(12)]
		[Key(11)]
		public int DefaultMessageNotifications;

		[JsonProperty("explicit_content_filter")]
		[ProtoMember(13)]
		[Key(12)]
		public int ExplicitContentFilter;

		[JsonProperty("roles")]
		[ProtoMember(14)]
		[Key(13)]
		public List<DiscordRolePacket> Roles = new List<DiscordRolePacket>();

		[JsonProperty("emojis")]
		[ProtoMember(15)]
		[Key(14)]
		public List<DiscordEmoji> Emojis = new List<DiscordEmoji>();

		[JsonProperty("features")]
		[ProtoMember(16)]
		[Key(15)]
		public List<string> Features;

		[JsonProperty("mfa_level")]
		[ProtoMember(17)]
		[Key(16)]
		public int MFALevel;

		[JsonProperty("application_id")]
		[ProtoMember(18)]
		[Key(17)]
		public ulong? ApplicationId;

		[JsonProperty("widget_enabled")]
		[ProtoMember(19)]
		[Key(18)]
		public bool? WidgetEnabled;

		[JsonProperty("widget_channel_id")]
		[ProtoMember(20)]
		[Key(19)]
		public ulong? WidgetChannelId;

		[JsonProperty("system_channel_id")]
		[ProtoMember(21)]
		[Key(20)]
		public ulong? SystemChannelId;

		[ProtoMember(22)]
		[Key(21)]
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
		[Key(22)]
		public bool? IsLarge;

		[JsonProperty("unavailable")]
		[ProtoMember(24)]
		[Key(23)]
		public bool? Unavailable;

		[JsonProperty("member_count")]
		[ProtoMember(25)]
		[Key(24)]
		public int? MemberCount;

		[JsonProperty("voice_states")]
		//[ProtoMember(26)]
		[IgnoreMember]
		public List<DiscordVoiceStatePacket> VoiceStates;
		
		[JsonProperty("members")]
		[IgnoreMember]

		public List<DiscordGuildMemberPacket> Members = new List<DiscordGuildMemberPacket>();

		[JsonProperty("channels")]

		[IgnoreMember]
		public List<DiscordChannelPacket> Channels = new List<DiscordChannelPacket>();

		[JsonProperty("presences")]
		//[ProtoMember(27)]
		[IgnoreMember]
		public List<DiscordPresencePacket> Presences = new List<DiscordPresencePacket>();

		[JsonProperty("owner")]
		[ProtoMember(26)]
		[Key(25)]
		public bool? IsOwner;

		[JsonProperty("permissions")]
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
