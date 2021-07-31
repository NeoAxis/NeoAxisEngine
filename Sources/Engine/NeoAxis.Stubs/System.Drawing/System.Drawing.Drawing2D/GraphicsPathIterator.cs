namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
	{
		public int Count
		{
			get
			{
				throw null;
			}
		}

		public int SubpathCount
		{
			get
			{
				throw null;
			}
		}

		public GraphicsPathIterator(GraphicsPath path)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~GraphicsPathIterator()
		{
			throw null;
		}

		public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
		{
			throw null;
		}

		public int NextSubpath(GraphicsPath path, out bool isClosed)
		{
			throw null;
		}

		public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
		{
			throw null;
		}

		public int NextMarker(out int startIndex, out int endIndex)
		{
			throw null;
		}

		public int NextMarker(GraphicsPath path)
		{
			throw null;
		}

		public bool HasCurve()
		{
			throw null;
		}

		public void Rewind()
		{
			throw null;
		}

		public int Enumerate(PointF points, ref byte[] types)
		{
			throw null;
		}

		public int CopyData(PointF points, ref byte[] types, int startIndex, int endIndex)
		{
			throw null;
		}
	}
}
