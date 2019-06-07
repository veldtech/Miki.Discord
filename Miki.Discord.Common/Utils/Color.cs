namespace Miki.Discord.Rest
{
	public class Color
	{

        public Color(uint baseValue)
		{
			Value = baseValue;
            R = (byte) (baseValue >> 16);
            G = (byte) (Value >> 8);
            B = (byte) Value;
        }

        public uint Value { get; }

        public byte R { get; }

        public byte G { get; }

        public byte B { get; }

        public Color(byte r, byte g, byte b)
			: this(((uint)r << 16) | ((uint)g << 8) | (uint)b)
		{
		}

		public Color(int r, int g, int b)
			: this(((uint)r << 16) | ((uint)g << 8) | (uint)b)
		{
		}

		public Color(float r, float g, float b)
			: this((byte)(r * 255), (byte)(g * 255), (byte)(b * 255))
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

        protected bool Equals(Color other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Color) obj);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}