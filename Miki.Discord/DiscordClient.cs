using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;

namespace Miki.Discord
{
    public class DiscordClient : BaseDiscordClient
    {
        private readonly ConcurrentDictionary<ulong, bool> _seenGuilds = new ConcurrentDictionary<ulong, bool>();

        public DiscordClient(DiscordClientConfigurations config) : base(config)
        {
        }

        protected override Task<DiscordUserPacket> GetCurrentUserPacketAsync()
        {
            return ApiClient.GetCurrentUserAsync();
        }

        protected override Task<DiscordUserPacket> GetUserPacketAsync(ulong id)
        {
            return ApiClient.GetUserAsync(id);
        }

        protected override async Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
        {
            var guild = await ApiClient.GetGuildAsync(id);

            if (!_seenGuilds.ContainsKey(id))
            {
                _seenGuilds.AddOrUpdate(id, true, (_, b) => b);
            }

            return guild;
        }

        protected override Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId)
        {
            return ApiClient.GetGuildUserAsync(userId, guildId);
        }

        protected override async Task<IReadOnlyList<DiscordGuildMemberPacket>> GetGuildMembersPacketAsync(ulong guildId)
        {
            return (await ApiClient.GetGuildAsync(guildId)).Members;
        }

        protected override Task<IReadOnlyList<DiscordChannelPacket>> GetGuildChannelPacketsAsync(ulong guildId)
        {
            return ApiClient.GetChannelsAsync(guildId);
        }

        protected override Task<IReadOnlyList<DiscordRolePacket>> GetRolePacketsAsync(ulong guildId)
        {
            return ApiClient.GetRolesAsync(guildId);
        }

        protected override Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId)
        {
            return ApiClient.GetRoleAsync(roleId, guildId);
        }

        protected override Task<DiscordChannelPacket> GetChannelPacketAsync(ulong id, ulong? guildId = null)
        {
            return ApiClient.GetChannelAsync(id);
        }

        protected override Task<bool> IsGuildNewAsync(ulong guildId)
        {
            return Task.FromResult(!_seenGuilds.ContainsKey(guildId));
        }
    }
}
