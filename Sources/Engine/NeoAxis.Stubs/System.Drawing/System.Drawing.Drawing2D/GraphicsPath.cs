namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
	{
		public FillMode FillMode
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

		public PathData PathData
		{
			get
			{
				throw null;
			}
		}

		public int PointCount
		{
			get
			{
				throw null;
			}
		}

		public byte[] PathTypes
		{
			get
			{
				throw null;
			}
		}

		public PointF[] PathPoints
		{
			get
			{
				throw null;
			}
		}

		public GraphicsPath()
		{
			throw null;
		}

		public GraphicsPath(FillMode fillMode)
		{
			throw null;
		}

		public GraphicsPath(PointF[] pts, byte[] types)
		{
			throw null;
		}

		public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
		{
			throw null;
		}

		public GraphicsPath(Point[] pts, byte[] types)
		{
			throw null;
		}

		public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~GraphicsPath()
		{
			throw null;
		}

		public void Reset()
		{
			throw null;
		}

		public void StartFigure()
		{
			throw null;
		}

		public void CloseFigure()
		{
			throw null;
		}

		public void CloseAllFigures()
		{
			throw null;
		}

		public void SetMarkers()
		{
			throw null;
		}

		public void ClearMarkers()
		{
			throw null;
		}

		public void Reverse()
		{
			throw null;
		}

		public PointF GetLastPoint()
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

		public bool IsVisible(float x, float y, Graphics graphics)
		{
			throw null;
		}

		public bool IsVisible(PointF pt, Graphics graphics)
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

		public bool IsVisible(int x, int y, Graphics graphics)
		{
			throw null;
		}

		public bool IsVisible(Point pt, Graphics graphics)
		{
			throw null;
		}

		public bool IsOutlineVisible(float x, float y, Pen pen)
		{
			throw null;
		}

		public bool IsOutlineVisible(PointF point, Pen pen)
		{
			throw null;
		}

		public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
		{
			throw null;
		}

		public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
		{
			throw null;
		}

		public bool IsOutlineVisible(int x, int y, Pen pen)
		{
			throw null;
		}

		public bool IsOutlineVisible(Point point, Pen pen)
		{
			throw null;
		}

		public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
		{
			throw null;
		}

		public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
		{
			throw null;
		}

		public void AddLine(PointF pt1, PointF pt2)
		{
			throw null;
		}

		public void AddLine(float x1, float y1, float x2, float y2)
		{
			throw null;
		}

		public void AddLines(PointF[] points)
		{
			throw null;
		}

		public void AddLine(Point pt1, Point pt2)
		{
			throw null;
		}

		public void AddLine(int x1, int y1, int x2, int y2)
		{
			throw null;
		}

		public void AddLines(Point[] points)
		{
			throw null;
		}

		public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			throw null;
		}

		public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			throw null;
		}

		public void AddBeziers(PointF[] points)
		{
			throw null;
		}

		public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
		{
			throw null;
		}

		public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			throw null;
		}

		public void AddBeziers(params Point[] points)
		{
			throw null;
		}

		public void AddCurve(PointF[] points)
		{
			throw null;
		}

		public void AddCurve(PointF[] points, float tension)
		{
			throw null;
		}

		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			throw null;
		}

		public void AddCurve(Point[] points)
		{
			throw null;
		}

		public void AddCurve(Point[] points, float tension)
		{
			throw null;
		}

		public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
		{
			throw null;
		}

		public void AddClosedCurve(PointF[] points)
		{
			throw null;
		}

		public void AddClosedCurve(PointF[] points, float tension)
		{
			throw null;
		}

		public void AddClosedCurve(Point[] points)
		{
			throw null;
		}

		public void AddClosedCurve(Point[] points, float tension)
		{
			throw null;
		}

		public void AddRectangle(RectangleF rect)
		{
			throw null;
		}

		public void AddRectangles(RectangleF[] rects)
		{
			throw null;
		}

		public void AddRectangle(Rectangle rect)
		{
			throw null;
		}

		public void AddRectangles(Rectangle[] rects)
		{
			throw null;
		}

		public void AddEllipse(RectangleF rect)
		{
			throw null;
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			throw null;
		}

		public void AddEllipse(Rectangle rect)
		{
			throw null;
		}

		public void AddEllipse(int x, int y, int width, int height)
		{
			throw null;
		}

		public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
		{
			throw null;
		}

		public void AddPolygon(PointF[] points)
		{
			throw null;
		}

		public void AddPolygon(Point[] points)
		{
			throw null;
		}

		public void AddPath(GraphicsPath addingPath, bool connect)
		{
			throw null;
		}

		public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
		{
			throw null;
		}

		public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
		{
			throw null;
		}

		public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
		{
			throw null;
		}

		public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
		{
			throw null;
		}

		public void Transform(Matrix matrix)
		{
			throw null;
		}

		public RectangleF GetBounds()
		{
			throw null;
		}

		public RectangleF GetBounds(Matrix matrix)
		{
			throw null;
		}

		public RectangleF GetBounds(Matrix matrix, Pen pen)
		{
			throw null;
		}

		public void Flatten()
		{
			throw null;
		}

		public void Flatten(Matrix matrix)
		{
			throw null;
		}

		public void Flatten(Matrix matrix, float flatness)
		{
			throw null;
		}

		public void Widen(Pen pen)
		{
			throw null;
		}

		public void Widen(Pen pen, Matrix matrix)
		{
			throw null;
		}

		public void Widen(Pen pen, Matrix matrix, float flatness)
		{
			throw null;
		}

		public void Warp(PointF[] destPoints, RectangleF srcRect)
		{
			throw null;
		}

		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
		{
			throw null;
		}

		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
		{
			throw null;
		}

		public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
		{
			throw null;
		}
	}
}
