using Miki.Discord.Common.Models;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public interface IDiscordGuildReaction : IDiscordReaction, IContainsGuild 
    {}

    public interface IDiscordReaction
    {
        /// <summary>
        /// Channel where the message contained into.
        /// </summary>
        ulong ChannelId { get; }
        
        /// <summary>
        /// The emoji information.
        /// </summary>
        DiscordEmoji Emoji { get; }

        /// <summary>
        /// ID of the message that was reacted on.
        /// </summary>
        ulong MessageId { get; }
        
        /// <summary>
        /// User who reacted.
        /// </summary>
        ValueTask<IDiscordUser> GetUserAsync();

        /// <summary>
        /// Gets the channel where the message appeared.
        /// </summary>
        ValueTask<IDiscordTextChannel> GetChannelAsync();
    }
}