using System.Drawing;

namespace System.Windows.Forms
{
	public class PaintEventArgs : EventArgs, IDisposable
	{
		public Rectangle ClipRectangle
		{
			get
			{
				throw null;
			}
		}

		public Graphics Graphics
		{
			get
			{
				throw null;
			}
		}

		public PaintEventArgs(Graphics graphics, Rectangle clipRect)
		{
			throw null;
		}

		~PaintEventArgs()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}
	}
}
