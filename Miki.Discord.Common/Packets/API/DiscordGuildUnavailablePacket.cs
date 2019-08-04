using System.Runtime.Serialization;

namespace Miki.Discord.Common.Packets
{
	[DataContract]
	public class DiscordGuildUnavailablePacket
	{
		[DataMember(Name = "id")]
		public ulong GuildId;

		[DataMember(Name = "unavailable")]
		public bool? IsUnavailable;

		/// <summary>
		/// A converter method to avoid protocol buffer serialization complexion
		/// </summary>
		/// <returns>A converted DiscordGuildPacket</returns>
		public DiscordGuildPacket ToGuildPacket()
		{
			return new DiscordGuildPacket
			{
				Id = GuildId,
				Unavailable = IsUnavailable
			};
		}
	}
}