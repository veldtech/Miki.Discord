#nullable enable
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Miki.Discord.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
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
        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelCreate => channelCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelDelete => channelDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordChannel> ChannelUpdate => channelUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildAvailable => guildAvailable;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildJoin => guildJoin;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildLeave => guildLeave;
        
        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildUnavailable => guildUnavailable;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildCreate => guildCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildDelete => guildDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordGuild> GuildUpdate => guildUpdate;

        /// <inheritdoc/>
        public IObservable<DiscordEmoji> GuildEmojiUpdate => guildEmojiUpdate;

        public IObservable<IDiscordGuildUser> GuildMemberCreate => guildMemberCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordGuildUser> GuildMemberDelete => guildMemberDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordGuildUser> GuildMemberUpdate => guildMemberUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleCreate => guildRoleCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleDelete => guildRoleDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordRole> GuildRoleUpdate => guildRoleUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageCreate => messageCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageDelete => messageDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordMessage> MessageUpdate => messageUpdate;

        /// <inheritdoc/>
        public IObservable<IDiscordReaction> MessageReactionCreate => messageReactionCreate;

        /// <inheritdoc/>
        public IObservable<IDiscordReaction> MessageReactionDelete => messageReactionDelete;

        /// <inheritdoc/>
        public IObservable<IDiscordPresence> PresenceUpdate => presenceUpdate;

        /// <inheritdoc/>
        public IObservable<TypingStartEventArgs> TypingStart => typingStart;

        /// <inheritdoc/>
        public IObservable<IDiscordUser> UserUpdate => userUpdate;

        private readonly Subject<IDiscordChannel> channelCreate;
        private readonly Subject<IDiscordChannel> channelDelete;
        private readonly Subject<IDiscordChannel> channelUpdate;
        private readonly Subject<IDiscordGuild> guildAvailable;
        private readonly Subject<IDiscordGuild> guildJoin;
        private readonly Subject<IDiscordGuild> guildLeave;
        private readonly Subject<IDiscordGuild> guildUnavailable;
        private readonly Subject<IDiscordGuild> guildCreate;
        private readonly Subject<IDiscordGuild> guildDelete;
        private readonly Subject<IDiscordGuild> guildUpdate;
        private readonly Subject<DiscordEmoji> guildEmojiUpdate;
        private readonly Subject<IDiscordGuildUser> guildMemberCreate;
        private readonly Subject<IDiscordGuildUser> guildMemberDelete;
        private readonly Subject<IDiscordGuildUser> guildMemberUpdate;
        private readonly Subject<IDiscordRole> guildRoleCreate;
        private readonly Subject<IDiscordRole> guildRoleDelete;
        private readonly Subject<IDiscordRole> guildRoleUpdate;
        private readonly Subject<IDiscordMessage> messageCreate;
        private readonly Subject<IDiscordMessage> messageDelete;
        private readonly Subject<IDiscordMessage> messageUpdate;
        private readonly Subject<IDiscordReaction> messageReactionCreate;
        private readonly Subject<IDiscordReaction> messageReactionDelete;
        private readonly Subject<IDiscordPresence> presenceUpdate;
        private readonly Subject<TypingStartEventArgs> typingStart;
        private readonly Subject<IDiscordUser> userUpdate;

        private readonly IDiscordClient client;
        private readonly ICacheHandler cacheHandler;

        public DiscordEventHandler(IDiscordClient client, ICacheHandler cacheHandler)
        {
            this.client = client;
            this.cacheHandler = cacheHandler;

            channelCreate = new Subject<IDiscordChannel>();
            channelDelete = new Subject<IDiscordChannel>();
            channelUpdate = new Subject<IDiscordChannel>();
            guildAvailable = new Subject<IDiscordGuild>();
            guildCreate = new Subject<IDiscordGuild>();
            guildDelete = new Subject<IDiscordGuild>();
            guildJoin = new Subject<IDiscordGuild>();
            guildLeave = new Subject<IDiscordGuild>();
            guildUpdate = new Subject<IDiscordGuild>();
            guildUnavailable = new Subject<IDiscordGuild>();
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
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelCreate.OnNext);
            
            gateway.Events.ChannelDelete
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelDelete.OnNext);
            
            gateway.Events.ChannelUpdate
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveChannel(client, x))
                .Subscribe(channelUpdate.OnNext);

            gateway.Events.GuildCreate
                .WhereNotNull()
                .SubscribeTask(OnGuildCreate);
            
            gateway.Events.GuildDelete
                .WhereNotNull()
                .SubscribeTask(OnGuildDelete);
            
            gateway.Events.GuildUpdate
                .WhereNotNull()
                .Select(x => new DiscordGuild(x, client))
                .Subscribe(guildCreate.OnNext);

            gateway.Events.GuildMemberCreate
                .WhereNotNull()
                .Select(x => new DiscordGuildUser(x, client))
                .Subscribe(guildMemberCreate.OnNext);
            
            gateway.Events.GuildMemberDelete
                .WhereNotNull()
                .SubscribeTask(async x =>
                {
                    var guildUser = await cacheHandler.Members.GetAsync(x.User.Id, x.GuildId);
                    guildMemberDelete.OnNext(new DiscordGuildUser(guildUser, client));
                });
            
            gateway.Events.GuildMemberUpdate
                .WhereNotNull()
                .SubscribeTask(async x =>
                {
                    var guildUser = await cacheHandler.Members.GetAsync(x.User.Id, x.GuildId);
                    guildMemberDelete.OnNext(new DiscordGuildUser(guildUser, client));
                });

            gateway.Events.MessageCreate
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveMessage(client, x))
                .Subscribe(messageCreate.OnNext);
            
            gateway.Events.MessageDelete
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveMessage(client,
                    new DiscordMessagePacket
                    {
                        Id = x.MessageId,
                        ChannelId = x.ChannelId
                    }))
                .Subscribe(messageDelete.OnNext);
            
            gateway.Events.MessageUpdate
                .WhereNotNull()
                .Select(x => AbstractionHelpers.ResolveMessage(client, x))
                .Subscribe(messageUpdate.OnNext);

            gateway.Events.MessageReactionCreate
                .WhereNotNull()
                .Select(x => new DiscordReaction(x, client))
                .Subscribe(messageReactionCreate.OnNext);
            
            gateway.Events.MessageReactionDelete
                .WhereNotNull()
                .Select(x => new DiscordReaction(x, client))
                .Subscribe(messageReactionDelete.OnNext);

            gateway.Events.PresenceUpdate
                .WhereNotNull()
                .Select(x => new DiscordPresence(x, client))
                .Subscribe(presenceUpdate.OnNext);

            gateway.Events.TypingStart
                .WhereNotNull()
                .Subscribe(typingStart.OnNext);

            gateway.Events.UserUpdate
                .WhereNotNull()
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

        private async Task OnGuildCreate(DiscordGuildPacket guild)
        { 
            var managedGuild = new DiscordGuild(guild, client);
            guildCreate.OnNext(managedGuild);
            if (!await cacheHandler.HasGuildAsync(guild.Id)
                && !(guild.Unavailable ?? true))
            {
                guildJoin.OnNext(managedGuild);
            }
            else
            {
                guildAvailable.OnNext(managedGuild);
            }
        }

        private async Task OnGuildDelete(DiscordGuildUnavailablePacket x)
        {
            var packet = await cacheHandler.Guilds.GetAsync(x.GuildId);
            var guild = new DiscordGuild(packet, client);

            guildDelete.OnNext(guild);
            if (x.IsUnavailable.HasValue && x.IsUnavailable.Value)
            {
                guildUnavailable.OnNext(guild);
            }
            else
            {
                guildLeave.OnNext(guild);
            }
        }
    }
}