using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface IDiscordReaction
    {
		/// <summary>
		/// The emoji information.
		/// </summary>
		DiscordEmoji Emoji { get; }

		/// <summary>
		/// The user information.
		/// </summary>
		IDiscordUser User { get; }
    }
}
