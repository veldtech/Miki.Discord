using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Messaging.Sharder
{
	public class ShardPacket
	{
		[JsonProperty("d")]
		public JToken Data;

		[JsonProperty("meta")]
		public ShardMeta Meta;
	}

	public class ShardPacket<T>
	{
		[JsonProperty("d")]
		public T Data;

		[JsonProperty("meta")]
		public ShardMeta Meta;
	}
}
