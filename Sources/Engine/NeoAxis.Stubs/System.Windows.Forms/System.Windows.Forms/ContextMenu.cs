using System.Drawing;

namespace System.Windows.Forms
{
	public class ContextMenu : Menu
	{
		public Control SourceControl
		{
			get
			{
				throw null;
			}
		}

		public virtual RightToLeft RightToLeft
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

		public event EventHandler Popup
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

		public event EventHandler Collapse
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

		public ContextMenu()
			:base(null)
		{
			throw null;
		}

		public ContextMenu(MenuItem[] menuItems)
			:base(menuItems)
		{
			throw null;
		}

		public void Show(Control control, Point pos)
		{
			throw null;
		}

		public void Show(Control control, Point pos, LeftRightAlignment alignment)
		{
			throw null;
		}
	}
}
