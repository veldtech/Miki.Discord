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

		int MemberCount { get; }

		GuildPermission Permissions { get; }

		IReadOnlyCollection<IDiscordGuildChannel> Channels { get; }

		IReadOnlyCollection<IDiscordRole> Roles { get; }

		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

		IDiscordChannel GetDefaultChannel();

		GuildPermission GetPermissions(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetOwnerAsync();

		IDiscordRole GetRole(ulong id);

		Task<IDiscordGuildUser[]> GetMembersAsync();

		/// <summary>
		/// Updates the guild member from the current updated cache and returns it
		/// </summary>
		/// <param name="id">specified guildmember id</param>
		/// <returns></returns>
		Task<IDiscordGuildUser> GetMemberAsync(ulong id);

		Task<IDiscordGuildUser> GetSelfAsync();

		Task RemoveBanAsync(IDiscordGuildUser user);
	}
}
