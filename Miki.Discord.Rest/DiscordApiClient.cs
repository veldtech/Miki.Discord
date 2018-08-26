using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest;
using Miki.Logging;
using Miki.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Rest
{
    public class DiscordApiClient : IApiClient
    {
		private RestClient _restClient;

		ICachePool cache;

		readonly JsonSerializerSettings serializer;

		public DiscordApiClient(string token, ICachePool cachePool)
		{
			_restClient = new RestClient(DiscordHelper.DiscordUrl + DiscordHelper.BaseUrl)
				.SetAuthorization("Bot", token);
			cache = cachePool;

			serializer = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
			};
		}

		public async Task AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null)
		{
			var cacheClient = cache.Get;
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

				await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () =>
					{
						return await _restClient.PutAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId) + qs.Query);
					});
			}
		}

		public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			var cacheClient = cache.Get;
			{
				await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", cacheClient,
				async () =>
				{
					return await _restClient.PutAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
				});
			}
		}

		public async Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
		{
			var response = await _restClient
				.PostAsync<DiscordChannelPacket>(DiscordApiRoutes.UserMeChannelsRoute(), $"{{ \"recipient_id\": {userId} }}");
			return response.Data;
		}

		public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			var cacheClient = cache.Get;
			{
				return (await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () =>
					{
						return await _restClient.PostAsync<DiscordRolePacket>(
							DiscordApiRoutes.GuildRolesRoute(guildId),
							JsonConvert.SerializeObject(args) ?? ""
						);
					})).Data;
			}
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			var cacheClient = cache.Get;
			{
				await RatelimitHelper.ProcessRateLimitedAsync(
					$"channels:{channelId}:delete", cacheClient,
					async () =>
					{
						return await _restClient.DeleteAsync(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
					});
			}
		}

		public async Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args)
		{
			var cacheClient = cache.Get;
			{
				return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", cacheClient,
				async () =>
				{
					return await _restClient.PatchAsync<DiscordMessagePacket>(
						DiscordApiRoutes.ChannelMessageRoute(channelId, messageId), 
						JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore })
					);
				})).Data;
			}
		}

		public async Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
		{
			var cacheClient = cache.Get;
			{
				return (await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () => await _restClient.PutAsync<DiscordRolePacket>(
						DiscordApiRoutes.GuildRoleRoute(guildId, role.Id),
						JsonConvert.SerializeObject(role)
					)
				)).Data;
			}
		}

		public async Task<DiscordUserPacket> GetCurrentUserAsync()
		{
			string key = $"discord:user:self";

			var cacheClient = cache.Get;
			{
				if (await cacheClient.ExistsAsync(key))
				{
					return await cacheClient.GetAsync<DiscordUserPacket>(key);
				}

				RestResponse<DiscordUserPacket> rc = await _restClient
					.GetAsync<DiscordUserPacket>(DiscordApiRoutes.UserMeRoute());
				await cacheClient.UpsertAsync(key, rc.Data);
				return rc.Data;
			}
		}

		public async Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
		{
			string key = $"discord:channel:{channelId}";
			DiscordChannelPacket packet = null;

			var cacheClient = cache.Get;
			if (await cacheClient.ExistsAsync(key))
				{
					packet = await cacheClient.GetAsync<DiscordChannelPacket>(key);

					if (packet == null)
					{
						Log.Debug($"cache hit on '{key}', but object was invalid");
						await cacheClient.RemoveAsync(key);
						return await GetChannelAsync(channelId);
					}
				}
				else
				{
					var data = await RatelimitHelper.ProcessRateLimitedAsync(
						$"channels:{channelId}", cache.Get,
						async () => await _restClient.GetAsync<DiscordChannelPacket>(DiscordApiRoutes.ChannelRoute(channelId))
						);

					await cacheClient.UpsertAsync(key, data.Data);
					packet = data.Data;
				}
			return packet;
		}

		public async Task<List<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
		{
			DiscordGuildPacket guild = await GetGuildAsync(guildId);
			return guild.Channels;
		}

		public async Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
		{
			string key = $"discord:guild:{guildId}";
			var cacheClient = cache.Get;
			
				if (await cacheClient.ExistsAsync(key))
				{
					var packet = await cacheClient.GetAsync<DiscordGuildPacket>(key);

					if (packet == null)
					{
						Log.Debug($"cache hit on '{key}', but object was invalid");
						await cacheClient.RemoveAsync(key);
						return await GetGuildAsync(guildId);
					}
					return packet;
				}
				else
				{
					var data = await RatelimitHelper.ProcessRateLimitedAsync(
						$"guilds:{guildId}", cacheClient,
						async () =>
						{
							return await _restClient.GetAsync<DiscordGuildPacket>(DiscordApiRoutes.GuildRoute(guildId));
						});

					await cacheClient.UpsertAsync(key, data.Data);
					return data.Data;
				}
			
		}

		public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
		{
			return (await GetGuildAsync(guildId)).Members.FirstOrDefault(x => x.User.Id == userId);
		}

		public async Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", cache.Get,
				async () =>
				{
					return await _restClient.GetAsync<DiscordMessagePacket>(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
				}
			)).Data;
		}

		public async Task<List<DiscordMessagePacket>> GetMessagesAsync(ulong channelId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", cache.Get,
				async () =>
				{
					return await _restClient.GetAsync<List<DiscordMessagePacket>>(DiscordApiRoutes.ChannelMessagesRoute(channelId));
				}
			)).Data;
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
			DiscordUserPacket packet = null;
			var cacheClient = cache.Get;
			{
				if (await cacheClient.ExistsAsync(key))
				{
					packet = await cacheClient.GetAsync<DiscordUserPacket>(key);
					if (packet == null)
					{
						Log.Debug($"cache hit on '{key}', but object was invalid");
						await cacheClient.RemoveAsync(key);
						packet = await GetUserAsync(userId);
					}
				}
				else
				{
					RestResponse<DiscordUserPacket> rc = await _restClient
						.GetAsync<DiscordUserPacket>(DiscordApiRoutes.UserRoute(userId));
					await cacheClient.UpsertAsync(key, rc.Data);
					packet = rc.Data;
				}
			}
			return packet;
		}

		public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
		{
			var cacheClient = cache.Get;
			{
				await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", cacheClient,
				async () =>
				{
					return await _restClient.PatchAsync(
						DiscordApiRoutes.GuildMemberRoute(guildId, userId),
						JsonConvert.SerializeObject(packet, serializer)
					);
				});
			}
		}

		public async Task RemoveGuildBanAsync(ulong guildId, ulong userId)
		{
			var cacheClient = cache.Get;
			{
				await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () => await _restClient.DeleteAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId))
				);
			}
		}

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
		{
			var cacheClient = cache.Get;
			{
				QueryString qs = new QueryString();

				if (!string.IsNullOrWhiteSpace(reason))
				{
					qs.Add("reason", reason);
				}

				await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () => await _restClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoute(guildId, userId) + qs.Query)
				);
			}
		}

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			var cacheClient = cache.Get;
			{
				await RatelimitHelper.ProcessRateLimitedAsync(
					$"guilds:{guildId}", cacheClient,
					async () =>
					{
						return await _restClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
					});
			}
		}

		public async Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args, bool toChannel = true)
		{
			if(stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			args.embed = new DiscordEmbed
			{
				Image = new EmbedImage
				{
					Url = "attachment://" + fileName
				}
			};

			string json = JsonConvert.SerializeObject(args, serializer);

			List<MultiformItem> items = new List<MultiformItem>();

			if (!string.IsNullOrEmpty(args.content))
			{
				var content = new StringContent(args.content);
				items.Add(new MultiformItem { Name = "content", Content = content });
			}

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

			var cacheClient = cache.Get;

			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}",
				cacheClient, async () =>
				{
					return new RestResponse<DiscordMessagePacket>(await _restClient
						.PostMultipartAsync(DiscordApiRoutes.ChannelMessagesRoute(channelId),
						items.ToArray()
					));
				}
			)).Data;
		}

		public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args, bool toChannel = true)
		{
			string json = JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			var cacheClient = cache.Get;
			{
				return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}",
				cacheClient, async () =>
				{
					return await _restClient.PostAsync<DiscordMessagePacket>(DiscordApiRoutes.ChannelMessagesRoute(channelId), json);
				})).Data;
			}
		}
	}
}
