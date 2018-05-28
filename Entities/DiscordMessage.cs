using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Miki.Discord.Rest.Entities
{
	[Serializable]
	public class DiscordMessage
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
		public DiscordUser Author { get; set; }

		[JsonProperty("timestamp")]
		public DateTimeOffset TimeStamp { get; set; }

		[JsonProperty("tts")]
		public bool IsTTS { get; set; }

		[JsonProperty("mention_everyone")]
		public bool MentionsEveryone { get; set; }

		[JsonProperty("mentions")]
		public List<DiscordUser> Mentions { get; set; }

		public DiscordClient _client;
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
