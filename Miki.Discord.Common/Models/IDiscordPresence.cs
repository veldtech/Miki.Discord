namespace Miki.Discord.Common
{
    using Miki.Discord.Common.Packets;

    public interface IDiscordPresence
    {
        DiscordActivity Activity { get; }
        UserStatus Status { get; }
    }
}