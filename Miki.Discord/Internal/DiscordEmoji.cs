using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Internal
{
	internal class DiscordEmoji : DiscordEntity, IDiscordEmoji
	{
		DiscordEmojiPacket _packet;

		public DiscordEmoji(DiscordEmojiPacket packet, DiscordClient client)
			: base(client)
		{
			_packet = packet;
		}

		public string Name => _packet.Name;

		public IEnumerable<ulong> Roles => _packet.WhitelistedRoles;

		public IDiscordUser Creator => new DiscordUser(_packet.Creator, _client);

		public bool? RequireColons => _packet.RequireColons;

		public bool? IsManaged => _packet.Managed;

		public bool? IsAnimated => _packet.Animated;

		public ulong Id => _packet.Id;
	}
}
