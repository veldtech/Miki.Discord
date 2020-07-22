using System.Collections.Generic;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Patterns.Repositories;

namespace Miki.Discord.Cache
{
    /// <summary>
    /// Cache service for Miki.Discord.
    /// </summary>
    public interface ICacheHandler
    {
        /// <summary>
        /// Channel repository
        /// </summary>
        IAsyncRepository<DiscordChannelPacket> Channels { get; }
        
        /// <summary>
        /// Guild repository
        /// </summary>
        IAsyncRepository<DiscordGuildPacket> Guilds { get; }
        
        /// <summary>
        /// Guild Member repository
        /// </summary>
        IAsyncRepository<DiscordGuildMemberPacket> Members { get; }
        
        /// <summary>
        /// Guild Role repository
        /// </summary>
        IAsyncRepository<DiscordRolePacket> Roles { get; }
        
        /// <summary>
        /// User repository
        /// </summary>
        IAsyncRepository<DiscordUserPacket> Users { get; }
        
        /// <summary>
        /// Gets the current bot connected to the gateway.
        /// </summary>
        ValueTask<DiscordUserPacket> GetCurrentUserAsync();
        ValueTask<IReadOnlyList<DiscordChannelPacket>> GetChannelsFromGuildAsync(ulong guildId);
        ValueTask<IReadOnlyList<DiscordGuildMemberPacket>> GetMembersFromGuildAsync(ulong guildId);
        ValueTask<IReadOnlyList<DiscordRolePacket>> GetRolesFromGuildAsync(ulong guildId);
        
        /// <summary>
        /// Sets the current user connected to the gateway.
        /// </summary>
        ValueTask SetCurrentUserAsync(DiscordUserPacket user);
    }
}
