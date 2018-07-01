using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using Miki.Discord.Messaging;
using Miki.Discord.Rest;
using Miki.Discord.Rest.Entities;
using Miki.Rest;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
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
		public DiscordApiClient _apiClient;
		public MessageClient _websocketClient;

		public DiscordClient(DiscordClientConfigurations config)
		{
			_apiClient = new DiscordApiClient(
				config.Token, config.CacheClient
			);

			_websocketClient = new MessageClient(
				new MessageClientConfiguration
				{
					Token = config.Token,
					DatabaseClient = config.CacheClient,
					ExchangeName = config.RabbitMQExchangeName,
					MessengerConfigurations = config.RabbitMQUri,
					QueueName = config.RabbitMQQueueName
				}
			);

			_websocketClient.MessageCreate += OnMessageCreate;
			_websocketClient.MessageUpdate += OnMessageUpdate;

			_websocketClient.GuildCreate += OnGuildJoin;
			_websocketClient.GuildDelete += OnGuildLeave;

			_websocketClient.UserUpdate += OnUserUpdate;

			_websocketClient.Start();
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

		public async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
			=> (await GetRolesAsync(guildId))
				.FirstOrDefault(x => x.Id == roleId);

		public async Task<IReadOnlyCollection<IDiscordRole>> GetRolesAsync(ulong guildId)
			=> (await _apiClient.GetRolesAsync(guildId))
				.Select(x => new DiscordRole(x, this))
				.ToList();

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong id)
			=> await _apiClient.RemoveGuildMemberAsync(guildId, id);

		public string GetUserAvatarUrl(ulong id, string hash)
			=> _apiClient.GetUserAvatarUrl(id, hash);

		public async Task<IReadOnlyCollection<IDiscordChannel>> GetChannelsAsync(ulong guildId)
			=> (await _apiClient.GetChannelsAsync(guildId))
				.Select(x => (x.GuildId != 0) 
					? new DiscordGuildChannel(x, this) 
					: new DiscordChannel(x, this)
				).ToList();

		public async Task<IDiscordChannel> GetChannelAsync(ulong id)
		{
			var packet = await _apiClient.GetChannelAsync(id);

			if (packet.GuildId != 0)
				return new DiscordGuildChannel(packet, this);
			return new DiscordChannel(packet, this);
		}

		public async Task<IDiscordUser> GetCurrentUserAsync()
			=> new DiscordUser(
				await _apiClient.GetCurrentUserAsync(), 
				this
			);

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
			=> new DiscordGuildUser(
				await _apiClient.GetGuildUserAsync(id, guildId), 
				this
			);

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
			=> await _apiClient.DeleteMessageAsync(channelId, messageId);

		public async Task RemoveBanAsync(ulong guildId, ulong userId)
			=> await _apiClient.RemoveGuildBanAsync(guildId, userId);

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
			=> await _apiClient.RemoveGuildMemberRoleAsync(guildId, userId, roleId);

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

	// Events
	public partial class DiscordClient
	{
		public event Func<IDiscordMessage, Task> MessageCreate;
		public event Func<IDiscordMessage, Task> MessageUpdate;

		public event Func<IDiscordGuild, Task> GuildJoin;
		public event Func<ulong, Task> GuildLeave;

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
			if(GuildJoin != null)
			{
				await GuildJoin(new DiscordGuild(guild, this));
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
	}
}
