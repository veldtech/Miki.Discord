using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Miki.Discord.Common.Packets.Arguments;

namespace Miki.Discord.Mocking
{
    /// <summary>
    /// This API client will always throw an <see cref="InvalidOperationException"/>. This is used to make sure API calls are not being called whenever they are not necessary.
    /// </summary>
    public class InvalidDummyApiClient : IApiClient
    {
        public Task AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
        {
            throw new InvalidOperationException();
        }

        public Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordEmoji> CreateEmojiAsync(ulong guildId, EmojiCreationArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteChannelAsync(ulong channelId)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteGuildAsync(ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteMessageAsync(ulong channelId, ulong messageId)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteMessagesAsync(ulong channelId, ulong[] messages)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
        {
            throw new InvalidOperationException();
        }

        public Task DeleteReactionsAsync(ulong channelId, ulong messageId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
        {
            throw new InvalidOperationException();
        }

        public Task<IEnumerable<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordUserPacket> GetCurrentUserAsync()
        {
            throw new InvalidOperationException();
        }

        public Task<IEnumerable<DiscordChannelPacket>> GetDMChannelsAsync()
        {
            throw new InvalidOperationException();
        }

        public Task<GatewayConnectionPacket> GetGatewayAsync()
        {
            throw new InvalidOperationException();
        }

        public Task<GatewayConnectionPacket> GetGatewayBotAsync()
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId)
        {
            throw new InvalidOperationException();
        }

        public Task<IEnumerable<DiscordMessagePacket>> GetMessagesAsync(ulong channelId, int amount = 100)
        {
            throw new InvalidOperationException();
        }

        public Task<int> GetPruneCountAsync(ulong guildId, int days)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emojiId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordRolePacket> GetRoleAsync(ulong roleId, ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task<IEnumerable<DiscordRolePacket>> GetRolesAsync(ulong guildId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordUserPacket> GetUserAsync(ulong userId)
        {
            throw new InvalidOperationException();
        }

        public Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
        {
            throw new InvalidOperationException();
        }

        public Task ModifySelfAsync(UserModifyArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task<int?> PruneGuildMembersAsync(ulong guildId, int days, bool computePrunedCount)
        {
            throw new InvalidOperationException();
        }

        public Task RemoveGuildBanAsync(ulong guildId, ulong userId)
        {
            throw new InvalidOperationException();
        }

        public Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
        {
            throw new InvalidOperationException();
        }

        public Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args)
        {
            throw new InvalidOperationException();
        }

        public Task TriggerTypingAsync(ulong channelId)
        {
            throw new InvalidOperationException();
        }
    }
    public class DefaultDummyApiClient : IApiClient
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

		public Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			return Task.FromResult(new DiscordRolePacket
			{
				Color = args.Color ?? 0,
				Id = 0,
				IsHoisted = args.Hoisted ?? false,
				Mentionable = args.Mentionable ?? false,
				Permissions = (int?)args.Permissions ?? 0,
				Name = args.Name,
			});
		}

		public Task CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
            return Task.CompletedTask;
        }

		public Task DeleteChannelAsync(ulong channelId)
		{
            return Task.CompletedTask;
        }

        public Task DeleteGuildAsync(ulong guildId)
		{
            return Task.CompletedTask;
        }

        public Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
            return Task.CompletedTask;
        }

        public Task DeleteMessagesAsync(ulong channelId, ulong[] messages)
		{
            return Task.CompletedTask;
        }

        public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
            return Task.CompletedTask;
        }

        public Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
		{
            return Task.CompletedTask;
        }

        public Task DeleteReactionsAsync(ulong channelId, ulong messageId)
		{
            return Task.CompletedTask;
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

		public Task<IEnumerable<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
		{
			throw new NotImplementedException();
		}

        public Task<IEnumerable<DiscordChannelPacket>> GetDMChannelsAsync()
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

		public Task<IEnumerable<DiscordMessagePacket>> GetMessagesAsync(ulong channelId, int amount = 100)
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

		public Task<IEnumerable<DiscordRolePacket>> GetRolesAsync(ulong guildId)
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

        public Task ModifySelfAsync(UserModifyArgs args)
        {
            return Task.CompletedTask;
        }

        public Task RemoveGuildBanAsync(ulong guildId, ulong userId)
		{
            return Task.CompletedTask;
        }

        public Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
		{
            return Task.CompletedTask;
        }

        public Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }

        public Task<int> GetPruneCountAsync(ulong guildId, int days)
        {
            return Task.FromResult(days);
        }

        public Task<int?> PruneGuildMembersAsync(ulong guildId, int days, bool computePrunedCount)
        {
            if(computePrunedCount)
            {
                return Task.FromResult<int?>(days);
            }
            return Task.FromResult<int?>(null);
        }
    }
}
