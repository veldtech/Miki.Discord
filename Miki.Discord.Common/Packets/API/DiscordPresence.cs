using Miki.Discord.Common.Packets;
using ProtoBuf;
using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
	[ProtoContract]
    [DataContract]
    public class DiscordPresence : IDiscordPresence
	{
		[DataMember(Name ="game")]
		[ProtoMember(1)]
		public Activity Activity { get; private set; }

		[DataMember(Name ="status")]
		[ProtoMember(2)]
		public UserStatus Status { get; private set; }

		[DataMember(Name ="since")]
		public long Since { get; private set; }

		[DataMember(Name ="afk")]
		public bool IsAFK { get; private set; }

		public DiscordPresence()
		{
		}
		public DiscordPresence(DiscordPresencePacket packet)
		{
			Activity = packet.Game;
			Status = SystemUtils.ParseEnum<UserStatus>(packet.Status);
		}
	}

	public static class SystemUtils
	{
		public static T ParseEnum<T>(string enumValue, bool ignoreCase = true) where T : struct
		{
			return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
		}
	}
}