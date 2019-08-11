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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user in question; null meaning the user will be self</param>
        /// <returns></returns>
        Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user = null);

        Task<IDiscordGuildUser> GetUserAsync(ulong id);

        Task<IDiscordGuild> GetGuildAsync();
    }
}