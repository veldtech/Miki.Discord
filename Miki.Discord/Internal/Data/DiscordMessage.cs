using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets.API;

namespace Miki.Discord.Internal.Data
{
    public class DiscordMessage : IDiscordMessage
    {
        protected readonly DiscordMessagePacket packet;
        protected readonly IDiscordClient client;

        public DiscordMessage(DiscordMessagePacket packet, IDiscordClient client)
        {
            this.packet = packet;
            if(this.packet.GuildId != null
                && this.packet.Member != null)
            {
                this.packet.Member.User = this.packet.Author;
                this.packet.Member.GuildId = this.packet.GuildId.Value;
            }
            this.client = client;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IDiscordAttachment> Attachments
            => packet.Attachments
                .Select(x => new DiscordAttachment(x))
                .ToList();

        /// <inheritdoc/>
        public IDiscordUser Author
            => packet.Member == null
                ? new DiscordUser(packet.Author, client)
                : new DiscordGuildUser(packet.Member, client);

        /// <inheritdoc/>
        public string Content
            => packet.Content;

        /// <inheritdoc/>
        public ulong ChannelId
            => packet.ChannelId;


        /// <inheritdoc/>
        public IReadOnlyList<ulong> MentionedUserIds
            => packet.Mentions.Select(x => x.Id)
                .ToList();

        /// <inheritdoc/>
        public DateTimeOffset Timestamp
            => packet.Timestamp;

        /// <inheritdoc/>
        public ulong Id
            => packet.Id;

        /// <inheritdoc/>
        public DiscordMessageType Type
            => packet.Type;

        /// <inheritdoc/>
        public async Task<IDiscordMessage> EditAsync(EditMessageArgs args)
            => await client.EditMessageAsync(ChannelId, Id, args.Content, args.Embed);

        /// <inheritdoc/>
        public async Task DeleteAsync()
            => await client.ApiClient.DeleteMessageAsync(packet.ChannelId, packet.Id);

        /// <inheritdoc/>
        public async Task<IDiscordTextChannel> GetChannelAsync()
        {
            var channel = await client.GetChannelAsync(packet.ChannelId, packet.GuildId);
            return channel as IDiscordTextChannel;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji)
            => await client.GetReactionsAsync(packet.ChannelId, Id, emoji);

        /// <inheritdoc/>
        public async Task CreateReactionAsync(DiscordEmoji emoji)
            => await client.ApiClient.CreateReactionAsync(ChannelId, Id, emoji);

        /// <inheritdoc/>
        public async Task DeleteReactionAsync(DiscordEmoji emoji)
            => await client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji);

        /// <inheritdoc/>
        public async Task DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user)
            => await DeleteReactionAsync(emoji, user.Id);

        /// <inheritdoc/>
        public async Task DeleteReactionAsync(DiscordEmoji emoji, ulong userId)
            => await client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji, userId);

        /// <inheritdoc/>
        public async Task DeleteAllReactionsAsync()
            => await client.ApiClient.DeleteReactionsAsync(ChannelId, Id);
    }
}