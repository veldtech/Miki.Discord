using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
	public interface IDiscordPresence
	{
		Activity Activity { get; }
		UserStatus Status { get; }
	}
}