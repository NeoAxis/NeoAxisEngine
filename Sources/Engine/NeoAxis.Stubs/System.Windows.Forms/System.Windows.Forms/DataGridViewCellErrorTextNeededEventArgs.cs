namespace System.Windows.Forms
{
	public class DataGridViewCellErrorTextNeededEventArgs : DataGridViewCellEventArgs
	{
		public DataGridViewCellErrorTextNeededEventArgs( int columnIndex, int rowIndex )
			:base(columnIndex, rowIndex)
		{
		}

		public string ErrorText
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
