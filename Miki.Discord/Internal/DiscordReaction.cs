using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Internal
{
	internal class DiscordReaction : DiscordEntity, IDiscordReaction
	{
		private readonly DiscordUserPacket _user;

		public DiscordReaction(DiscordEmoji packet, DiscordUserPacket user, DiscordClient client)
			: base(client)
		{
			_user = user;
			Emoji = packet;
		}

		public DiscordEmoji Emoji { get; }

		public IDiscordUser User => new DiscordUser(_user, _client);
	}
}
