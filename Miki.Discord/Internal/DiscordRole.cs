using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;

namespace Miki.Discord.Internal
{
	public class DiscordRole : IDiscordRole
	{
		private DiscordRolePacket _packet;
		private readonly IDiscordClient _client;

		public DiscordRole(DiscordRolePacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public Color Color
			=> new Color((uint)_packet.Color);

		public int Position
			=> _packet.Position;

		public ulong Id
			=> _packet.Id;

		public GuildPermission Permissions
			=> (GuildPermission)_packet.Permissions;
	}
}