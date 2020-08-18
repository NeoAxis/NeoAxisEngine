namespace System.Windows.Forms
{
	public class DataGridViewCellToolTipTextNeededEventArgs : DataGridViewCellEventArgs
	{
		public DataGridViewCellToolTipTextNeededEventArgs( int columnIndex, int rowIndex )
			:base(columnIndex, rowIndex)
		{
		}

		public string ToolTipText
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
	}
}
