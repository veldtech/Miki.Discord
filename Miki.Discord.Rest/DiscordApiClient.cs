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
		public RestClient HttpClient { get; private set; }

		ICacheClient _cache;

		readonly JsonSerializerSettings serializer;
		
		public DiscordApiClient(string token, ICacheClient cache)
		{
			HttpClient = new RestClient(DiscordHelper.DiscordUrl + DiscordHelper.BaseUrl)
				.SetAuthorization("Bot", token);
			_cache = cache;

			serializer = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
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

			await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.PutAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId) + qs.Query);
				});
		}

		public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			await RatelimitHelper.ProcessRateLimitedAsync(
			$"guilds:{guildId}", _cache,
			async () =>
			{
				return await HttpClient.PutAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
			});
		}

		public async Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
		{
			return (await HttpClient.PostAsync<DiscordChannelPacket>(DiscordApiRoutes.UserMeChannelsRoute(), $"{{ \"recipient_id\": {userId} }}")).Data;
		}

		public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.PostAsync<DiscordRolePacket>(
						DiscordApiRoutes.GuildRolesRoute(guildId),
						JsonConvert.SerializeObject(args) ?? ""
					);
				})).Data;
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}:delete", _cache,
				async () =>
				{
					return await HttpClient.DeleteAsync(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
				});
		}

		public async Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", _cache,
				async () =>
				{
					return await HttpClient.PatchAsync<DiscordMessagePacket>(
						DiscordApiRoutes.ChannelMessageRoute(channelId, messageId),
						JsonConvert.SerializeObject(args, serializer)
					);
				})).Data;
		}

		public async Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () => await HttpClient.PutAsync<DiscordRolePacket>(
					DiscordApiRoutes.GuildRoleRoute(guildId, role.Id),
					JsonConvert.SerializeObject(role)
				)
			)).Data;
		}

		public async Task<DiscordUserPacket> GetCurrentUserAsync()
		{
			return (await HttpClient.GetAsync<DiscordUserPacket>(DiscordApiRoutes.UserMeRoute())).Data;
		}

		public async Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", _cache,
				async () => await HttpClient.GetAsync<DiscordChannelPacket>(DiscordApiRoutes.ChannelRoute(channelId))
				)).Data;
		}

		public async Task<List<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<List<DiscordChannelPacket>>(DiscordApiRoutes.GuildChannelsRoute(guildId));
				})).Data;
		}

		public async Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<DiscordGuildPacket>(DiscordApiRoutes.GuildRoute(guildId));
				})).Data;
		}

		public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<DiscordGuildMemberPacket>(DiscordApiRoutes.GuildMemberRoute(guildId, userId));
				}
			)).Data;
		}

		public async Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<DiscordMessagePacket>(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
				}
			)).Data;
		}

		public async Task<List<DiscordMessagePacket>> GetMessagesAsync(ulong channelId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<List<DiscordMessagePacket>>(DiscordApiRoutes.ChannelMessagesRoute(channelId));
				}
			)).Data;
		}

		public async Task<List<DiscordRolePacket>> GetRolesAsync(ulong guildId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<List<DiscordRolePacket>>(DiscordApiRoutes.GuildRolesRoute(guildId));
				}
			)).Data;
		}

		public async Task<DiscordUserPacket> GetUserAsync(ulong userId)
		{
			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"user", _cache,
				async () =>
				{
					return await HttpClient.GetAsync<DiscordUserPacket>(DiscordApiRoutes.UserRoute(userId));
				})).Data;
		}

		public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
		{
			await RatelimitHelper.ProcessRateLimitedAsync(
			$"guilds:{guildId}", _cache,
			async () =>
			{
				return await HttpClient.PatchAsync(
					DiscordApiRoutes.GuildMemberRoute(guildId, userId),
					JsonConvert.SerializeObject(packet, serializer)
				);
			});
		}

		public async Task RemoveGuildBanAsync(ulong guildId, ulong userId)
		{
			await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () => await HttpClient.DeleteAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId))
			);
		}

		public async Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
		{
			QueryString qs = new QueryString();

			if (!string.IsNullOrWhiteSpace(reason))
			{
				qs.Add("reason", reason);
			}

			await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () => await HttpClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoute(guildId, userId) + qs.Query)
			);
		}

		public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
		{
			await RatelimitHelper.ProcessRateLimitedAsync(
				$"guilds:{guildId}", _cache,
				async () =>
				{
					return await HttpClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
				});
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

			return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}",
				_cache, async () =>
				{
					return new RestResponse<DiscordMessagePacket>(await HttpClient
						.PostMultipartAsync(DiscordApiRoutes.ChannelMessagesRoute(channelId),
						items.ToArray()
					));
				}
			)).Data;
		}

		public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args, bool toChannel = true)
		{
			string json = JsonConvert.SerializeObject(args, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			{
				return (await RatelimitHelper.ProcessRateLimitedAsync(
				$"channels:{channelId}",
				_cache, async () =>
				{
					return await HttpClient.PostAsync<DiscordMessagePacket>(DiscordApiRoutes.ChannelMessagesRoute(channelId), json);
				})).Data;
			}
		}
	}
}
