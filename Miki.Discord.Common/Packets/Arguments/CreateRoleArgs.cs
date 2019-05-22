using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
    [DataContract]
    public class CreateRoleArgs
	{
		[DataMember(Name ="name")]
		public string Name;

		[DataMember(Name ="permissions")]
		public GuildPermission? Permissions;

		[DataMember(Name ="color")]
		public int? Color;

		[DataMember(Name ="hoist")]
		public bool? Hoisted;

		[DataMember(Name ="mentionable")]
		public bool? Mentionable;
	}
}