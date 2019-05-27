using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Helpers
{
    public static class DiscordChannelHelper
    {
        public static async Task<DiscordMessage> CreateMessageAsync(
            DiscordClient client, 
            DiscordChannelPacket channel, 
            MessageArgs args)
        {
            var message = await client.ApiClient.SendMessageAsync(channel.Id, args);
            if(channel.Type == ChannelType.GUILDTEXT 
                || channel.Type == ChannelType.GUILDVOICE
                || channel.Type == ChannelType.CATEGORY)
            {
                message.GuildId = channel.GuildId;
            }
            return new DiscordMessage(message, client);
        }
    }
}
