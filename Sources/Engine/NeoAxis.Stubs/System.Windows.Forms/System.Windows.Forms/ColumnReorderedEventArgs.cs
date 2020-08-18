using System.ComponentModel;

namespace System.Windows.Forms
{
	public class ColumnReorderedEventArgs : CancelEventArgs
	{
		public int OldDisplayIndex
		{
			get
			{
				throw null;
			}
		}

		public int NewDisplayIndex
		{
			get
			{
				throw null;
			}
		}

		public ColumnHeader Header
		{
			get
			{
				throw null;
			}
		}

		public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header)
		{
			throw null;
		}
	}
}
