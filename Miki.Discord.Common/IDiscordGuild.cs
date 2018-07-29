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

		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

		Task<IReadOnlyCollection<IDiscordGuildChannel>> GetChannelsAsync();

		Task<IDiscordChannel> GetDefaultChannelAsync();

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetOwnerAsync();

		Task<IDiscordRole> GetRoleAsync(ulong id);

		Task<IReadOnlyCollection<IDiscordRole>> GetRolesAsync();

		Task<IDiscordGuildUser> GetUserAsync(ulong id);

		Task<IReadOnlyCollection<IDiscordGuildUser>> GetUsersAsync();

		Task<IDiscordGuildUser> GetSelfAsync();

		Task<IReadOnlyCollection<IDiscordChannel>> GetTextChannelsAsync();

		Task<IReadOnlyCollection<IDiscordChannel>> GetVoiceChannelsAsync();

		Task RemoveBanAsync(IDiscordGuildUser user);
	}
}
