using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Arguments;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
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
