using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[Serializable]
    [DataContract]
    public class DiscordMessagePacket
	{
		[DataMember(Name ="id")]
		public ulong Id { get; set; }

		[DataMember(Name ="type")]
		public MessageType Type { get; set; }

		[DataMember(Name ="content")]
		public string Content { get; set; }

		[DataMember(Name ="channel_id")]
		public ulong ChannelId { get; set; }

		[DataMember(Name ="author")]
		public DiscordUserPacket Author { get; set; }

		[DataMember(Name ="timestamp")]
		public DateTimeOffset Timestamp { get; set; }

		[DataMember(Name ="tts")]
		public bool IsTTS { get; set; }

		[DataMember(Name ="mention_everyone")]
		public bool MentionsEveryone { get; set; }

		[DataMember(Name ="mentions")]
		public List<DiscordUserPacket> Mentions { get; set; }

        [DataMember(Name ="attachments")]
        public List<DiscordAttachmentPacket> Attachments { get; set; }

		[DataMember(Name ="guild_id")]
		public ulong? GuildId { get; set; }
	}

	public enum MessageType
	{
		GUILDTEXT,
		DM,
		GUILDVOICE,
		GROUPDM,
		CATEGORY
	}
}