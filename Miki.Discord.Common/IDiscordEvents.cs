using Miki.Discord.Common;

namespace Miki.Discord.Events
{
    public interface IDiscordEvents
    {
        void SubscribeTo(IGateway gateway);
    }
}
