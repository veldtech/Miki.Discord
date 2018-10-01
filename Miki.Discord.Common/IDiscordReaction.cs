using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface IDiscordReaction
    {
		/// <summary>
		/// times this emoji has been used to react.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// The emoji information.
		/// </summary>
		IDiscordEmoji Emoji { get; }

		/// <summary>
		/// Whether the current user reacted using this emoji.
		/// </summary>
		bool ReactedToThis { get; }
    }
}
