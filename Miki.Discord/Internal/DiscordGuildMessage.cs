namespace Miki.Discord.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Discord.Common.Models;
    using Miki.Discord.Common.Packets.API;

    internal class DiscordGuildMessage : DiscordMessage, IDiscordGuildMessage
    {
        /// <inheritdoc />
        public DiscordGuildMessage(DiscordMessagePacket packet, IDiscordClient client)
            : base(packet, client)
        {
        }

        /// <inheritdoc />
        public ulong GuildId 
            => packet.GuildId ?? throw new InvalidOperationException("Guild Message does not have guild ID set.");

        /// <inheritdoc />
        public Task<IDiscordGuild> GetGuildAsync()
        {
            return client.GetGuildAsync(GuildId);
        }
    }
}
