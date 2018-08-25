using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Rest;
using Miki.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord
{
    public partial class DiscordClient
    {
		// TODO (Veld): Privatize these again.
		public IApiClient _apiClient;
		public IGateway _gateway;

		private ICachePool _cachePool;

		public DiscordClient(DiscordClientConfigurations config)
		{
			_cachePool = config.Pool;

			_apiClient = new DiscordApiClient(
				config.Token, config.Pool
			);

			_gateway = config.Gateway;

			_gateway.OnMessageCreate += OnMessageCreate;
			_gateway.OnMessageUpdate += OnMessageUpdate;
					 
			_gateway.OnGuildCreate += OnGuildJoin;
			_gateway.OnGuildDelete += OnGuildLeave;
					 
			_gateway.OnUserUpdate += OnUserUpdate;
		}

		public async Task AddBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
			=> await _apiClient.AddGuildBanAsync(guildId, userId, pruneDays, reason);

		public async Task<IDiscordMessage> EditMessageAsync(
			ulong channelId, ulong messageId, string text, DiscordEmbed embed = null
		)
			=> new DiscordMessage(
				await _apiClient.EditMessageAsync(channelId, messageId, new EditMessageArgs
				{
					content = text,
					embed = embed
				}),
				this
			);

		public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
			=> await _apiClient.AddGuildMemberRoleAsync(guildId, userId, roleId);

		public async Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
			=> new DiscordRole(await _apiClient.EditRoleAsync(guildId, role), this);

		public async Task<IDiscordPresence> GetUserPresence(ulong userId)
			=> await _cachePool.Get.GetAsync<DiscordPresence>($"discord:user:presence:{userId}");

		public async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
			=> (await GetRolesAsync(guildId))
				.FirstOrDefault(x => x.Id == roleId);

		public async Task<IReadOnlyCollection<IDiscordRole>> GetRolesAsync(ulong guildId)
			=> (await _apiClient.GetRolesAsync(guildId))
				.Select(x => new DiscordRole(x, this))
				.ToList();

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong id, string reason = null)
			=> await _apiClient.RemoveGuildMemberAsync(guildId, id, reason);

		public string GetUserAvatarUrl(ulong id, string hash)
			=> DiscordHelper.GetAvatarUrl(id, hash);

		public string GetUserAvatarUrl(ushort discriminator)
			=> DiscordHelper.GetAvatarUrl(discriminator);

		public async Task<IReadOnlyCollection<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
			=> (await _apiClient.GetChannelsAsync(guildId))
				.Select(x => new DiscordGuildChannel(x, this))
				.ToList();

		public async Task<IDiscordChannel> GetChannelAsync(ulong id)
		{
			var packet = await _apiClient.GetChannelAsync(id);

			if (packet.GuildId != null)
			{
				return new DiscordGuildChannel(packet, this);
			}
			return new DiscordChannel(packet, this);
		}

		public async Task<IDiscordUser> GetCurrentUserAsync()
		{
			var me = await _apiClient.GetCurrentUserAsync();

			return new DiscordUser(
				me,
				this
			);
		}

		public async Task<IDiscordChannel> CreateDMAsync(ulong userid)
		{
			return new DiscordChannel(
				await _apiClient.CreateDMChannelAsync(userid), 
				this
			);
		}

		public async Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null)
			=> new DiscordRole(
				await _apiClient.CreateGuildRoleAsync(guildId, args),
				this
			);

		public async Task<IDiscordGuild> GetGuildAsync(ulong id)
			=> new DiscordGuild(
				await _apiClient.GetGuildAsync(id),
				this
			);

		public async Task<IDiscordUser> GetUserAsync(ulong id)
			=> new DiscordUser(
				await _apiClient.GetUserAsync(id),
				this
			);

		public async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
			=> (await GetGuildAsync(guildId)).GetMember(id);

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
			=> await _apiClient.DeleteMessageAsync(channelId, messageId);

		public async Task RemoveBanAsync(ulong guildId, ulong userId)
			=> await _apiClient.RemoveGuildBanAsync(guildId, userId);

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
			=> await _apiClient.RemoveGuildMemberRoleAsync(guildId, userId, roleId);

		public async Task SetGameAsync(int shardId, DiscordStatus status)
		{
			await _gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public async Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await _apiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message, bool toChannel)
			=> new DiscordMessage(
				await _apiClient.SendMessageAsync(channelId, message, toChannel),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null, bool toChannel = true)
			=> await SendMessageAsync(channelId, new MessageArgs
			{
				content = text,
				embed  = embed
			}, toChannel);
	}

	public partial class DiscordClient
	{
		public event Func<IDiscordMessage, Task> MessageCreate;
		public event Func<IDiscordMessage, Task> MessageUpdate;

		public event Func<IDiscordGuild, Task> GuildJoin;
		public event Func<IDiscordGuild, Task> GuildAvailable;

		public event Func<ulong, Task> GuildLeave;

		public event Func<GatewayReadyPacket, Task> Ready;

		public event Func<IDiscordUser, IDiscordUser, Task> UserUpdate;

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
			ICacheClient cache = _cachePool.Get;
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
