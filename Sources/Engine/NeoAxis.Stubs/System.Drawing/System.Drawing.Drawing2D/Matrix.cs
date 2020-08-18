namespace System.Drawing.Drawing2D
{
	public sealed class Matrix : MarshalByRefObject, IDisposable
	{
		public float[] Elements
		{
			get
			{
				throw null;
			}
		}

		public float OffsetX
		{
			get
			{
				throw null;
			}
		}

		public float OffsetY
		{
			get
			{
				throw null;
			}
		}

		public bool IsInvertible
		{
			get
			{
				throw null;
			}
		}

		public bool IsIdentity
		{
			get
			{
				throw null;
			}
		}

		public Matrix()
		{
			throw null;
		}

		public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
		{
			throw null;
		}

		public Matrix(RectangleF rect, PointF[] plgpts)
		{
			throw null;
		}

		public Matrix(Rectangle rect, Point[] plgpts)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~Matrix()
		{
			throw null;
		}

		public Matrix Clone()
		{
			throw null;
		}

		public void Reset()
		{
			throw null;
		}

		public void Multiply(Matrix matrix)
		{
			throw null;
		}

		public void Multiply(Matrix matrix, MatrixOrder order)
		{
			throw null;
		}

		public void Translate(float offsetX, float offsetY)
		{
			throw null;
		}

		public void Translate(float offsetX, float offsetY, MatrixOrder order)
		{
			throw null;
		}

		public void Scale(float scaleX, float scaleY)
		{
			throw null;
		}

		public void Scale(float scaleX, float scaleY, MatrixOrder order)
		{
			throw null;
		}

		public void Rotate(float angle)
		{
			throw null;
		}

		public void Rotate(float angle, MatrixOrder order)
		{
			throw null;
		}

		public void RotateAt(float angle, PointF point)
		{
			throw null;
		}

		public void RotateAt(float angle, PointF point, MatrixOrder order)
		{
			throw null;
		}

		public void Shear(float shearX, float shearY)
		{
			throw null;
		}

		public void Shear(float shearX, float shearY, MatrixOrder order)
		{
			throw null;
		}

		public void Invert()
		{
			throw null;
		}

		public void TransformPoints(PointF[] pts)
		{
			throw null;
		}

		public void TransformPoints(Point[] pts)
		{
			throw null;
		}

		public void TransformVectors(PointF[] pts)
		{
			throw null;
		}

		public void VectorTransformPoints(Point[] pts)
		{
			throw null;
		}

		public void TransformVectors(Point[] pts)
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}
	}
}
