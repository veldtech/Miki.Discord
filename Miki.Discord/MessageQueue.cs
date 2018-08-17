using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Miki.Discord.Rest
{
	/// <summary>
	/// Message request queue, used to keep track of requests sent to the external server.
	/// </summary>
	internal class MessageQueue
	{
		readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
	}
}
