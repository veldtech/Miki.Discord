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
		public void Initialize(CacheClient client)
		{
			// Both should upsert the current channel.
			client.OnChannelCreate += OnChannelCreate;
			client.OnChannelUpdate += OnChannelCreate;

			client.OnChannelDelete += OnChannelDelete;

			client.OnGuildCreate += OnGuildCreate;
			client.OnGuildUpdate += OnGuildUpdate;
			client.OnGuildDelete += OnGuildDelete;

			client.OnGuildMemberAdd += OnGuildMemberAdd;
			client.OnGuildMemberRemove += OnGuildMemberRemove;
			client.OnGuildMemberUpdate += OnGuildMemberUpdate;

			client.OnGuildRoleCreate += OnRoleCreate;
			client.OnGuildRoleUpdate += OnRoleCreate;

			client.OnGuildRoleDelete += OnRoleDelete;

			client.OnUserUpdate += OnUserUpdate;

			client.OnReady += OnReady;
		}

		private async Task OnRoleDelete(ulong guildId, ulong roleId, IExtendedCacheClient cache)
			=> await cache.HashDeleteAsync(CacheUtils.GuildRolesKey(guildId), roleId.ToString());

		private async Task OnRoleCreate(ulong guildId, DiscordRolePacket role, IExtendedCacheClient cache)
			=> await cache.HashUpsertAsync(CacheUtils.GuildRolesKey(guildId), role.Id.ToString(), role);

		// Consider doing in gateway?
		private async Task OnReady(GatewayReadyPacket ready, IExtendedCacheClient cache)
		{
			KeyValuePair<string, DiscordGuildPacket>[] readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Count()];

			for(int i = 0, max = readyPackets.Count(); i < max; i++)
			{
				readyPackets[i] = new KeyValuePair<string, DiscordGuildPacket>(ready.Guilds[i].Id.ToString(), ready.Guilds[i]);
			}

			await cache.HashUpsertAsync(CacheUtils.GuildsCacheKey(), readyPackets);
		}

		private async Task OnUserUpdate(DiscordPresencePacket user, IExtendedCacheClient cache)
		{
			await cache.HashUpsertAsync(CacheUtils.UsersCacheKey(), user.User.Id.ToString(), user.User);

			var guildMember = await cache.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(user.User.Id), user.User.Id.ToString());

			if (guildMember != null)
			{
				guildMember.User = user.User;
				await cache.HashUpsertAsync(CacheUtils.GuildMembersKey(user.GuildId), user.User.Id.ToString(), guildMember);
			}
		}

		private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs member, IExtendedCacheClient cache)
		{
			var m = await cache.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString());

			if(m == null)
			{
				m = new DiscordGuildMemberPacket();
			}

			m.User = member.User;
			m.Roles = member.RoleIds.ToList();
			m.Nickname = member.Nickname;
		
			await cache.HashUpsertAsync(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString(), m);
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket user, IExtendedCacheClient cache)
			=> await cache.HashDeleteAsync(CacheUtils.GuildMembersKey(guildId), user.Id.ToString());

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket member, IExtendedCacheClient cache)
			=> await cache.HashUpsertAsync(CacheUtils.GuildMembersKey(member.GuildId), member.User.Id.ToString(), member);

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild, IExtendedCacheClient cache)
			=> await cache.HashDeleteAsync(CacheUtils.GuildsCacheKey(), unavailableGuild.GuildId.ToString());

		private async Task OnGuildUpdate(DiscordGuildPacket arg1, IExtendedCacheClient cache)
		{
			DiscordGuildPacket guild = await cache.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey(), arg1.Id.ToString());

			if (guild == null)
			{
				guild = arg1;
			}
			else
			{
				guild.OverwriteContext(arg1);
			}

			await cache.HashUpsertAsync(CacheUtils.GuildsCacheKey(), guild.Id.ToString(), guild);
		}

		private async Task OnGuildCreate(DiscordGuildPacket guild, IExtendedCacheClient cache)
		{
			await Task.WhenAll(
				cache.HashUpsertAsync(CacheUtils.GuildsCacheKey(), guild.Id.ToString(), guild),
				cache.HashUpsertAsync(CacheUtils.ChannelsKey(guild.Id),
					guild.Channels.Select(x =>
					{
						x.GuildId = guild.Id;
						return new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x);
					}).ToArray()
				),
					cache.HashUpsertAsync(CacheUtils.GuildMembersKey(guild.Id), guild.Members.Select(x =>
					{
						x.GuildId = guild.Id;
						return new KeyValuePair<string, DiscordGuildMemberPacket>(x.User.Id.ToString(), x);
					}).ToArray()
				),
					cache.HashUpsertAsync(CacheUtils.GuildRolesKey(guild.Id), guild.Roles.Select(x =>
					{
						return new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x);
					}).ToArray()
				),
					cache.HashUpsertAsync(CacheUtils.UsersCacheKey(), guild.Members.Select(x =>
					{
						return new KeyValuePair<string, DiscordUserPacket>(x.User.Id.ToString(), x.User);
					}).ToArray()
				)
			);
		}

		private async Task OnChannelCreate(DiscordChannelPacket channel, IExtendedCacheClient cache)
		{
			await cache.HashUpsertAsync(CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString(), channel);
		}

		private async Task OnChannelDelete(DiscordChannelPacket channel, IExtendedCacheClient cache)
		{
			await cache.HashDeleteAsync(CacheUtils.ChannelsKey(channel.GuildId), channel.Id.ToString());
		}
	}
}
