using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Internal;
using Miki.Discord.Rest;
using Miki.Discord.Rest.Entities;
using Miki.Rest;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Rest
{
    public class DiscordApiClient
    {
		RestClient rest;
		ICacheClient cache;

		const string discordUrl = "https://discordapp.com";
		const string baseUrl = "/api/v6";
		const string cdnUrl = "https://cdn.discordapp.com/";

		JsonSerializerSettings serializer;

		public DiscordApiClient(string token, ICacheClient cacheClient)
		{
			rest = new RestClient(discordUrl + baseUrl)
				.SetAuthorization("Bot", token);
			cache = cacheClient;

			serializer = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		public async Task AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
		{
			QueryString qs = new QueryString();

			if (!string.IsNullOrWhiteSpace(reason))
			{
				qs.Add("reason", reason);
			}

			if (pruneDays != 0)
			{
				qs.Add("delete-message-days", pruneDays);
			}

			await rest.PutAsync($"/guilds/{guildId}/bans/{userId}" + qs.Query);
		}

		public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			await rest.PutAsync($"/guilds/{guildId}/members/{userId}/roles/{roleId}");
		}

		public async Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
		{
			var response = await rest
				.PostAsync<DiscordChannelPacket>($"/users/@me/channels", $"{{ \"recipient_id\": {userId} }}");
			return response.Data;
		}

		public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			var  response = await rest.PostAsync<DiscordRolePacket>(
					$"/guilds/{guildId}/roles", 
					JsonConvert.SerializeObject(args) ?? ""
			);

			return response.Data;
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			await rest.DeleteAsync($"/channels/{channelId}/messages/{messageId}");
		}

		public async Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args, bool toChannel = true)
		{
			string key = $"discord:ratelimit:patch:channel:{channelId}:messages:{messageId}";
			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			string route = toChannel ? "channels" : "users";

			if (!IsRatelimited(rateLimit))
			{
				RestResponse<DiscordMessagePacket> rc = await rest
					.PatchAsync<DiscordMessagePacket>($"/{route}/{channelId}/messages/{messageId}", JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
				await HandleRateLimit(rc, rateLimit, key);
				return rc.Data;
			}
			return null;
		}

		public async Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
		{
			string key = $"discord:ratelimit:put:guild:{guildId}:role:{role.Id}";

			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);

			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			return (await rest.PutAsync<DiscordRolePacket>(
				$"/guilds/{guildId}/roles/{role.Id}", 
				JsonConvert.SerializeObject(role)
			)).Data;
		}

		public async Task<DiscordUserPacket> GetCurrentUserAsync()
		{
			string key = $"discord:user:self";

			if (await cache.ExistsAsync(key))
			{
				return await cache.GetAsync<DiscordUserPacket>(key);
			}

			RestResponse<DiscordUserPacket> rc = await rest
				.GetAsync<DiscordUserPacket>($"/users/@me");
			await cache.AddAsync(key, rc.Data);
			return rc.Data;
		}

		public async Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
		{
			string key = $"discord:channel:{channelId}";
			DiscordChannelPacket packet = null;

			if (await cache.ExistsAsync(key))
			{
				packet = await cache.GetAsync<DiscordChannelPacket>(key);
			}
			else
			{
				var response = await rest.GetAsync<DiscordChannelPacket>($"/channels/{channelId}");
				response.HttpResponseMessage.EnsureSuccessStatusCode();

				await cache.AddAsync(key, response.Data);

				packet = response.Data;
			}

			return packet;
		}

		public async Task<List<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
		{
			string key = $"discord:guilds:{guildId}:channels";
			List<DiscordChannelPacket> packet = null;

			if (await cache.ExistsAsync(key))
			{
				packet = await cache.GetAsync<List<DiscordChannelPacket>>(key);
			}
			else
			{
				var response = await rest.GetAsync<List<DiscordChannelPacket>>($"/guild/{guildId}/channels");
				response.HttpResponseMessage.EnsureSuccessStatusCode();

				await cache.AddAsync(key, response.Data);

				packet = response.Data;
			}
			return packet;
		}

		public async Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
		{
			string key = $"discord:guild:{guildId}";

			if (await cache.ExistsAsync(key))
			{
				return await cache.GetAsync<DiscordGuildPacket>(key);
			}
			else
			{
				RestResponse<DiscordGuildPacket> rc = await rest
					.GetAsync<DiscordGuildPacket>($"/guilds/{guildId}");

				await cache.AddAsync(key, rc.Data);

				return rc.Data;
			}
		}

		public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
		{
			string key = $"discord:guild:{guildId}:user:{userId}";

			DiscordGuildMemberPacket packet;

			if (await cache.ExistsAsync(key))
			{
				packet = await cache.GetAsync<DiscordGuildMemberPacket>(key);
			}
			else
			{
				RestResponse<DiscordGuildMemberPacket> rc = await rest
					.GetAsync<DiscordGuildMemberPacket>($"/guilds/{guildId}/members/{userId}");
				packet = rc.Data;
				await cache.AddAsync(key, rc.Data);
			}

			packet.User = await GetUserAsync(userId);
			packet.GuildId = guildId;
			packet.UserId = userId;

			return packet;
		}

		public async Task<List<DiscordRolePacket>> GetRolesAsync(ulong guildId)
		{
			DiscordGuildPacket guild = await GetGuildAsync(guildId);

			if (guild != null)
			{
				return guild.Roles;
			}
			return null;
		}

		public async Task<DiscordUserPacket> GetUserAsync(ulong userId)
		{
			string key = $"discord:user:{userId}";

			if (await cache.ExistsAsync(key))
			{
				return await cache.GetAsync<DiscordUserPacket>(key);
			}
			else
			{
				RestResponse<DiscordUserPacket> rc = await rest
					.GetAsync<DiscordUserPacket>($"/users/{userId}");
				await cache.AddAsync(key, rc.Data);
				return rc.Data;
			}
		}

		public string GetUserAvatarUrl(ulong id, string hash)
		{
			return $"{cdnUrl}avatars/{id}/{hash}.png";
		}

		private async Task HandleRateLimit(RestResponse rc, Ratelimit ratelimit, string key)
		{
			if (!IsRatelimited(ratelimit))
			{
				if (rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Limit"))
				{
					ratelimit = new Ratelimit();
					ratelimit.Remaining = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Remaining").ToList().FirstOrDefault());
					ratelimit.Limit = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Limit").ToList().FirstOrDefault());
					ratelimit.Reset = long.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Reset").ToList().FirstOrDefault());
					if (rc.HttpResponseMessage.Headers.Contains("X-RateLimit-Global"))
					{
						ratelimit.Global = int.Parse(rc.HttpResponseMessage.Headers.GetValues("X-RateLimit-Global").ToList().FirstOrDefault());
					}
					await cache.AddAsync(key, ratelimit);
				}
			}
		}

		private bool IsRatelimited(Ratelimit rl)
		{
			return (rl?.Remaining ?? 1) <= 0 && DateTime.UtcNow <= DateTimeOffset.FromUnixTimeSeconds(rl?.Reset ?? 0);
		}

		public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
		{
			await rest.PatchAsync($"/guilds/{guildId}/members/{userId}",
				JsonConvert.SerializeObject(packet, serializer)
			);
		}

		public async Task RemoveGuildBanAsync(ulong guildId, ulong userId)
			=> await rest.DeleteAsync($"/guilds/{guildId}/bans/{userId}");

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong userId)
		{
			string key = $"discord:ratelimit:delete:guild:{guildId}:members:{userId}";

			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);

			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			await rest.DeleteAsync($"/guilds/{guildId}/members/{userId}");
		}

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			await rest.DeleteAsync($"/guilds/{guildId}/members/{userId}/roles/{roleId}");
		}


		public async Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args, bool toChannel = true)
		{
			string key = $"discord:ratelimit:post:channel:{channelId}:messages";

			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);

			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			args.embed = new DiscordEmbed();
			args.embed.Image = new EmbedImage()
			{
				Url = "attachment://" + fileName
			};

			string json = JsonConvert.SerializeObject(args, serializer);
			string route = toChannel ? "channels" : "users";

			if (!IsRatelimited(rateLimit))
			{
				List<MultiformItem> items = new List<MultiformItem>();

				var content = new StringContent(args.content);
				items.Add(new MultiformItem { Name = "content", Content = content });

				if (stream.CanSeek)
				{
					var memoryStream = new MemoryStream();
					await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
					memoryStream.Position = 0;
					stream = memoryStream;
				}

				var image = new StreamContent(stream);
				items.Add(new MultiformItem { Name = "file", Content = image, FileName = fileName });
				image.Headers.Add("Content-Type", "image/png");

				image.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + fileName + "\"");
				RestResponse rc = await rest
					.PostMultipartAsync($"/{route}/{channelId}/messages",
					items.ToArray()
				);
				await HandleRateLimit(rc, rateLimit, key);
				return null;
			}
			return null;
		}

		public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args, bool toChannel = true)
		{
			string key = $"discord:ratelimit:post:channel:{channelId}:messages";
			Ratelimit rateLimit = await cache.GetAsync<Ratelimit>(key);
			if (rateLimit != null)
			{
				rateLimit.Remaining--;
				await cache.AddAsync(key, rateLimit);
			}

			string json = JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			string route = toChannel ? "channels" : "users";

			if (!IsRatelimited(rateLimit))
			{
				RestResponse<DiscordMessagePacket> rc = await rest
					.PostAsync<DiscordMessagePacket>($"/{route}/{channelId}/messages", json);
				await HandleRateLimit(rc, rateLimit, key);
				return rc.Data;
			}
			return null;
		}
	}
}
