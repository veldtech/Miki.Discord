using Newtonsoft.Json;
using ProtoBuf;
using System;

namespace Miki.Discord.Rest.Entities
{
	[Serializable]
	[ProtoContract]
	public class DiscordChannel
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		public ulong Id { get; set; }

		[JsonProperty("type")]
		[ProtoMember(2)]
		public ChannelType Type { get; set; }

		[JsonProperty("created_at")]
		[ProtoMember(3)]
		public long CreatedAt { get; set; }

		[JsonProperty("name")]
		[ProtoMember(4)]
		public string Name { get; set; }

		[JsonProperty("guild_id")]
		[ProtoMember(5)]
		public ulong GuildId { get; set; }

		[JsonProperty("position")]
		[ProtoMember(6)]
		public int Position { get; set; }

		[JsonProperty("permission_overwrites")]
		[ProtoMember(7)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

		[JsonProperty("parent_id")]
		[ProtoMember(8)]
		public ulong? ParentId { get; set; }

		[JsonProperty("nsfw")]
		[ProtoMember(9)]
		public bool IsNsfw { get; set; }

		[JsonProperty("topic")]
		[ProtoMember(10)]
		public string Topic { get; set; }
	}

	[ProtoContract]
	public class PermissionOverwrite
	{
		[JsonProperty("id")]
		[ProtoMember(1)]
		public ulong Id;

		[JsonProperty("type")]
		[ProtoMember(2)]
		public string Type;

		[JsonProperty("allow")]
		[ProtoMember(3)]
		public int Allow;

		[JsonProperty("deny")]
		[ProtoMember(4)]
		public int Deny;
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
