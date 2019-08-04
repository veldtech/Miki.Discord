using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
	[Serializable]
	[DataContract]
	public class DiscordChannelPacket
	{
		[DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

		[DataMember(Name = "type", Order = 2)]
		public ChannelType Type { get; set; }

		[DataMember(Name = "created_at", Order = 3)]
		public long CreatedAt { get; set; }

		[DataMember(Name = "name", Order = 4)]
		public string Name { get; set; }

		[DataMember(Name = "guild_id", Order = 5)]
		public ulong? GuildId { get; set; }

		[DataMember(Name = "position", Order = 6)]
		public int? Position { get; set; }

		[DataMember(Name = "permission_overwrites", Order = 7)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

		[DataMember(Name = "parent_id", Order = 8)]
		public ulong? ParentId { get; set; }

		[DataMember(Name = "nsfw", Order = 9)]
		public bool? IsNsfw { get; set; }

		[DataMember(Name = "topic", Order = 10)]
		public string Topic { get; set; }
	}

	public enum ChannelType
	{
		GUILDTEXT = 0,
		DM,
		GUILDVOICE,
		GROUPDM,
		CATEGORY,
	}
}