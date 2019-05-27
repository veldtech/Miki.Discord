
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
    [DataContract]
    public class DiscordPresencePacket
	{
        [DataMember(Name = "user", Order = 1)]
		public DiscordUserPacket User { get; set; }

        [DataMember(Name = "roles", Order = 2)]
		public List<ulong> RoleIds { get; set; }

        [DataMember(Name = "game", Order = 3)]
		public Activity Game { get; set; }

        [DataMember(Name = "guild_id", Order = 4)]
		public ulong? GuildId { get; set; }

        [DataMember(Name = "status", Order = 5)]
		public string Status { get; set; }
	}

    [DataContract]
    public class DiscordStatus
	{
        [DataMember(Name = "since", Order = 1)]
		public int? Since { get; set; }

        [DataMember(Name = "game", Order = 2)]
        public Activity Game { get; set; }

        [DataMember(Name = "status", Order = 3)]
		public string Status { get; set; }

        [DataMember(Name = "afk", Order = 4)]
		public bool IsAFK { get; set; }
	}

    [DataContract]
	public class Activity
	{
        [DataMember(Name = "name", Order = 1)]
		public string Name { get; set; }

        [DataMember(Name = "type", Order = 2)]
		public ActivityType Type { get; set; }

        [DataMember(Name = "url", Order = 3)]
		public string Url { get; set; }

        [DataMember(Name = "timestamps", Order = 4)]
		public TimeStampsObject Timestamps { get; set; }

        [DataMember(Name = "application_id", Order = 5)]
		public ulong? ApplicationId { get; set; }

        [DataMember(Name = "state", Order = 6)]
		public string State { get; set; }

        [DataMember(Name = "details", Order = 7)]
		public string Details { get; set; }

        [DataMember(Name = "party", Order = 8)]
		public RichPresenceParty Party { get; set; }

        [DataMember(Name = "assets", Order = 9)]
		public RichPresenceAssets Assets { get; set; }
	}

    [DataContract]
    public class RichPresenceParty
	{
        [DataMember(Name = "id", Order = 1)]
		public string Id { get; set; }

        [DataMember(Name = "size", Order = 2)]
		public int[] Size { get; set; }
	}

    [DataContract]
    public class RichPresenceAssets
	{
        [DataMember(Name = "large_image", Order = 1)]
		public string LargeImage { get; set; }

        [DataMember(Name = "large_text", Order = 2)]
		public string LargeText { get; set; }

        [DataMember(Name = "small_image", Order = 3)]
		public string SmallImage { get; set; }

        [DataMember(Name = "small_text", Order = 4)]
		public string SmallText { get; set; }
	}

    [DataContract]
    public class TimeStampsObject
	{
        [DataMember(Name = "start", Order = 1)]
		public long Start;

        [DataMember(Name = "end", Order = 2)]
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