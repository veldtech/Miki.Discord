using Miki.Discord.Common;

namespace Miki.Discord.Internal.Data
{
    public class DiscordRole : IDiscordRole
    {
        protected readonly DiscordRolePacket packet;
        protected readonly IDiscordClient client;

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