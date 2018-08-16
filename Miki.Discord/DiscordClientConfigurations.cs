using Miki.Cache;
using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord
{
    public class DiscordClientConfigurations
    {
		public string Token;
		public IGateway Gateway;
		public ICachePool Pool;
	}
}
