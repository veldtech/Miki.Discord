using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Messaging;
using Miki.Discord.Rest;
using Miki.Discord.Rest.Entities;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Caching
{
    public class CacheClient
    {
		MessageClient _messenger;
		ICachePool _cacheClient;
		DiscordApiClient _discordClient;

		public CacheClient(
			MessageClient messenger,
			ICachePool cacheClient,
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
			try
			{
				DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

				if (guild.Members == null)
				{
					guild.Members = new List<DiscordGuildMemberPacket>();
				}

				int index = guild.Members.FindIndex(x => x.UserId == arg.User.Id);

				DiscordGuildMemberPacket packet;

				if (index == -1)
				{
					packet = new DiscordGuildMemberPacket
					{
						User = arg.User,
						Roles = arg.RoleIds.ToList(),
						Nickname = arg.Nickname,
						UserId = arg.User.Id,
						GuildId = arg.GuildId,
					};

					guild.Members.Add(packet);
				}
				else
				{
					guild.Members[index].Nickname = arg.Nickname;
					guild.Members[index].Roles = arg.RoleIds.ToList();
					guild.Members[index].User = arg.User;

					packet = guild.Members[index];
				}

				var cache = _cacheClient.Get;

				await cache.UpsertAsync($"discord:guild:{arg.GuildId}:user:{arg.User.Id}", packet);
					await cache.UpsertAsync($"discord:guild:{arg.GuildId}", guild);
				
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket arg2)
		{
			try
			{
				DiscordGuildPacket guild = await _discordClient.GetGuildAsync(guildId);

				if (guild == null)
				{
					Log.Error($"Guild '{guildId}' not found");
					return;
				}

				if (guild.Members == null)
				{
					guild.Members = new List<DiscordGuildMemberPacket>();
				}
				else
				{
					int index = guild.Members.FindIndex(x => x.UserId == arg2.Id);

					if (index != -1)
					{
						guild.Members.RemoveAt(index);
					}
				}

				var cache = _cacheClient.Get;

				await cache.RemoveAsync($"discord:guild:{guildId}:user:{arg2.Id}");
					await cache.UpsertAsync($"discord:guild:{guildId}", guild);
				
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket arg)
		{
			try
			{
				DiscordGuildPacket guild = await _discordClient.GetGuildAsync(arg.GuildId);

				if (guild.Members == null)
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

				var cache = _cacheClient.Get;

				await cache.UpsertAsync($"discord:guild:{arg.GuildId}:user:{arg.UserId}", arg);
					await cache.UpsertAsync($"discord:guild:{arg.GuildId}", guild);
				
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnGuildRoleUpdate(ulong guildId, DiscordRolePacket arg2)
		{
			try
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

				var cache = _cacheClient.Get;

				await cache.UpsertAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
					await cache.UpsertAsync($"discord:guild:{guildId}", guild);
				
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnGuildRoleDelete(ulong guildId, ulong roleId)
		{
			try
			{
				var cache = _cacheClient.Get;

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

						await cache.UpsertAsync($"discord:guild:{guildId}", guild);
					}

					await cache.RemoveAsync($"discord:guild:{guildId}:role:{roleId}");
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnGuildRoleCreate(ulong guildId, DiscordRolePacket arg2)
		{
			try
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

				var cache = _cacheClient.Get;

				await cache.UpsertAsync($"discord:guild:{guildId}:role:{arg2.Id}", arg2);
					await cache.UpsertAsync($"discord:guild:{guildId}", guild);
				
			}
			catch (Exception e)
			{
				Log.Trace(e.ToString());
			}
		}

		private async Task OnUserUpdate(DiscordUserPacket arg)
		{
			var cache = _cacheClient.Get;

			await cache.UpsertAsync($"discord:user:{arg.Id}", arg);
		}

		private async Task OnGuildUpdate(DiscordGuildPacket arg)
		{
			var cache = _cacheClient.Get;

			await cache.UpsertAsync($"discord:guild:{arg.Id}", arg);
			
		}

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket arg)
		{
			var cache = _cacheClient.Get;

			await cache.RemoveAsync($"discord:guild:{arg.GuildId}");
			
		}

		private async Task OnGuildCreate(DiscordGuildPacket arg)
		{
			var cache = _cacheClient.Get;

			await cache.UpsertAsync($"discord:guild:{arg.Id}", arg);
			
		}

		private async Task OnGuildBanRemove(ulong arg1, DiscordUserPacket arg2)
		{
		}

		private async Task OnGuildBanAdd(ulong arg1, DiscordUserPacket arg2)
		{
		}

		private async Task OnChannelUpdate(DiscordChannelPacket arg)
		{
			var cache = _cacheClient.Get;

			await cache.UpsertAsync($"discord:channel:{arg.Id}", arg);
			
		}

		private async Task OnChannelDelete(DiscordChannelPacket arg)
		{
			var cache = _cacheClient.Get;
			await cache.RemoveAsync($"discord:channel:{arg.Id}");
		}

		private async Task OnChannelCreate(DiscordChannelPacket arg)
		{
			var cache = _cacheClient.Get;
			
			await cache.UpsertAsync($"discord:channel:{arg.Id}", arg);
		}
	}
}
