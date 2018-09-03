using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord
{
	public partial class DiscordClient
    {
		public IApiClient ApiClient { get; private set; }
		public IGateway Gateway { get; private set; }

		public IExtendedCacheClient CacheClient { get; private set; }

		public DiscordClient(DiscordClientConfigurations config)
		{
			CacheClient = config.CacheClient;

			ApiClient = new DiscordApiClient(
				config.Token, CacheClient
			);

			Gateway = config.Gateway;

			Gateway.OnMessageCreate += OnMessageCreate;
			Gateway.OnMessageUpdate += OnMessageUpdate;
					 
			Gateway.OnGuildCreate += OnGuildJoin;
			Gateway.OnGuildDelete += OnGuildLeave;

			Gateway.OnGuildMemberAdd += OnGuildMemberCreate;
			Gateway.OnGuildMemberRemove += OnGuildMemberDelete;
					 
			Gateway.OnUserUpdate += OnUserUpdate;
		}

		public async Task AddBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
			=> await ApiClient.AddGuildBanAsync(guildId, userId, pruneDays, reason);

		public async Task<IDiscordMessage> EditMessageAsync(
			ulong channelId, ulong messageId, string text, DiscordEmbed embed = null
		)
			=> new DiscordMessage(
				await ApiClient.EditMessageAsync(channelId, messageId, new EditMessageArgs
				{
					content = text,
					embed = embed
				}),
				this
			);

		public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
			=> await ApiClient.AddGuildMemberRoleAsync(guildId, userId, roleId);

		public async Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
			=> new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);

		public async Task<IDiscordPresence> GetUserPresence(ulong userId)
			=> await CacheClient.HashGetAsync<DiscordPresence>(CacheUtils.GuildPresencesKey(), userId.ToString());

		public async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
			=> new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);

		public async Task<IReadOnlyList<IDiscordRole>> GetRolesAsync(ulong guildId)
			=> (await GetRolePacketsAsync(guildId))
				.Select(x => new DiscordRole(x, this))
				.ToList();

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong id, string reason = null)
			=> await ApiClient.RemoveGuildMemberAsync(guildId, id, reason);

		public string GetUserAvatarUrl(ulong id, string hash)
			=> DiscordHelper.GetAvatarUrl(id, hash);

		public string GetUserAvatarUrl(ushort discriminator)
			=> DiscordHelper.GetAvatarUrl(discriminator);

		public async Task<IReadOnlyList<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
			=> (await GetGuildChannelPacketsAsync(guildId))
				.Select(x => new DiscordGuildChannel(x, this))
				.ToList();

		public async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
		{		
			if(guildId.HasValue)
			{
				return new DiscordGuildChannel(await GetGuildChannelPacketAsync(id, guildId.Value), this);
			}
			else
			{
				return new DiscordChannel(await GetDMChannelPacketAsync(id), this);
			}
		}

		public async Task<IDiscordUser> GetCurrentUserAsync()
		{
			return new DiscordUser(
				await GetCurrentUserPacketAsync(),
				this
			);
		}

		public async Task<IDiscordChannel> CreateDMAsync(ulong userid)
		{
			return new DiscordChannel(
				await ApiClient.CreateDMChannelAsync(userid), 
				this
			);
		}

		public async Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null)
			=> new DiscordRole(
				await ApiClient.CreateGuildRoleAsync(guildId, args),
				this
			);

		public async Task<IDiscordGuild> GetGuildAsync(ulong id)
		{
			var packet = await GetGuildPacketAsync(id);

			return new DiscordGuild(
				packet,
				this
			);
		}

		public async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
		{
			return new DiscordGuildUser(
				await GetGuildMemberPacketAsync(id, guildId),
				await GetUserPacketAsync(id),
				this,
				await GetGuildAsync(guildId)
			);
		}

		public async Task<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
				packet,
				this
			);
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
			=> await ApiClient.DeleteMessageAsync(channelId, messageId);

		public async Task RemoveBanAsync(ulong guildId, ulong userId)
			=> await ApiClient.RemoveGuildBanAsync(guildId, userId);

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
			=> await ApiClient.RemoveGuildMemberRoleAsync(guildId, userId, roleId);

		public async Task SetGameAsync(int shardId, DiscordStatus status)
		{
			await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public async Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await ApiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message, bool toChannel)
			=> new DiscordMessage(
				await ApiClient.SendMessageAsync(channelId, message, toChannel),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null, bool toChannel = true)
			=> await SendMessageAsync(channelId, new MessageArgs
			{
				content = text,
				embed  = embed
			}, toChannel);


		internal async Task<DiscordChannelPacket> GetDMChannelPacketAsync(ulong id)
		{
			DiscordChannelPacket packet = await CacheClient.HashGetAsync<DiscordChannelPacket>(CacheUtils.DirectChannelsKey(), id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetChannelAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.DirectChannelsKey(), id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordChannelPacket> GetGuildChannelPacketAsync(ulong id, ulong guildId)
		{
			DiscordChannelPacket packet = await CacheClient.HashGetAsync<DiscordChannelPacket>(CacheUtils.GuildChannelsKey(guildId), id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetChannelAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.GuildChannelsKey(guildId), id.ToString(), packet);
				}
			}
			return packet;
		}

		internal async Task<DiscordChannelPacket[]> GetGuildChannelPacketsAsync(ulong guildId)
		{
			DiscordChannelPacket[] packets = await CacheClient.HashValuesAsync<DiscordChannelPacket>(CacheUtils.GuildChannelsKey(guildId)); 

			if((packets?.Length ?? 0) == 0)
			{
				packets = (await ApiClient.GetChannelsAsync(guildId)).ToArray();

				if(packets?.Length > 0)
				{
					await CacheClient.HashUpsertAsync(
						CacheUtils.GuildChannelsKey(guildId), 
						packets.Select(x => new KeyValuePair<string, DiscordChannelPacket>(x.Id.ToString(), x)
					).ToArray());
				}
			}

			return packets;
		}

		internal async Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId)
		{
			DiscordGuildMemberPacket packet = await CacheClient.HashGetAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guildId), userId.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetGuildUserAsync(userId, guildId);

				if (packet != null)
				{
					packet.GuildId = guildId;

					await CacheClient.UpsertAsync(CacheUtils.GuildMembersKey(guildId), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId)
		{
			DiscordRolePacket packet = await CacheClient.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId), roleId.ToString());
			if (packet == null)
			{
				packet = await ApiClient.GetRoleAsync(roleId, guildId);

				if (packet != null)
				{
					await CacheClient.UpsertAsync(CacheUtils.GuildRolesKey(guildId), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordRolePacket[]> GetRolePacketsAsync(ulong guildId)
		{
			DiscordRolePacket[] packets = await CacheClient.HashValuesAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId));

			if ((packets?.Length ?? 0) == 0)
			{
				packets = (await ApiClient.GetRolesAsync(guildId)).ToArray();

				if (packets?.Length > 0)
				{
					await CacheClient.HashUpsertAsync(
						CacheUtils.GuildChannelsKey(guildId),
						packets.Select(x => new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x)
					).ToArray());
				}
			}

			return packets;
		}


		internal async Task<DiscordGuildMemberPacket[]> GetGuildMemberPacketsAsync(ulong guildId)
		{
			return await CacheClient.HashValuesAsync<DiscordGuildMemberPacket>(CacheUtils.GuildMembersKey(guildId));
		}

		internal async Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
		{
			DiscordGuildPacket packet = await CacheClient.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey(), id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetGuildAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.GuildsCacheKey(), id.ToString(), packet);
				}
			}	

			return packet;
		}

		internal async Task<DiscordUserPacket> GetUserPacketAsync(ulong id)
		{
			DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey(), id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetUserAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey(), id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordUserPacket> GetCurrentUserPacketAsync()
		{
			DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey(), "me");

			if (packet == null)
			{
				packet = await ApiClient.GetCurrentUserAsync();

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey(), "me", packet);
				}
			}

			return packet;
		}
	}

	public partial class DiscordClient
	{
		public event Func<IDiscordMessage, Task> MessageCreate;
		public event Func<IDiscordMessage, Task> MessageUpdate;

		public event Func<IDiscordGuild, Task> GuildJoin;
		public event Func<IDiscordGuild, Task> GuildAvailable;

		public event Func<IDiscordGuildUser, Task> GuildMemberCreate;
		public event Func<IDiscordGuildUser, Task> GuildMemberDelete;

		public event Func<ulong, Task> GuildLeave;

		public event Func<GatewayReadyPacket, Task> Ready;

		public event Func<IDiscordUser, IDiscordUser, Task> UserUpdate;
	
		private async Task OnGuildMemberDelete(ulong guildId, DiscordUserPacket packet)
		{
			if (GuildMemberDelete != null)
			{
				IDiscordGuild guild = await GetGuildAsync(guildId);
				DiscordGuildMemberPacket member = await GetGuildMemberPacketAsync(packet.Id, guildId);

				await GuildMemberDelete(
					new DiscordGuildUser(member, packet, this, guild)
				);
			}
		}

		private async Task OnGuildMemberCreate(DiscordGuildMemberPacket packet)
		{
			if (GuildMemberCreate != null)
			{
				IDiscordGuild guild = await GetGuildAsync(packet.GuildId);
				DiscordUserPacket p = await GetUserPacketAsync(packet.User.Id);

				await GuildMemberCreate(
					new DiscordGuildUser(packet, p, this, guild)
				);
			}
		}

		private async Task OnMessageCreate(DiscordMessagePacket packet)
		{
			if (MessageCreate != null)
			{
				await MessageCreate(new DiscordMessage(packet, this));
			}
		}

		private async Task OnMessageUpdate(DiscordMessagePacket packet)
		{
			if (MessageUpdate != null)
			{
				await MessageUpdate(new DiscordMessage(packet, this));
			}
		}

		private async Task OnGuildJoin(DiscordGuildPacket guild)
		{
			DiscordGuild g = new DiscordGuild(guild, this);

			if (!await CacheClient.HashExistsAsync(CacheUtils.GuildsCacheKey(), guild.Id.ToString()))
			{
				if (GuildJoin != null)
				{
					await GuildJoin(g);
				}
			}
			else
			{
				if(GuildAvailable != null)
				{
					await GuildAvailable(g);
				}
			}
		}

		private async Task OnGuildLeave(DiscordGuildUnavailablePacket guild)
		{
			if(GuildLeave != null)
			{
				await GuildLeave(guild.GuildId);
			}
		}

		private async Task OnUserUpdate(DiscordUserPacket user)
		{
			if(UserUpdate != null)
			{
				await UserUpdate(
					await GetUserAsync(user.Id),
					new DiscordUser(user, this)
				);
			}
		}
	
		private async Task OnReady(GatewayReadyPacket readyPacket)
		{
			if(Ready != null)
			{
				await Ready(readyPacket);
			}
		}
	}
}
