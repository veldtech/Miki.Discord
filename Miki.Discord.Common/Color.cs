using System;

namespace Miki.Discord.Rest
{
	public class Color
	{
		public uint Value => _value;

		public byte R => (byte)(_value >> 16);
		public byte G => (byte)(_value >> 8);
		public byte B => (byte)(_value);

		private uint _value;

		public Color(uint baseValue)
		{
			_value = baseValue;
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
	}
}