namespace System.Drawing
{
	public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
	{
		public abstract object Clone();

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		~Brush()
		{
			throw null;
		}

		protected Brush()
		{
			throw null;
		}
	}
}
