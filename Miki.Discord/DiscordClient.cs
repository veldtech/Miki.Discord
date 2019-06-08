using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;

namespace Miki.Discord
{
    public partial class DiscordClient : BaseDiscordClient
    {
        public DiscordClient(DiscordClientConfigurations config) 
            : this(config.ApiClient, config.Gateway, config.CacheClient)
        {
        }

        public DiscordClient(IApiClient apiClient, IGateway gateway, IExtendedCacheClient cacheClient) 
            : base(apiClient, gateway)
        {
            CacheClient = cacheClient;
            AttachHandlers();
        }

        public IExtendedCacheClient CacheClient { get; }

        protected override async Task<DiscordChannelPacket> GetChannelPacketAsync(ulong id, ulong? guildId = null)
        {
            var packet = await CacheClient.HashGetAsync<DiscordChannelPacket>(
                CacheUtils.ChannelsKey(guildId),
                id.ToString());

            if (packet == null)
            {
                packet = await ApiClient.GetChannelAsync(id);
                if (packet != null)
                {
                    await CacheClient.HashUpsertAsync(
                        CacheUtils.ChannelsKey(),
                        id.ToString(),
                        packet);
                }
            }
            return packet;
        }

        protected override async Task<bool> IsGuildNewAsync(ulong guildId)
        {
            return !await CacheClient.HashExistsAsync(CacheUtils.GuildsCacheKey, guildId.ToString());
        }

        protected override async Task<IEnumerable<DiscordGuildMemberPacket>> GetGuildMembersPacketAsync(ulong guildId)
        {
            IReadOnlyList<DiscordGuildMemberPacket> packets =
                (await CacheClient.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guildId)))?.ToArray();

            if (packets == null || packets.Count == 0)
            {
                packets = (await ApiClient.GetGuildAsync(guildId)).Members;

                if (packets.Count > 0)
                {
                    await CacheClient.HashUpsertAsync(
                        CacheUtils.ChannelsKey(guildId),
                        packets.Select(x => new KeyValuePair<string, DiscordGuildMemberPacket>(x.User.Id.ToString(), x)
                        ));
                }
            }

            return packets;
        }

        protected override async Task<IEnumerable<DiscordChannelPacket>> GetGuildChannelPacketsAsync(ulong guildId)
        {
            IReadOnlyList<DiscordChannelPacket> packets =
                (await CacheClient.HashValuesAsync<DiscordChannelPacket>(CacheUtils.ChannelsKey(guildId)))?.ToArray();

            if (packets == null || packets.Count == 0)
            {
                var result = await ApiClient.GetChannelsAsync(guildId);

                packets = result as IReadOnlyList<DiscordChannelPacket> ?? result.ToArray();

                if (packets.Any())
                {
                    await CacheClient.HashUpsertAsync(
                        CacheUtils.ChannelsKey(guildId),
                        packets.Select(x => new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x))
                    );
                }
            }

            return packets;
        }

        protected override async Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId)
        {
            DiscordGuildMemberPacket packet =
                await CacheClient.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guildId), userId.ToString());

            if (packet == null)
            {
                packet = await ApiClient.GetGuildUserAsync(userId, guildId);

                if (packet != null)
                {
                    packet.GuildId = guildId;

                    await CacheClient.HashUpsertAsync(CacheUtils.GuildMembersKey(guildId), userId.ToString(), packet);
                }
            }

            return packet;
        }

        protected override async Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId)
        {
            DiscordRolePacket packet =
                await CacheClient.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId), roleId.ToString());

            if (packet == null)
            {
                packet = await ApiClient.GetRoleAsync(roleId, guildId);

                if (packet != null)
                {
                    await CacheClient.HashUpsertAsync(CacheUtils.GuildRolesKey(guildId), roleId.ToString(), packet);
                }
            }

            return packet;
        }

        protected override async Task<IEnumerable<DiscordRolePacket>> GetRolePacketsAsync(ulong guildId)
        {
            IReadOnlyList<DiscordRolePacket> packets =
                (await CacheClient.HashValuesAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId)))?.ToArray();

            if (packets == null || packets.Count == 0)
            {
                var result = await ApiClient.GetRolesAsync(guildId);

                packets = result as IReadOnlyList<DiscordRolePacket> ?? result.ToArray();

                if (packets.Count > 0)
                {
                    await CacheClient.HashUpsertAsync(
                        CacheUtils.ChannelsKey(guildId),
                        packets.Select(x => new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x)
                    ));
                }
            }

            return packets;
        }

        protected override async Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
        {
            DiscordGuildPacket packet = await CacheClient.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey, id.ToString());

            if (packet == null)
            {
                packet = await ApiClient.GetGuildAsync(id);

                if (packet != null)
                {
                    await CacheClient.HashUpsertAsync(CacheUtils.GuildsCacheKey, id.ToString(), packet);
                }
            }

            return packet;
        }

        protected override async Task<DiscordUserPacket> GetUserPacketAsync(ulong id)
        {
            DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, id.ToString());

            if (packet == null)
            {
                packet = await ApiClient.GetUserAsync(id);
                if (packet != null)
                {
                    await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey, id.ToString(), packet);
                }
            }

            return packet;
        }

        protected override async Task<DiscordUserPacket> GetCurrentUserPacketAsync()
        {
            DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, "me");

            if (packet == null)
            {
                packet = await ApiClient.GetCurrentUserAsync();
                if (packet != null)
                {
                    await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey, "me", packet);
                }
            }

            return packet;
        }

        public override void Dispose()
        {
            base.Dispose();
            DetachHandlers();
        }
    }
}
