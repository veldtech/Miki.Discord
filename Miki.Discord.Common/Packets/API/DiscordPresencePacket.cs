using MessagePack;
using ProtoBuf;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[ProtoContract]
	[MessagePackObject]
    [DataContract]
    public class DiscordPresencePacket
	{
        [DataMember(Name = "user")]
        [ProtoMember(1)]
		[Key(0)]
		public DiscordUserPacket User { get; set; }

        [DataMember(Name = "roles")]
        [ProtoMember(2)]
		[Key(1)]
		public List<ulong> RoleIds { get; set; }

        [DataMember(Name = "game")]
        [ProtoMember(3)]
		[Key(2)]
		public Activity Game { get; set; }

        [DataMember(Name = "guild_id")]
        [ProtoMember(4)]
		[Key(3)]
		public ulong? GuildId { get; set; }

        [DataMember(Name = "status")]
        [ProtoMember(5)]
		[Key(4)]
		public string Status { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class DiscordStatus
	{
		[ProtoMember(1)]
        [DataMember(Name = "since")]
		[Key(0)]
		public int? Since { get; set; }

		[ProtoMember(2)]
        [DataMember(Name = "game")]
        [Key(1)]
		public Activity Game { get; set; }

		[ProtoMember(3)]
        [DataMember(Name = "status")]
        [Key(2)]
		public string Status { get; set; }

		[ProtoMember(4)]
        [DataMember(Name = "afk")]
        [Key(3)]
		public bool IsAFK { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class Activity
	{
        [DataMember(Name = "name")]
        [ProtoMember(1)]
		[Key(0)]
		public string Name { get; set; }

        [DataMember(Name = "type")]
        [ProtoMember(2)]
		[Key(1)]
		public ActivityType Type { get; set; }

        [DataMember(Name = "url")]
        [ProtoMember(3)]
		[Key(2)]
		public string Url { get; set; }

        [DataMember(Name = "timestamps")]
        [ProtoMember(4)]
		[Key(3)]
		public TimeStampsObject Timestamps { get; set; }

        [DataMember(Name = "application_id")]
        [ProtoMember(5)]
		[Key(4)]
		public ulong? ApplicationId { get; set; }

        [DataMember(Name = "state")]
        [ProtoMember(6)]
		[Key(5)]
		public string State { get; set; }

        [DataMember(Name = "details")]
        [ProtoMember(7)]
		[Key(6)]
		public string Details { get; set; }

        [DataMember(Name = "party")]
        [ProtoMember(8)]
		[Key(7)]
		public RichPresenceParty Party { get; set; }

        [DataMember(Name = "assets")]
        [ProtoMember(9)]
		[Key(8)]
		public RichPresenceAssets Assets { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class RichPresenceParty
	{
        [DataMember(Name = "id")]
        [ProtoMember(1)]
		[Key(0)]
		public string Id { get; set; }

        [DataMember(Name = "size")]
        [ProtoMember(2)]
		[Key(1)]
		public int[] Size { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class RichPresenceAssets
	{
        [DataMember(Name = "large_image")]
        [ProtoMember(1)]
		[Key(0)]
		public string LargeImage { get; set; }

        [DataMember(Name = "large_text")]
        [ProtoMember(2)]
		[Key(1)]
		public string LargeText { get; set; }

        [DataMember(Name = "small_image")]
        [ProtoMember(3)]
		[Key(2)]
		public string SmallImage { get; set; }

        [DataMember(Name = "small_text")]
        [ProtoMember(4)]
		[Key(3)]
		public string SmallText { get; set; }
	}

	[ProtoContract]
	[MessagePackObject]
	public class TimeStampsObject
	{
        [DataMember(Name = "start")]
        [ProtoMember(1)]
		[Key(0)]
		public long Start;

        [DataMember(Name = "end")]
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