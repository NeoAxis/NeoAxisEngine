using System.ComponentModel;

namespace System.Windows.Forms
{
	public class FormClosingEventArgs : CancelEventArgs
	{
		public CloseReason CloseReason
		{
			get
			{
				throw null;
			}
		}

		public FormClosingEventArgs(CloseReason closeReason, bool cancel)
		{
			throw null;
		}
	}
}
