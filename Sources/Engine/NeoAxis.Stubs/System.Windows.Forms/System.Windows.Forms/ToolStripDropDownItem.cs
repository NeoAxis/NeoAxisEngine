using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class ToolStripDropDownItem : ToolStripItem
	{
		public ToolStripDropDown DropDown
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

		public ToolStripDropDownDirection DropDownDirection
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

		public ToolStripItemCollection DropDownItems
		{
			get
			{
				throw null;
			}
		}

		public virtual bool HasDropDownItems
		{
			get
			{
				throw null;
			}
		}

		public bool HasDropDown
		{
			get
			{
				throw null;
			}
		}

		public override bool Pressed
		{
			get
			{
				throw null;
			}
		}

		public event EventHandler DropDownClosed
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

		public event EventHandler DropDownOpening
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

		public event EventHandler DropDownOpened
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

		public event ToolStripItemClickedEventHandler DropDownItemClicked
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

		protected ToolStripDropDownItem()
		{
			throw null;
		}

		protected ToolStripDropDownItem(string text, Image image, EventHandler onClick)
		{
			throw null;
		}

		protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name)
		{
			throw null;
		}

		protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected virtual ToolStripDropDown CreateDefaultDropDown()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public void HideDropDown()
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBoundsChanged()
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDropDownHide(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDropDownShow(EventArgs e)
		{
			throw null;
		}

		public void ShowDropDown()
		{
			throw null;
		}
	}
}
