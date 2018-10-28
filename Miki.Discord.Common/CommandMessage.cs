using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Gateway.Packets;
using Newtonsoft.Json;

namespace Miki.Discord.Common
{
	public class CommandMessage
	{
		[JsonProperty("shard_id")]
		public int ShardId { get; set; }

		[JsonProperty("opcode")]
		public GatewayOpcode Opcode { get; set; }

		[JsonProperty("data")]
		public object Data { get; set; }

		public static CommandMessage FromGatewayMessage<T>(int shardId, GatewayMessage message)
			where T : class
		{
			CommandMessage msg = new CommandMessage();
			msg.ShardId = shardId;
			msg.Opcode = message.OpCode;
			msg.Data = message.Data as T;
			return msg;
		}
	}
}