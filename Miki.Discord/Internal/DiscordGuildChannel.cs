namespace Miki.Discord.Internal
{
    using System;
    using System.Collections.Generic;
    using Miki.Discord.Common;
    using System.Threading.Tasks;
    using Miki.Discord.Helpers;

    public class DiscordGuildChannel : DiscordChannel, IDiscordGuildChannel
    {
        public IEnumerable<PermissionOverwrite> PermissionOverwrites => packet.PermissionOverwrites;

        public DiscordGuildChannel(DiscordChannelPacket packet, IDiscordClient client)
            : base(packet, client)
        {
        }

        public ulong GuildId
            => packet.GuildId ?? throw new InvalidOperationException("Guild ID was invalid");

        public ChannelType Type
            => packet.Type;

        public async Task<IDiscordGuild> GetGuildAsync()
            => await client.GetGuildAsync(GuildId);

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

        public async Task<IDiscordGuildUser> GetSelfAsync()
        {
            var selfUser = await client.GetSelfAsync();
            return await GetUserAsync(selfUser.Id);
        }

        public async Task<IDiscordGuildUser> GetUserAsync(ulong id)
            => await client.GetGuildUserAsync(id, GuildId);
    }
}