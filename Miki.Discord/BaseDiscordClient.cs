using Miki.Discord.Common;
using Miki.Discord.Common.Extensions;
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
	public abstract class BaseDiscordClient : IDiscordClient
	{
		/// <summary>
		/// The api client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
		/// </summary>
		public IApiClient ApiClient { get; }

		/// <summary>
		/// The gateway client used in the discord client and was given in <see cref="DiscordClientConfigurations"/> at the beginning.
		/// </summary>
		public IGateway Gateway { get; }

		protected BaseDiscordClient(IApiClient apiClient, IGateway gateway)
		{
			ApiClient = apiClient;
			Gateway = gateway;

			Gateway.OnMessageCreate += OnMessageCreate;
			Gateway.OnMessageUpdate += OnMessageUpdate;

			Gateway.OnGuildCreate += OnGuildJoin;
			Gateway.OnGuildDelete += OnGuildLeave;

			Gateway.OnGuildMemberAdd += OnGuildMemberCreate;
			Gateway.OnGuildMemberRemove += OnGuildMemberDelete;

			Gateway.OnUserUpdate += OnUserUpdate;

			Gateway.OnReady += OnReady;
		}

		protected abstract Task<DiscordUserPacket> GetCurrentUserPacketAsync();

		protected abstract Task<DiscordUserPacket> GetUserPacketAsync(ulong id);

		protected abstract Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id);

		protected abstract Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId);

		protected abstract Task<IEnumerable<DiscordGuildMemberPacket>> GetGuildMembersPacketAsync(ulong guildId);

		protected abstract Task<IEnumerable<DiscordChannelPacket>> GetGuildChannelPacketsAsync(ulong guildId);

		protected abstract Task<IEnumerable<DiscordRolePacket>> GetRolePacketsAsync(ulong guildId);

		protected abstract Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId);

		protected abstract Task<DiscordChannelPacket> GetChannelPacketAsync(ulong id, ulong? guildId = null);

		protected abstract Task<bool> IsGuildNewAsync(ulong guildId);

		public virtual async Task<IDiscordMessage> EditMessageAsync(
			ulong channelId, ulong messageId, string text, DiscordEmbed embed = null)
		{
			return new DiscordMessage(
				await ApiClient.EditMessageAsync(channelId, messageId, new EditMessageArgs
				{
					Content = text,
					Embed = embed
				}),
				this
			);
		}

		public virtual async Task<IDiscordTextChannel> CreateDMAsync(
			ulong userid)
		{
			var channel = await ApiClient.CreateDMChannelAsync(userid);

			return ResolveChannel(channel) as IDiscordTextChannel;
		}

		public virtual async Task<IDiscordRole> CreateRoleAsync(
			ulong guildId,
			CreateRoleArgs args = null)
		{
			return new DiscordRole(
				await ApiClient.CreateGuildRoleAsync(guildId, args),
				this
			);
		}

		public virtual async Task<IDiscordRole> EditRoleAsync(
			ulong guildId,
			DiscordRolePacket role)
		{
			return new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);
		}

		public virtual async Task<IDiscordPresence> GetUserPresence(
			ulong userId,
			ulong? guildId = null)
		{
			if(!guildId.HasValue)
			{
				throw new NotSupportedException("The default Discord Client cannot get the presence of the user without the guild ID. Use the cached client instead.");
			}

			// We have to get the guild because there is no API end-point for user presence.
			// This is a known issue: https://github.com/discordapp/discord-api-docs/issues/666

			var guild = await GetGuildPacketAsync(guildId.Value);
			var presence = guild.Presences.FirstOrDefault(p => p.User.Id == userId);

			return presence != null ? new DiscordPresence(presence) : null;
		}

		public virtual async Task<IDiscordRole> GetRoleAsync(
			ulong guildId,
			ulong roleId)
		{
			return new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);
		}

		public virtual async Task<IEnumerable<IDiscordRole>> GetRolesAsync(
			ulong guildId)
		{
			return (await GetRolePacketsAsync(guildId))
				.Select(x => new DiscordRole(x, this));
		}

		public virtual async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
		{
			var channelPackets = await GetGuildChannelPacketsAsync(guildId);

			return channelPackets.Select(x => ResolveChannel(x) as IDiscordGuildChannel);
		}

		public virtual async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
		{
			var channel = await GetChannelPacketAsync(id, guildId);

			return ResolveChannel(channel);
		}

		public virtual async Task<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel
		{
			var channel = await GetChannelPacketAsync(id, guildId);

			return ResolveChannel(channel) as T;
		}

		public virtual async Task<IDiscordSelfUser> GetSelfAsync()
		{
			return new DiscordSelfUser(
				await GetCurrentUserPacketAsync(),
				this);
		}

		public virtual async Task<IDiscordGuild> GetGuildAsync(ulong id)
		{
			var packet = await GetGuildPacketAsync(id);

			return new DiscordGuild(
				packet,
				this
			);
		}

		public virtual async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
		{
			return new DiscordGuildUser(
				await GetGuildMemberPacketAsync(id, guildId),
				this
			);
		}

		public async Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
		{
			return (await GetGuildMembersPacketAsync(guildId))
				.Select(x => new DiscordGuildUser(x, this));
		}

		public virtual async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
			var users = await ApiClient.GetReactionsAsync(channelId, messageId, emoji);

			if(users != null)
			{
				return users.Select(
					x => new DiscordUser(x, this)
				);
			}

			return new List<IDiscordUser>();
		}

		public virtual async Task<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
				packet,
				this
			);
		}

		public virtual async Task SetGameAsync(int shardId, DiscordStatus status)
		{
			await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public virtual async Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await ApiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public virtual async Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
			=> new DiscordMessage(
				await ApiClient.SendMessageAsync(channelId, message),
				this
			);

		public virtual async Task<IDiscordMessage> SendMessageAsync(ulong channelId, string text, DiscordEmbed embed = null)
			=> await SendMessageAsync(channelId, new MessageArgs
			{
				Content = text,
				Embed = embed
			});

		protected IDiscordChannel ResolveChannel(DiscordChannelPacket packet)
		{
			switch(packet.Type)
			{
				case ChannelType.GUILDTEXT:
					return new DiscordGuildTextChannel(packet, this);

				case ChannelType.CATEGORY:
				case ChannelType.GUILDVOICE:
					return new DiscordGuildChannel(packet, this);

				case ChannelType.DM:
				case ChannelType.GROUPDM:
					return new DiscordTextChannel(packet, this);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public virtual event Func<IDiscordMessage, Task> MessageCreate;
		public virtual event Func<IDiscordMessage, Task> MessageUpdate;

		public virtual event Func<IDiscordGuild, Task> GuildJoin;
		public virtual event Func<IDiscordGuild, Task> GuildAvailable;

		public virtual event Func<IDiscordGuildUser, Task> GuildMemberCreate;
		public virtual event Func<IDiscordGuildUser, Task> GuildMemberDelete;

		public virtual event Func<ulong, Task> GuildLeave;
		public virtual event Func<ulong, Task> GuildUnavailable;

		public virtual event Func<GatewayReadyPacket, Task> Ready;

		public virtual event Func<IDiscordUser, IDiscordUser, Task> UserUpdate;

		private async Task OnGuildMemberDelete(ulong guildId, DiscordUserPacket packet)
		{
			DiscordGuildMemberPacket member = await GetGuildMemberPacketAsync(packet.Id, guildId);

			await GuildMemberDelete.InvokeAsync(
				new DiscordGuildUser(member, this)
			);
		}

		private Task OnGuildMemberCreate(DiscordGuildMemberPacket packet)
		{
			return GuildMemberCreate.InvokeAsync(
				new DiscordGuildUser(packet, this));
		}

		private Task OnMessageCreate(DiscordMessagePacket packet)
		{
			return MessageCreate.InvokeAsync(
				new DiscordMessage(packet, this));
		}

		private Task OnMessageUpdate(DiscordMessagePacket packet)
		{
			return MessageUpdate.InvokeAsync(new DiscordMessage(packet, this));
		}

		private async Task OnGuildJoin(DiscordGuildPacket guild)
		{
			var g = new DiscordGuild(guild, this);

			if(await IsGuildNewAsync(guild.Id))
			{
				await GuildJoin.InvokeAsync(g);
			}
			else
			{
				await GuildAvailable.InvokeAsync(g);
			}
		}

		private Task OnGuildLeave(DiscordGuildUnavailablePacket guild)
		{
			if(guild.IsUnavailable.GetValueOrDefault(false))
			{
				return GuildUnavailable.InvokeAsync(guild.GuildId);
			}

			return GuildLeave.InvokeAsync(guild.GuildId);
		}

		private async Task OnUserUpdate(DiscordPresencePacket user)
		{
			await UserUpdate.InvokeAsync(
				await GetUserAsync(user.User.Id),
				new DiscordUser(user.User, this)
			);
		}

		private async Task OnReady(GatewayReadyPacket readyPacket)
		{
			await Ready.InvokeAsync(readyPacket);
		}

		public virtual void Dispose()
		{
			Gateway.OnMessageCreate -= OnMessageCreate;
			Gateway.OnMessageUpdate -= OnMessageUpdate;

			Gateway.OnGuildCreate -= OnGuildJoin;
			Gateway.OnGuildDelete -= OnGuildLeave;

			Gateway.OnGuildMemberAdd -= OnGuildMemberCreate;
			Gateway.OnGuildMemberRemove -= OnGuildMemberDelete;

			Gateway.OnUserUpdate -= OnUserUpdate;
		}
	}
}