using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Cache
{
    /// <summary>
    /// Handles gateway caching.
    /// </summary>
    public class EventCacheHandler : IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private readonly ICacheHandler cacheHandler;
        private readonly IGateway gateway;

        public EventCacheHandler(IGateway gateway, ICacheHandler cacheHandler)
        {
            this.gateway = gateway;
            this.cacheHandler = cacheHandler;
            
            disposables.AddRange(
                new [] {
                    gateway.Events.ChannelCreate.SubscribeTask(OnChannelCreate),
                    gateway.Events.ChannelUpdate.SubscribeTask(OnChannelCreate),
                    gateway.Events.ChannelDelete.SubscribeTask(OnChannelDelete),
                    gateway.Events.GuildCreate.SubscribeTask(OnGuildCreate),
                    gateway.Events.GuildUpdate.SubscribeTask(OnGuildUpdate),
                    gateway.Events.GuildDelete.SubscribeTask(OnGuildDelete),
                    gateway.Events.GuildEmojiUpdate.SubscribeTask(OnGuildEmojiUpdate),
                    gateway.Events.GuildMemberCreate.SubscribeTask(OnGuildMemberAdd),
                    gateway.Events.GuildMemberDelete.SubscribeTask(OnGuildMemberRemove),
                    gateway.Events.GuildMemberUpdate.SubscribeTask(OnGuildMemberUpdate),
                    gateway.Events.GuildRoleCreate.SubscribeTask(OnRoleCreate),
                    gateway.Events.GuildRoleUpdate.SubscribeTask(OnRoleCreate),
                    gateway.Events.GuildRoleDelete.SubscribeTask(OnRoleDelete),
                    gateway.Events.UserUpdate.SubscribeTask(OnUserUpdate),
                    gateway.Events.Ready.SubscribeTask(OnReady) 
                });
        }

        private async Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs eventArgs)
        {
            var guild = await cacheHandler.Guilds.GetAsync(eventArgs.GuildId)
                        ?? new DiscordGuildPacket {Id = eventArgs.GuildId};
            guild.Emojis = eventArgs.Emojis.ToArray();
            await cacheHandler.Guilds.EditAsync(guild);
        }

        private async Task OnRoleDelete(RoleDeleteEventArgs eventArgs)
        {
            await cacheHandler.Roles.DeleteAsync(new DiscordRolePacket
            {
                Id = eventArgs.RoleId,
                GuildId = eventArgs.GuildId
            });
        }

        private async Task OnRoleCreate(RoleEventArgs eventArgs)
        {
            await cacheHandler.Roles.AddAsync(eventArgs.Role);
        }

        private async Task OnReady(GatewayReadyPacket ready)
        {
            await Task.WhenAll(
                cacheHandler.Guilds.AddAsync(ready.Guilds).AsTask(),
                cacheHandler.Users.AddAsync(ready.CurrentUser).AsTask(),
                cacheHandler.SetCurrentUserAsync(ready.CurrentUser).AsTask());
        }

        private async Task OnUserUpdate(DiscordUserPacket user)
        {
            await cacheHandler.Users.EditAsync(user);
        }

        private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs updateEventArgs)
        {
            var member = await cacheHandler.Members.GetAsync(
                updateEventArgs.GuildId, updateEventArgs.User.Id) 
                         ?? new DiscordGuildMemberPacket();

            member.User = updateEventArgs.User;
            member.Roles = updateEventArgs.RoleIds.ToList();
            member.Nickname = updateEventArgs.Nickname;

            await cacheHandler.Members.EditAsync(member);
        }

        private async Task OnGuildMemberRemove(GuildIdUserArgs args)
        {
            await cacheHandler.Members.DeleteAsync(new DiscordGuildMemberPacket
            {
                User =  args.User,
                GuildId = args.GuildId
            });
        }

        private async Task OnGuildMemberAdd(DiscordGuildMemberPacket member)
        {
            await cacheHandler.Members.AddAsync(member);
        }

        private async Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild)
        {
            await cacheHandler.Guilds.DeleteAsync(new DiscordGuildPacket
            {
                Id  = unavailableGuild.GuildId
            });
        }

        private async Task OnGuildUpdate(DiscordGuildPacket arg1)
        {
            var guild = await cacheHandler.Guilds.GetAsync(arg1.Id) ?? arg1;
            
            guild.OverwriteContext(arg1);
            
            await cacheHandler.Guilds.EditAsync(guild);
        }

        private Task OnGuildCreate(DiscordGuildPacket guild)
        {
            guild.Members.RemoveAll(x => x == null);

            return Task.WhenAll(
                cacheHandler.Guilds.AddAsync(guild).AsTask(),
                cacheHandler.Channels.AddAsync(
                    guild.Channels.Select(x =>
                    {
                         x.GuildId = guild.Id;
                         return x;
                    })).AsTask(),
                cacheHandler.Members.AddAsync(
                    guild.Members.Select(x =>
                    {
                        x.GuildId = guild.Id;
                        return x;
                    })).AsTask(),
                cacheHandler.Roles.AddAsync(
                    guild.Roles.Select(x =>
                    {
                        x.GuildId = guild.Id;
                        return x;
                    })).AsTask(),
                cacheHandler.Users.AddAsync(
                    guild.Members.Select(x => x.User)).AsTask());
        }

        private async Task OnChannelCreate(DiscordChannelPacket channel)
        {
            await cacheHandler.Channels.AddAsync(channel); 
        }

        private async Task OnChannelDelete(DiscordChannelPacket channel)
        {
            await cacheHandler.Channels.DeleteAsync(channel);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var d in disposables)
            {
                d.Dispose();
            }
        }
    }
}
