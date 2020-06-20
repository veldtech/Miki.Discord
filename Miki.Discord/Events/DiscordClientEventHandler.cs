namespace Miki.Discord.Events
{
    using Miki.Discord.Cache;
    using Miki.Discord.Common;

    public class DiscordClientEventHandler : IDiscordEvents
    {
        private readonly ICacheHandler cache;

        public DiscordClientEventHandler(ICacheHandler cache)
        {
            this.cache = cache;
        }


        /// <inheritdoc />
        public void SubscribeTo(IGateway gateway)
        {
        }
    }
}
