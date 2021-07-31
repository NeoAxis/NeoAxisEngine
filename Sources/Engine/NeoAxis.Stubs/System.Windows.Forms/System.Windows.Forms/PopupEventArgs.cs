using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class PopupEventArgs : CancelEventArgs
	{
		public IWin32Window AssociatedWindow
		{
			get
			{
				throw null;
			}
		}

		public Control AssociatedControl
		{
			get
			{
				throw null;
			}
		}

		public bool IsBalloon
		{
			get
			{
				throw null;
			}
		}

		public Size ToolTipSize
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

		public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
		{
			throw null;
		}
	}
}
