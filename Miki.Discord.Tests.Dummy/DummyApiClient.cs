using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;
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

		public Task<DiscordEmoji> CreateEmojiAsync(ulong guildId, EmojiCreationArgs args)
		{
			return Task.FromResult(new DiscordEmoji());	
		}

		public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			await Task.Yield();
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

		public Task CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAllReactions(ulong channelId, ulong messageId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteChannelAsync(ulong channelId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteGuildAsync(ulong guildId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteMessagesAsync(ulong channelId, ulong[] messages)
		{
			throw new NotImplementedException();
		}

		public Task DeleteReaction(ulong channelId, ulong messageId, ulong userId, DiscordEmoji emoji)
		{
			throw new NotImplementedException();
		}

		public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
			throw new NotImplementedException();
		}

		public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
		{
			throw new NotImplementedException();
		}

		public Task DeleteReactionsAsync(ulong channelId, ulong messageId)
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

		public Task<GatewayConnectionPacket> GetGatewayAsync()
		{
			throw new NotImplementedException();
		}

		public Task<GatewayConnectionPacket> GetGatewayBotAsync()
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

		public Task<List<DiscordMessagePacket>> GetMessagesAsync(ulong channelId, int amount = 100)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, ulong emojiId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emojiId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordRolePacket> GetRoleAsync(ulong roleId, ulong guildId)
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

		public Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
		{
			throw new NotImplementedException();
		}

		public Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			throw new NotImplementedException();
		}

		public Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args)
		{
			throw new NotImplementedException();
		}

        public Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args)
		{
			throw new NotImplementedException();
		}

		public Task TriggerTypingAsync(ulong channelId)
		{
			throw new NotImplementedException();
		}
	}
}
