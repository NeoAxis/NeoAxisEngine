namespace System.Windows.Forms
{
	public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
	{
		public DataGridViewDataErrorContexts Context
		{
			get
			{
				throw null;
			}
		}

		public Exception Exception
		{
			get
			{
				throw null;
			}
		}

		public bool ThrowException
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

		public DataGridViewDataErrorEventArgs(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context)
			:base(columnIndex, rowIndex)
		{
			throw null;
		}
	}
}
