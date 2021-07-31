using System.Drawing.Drawing2D;

namespace System.Drawing
{
	public sealed class Pen : MarshalByRefObject, ICloneable, IDisposable
	{
		public float Width
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

		public LineCap StartCap
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

		public LineCap EndCap
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

		public DashCap DashCap
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

		public LineJoin LineJoin
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

		public CustomLineCap CustomStartCap
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

		public CustomLineCap CustomEndCap
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

		public float MiterLimit
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

		public PenAlignment Alignment
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

		public PenType PenType
		{
			get
			{
				throw null;
			}
		}

		public Color Color
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

		public Brush Brush
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

		public DashStyle DashStyle
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

		public float DashOffset
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

		public float[] DashPattern
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

		public float[] CompoundArray
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

		public Pen(Color color)
		{
			throw null;
		}

		public Pen(Color color, float width)
		{
			throw null;
		}

		public Pen(Brush brush)
		{
			throw null;
		}

		public Pen(Brush brush, float width)
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

		~Pen()
		{
			throw null;
		}

		public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
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
	}
}
