using System.Drawing;

namespace System.Windows.Forms
{
	public class DrawListViewItemEventArgs : EventArgs
	{
		public bool DrawDefault
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

		public Graphics Graphics
		{
			get
			{
				throw null;
			}
		}

		public ListViewItem Item
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

		public int ItemIndex
		{
			get
			{
				throw null;
			}
		}

		public ListViewItemStates State
		{
			get
			{
				throw null;
			}
		}

		public DrawListViewItemEventArgs(Graphics graphics, ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
		{
			throw null;
		}

		public void DrawBackground()
		{
			throw null;
		}

		public void DrawFocusRectangle()
		{
			throw null;
		}

		public void DrawText()
		{
			throw null;
		}

		public void DrawText(TextFormatFlags flags)
		{
			throw null;
		}
	}
}
