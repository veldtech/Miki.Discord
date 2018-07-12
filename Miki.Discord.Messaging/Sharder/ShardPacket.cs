using Miki.Discord.Rest;
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

		[JsonProperty("t")]
		public string Opcode {
			get
			{
				return opcode.ToString();
			}
			set
			{
				if (Enum.TryParse(value.Replace("_", ""), true, out Opcode op))
				{
					opcode = op;
				}
				else
				{
					opcode = Rest.Opcode.None;
				}
			}
		}
			internal Opcode opcode;
		}

		public class ShardPacket<T>
	{
		[JsonProperty("d")]
		public T Data;

		[JsonProperty("t")]
		public Opcode Opcode;
	}
}
