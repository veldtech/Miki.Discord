using Miki.Discord.Common;

namespace Miki.Discord.Internal.Data
{
    internal class DiscordPresence : IDiscordPresence
    {
        /// <inheritdoc />
        public DiscordActivity Activity => packet.Game;

        /// <inheritdoc />
        public UserStatus Status => packet.Status;

        private readonly DiscordPresencePacket packet;
        private readonly IDiscordClient client;
        
        public DiscordPresence(DiscordPresencePacket packet, IDiscordClient client)
        {
            this.packet = packet;
            this.client = client;
        }
    }
}