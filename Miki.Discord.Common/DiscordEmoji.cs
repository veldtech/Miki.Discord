using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MessagePack;
using System.Text.RegularExpressions;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
	[ProtoContract]
	[MessagePackObject]
    public class DiscordEmoji
    {
		[ProtoMember(1)]
		[JsonProperty("id")]
		[Key(0)]
		public ulong Id { get; set; }

		[ProtoMember(2)]
		[JsonProperty("name")]
		[Key(1)]
		public string Name { get; set; }

		[ProtoMember(3)]
		[JsonProperty("roles")]
		[Key(2)]
		public List<ulong> WhitelistedRoles { get; set; }

		[ProtoMember(4)]
		[JsonProperty("user")]
		[Key(3)]
		public DiscordUserPacket Creator { get; set; }

		[ProtoMember(5)]
		[Key(4)]
		[JsonProperty("require_colons")]
		public bool RequireColons { get; set; }

		[ProtoMember(6)]
		[Key(5)]
		[JsonProperty("managed")]
		public bool Managed { get; set; }

		[ProtoMember(7)]
		[Key(6)]
		[JsonProperty("animated")]
		public bool Animated { get; set; }

		public static DiscordEmoji Parse(string emoji)
		{
			if(emoji.Length == 0)
			{
				throw new ArgumentNullException();
			}

			Match matchedString = Regex.Match(emoji, "<(a?):(.*):(\\d+)>");

			if(matchedString.Success)
			{
				var newEmoji = new DiscordEmoji();

				return newEmoji;
			}
			throw new FormatException($"Can not parse emoji '{emoji}'.");
		}
	}
}
