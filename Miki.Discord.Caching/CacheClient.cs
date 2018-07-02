using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Messaging;
using Miki.Discord.Rest;
using Miki.Discord.Rest.Entities;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Caching
{
    public class CacheClient
    {
		MessageClient _messenger;
		ICacheClient _cacheClient;
		DiscordApiClient _discordClient;

		public CacheClient(
			MessageClient messenger,
			ICacheClient cacheClient,
			DiscordApiClient discordClient)
		{
			_messenger = messenger;
			_cacheClient = cacheClient;
			_discordClient = discordClient;

			_messenger.ChannelCreate += OnChannelCreate;
			_messenger.ChannelDelete += OnChannelDelete;
			_messenger.ChannelUpdate += OnChannelUpdate;

			_messenger.GuildBanAdd += OnGuildBanAdd;
			_messenger.GuildBanRemove += OnGuildBanRemove;

			_messenger.GuildCreate += OnGuildCreate;
			_messenger.GuildDelete += OnGuildDelete;
			_messenger.GuildUpdate += OnGuildUpdate;

			_messenger.UserUpdate += OnUserUpdate;

			_messenger.GuildRoleCreate += OnGuildRoleCreate;
			_messenger.GuildRoleDelete += OnGuildRoleDelete;
			_messenger.GuildRoleUpdate += OnGuildRoleUpdate;

			_messenger.GuildMemberAdd += OnGuildMemberAdd;
			_messenger.GuildMemberRemove += OnGuildMemberRemove;
			_messenger.GuildMemberUpdate += OnGuildMemberUpdate;
		}

		private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs arg)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

			if(guild.Members == null)
			{
				guild.Members = new List<DiscordGuildMemberPacket>();
			}

			int index = guild.Members.FindIndex(x => x.UserId == arg.User.Id);

			DiscordGuildMemberPacket packet;

			if(index == -1)
			{
				var u = await _discordClient.GetGuildUserAsync(arg.User.Id, guild.Id);

				packet = u;
				guild.Members.Add(u);
			}
			else
			{
				guild.Members[index].Nickname = arg.Nickname;
				guild.Members[index].Roles = arg.RoleIds.ToList();
				guild.Members[index].User = arg.User;

				packet = guild.Members[index];
			}

			await _cacheClient.AddAsync($"discord:guild:{arg.GuildId}:user:{arg.User.Id}", packet);
			await _cacheClient.AddAsync($"discord:guild:{arg.GuildId}", guild);
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket arg2)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

			if (guild.Members == null)
			{
				guild.Members = new List<DiscordGuildMemberPacket>();
			}

			int index = guild.Members.FindIndex(x => x.UserId == arg2.Id);

			if (index != -1)
			{
				guild.Members.RemoveAt(index);
			}

			await _cacheClient.RemoveAsync($"discord:guild:{guildId}:user:{arg2.Id}");
			await _cacheClient.AddAsync($"discord:guild:{guildId}", guild);
		}

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket arg)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

			if(guild.Members == null)
			{
				guild.Members = new List<DiscordGuildMemberPacket>();
			}

			int index = guild.Members.FindIndex(x => x.UserId == arg.UserId);

			if (index != -1)
			{
				guild.Members.RemoveAt(index);
			}

			guild.Members.Add(
				await _discordClient.GetGuildUserAsync(arg.UserId, arg.GuildId)	
			);

			await _cacheClient.AddAsync($"discord:guild:{arg.GuildId}:user:{arg.UserId}", arg);
			await _cacheClient.AddAsync($"discord:guild:{arg.GuildId}", guild);
		}

		private async Task OnGuildRoleUpdate(ulong guildId, DiscordRolePacket arg2)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

			if (guild.Roles == null)
			{
				guild.Roles = new List<DiscordRolePacket>();
			}

			int index = guild.Roles.FindIndex(x => x.Id == guildId);

			if (index == -1)
			{
				guild.Roles.Add(arg2);
			}
			else
			{
				guild.Roles[index] = arg2;
			}

			await _cacheClient.AddAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
			await _cacheClient.AddAsync($"discord:guild:{guildId}", guild);
		}

		private async Task OnGuildRoleDelete(ulong guildId, ulong roleId)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

			if (guild == null)
				Console.WriteLine("OnGuildRoleDelete: Cache miss: guild was not found.");

			if (guild.Roles != null)
			{
				int index = guild.Roles.FindIndex(x => x.Id == roleId);

				if (index != -1)
				{
					guild.Roles.RemoveAt(index);
				}

				await _cacheClient.AddAsync($"discord:guild:{guildId}", guild);
			}

			await _cacheClient.RemoveAsync($"discord:guild:{guildId}:role:{roleId}");
		}

		private async Task OnGuildRoleCreate(ulong guildId, DiscordRolePacket arg2)
		{
			DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

			if (guild.Roles == null)
			{
				guild.Roles = new List<DiscordRolePacket>();
			}

			int index = guild.Roles.FindIndex(x => x.Id == guildId);

			if (index == -1)
			{
				guild.Roles.Add(arg2);
			}
			else
			{
				guild.Roles[index] = arg2;
			}

			await _cacheClient.AddAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
			await _cacheClient.AddAsync($"discord:guild:{guildId}", guild);
		}

		private async Task OnUserUpdate(DiscordUserPacket arg)
		{
			await _cacheClient.AddAsync($"discord:user:{arg.Id}", arg);
		}

		private async Task OnGuildUpdate(DiscordGuildPacket arg)
		{
			await _cacheClient.AddAsync($"discord:guild:{arg.Id}", arg);
		}

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket arg)
		{
			await _cacheClient.RemoveAsync($"discord:guild:{arg.GuildId}");
		}

		private async Task OnGuildCreate(DiscordGuildPacket arg)
		{
			await _cacheClient.AddAsync($"discord:guild:{arg.Id}", arg);
		}

		private async Task OnGuildBanRemove(ulong arg1, DiscordUserPacket arg2)
		{
		}

		private async Task OnGuildBanAdd(ulong arg1, DiscordUserPacket arg2)
		{
		}

		private async Task OnChannelUpdate(DiscordChannelPacket arg)
		{
			await _cacheClient.AddAsync($"discord:channel:{arg.Id}", arg);
		}

		private async Task OnChannelDelete(DiscordChannelPacket arg)
		{
			await _cacheClient.RemoveAsync($"discord:channel:{arg.Id}");
		}

		private async Task OnChannelCreate(DiscordChannelPacket arg)
		{
			await _cacheClient.AddAsync($"discord:channel:{arg.Id}", arg);
		}
	}
}
