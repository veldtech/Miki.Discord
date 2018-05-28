namespace Miki.Discord.Rest
{
	public class Color
	{
		public int r = 0, g = 0, b = 0;

		public Color(int r, int g, int b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public Color Lerp(Color c, float t)
		{
			return Lerp(this, c, t);
		}

		public int ToHex()
		{
			return ToHex(r, g, b);
		}

		public static Color Lerp(Color colorA, Color ColorB, float time)
		{
			int newR = (int)(colorA.r + (ColorB.r - colorA.r) * time);
			int newG = (int)(colorA.g + (ColorB.g - colorA.g) * time);
			int newB = (int)(colorA.b + (ColorB.b - colorA.b) * time);
			return new Color(newR, newG, newB);
		}

		public static int ToHex(Color c)
		{
			return ToHex(c.r, c.g, c.b);
		}
		public static int ToHex(int r, int g, int b)
		{
			return (255 << 24) | ((byte)r << 16) | ((byte)g << 8) | ((byte)b << 0);
		}
	}
}