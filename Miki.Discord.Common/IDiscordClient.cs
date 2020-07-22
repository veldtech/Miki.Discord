using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Miki.Discord.Events;

namespace Miki.Discord.Common
{
    public interface IDiscordClient : IDisposable, IHostedService
    {
        /// <summary>
        /// The api client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IApiClient ApiClient { get; }

        /// <summary>
        /// The gateway client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IGateway Gateway { get; }

        /// <summary>
        /// Object containing all Discord gateway events.
        /// </summary>
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
        
        Task<IDiscordSelfUser> GetSelfAsync();

        Task<IDiscordGuild> GetGuildAsync(ulong id);

        Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId);

        Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId);

        Task<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

        Task<IDiscordUser> GetUserAsync(ulong id);

        Task SetGameAsync(int shardId, DiscordStatus status);

        /// <summary>
        /// Sends a file from containing <paramref name="stream"/>.
        /// </summary>
        Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null);

        /// <summary>
        /// Sends a message to <paramref name="channelId"/>.
        /// </summary>
        Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message);

        /// <summary>
        /// Sends a message to <paramref name="channelId"/>.
        /// </summary>
        Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null);
    }
}