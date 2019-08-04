using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[DataContract]
	public class DiscordAttachmentPacket
	{
		[DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

		[DataMember(Name = "filename", Order = 2)]
		public string FileName { get; set; }

		[DataMember(Name = "size", Order = 3)]
		public int Size { get; set; }

		[DataMember(Name = "url", Order = 4)]
		public string Url { get; set; }

		[DataMember(Name = "proxy_url", Order = 5)]
		public string ProxyUrl { get; set; }

		[DataMember(Name = "height", Order = 6)]
		public int? Height { get; set; }

		[DataMember(Name = "width", Order = 7)]
		public int? Width { get; set; }

	}
}
