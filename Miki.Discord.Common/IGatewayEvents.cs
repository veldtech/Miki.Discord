using System;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.API;
using Miki.Discord.Common.Packets.Events;

namespace Miki.Discord.Common
{
    public interface IGatewayEvents
    {
        IObservable<DiscordChannelPacket> ChannelCreate { get; }
        IObservable<DiscordChannelPacket> ChannelDelete { get; }
        IObservable<DiscordChannelPacket> ChannelUpdate { get; }
        IObservable<DiscordGuildPacket> GuildCreate { get; }
        IObservable<DiscordGuildUnavailablePacket> GuildDelete { get; }
        IObservable<DiscordGuildPacket> GuildUpdate { get; }
        IObservable<GuildEmojisUpdateEventArgs> GuildEmojiUpdate { get; }
        IObservable<DiscordGuildMemberPacket> GuildMemberCreate { get; }
        IObservable<GuildIdUserArgs> GuildMemberDelete { get; }
        IObservable<GuildMemberUpdateEventArgs> GuildMemberUpdate { get; }
        IObservable<RoleEventArgs> GuildRoleCreate { get; }
        IObservable<RoleDeleteEventArgs> GuildRoleDelete { get; }
        IObservable<RoleEventArgs> GuildRoleUpdate { get; }
        IObservable<DiscordMessagePacket> MessageCreate { get; }
        IObservable<DiscordMessageDeleteArgs> MessageDelete { get; }
        IObservable<DiscordMessagePacket> MessageUpdate { get; }
        IObservable<DiscordReactionPacket> MessageReactionCreate { get; }
        IObservable<DiscordReactionPacket> MessageReactionDelete { get; }
        IObservable<DiscordPresencePacket> PresenceUpdate { get; }
        IObservable<GatewayReadyPacket> Ready { get; }
        IObservable<TypingStartEventArgs> TypingStart { get; }
        IObservable<DiscordUserPacket> UserUpdate { get; }
    }
}