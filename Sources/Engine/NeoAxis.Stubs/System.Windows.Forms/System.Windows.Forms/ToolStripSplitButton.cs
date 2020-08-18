using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolStripSplitButton : ToolStripDropDownItem
	{
		public class ToolStripSplitButtonAccessibleObject : ToolStripItemAccessibleObject
		{
			public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item)
				:base(item)
			{
				throw null;
			}

			public override void DoDefaultAction()
			{
				throw null;
			}
		}

		public new bool AutoToolTip
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

		public Rectangle ButtonBounds
		{
			get
			{
				throw null;
			}
		}

		public bool ButtonPressed
		{
			get
			{
				throw null;
			}
		}

		public bool ButtonSelected
		{
			get
			{
				throw null;
			}
		}

		protected override bool DefaultAutoToolTip
		{
			get
			{
				throw null;
			}
		}

		public ToolStripItem DefaultItem
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

		public Rectangle DropDownButtonBounds
		{
			get
			{
				throw null;
			}
		}

		public bool DropDownButtonPressed
		{
			get
			{
				throw null;
			}
		}

		public bool DropDownButtonSelected
		{
			get
			{
				throw null;
			}
		}

		public int DropDownButtonWidth
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

		public Rectangle SplitterBounds
		{
			get
			{
				throw null;
			}
		}

		public event EventHandler ButtonClick
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

		public event EventHandler ButtonDoubleClick
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

		public event EventHandler DefaultItemChanged
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

		public ToolStripSplitButton()
		{
			throw null;
		}

		public ToolStripSplitButton(string text)
		{
			throw null;
		}

		public ToolStripSplitButton(Image image)
		{
			throw null;
		}

		public ToolStripSplitButton(string text, Image image)
		{
			throw null;
		}

		public ToolStripSplitButton(string text, Image image, EventHandler onClick)
		{
			throw null;
		}

		public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name)
		{
			throw null;
		}

		public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override ToolStripDropDown CreateDefaultDropDown()
		{
			throw null;
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			throw null;
		}

		protected virtual void OnButtonClick(EventArgs e)
		{
			throw null;
		}

		public virtual void OnButtonDoubleClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDefaultItemChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		public void PerformButtonClick()
		{
			throw null;
		}

		public virtual void ResetDropDownButtonWidth()
		{
			throw null;
		}
	}
}
