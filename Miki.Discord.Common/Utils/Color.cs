using System;

namespace Miki.Discord.Rest
{
	public struct Color : IEquatable<Color>
	{
        public uint Value { get; }

        public byte R => (byte)(Value >> 16);
        public byte G => (byte)(Value >> 8);
        public byte B => (byte)Value;

        public Color(uint baseValue)
        {
            Value = baseValue;
        }
        public Color(byte r, byte g, byte b)
			: this(((uint)r << 16) | ((uint)g << 8) | (uint)b)
		{
		}
		public Color(int r, int g, int b)
			: this(((uint)r << 16) | ((uint)g << 8) | (uint)b)
		{
		}
		public Color(float r, float g, float b)
			: this((byte)(r * byte.MaxValue), (byte)(g * byte.MaxValue), (byte)(b * byte.MaxValue))
		{
		}

		public Color Lerp(Color c, float t)
		{
			return Lerp(this, c, t);
		}

		public static Color Lerp(Color colorA, Color ColorB, float time)
		{
			int newR = (int)(colorA.R + (ColorB.R - colorA.R) * time);
			int newG = (int)(colorA.G + (ColorB.G - colorA.G) * time);
			int newB = (int)(colorA.B + (ColorB.B - colorA.B) * time);
			return new Color(newR, newG, newB);
		}

		public override string ToString()
		{
			return $"#{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}";
		}

        public static bool operator==(Color c, int value)
            => value == c.Value;
        public static bool operator !=(Color c, int value)
            => value != c.Value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Color)obj);
        }

        public bool Equals(Color other)
        {
            return other.Value == Value;
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}