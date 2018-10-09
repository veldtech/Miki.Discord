using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest.Arguments
{
    public class ChannelBulkDeleteArgs
    {
		[JsonProperty("messages")]
		public ulong[] Messages { get; set; }

		public ChannelBulkDeleteArgs(ulong[] messages)
		{
			Messages = messages;
		}
    }
}
