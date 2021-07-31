using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolTip : Component, IExtenderProvider
	{
		public bool Active
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

		public int AutomaticDelay
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

		public int AutoPopDelay
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

		public Color BackColor
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

		protected virtual CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		public Color ForeColor
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

		public bool IsBalloon
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

		public int InitialDelay
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

		public bool OwnerDraw
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

		public int ReshowDelay
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

		public bool ShowAlways
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

		public bool StripAmpersands
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

		public object Tag
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

		public ToolTipIcon ToolTipIcon
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

		public string ToolTipTitle
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

		public bool UseAnimation
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

		public bool UseFading
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

		public event DrawToolTipEventHandler Draw
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event PopupEventHandler Popup
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public ToolTip(IContainer cont)
		{
			throw null;
		}

		public ToolTip()
		{
			throw null;
		}

		public bool CanExtend(object target)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public string GetToolTip(Control control)
		{
			throw null;
		}

		public void RemoveAll()
		{
			throw null;
		}

		public void SetToolTip(Control control, string caption)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window, int duration)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window, Point point)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window, Point point, int duration)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window, int x, int y)
		{
			throw null;
		}

		public void Show(string text, IWin32Window window, int x, int y, int duration)
		{
			throw null;
		}

		public void Hide(IWin32Window win)
		{
			throw null;
		}

		protected void StopTimer()
		{
			throw null;
		}

		~ToolTip()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
