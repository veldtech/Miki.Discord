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

        /// <summary>
        /// Bans a discord user from the discord guild.
        /// </summary>
        /// <param name="user">The user in question.</param>
        /// <param name="pruneDays">Amount of days of messages sent to be pruned</param>
        /// <param name="reason">Reason of the ban</param>
		Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null);

        /// <summary>
        /// Creates a new role. Requires <see cref="GuildPermission.ManageRoles"/> permission.
        /// </summary>
        /// <param name="roleArgs">Role properties</param>
		Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs);

        /// <summary>
        /// Gets the system channel.
        /// </summary>
		Task<IDiscordChannel> GetDefaultChannelAsync();

        /// <summary>
        /// Get permissions that are attached the <see cref="IDiscordGuildUser"/> based on the guild's permissions and the user's roles.
        /// </summary>
        Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

        /// <summary>
        /// Gets the <see cref="IDiscordGuildUser"/> of the current discord server owner.
        /// </summary>
		Task<IDiscordGuildUser> GetOwnerAsync();

        /// <summary>
        /// Gets a <see cref="IDiscordGuildChannel"/> where its snowflake matches {id}. Will return null if non-existant.
        /// </summary>
        /// <param name="id">Snowflake of the channel.</param>
        Task<IDiscordGuildChannel> GetChannelAsync(ulong id);

        /// <summary>
        /// Returns all channels in a guild.
        /// </summary>
        Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync();

        /// <summary>
        /// Gets a <see cref="IDiscordRole"/> where its snowflake matches {id}. Will return null if non-existant.
        /// </summary>
        /// <param name="id">Snowflake of the role.</param>
		Task<IDiscordRole> GetRoleAsync(ulong id);

        /// <summary>
        /// Returns all roles in a guild.
        /// </summary>
		Task<IEnumerable<IDiscordRole>> GetRolesAsync();

        /// <summary>
        /// Returns all members in a guild.
        /// </summary>
		Task<IEnumerable<IDiscordGuildUser>> GetMembersAsync();

		/// <summary>
		/// Gets the <see cref="IDiscordGuildUser"/> that fits with the current snowflake.
		/// </summary>
		/// <param name="id">Specified <seealso cref="ISnowflake"/> pointing to a <seealso cref="IDiscordGuildUser"/>.</param>
		/// <returns></returns>
		Task<IDiscordGuildUser> GetMemberAsync(ulong id);

        /// <summary>
        /// Get the amount of users that would be pruned if you would prune by {days} days.
        /// </summary>
        /// <param name="days">Amount of days since user was last online.</param>
        /// <returns>Amount of users that will be pruned.</returns>
        Task<int> GetPruneCountAsync(int days);
        /// <summary>
        /// Starts a prune operation. Requires <see cref="GuildPermission.KickMembers"/> to be active for the current user.
        /// </summary>
        /// <param name="days">Amount of days since user was last online.</param>
        /// <param name="computeCount">Whether a count is calculated and returned, discouraged for larger guilds.</param>
        /// <returns>Amount of users that will be pruned.</returns>
        Task<int?> PruneMembersAsync(int days, bool computeCount = false);

        /// <summary>
        /// Gets the current user as a <see cref="IDiscordGuildUser"/>.
        /// </summary>
		Task<IDiscordGuildUser> GetSelfAsync();

        /// <summary>
        /// Removes a ban from the server, requires <see cref="GuildPermission.BanMembers"/>.
        /// </summary>
        /// <param name="user">User you want to unban.</param>
		Task RemoveBanAsync(IDiscordGuildUser user);
	}
}