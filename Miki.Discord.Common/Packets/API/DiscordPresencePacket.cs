using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Miki.Discord.Common.Packets
{
	// TODO (Veld): What was this for again?
	[ProtoContract]
	public class DiscordPresencePacket
	{
		[JsonProperty("user")]
		[ProtoMember(1)]
		public DiscordUserPacket User { get; set; }

		[JsonProperty("roles")]
		[ProtoMember(2)]
		public List<ulong> RoleIds { get; set; }

		[JsonProperty("game")]
		[ProtoMember(3)]
		public Activity Game { get; set; }

		[JsonProperty("guild_id")]
		[ProtoMember(4)]
		public ulong GuildId { get; set; }

		[JsonProperty("status")]
		[ProtoMember(5)]
		public string Status { get; set; }
	}

	[ProtoContract]
	public class DiscordStatus
	{
		[ProtoMember(1)]
		[JsonProperty("since")]
		public int? Since { get; set; }

		[ProtoMember(2)]
		[JsonProperty("game")]
		public Activity Game { get; set; }

		[ProtoMember(3)]
		[JsonProperty("status")]
		public string Status { get; set; }

		[ProtoMember(4)]
		[JsonProperty("afk")]
		public bool IsAFK { get; set; }
	}

	[ProtoContract]
	public class Activity
	{
		[JsonProperty("name")]
		[ProtoMember(1)]
		public string Name { get; set; }

		[JsonProperty("type")]
		[ProtoMember(2)]
		public ActivityType Type { get; set; }

		[JsonProperty("url")]
		[ProtoMember(3)]
		public string Url { get; set; }

		[JsonProperty("timestamps")]
		[ProtoMember(4)]
		public TimeStampsObject Timestamps { get; set; }

		[JsonProperty("application_id")]
		[ProtoMember(5)]
		public ulong? ApplicationId { get; set; }

		[JsonProperty("state")]
		[ProtoMember(6)]
		public string State { get; set; }

		[JsonProperty("details")]
		[ProtoMember(7)]
		public string Details { get; set; }
		
		[JsonProperty("party")]
		[ProtoMember(8)]
		public RichPresenceParty Party { get; set; }

		[JsonProperty("assets")]
		[ProtoMember(9)]
		public RichPresenceAssets Assets { get; set; }
	}

	[ProtoContract]
	public class RichPresenceParty
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		public string Id { get; set; }

		[JsonProperty("size")]
		[ProtoMember(2)]
		public int[] Size { get; set; }
	}

	[ProtoContract]
	public class RichPresenceAssets
	{
		[JsonProperty("large_image")]
		[ProtoMember(1)]
		public string LargeImage { get; set; }

		[JsonProperty("large_text")]
		[ProtoMember(2)]
		public string LargeText { get; set; }

		[JsonProperty("small_image")]
		[ProtoMember(3)]
		public string SmallImage { get; set; }

		[JsonProperty("small_text")]
		[ProtoMember(4)]
		public string SmallText { get; set; }
	}

	[ProtoContract]
	public class TimeStampsObject
	{
		[JsonProperty("start")]
		[ProtoMember(1)]
		public long Start;

		[JsonProperty("end")]
		[ProtoMember(2)]
		public long End;
	}

	public enum UserStatus
	{
		ONLINE,
		IDLE,
		DND,
		OFFLINE
	}
}