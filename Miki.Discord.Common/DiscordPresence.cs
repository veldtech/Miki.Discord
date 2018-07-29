using Miki.Discord.Rest.Entities;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

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

		public DiscordPresence() { }
		public DiscordPresence(DiscordPresencePacket packet)
		{
			Activity = packet.Game;
			Status = Utils.ParseEnum<UserStatus>(packet.Status);
		}
	}

	public static class Utils
	{
		public static T ParseEnum<T>(string enumValue, bool ignoreCase = true) where T : Enum
		{
			return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
		}
	}
}
