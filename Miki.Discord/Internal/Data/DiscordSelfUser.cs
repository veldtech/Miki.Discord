using System;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Discord.Common.Packets;

namespace Miki.Discord.Internal.Data
{
    public class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        /// <inheritdoc />
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
