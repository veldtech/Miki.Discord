using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Miki.Discord.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets.API;
using Miki.Discord.Common.Packets.Events;
using Miki.Discord.Helpers;
using Miki.Discord.Internal.Data;

namespace Miki.Discord.Events
{
    /// <summary>
    /// Wraps gateway events into managed events.
    /// </summary>
    public class DiscordEventHandler : IDiscordEvents
    {
        private readonly Subject<IDiscordChannel> channelCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelCreate => channelCreate;
        
        private readonly Subject<IDiscordChannel> channelDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelDelete => channelDelete;

        private readonly Subject<IDiscordChannel> channelUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelUpdate => channelUpdate;
        
        private readonly Subject<IDiscordGuild> guildCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildCreate => guildCreate;
        
        private readonly Subject<IDiscordGuild> guildDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildDelete => guildDelete;

        private readonly Subject<IDiscordGuild> guildUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildUpdate => guildUpdate;

        private readonly Subject<DiscordEmoji> guildEmojiUpdate;

        /// <inheritdoc/>
        public IObservable<DiscordEmoji> GuildEmojiUpdate => guildEmojiUpdate;
        

        /// <inheritdoc/>
        private readonly Subject<IDiscordGuildUser> guildMemberCreate;
        public IObservable<IDiscordGuildUser> GuildMemberCreate => guildMemberCreate;

        private readonly Subject<IDiscordGuildUser> guildMemberDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordGuildUser> GuildMemberDelete => guildMemberDelete;
        
        private readonly Subject<IDiscordGuildUser> guildMemberUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuildUser> GuildMemberUpdate => guildMemberUpdate;
        
        private readonly Subject<IDiscordRole> guildRoleCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleCreate => guildRoleCreate;
        
        private readonly Subject<IDiscordRole> guildRoleDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleDelete => guildRoleDelete;
        
        private readonly Subject<IDiscordRole> guildRoleUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleUpdate => guildRoleUpdate;
        
        private readonly Subject<IDiscordMessage> messageCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageCreate => messageCreate;

        private readonly Subject<IDiscordMessage> messageDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageDelete => messageDelete;
        
        private readonly Subject<IDiscordMessage> messageUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageUpdate => messageUpdate;
        
        private readonly Subject<IDiscordReaction> messageReactionCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordReaction> MessageReactionCreate => messageReactionCreate;
        
        private readonly Subject<IDiscordReaction> messageReactionDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordReaction> MessageReactionDelete => messageReactionDelete;
        
        private readonly Subject<IDiscordPresence> presenceUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordPresence> PresenceUpdate => presenceUpdate;
        
        private readonly Subject<TypingStartEventArgs> typingStart;

        /// <inheritdoc/>
        public IObservable<TypingStartEventArgs> TypingStart => typingStart;
        
        private readonly Subject<IDiscordUser> userUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordUser> UserUpdate => userUpdate;

        private readonly IDiscordClient client;
        private readonly ICacheHandler cacheHandler;

        public DiscordEventHandler(IDiscordClient client, ICacheHandler cacheHandler)
        {
            this.client = client;
            this.cacheHandler = cacheHandler;

            channelCreate = new Subject<IDiscordChannel>();
            channelDelete = new Subject<IDiscordChannel>();
            channelUpdate = new Subject<IDiscordChannel>();
            guildCreate = new Subject<IDiscordGuild>();
            guildDelete = new Subject<IDiscordGuild>();
            guildUpdate = new Subject<IDiscordGuild>();
            guildEmojiUpdate = new Subject<DiscordEmoji>();
            guildMemberCreate = new Subject<IDiscordGuildUser>();
            guildMemberDelete = new Subject<IDiscordGuildUser>();
            guildMemberUpdate = new Subject<IDiscordGuildUser>();
            guildRoleCreate = new Subject<IDiscordRole>();
            guildRoleDelete = new Subject<IDiscordRole>();
            guildRoleUpdate = new Subject<IDiscordRole>();
            messageCreate = new Subject<IDiscordMessage>();
            messageDelete = new Subject<IDiscordMessage>();
            messageUpdate = new Subject<IDiscordMessage>();
            messageReactionCreate = new Subject<IDiscordReaction>();
            messageReactionDelete = new Subject<IDiscordReaction>();
            presenceUpdate = new Subject<IDiscordPresence>();
            typingStart = new Subject<TypingStartEventArgs>();
            userUpdate = new Subject<IDiscordUser>();
        }

