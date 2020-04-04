namespace Miki.Discord
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Events;
    using Miki.Discord.Common.Gateway.Packets;
    using Miki.Discord.Common.Packets;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class DiscordClient
    {
        private void AttachHandlers()
        {
            Gateway.OnChannelCreate += OnChannelCreate;
            Gateway.OnChannelUpdate += OnChannelCreate;

            Gateway.OnChannelDelete += OnChannelDelete;

            Gateway.OnGuildCreate += OnGuildCreate;
            Gateway.OnGuildUpdate += OnGuildUpdate;
            Gateway.OnGuildDelete += OnGuildDelete;

            Gateway.OnGuildEmojiUpdate += OnGuildEmojiUpdate;

            Gateway.OnGuildMemberAdd += OnGuildMemberAdd;
            Gateway.OnGuildMemberRemove += OnGuildMemberRemove;
            Gateway.OnGuildMemberUpdate += OnGuildMemberUpdate;

            Gateway.OnGuildRoleCreate += OnRoleCreate;
            Gateway.OnGuildRoleUpdate += OnRoleCreate;

            Gateway.OnGuildRoleDelete += OnRoleDelete;

            Gateway.OnUserUpdate += OnUserUpdate;
            Gateway.OnPresenceUpdate += OnUserUpdate;

            Gateway.OnReady += OnReady;
        }

        private void DetachHandlers()
        {
            Gateway.OnChannelCreate -= OnChannelCreate;
            Gateway.OnChannelUpdate -= OnChannelCreate;

            Gateway.OnChannelDelete -= OnChannelDelete;

            Gateway.OnGuildCreate -= OnGuildCreate;
            Gateway.OnGuildUpdate -= OnGuildUpdate;
            Gateway.OnGuildDelete -= OnGuildDelete;

            Gateway.OnGuildEmojiUpdate -= OnGuildEmojiUpdate;

            Gateway.OnGuildMemberAdd -= OnGuildMemberAdd;
            Gateway.OnGuildMemberRemove -= OnGuildMemberRemove;
            Gateway.OnGuildMemberUpdate -= OnGuildMemberUpdate;

            Gateway.OnGuildRoleCreate -= OnRoleCreate;
            Gateway.OnGuildRoleUpdate -= OnRoleCreate;

            Gateway.OnGuildRoleDelete -= OnRoleDelete;

            Gateway.OnUserUpdate -= OnUserUpdate;
            Gateway.OnPresenceUpdate -= OnUserUpdate;

            Gateway.OnReady -= OnReady;
        }

        private async Task OnGuildEmojiUpdate(ulong guildId, DiscordEmoji[] emojis)
        {
            var guild = await CacheClient.HashGetAsync<DiscordGuildPacket>(
                CacheUtils.GuildsCacheKey, guildId.ToString());

            if(guild != null)
            {
                guild.Emojis = emojis;
                await CacheClient.HashUpsertAsync(
                    CacheUtils.GuildsCacheKey,
                    guildId.ToString(),
                    guild);
            }
        }

        private async Task OnRoleDelete(ulong guildId, ulong roleId)
        {
            await CacheClient.HashDeleteAsync(
                CacheUtils.GuildRolesKey(guildId),
                roleId.ToString());
        }

        private async Task OnRoleCreate(ulong guildId, DiscordRolePacket role)
        {
            await CacheClient.HashUpsertAsync(
                CacheUtils.GuildRolesKey(guildId),
                role.Id.ToString(),
                role);
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

        private async Task OnUserUpdate(DiscordPresencePacket user)
        {
            var tasks = new List<Task>
            {
                CacheClient.HashUpsertAsync(
                    CacheUtils.UsersCacheKey,
                    user.User.Id.ToString(),
                    user.User)
            };

            if(user.GuildId.HasValue)
            {
                var guildMember = await CacheClient.HashGetAsync<DiscordGuildMemberPacket>(
                    CacheUtils.GuildMembersKey(user.GuildId.Value),
                    user.User.Id.ToString());
                if(guildMember == null)
                {
                    guildMember = new DiscordGuildMemberPacket();
                }
                guildMember.User = user.User;
                tasks.Add(CacheClient.HashUpsertAsync(
                    CacheUtils.GuildMembersKey(user.GuildId.Value),
                    user.User.Id.ToString(),
                    guildMember));
            }

            await Task.WhenAll(tasks);
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

        private Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket user)
        {
            return CacheClient.HashDeleteAsync(
                CacheUtils.GuildMembersKey(guildId), user.Id.ToString());
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
