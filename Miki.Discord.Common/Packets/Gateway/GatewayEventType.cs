using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest
{
	public enum GatewayEventType
	{
		ChannelCreate = 0,
		ChannelDelete = 1,
		ChannelUpdate = 2,
		GuildBanAdd = 3,
		GuildBanRemove = 4,
		GuildCreate = 5,
		GuildDelete = 6,
		GuildEmojiUpdate = 7,
		GuildIntegrationsUpdate = 22,
		GuildMemberAdd = 8,
		GuildMemberRemove = 9,
		GuildMemberUpdate = 10,
		GuildMembersChunk = 23,
		GuildRoleCreate = 11,
		GuildRoleDelete = 12,
		GuildRoleUpdate = 13,
		GuildUpdate = 14,
		MessageCreate = 15,
		MessageDelete = 24,
		MessageDeleteBulk = 25,
		MessageUpdate = 26,
		PresenceUpdate = 16,
		Ready = 17,
		Resumed = 27,
		TypingStart = 18,
		UserUpdate = 19,
		VoiceServerUpdate = 20,
		VoiceStateUpdate = 21,
		None = 99,
	}
}
