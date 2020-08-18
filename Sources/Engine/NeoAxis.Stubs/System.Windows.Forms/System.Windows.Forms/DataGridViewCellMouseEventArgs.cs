namespace System.Windows.Forms
{
	public class DataGridViewCellMouseEventArgs : MouseEventArgs
	{
		public int ColumnIndex
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

		public DataGridViewCellMouseEventArgs(int columnIndex, int rowIndex, int localX, int localY, MouseEventArgs e)
			:base(MouseButtons.Left, 0,0,0,0)
		{
			throw null;
		}
	}
}
