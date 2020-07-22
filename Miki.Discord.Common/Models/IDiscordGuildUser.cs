using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Miki.Discord.Common.Models;

namespace Miki.Discord.Common
{
    public interface IDiscordGuildUser : IDiscordUser, IContainsGuild
    {
        string Nickname { get; }

        IReadOnlyCollection<ulong> RoleIds { get; }

        DateTimeOffset JoinedAt { get; }

        /// <summary>
        /// This user nitro boosting current 
        /// </summary>
        DateTimeOffset? PremiumSince { get; }

        Task AddRoleAsync(IDiscordRole role);

        Task<int> GetHierarchyAsync();

        Task<IEnumerable<IDiscordRole>> GetRolesAsync();

        Task<bool> HasPermissionsAsync(GuildPermission permissions);

        Task KickAsync(string reason = "");

        Task RemoveRoleAsync(IDiscordRole role);
    }
}