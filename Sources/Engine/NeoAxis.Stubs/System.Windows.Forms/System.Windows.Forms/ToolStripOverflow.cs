using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class ToolStripOverflow : ToolStripDropDown, IComponent, IDisposable
	{
		public override ToolStripItemCollection Items
		{
			get
			{
				throw null;
			}
		}

		public override LayoutEngine LayoutEngine
		{
			get
			{
				throw null;
			}
		}

		public ToolStripOverflow(ToolStripItem parentItem)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected override void SetDisplayedItems()
		{
			throw null;
		}
	}
}
