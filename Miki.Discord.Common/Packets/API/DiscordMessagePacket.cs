﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Miki.Discord.Common.Packets
{
	[Serializable]
	public class DiscordMessagePacket
	{
		[JsonProperty("id")]
		public ulong Id { get; set; }

		[JsonProperty("type")]
		public MessageType Type { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }

		[JsonProperty("channel_id")]
		public ulong ChannelId { get; set; }

		[JsonProperty("author")]
		public DiscordUserPacket Author { get; set; }

		[JsonProperty("timestamp")]
		public DateTimeOffset Timestamp { get; set; }

		[JsonProperty("tts")]
		public bool IsTTS { get; set; }

		[JsonProperty("mention_everyone")]
		public bool MentionsEveryone { get; set; }

		[JsonProperty("mentions")]
		public List<DiscordUserPacket> Mentions { get; set; }

        [JsonProperty("attachments")]
        public List<DiscordAttachmentPacket> Attachments { get; set; }

		[JsonProperty("guild_id")]
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