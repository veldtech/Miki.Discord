using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordGuild : ISnowflake
    {
		string Name { get; }

		string IconUrl { get; }

		ulong OwnerId { get; }

		GuildPermission Permissions { get; }

		IReadOnlyCollection<IDiscordGuildChannel> Channels { get; }

		IReadOnlyCollection<IDiscordGuildUser> Members { get; }

		IReadOnlyCollection<IDiscordRole> Roles { get; }

		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

		IDiscordChannel GetDefaultChannel();

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		IDiscordGuildUser GetOwner();

		IDiscordRole GetRole(ulong id);
		
		IDiscordGuildUser GetMember(ulong id);

		Task<IDiscordGuildUser> GetSelfAsync();

		Task RemoveBanAsync(IDiscordGuildUser user);
	}
}
