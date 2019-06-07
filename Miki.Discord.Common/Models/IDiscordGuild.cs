using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
	public interface IDiscordGuild : ISnowflake
	{
        /// <summary>
        /// Display name of the guild.
        /// </summary>
		string Name { get; }

        /// <summary>
        /// A Discord CDN URL pointing to the guild's icon.
        /// </summary>
		string IconUrl { get; }

        /// <summary>
        /// Snowflake referring to the current owner of the guild.
        /// </summary>
		ulong OwnerId { get; }

        /// <summary>
        /// Current amount of members in the guild.
        /// </summary>
		int MemberCount { get; }

        /// <summary>
        /// Current amount of members that are nitro boosting this server.
        /// </summary>
        int PremiumSubscriberCount { get; }

        /// <summary>
        /// Current premium tier.
        /// </summary>
        int PremiumTier { get; }

		GuildPermission Permissions { get; }

		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

		Task<IDiscordChannel> GetDefaultChannelAsync();

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetOwnerAsync();

		Task<IDiscordGuildChannel> GetChannelAsync(ulong id);

		Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync();

		Task<IDiscordRole> GetRoleAsync(ulong id);

		Task<IEnumerable<IDiscordRole>> GetRolesAsync();

		Task<IReadOnlyList<IDiscordGuildUser>> GetMembersAsync();

		/// <summary>
		/// Gets the <see cref="IDiscordGuildUser"/> that fits with the current snowflake.
		/// </summary>
		/// <param name="id">Specified <seealso cref="ISnowflake"/> pointing to a <seealso cref="IDiscordGuildUser"/>.</param>
		/// <returns></returns>
		Task<IDiscordGuildUser> GetMemberAsync(ulong id);

		Task<IDiscordGuildUser> GetSelfAsync();

		Task RemoveBanAsync(IDiscordGuildUser user);
	}
}