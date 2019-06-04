using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Caching.Stages
{
	public class BasicCacheStage : ICacheStage
	{
		private IExtendedCacheClient _cache;

		public void Initialize(IGateway client, IExtendedCacheClient cache)
		{
			_cache = cache;

			// Both should upsert the current channel.
			client.OnChannelCreate += OnChannelCreate;
			client.OnChannelUpdate += OnChannelCreate;

			client.OnChannelDelete += OnChannelDelete;

			client.OnGuildCreate += OnGuildCreate;
			client.OnGuildUpdate += OnGuildUpdate;
			client.OnGuildDelete += OnGuildDelete;

			client.OnGuildEmojiUpdate += OnGuildEmojiUpdate;

			client.OnGuildMemberAdd += OnGuildMemberAdd;
			client.OnGuildMemberRemove += OnGuildMemberRemove;
			client.OnGuildMemberUpdate += OnGuildMemberUpdate;

			client.OnGuildRoleCreate += OnRoleCreate;
			client.OnGuildRoleUpdate += OnRoleCreate;

			client.OnGuildRoleDelete += OnRoleDelete;

			client.OnUserUpdate += OnUserUpdate;
			client.OnPresenceUpdate += OnUserUpdate;

			client.OnReady += OnReady;
		}

		private async Task OnGuildEmojiUpdate(ulong guildId, DiscordEmoji[] emojis)
		{
			var guild = await _cache.HashGetAsync<DiscordGuildPacket>(
				CacheUtils.GuildsCacheKey, guildId.ToString());

			if(guild != null)
			{
				guild.Emojis = emojis;
				await _cache.HashUpsertAsync(CacheUtils.GuildsCacheKey, guildId.ToString(), guild);
			}
		}

		private async Task OnRoleDelete(ulong guildId, ulong roleId)
			=> await _cache.HashDeleteAsync(CacheUtils.GuildRolesKey(guildId), roleId.ToString());

		private async Task OnRoleCreate(ulong guildId, DiscordRolePacket role)
			=> await _cache.HashUpsertAsync(CacheUtils.GuildRolesKey(guildId), role.Id.ToString(), role);

		// Consider doing in gateway?
		private async Task OnReady(GatewayReadyPacket ready)
		{
			KeyValuePair<string, DiscordGuildPacket>[] readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Count()];

			for(int i = 0, max = readyPackets.Count(); i < max; i++)
			{
				readyPackets[i] = new KeyValuePair<string, DiscordGuildPacket>(ready.Guilds[i].Id.ToString(), ready.Guilds[i]);
			}

			await Task.WhenAll(
				_cache.HashUpsertAsync(CacheUtils.GuildsCacheKey, readyPackets),
				_cache.HashUpsertAsync(CacheUtils.UsersCacheKey, "me", ready.CurrentUser),
				_cache.HashUpsertAsync(CacheUtils.UsersCacheKey, ready.CurrentUser.Id.ToString(), ready.CurrentUser)
			);
		}

		private async Task OnUserUpdate(DiscordPresencePacket user)
		{
            List<Task> tasks = new List<Task>
            {
                _cache.HashUpsertAsync(CacheUtils.UsersCacheKey, user.User.Id.ToString(), user.User)
            };

            if (user.GuildId.HasValue)
			{
				var guildMember = await _cache.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(user.GuildId.Value), user.User.Id.ToString());
				if(guildMember == null)
				{
					guildMember = new DiscordGuildMemberPacket();
				}
				guildMember.User = user.User;
				tasks.Add(_cache.HashUpsertAsync(CacheUtils.GuildMembersKey(user.GuildId.Value), user.User.Id.ToString(), guildMember));
			}

			await Task.WhenAll(tasks);
		}

		private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs member)
		{
			var m = await _cache.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString());

			if(m == null)
			{
				m = new DiscordGuildMemberPacket();
			}

			m.User = member.User;
			m.Roles = member.RoleIds.ToList();
			m.Nickname = member.Nickname;
		
			await _cache.HashUpsertAsync(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString(), m);
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket user)
			=> await _cache.HashDeleteAsync(CacheUtils.GuildMembersKey(guildId), user.Id.ToString());

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket member)
			=> await _cache.HashUpsertAsync(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString(), member);

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild)
			=> await _cache.HashDeleteAsync(CacheUtils.GuildsCacheKey, unavailableGuild.GuildId.ToString());

		private async Task OnGuildUpdate(DiscordGuildPacket arg1)
		{
			DiscordGuildPacket guild = await _cache.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey, arg1.Id.ToString());

			if (guild == null)
			{
				guild = arg1;
			}
			else
			{
				guild.OverwriteContext(arg1);
			}

			await _cache.HashUpsertAsync(CacheUtils.GuildsCacheKey, guild.Id.ToString(), guild);
		}

		private async Task OnGuildCreate(DiscordGuildPacket guild)
		{
            guild.Members.RemoveAll(x => x == null);

            await Task.WhenAll(
				_cache.HashUpsertAsync(CacheUtils.GuildsCacheKey, guild.Id.ToString(), guild),
				_cache.HashUpsertAsync(CacheUtils.ChannelsKey(guild.Id), guild.Channels.Select(x =>
                {
                    x.GuildId = guild.Id;
                    return new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x);
                })),
				_cache.HashUpsertAsync(CacheUtils.GuildMembersKey(guild.Id), guild.Members.Select(x =>
				{
					x.GuildId = guild.Id;
					return new KeyValuePair<string, DiscordGuildMemberPacket>(x.User.Id.ToString(), x);
				})),
				_cache.HashUpsertAsync(CacheUtils.GuildRolesKey(guild.Id), guild.Roles.Select(x =>
				{
					return new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x);
				})),
				_cache.HashUpsertAsync(CacheUtils.UsersCacheKey, guild.Members.Select(x =>
				{
					return new KeyValuePair<string, DiscordUserPacket>(x.User.Id.ToString(), x.User);
				}))
			);
		}

		private async Task OnChannelCreate(DiscordChannelPacket channel)
			=> await _cache.HashUpsertAsync(CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString(), channel);

		private async Task OnChannelDelete(DiscordChannelPacket channel)
			=>	await _cache.HashDeleteAsync(CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString());
	}
}
