namespace Miki.Discord.Internal
{
    using Miki.Discord.Common;
    using Miki.Discord.Common.Packets;
    using System;
    using System.Threading.Tasks;

    public class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        public DiscordSelfUser(DiscordUserPacket user, IDiscordClient client)
            : base(user, client)
        { }

        public Task<IDiscordChannel> GetDMChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task ModifyAsync(Action<UserModifyArgs> modifyArgs)
        {
            var args = new UserModifyArgs();
            modifyArgs(args);
            await client.ApiClient.ModifySelfAsync(args);
        }
    }
}
