using System.Drawing;

namespace System.Windows.Forms
{
	public class DrawItemEventArgs : EventArgs
	{
		public Color BackColor
		{
			get
			{
				throw null;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public Font Font
		{
			get
			{
				throw null;
			}
		}

		public Color ForeColor
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

		public int Index
		{
			get
			{
				throw null;
			}
		}

		public DrawItemState State
		{
			get
			{
				throw null;
			}
		}

		public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state)
		{
			throw null;
		}

		public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
		{
			throw null;
		}

		public virtual void DrawBackground()
		{
			throw null;
		}

		public virtual void DrawFocusRectangle()
		{
			throw null;
		}
	}
}
