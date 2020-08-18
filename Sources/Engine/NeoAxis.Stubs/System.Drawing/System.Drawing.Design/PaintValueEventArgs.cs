using System.ComponentModel;

namespace System.Drawing.Design
{
	public class PaintValueEventArgs : EventArgs
	{
		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public ITypeDescriptorContext Context
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

		public object Value
		{
			get
			{
				throw null;
			}
		}

		public PaintValueEventArgs(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
		{
			throw null;
		}
	}
}
