using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using System.Collections.Generic;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordPresencePacket
	{
		[JsonProperty("user")]
		[ProtoMember(1)]
		[Key(0)]
		public DiscordUserPacket User { get; set; }

		[JsonProperty("roles")]
		[ProtoMember(2)]
		[Key(1)]
		public List<ulong> RoleIds { get; set; }

		[JsonProperty("game")]
		[ProtoMember(3)]
		[Key(2)]
		public Activity Game { get; set; }

		[JsonProperty("guild_id")]
		[ProtoMember(4)]
		[Key(3)]
		public ulong? GuildId { get; set; }

		[JsonProperty("status")]
		[ProtoMember(5)]
		[Key(4)]
		public string Status { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class DiscordStatus
	{
		[ProtoMember(1)]
		[JsonProperty("since")]
		[Key(0)]
		public int? Since { get; set; }

		[ProtoMember(2)]
		[JsonProperty("game")]
		[Key(1)]
		public Activity Game { get; set; }

		[ProtoMember(3)]
		[JsonProperty("status")]
		[Key(2)]
		public string Status { get; set; }

		[ProtoMember(4)]
		[JsonProperty("afk")]
		[Key(3)]
		public bool IsAFK { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class Activity
	{
		[JsonProperty("name")]
		[ProtoMember(1)]
		[Key(0)]
		public string Name { get; set; }

		[JsonProperty("type")]
		[ProtoMember(2)]
		[Key(1)]
		public ActivityType Type { get; set; }

		[JsonProperty("url")]
		[ProtoMember(3)]
		[Key(2)]
		public string Url { get; set; }

		[JsonProperty("timestamps")]
		[ProtoMember(4)]
		[Key(3)]
		public TimeStampsObject Timestamps { get; set; }

		[JsonProperty("application_id")]
		[ProtoMember(5)]
		[Key(4)]
		public ulong? ApplicationId { get; set; }

		[JsonProperty("state")]
		[ProtoMember(6)]
		[Key(5)]
		public string State { get; set; }

		[JsonProperty("details")]
		[ProtoMember(7)]
		[Key(6)]
		public string Details { get; set; }

		[JsonProperty("party")]
		[ProtoMember(8)]
		[Key(7)]
		public RichPresenceParty Party { get; set; }

		[JsonProperty("assets")]
		[ProtoMember(9)]
		[Key(8)]
		public RichPresenceAssets Assets { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class RichPresenceParty
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		[Key(0)]
		public string Id { get; set; }

		[JsonProperty("size")]
		[ProtoMember(2)]
		[Key(1)]
		public int[] Size { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class RichPresenceAssets
	{
		[JsonProperty("large_image")]
		[ProtoMember(1)]
		[Key(0)]
		public string LargeImage { get; set; }

		[JsonProperty("large_text")]
		[ProtoMember(2)]
		[Key(1)]
		public string LargeText { get; set; }

		[JsonProperty("small_image")]
		[ProtoMember(3)]
		[Key(2)]
		public string SmallImage { get; set; }

		[JsonProperty("small_text")]
		[ProtoMember(4)]
		[Key(3)]
		public string SmallText { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class TimeStampsObject
	{
		[JsonProperty("start")]
		[ProtoMember(1)]
		[Key(0)]
		public long Start;

		[JsonProperty("end")]
		[ProtoMember(2)]
		[Key(1)]
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