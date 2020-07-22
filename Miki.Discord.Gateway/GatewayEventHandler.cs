using System;
using System.Reactive.Subjects;
using System.Text.Json;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.API;
using Miki.Discord.Common.Packets.Events;
using Miki.Logging;

namespace Miki.Discord.Gateway
{
    public class GatewayEventHandler : IGatewayEvents
    {
        /// <inheritdoc/>
        public IObservable<DiscordChannelPacket> ChannelCreate => channelCreateSubject;
        /// <inheritdoc/>
        public IObservable<DiscordChannelPacket> ChannelDelete => channelDeleteSubject;
        /// <inheritdoc/>
        public IObservable<DiscordChannelPacket> ChannelUpdate => channelUpdateSubject;
        /// <inheritdoc/>
        public IObservable<DiscordGuildPacket> GuildCreate => guildCreateSubject;
        /// <inheritdoc/>
        public IObservable<DiscordGuildUnavailablePacket> GuildDelete => guildDeleteSubject;
        /// <inheritdoc/>
        public IObservable<DiscordGuildPacket> GuildUpdate => guildUpdateSubject;

        /// <inheritdoc />
        public IObservable<GuildEmojisUpdateEventArgs> GuildEmojiUpdate => guildEmojiUpdateSubject;

        /// <inheritdoc/>
        public IObservable<DiscordGuildMemberPacket> GuildMemberCreate => guildMemberCreateSubject;
        /// <inheritdoc/>
        public IObservable<GuildIdUserArgs> GuildMemberDelete => guildMemberDeleteSubject;
        /// <inheritdoc/>
        public IObservable<GuildMemberUpdateEventArgs> GuildMemberUpdate => guildMemberUpdateSubject;

        /// <inheritdoc />
        public IObservable<RoleEventArgs> GuildRoleCreate => guildRoleCreateSubject;

        /// <inheritdoc />
        public IObservable<RoleDeleteEventArgs> GuildRoleDelete => guildRoleDeleteSubject;

        /// <inheritdoc />
        public IObservable<RoleEventArgs> GuildRoleUpdate => guildRoleUpdateSubject;

        /// <inheritdoc/>
        public IObservable<DiscordMessagePacket> MessageCreate => messageCreateSubject;
        /// <inheritdoc/>
        public IObservable<DiscordMessageDeleteArgs> MessageDelete => messageDeleteSubject;
        /// <inheritdoc/>
        public IObservable<DiscordMessagePacket> MessageUpdate => messageUpdateSubject;

        /// <inheritdoc />
        public IObservable<DiscordReactionPacket> MessageReactionCreate
            => messageReactionCreateSubject;

        /// <inheritdoc />
        public IObservable<DiscordReactionPacket> MessageReactionDelete
            => messageReactionDeleteSubject;

        /// <inheritdoc/>
        public IObservable<DiscordPresencePacket> PresenceUpdate => presenceUpdateSubject;
        /// <inheritdoc/>
        public IObservable<GatewayReadyPacket> Ready => readySubject;
        /// <inheritdoc/>
        public IObservable<TypingStartEventArgs> TypingStart => typingStartSubject;

        /// <inheritdoc />
        public IObservable<DiscordUserPacket> UserUpdate => userUpdateSubject;

        private readonly Subject<DiscordChannelPacket> channelCreateSubject;
        private readonly Subject<DiscordChannelPacket> channelDeleteSubject;
        private readonly Subject<DiscordChannelPacket> channelUpdateSubject;
        private readonly Subject<DiscordGuildPacket> guildCreateSubject;
        private readonly Subject<DiscordGuildUnavailablePacket> guildDeleteSubject;
        private readonly Subject<DiscordGuildPacket> guildUpdateSubject;
        private readonly Subject<GuildEmojisUpdateEventArgs> guildEmojiUpdateSubject;
        private readonly Subject<DiscordGuildMemberPacket> guildMemberCreateSubject;
        private readonly Subject<GuildIdUserArgs> guildMemberDeleteSubject;
        private readonly Subject<GuildMemberUpdateEventArgs> guildMemberUpdateSubject;
        private readonly Subject<RoleEventArgs> guildRoleCreateSubject;
        private readonly Subject<RoleDeleteEventArgs> guildRoleDeleteSubject;
        private readonly Subject<RoleEventArgs> guildRoleUpdateSubject;
        private readonly Subject<DiscordMessagePacket> messageCreateSubject;
        private readonly Subject<DiscordMessageDeleteArgs> messageDeleteSubject;
        private readonly Subject<DiscordMessagePacket> messageUpdateSubject;
        private readonly Subject<DiscordReactionPacket> messageReactionCreateSubject;
        private readonly Subject<DiscordReactionPacket> messageReactionDeleteSubject;
        private readonly Subject<DiscordPresencePacket> presenceUpdateSubject;
        private readonly Subject<GatewayReadyPacket> readySubject;
        private readonly Subject<TypingStartEventArgs> typingStartSubject;
        private readonly Subject<DiscordUserPacket> userUpdateSubject;

        private readonly JsonSerializerOptions serializerOptions;

