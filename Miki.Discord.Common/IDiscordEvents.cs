using System;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets.Events;

namespace Miki.Discord.Events
{
    public interface IDiscordEvents : IDisposable
    {
        IObservable<IDiscordChannel> ChannelCreate { get; }
        IObservable<IDiscordChannel> ChannelDelete { get; }
        IObservable<IDiscordChannel> ChannelUpdate { get; }
        IObservable<IDiscordGuild> GuildCreate { get; }
        IObservable<IDiscordGuild> GuildDelete { get; }
        IObservable<IDiscordGuild> GuildUpdate { get; }
        IObservable<DiscordEmoji> GuildEmojiUpdate { get; }
        IObservable<IDiscordGuildUser> GuildMemberCreate { get; }
        IObservable<IDiscordGuildUser> GuildMemberDelete { get; }
        IObservable<IDiscordGuildUser> GuildMemberUpdate { get; }
        IObservable<IDiscordRole> GuildRoleCreate { get; }
        IObservable<IDiscordRole> GuildRoleDelete { get; }
        IObservable<IDiscordRole> GuildRoleUpdate { get; }
        IObservable<IDiscordMessage> MessageCreate { get; }
        IObservable<IDiscordMessage> MessageDelete { get; }
        IObservable<IDiscordMessage> MessageUpdate { get; }
        IObservable<IDiscordReaction> MessageReactionCreate { get; }
        IObservable<IDiscordReaction> MessageReactionDelete { get; }
        IObservable<IDiscordPresence> PresenceUpdate { get; }
        IObservable<TypingStartEventArgs> TypingStart { get; }
        IObservable<IDiscordUser> UserUpdate { get; }
        
        void SubscribeTo(IGateway gateway);
    }
}
