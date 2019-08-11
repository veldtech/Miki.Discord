using System.Collections.Generic;
using Miki.Discord.Common;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Helpers;

namespace Miki.Discord.Internal
{
    public class DiscordGuildChannel : DiscordChannel, IDiscordGuildChannel
    {
        public IEnumerable<PermissionOverwrite> PermissionOverwrites => _packet.PermissionOverwrites;

        public DiscordGuildChannel(DiscordChannelPacket packet, IDiscordClient client)
            : base(packet, client)
        {
        }

        public ulong GuildId
            => _packet.GuildId.Value;

        public ChannelType Type
            => _packet.Type;

        public async Task<IDiscordGuild> GetGuildAsync()
            => await _client.GetGuildAsync(GuildId);

        public async Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user = null)
        {
            IDiscordGuild guild = await GetGuildAsync();
            if(user == null)
            {
                user = await guild.GetSelfAsync();
            }
            GuildPermission permissions = await guild.GetPermissionsAsync(user);
            return DiscordChannelHelper.GetOverwritePermissions(user, this, permissions);
        }

        public async Task<IDiscordGuildUser> GetUserAsync(ulong id)
            => await _client.GetGuildUserAsync(id, GuildId);
    }
}