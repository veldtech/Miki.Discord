using Miki.Discord.Common.Packets;
using Miki.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface IDiscordPresence
    {
		Activity Activity { get; }
		UserStatus Status { get; }
    }
}
