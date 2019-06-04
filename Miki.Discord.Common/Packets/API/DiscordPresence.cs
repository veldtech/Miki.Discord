using Miki.Discord.Common.Packets;
using System;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class DiscordPresence : IDiscordPresence
	{
		[DataMember(Name ="game", Order = 1)]
		public DiscordActivity Activity { get; private set; }

		[DataMember(Name ="status", Order = 2)]
		public UserStatus Status { get; private set; }

		[DataMember(Name ="since", Order = 3)]
		public long Since { get; private set; }

		[DataMember(Name ="afk", Order = 4)]
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