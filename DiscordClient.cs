using Miki.Discord.Rest.Entities;
using Miki.Rest;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Rest
{
    public class DiscordClient
    {
		ICacheClient redis;
		RestClient rest;

		string token;

		const string discordUrl = "https://discordapp.com";
		const string baseUrl = "/api/v6";

		public DiscordClient(string token, ICacheClient redis)
		{
			this.redis = redis;
			this.token = token;
			rest = new RestClient(discordUrl + baseUrl)
				.SetAuthorization("Bot", token);
		}

		public async Task<DiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string text, EmbedBuilder embed = null)
		{
			string key = $"discord:ratelimit:patch:channel:{channelId}:messages:{messageId}";
			Ratelimit rateLimit = await redis.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await redis.AddAsync(key, rateLimit);
			}

			EditMessageArgs ma = new EditMessageArgs();
			ma.content = text;
			ma.embed = embed?.ToEmbed() ?? null;

			return await InternalEditMessageAsync(rateLimit, key, channelId, messageId, ma);
		}

		public string GetAvatarUrl(ulong id, string hash)
		{
			return $"/avatars/{id}/{hash}";
		}

		public async Task<DiscordChannel> GetChannelAsync(ulong id)
		{
			string key = $"discord:channel:{id}";

			if (await redis.ExistsAsync(key))
			{
				return await redis.GetAsync<DiscordChannel>(key);
			}
			else
			{
				RestResponse<DiscordChannel> rc = await rest
					.GetAsync<DiscordChannel>($"/channels/{id}");
				await redis.AddAsync(key, rc.Data);
				return rc.Data;
			}
		}
		public async Task<DiscordUser> GetCurrentUserAsync()
		{
			string key = $"discord:user:self";

			if (await redis.ExistsAsync(key))
			{
				return await redis.GetAsync<DiscordUser>(key);
			}

			RestResponse<DiscordUser> rc = await rest
				.GetAsync<DiscordUser>($"/users/@me");
			await redis.AddAsync(key, rc.Data);
			return rc.Data;
		}

		public async Task<DiscordChannel> CreateDMAsync(ulong userid)
		{
			RestResponse<DiscordChannel> rc = await rest
				.PostAsync<DiscordChannel>($"/users/@me/channels", $"{{ \"recipient_id\": {userid} }}");
			return rc.Data;
		}

		public async Task<DiscordGuild> GetGuildAsync(ulong id)
		{
			string key = $"discord:guild:{id}";

			if (await redis.ExistsAsync(key))
			{
				return await redis.GetAsync<DiscordGuild>(key);
			}
			else
			{
				RestResponse<DiscordGuild> rc = await rest
					.GetAsync<DiscordGuild>($"/guilds/{id}");
				await redis.AddAsync(key, rc.Data);
				return rc.Data;
			}
		}
		public async Task<DiscordUser> GetUserAsync(ulong id)
		{
			string key = $"discord:user:{id}";

			if (await redis.ExistsAsync(key))
			{
				return await redis.GetAsync<DiscordUser>(key);
			}
			else
			{
				RestResponse<DiscordUser> rc = await rest
					.GetAsync<DiscordUser>($"/users/{id}");
				await redis.AddAsync(key, rc.Data);
				return rc.Data;
			}
		}

		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
		{
			string key = $"discord:ratelimit:post:channel:{channelId}:messages";
			Ratelimit rateLimit = await redis.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await redis.AddAsync(key, rateLimit);
			}
			return await InternalSendMessageAsync(rateLimit, key, channelId, message);
		}
		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, string text, EmbedBuilder embed = null)
		{
			MessageArgs margs = new MessageArgs();
			margs.content = text;
			margs.embed = embed?.Build() ?? null;
			return await SendMessageAsync(channelId, margs);
		}

		private async Task<DiscordMessage> InternalSendMessageAsync(Ratelimit ratelimit, string key, ulong channelId, MessageArgs args, bool toChannel = true)
		{
			string json = JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			string route = toChannel ? "channels" : "users";

			if (!IsRatelimited(ratelimit))
			{
				RestResponse<DiscordMessage> rc = await rest
					.PostAsync<DiscordMessage>($"/{route}/{channelId}/messages", json);
				await HandleRateLimit(rc, ratelimit, key);
				return rc.Data;
			}
			return null;
		}
		private async Task<DiscordMessage> InternalEditMessageAsync(Ratelimit ratelimit, string key, ulong channelId, ulong messageId, EditMessageArgs args)
		{
			if (!IsRatelimited(ratelimit))
			{
				RestResponse<DiscordMessage> rc = await rest
					.PatchAsync<DiscordMessage>($"/channels/{channelId}/messages/{messageId}", JsonConvert.SerializeObject(args, new JsonSerializerSettings(){ NullValueHandling = NullValueHandling.Ignore }));
				await HandleRateLimit(rc, ratelimit, key);
				return rc.Data;
			}
			return null;
		}

		private async Task HandleRateLimit<T>(RestResponse<T> rc, Ratelimit ratelimit, string key)
		{
			if (!IsRatelimited(ratelimit))
			{
				if (rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Limit"))
				{
					ratelimit = new Ratelimit();
					ratelimit.Remaining = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Remaining").ToList().FirstOrDefault());
					ratelimit.Limit = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Limit").ToList().FirstOrDefault());
					ratelimit.Reset = long.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Reset").ToList().FirstOrDefault());
					if(rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Global"))
					{
						ratelimit.Global = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Global").ToList().FirstOrDefault());
					}
					await redis.AddAsync(key, ratelimit);
				}
			}
		}

		private bool IsRatelimited(Ratelimit rl)
		{
			return (rl?.Remaining ?? 1) <= 0 && DateTime.UtcNow <= DateTimeOffset.FromUnixTimeSeconds(rl?.Reset ?? 0);
		}
	}

	public class MessageArgs : EditMessageArgs
	{
		public bool tts = false;
	}

	public class EditMessageArgs
	{
		public string content;
		public DiscordEmbed embed;
	}
}
