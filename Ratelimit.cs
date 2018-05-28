using ProtoBuf;

namespace Miki.Discord.Rest
{
	[ProtoContract]
	public class Ratelimit
	{
		[ProtoMember(1)]
		public int Remaining { get; set; }

		[ProtoMember(2)]
		public int Limit { get; set; }

		[ProtoMember(3)]
		public long Reset { get; set; }

		[ProtoMember(4)]
		public int? Global { get; set; }
	}
}