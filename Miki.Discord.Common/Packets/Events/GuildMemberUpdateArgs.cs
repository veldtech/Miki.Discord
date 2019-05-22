using System.Runtime.Serialization;

namespace Miki.Discord.Common.Events
{
    [DataContract]
    public class ModifyGuildMemberArgs
	{
		[DataMember(Name ="nick")]
		public string Nickname;

		[DataMember(Name ="roles")]
		public ulong[] RoleIds;

		[DataMember(Name ="mute")]
		public bool? Muted;

		[DataMember(Name ="deaf")]
		public bool? Deafened;

		[DataMember(Name ="channel_id")]
		public ulong? MoveToChannelId;
	}
}