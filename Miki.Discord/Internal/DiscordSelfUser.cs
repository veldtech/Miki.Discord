using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using Miki.Discord.Common.Packets.Arguments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
    class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        public DiscordSelfUser(DiscordUserPacket user, DiscordClient client)
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
            await _client.ApiClient.ModifySelfAsync(args);
        }
    }
}
