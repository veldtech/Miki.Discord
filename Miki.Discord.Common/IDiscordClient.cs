using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Miki.Discord.Events;

namespace Miki.Discord.Common
{
    public interface IDiscordClient : IDisposable
    {
        /// <summary>
        /// The api client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IApiClient ApiClient { get; }

        /// <summary>
        /// The gateway client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IGateway Gateway { get; }

        IDiscordEvents Events { get; }  

        Task<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string text, DiscordEmbed embed = null);

        Task<IDiscordTextChannel> CreateDMAsync(ulong userid);

        Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null);

        Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role);

        Task<IDiscordPresence> GetUserPresence(ulong userId, ulong? guildId = null);

        Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId);

        Task<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId);

        Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId);

        Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null);

        Task<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel;

        Task<IDiscordSelfUser> GetSelfAsync();

        Task<IDiscordGuild> GetGuildAsync(ulong id);

        Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId);

        Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId);

        Task<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

        Task<IDiscordUser> GetUserAsync(ulong id);

        Task SetGameAsync(int shardId, DiscordStatus status);

        Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null);

        Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message);

        Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null);
    }
}