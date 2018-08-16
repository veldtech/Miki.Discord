using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Miki.Discord.Mocking
{
	public class DummyApiClient : IApiClient
	{
		public Task AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
		{
			return Task.CompletedTask;
		}

		public Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			return Task.CompletedTask;
		}

		public Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
		{
			throw new NotImplementedException();
		}

		public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			return new DiscordRolePacket
			{
				Color = args.Color ?? 0,
				Id = 0,
				IsHoisted = args.Hoisted ?? false,
				Mentionable = args.Mentionable ?? false,
				Permissions = (int?)args.Permissions ?? 0,
				Name = args.Name,
			};
		}

		public Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
		{
			throw new NotImplementedException();
		}

		public Task<List<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordUserPacket> GetCurrentUserAsync()
		{
			throw new NotImplementedException();
		}

		public Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId)
		{
			throw new NotImplementedException();
		}

		public Task<List<DiscordMessagePacket>> GetMessagesAsync(ulong channelId)
		{
			throw new NotImplementedException();
		}

		public Task<List<DiscordRolePacket>> GetRolesAsync(ulong guildId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordUserPacket> GetUserAsync(ulong userId)
		{
			throw new NotImplementedException();
		}

		public Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
		{
			throw new NotImplementedException();
		}

		public Task RemoveGuildBanAsync(ulong guildId, ulong userId)
		{
			throw new NotImplementedException();
		}

		public Task RemoveGuildMemberAsync(ulong guildId, ulong userId)
		{
			throw new NotImplementedException();
		}

		public Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args, bool toChannel = true)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args, bool toChannel = true)
		{
			throw new NotImplementedException();
		}
	}
}
