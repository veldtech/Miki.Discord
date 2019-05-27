using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace Miki.Discord.Common.Packets.Arguments
{
    public class UserAvatar
    {
        private static readonly byte[] JpegHeader = new byte[] { 0xff, 0xd8 };
        private static readonly byte[] Gif89aHeader = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };
        private static readonly byte[] Gif87aHeader = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };
        private static readonly byte[] PngHeader = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

        public MemoryStream Stream { get; set; }

        public ImageType Type { get; set; }

        public UserAvatar(Stream stream, ImageType type = ImageType.AUTO)
        {
            Stream = new MemoryStream();
            stream.CopyTo(Stream);

            if (type == ImageType.AUTO)
            {
                var buffer = Stream.GetBuffer();

                if(Validate(buffer.Take(JpegHeader.Length), JpegHeader))
                {
                    Type = ImageType.JPEG;
                    return;
                }

                if (Validate(buffer.Take(Gif89aHeader.Length), Gif89aHeader)
                    || Validate(buffer.Take(Gif87aHeader.Length), Gif87aHeader))
                {
                    Type = ImageType.GIF;
                    return;
                }

                if (Validate(buffer.Take(PngHeader.Length), PngHeader))
                {
                    Type = ImageType.PNG;
                    return;
                }
            }
        }

        public static implicit operator UserAvatar(Stream s)
        {
            return new UserAvatar(s);
        }

        private bool Validate(IEnumerable<byte> a, IEnumerable<byte> b)
        {
            if(a.Count() != b.Count())
            {
                return false;
            }

            for(var i = 0; i < a.Count(); i++)
            {
                if(a.ElementAt(i) != b.ElementAt(i))
                {
                    return false;
                }
            }
            return true;
        }
    }

    [DataContract]
    public class UserModifyArgs
    {
        [DataMember(Name = "avatar")]   
        public UserAvatar Avatar { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
