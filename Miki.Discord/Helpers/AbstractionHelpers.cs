using Miki.Discord.Common;
using Miki.Discord.Common.Packets.API;
using Miki.Discord.Internal.Data;

namespace Miki.Discord.Helpers
{
    internal static class AbstractionHelpers
    {
        internal static IDiscordMessage ResolveMessage(IDiscordClient client, DiscordMessagePacket packet)
        {
            if (packet.GuildId.HasValue)
            {
                return new DiscordGuildMessage(packet, client);
            }
            return new DiscordMessage(packet, client);
        }

        internal static IDiscordChannel ResolveChannel(
            IDiscordClient client, DiscordChannelPacket packet)
        {
            switch (packet.Type)
            {
                case ChannelType.GuildText:
                    return new DiscordGuildTextChannel(packet, client);

                case ChannelType.DirectText:
                case ChannelType.GroupDirect:
                    return new DiscordTextChannel(packet, client);

                default:
                    return new DiscordGuildChannel(packet, client);
            }
        }

        internal static T ResolveChannelAs<T>(IDiscordClient client, DiscordChannelPacket packet)
            where T : IDiscordChannel
        {
            var channel = ResolveChannel(client, packet);
            if(channel is T t)
            {
                return t;
            }

            return default;
        }
    }
}
