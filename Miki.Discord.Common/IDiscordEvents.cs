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

        /// <summary>
        /// Emits events for guilds that were previously unavailable that have been loaded now.
        /// </summary>
        IObservable<IDiscordGuild> GuildAvailable { get; }

        /// <summary>
        /// Raw guild create call, will respond with every server.
        /// </summary>
        IObservable<IDiscordGuild> GuildCreate { get; }

        /// <summary>
        /// Emits events when the bot user joins a new guild.
        /// </summary>
        IObservable<IDiscordGuild> GuildJoin { get; }
        IObservable<IDiscordGuild> GuildLeave { get; }
        IObservable<IDiscordGuild> GuildUpdate { get; }
        IObservable<IDiscordGuild> GuildUnavailable { get; }
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
