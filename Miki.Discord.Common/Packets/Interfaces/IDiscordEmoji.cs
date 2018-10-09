using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface IDiscordEmoji : ISnowflake
    {
		string Name { get; }

		IEnumerable<ulong> Roles { get; }

		IDiscordUser Creator { get; }

		bool? RequireColons { get; }

		bool? IsManaged { get; }

		bool? IsAnimated { get; }
    }
}
