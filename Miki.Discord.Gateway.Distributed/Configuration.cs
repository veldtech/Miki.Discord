using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Gateway.Distributed
{
	public class MessageClientConfiguration
	{
		/// <summary>
		/// The connection uri to connect to the message queue.
		/// </summary>
		public Uri ConnectionString { get; set; }

		/// <summary>
		/// Checks if the consumer should automatically acknowledge packages. This means that you will disable persistence in favor of performance.
		/// </summary>
		public bool ConsumerAutoAck { get; set; } = true;

		/// <summary>
		/// The name of the exchange the messages get sent to.
		/// </summary>
		public string ExchangeName { get; set; }

		/// <summary>
		/// Routing key of the Exchange. This can be used to route specific messages 
		/// </summary>
		public string ExchangeRoutingKey { get; set; } = "*";

		/// <summary>
		/// Automatically deletes the queue if no consumers or  are active.
		/// </summary>
		public bool QueueAutoDelete { get; set; } = false;

		/// <summary>
		/// 
		/// </summary>
		public bool QueueDurable { get; set; } = true;
		public bool QueueExclusive { get; set; } = false;
		public string QueueName { get; set; }
	}
}
