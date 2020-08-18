using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewRowPrePaintEventArgs : HandledEventArgs
	{
		public Rectangle ClipBounds
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

		public string ErrorText
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

		public DataGridViewCellStyle InheritedRowStyle
		{
			get
			{
				throw null;
			}
		}

		public bool IsFirstDisplayedRow
		{
			get
			{
				throw null;
			}
		}

		public bool IsLastVisibleRow
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewPaintParts PaintParts
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

		public Rectangle RowBounds
		{
			get
			{
				throw null;
			}
		}

		public int RowIndex
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewElementStates State
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewRowPrePaintEventArgs(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
		{
			throw null;
		}

		public void DrawFocus(Rectangle bounds, bool cellsPaintSelectionBackground)
		{
			throw null;
		}

		public void PaintCells(Rectangle clipBounds, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public void PaintCellsBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
		{
			throw null;
		}

		public void PaintCellsContent(Rectangle clipBounds)
		{
			throw null;
		}

		public void PaintHeader(bool paintSelectionBackground)
		{
			throw null;
		}

		public void PaintHeader(DataGridViewPaintParts paintParts)
		{
			throw null;
		}
	}
}
