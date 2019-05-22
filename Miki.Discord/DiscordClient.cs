﻿using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Discord
{
	public partial class DiscordClient
	{
		internal IApiClient _apiClient;
		internal IGateway _gateway;

		public IExtendedCacheClient CacheClient { get; private set; }

		public DiscordClient(DiscordClientConfigurations config)
		{
			CacheClient = config.CacheClient;

			_apiClient = config.ApiClient;
			_gateway = config.Gateway;

			_gateway.OnMessageCreate += OnMessageCreate;
			_gateway.OnMessageUpdate += OnMessageUpdate;

			_gateway.OnGuildCreate += OnGuildJoin;
			_gateway.OnGuildDelete += OnGuildLeave;

			_gateway.OnGuildMemberAdd += OnGuildMemberCreate;
			_gateway.OnGuildMemberRemove += OnGuildMemberDelete;

			_gateway.OnUserUpdate += OnUserUpdate;
		}

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

		public async Task<IDiscordTextChannel> CreateDMAsync(ulong userid)
		{
			var packet = await CacheClient.HashGetAsync<DiscordChannelPacket>(CacheUtils.ChannelsKey(), userid.ToString());
			if (packet == null)
			{
				packet = await _apiClient.CreateDMChannelAsync(userid);
				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.ChannelsKey(), userid.ToString(), packet);
				}
				else
				{
					return null;
				}
			}
			return ResolveChannel(packet) as IDiscordTextChannel;
		}

		public async Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null)
			=> new DiscordRole(
				await _apiClient.CreateGuildRoleAsync(guildId, args),
				this
			);

		public async Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
			=> new DiscordRole(await _apiClient.EditRoleAsync(guildId, role), this);

		public async Task<IDiscordPresence> GetUserPresence(ulong userId)
			=> await CacheClient.HashGetAsync<DiscordPresence>(CacheUtils.GuildPresencesKey(), userId.ToString());

		public async Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
			=> new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);

		public async Task<IReadOnlyList<IDiscordRole>> GetRolesAsync(ulong guildId)
			=> (await GetRolePacketsAsync(guildId))
				.Select(x => new DiscordRole(x, this))
				.ToList();

		public async Task<IReadOnlyList<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
			=> (await GetGuildChannelPacketsAsync(guildId))
				.Select(x => ResolveChannel(x) as IDiscordGuildChannel)
				.ToList();

		public async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
		{
			var channel = await GetChannelPacketAsync(id, guildId);
			return ResolveChannel(channel);
		}

		public async Task<IDiscordUser> GetCurrentUserAsync()
		{
			return new DiscordUser(
				await GetCurrentUserPacketAsync(),
				this
			);
		}

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
				this,
				await GetGuildAsync(guildId)
			);
		}

		public async Task<IReadOnlyList<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
			var users = await _apiClient.GetReactionsAsync(channelId, messageId, emoji);

			if(users != null)
			{
				return users.Select(
					x => new DiscordUser(x, this)
				).ToList();
			}

			return new List<IDiscordUser>();
		}

		public async Task<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
				packet,
				this
			);
		}

		public async Task SetGameAsync(int shardId, DiscordStatus status)
		{
			await _gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public async Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await _apiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
			=> new DiscordMessage(
				await _apiClient.SendMessageAsync(channelId, message),
				this
			);

		public async Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null)
			=> await SendMessageAsync(channelId, new MessageArgs
			{
				content = text,
				embed = embed
			});

		internal async Task<DiscordChannelPacket> GetChannelPacketAsync(ulong id, ulong? guildId)
		{
			DiscordChannelPacket packet = await CacheClient.HashGetAsync<DiscordChannelPacket>(CacheUtils.ChannelsKey(guildId), id.ToString());

			if (packet == null)
			{
				packet = await _apiClient.GetChannelAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.ChannelsKey(guildId), id.ToString(), packet);
				}
			}
			return packet;
		}

		internal async Task<DiscordChannelPacket[]> GetGuildChannelPacketsAsync(ulong guildId)
		{
			DiscordChannelPacket[] packets = (await CacheClient.HashValuesAsync<DiscordChannelPacket>(CacheUtils.ChannelsKey(guildId)))?.ToArray();

			if ((packets?.Length ?? 0) == 0)
			{
				packets = (await _apiClient.GetChannelsAsync(guildId)).ToArray();

				if (packets?.Length > 0)
				{
					await CacheClient.HashUpsertAsync(
						CacheUtils.ChannelsKey(guildId),
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
				packet = await _apiClient.GetGuildUserAsync(userId, guildId);

				if (packet != null)
				{
					packet.GuildId = guildId;

					await CacheClient.HashUpsertAsync(CacheUtils.GuildMembersKey(guildId), userId.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId)
		{
			DiscordRolePacket packet = await CacheClient.HashGetAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId), roleId.ToString());
			if (packet == null)
			{
				packet = await _apiClient.GetRoleAsync(roleId, guildId);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.GuildRolesKey(guildId), roleId.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordRolePacket[]> GetRolePacketsAsync(ulong guildId)
		{
			DiscordRolePacket[] packets = (await CacheClient.HashValuesAsync<DiscordRolePacket>(CacheUtils.GuildRolesKey(guildId)))?.ToArray();

			if ((packets?.Length ?? 0) == 0)
			{
				packets = (await _apiClient.GetRolesAsync(guildId)).ToArray();

				if (packets?.Length > 0)
				{
					await CacheClient.HashUpsertAsync(
						CacheUtils.ChannelsKey(guildId),
						packets.Select(x => new KeyValuePair<string, DiscordRolePacket>(x.Id.ToString(), x)
					).ToArray());
				}
			}

			return packets;
		}

		internal async Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
		{
			DiscordGuildPacket packet = await CacheClient.HashGetAsync<DiscordGuildPacket>(CacheUtils.GuildsCacheKey, id.ToString());

			if (packet == null)
			{
				packet = await _apiClient.GetGuildAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.GuildsCacheKey, id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordUserPacket> GetUserPacketAsync(ulong id)
		{
			DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, id.ToString());

			if (packet == null)
			{
				packet = await _apiClient.GetUserAsync(id);

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey, id.ToString(), packet);
				}
			}

			return packet;
		}

		internal async Task<DiscordUserPacket> GetCurrentUserPacketAsync()
		{
			DiscordUserPacket packet = await CacheClient.HashGetAsync<DiscordUserPacket>(CacheUtils.UsersCacheKey, "me");

			if (packet == null)
			{
				packet = await _apiClient.GetCurrentUserAsync();

				if (packet != null)
				{
					await CacheClient.HashUpsertAsync(CacheUtils.UsersCacheKey, "me", packet);
				}
			}

			return packet;
		}

		private IDiscordChannel ResolveChannel(DiscordChannelPacket packet)
		{
			if (packet.GuildId.HasValue)
			{
				switch (packet.Type)
				{
					case ChannelType.GUILDTEXT:
					{
						return new DiscordGuildTextChannel(packet, this);
					}

					case ChannelType.GUILDVOICE:
					default:
					{
						return new DiscordGuildChannel(packet, this);
					}
				}
			}
			else
			{
				switch (packet.Type)
				{
					case ChannelType.DM:
					case ChannelType.GROUPDM:
					{
						return new DiscordTextChannel(packet, this);
					}

					default:
					{
						return new DiscordChannel(packet, this);
					}
				}
			}
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
					new DiscordGuildUser(member, this, guild)
				);
			}
		}

		private async Task OnGuildMemberCreate(DiscordGuildMemberPacket packet)
		{
			if (GuildMemberCreate != null)
			{
				IDiscordGuild guild = await GetGuildAsync(packet.GuildId);

				await GuildMemberCreate(
					new DiscordGuildUser(packet, this, guild)
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

			if (!await CacheClient.HashExistsAsync(CacheUtils.GuildsCacheKey, guild.Id.ToString()))
			{
				if (GuildJoin != null)
				{
					await GuildJoin(g);
				}
			}
			else
			{
				if (GuildAvailable != null)
				{
					await GuildAvailable(g);
				}
			}
		}

		private async Task OnGuildLeave(DiscordGuildUnavailablePacket guild)
		{
			if (GuildLeave != null)
			{
				await GuildLeave(guild.GuildId);
			}
		}

		private async Task OnUserUpdate(DiscordPresencePacket user)
		{
			if (UserUpdate != null)
			{
				await UserUpdate(
					await GetUserAsync(user.User.Id),
					new DiscordUser(user.User, this)
				);
			}
		}

		private async Task OnReady(GatewayReadyPacket readyPacket)
		{
			if (Ready != null)
			{
				await Ready(readyPacket);
			}
		}
	}
}