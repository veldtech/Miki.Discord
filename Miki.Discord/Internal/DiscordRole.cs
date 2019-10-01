using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;

namespace Miki.Discord.Internal
{
    public class DiscordRole : IDiscordRole
    {
        private DiscordRolePacket packet;
        private readonly IDiscordClient client;

        public DiscordRole(DiscordRolePacket packet, IDiscordClient client)
        {
            this.packet = packet;
            this.client = client;
        }

        public string Name
            => packet.Name;

        public Color Color
            => new Color((uint)packet.Color);

        public int Position
            => packet.Position;

        public ulong Id
            => packet.Id;

        public GuildPermission Permissions
            => (GuildPermission)packet.Permissions;

        public bool IsManaged
            => packet.Managed;

        public bool IsHoisted
            => packet.IsHoisted;

        public bool IsMentionable
            => packet.Mentionable;
    }
}