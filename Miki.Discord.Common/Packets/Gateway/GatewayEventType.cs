using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Rest
{
	public enum GatewayEventType
	{
		ChannelCreate,
		ChannelDelete,
		ChannelUpdate,
		ChannelPinsUpdate,
		GuildBanAdd,
		GuildBanRemove,
		GuildCreate,
		GuildDelete,
		GuildEmojisUpdate,
		GuildIntegrationsUpdate,
		GuildMemberAdd,
		GuildMemberRemove,
		GuildMemberUpdate,
		GuildMembersChunk,
		GuildRoleCreate,
		GuildRoleDelete,
		GuildRoleUpdate,
		GuildUpdate,
		MessageCreate,
		MessageDelete,
		MessageDeleteBulk,
		MessageUpdate,
		MessageReactionAdd,
		MessageReactionRemove,
		MessageReactionRemoveAll,
		PresenceUpdate,
		Ready,
		Resumed,
		TypingStart,
		UserUpdate,
		VoiceServerUpdate,
		VoiceStateUpdate,
		WebhooksUpdate,
		Undefined,
	}
}
