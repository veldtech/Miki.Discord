namespace Miki.Discord.Common.Models
{
    using System.Threading.Tasks;

    /// <summary>
    /// This member has a guild connection.
    /// </summary>
    public interface IContainsGuild
    {
        ulong GuildId { get; }

        Task<IDiscordGuild> GetGuildAsync();
    }
}
