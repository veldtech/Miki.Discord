﻿namespace Miki.Discord.Common.Events
{
    using Miki.Discord.Common.Packets;
    using System.Runtime.Serialization;

    [DataContract]
    public class RoleEventArgs
    {
        [DataMember(Name = "guild_id")] public ulong GuildId;

        [DataMember(Name = "role")] public DiscordRolePacket Role;
    }
}