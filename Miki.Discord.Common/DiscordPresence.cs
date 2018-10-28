using Miki.Discord.Common.Packets;
using Newtonsoft.Json;
using ProtoBuf;
using System;

namespace Miki.Discord.Common
{
	[ProtoContract]
	public class DiscordPresence : IDiscordPresence
	{
		[JsonProperty("game")]
		[ProtoMember(1)]
		public Activity Activity { get; private set; }

		[JsonProperty("status")]
		[ProtoMember(2)]
		public UserStatus Status { get; private set; }

		[JsonProperty("since")]
		public long Since { get; private set; }

		[JsonProperty("afk")]
		public bool IsAFK { get; private set; }

		public DiscordPresence()
		{
		}

		public DiscordPresence(DiscordPresencePacket packet)
		{
			Activity = packet.Game;
			Status = Utils.ParseEnum<UserStatus>(packet.Status);
		}
	}

	public static class Utils
	{
		public static T ParseEnum<T>(string enumValue, bool ignoreCase = true) where T : struct
		{
			return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
		}
	}
}