using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common.Gateway;

namespace Miki.Discord
{
    public partial class DiscordClient
    {
        private void AttachHandlers()
        {
            Gateway.Events.ChannelCreate.SubscribeTask(OnChannelCreate);
            Gateway.Events.ChannelUpdate.SubscribeTask(OnChannelCreate);
            Gateway.Events.ChannelDelete.SubscribeTask(OnChannelDelete);
            Gateway.Events.GuildCreate.SubscribeTask(OnGuildCreate);
            Gateway.Events.GuildUpdate.SubscribeTask(OnGuildUpdate);
            Gateway.Events.GuildDelete.SubscribeTask(OnGuildDelete);
            Gateway.Events.GuildEmojiUpdate.SubscribeTask(OnGuildEmojiUpdate);
            Gateway.Events.GuildMemberCreate.SubscribeTask(OnGuildMemberAdd);
            Gateway.Events.GuildMemberDelete.SubscribeTask(OnGuildMemberRemove);
            Gateway.Events.GuildMemberUpdate.SubscribeTask(OnGuildMemberUpdate);
            Gateway.Events.GuildRoleCreate.SubscribeTask(OnRoleCreate);
            Gateway.Events.GuildRoleUpdate.SubscribeTask(OnRoleCreate);
            Gateway.Events.GuildRoleDelete.SubscribeTask(OnRoleDelete);
            Gateway.Events.UserUpdate.SubscribeTask(OnUserUpdate);
            Gateway.Events.Ready.SubscribeTask(OnReady);
        }

        private async Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs eventArgs)
        {
            var guild = await CacheClient.HashGetAsync<DiscordGuildPacket>(
                CacheUtils.GuildsCacheKey, eventArgs.GuildId.ToString());

            if(guild != null)
            {
                guild.Emojis = eventArgs.Emojis.ToArray();
                await CacheClient.HashUpsertAsync(
                    CacheUtils.GuildsCacheKey, eventArgs.GuildId.ToString(), guild);
            }
        }

        private async Task OnRoleDelete(RoleDeleteEventArgs eventArgs)
        {
            await CacheClient.HashDeleteAsync(
                CacheUtils.GuildRolesKey(eventArgs.GuildId),
                eventArgs.RoleId.ToString());
        }

        private async Task OnRoleCreate(RoleEventArgs eventArgs)
        {
            await CacheClient.HashUpsertAsync(
                CacheUtils.GuildRolesKey(eventArgs.GuildId),
                eventArgs.Role.Id.ToString(),
                eventArgs.Role);
        }

        private Task OnReady(GatewayReadyPacket ready)
        {
            var readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Count()];

            for(int i = 0, max = readyPackets.Length; i < max; i++)
            {
                readyPackets[i] = new KeyValuePair<string, DiscordGuildPacket>(
                    ready.Guilds[i].Id.ToString(),
                    ready.Guilds[i]);
            }

            return Task.WhenAll(
                CacheClient.HashUpsertAsync(
                    CacheUtils.GuildsCacheKey,
                    readyPackets),
                CacheClient.HashUpsertAsync(
                    CacheUtils.UsersCacheKey,
                    ready.CurrentUser.Id.ToString(),
                    ready.CurrentUser)
            );
        }

        private async Task OnUserUpdate(DiscordUserPacket user)
        {
            await cacheHandler.SetUserAsync(user);
        }

        private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs member)
        {
            var m = await CacheClient.HashGetAsync<DiscordGuildMemberPacket>(
                CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString());

            if(m == null)
            {
                m = new DiscordGuildMemberPacket();
            }

            m.User = member.User;
            m.Roles = member.RoleIds.ToList();
            m.Nickname = member.Nickname;

            await CacheClient.HashUpsertAsync(
                CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString(), m);
        }

        private Task OnGuildMemberRemove(GuildIdUserArgs args)
        {
            return CacheClient.HashDeleteAsync(
                CacheUtils.GuildMembersKey(args.GuildId), args.User.Id.ToString());
        }

        private Task OnGuildMemberAdd(DiscordGuildMemberPacket member)
        {
            return CacheClient.HashUpsertAsync(
                CacheUtils.GuildMembersKey(member.GuildId),
                member.User.Id.ToString(),
                member);
        }

        private Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild)
        {
            return CacheClient.HashDeleteAsync(
                CacheUtils.GuildsCacheKey,
                unavailableGuild.GuildId.ToString());
        }

        private async Task OnGuildUpdate(DiscordGuildPacket arg1)
        {
            var guild = await CacheClient.HashGetAsync<DiscordGuildPacket>(
                CacheUtils.GuildsCacheKey,
                arg1.Id.ToString());

            if(guild == null)
            {
                guild = arg1;
            }
            else
            {
                guild.OverwriteContext(arg1);
            }

            await CacheClient.HashUpsertAsync(
                CacheUtils.GuildsCacheKey,
                guild.Id.ToString(),
                guild);
        }

        private Task OnGuildCreate(DiscordGuildPacket guild)
        {
            guild.Members.RemoveAll(x => x == null);

            return Task.WhenAll(
                CacheClient.HashUpsertAsync(
                    CacheUtils.GuildsCacheKey, guild.Id.ToString(), guild),
                CacheClient.HashUpsertAsync(
                    CacheUtils.ChannelsKey(guild.Id),
                    guild.Channels.Select(x =>
                {
                    x.GuildId = guild.Id;
                    return new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x);
                })),
                CacheClient.HashUpsertAsync(
                    CacheUtils.GuildMembersKey(guild.Id),
                    guild.Members.Select(x =>
                {
                    x.GuildId = guild.Id;
                    return new KeyValuePair<string, DiscordGuildMemberPacket>(x.User.Id.ToString(), x);
                })),
                CacheClient.HashUpsertAsync(
                    CacheUtils.GuildRolesKey(guild.Id),
                    guild.Roles.Select(x =>
                {
                    return new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x);
                })),
                CacheClient.HashUpsertAsync(
                    CacheUtils.UsersCacheKey,
                    guild.Members.Select(x =>
                {
                    return new KeyValuePair<string, DiscordUserPacket>(x.User.Id.ToString(), x.User);
                }))
            );
        }

        private Task OnChannelCreate(DiscordChannelPacket channel)
        {
            return CacheClient.HashUpsertAsync(
                CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString(), channel);
        }

        private Task OnChannelDelete(DiscordChannelPacket channel)
        {
            return CacheClient.HashDeleteAsync(
                CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString());
        }
    }
}
