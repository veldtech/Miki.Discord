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
		#region constants
		string DmChannelCacheKey()
			=> $"discord:dmchannels";

		string GuildCacheKey(ulong id)
			=> $"discord:guild:{id}";

		string GuildMembersCacheKey(ulong id)
			=> GuildCacheKey(id) + ":members";

		string GuildChannelsCacheKey(ulong id)
			=> GuildCacheKey(id) + ":channels";

		string UserCacheKey(ulong id)
			=> $"discord:user:{id}";
		#endregion

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
		{
			DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(guildId));

			if (guild == null)
			{
				Log.Debug("Tried to Update/Create role, but guild is not in cache.");
				return;
			}

			if (guild.Roles == null)
			{
				guild.Roles = new List<DiscordRolePacket>();
			}

			int index = guild.Roles.RemoveAll(x => x.Id == roleId);

			await cache.UpsertAsync(GuildCacheKey(guildId), guild);
		}

		private async Task OnRoleCreate(ulong guildId, DiscordRolePacket role, IExtendedCacheClient cache)
		{
			DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(guildId));

			if(guild == null)
			{
				Log.Debug("Tried to Update/Create role, but guild is not in cache.");
				return;
			}

			if (guild.Roles == null)
			{
				guild.Roles = new List<DiscordRolePacket>();
			}

			int index = guild.Roles.FindIndex(x => x.Id == role.Id);

			if (index != -1)
			{
				guild.Roles[index] = role;
			}
			else
			{
				guild.Roles.Add(role);
			}

			await cache.UpsertAsync(GuildCacheKey(guildId), guild);
		}

		private async Task OnReady(GatewayReadyPacket ready, IExtendedCacheClient cache)
		{
			KeyValuePair<string, DiscordGuildPacket>[] readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Count()];

			for(int i = 0, max = readyPackets.Count(); i < max; i++)
			{
				readyPackets[i] = new KeyValuePair<string, DiscordGuildPacket>(GuildCacheKey(ready.Guilds[i].Id), ready.Guilds[i]);
			}

			await cache.UpsertAsync(readyPackets);
		}

		private async Task OnUserUpdate(DiscordUserPacket user, IExtendedCacheClient cache)
		{
			await cache.UpsertAsync(UserCacheKey(user.Id), user);
		}

		private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs member, IExtendedCacheClient cache)
		{
			DiscordGuildMemberPacket m = await cache.HashGetAsync<DiscordGuildMemberPacket>(GuildMembersCacheKey(member.GuildId), member.User.Id.ToString());

			if(m == null)
			{
				m = new DiscordGuildMemberPacket();
			}

			m.User = member.User;
			m.Roles = member.RoleIds.ToList();
			m.Nickname = member.Nickname;
		
			await cache.HashUpsertAsync(GuildCacheKey(member.GuildId), member.User.Id.ToString(), m);
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket user, IExtendedCacheClient cache)
		{
			await cache.HashDeleteAsync(GuildMembersCacheKey(guildId), user.Id.ToString());
		}

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket member, IExtendedCacheClient cache)
		{
			await cache.HashUpsertAsync(GuildMembersCacheKey(member.GuildId), member.User.Id.ToString(), member);
		}

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild, IExtendedCacheClient cache)
		{
			await cache.RemoveAsync(GuildCacheKey(unavailableGuild.GuildId));
		}

		private async Task OnGuildUpdate(DiscordGuildPacket arg1, IExtendedCacheClient cache)
		{
			DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(arg1.Id));

			if (guild == null)
			{
				guild = arg1;
			}
			else
			{
				guild.OverwriteContext(arg1);
			}

			await cache.UpsertAsync(GuildCacheKey(arg1.Id), guild);
		}

		private async Task OnGuildCreate(DiscordGuildPacket guild, IExtendedCacheClient cache)
		{
			await cache.UpsertAsync(GuildCacheKey(guild.Id), guild);

			await cache.HashUpsertAsync(GuildChannelsCacheKey(guild.Id),
				guild.Channels.Select(x =>
				{
					x.GuildId = guild.Id;
					return new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x);
				}).ToArray()
			);

			await cache.HashUpsertAsync(GuildMembersCacheKey(guild.Id), guild.Members.Select(x =>
				{
					x.GuildId = guild.Id;
					return new KeyValuePair<string, DiscordUserPacket>(UserCacheKey(x.User.Id), x.User);
				}).ToArray()
			);
		}

		private async Task OnChannelCreate(DiscordChannelPacket channel, IExtendedCacheClient cache)
		{
			string key = channel.GuildId.HasValue 
				? GuildCacheKey(channel.GuildId.Value) 
				: DmChannelCacheKey();

			await cache.HashUpsertAsync(key, channel.Id.ToString(), channel);
		}

		private async Task OnChannelDelete(DiscordChannelPacket channel, IExtendedCacheClient cache)
		{
			string key = channel.GuildId.HasValue 
				? GuildCacheKey(channel.GuildId.Value) 
				: DmChannelCacheKey();

			await cache.HashDeleteAsync(key, channel.Id.ToString());
		}
	}
}
