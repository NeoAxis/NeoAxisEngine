using System.ComponentModel;

namespace System.Windows.Forms
{
	public class TabControlCancelEventArgs : CancelEventArgs
	{
		public TabPage TabPage
		{
			get
			{
				throw null;
			}
		}

		public int TabPageIndex
		{
			get
			{
				throw null;
			}
		}

		public TabControlAction Action
		{
			get
			{
				throw null;
			}
		}

		public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action)
		{
			throw null;
		}
	}
}
