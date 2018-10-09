using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Internal
{
	internal class DiscordReaction : DiscordEmoji, IDiscordReaction
	{
		DiscordUserPacket _user;

		public DiscordReaction(DiscordEmojiPacket packet, DiscordUserPacket user, DiscordClient client)
			: base(packet, client)
		{
			_user = user;
		}

		public IDiscordEmoji Emoji => this;

		public IDiscordUser User => new DiscordUser(_user, _client);
	}
}
