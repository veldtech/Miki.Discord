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

		public ICachePool CachePool { get; private set; }
		private IExtendedCacheClient _cacheClient;

		public DiscordClient(DiscordClientConfigurations config)
		{
			CachePool = config.Pool;
			_cacheClient = CachePool.GetAsync().Result as IExtendedCacheClient;

			ApiClient = new DiscordApiClient(
				config.Token, CachePool.GetAsync().Result
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
			=> await _cacheClient.GetAsync<DiscordPresence>($"discord:user:presence:{userId}");

		public async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
			=> (await GetRolesAsync(guildId))
				.FirstOrDefault(x => x.Id == roleId);

		public async Task<IReadOnlyCollection<IDiscordRole>> GetRolesAsync(ulong guildId)
			=> (await ApiClient.GetRolesAsync(guildId))
				.Select(x => new DiscordRole(x, this))
				.ToList();

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong id, string reason = null)
			=> await ApiClient.RemoveGuildMemberAsync(guildId, id, reason);

		public string GetUserAvatarUrl(ulong id, string hash)
			=> DiscordHelper.GetAvatarUrl(id, hash);

		public string GetUserAvatarUrl(ushort discriminator)
			=> DiscordHelper.GetAvatarUrl(discriminator);

		public async Task<IReadOnlyCollection<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
			=> (await ApiClient.GetChannelsAsync(guildId))
				.Select(x => new DiscordGuildChannel(x, this))
				.ToList();

		public async Task<IDiscordChannel> GetDMChannelAsync(ulong id)
		{
			DiscordChannelPacket packet = await GetDMChannelPacketAsync(id);

			return new DiscordChannel(packet, this);
		}

		public async Task<IDiscordGuildChannel> GetGuildChannelAsync(ulong id, ulong guildId)
		{
			DiscordChannelPacket packet = await GetGuildChannelPacketAsync(id, guildId);
			return new DiscordGuildChannel(packet, this);
		}

		public async Task<IDiscordUser> GetCurrentUserAsync()
		{
			var me = await ApiClient.GetCurrentUserAsync();

			return new DiscordUser(
				me,
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

		public async Task<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
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
			DiscordChannelPacket packet = await _cacheClient.HashGetAsync<DiscordChannelPacket>($"discord:channels", id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetChannelAsync(id);

				if (packet != null)
				{
					await _cacheClient.HashUpsertAsync($"discord:channels", id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordChannelPacket> GetGuildChannelPacketAsync(ulong id, ulong guildId)
		{
			DiscordChannelPacket packet = await _cacheClient.HashGetAsync<DiscordChannelPacket>($"discord:channels:{guildId}", id.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetChannelAsync(id);

				if (packet != null)
				{
					await _cacheClient.HashUpsertAsync($"discord:channels:{guildId}", id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId)
		{
			DiscordGuildMemberPacket packet = await _cacheClient.HashGetAsync<DiscordGuildMemberPacket>($"discord:users:{guildId}", userId.ToString());

			if (packet == null)
			{
				packet = await ApiClient.GetGuildUserAsync(userId, guildId);
				await _cacheClient.UpsertAsync($"discord:users:{guildId}", packet);
			}

			return packet;
		}

		internal async Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
		{
			DiscordGuildPacket packet = await _cacheClient.GetAsync<DiscordGuildPacket>($"discord:guild:{id}");

			if (packet == null)
			{
				packet = await ApiClient.GetGuildAsync(id);
				await _cacheClient.UpsertAsync($"discord:guild:{id}", packet);
			}

			return packet;
		}

		internal async Task<DiscordUserPacket> GetUserPacketAsync(ulong id)
		{
			DiscordUserPacket packet = await _cacheClient.GetAsync<DiscordUserPacket>($"discord:user:{id}");

			if (packet == null)
			{
				packet = await ApiClient.GetUserAsync(id);
				await _cacheClient.UpsertAsync($"discord:user:{id}", packet);
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

				await GuildMemberDelete(
					await guild.GetMemberAsync(packet.Id)
				);
			}
		}

		private async Task OnGuildMemberCreate(DiscordGuildMemberPacket packet)
		{
			if (GuildMemberCreate != null)
			{
				IDiscordGuild guild = await GetGuildAsync(packet.GuildId);
				DiscordUserPacket p = await ApiClient.GetUserAsync(packet.User.Id);

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
			ICacheClient cache = await CachePool.GetAsync();
			DiscordGuild g = new DiscordGuild(guild, this);

			if (!await cache.ExistsAsync($"discord:guild:{guild.Id}"))
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
