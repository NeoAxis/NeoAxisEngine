using System.ComponentModel;

namespace System.Windows.Forms
{
	public class ColumnWidthChangingEventArgs : CancelEventArgs
	{
		public int ColumnIndex
		{
			get
			{
				throw null;
			}
		}

		public int NewWidth
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

		public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel)
		{
			throw null;
		}

		public ColumnWidthChangingEventArgs(int columnIndex, int newWidth)
		{
			throw null;
		}
	}
}
