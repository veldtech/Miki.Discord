﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordUser : ISnowflake
    {
		string AvatarId { get; }

		string Mention { get; }

		string Username { get; }

		string Discriminator { get; }

		DateTimeOffset CreatedAt { get; }

		bool IsBot { get; }

		Task<IDiscordPresence> GetPresenceAsync();

		Task<IDiscordTextChannel> GetDMChannelAsync();

		string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256);
    }
}
