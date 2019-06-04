using Miki.Discord.Common.Packets;

namespace Miki.Discord.Common
{
	public interface IDiscordPresence
	{
		DiscordActivity Activity { get; }
		UserStatus Status { get; }
	}
}