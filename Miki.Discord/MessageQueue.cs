using System.Threading;

namespace Miki.Discord.Rest
{
	/// <summary>
	/// Message request queue, used to keep track of requests sent to the external server.
	/// </summary>
	internal class MessageQueue
	{
		private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
	}
}