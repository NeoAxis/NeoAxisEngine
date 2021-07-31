using System.Drawing.Drawing2D;

namespace System.Drawing
{
	public sealed class Region : MarshalByRefObject, IDisposable
	{
		public Region()
		{
			throw null;
		}

		public Region(RectangleF rect)
		{
			throw null;
		}

		public Region(Rectangle rect)
		{
			throw null;
		}

		public Region(GraphicsPath path)
		{
			throw null;
		}

		public Region(RegionData rgnData)
		{
			throw null;
		}

		public static Region FromHrgn(IntPtr hrgn)
		{
			throw null;
		}

		public Region Clone()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~Region()
		{
			throw null;
		}

		public void MakeInfinite()
		{
			throw null;
		}

		public void MakeEmpty()
		{
			throw null;
		}

		public void Intersect(RectangleF rect)
		{
			throw null;
		}

		public void Intersect(Rectangle rect)
		{
			throw null;
		}

		public void Intersect(GraphicsPath path)
		{
			throw null;
		}

		public void Intersect(Region region)
		{
			throw null;
		}

		public void ReleaseHrgn(IntPtr regionHandle)
		{
			throw null;
		}

		public void Union(RectangleF rect)
		{
			throw null;
		}

		public void Union(Rectangle rect)
		{
			throw null;
		}

		public void Union(GraphicsPath path)
		{
			throw null;
		}

		public void Union(Region region)
		{
			throw null;
		}

		public void Xor(RectangleF rect)
		{
			throw null;
		}

		public void Xor(Rectangle rect)
		{
			throw null;
		}

		public void Xor(GraphicsPath path)
		{
			throw null;
		}

		public void Xor(Region region)
		{
			throw null;
		}

		public void Exclude(RectangleF rect)
		{
			throw null;
		}

		public void Exclude(Rectangle rect)
		{
			throw null;
		}

		public void Exclude(GraphicsPath path)
		{
			throw null;
		}

		public void Exclude(Region region)
		{
			throw null;
		}

		public void Complement(RectangleF rect)
		{
			throw null;
		}

		public void Complement(Rectangle rect)
		{
			throw null;
		}

		public void Complement(GraphicsPath path)
		{
			throw null;
		}

		public void Complement(Region region)
		{
			throw null;
		}

		public void Translate(float dx, float dy)
		{
			throw null;
		}

		public void Translate(int dx, int dy)
		{
			throw null;
		}

		public void Transform(Matrix matrix)
		{
			throw null;
		}

		public RectangleF GetBounds(Graphics g)
		{
			throw null;
		}

		public IntPtr GetHrgn(Graphics g)
		{
			throw null;
		}

		public bool IsEmpty(Graphics g)
		{
			throw null;
		}

		public bool IsInfinite(Graphics g)
		{
			throw null;
		}

		public bool Equals(Region region, Graphics g)
		{
			throw null;
		}

		public RegionData GetRegionData()
		{
			throw null;
		}

		public bool IsVisible(float x, float y)
		{
			throw null;
		}

		public bool IsVisible(PointF point)
		{
			throw null;
		}

		public bool IsVisible(float x, float y, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(PointF point, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(float x, float y, float width, float height)
		{
			throw null;
		}

		public bool IsVisible(RectangleF rect)
		{
			throw null;
		}

		public bool IsVisible(float x, float y, float width, float height, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(RectangleF rect, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(int x, int y, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(Point point)
		{
			throw null;
		}

		public bool IsVisible(Point point, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(int x, int y, int width, int height)
		{
			throw null;
		}

		public bool IsVisible(Rectangle rect)
		{
			throw null;
		}

		public bool IsVisible(int x, int y, int width, int height, Graphics g)
		{
			throw null;
		}

		public bool IsVisible(Rectangle rect, Graphics g)
		{
			throw null;
		}

		public RectangleF[] GetRegionScans(Matrix matrix)
		{
			throw null;
		}
	}
}
