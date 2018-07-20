using Miki.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord
{
    public class DiscordClientConfigurations
    {
		public string Token;
		public string RabbitMQUri;
		public string RabbitMQExchangeName;
		public string RabbitMQQueueName;
		public ICachePool Pool;
	}
}
