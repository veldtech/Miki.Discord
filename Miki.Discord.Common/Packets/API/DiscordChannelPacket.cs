using MessagePack;
using ProtoBuf;
using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
	[Serializable]
	[ProtoContract]
	[MessagePackObject]
	public class DiscordChannelPacket
	{
        [DataMember(Name = "id")]
        [ProtoMember(1)]
		[Key(0)]
		public ulong Id { get; set; }

        [DataMember(Name = "type")]
        [ProtoMember(2)]
		[Key(1)]
		public ChannelType Type { get; set; }

        [DataMember(Name = "created_at")]
        [ProtoMember(3)]
		[Key(2)]
		public long CreatedAt { get; set; }

        [DataMember(Name = "name")]
        [ProtoMember(4)]
		[Key(3)]
		public string Name { get; set; }

        [DataMember(Name = "guild_id")]
        [ProtoMember(5)]
		[Key(4)]
		public ulong? GuildId { get; set; }

        [DataMember(Name = "position")]
        [ProtoMember(6)]
		[Key(5)]
		public int? Position { get; set; }

        [DataMember(Name = "permission_overwrites")]
        [ProtoMember(7)]
		[Key(6)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

        [DataMember(Name = "parent_id")]
        [ProtoMember(8)]
		[Key(7)]
		public ulong? ParentId { get; set; }

        [DataMember(Name = "nsfw")]
        [ProtoMember(9)]
		[Key(8)]
		public bool? IsNsfw { get; set; }

        [DataMember(Name = "topic")]
        [ProtoMember(10)]
		[Key(9)]
		public string Topic { get; set; }
	}

	public enum ChannelType
	{
		GUILDTEXT,
		DM,
		GUILDVOICE,
		GROUPDM,
		CATEGORY,
	}
}