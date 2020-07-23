using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Discord.Common.Events;
using Miki.Discord.Common.Gateway;
using Miki.Discord.Common.Packets;
using Miki.Discord.Rest.Arguments;
using Miki.Discord.Rest.Converters;
using Miki.Discord.Rest.Exceptions;
using Miki.Discord.Rest.Http;
using Miki.Net.Http;
using Miki.Net.Http.Factories;
using Newtonsoft.Json;
using Miki.Discord.Common.Packets.API;

namespace Miki.Discord.Rest
{
    /// <summary>
    /// A client for Discord's API. Used to perform calls to their RESTful API.
    /// </summary>
    public class DiscordApiClient
        : IApiClient, IDisposable
    {
        private readonly IHttpClient httpClient;
        private readonly JsonSerializerSettings serializer;

        /// <summary>
        /// Creates a new Discord API instance.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cache"></param>
        public DiscordApiClient(DiscordToken token, ICacheClient cache)
        {
            httpClient = new HttpClientFactory()
                .HasBaseUri(DiscordHelpers.DiscordUrl + DiscordHelpers.BasePath)
                .WithRateLimiter(new DiscordRateLimiter(cache))
                .CreateNew()
                .SetAuthorization(token.GetOAuthType(), token.Token);

            serializer = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new UserAvatarConverter()
                }
            };
        }

        /// <inheritdoc/>
        public async Task AddGuildBanAsync(
            ulong guildId,
            ulong userId,
            int pruneDays = 7,
            string reason = null)
        {
            QueryString qs = new QueryString();

            if(!string.IsNullOrWhiteSpace(reason))
            {
                qs.Add("reason", reason);
            }

            if(pruneDays != 0)
            {
                qs.Add("delete-message-days", pruneDays);
            }

            var response = await httpClient.PutAsync(
                    DiscordApiRoutes.GuildBan(guildId, userId) + qs.Query)
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task AddGuildMemberRoleAsync(
            ulong guildId,
            ulong userId,
            ulong roleId)
        {
            var response = await httpClient.PutAsync(
                    DiscordApiRoutes.GuildMemberRole(guildId, userId, roleId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }
    
        /// <inheritdoc/>
        public async Task<DiscordChannelPacket> CreateDMChannelAsync(
            ulong userId)
        {
            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.UserMeChannels(),
                    $"{{\"recipient_id\":{userId}}}")
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordEmoji> CreateEmojiAsync(
            ulong guildId,
            EmojiCreationArgs args)
        {
            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.GuildEmoji(guildId),
                    JsonConvert.SerializeObject(args, serializer))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordRolePacket> CreateGuildRoleAsync(
            ulong guildId,
            CreateRoleArgs args)
        {
            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.GuildRoles(guildId),
                    JsonConvert.SerializeObject(args) ?? "")
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task CreateReactionAsync(
            ulong channelId,
            ulong messageId,
            DiscordEmoji emoji)
        {
            var response = await httpClient.PutAsync(
                    DiscordApiRoutes.MessageReactionMe(channelId, messageId, emoji))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteChannelAsync(
            ulong channelId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.Channel(channelId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteGuildAsync(
            ulong guildId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.Guild(guildId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteEmojiAsync(
            ulong guildId,
            ulong emojiId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.GuildEmoji(guildId, emojiId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteMessageAsync(
            ulong channelId,
            ulong messageId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.ChannelMessage(channelId, messageId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteMessagesAsync(
            ulong channelId,
            params ulong[] messageId)
        {
            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.ChannelBulkDeleteMessages(channelId),
                    JsonConvert.SerializeObject(new ChannelBulkDeleteArgs(messageId)))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteReactionAsync(
            ulong channelId,
            ulong messageId,
            DiscordEmoji emoji)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.MessageReactionMe(channelId, messageId, emoji))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteReactionAsync(
            ulong channelId,
            ulong messageId,
            DiscordEmoji emoji,
            ulong userId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.MessageReaction(channelId, messageId, emoji, userId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task DeleteReactionsAsync(
            ulong channelId,
            ulong messageId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.MessageReactions(channelId, messageId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task<DiscordEmoji> EditEmojiAsync(
            ulong guildId,
            ulong emojiId,
            EmojiModifyArgs args)
        {
            var response = await httpClient.PatchAsync(
                    DiscordApiRoutes.GuildEmoji(guildId, emojiId),
                    JsonConvert.SerializeObject(args, serializer))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }
        
        /// <inheritdoc/>
        public void Dispose()
        {
            // TODO: Dispose IHttpClient
        }

        /// <inheritdoc/>
        public async Task<DiscordMessagePacket> EditMessageAsync(
            ulong channelId,
            ulong messageId,
            EditMessageArgs args)
        {
            var response = await httpClient.PatchAsync(
                    DiscordApiRoutes.ChannelMessage(channelId, messageId),
                    JsonConvert.SerializeObject(args, serializer))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordRolePacket> EditRoleAsync(
            ulong guildId,
            DiscordRolePacket role)
        {
            var response = await httpClient.PutAsync(
                    DiscordApiRoutes.GuildRole(guildId, role.Id),
                    JsonConvert.SerializeObject(role))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordUserPacket> GetCurrentUserAsync()
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.UserMe())
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordChannelPacket> GetChannelAsync(
            ulong channelId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.Channel(channelId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DiscordChannelPacket>> GetChannelsAsync(
            ulong guildId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildChannels(guildId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordChannelPacket>>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DiscordChannelPacket>> GetDMChannelsAsync()
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.UserMeChannels())
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordChannelPacket>>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordEmoji> GetEmojiAsync(
            ulong guildId,
            ulong emojiId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildEmoji(guildId, emojiId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordEmoji[]> GetEmojisAsync(
            ulong guildId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildEmoji(guildId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji[]>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordGuildPacket> GetGuildAsync(
            ulong guildId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.Guild(guildId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(
            ulong userId,
            ulong guildId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildMember(guildId, userId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildMemberPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordMessagePacket> GetMessageAsync(
            ulong channelId,
            ulong messageId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.ChannelMessage(channelId, messageId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DiscordMessagePacket>> GetMessagesAsync(
            ulong channelId,
            int amount = 100)
        {
            QueryString qs = new QueryString();

            qs.Add("limit", amount);

            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.ChannelMessages(channelId) + qs.Query)
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket[]>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<int> GetPruneCountAsync(
            ulong guildId,
            int days)
        {
            if(days <= 0)
            {
                throw new InvalidOperationException(
                    $"Parameter '{nameof(days)}' cannot be lower than 1.");
            }

            QueryString qs = new QueryString();
            qs.Add("days", days);

            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildPrune(guildId) + qs.Query)
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordPruneObject>(response.Body).Pruned;
        }

        /// <inheritdoc/>
        public async Task<DiscordUserPacket[]> GetReactionsAsync(
            ulong channelId,
            ulong messageId,
            DiscordEmoji emoji)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.MessageReaction(channelId, messageId, emoji))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket[]>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DiscordRolePacket>> GetRolesAsync(
            ulong guildId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.GuildRoles(guildId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordRolePacket>>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<DiscordUserPacket> GetUserAsync(ulong userId)
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.User(userId))
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task ModifySelfAsync(UserModifyArgs args)
        {
            if(args.Avatar.Type == ImageType.WEBP)
            {
                throw new InvalidDataException("Can't upload WEBP images.");
            }

            var json = JsonConvert.SerializeObject(args, serializer);
            var response = await httpClient.PatchAsync(
                    DiscordApiRoutes.UserMe(),
                    json)
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task ModifyGuildMemberAsync(
            ulong guildId,
            ulong userId,
            ModifyGuildMemberArgs packet)
        {
            var response = await httpClient.PatchAsync(
                    DiscordApiRoutes.GuildMember(guildId, userId),
                    JsonConvert.SerializeObject(packet, serializer))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task<int?> PruneGuildMembersAsync(
            ulong guildId,
            int days,
            bool computePruneCount = false)
        {
            if(days <= 0)
            {
                throw new InvalidOperationException(
                    $"Parameter '{nameof(days)}' cannot be lower than 1.");
            }

            QueryString qs = new QueryString();
            qs.Add("days", days);
            qs.Add("compute_prune_count", computePruneCount);

            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.GuildPrune(
                        guildId) + qs.Query)
                .ConfigureAwait(false);
            HandleErrors(response);
            if(computePruneCount)
            {
                return JsonConvert.DeserializeObject<DiscordPruneObject>(response.Body).Pruned;
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task RemoveGuildBanAsync(
            ulong guildId,
            ulong userId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.GuildBan(guildId, userId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task RemoveGuildMemberAsync(
            ulong guildId,
            ulong userId,
            string reason = null)
        {
            QueryString qs = new QueryString();

            if(!string.IsNullOrWhiteSpace(reason))
            {
                qs.Add("reason", reason);
            }

            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.GuildMember(guildId, userId) + qs.Query)
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task RemoveGuildMemberRoleAsync(
            ulong guildId,
            ulong userId,
            ulong roleId)
        {
            var response = await httpClient.DeleteAsync(
                    DiscordApiRoutes.GuildMemberRole(guildId, userId, roleId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task<DiscordMessagePacket> SendFileAsync(
            ulong channelId,
            Stream stream,
            string fileName,
            MessageArgs args)
        {
            if(stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            args.Embed = new DiscordEmbed
            {
                Image = new EmbedImage
                {
                    Url = "attachment://" + fileName
                }
            };


            var form = new MultipartFormDataContent();

            if(!string.IsNullOrEmpty(args.Content))
            {
                form.Add(new StringContent(args.Content), "content");
            }

            if(stream.CanSeek)
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
                memoryStream.Position = 0;
                stream = memoryStream;
            }

            form.Add(new StreamContent(stream), "file", fileName);

            var response = await httpClient.InnerClient.PostAsync(
                DiscordApiRoutes.ChannelMessages(channelId), form);
            await HandleErrorsAsync(response);

            return JsonConvert.DeserializeObject<DiscordMessagePacket>(
                await response.Content.ReadAsStringAsync());
        }

        /// <inheritdoc/>
        public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args)
        {
            string json = JsonConvert.SerializeObject(args, serializer);
            {
                var response = await httpClient.PostAsync(
                        DiscordApiRoutes.ChannelMessages(channelId), json)
                    .ConfigureAwait(false);
                HandleErrors(response);
                return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
            }
        }

        /// <inheritdoc/>
        public async Task TriggerTypingAsync(ulong channelId)
        {
            var response = await httpClient.PostAsync(
                    DiscordApiRoutes.ChannelTyping(channelId))
                .ConfigureAwait(false);
            HandleErrors(response);
        }

        /// <inheritdoc/>
        public async Task<GatewayConnectionPacket> GetGatewayAsync()
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.Gateway())
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        /// <inheritdoc/>
        public async Task<GatewayConnectionPacket> GetGatewayBotAsync()
        {
            var response = await httpClient.GetAsync(
                    DiscordApiRoutes.BotGateway())
                .ConfigureAwait(false);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleErrors(IHttpResponse response)
        {
            if(!response.Success)
            {
                throw new DiscordRestException(
                    JsonConvert.DeserializeObject<DiscordRestError>(response.Body));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task HandleErrorsAsync(HttpResponseMessage response)
        {
            if(!response.IsSuccessStatusCode)
            {
                throw new DiscordRestException(
                    JsonConvert.DeserializeObject<DiscordRestError>(
                        await response.Content.ReadAsStringAsync()));
            }
        }
    }
}