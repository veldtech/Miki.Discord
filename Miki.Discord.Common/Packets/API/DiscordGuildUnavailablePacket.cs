using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Packets
{
	public class DiscordGuildUnavailablePacket
	{
		[JsonProperty("id")]
		public ulong GuildId;

		[JsonProperty("unavailable")]
		public bool? IsUnavailable;

		/// <summary>
		/// A converter method to avoid protocol buffer serialization complexion
		/// </summary>
		/// <returns>A converted DiscordGuildPacket</returns>
		public DiscordGuildPacket ToGuildPacket()
		{
			return new DiscordGuildPacket
			{
				Id = GuildId,
				Unavailable = IsUnavailable
			};
		}
	}
}