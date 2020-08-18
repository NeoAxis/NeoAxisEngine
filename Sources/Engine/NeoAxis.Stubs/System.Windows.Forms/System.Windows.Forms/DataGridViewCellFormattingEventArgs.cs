namespace System.Windows.Forms
{
	public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
	{
		public DataGridViewCellStyle CellStyle
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

		public int ColumnIndex
		{
			get
			{
				throw null;
			}
		}

		public bool FormattingApplied
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

		public int RowIndex
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellFormattingEventArgs(int columnIndex, int rowIndex, object value, Type desiredType, DataGridViewCellStyle cellStyle)
			:base(value, desiredType)
		{
			throw null;
		}
	}
}
