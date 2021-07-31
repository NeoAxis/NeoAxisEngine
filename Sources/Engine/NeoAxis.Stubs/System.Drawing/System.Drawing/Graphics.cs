using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace System.Drawing
{
	public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
	{
		public delegate bool DrawImageAbort(IntPtr callbackdata);

		public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

		public CompositingMode CompositingMode
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Point RenderingOrigin
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public CompositingQuality CompositingQuality
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int TextContrast
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public InterpolationMode InterpolationMode
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Matrix Transform
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public GraphicsUnit PageUnit
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public float PageScale
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public float DpiX
		{
			get
			{
				throw null;
			}
		}

		public float DpiY
		{
			get
			{
				throw null;
			}
		}

		public Region Clip
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public RectangleF ClipBounds
		{
			get
			{
				throw null;
			}
		}

		public bool IsClipEmpty
		{
			get
			{
				throw null;
			}
		}

		public RectangleF VisibleClipBounds
		{
			get
			{
				throw null;
			}
		}

		public bool IsVisibleClipEmpty
		{
			get
			{
				throw null;
			}
		}

		public static Graphics FromHdc(IntPtr hdc)
		{
			throw null;
		}

		public static Graphics FromHdcInternal(IntPtr hdc)
		{
			throw null;
		}

		public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
		{
			throw null;
		}

		public static Graphics FromHwnd(IntPtr hwnd)
		{
			throw null;
		}

		public static Graphics FromHwndInternal(IntPtr hwnd)
		{
			throw null;
		}

		public static Graphics FromImage(Image image)
		{
			throw null;
		}

		public IntPtr GetHdc()
		{
			throw null;
		}

		public void ReleaseHdc(IntPtr hdc)
		{
			throw null;
		}

		public void ReleaseHdc()
		{
			throw null;
		}

		public void ReleaseHdcInternal(IntPtr hdc)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~Graphics()
		{
			throw null;
		}

		public void Flush()
		{
			throw null;
		}

		public void Flush(FlushIntention intention)
		{
			throw null;
		}

		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
		{
			throw null;
		}

		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
		{
			throw null;
		}

		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			throw null;
		}

		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			throw null;
		}

		public void ResetTransform()
		{
			throw null;
		}

		public void MultiplyTransform(Matrix matrix)
		{
			throw null;
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			throw null;
		}

		public void TranslateTransform(float dx, float dy)
		{
			throw null;
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			throw null;
		}

		public void ScaleTransform(float sx, float sy)
		{
			throw null;
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			throw null;
		}

		public void RotateTransform(float angle)
		{
			throw null;
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			throw null;
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
		{
			throw null;
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
		{
			throw null;
		}

		public Color GetNearestColor(Color color)
		{
			throw null;
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			throw null;
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			throw null;
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			throw null;
		}

		public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
		{
			throw null;
		}

		public void DrawLine(Pen pen, Point pt1, Point pt2)
		{
			throw null;
		}

		public void DrawLines(Pen pen, Point[] points)
		{
			throw null;
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			throw null;
		}

		public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			throw null;
		}

		public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			throw null;
		}

		public void DrawBeziers(Pen pen, PointF[] points)
		{
			throw null;
		}

		public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
		{
			throw null;
		}

		public void DrawBeziers(Pen pen, Point[] points)
		{
			throw null;
		}

		public void DrawRectangle(Pen pen, Rectangle rect)
		{
			throw null;
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			throw null;
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			throw null;
		}

		public void DrawRectangles(Pen pen, RectangleF[] rects)
		{
			throw null;
		}

		public void DrawRectangles(Pen pen, Rectangle[] rects)
		{
			throw null;
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			throw null;
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			throw null;
		}

		public void DrawEllipse(Pen pen, Rectangle rect)
		{
			throw null;
		}

		public void DrawEllipse(Pen pen, int x, int y, int width, int height)
		{
			throw null;
		}

		public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			throw null;
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			throw null;
		}

		public void DrawPolygon(Pen pen, Point[] points)
		{
			throw null;
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, PointF[] points)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, PointF[] points, float tension)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, Point[] points)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, Point[] points, float tension)
		{
			throw null;
		}

		public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
		{
			throw null;
		}

		public void DrawClosedCurve(Pen pen, PointF[] points)
		{
			throw null;
		}

		public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
		{
			throw null;
		}

		public void DrawClosedCurve(Pen pen, Point[] points)
		{
			throw null;
		}

		public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
		{
			throw null;
		}

		public void Clear(Color color)
		{
			throw null;
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			throw null;
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			throw null;
		}

		public void FillRectangle(Brush brush, Rectangle rect)
		{
			throw null;
		}

		public void FillRectangle(Brush brush, int x, int y, int width, int height)
		{
			throw null;
		}

		public void FillRectangles(Brush brush, RectangleF[] rects)
		{
			throw null;
		}

		public void FillRectangles(Brush brush, Rectangle[] rects)
		{
			throw null;
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			throw null;
		}

		public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
		{
			throw null;
		}

		public void FillPolygon(Brush brush, Point[] points)
		{
			throw null;
		}

		public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
		{
			throw null;
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			throw null;
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			throw null;
		}

		public void FillEllipse(Brush brush, Rectangle rect)
		{
			throw null;
		}

		public void FillEllipse(Brush brush, int x, int y, int width, int height)
		{
			throw null;
		}

		public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			throw null;
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, PointF[] points)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, Point[] points)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
		{
			throw null;
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
		{
			throw null;
		}

		public void FillRegion(Brush brush, Region region)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, PointF point)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
		{
			throw null;
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, int width)
		{
			throw null;
		}

		public SizeF MeasureString(string text, Font font, int width, StringFormat format)
		{
			throw null;
		}

		public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
		{
			throw null;
		}

		public void DrawIcon(Icon icon, int x, int y)
		{
			throw null;
		}

		public void DrawIcon(Icon icon, Rectangle targetRect)
		{
			throw null;
		}

		public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF point)
		{
			throw null;
		}

		public void DrawImage(Image image, float x, float y)
		{
			throw null;
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			throw null;
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			throw null;
		}

		public void DrawImage(Image image, Point point)
		{
			throw null;
		}

		public void DrawImage(Image image, int x, int y)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle rect)
		{
			throw null;
		}

		public void DrawImage(Image image, int x, int y, int width, int height)
		{
			throw null;
		}

		public void DrawImageUnscaled(Image image, Point point)
		{
			throw null;
		}

		public void DrawImageUnscaled(Image image, int x, int y)
		{
			throw null;
		}

		public void DrawImageUnscaled(Image image, Rectangle rect)
		{
			throw null;
		}

		public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
		{
			throw null;
		}

		public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF[] destPoints)
		{
			throw null;
		}

		public void DrawImage(Image image, Point[] destPoints)
		{
			throw null;
		}

		public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			throw null;
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
		{
			throw null;
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			throw null;
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			throw null;
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw null;
		}

		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw null;
		}

		public void SetClip(Graphics g)
		{
			throw null;
		}

		public void SetClip(Graphics g, CombineMode combineMode)
		{
			throw null;
		}

		public void SetClip(Rectangle rect)
		{
			throw null;
		}

		public void SetClip(Rectangle rect, CombineMode combineMode)
		{
			throw null;
		}

		public void SetClip(RectangleF rect)
		{
			throw null;
		}

		public void SetClip(RectangleF rect, CombineMode combineMode)
		{
			throw null;
		}

		public void SetClip(GraphicsPath path)
		{
			throw null;
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			throw null;
		}

		public void SetClip(Region region, CombineMode combineMode)
		{
			throw null;
		}

		public void IntersectClip(Rectangle rect)
		{
			throw null;
		}

		public void IntersectClip(RectangleF rect)
		{
			throw null;
		}

		public void IntersectClip(Region region)
		{
			throw null;
		}

		public void ExcludeClip(Rectangle rect)
		{
			throw null;
		}

		public void ExcludeClip(Region region)
		{
			throw null;
		}

		public void ResetClip()
		{
			throw null;
		}

		public void TranslateClip(float dx, float dy)
		{
			throw null;
		}

		public void TranslateClip(int dx, int dy)
		{
			throw null;
		}

		public object GetContextInfo()
		{
			throw null;
		}

		public bool IsVisible(int x, int y)
		{
			throw null;
		}

		public bool IsVisible(Point point)
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

		public bool IsVisible(int x, int y, int width, int height)
		{
			throw null;
		}

		public bool IsVisible(Rectangle rect)
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

		public GraphicsState Save()
		{
			throw null;
		}

		public void Restore(GraphicsState gstate)
		{
			throw null;
		}

		public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
		{
			throw null;
		}

		public GraphicsContainer BeginContainer()
		{
			throw null;
		}

		public void EndContainer(GraphicsContainer container)
		{
			throw null;
		}

		public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
		{
			throw null;
		}

		public void AddMetafileComment(byte[] data)
		{
			throw null;
		}

		public static IntPtr GetHalftonePalette()
		{
			throw null;
		}
	}
}
