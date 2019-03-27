using MessagePack;
using Miki.Discord.Common.Packets;
using ProtoBuf;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Miki.Discord.Common
{
	[ProtoContract]
	[MessagePackObject]
	public class DiscordEmoji
	{
		[ProtoMember(1)]
		[DataMember(Name ="id")]
		[Key(0)]
		public ulong? Id { get; set; }

		[ProtoMember(2)]
		[DataMember(Name ="name")]
		[Key(1)]
		public string Name { get; set; }

		[ProtoMember(3)]
		[DataMember(Name ="roles")]
		[Key(2)]
		public List<ulong> WhitelistedRoles { get; set; }

		[ProtoMember(4)]
		[DataMember(Name ="user")]
		[Key(3)]
		public DiscordUserPacket Creator { get; set; }

		[ProtoMember(5)]
		[Key(4)]
		[DataMember(Name ="require_colons")]
		public bool? RequireColons { get; set; }

		[ProtoMember(6)]
		[Key(5)]
		[DataMember(Name ="managed")]
		public bool? Managed { get; set; }

		[ProtoMember(7)]
		[Key(6)]
		[DataMember(Name ="animated")]
		public bool? Animated { get; set; }

		public static bool TryParse(string text, out DiscordEmoji emoji)
		{
			emoji = null;
			if (text.Length >= 4 && text[0] == '<' && (text[1] == ':' || (text[1] == 'a' && text[2] == ':')) && text[text.Length - 1] == '>')
			{
				bool animated = text[1] == 'a';
				int startIndex = animated ? 3 : 2;

				int splitIndex = text.IndexOf(':', startIndex);
				if (splitIndex == -1)
				{
					return false;
				}

				if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
				{
					return false;
				}

				string name = text.Substring(startIndex, splitIndex - startIndex);

				emoji = new DiscordEmoji
				{
					Name = name,
					Id = id,
					Animated = animated
				};
				return true;
			}
			else if (text.Length > 0 && text.All((t) => char.IsSurrogate(t)))
			{
				emoji = new DiscordEmoji
				{
					Id = null,
					Name = text
				};
				return true;
			}
			return false;
		}

        public override string ToString()
        {
            if(Id.HasValue)
            {
                return $"{Name}:{Id}";
            }
            return Name;
        }
	}
}