        public GatewayEventHandler(
            IObservable<GatewayMessage> messageObserver,
            JsonSerializerOptions serializerOptions)
        {
            this.serializerOptions = serializerOptions;

            channelCreateSubject = new Subject<DiscordChannelPacket>();
            channelDeleteSubject = new Subject<DiscordChannelPacket>();
            channelUpdateSubject = new Subject<DiscordChannelPacket>();
            guildCreateSubject = new Subject<DiscordGuildPacket>();
            guildDeleteSubject = new Subject<DiscordGuildUnavailablePacket>();
            guildUpdateSubject = new Subject<DiscordGuildPacket>();
            guildEmojiUpdateSubject = new Subject<GuildEmojisUpdateEventArgs>();
            guildMemberCreateSubject = new Subject<DiscordGuildMemberPacket>();
            guildMemberDeleteSubject = new Subject<GuildIdUserArgs>();
            guildMemberUpdateSubject = new Subject<GuildMemberUpdateEventArgs>();
            guildRoleCreateSubject = new Subject<RoleEventArgs>();
            guildRoleDeleteSubject = new Subject<RoleDeleteEventArgs>();
            guildRoleUpdateSubject = new Subject<RoleEventArgs>();
            messageCreateSubject = new Subject<DiscordMessagePacket>();
            messageUpdateSubject = new Subject<DiscordMessagePacket>();
            messageDeleteSubject = new Subject<DiscordMessageDeleteArgs>();
            messageReactionCreateSubject = new Subject<DiscordReactionPacket>();
            messageReactionDeleteSubject = new Subject<DiscordReactionPacket>();
            presenceUpdateSubject = new Subject<DiscordPresencePacket>();
            readySubject = new Subject<GatewayReadyPacket>();
            typingStartSubject = new Subject<TypingStartEventArgs>();
            userUpdateSubject = new Subject<DiscordUserPacket>();

            messageObserver.Subscribe(OnPacketReceived);
        }

        /// <summary>
        /// Handles the observer's object to handle seperate typed observers.
        /// </summary>
        protected virtual void OnPacketReceived(GatewayMessage text)
        {
            if(text.OpCode != GatewayOpcode.Dispatch || !(text.Data is JsonElement elem))
            {
                return;
            }

            switch(text.EventName)
            {
                case "CHANNEL_CREATE":
                    channelCreateSubject.OnNext(
                        elem.ToObject<DiscordChannelPacket>(serializerOptions));
                    break;

                case "CHANNEL_DELETE":
                    channelDeleteSubject.OnNext(
                        elem.ToObject<DiscordChannelPacket>(serializerOptions));
                    break;

                case "CHANNEL_UPDATE":
                    channelUpdateSubject.OnNext(
                        elem.ToObject<DiscordChannelPacket>(serializerOptions));
                    break;

                case "GUILD_CREATE":
                    guildCreateSubject.OnNext(
                        elem.ToObject<DiscordGuildPacket>(serializerOptions));
                    break;

                case "GUILD_DELETE":
                    guildDeleteSubject.OnNext(
                        elem.ToObject<DiscordGuildUnavailablePacket>(serializerOptions));
                    break;

                case "GUILD_MEMBER_ADD":
                    guildMemberCreateSubject.OnNext(
                        elem.ToObject<DiscordGuildMemberPacket>(serializerOptions));
                    break;

                case "GUILD_MEMBER_REMOVE":
                    guildMemberDeleteSubject.OnNext(
                        elem.ToObject<GuildIdUserArgs>(serializerOptions));
                    break;

                case "GUILD_MEMBER_UPDATE":
                    guildMemberUpdateSubject.OnNext(
                        elem.ToObject<GuildMemberUpdateEventArgs>(serializerOptions));
                    break;

                case "GUILD_ROLE_UPDATE":
                    guildRoleUpdateSubject.OnNext(elem.ToObject<RoleEventArgs>(serializerOptions));
                    break;

                case "GUILD_UPDATE":
                    guildUpdateSubject.OnNext(
                        elem.ToObject<DiscordGuildPacket>(serializerOptions));
                    break;

                case "MESSAGE_CREATE":
                    messageCreateSubject.OnNext(
                        elem.ToObject<DiscordMessagePacket>(serializerOptions));
                    break;
                
                case "MESSAGE_DELETE":
                    messageDeleteSubject.OnNext(
                        elem.ToObject<DiscordMessageDeleteArgs>(serializerOptions));
                    break;

                case "MESSAGE_REACTION_ADD":
                    messageReactionCreateSubject.OnNext(
                        elem.ToObject<DiscordReactionPacket>(serializerOptions));
                    break;
                
                case "MESSAGE_REACTION_REMOVE":
                    messageReactionDeleteSubject.OnNext(
                        elem.ToObject<DiscordReactionPacket>(serializerOptions));
                    break;
                
                case "MESSAGE_UPDATE":
                    messageUpdateSubject.OnNext(
                        elem.ToObject<DiscordMessagePacket>(serializerOptions));
                    break;

                case "PRESENCE_UPDATE":
                    presenceUpdateSubject.OnNext(
                        elem.ToObject<DiscordPresencePacket>(serializerOptions));
                    break;

                case "READY":
                    readySubject.OnNext(
                        elem.ToObject<GatewayReadyPacket>(serializerOptions));
                    break;

                case "TYPING_START":
                    typingStartSubject.OnNext(
                        elem.ToObject<TypingStartEventArgs>(serializerOptions));
                    break;

                case "USER_UPDATE":
                    userUpdateSubject.OnNext(
                        elem.ToObject<DiscordUserPacket>(serializerOptions));
                    break;

                default:
                    Log.Debug($"{text.EventName} is not implemented.");
                    break;
            }
        }
    }
}
