using Miki.Cache;
using Miki.Discord.Caching.Stages;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Caching
{
	public class CacheClient
	{
		IGateway _gateway;
		IExtendedCacheClient _cachePool;
		public readonly IApiClient ApiClient;

		public Func<DiscordChannelPacket, IExtendedCacheClient, Task> OnChannelCreate;
		public Func<DiscordChannelPacket, IExtendedCacheClient, Task> OnChannelUpdate;
		public Func<DiscordChannelPacket, IExtendedCacheClient, Task> OnChannelDelete;

		public Func<DiscordGuildPacket, IExtendedCacheClient, Task> OnGuildCreate;
		public Func<DiscordGuildPacket, IExtendedCacheClient, Task> OnGuildUpdate;
		public Func<DiscordGuildUnavailablePacket, IExtendedCacheClient, Task> OnGuildDelete;

		public Func<DiscordGuildMemberPacket, IExtendedCacheClient, Task> OnGuildMemberAdd;
		public Func<GuildMemberUpdateEventArgs, IExtendedCacheClient, Task> OnGuildMemberUpdate;
		public Func<ulong, DiscordUserPacket, IExtendedCacheClient, Task> OnGuildMemberRemove;

		public Func<ulong, DiscordRolePacket, IExtendedCacheClient, Task> OnGuildRoleCreate;
		public Func<ulong, DiscordRolePacket, IExtendedCacheClient, Task> OnGuildRoleUpdate;
		public Func<ulong, ulong, IExtendedCacheClient, Task> OnGuildRoleDelete;

		public Func<GatewayReadyPacket, IExtendedCacheClient, Task> OnReady;

		public Func<DiscordUserPacket, IExtendedCacheClient, Task> OnUserUpdate;

		public CacheClient(
			IGateway messenger,
			IExtendedCacheClient cachePool,
			IApiClient discordClient)
		{
			_gateway = messenger;
			_cachePool = cachePool;
			ApiClient = discordClient;

			_gateway.OnChannelCreate += async (p) =>
			{
				if (OnChannelCreate != null)
				{
					await OnChannelCreate(p, _cachePool);
				}
			};

			_gateway.OnChannelDelete += async (p) =>
			{
				if (OnChannelDelete != null)
				{
					await OnChannelDelete(p, _cachePool);
				}
			};

			_gateway.OnChannelUpdate += async (p) =>
			{
				if (OnChannelUpdate != null)
				{
					await OnChannelUpdate(p, _cachePool);
				}
			};

			// TODO (Veld): Reimplement
			//_gateway.OnGuildBanAdd += OnGuildBanAdd;
			//_gateway.OnGuildBanRemove += OnGuildBanRemove;

			_gateway.OnGuildCreate += async (p) =>
			{
				if (OnGuildCreate != null)
				{
					await OnGuildCreate(p, _cachePool);
				}
			};

			_gateway.OnGuildDelete += async (p) =>
			{
				if (OnGuildDelete != null)
				{
					await OnGuildDelete(p, _cachePool);
				}
			};

			_gateway.OnGuildUpdate += async (p) =>
			{
				if (OnGuildUpdate != null)
				{
					await OnGuildUpdate(p, _cachePool);
				}
			};

			_gateway.OnGuildMemberAdd += async (p) =>
			{
				if (OnGuildMemberAdd != null)
				{
					await OnGuildMemberAdd(p, _cachePool);
				}
			};

			_gateway.OnGuildMemberRemove += async (id, user) =>
			{
				if (OnGuildMemberRemove != null)
				{
					await OnGuildMemberRemove(id, user, _cachePool);
				}
			};

			_gateway.OnGuildMemberUpdate += async (p) =>
			{
				if (OnGuildMemberUpdate != null)
				{
					await OnGuildMemberUpdate(p, _cachePool);
				}
			};

			_gateway.OnUserUpdate += async (p) =>
			{
				if (OnUserUpdate != null)
				{
					await OnUserUpdate(p, _cachePool);
				}
			};

			_gateway.OnGuildRoleCreate += async (guildId, role) =>
			{
				if (OnGuildRoleCreate != null)
				{
					await OnGuildRoleCreate(guildId, role, _cachePool);
				}
			};

			_gateway.OnGuildRoleDelete += async (guildId, roleId) =>
			{
				if (OnGuildRoleDelete != null)
				{
					await OnGuildRoleDelete(guildId, roleId, _cachePool);
				}
			};

			_gateway.OnGuildRoleUpdate += async (guildId, role) =>
			{
				if (OnGuildRoleUpdate != null)
				{
					await OnGuildRoleUpdate(guildId, role, _cachePool);
				}
			};

			// TODO (Veld): reimplement
			//_gateway.OnPresenceUpdate += OnPresenceUpdate;
		}

		//private async Task OnPresenceUpdate(DiscordPresencePacket arg)
		//{
		//	try
		//	{
		//		ICacheClient client = _cacheClient.Get;

		//		if (arg.User.Avatar != null || arg.User.Discriminator != null || arg.User.Username != null)
		//		{
		//			DiscordUserPacket user = await _discordClient.GetUserAsync(arg.User.Id);
		//			user.Username = arg.User.Username ?? user.Username;
		//			user.Discriminator = arg.User.Discriminator ?? user.Discriminator;
		//			user.Avatar = arg.User.Avatar ?? user.Avatar;
		//			await client.UpsertAsync($"discord:user:{arg.User.Id}", user);
		//		}

		//		await client.UpsertAsync($"discord:user:presence:{arg.User.Id}", new DiscordPresence(arg));
		//	}
		//	catch(Exception e)
		//	{
		//		Log.Error(e);
		//	}
		//}

		//private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs arg)
		//{
		//	try
		//	{
		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

		//		if (guild.Members == null)
		//		{
		//			guild.Members = new List<DiscordGuildMemberPacket>();
		//		}

		//		int index = guild.Members.FindIndex(x => x.User.Id == arg.User.Id);

		//		DiscordGuildMemberPacket packet;

		//		if (index == -1)
		//		{
		//			packet = new DiscordGuildMemberPacket
		//			{
		//				User = arg.User,
		//				Roles = arg.RoleIds.ToList(),
		//				Nickname = arg.Nickname,
		//				GuildId = arg.GuildId,
		//			};

		//			guild.Members.Add(packet);
		//		}
		//		else
		//		{
		//			guild.Members[index].Nickname = arg.Nickname;
		//			guild.Members[index].Roles = arg.RoleIds.ToList();
		//			guild.Members[index].User = arg.User;

		//			packet = guild.Members[index];
		//		}

		//		var cache = _cacheClient.Get;

		//		await cache.UpsertAsync($"discord:guild:{arg.GuildId}:user:{arg.User.Id}", packet);
		//			await cache.UpsertAsync($"discord:guild:{arg.GuildId}", guild);

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket arg2)
		//{
		//	try
		//	{
		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

		//		if (guild == null)
		//		{
		//			Log.Error($"Guild '{guildId}' not found");
		//			return;
		//		}

		//		if (guild.Members == null)
		//		{
		//			guild.Members = new List<DiscordGuildMemberPacket>();
		//		}
		//		else
		//		{
		//			int index = guild.Members.FindIndex(x => x.User.Id == arg2.Id);

		//			if (index != -1)
		//			{
		//				guild.Members.RemoveAt(index);
		//			}
		//		}

		//		var cache = _cacheClient.Get;

		//		string guildsJoinedCacheKey = $"discord:user:{arg2.Id}:guilds";

		//		List<ulong> guildsJoined = await cache.GetAsync<List<ulong>>(guildsJoinedCacheKey);

		//		if (guildsJoined != null)
		//		{
		//			guildsJoined.RemoveAll(x => x == guildId);
		//			await cache.UpsertAsync(guildsJoinedCacheKey, guildsJoined);
		//		}

		//		await cache.RemoveAsync($"discord:guild:{guildId}:user:{arg2.Id}");
		//		await cache.UpsertAsync($"discord:guild:{guildId}", guild);

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnGuildMemberAdd(DiscordGuildMemberPacket arg)
		//{
		//	try
		//	{
		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

		//		if (guild.Members == null)
		//		{
		//			guild.Members = new List<DiscordGuildMemberPacket>();
		//		}

		//		int index = guild.Members.FindIndex(x => x.User.Id == arg.User.Id);

		//		if (index != -1)
		//		{
		//			guild.Members.RemoveAt(index);
		//		}

		//		guild.Members.Add(
		//			await _discordClient.GetGuildUserAsync(arg.User.Id, arg.GuildId)
		//		);

		//		var cache = _cacheClient.Get;

		//		string guildsJoinedCacheKey = $"discord:user:{arg.User.Id}:guilds";

		//		List<ulong> guildsJoined = await cache.GetAsync<List<ulong>>(guildsJoinedCacheKey);

		//		if(guildsJoined == null)
		//		{
		//			guildsJoined = new List<ulong>();
		//		}

		//		guildsJoined.Add(arg.GuildId);

		//		await cache.UpsertAsync(guildsJoinedCacheKey, guildsJoined);
		//		await cache.UpsertAsync($"discord:guild:{arg.GuildId}:user:{arg.User.Id}", arg);
		//		await cache.UpsertAsync($"discord:guild:{arg.GuildId}", guild);

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnGuildRoleUpdate(ulong guildId, DiscordRolePacket arg2)
		//{
		//	try
		//	{
		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

		//		if (guild.Roles == null)
		//		{
		//			guild.Roles = new List<DiscordRolePacket>();
		//		}

		//		int index = guild.Roles.FindIndex(x => x.Id == arg2.Id);

		//		if (index == -1)
		//		{
		//			guild.Roles.Add(arg2);
		//		}
		//		else
		//		{
		//			guild.Roles[index] = arg2;
		//		}

		//		var cache = _cacheClient.Get;

		//		await cache.UpsertAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
		//			await cache.UpsertAsync($"discord:guild:{guildId}", guild);

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnGuildRoleDelete(ulong guildId, ulong roleId)
		//{
		//	try
		//	{
		//		var cache = _cacheClient.Get;

		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

		//			if (guild == null)
		//				Console.WriteLine("OnGuildRoleDelete: Cache miss: guild was not found.");

		//			if (guild.Roles != null)
		//			{
		//				int index = guild.Roles.FindIndex(x => x.Id == roleId);

		//				if (index != -1)
		//				{
		//					guild.Roles.RemoveAt(index);
		//				}

		//				await cache.UpsertAsync($"discord:guild:{guildId}", guild);
		//			}

		//			await cache.RemoveAsync($"discord:guild:{guildId}:role:{roleId}");
		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnGuildRoleCreate(ulong guildId, DiscordRolePacket arg2)
		//{
		//	try
		//	{
		//		DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

		//		if (guild.Roles == null)
		//		{
		//			guild.Roles = new List<DiscordRolePacket>();
		//		}

		//		int index = guild.Roles.FindIndex(x => x.Id == arg2.Id);

		//		guild.Roles.Add(arg2);

		//		var cache = _cacheClient.Get;

		//		await cache.UpsertAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
		//		await cache.UpsertAsync($"discord:guild:{guildId}", guild);

		//	}
		//	catch (Exception e)
		//	{
		//		Log.Trace(e.ToString());
		//	}
		//}

		//private async Task OnUserUpdate(DiscordUserPacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.UpsertAsync($"discord:user:{arg.Id}", arg);
		//}

		//private async Task OnGuildUpdate(DiscordGuildPacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.UpsertAsync($"discord:guild:{arg.Id}", arg);

		//}

		//private async Task OnGuildDelete(DiscordGuildUnavailablePacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.RemoveAsync($"discord:guild:{arg.GuildId}");

		//}

		//private async Task OnGuildCreate(DiscordGuildPacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.UpsertAsync($"discord:guild:{arg.Id}", arg);

		//}

		//private async Task OnGuildBanRemove(ulong arg1, DiscordUserPacket arg2)
		//{
		//}

		//private async Task OnGuildBanAdd(ulong arg1, DiscordUserPacket arg2)
		//{
		//}

		//private async Task OnChannelUpdate(DiscordChannelPacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.UpsertAsync($"discord:channel:{arg.Id}", arg);

		//}

		//private async Task OnChannelDelete(DiscordChannelPacket arg)
		//{
		//	var cache = _cacheClient.Get;
		//	await cache.RemoveAsync($"discord:channel:{arg.Id}");
		//}

		//private async Task OnChannelCreate(DiscordChannelPacket arg)
		//{
		//	var cache = _cacheClient.Get;

		//	await cache.UpsertAsync($"discord:channel:{arg.Id}", arg);
		//}
	}
}
