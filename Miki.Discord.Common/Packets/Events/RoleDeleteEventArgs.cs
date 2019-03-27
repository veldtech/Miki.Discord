﻿using System.Runtime.Serialization;

namespace Miki.Discord.Common.Events
{
	public class RoleDeleteEventArgs
	{
		[DataMember(Name ="guild_id")]
		public ulong GuildId;

		[DataMember(Name ="role_id")]
		public ulong RoleId;
	}
}