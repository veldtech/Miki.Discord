using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
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
		string ChannelCacheKey(ulong id)
			=> $"discord:channel:{id}";

		string GuildCacheKey(ulong id)
			=> $"discord:guild:{id}";

		string UserCacheKey(ulong id)
			=> $"discord:user:{id}";

		string UserInGuilds(ulong id)
			=> $"{UserCacheKey(id)}:guilds";
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

			client.OnUserUpdate += OnUserUpdate;
		}

		private async Task OnUserUpdate(DiscordUserPacket user, ICacheClient cache)
		{
			await cache.UpsertAsync(UserCacheKey(user.Id), user);

			HashSet<ulong> guildsFromUser = await cache.GetAsync<HashSet<ulong>>(UserInGuilds(user.Id));

			foreach(ulong u in guildsFromUser)
			{
				DiscordGuildPacket p = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(u));

				if(p == null)
				{
					continue;
				}

				int index = p.Members.FindIndex(x => x.User.Id == user.Id);

				if(index != -1)
				{
					p.Members[index].User = user;
				}

				await cache.UpsertAsync(GuildCacheKey(u), p);
			}

		}

		private async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs member, ICacheClient cache)
		{
			DiscordGuildPacket p = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(member.GuildId));

			if (p == null)
			{
				return;
			}

			int index = p.Members.FindIndex(x => x.User.Id == member.User.Id);

			if (index != -1)
			{
				p.Members[index].User = member.User;
				p.Members[index].Roles = member.RoleIds.ToList();
				p.Members[index].Nickname = member.Nickname;
			}

			await cache.UpsertAsync(GuildCacheKey(member.GuildId), p);
		}

		private async Task OnGuildMemberRemove(ulong guildId, DiscordUserPacket user, ICacheClient cache)
		{
			DiscordGuildPacket p = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(guildId));

			if (p == null)
			{
				return;
			}

			p.Members.RemoveAll(x => x.User.Id == user.Id);
			p.MemberCount = p.Members.Count;

			await cache.UpsertAsync(GuildCacheKey(guildId), p);

			HashSet<ulong> allGuilds = await cache.GetAsync<HashSet<ulong>>(UserInGuilds(user.Id));

			if (allGuilds == null)
			{
				allGuilds = new HashSet<ulong>();
			}

			allGuilds.Remove(guildId);

			await cache.UpsertAsync(UserInGuilds(user.Id), allGuilds);
		}

		private async Task OnGuildMemberAdd(DiscordGuildMemberPacket member, ICacheClient cache)
		{
			DiscordGuildPacket p = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(member.GuildId));

			if (p == null)
			{
				return;
			}

			if(p.Members == null)
			{
				p.Members = new List<DiscordGuildMemberPacket>();
			}

			int index = p.Members.FindIndex(x => x.User.Id == member.User.Id);

			if (index == -1)
			{
				p.Members.Add(member);
			}
			else
			{
				p.Members[index] = member;
			}

			p.MemberCount = p.Members.Count;

			await cache.UpsertAsync(GuildCacheKey(member.GuildId), p);

			HashSet<ulong> allGuilds = await cache.GetAsync<HashSet<ulong>>(UserInGuilds(member.User.Id));

			if(allGuilds == null)
			{
				allGuilds = new HashSet<ulong>();
			}

			allGuilds.Add(member.GuildId);

			await cache.UpsertAsync(UserInGuilds(member.User.Id), allGuilds);
		}

		private async Task OnGuildDelete(DiscordGuildUnavailablePacket unavailableGuild, ICacheClient cache)
		{
			DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(unavailableGuild.GuildId));

			if(guild != null)
			{
				foreach(var user in guild.Members)
				{
					HashSet<ulong> userInGuild = await cache.GetAsync<HashSet<ulong>>(UserInGuilds(user.User.Id));

					if (userInGuild != null)
					{
						userInGuild.Remove(guild.Id);
						await cache.UpsertAsync(UserInGuilds(user.User.Id), userInGuild);
					}
				}
			}

			await cache.RemoveAsync(GuildCacheKey(unavailableGuild.GuildId));
		}

		private async Task OnGuildUpdate(DiscordGuildPacket arg1, ICacheClient cache)
		{
			DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(arg1.Id));

			if(guild == null)
			{
				// TODO (Veld): Consider logging this.
				return;
			}

			guild.OverwriteContext(arg1);

			await cache.UpsertAsync(GuildCacheKey(arg1.Id), guild);
		}

		private async Task OnGuildCreate(DiscordGuildPacket guild, ICacheClient cache)
		{
			await cache.UpsertAsync(GuildCacheKey(guild.Id), guild);

			foreach (var user in guild.Members)
			{
				HashSet<ulong> userInGuild = await cache.GetAsync<HashSet<ulong>>(UserInGuilds(user.User.Id));

				if(userInGuild == null)
				{
					userInGuild = new HashSet<ulong>();
				}

				userInGuild.Add(guild.Id);
				await cache.UpsertAsync(UserInGuilds(user.User.Id), userInGuild);
			}
		}

		private async Task OnChannelCreate(DiscordChannelPacket channel, ICacheClient cache)
		{
			if(channel.GuildId.HasValue)
			{
				DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(channel.GuildId.Value));

				if (guild != null)
				{
					int index = guild.Channels.FindIndex(x => x.Id == channel.Id);

					if (index != -1)
					{
						guild.Channels[index] = channel;
					}
					else
					{
						guild.Channels.Add(channel);
					}

					await cache.UpsertAsync(GuildCacheKey(channel.GuildId.Value), guild);
				}
			}

			await cache.UpsertAsync(ChannelCacheKey(channel.Id), channel);
		}

		private async Task OnChannelDelete(DiscordChannelPacket arg, ICacheClient cache)
		{
			if (arg.GuildId.HasValue)
			{
				DiscordGuildPacket guild = await cache.GetAsync<DiscordGuildPacket>(GuildCacheKey(arg.GuildId.Value));

				if (guild != null)
				{
					guild.Channels.RemoveAll(x => x.Id == arg.Id);
					await cache.UpsertAsync(GuildCacheKey(arg.GuildId.Value), guild);
				}
			}
			await cache.RemoveAsync(ChannelCacheKey(arg.Id));
		}
	}
}
