using System.Drawing;

namespace System.Windows.Forms.VisualStyles
{
	public sealed class VisualStyleRenderer
	{
		public static bool IsSupported
		{
			get
			{
				throw null;
			}
		}

		public string Class
		{
			get
			{
				throw null;
			}
		}

		public int Part
		{
			get
			{
				throw null;
			}
		}

		public int State
		{
			get
			{
				throw null;
			}
		}

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public int LastHResult
		{
			get
			{
				throw null;
			}
		}

		public static bool IsElementDefined(VisualStyleElement element)
		{
			throw null;
		}

		public VisualStyleRenderer(VisualStyleElement element)
		{
			throw null;
		}

		public VisualStyleRenderer(string className, int part, int state)
		{
			throw null;
		}

		public void SetParameters(VisualStyleElement element)
		{
			throw null;
		}

		public void SetParameters(string className, int part, int state)
		{
			throw null;
		}

		public void DrawBackground(IDeviceContext dc, Rectangle bounds)
		{
			throw null;
		}

		public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
		{
			throw null;
		}

		public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
		{
			throw null;
		}

		public void DrawImage(Graphics g, Rectangle bounds, Image image)
		{
			throw null;
		}

		public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex)
		{
			throw null;
		}

		public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
		{
			throw null;
		}

		public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw)
		{
			throw null;
		}

		public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled)
		{
			throw null;
		}

		public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags)
		{
			throw null;
		}

		public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds)
		{
			throw null;
		}

		public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds)
		{
			throw null;
		}

		public Region GetBackgroundRegion(IDeviceContext dc, Rectangle bounds)
		{
			throw null;
		}

		public bool GetBoolean(BooleanProperty prop)
		{
			throw null;
		}

		public Color GetColor(ColorProperty prop)
		{
			throw null;
		}

		public int GetEnumValue(EnumProperty prop)
		{
			throw null;
		}

		public string GetFilename(FilenameProperty prop)
		{
			throw null;
		}

		public Font GetFont(IDeviceContext dc, FontProperty prop)
		{
			throw null;
		}

		public int GetInteger(IntegerProperty prop)
		{
			throw null;
		}

		public Size GetPartSize(IDeviceContext dc, ThemeSizeType type)
		{
			throw null;
		}

		public Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type)
		{
			throw null;
		}

		public Point GetPoint(PointProperty prop)
		{
			throw null;
		}

		public Padding GetMargins(IDeviceContext dc, MarginProperty prop)
		{
			throw null;
		}

		public string GetString(StringProperty prop)
		{
			throw null;
		}

		public Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
		{
			throw null;
		}

		public Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
		{
			throw null;
		}

		public TextMetrics GetTextMetrics(IDeviceContext dc)
		{
			throw null;
		}

		public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
		{
			throw null;
		}

		public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
		{
			throw null;
		}

		public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
		{
			throw null;
		}

		public bool IsBackgroundPartiallyTransparent()
		{
			throw null;
		}
	}
}
