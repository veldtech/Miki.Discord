namespace Miki.Discord
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Gateway;
    using Miki.Discord.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Miki.Discord.Common.Packets.API;
    using Miki.Discord.Events;
    using Miki.Discord.Cache;

    public abstract class BaseDiscordClient : IDiscordClient
    {
        /// <summary>
        /// The api client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        public IApiClient ApiClient { get; }

        /// <summary>
        /// The gateway client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
        /// </summary>
        public IGateway Gateway { get; }

        public IDiscordEvents Events { get; }

        private readonly ICacheHandler cache;

        protected BaseDiscordClient(
            IApiClient apiClient, IGateway gateway, ICacheHandler cache)
        {
            this.cache = cache;

            ApiClient = apiClient;
            Gateway = gateway;
            Events = new DiscordClientEventHandler(cache);
        }

        public virtual async Task<IDiscordMessage> EditMessageAsync(
            ulong channelId, ulong messageId, string text, DiscordEmbed embed = null)
        {
            return ResolveMessage(await ApiClient.EditMessageAsync(
                channelId, messageId, new EditMessageArgs
                {
                    Content = text,
                    Embed = embed
                }));
        }

        public virtual async Task<IDiscordTextChannel> CreateDMAsync(ulong userid)
        {
            var channel = await ApiClient.CreateDMChannelAsync(userid);

            return ResolveChannel(channel) as IDiscordTextChannel;
        }

        public virtual async Task<IDiscordRole> CreateRoleAsync(
            ulong guildId, CreateRoleArgs args = null)
        {
            return new DiscordRole(
                await ApiClient.CreateGuildRoleAsync(guildId, args),
                this
            );
        }

        public virtual async Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            return new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);
        }

        public virtual async Task<IDiscordPresence> GetUserPresence(
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

            var guild = await GetGuildPacketAsync(guildId.Value);
            var presence = guild.Presences.FirstOrDefault(p => p.User.Id == userId);
            return presence != null ? new DiscordPresence(presence) : null;
        }

        public virtual async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
        {
            return new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);
        }

        public virtual async Task<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId)
        {
            return (await GetRolePacketsAsync(guildId))
                .Select(x => new DiscordRole(x, this))
                .ToList();
        }

        public virtual async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
        {
            var channelPackets = await GetGuildChannelPacketsAsync(guildId);
            return channelPackets.Select(x => ResolveChannel(x) as IDiscordGuildChannel);
        }

        public virtual async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
        {
            var channel = await GetChannelPacketAsync(id, guildId);

            return ResolveChannel(channel);
        }

        public virtual async Task<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) 
            where T : class, IDiscordChannel
        {
            var channel = await GetChannelPacketAsync(id, guildId);

            return ResolveChannel(channel) as T;
        }

        public virtual async Task<IDiscordSelfUser> GetSelfAsync()
        {
            return new DiscordSelfUser(
                await GetCurrentUserPacketAsync(),
                this);
        }

        public virtual async Task<IDiscordGuild> GetGuildAsync(ulong id)
        {
            var packet = await GetGuildPacketAsync(id);
            return new DiscordGuild(packet, this);
        }

        public virtual async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
        {
            return new DiscordGuildUser(await GetGuildMemberPacketAsync(id, guildId), this);
        }

        public async Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
        {
            return (await GetGuildMembersPacketAsync(guildId))
                .Select(x => new DiscordGuildUser(x, this));
        }

        public virtual async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(
            ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            var users = await ApiClient.GetReactionsAsync(channelId, messageId, emoji);

            if(users != null)
            {
                return users.Select(
                    x => new DiscordUser(x, this)
                );
            }

            return new List<IDiscordUser>();
        }

        public virtual async Task<IDiscordUser> GetUserAsync(ulong id)
        {
            var packet = await GetUserPacketAsync(id);

            return new DiscordUser(
                packet,
                this
            );
        }

        public virtual async Task SetGameAsync(
            int shardId, DiscordStatus status)
        {
            await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
        }

        public virtual async Task<IDiscordMessage> SendFileAsync(
            ulong channelId, Stream stream, string fileName, MessageArgs message = null)
            => ResolveMessage(await ApiClient.SendFileAsync(channelId, stream, fileName, message));

        public virtual async Task<IDiscordMessage> SendMessageAsync(
            ulong channelId, MessageArgs message)
            => ResolveMessage(await ApiClient.SendMessageAsync(channelId, message));

        public virtual async Task<IDiscordMessage> SendMessageAsync(
            ulong channelId, string text, DiscordEmbed embed = null)
            => await SendMessageAsync(channelId, new MessageArgs
            {
                Content = text,
                Embed = embed
            });

        protected IDiscordMessage ResolveMessage(DiscordMessagePacket packet)
        {
            if(packet.GuildId.HasValue)
            {
                return new DiscordGuildMessage(packet, this);
            }
            return new DiscordMessage(packet, this);
        }

        protected IDiscordChannel ResolveChannel(DiscordChannelPacket packet)
        {
            switch(packet.Type)
            {
                case ChannelType.GuildText:
                    return new DiscordGuildTextChannel(packet, this);

                default:
                    return new DiscordGuildChannel(packet, this);

                case ChannelType.DirectText:
                case ChannelType.GroupDirect:
                    return new DiscordTextChannel(packet, this);
            }
        }

        public virtual void Dispose()
        {
        }
    }
}