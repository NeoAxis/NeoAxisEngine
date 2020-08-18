using System.ComponentModel;

namespace System.Windows.Forms
{
	public abstract class CommonDialog : Component
	{
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

		public event EventHandler HelpRequest
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

		public CommonDialog()
		{
			throw null;
		}

		protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			throw null;
		}

		protected virtual void OnHelpRequest(EventArgs e)
		{
			throw null;
		}

		protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			throw null;
		}

		public abstract void Reset();

		protected abstract bool RunDialog(IntPtr hwndOwner);

		public DialogResult ShowDialog()
		{
			throw null;
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			throw null;
		}
	}
}
