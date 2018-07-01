using Miki.Discord.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Messaging.Sharder
{
	public class ShardMeta
	{
		[JsonProperty("op")]
		public Opcode Opcode = 0;

		[JsonProperty("shard_id")]
		public uint ShardId = 0;
	}
}
