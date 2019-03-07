using Miki.Discord.Rest;

namespace Miki.Discord.Common
{
	public interface IDiscordRole : ISnowflake
	{
		string Name { get; }
		Color Color { get; }
		int Position { get; }
		GuildPermission Permissions { get; }
	}
}