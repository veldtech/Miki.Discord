using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Helpers;

namespace Miki.Discord.Internal.Data
{
    internal class DiscordTextChannel : DiscordChannel, IDiscordTextChannel
    {
        public DiscordTextChannel(DiscordChannelPacket packet, IDiscordClient client)
            : base(packet, client)
        {
        }
        public async Task DeleteMessagesAsync(params ulong[] id)
        {
            if(id.Length == 0)
            {
                throw new ArgumentNullException();
            }

            if(id.Length < 2)
            {
                await client.ApiClient.DeleteMessageAsync(Id, id[0]);
            }

            if(id.Length > 100)
            {
                id = id.Take(100).ToArray();
            }

            await client.ApiClient.DeleteMessagesAsync(Id, id);
        }

        public async Task DeleteMessagesAsync(params IDiscordMessage[] messages)
        {
            await DeleteMessagesAsync(messages.Select(x => x.Id).ToArray());
        }

        public async Task<IDiscordMessage> GetMessageAsync(ulong id)
        {
            return new DiscordMessage(await client.ApiClient.GetMessageAsync(Id, id), client);
        }

        public async Task<IEnumerable<IDiscordMessage>> GetMessagesAsync(int amount = 100)
        {
            return (await client.ApiClient.GetMessagesAsync(Id, amount))
                .Select(x => new DiscordMessage(x, client));
        }

        public async Task<IDiscordMessage> SendFileAsync(
            Stream file,
            string fileName,
            string content,
            bool isTTS = false,
            DiscordEmbed embed = null)
        {
            return await client.SendFileAsync(
                Id, file, fileName, new MessageArgs(content, embed, isTTS));
        }

        public async Task<IDiscordMessage> SendMessageAsync(
            string content, bool isTTS = false, DiscordEmbed embed = null)
            => await DiscordChannelHelper.CreateMessageAsync(
                client, packet, new MessageArgs(content, embed, isTTS));

        public async Task TriggerTypingAsync()
        {
            await client.ApiClient.TriggerTypingAsync(Id);
        }
    }
}
