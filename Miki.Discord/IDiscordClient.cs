using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;

namespace Miki.Discord
{
    public interface IDiscordClient : IDisposable
    {
        event Func<IDiscordMessage, Task> MessageCreate;
        event Func<IDiscordMessage, Task> MessageUpdate;
        event Func<IDiscordGuild, Task> GuildJoin;
        event Func<IDiscordGuild, Task> GuildAvailable;
        event Func<IDiscordGuildUser, Task> GuildMemberCreate;
        event Func<IDiscordGuildUser, Task> GuildMemberDelete;
        event Func<ulong, Task> GuildLeave;
        event Func<ulong, Task> GuildUnavailable;
        event Func<GatewayReadyPacket, Task> Ready;
        event Func<IDiscordUser, IDiscordUser, Task> UserUpdate;

        /// <summary>
        /// The api client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IApiClient ApiClient { get; }

        /// <summary>
        /// The gateway client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        IGateway Gateway { get; }

        Task<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string text, DiscordEmbed embed = null);

        Task<IDiscordTextChannel> CreateDMAsync(ulong userid);

        Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null);

        Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role);

        Task<IDiscordPresence> GetUserPresence(ulong userId, ulong? guildId = null);

        Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId);

        Task<IReadOnlyList<IDiscordRole>> GetRolesAsync(ulong guildId);

        Task<IReadOnlyList<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId);

        Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null);

        Task<IDiscordSelfUser> GetSelfAsync();

        Task<IDiscordGuild> GetGuildAsync(ulong id);

        Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId);

        Task<IReadOnlyList<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId);

        Task<IReadOnlyList<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

        Task<IDiscordUser> GetUserAsync(ulong id);

        Task SetGameAsync(int shardId, DiscordStatus status);

        Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null);

        Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message);

        Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null);
    }
}