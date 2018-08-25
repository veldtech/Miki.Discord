using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordGuildUser : IDiscordUser
    {
		string Nickname { get; }

		IReadOnlyCollection<ulong> RoleIds { get; }

		ulong GuildId { get; }

		DateTimeOffset JoinedAt { get; }

		int Hierarchy { get; }

		Task AddRoleAsync(IDiscordRole role);

		Task<IDiscordGuild> GetGuildAsync();

		Task KickAsync(string reason = "");

		Task RemoveRoleAsync(IDiscordRole role);
    }
}
