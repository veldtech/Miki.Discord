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
using System.Runtime.CompilerServices;
using Miki.Discord.Rest.Http;
using Miki.Discord.Rest.Exceptions;
using Miki.Discord.Common.Packets.Arguments;
using Miki.Discord.Rest.Converters;
using Miki.Discord.Common.Utils;

namespace Miki.Discord.Rest
{
    /// <summary>
    /// A client for Discord's API. Used to perform calls to their RESTful API.
    /// </summary>
    public class DiscordApiClient 
        : IApiClient, IGatewayApiClient, IDisposable
    {
        private readonly JsonSerializerSettings _serializer;

        /// <summary>
        /// Creates a new Discord API instance.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cache"></param>
        public DiscordApiClient(DiscordToken token, ICacheClient cache)
        {
            RestClient = new HttpClientFactory()
                .HasBaseUri(DiscordUtils.DiscordUrl + DiscordUtils.BaseUrl)
                .WithRateLimiter(new DiscordRateLimiter(cache))
                .CreateNew()
                .SetAuthorization(token.GetOAuthType(), token.Token);

            _serializer = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new UserAvatarConverter()
                }
            };
        }

        public Net.Http.HttpClient RestClient { get; }

        /// <summary>
        /// Adds a user to a guild's ban list.
        /// </summary>
        /// <param name="guildId">Id of the guild you want to ban a user from</param>
        /// <param name="userId">Id of the user you want to ban</param>
        /// <param name="pruneDays">Amount of days you want to prune messages from the user</param>
        /// <param name="reason">Reason for the ban</param>
        public async Task AddGuildBanAsync(
            ulong guildId, 
            ulong userId, 
            int pruneDays = 7, 
            string reason = null)
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


            var response = await RestClient.PutAsync(DiscordApiRoutes.GuildBan(guildId, userId) + qs.Query);
            HandleErrors(response);
        }

        /// <summary>
        /// Adds a role to a guild member.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        public async Task AddGuildMemberRoleAsync(
            ulong guildId, 
            ulong userId, 
            ulong roleId)
        {
            var response = await RestClient.PutAsync(
                DiscordApiRoutes.GuildMemberRole(guildId, userId, roleId));
            HandleErrors(response);
        }

        /// <summary>
        /// Creates a Direct channel to a user.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        public async Task<DiscordChannelPacket> CreateDMChannelAsync(
            ulong userId)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.UserMeChannels(), 
                $"{{\"recipient_id\":{userId}}}");
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        /// <summary>
        /// Creates and uploads a new emoji for a guild.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="args"></param>
        /// <returns>The created emoji.</returns>
        public async Task<DiscordEmoji> CreateEmojiAsync(
            ulong guildId, 
            EmojiCreationArgs args)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.GuildEmoji(guildId),
                JsonConvert.SerializeObject(args, _serializer)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        /// <summary>
        /// Creates a new role in the guild specified.
        /// </summary>
        /// <param name="guildId">The guild in which you want to create a role.</param>
        /// <param name="args">The properties of the role.</param>
        /// <returns>The role you've created, if successful</returns>
        public async Task<DiscordRolePacket> CreateGuildRoleAsync(
            ulong guildId, 
            CreateRoleArgs args)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.GuildRoles(guildId),
                JsonConvert.SerializeObject(args) ?? ""
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task CreateReactionAsync(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji)
        {
            var response = await RestClient.PutAsync(
                DiscordApiRoutes.MessageReactionMe(channelId, messageId, emoji)
            );
            HandleErrors(response);
        }

        public async Task DeleteChannelAsync(
            ulong channelId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.Channel(channelId));
            HandleErrors(response);
        }

        public async Task DeleteGuildAsync(
            ulong guildId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.Guild(guildId));
            HandleErrors(response);
        }

        public async Task DeleteEmojiAsync(
            ulong guildId, 
            ulong emojiId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.GuildEmoji(guildId, emojiId));
            HandleErrors(response);
        }

        public async Task DeleteMessageAsync(
            ulong channelId, 
            ulong messageId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.ChannelMessage(channelId, messageId));
            HandleErrors(response);
        }

        public async Task DeleteMessagesAsync(
            ulong channelId, 
            params ulong[] messageId)
        {
            var response = await RestClient.PostAsync(
                DiscordApiRoutes.ChannelBulkDeleteMessages(channelId), 
                JsonConvert.SerializeObject(new ChannelBulkDeleteArgs(messageId)));
            HandleErrors(response);
        }

        public async Task DeleteReactionAsync(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.MessageReactionMe(channelId, messageId, emoji));
            HandleErrors(response);
        }

        public async Task DeleteReactionAsync(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji, 
            ulong userId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.MessageReaction(channelId, messageId, emoji, userId));
            HandleErrors(response);
        }

        public async Task DeleteReactionsAsync(
            ulong channelId, 
            ulong messageId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.MessageReactions(channelId, messageId));
            HandleErrors(response);
        }

        public async Task<DiscordEmoji> EditEmojiAsync(
            ulong guildId, 
            ulong emojiId, 
            EmojiModifyArgs args)
        {
            var response = await RestClient.PatchAsync(
                DiscordApiRoutes.GuildEmoji(guildId, emojiId),
                JsonConvert.SerializeObject(args, _serializer)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        public void Dispose()
        {
            RestClient.Dispose();
        }

        public async Task<DiscordMessagePacket> EditMessageAsync(
            ulong channelId, 
            ulong messageId, 
            EditMessageArgs args)
        {
            var response = await RestClient.PatchAsync(
                DiscordApiRoutes.ChannelMessage(channelId, messageId),
                JsonConvert.SerializeObject(args, _serializer)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<DiscordRolePacket> EditRoleAsync(
            ulong guildId,
            DiscordRolePacket role)
        {
            var response = await RestClient.PutAsync(
                DiscordApiRoutes.GuildRole(guildId, role.Id),
                JsonConvert.SerializeObject(role)
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task<DiscordUserPacket> GetCurrentUserAsync()
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.UserMe());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        public async Task<DiscordChannelPacket> GetChannelAsync(
            ulong channelId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.Channel(channelId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordChannelPacket>(response.Body);
        }

        public async Task<IReadOnlyList<DiscordChannelPacket>> GetChannelsAsync(
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildChannels(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordChannelPacket>>(response.Body);
        }

        public async Task<IReadOnlyList<DiscordChannelPacket>> GetDMChannelsAsync()
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.UserMeChannels());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordChannelPacket>>(response.Body);
        }

        public async Task<DiscordEmoji> GetEmojiAsync(
            ulong guildId, 
            ulong emojiId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildEmoji(guildId, emojiId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji>(response.Body);
        }

        public async Task<DiscordEmoji[]> GetEmojisAsync(
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildEmoji(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordEmoji[]>(response.Body);
        }

        public async Task<DiscordGuildPacket> GetGuildAsync(
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.Guild(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildPacket>(response.Body);
        }

        public async Task<DiscordGuildMemberPacket> GetGuildUserAsync(
            ulong userId, 
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildMember(guildId, userId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordGuildMemberPacket>(response.Body);
        }

        public async Task<DiscordMessagePacket> GetMessageAsync(
            ulong channelId, 
            ulong messageId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.ChannelMessage(channelId, messageId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<IReadOnlyList<DiscordMessagePacket>> GetMessagesAsync(
            ulong channelId, 
            int amount = 100)
        {
            QueryString qs = new QueryString();

            qs.Add("limit", amount);

            var response = await RestClient.GetAsync(DiscordApiRoutes.ChannelMessages(channelId) + qs.Query);
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket[]>(response.Body);
        }

        public async Task<DiscordUserPacket[]> GetReactionsAsync(
            ulong channelId, 
            ulong messageId, 
            DiscordEmoji emoji)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.MessageReaction(channelId, messageId, emoji));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket[]>(response.Body);
        }

        public async Task<DiscordRolePacket> GetRoleAsync(
            ulong roleId, 
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildRole(guildId, roleId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordRolePacket>(response.Body);
        }

        public async Task<IReadOnlyList<DiscordRolePacket>> GetRolesAsync(
            ulong guildId)
        {
            var response = await RestClient.GetAsync(
                DiscordApiRoutes.GuildRoles(guildId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<List<DiscordRolePacket>>(response.Body);
        }

        public async Task<DiscordUserPacket> GetUserAsync(ulong userId)
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.User(userId));
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordUserPacket>(response.Body);
        }

        public async Task ModifySelfAsync(UserModifyArgs args)
        {
            if (args.Avatar.Type == ImageType.WEBP)
            {
                throw new InvalidDataException("Can't upload WEBP images.");
            }

            var json = JsonConvert.SerializeObject(args, _serializer);
            var response = await RestClient.PatchAsync(DiscordApiRoutes.UserMe(), json);
            HandleErrors(response);
        }

        public async Task ModifyGuildMemberAsync(
            ulong guildId, 
            ulong userId, 
            ModifyGuildMemberArgs packet)
        {
            var json = JsonConvert.SerializeObject(packet, _serializer);

            var response = await RestClient.PatchAsync(DiscordApiRoutes.GuildMember(guildId, userId), json);
            HandleErrors(response);
        }

        public async Task RemoveGuildBanAsync(
            ulong guildId, 
            ulong userId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.GuildBan(guildId, userId));
            HandleErrors(response);
        }

        public async Task RemoveGuildMemberAsync(
            ulong guildId, 
            ulong userId, 
            string reason = null)
        {
            QueryString qs = new QueryString();

            if (!string.IsNullOrWhiteSpace(reason))
            {
                qs.Add("reason", reason);
            }

            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.GuildMember(guildId, userId) + qs.Query);
            HandleErrors(response);
        }

        public async Task RemoveGuildMemberRoleAsync(
            ulong guildId, 
            ulong userId, 
            ulong roleId)
        {
            var response = await RestClient.DeleteAsync(
                DiscordApiRoutes.GuildMemberRole(guildId, userId, roleId));
            HandleErrors(response);
        }

        public async Task<DiscordMessagePacket> SendFileAsync(
            ulong channelId, 
            Stream stream, 
            string fileName, 
            MessageArgs args)
        {
            if (stream == null)
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

            List<MultiformItem> items = new List<MultiformItem>();

            if (!string.IsNullOrEmpty(args.Content))
            {
                var content = new StringContent(args.Content);
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
                DiscordApiRoutes.ChannelMessages(channelId),
                items.ToArray()
            );
            HandleErrors(response);
            return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
        }

        public async Task<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args)
        {
            string json = JsonConvert.SerializeObject(args, _serializer);
            {
                var response = await RestClient.PostAsync(DiscordApiRoutes.ChannelMessages(channelId), json);
                HandleErrors(response);
                return JsonConvert.DeserializeObject<DiscordMessagePacket>(response.Body);
            }
        }

        public async Task TriggerTypingAsync(ulong channelId)
        {
            var response = await RestClient.PostAsync(DiscordApiRoutes.ChannelTyping(channelId));
            HandleErrors(response);
        }

        public async Task<GatewayConnectionPacket> GetGatewayAsync()
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.Gateway());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        public async Task<GatewayConnectionPacket> GetGatewayBotAsync()
        {
            var response = await RestClient.GetAsync(DiscordApiRoutes.BotGateway());
            HandleErrors(response);
            return JsonConvert.DeserializeObject<GatewayConnectionPacket>(response.Body);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleErrors(HttpResponse response)
        {
            if (!response.Success)
            {
                throw new DiscordRestException(JsonConvert.DeserializeObject<DiscordRestError>(response.Body));
            }
        }
    }
}