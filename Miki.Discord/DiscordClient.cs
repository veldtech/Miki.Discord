#nullable enable

using Microsoft.Extensions.Hosting;
using Miki.Cache;
using Miki.Discord.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Events;
using Miki.Discord.Helpers;
using Miki.Discord.Internal.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Discord
{
    public class DiscordClient : IDiscordClient, IHostedService
    {
        /// <summary>
        /// The api client used in the discord client and was given in 
        /// <see cref="DiscordClientConfiguration"/>
        /// at the beginning.
        /// </summary>
        public IApiClient ApiClient { get; }

        /// <summary>
        /// The gateway client used in the discord client and was given in 
        /// <see cref="DiscordClientConfiguration"/>
        /// at the beginning.
        /// </summary>
        public IGateway Gateway { get; }

        /// <summary>
        /// Managed gateway events.
        /// </summary>
        public IDiscordEvents Events { get; }

        private readonly ICacheHandler cacheHandler;

        private readonly EventCacheHandler eventCacheHandler;

        /// <summary>
        /// Creates a new discord client.
        /// </summary>
        public DiscordClient(DiscordClientConfiguration configurations)
            : this(configurations.ApiClient, configurations.Gateway, configurations.CacheClient)
        {
        }

        /// <summary>
        /// Constructs a default implementation of Miki.Discord for dependency injection.
        /// </summary>
        public DiscordClient(IApiClient apiClient, IGateway gateway, IExtendedCacheClient cacheClient)
        {
            ApiClient = apiClient;
            Gateway = gateway;
            cacheHandler = new DefaultCacheHandler(cacheClient, apiClient);

            Events = new DiscordEventHandler(this, cacheHandler);
            Events.SubscribeTo(Gateway);

            eventCacheHandler = new EventCacheHandler(gateway, cacheHandler);
        }

        /// <summary>
        /// Constructs a fully customized implementation of Miki.Discord.
        /// </summary>
        /// <param name="apiClient"></param>
        /// <param name="gateway"></param>
        /// <param name="cacheHandler"></param>
        /// <param name="eventHandler"></param>
        public DiscordClient(
            IApiClient apiClient, 
            IGateway gateway, 
            ICacheHandler cacheHandler,
            IDiscordEvents eventHandler)
        {
            ApiClient = apiClient;
            Gateway = gateway;
            Events = eventHandler;
            this.cacheHandler = cacheHandler;
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordTextChannel> CreateDMAsync(ulong userid)
        {
            var channel = await ApiClient.CreateDMChannelAsync(userid);
            if(!(AbstractionHelpers.ResolveChannel(this, channel) is IDiscordTextChannel textChannel))
            {
                throw new InvalidDataException("DM channel was not a text channel");
            }

            return textChannel;
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs? args)
        {
            return new DiscordRole(
                await ApiClient.CreateGuildRoleAsync(guildId, args),
                this
            );
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            eventCacheHandler.Dispose();
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordMessage> EditMessageAsync(
            ulong channelId, ulong messageId, string text, DiscordEmbed? embed)
        {
            return AbstractionHelpers.ResolveMessage(this, await ApiClient.EditMessageAsync(
                channelId, messageId, new EditMessageArgs
                {
                    Content = text,
                    Embed = embed
                }));
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            return new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordPresence?> GetUserPresence(
            ulong userId, ulong? guildId = null)
        {
            if(!guildId.HasValue)
            {
                throw new NotSupportedException(
                    @"The default Discord Client cannot get the presence of 
the user without the guild ID. Use the cached client instead.");
            }

            // We have to get the guild because there is no API end-point for user presence.
            // This is a known issue: https://github.com/discordapp/discord-api-docs/issues/666

            var guild = await cacheHandler.Guilds.GetAsync(guildId.Value);
            var presence = guild.Presences.FirstOrDefault(p => p.User.Id == userId);
            return presence != null ? new DiscordPresence(presence, this) : null;
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordRole?> GetRoleAsync(ulong guildId, ulong roleId)
        {
            var role = await cacheHandler.Roles.GetAsync(roleId, guildId);
            if(role == null)
            {
                return null;
            }

            return new DiscordRole(role, this);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId)
        {
            return (await cacheHandler.GetRolesFromGuildAsync(guildId))
                .Select(x => new DiscordRole(x, this))
                .ToList();
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
        {
            var channelPackets = await cacheHandler.GetChannelsFromGuildAsync(guildId);
            if(channelPackets == null)
            {
                return new List<IDiscordGuildChannel>();
            }

            return channelPackets
                .Select(x => AbstractionHelpers.ResolveChannelAs<IDiscordGuildChannel>(this, x))
                .Where(x => x != null);
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
        {
            DiscordChannelPacket channel = (guildId == null) ? (await cacheHandler.Channels.GetAsync(id)) : (await cacheHandler.Channels.GetAsync(id, guildId));
            return AbstractionHelpers.ResolveChannel(this, channel);
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordSelfUser> GetSelfAsync()
            => new DiscordSelfUser(await cacheHandler.GetCurrentUserAsync(), this);

        /// <inheritdoc/>
        public virtual async Task<IDiscordGuild?> GetGuildAsync(ulong id)
        {
            var packet = await cacheHandler.Guilds.GetAsync(id);
            if(packet == null)
            {
                return null;
            }

            return new DiscordGuild(packet, this);
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
        {
            return new DiscordGuildUser(await cacheHandler.Members.GetAsync(id, guildId), this);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
        {
            return (await cacheHandler.GetMembersFromGuildAsync(guildId))
                .Select(x => new DiscordGuildUser(x, this));
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(
            ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            var users = await ApiClient.GetReactionsAsync(channelId, messageId, emoji);
            if(users == null)
            {
                return new List<IDiscordUser>();
            }

            return users.Where(x => x != null).Select(x => new DiscordUser(x, this));
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordUser?> GetUserAsync(ulong id)
        {
            var packet = await cacheHandler.Users.GetAsync(id);
            if(packet == null)
            {
                return null;
            }

            return new DiscordUser(packet, this);
        }

        /// <inheritdoc/>
        public virtual async Task SetGameAsync(int shardId, DiscordStatus status)
        {
            await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
        }

        /// <inheritdoc/>
        public virtual async Task<IDiscordMessage> SendFileAsync(
            ulong channelId, Stream stream, string fileName, MessageArgs? message)
            => AbstractionHelpers.ResolveMessage(
                this, await ApiClient.SendFileAsync(channelId, stream, fileName, message));

        /// <inheritdoc/>
        public virtual async Task<IDiscordMessage> SendMessageAsync(
            ulong channelId, MessageArgs message)
            => AbstractionHelpers.ResolveMessage(
                this, await ApiClient.SendMessageAsync(channelId, message));

        /// <inheritdoc/>
        public virtual async Task<IDiscordMessage> SendMessageAsync(
            ulong channelId, string text, DiscordEmbed? embed)
        {
            return await SendMessageAsync(channelId, new MessageArgs
            {
                Content = text,
                Embed = embed
            });
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken token)
        {
            await Gateway.StartAsync(token);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken token)
        {
            await Gateway.StopAsync(token);
        }
    }
}