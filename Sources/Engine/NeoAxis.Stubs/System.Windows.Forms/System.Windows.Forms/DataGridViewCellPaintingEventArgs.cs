using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewCellPaintingEventArgs : HandledEventArgs
	{
		public DataGridViewAdvancedBorderStyle AdvancedBorderStyle
		{
			get
			{
				throw null;
			}
		}

		public Rectangle CellBounds
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellStyle CellStyle
		{
			get
			{
				throw null;
			}
		}

		public Rectangle ClipBounds
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

		public string ErrorText
		{
			get
			{
				throw null;
			}
		}

		public object FormattedValue
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

		public DataGridViewPaintParts PaintParts
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

		public object Value
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellPaintingEventArgs(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, int columnIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public void Paint(Rectangle clipBounds, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public void PaintBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
		{
			throw null;
		}

		public void PaintContent(Rectangle clipBounds)
		{
			throw null;
		}
	}
}
