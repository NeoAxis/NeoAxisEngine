using System.Drawing;

namespace System.Windows.Forms
{
	public class TableLayoutCellPaintEventArgs : PaintEventArgs
	{
		public Rectangle CellBounds
		{
			get
			{
				throw null;
			}
		}

		public int Row
		{
			get
			{
				throw null;
			}
		}

		public int Column
		{
			get
			{
				throw null;
			}
		}

		public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row)
			:base(g, clipRectangle)
		{
			throw null;
		}
	}
}
