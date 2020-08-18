using System.Drawing;

namespace System.Windows.Forms
{
	public class DrawListViewSubItemEventArgs : EventArgs
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

		public Rectangle Bounds
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

		public ListViewItem.ListViewSubItem SubItem
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

		public int ColumnIndex
		{
			get
			{
				throw null;
			}
		}

		public ColumnHeader Header
		{
			get
			{
				throw null;
			}
		}

		public ListViewItemStates ItemState
		{
			get
			{
				throw null;
			}
		}

		public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
		{
			throw null;
		}

		public void DrawBackground()
		{
			throw null;
		}

		public void DrawFocusRectangle(Rectangle bounds)
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
