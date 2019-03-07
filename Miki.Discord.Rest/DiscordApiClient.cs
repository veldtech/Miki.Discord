using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest.Arguments;
using Miki.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Miki.Net.Http;
using System.Net.Http;
using Miki.Discord.Rest.Http;
using Miki.Discord.Rest.Exceptions;

namespace Miki.Discord.Rest
{
    public class DiscordApiClient : IApiClient, IGatewayApiClient
    {
        public Net.Http.HttpClient RestClient { get; private set; }

        private readonly JsonSerializerSettings serializer;

        public DiscordApiClient(string token, ICacheClient cache)
        {
            RestClient = new HttpClientFactory()
                .HasBaseUri(DiscordHelper.DiscordUrl + DiscordHelper.BaseUrl)
                .WithRateLimiter(new DiscordRateLimiter(cache))
                .CreateNew()
                .SetAuthorization("Bot", token);

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


            var response = await RestClient.PutAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId) + qs.Query);
            HandleErrors(response);
        }

        public async Task AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var response = await RestClient.PutAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
            HandleErrors(response);
        }

        public async Task<DiscordChannelPacket> CreateDMChannelAsync(ulong userId)
        {
            var response = await RestClient.PostAsync(DiscordApiRoutes.UserMeChannelsRoute(), $"{{ \"recipient_id\": {userId} }}");
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        public async Task<DiscordEmoji> CreateEmojiAsync(ulong guildId, EmojiCreationArgs args)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.GuildEmojiRoute(guildId),
                JsonConvert.SerializeObject(args, serializer)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        public async Task<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.GuildRolesRoute(guildId),
                JsonConvert.SerializeObject(args) ?? ""
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            var response = await RestClient.PutAsync(
                DiscordApiRoutes.MessageReactionMeRoute(channelId, messageId, emoji)
            );
            HandleErrors(response);
        }

        public async Task DeleteChannelAsync(ulong channelId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.ChannelRoute(channelId));
            HandleErrors(response);
        }

        public async Task DeleteGuildAsync(ulong guildId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.GuildRoute(guildId));
            HandleErrors(response);
        }

        public async Task DeleteEmojiAsync(ulong guildId, ulong emojiId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.GuildEmojiRoute(guildId, emojiId));
            HandleErrors(response);
        }

        public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
            HandleErrors(response);
        }

        public async Task DeleteMessagesAsync(ulong channelId, ulong[] messageId)
        {
            var response = await RestClient.PostAsync(DiscordApiRoutes.ChannelBulkDeleteMessages(channelId), JsonConvert.SerializeObject(new ChannelBulkDeleteArgs(messageId)));
            HandleErrors(response);
        }

        public async Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.MessageReactionMeRoute(channelId, messageId, emoji));
            HandleErrors(response);
        }

        public async Task DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.MessageReactionRoute(channelId, messageId, emoji, userId));
            HandleErrors(response);
        }

        public async Task DeleteReactionsAsync(ulong channelId, ulong messageId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.MessageReactionsRoute(channelId, messageId));
            HandleErrors(response);
        }

        public async Task<DiscordEmoji> EditEmojiAsync(ulong guildId, ulong emojiId, EmojiModifyArgs args)
        {
            var response = await RestClient.PatchAsync(
                        DiscordApiRoutes.GuildEmojiRoute(guildId, emojiId),
                        JsonConvert.SerializeObject(args, serializer)

            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        public async Task<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args)
        {
            var response = await RestClient.PatchAsync(
                DiscordApiRoutes.ChannelMessageRoute(channelId, messageId),
                JsonConvert.SerializeObject(args, serializer)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            var response = await RestClient.PutAsync(
                DiscordApiRoutes.GuildRoleRoute(guildId, role.Id),
                JsonConvert.SerializeObject(role)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task<DiscordUserPacket> GetCurrentUserAsync()
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.UserMeRoute());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        public async Task<DiscordChannelPacket> GetChannelAsync(ulong channelId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.ChannelRoute(channelId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        public async Task<List<DiscordChannelPacket>> GetChannelsAsync(ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildChannelsRoute(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordChannelPacket>>(response.Body);
        }

        public async Task<DiscordEmoji> GetEmojiAsync(ulong guildId, ulong emojiId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildEmojiRoute(guildId, emojiId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        public async Task<DiscordEmoji[]> GetEmojisAsync(ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildEmojiRoute(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji[]>(response.Body);
        }

        public async Task<DiscordGuildPacket> GetGuildAsync(ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildRoute(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildPacket>(response.Body);
        }

        public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildMemberRoute(guildId, userId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildMemberPacket>(response.Body);
        }

        public async Task<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.ChannelMessageRoute(channelId, messageId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<List<DiscordMessagePacket>> GetMessagesAsync(ulong channelId, int amount = 100)
        {
            QueryString qs = new QueryString();

            qs.Add("limit", amount);

            var response = await RestClient.GetAsync(DiscordApiRoutes.ChannelMessagesRoute(channelId) + qs.Query);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordMessagePacket>>(response.Body);
        }

        public async Task<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.MessageReactionRoute(channelId, messageId, emoji));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket[]>(response.Body);
        }

        public async Task<DiscordRolePacket> GetRoleAsync(ulong roleId, ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildRoleRoute(guildId, roleId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task<List<DiscordRolePacket>> GetRolesAsync(ulong guildId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GuildRolesRoute(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordRolePacket>>(response.Body);
        }

        public async Task<DiscordUserPacket> GetUserAsync(ulong userId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.UserRoute(userId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet)
        {
            var response = await RestClient.PatchAsync(
                DiscordApiRoutes.GuildMemberRoute(guildId, userId),
                JsonConvert.SerializeObject(packet, serializer)
            );
            HandleErrors(response);
        }

        public async Task RemoveGuildBanAsync(ulong guildId, ulong userId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.GuildBanRoute(guildId, userId));
            HandleErrors(response);
        }

        public async Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason = null)
        {
            QueryString qs = new QueryString();

            if (!string.IsNullOrWhiteSpace(reason))
            {
                qs.Add("reason", reason);
            }

            var response = await RestClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoute(guildId, userId) + qs.Query);
            HandleErrors(response);
        }

        public async Task RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            var response = await RestClient.DeleteAsync(DiscordApiRoutes.GuildMemberRoleRoute(guildId, userId, roleId));
            HandleErrors(response);
        }

        public async Task<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args, bool toChannel = true)
        {
            if (stream == null)
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

            var response = await RestClient.PostMultipartAsync(
                DiscordApiRoutes.ChannelMessagesRoute(channelId),
                items.ToArray()
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args)
        {
            string json = JsonConvert.SerializeObject(args, serializer);
            {
                var response = await RestClient.PostAsync(DiscordApiRoutes.ChannelMessagesRoute(channelId), json);
                HandleErrors(response);
                return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
            }
        }

        public async Task TriggerTypingAsync(ulong channelId)
        {
            var response = await RestClient.PostAsync(DiscordApiRoutes.ChannelTypingRoute(channelId));
            HandleErrors(response);
        }

        public async Task<GatewayConnectionPacket> GetGatewayAsync()
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.GatewayRoute());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        public async Task<GatewayConnectionPacket> GetGatewayBotAsync()
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.BotGatewayRoute());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        private void HandleErrors(HttpResponse response)
        {
            if (response.Success)
            {
                return;
            }

            DiscordRestError error = JsonConvert.DeserializeObject<DiscordRestError>(response.Body);
            throw new DiscordRestException(error);
        }
    }
}