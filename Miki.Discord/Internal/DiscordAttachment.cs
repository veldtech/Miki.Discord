using Miki.Discord.Common;
using Miki.Discord.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Internal
{
    internal class DiscordAttachment : IDiscordAttachment
    {
        readonly DiscordAttachmentPacket _packet;

        internal DiscordAttachment(DiscordAttachmentPacket packet)
        {
            _packet = packet;
        }

        /// <inheritdoc/>
        public string FileName => _packet.FileName;

        /// <inheritdoc/>
        public int? Height => _packet.Height;

        /// <inheritdoc/>
        public ulong Id => _packet.Id;

        /// <inheritdoc/>
        public string ProxyUrl => _packet.ProxyUrl;

        /// <inheritdoc/>
        public int Size => _packet.Size;

        /// <inheritdoc/>
        public string Url => _packet.Url;

        /// <inheritdoc/>
        public int? Width => _packet.Width;
    }
}
