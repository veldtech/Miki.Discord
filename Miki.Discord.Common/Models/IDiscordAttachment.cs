using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface IDiscordAttachment
    {
        /// <summary>
        /// Full name of the file attached.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// The height of the file (if the attachment is an image).
        /// </summary>
        int? Height { get; }

        /// <summary>
        /// The attachment Id.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// The proxy version of the Url.
        /// </summary>
        string ProxyUrl { get; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// The source url of the attachment.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The width of the file (if the attachment is an image).
        /// </summary>
        int? Width { get; }
    }
}
