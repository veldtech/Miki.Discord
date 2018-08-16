using Miki.Discord.Common.Packets;
using Miki.Discord.Rest.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common.Events
{
    public class RoleEventArgs
    {
		[JsonProperty("guild_id")]
		public ulong GuildId;

		[JsonProperty("role")]
		public DiscordRolePacket Role;
	}
}
