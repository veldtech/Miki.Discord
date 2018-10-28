using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using System;

namespace Miki.Discord.Common
{
	[Serializable]
	[ProtoContract]
	[MessagePackObject]
	public class DiscordChannelPacket
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		[Key(0)]
		public ulong Id { get; set; }

		[JsonProperty("type")]
		[ProtoMember(2)]
		[Key(1)]
		public ChannelType Type { get; set; }

		[JsonProperty("created_at")]
		[ProtoMember(3)]
		[Key(2)]
		public long CreatedAt { get; set; }

		[JsonProperty("name")]
		[ProtoMember(4)]
		[Key(3)]
		public string Name { get; set; }

		[JsonProperty("guild_id")]
		[ProtoMember(5)]
		[Key(4)]
		public ulong? GuildId { get; set; }

		[JsonProperty("position")]
		[ProtoMember(6)]
		[Key(5)]
		public int? Position { get; set; }

		[JsonProperty("permission_overwrites")]
		[ProtoMember(7)]
		[Key(6)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

		[JsonProperty("parent_id")]
		[ProtoMember(8)]
		[Key(7)]
		public ulong? ParentId { get; set; }

		[JsonProperty("nsfw")]
		[ProtoMember(9)]
		[Key(8)]
		public bool? IsNsfw { get; set; }

		[JsonProperty("topic")]
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