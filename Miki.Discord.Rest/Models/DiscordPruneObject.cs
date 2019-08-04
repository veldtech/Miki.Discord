using System.Runtime.Serialization;

namespace Miki.Discord.Rest
{
	[DataContract]
	public class DiscordPruneObject
	{
		[DataMember(Name = "pruned")]
		public int Pruned { get; set; }
	}
}
