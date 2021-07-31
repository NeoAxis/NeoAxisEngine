using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
	[Serializable]
	public sealed class Bitmap : Image
	{
		public Bitmap(string filename)
		{
			throw null;
		}

		public Bitmap(string filename, bool useIcm)
		{
			throw null;
		}

		public Bitmap(Type type, string resource)
		{
			throw null;
		}

		public Bitmap(Stream stream)
		{
			throw null;
		}

		public Bitmap(Stream stream, bool useIcm)
		{
			throw null;
		}

		public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		{
			throw null;
		}

		public Bitmap(int width, int height, PixelFormat format)
		{
			throw null;
		}

		public Bitmap(int width, int height)
		{
			throw null;
		}

		public Bitmap(int width, int height, Graphics g)
		{
			throw null;
		}

		public Bitmap(Image original)
		{
			throw null;
		}

		public Bitmap(Image original, int width, int height)
		{
			throw null;
		}

		public static Bitmap FromHicon(IntPtr hicon)
		{
			throw null;
		}

		public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			throw null;
		}

		public IntPtr GetHbitmap()
		{
			throw null;
		}

		public IntPtr GetHbitmap(Color background)
		{
			throw null;
		}

		public IntPtr GetHicon()
		{
			throw null;
		}

		public Bitmap(Image original, Size newSize)
		{
			throw null;
		}

		public Bitmap Clone(Rectangle rect, PixelFormat format)
		{
			throw null;
		}

		public Bitmap Clone(RectangleF rect, PixelFormat format)
		{
			throw null;
		}

		public void MakeTransparent()
		{
			throw null;
		}

		public void MakeTransparent(Color transparentColor)
		{
			throw null;
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			throw null;
		}

		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			throw null;
		}

		public void UnlockBits(BitmapData bitmapdata)
		{
			throw null;
		}

		public Color GetPixel(int x, int y)
		{
			throw null;
		}

		public void SetPixel(int x, int y, Color color)
		{
			throw null;
		}

		public void SetResolution(float xDpi, float yDpi)
		{
			throw null;
		}
	}
}
