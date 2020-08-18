namespace System.Windows.Forms
{
	public class DataGridViewCellParsingEventArgs : ConvertEventArgs
	{
		public int RowIndex
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

		public DataGridViewCellStyle InheritedCellStyle
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

		public bool ParsingApplied
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

		public DataGridViewCellParsingEventArgs(int rowIndex, int columnIndex, object value, Type desiredType, DataGridViewCellStyle inheritedCellStyle)
			:base(value, desiredType)
		{
			throw null;
		}
	}
}
