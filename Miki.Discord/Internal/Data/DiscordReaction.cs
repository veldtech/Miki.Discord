using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Discord.Internal.Data
{
    internal class DiscordReaction : IDiscordReaction
    {
        private readonly DiscordReactionPacket packet;
        private readonly IDiscordClient client;

        public ulong ChannelId => packet.ChannelId;
        
        public DiscordEmoji Emoji => packet.Emoji;

        public ulong MessageId => packet.MessageId;

        public DiscordReaction(DiscordReactionPacket packet, IDiscordClient client)
        {
            this.packet = packet;
            this.client = client;
        }

        /// <inheritdoc/>
        public async ValueTask<IDiscordTextChannel> GetChannelAsync()
        {
            return await client.GetChannelAsync(packet.ChannelId, packet.GuildId)
                         as IDiscordTextChannel;
        }

        /// <inheritdoc />
        public async ValueTask<IDiscordUser> GetUserAsync()
        {
            return await client.GetUserAsync(packet.UserId);
        }
    }
}