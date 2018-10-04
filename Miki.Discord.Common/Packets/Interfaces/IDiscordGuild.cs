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

		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

		Task<IDiscordChannel> GetDefaultChannelAsync();

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetOwnerAsync();

		Task<IDiscordGuildChannel> GetChannelAsync(ulong id);

		Task<IReadOnlyList<IDiscordGuildChannel>> GetChannelsAsync();

		Task<IDiscordRole> GetRoleAsync(ulong id);

		Task<IReadOnlyList<IDiscordRole>> GetRolesAsync();

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
