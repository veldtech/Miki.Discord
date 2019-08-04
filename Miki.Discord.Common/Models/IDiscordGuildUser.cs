using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordGuildUser : IDiscordUser
    {
        string Nickname { get; }

        IReadOnlyCollection<ulong> RoleIds { get; }

        ulong GuildId { get; }

        DateTimeOffset JoinedAt { get; }

        /// <summary>
        /// This user nitro boosting current 
        /// </summary>
        DateTimeOffset PremiumSince { get; }

        Task AddRoleAsync(IDiscordRole role);

        Task<IDiscordGuild> GetGuildAsync();

        Task<int> GetHierarchyAsync();

        Task<bool> HasPermissionsAsync(GuildPermission permissions);

        Task KickAsync(string reason = "");

        Task RemoveRoleAsync(IDiscordRole role);
    }
}