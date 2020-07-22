using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordChannel : ISnowflake
    {
        bool IsNsfw { get; }

        string Name { get; }

        Task DeleteAsync();

        Task ModifyAsync(object todo);
    }

    public interface IDiscordGuildChannel : IDiscordChannel
    {
        IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        ulong GuildId { get; }

        ChannelType Type { get; }

        Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user = null);

        Task<IDiscordGuildUser> GetUserAsync(ulong id);

        /// <summary>
        /// Gets the current user in the guild.
        /// </summary>
        Task<IDiscordGuildUser> GetSelfAsync();

        Task<IDiscordGuild> GetGuildAsync();
    }
}