        public void SubscribeTo(IGateway gateway)
        {
            gateway.Events.ChannelCreate
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelCreate.OnNext);
            
            gateway.Events.ChannelDelete
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelDelete.OnNext);
            
            gateway.Events.ChannelUpdate
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelUpdate.OnNext);

            gateway.Events.GuildCreate
                .Select(x => new DiscordGuild(x, client))
                .Subscribe(guildCreate.OnNext);
            
            gateway.Events.GuildDelete
                .SubscribeTask(async x =>
                {
                    var guild = await cacheHandler.Guilds.GetAsync(x.GuildId);
                    guildDelete.OnNext(new DiscordGuild(guild, client));
                });
            
            gateway.Events.GuildUpdate
                .Select(x => new DiscordGuild(x, client))
                .Subscribe(guildCreate.OnNext);

            gateway.Events.GuildMemberCreate
                .Select(x => new DiscordGuildUser(x, client))
                .Subscribe(guildMemberCreate.OnNext);
            
            gateway.Events.GuildMemberDelete
                .SubscribeTask(async x =>
                {
                    var guildUser = await cacheHandler.Members.GetAsync(x.User.Id, x.GuildId);
                    guildMemberDelete.OnNext(new DiscordGuildUser(guildUser, client));
                });
            
            gateway.Events.GuildMemberUpdate
                .SubscribeTask(async x =>
                {
                    var guildUser = await cacheHandler.Members.GetAsync(x.User.Id, x.GuildId);
                    guildMemberDelete.OnNext(new DiscordGuildUser(guildUser, client));
                });

            gateway.Events.MessageCreate
                .Select(x => AbstractionHelpers.ResolveMessage(client, x))
                .Subscribe(messageCreate.OnNext);
            
            gateway.Events.MessageDelete
                .Select(x => AbstractionHelpers.ResolveMessage(client,
                    new DiscordMessagePacket
                    {
                        Id = x.MessageId,
                        ChannelId = x.ChannelId
                    }))
                .Subscribe(messageDelete.OnNext);
            
            gateway.Events.MessageUpdate
                .Select(x => AbstractionHelpers.ResolveMessage(client, x))
                .Subscribe(messageCreate.OnNext);

            gateway.Events.MessageReactionCreate
                .Select(x => new DiscordReaction(x, client))
                .Subscribe(messageReactionCreate.OnNext);
            
            gateway.Events.MessageReactionDelete
                .Select(x => new DiscordReaction(x, client))
                .Subscribe(messageReactionDelete.OnNext);

            gateway.Events.PresenceUpdate
                .Select(x => new DiscordPresence(x, client))
                .Subscribe(presenceUpdate.OnNext);

            gateway.Events.TypingStart
                .Subscribe(typingStart.OnNext);

            gateway.Events.UserUpdate
                .Select(x => new DiscordUser(x, client))
                .Subscribe(userUpdate.OnNext);
        }

        public void Dispose()
        {
            channelCreate?.Dispose();
            channelDelete?.Dispose();
            channelUpdate?.Dispose();
            guildCreate?.Dispose();
            guildDelete?.Dispose();
            guildUpdate?.Dispose();
            guildEmojiUpdate?.Dispose();
            guildMemberCreate?.Dispose();
            guildMemberDelete?.Dispose();
            guildMemberUpdate?.Dispose();
            guildRoleCreate?.Dispose();
            guildRoleDelete?.Dispose();
            guildRoleUpdate?.Dispose();
            messageCreate?.Dispose();
            messageDelete?.Dispose();
            messageUpdate?.Dispose();
            messageReactionCreate?.Dispose();
            messageReactionDelete?.Dispose();
            presenceUpdate?.Dispose();
            typingStart?.Dispose();
            userUpdate?.Dispose();
        }
    }
}