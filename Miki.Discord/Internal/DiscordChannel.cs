﻿using Miki.Discord.Common;
using System;
using System.Threading.Tasks;

namespace Miki.Discord.Internal
{
    public class DiscordChannel : IDiscordChannel
    {
        protected DiscordChannelPacket packet;
        protected IDiscordClient client;

        public DiscordChannel()
        {
        }

        public DiscordChannel(DiscordChannelPacket packet, IDiscordClient client)
        {
            this.packet = packet;
            this.client = client;
        }

        public string Name
            => packet.Name;

        public ulong Id
            => packet.Id;

        public bool IsNsfw
            => packet?.IsNsfw.GetValueOrDefault(false) ?? false;

        public async Task DeleteAsync()
        {
            await client.ApiClient.DeleteChannelAsync(Id);
        }

        public Task ModifyAsync(object todo)
        {
            throw new NotImplementedException();
        }
    }
}