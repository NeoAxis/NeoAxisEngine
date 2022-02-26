using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Internal.ComponentFactory.Krypton.Ribbon
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem(true)]
	public class BackstageAppMenu : UserControl
	{
		private KryptonRibbon _ribbon;

		public KryptonRibbon Ribbon
		{
			get { return _ribbon; }
			set
			{
				_ribbon = value;
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			if (Parent != null)
				Parent.SizeChanged += Parent_SizeChanged;
		}

		private void Parent_SizeChanged(object sender, EventArgs e)
		{
			if (Parent != null && Visible)
				Size = Parent.Size;
		}

        public ToolStripDropDownCloseReason? CloseReason 
        {
            get { return ToolStripDropDownCloseReason.AppClicked; }
        }

        protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (DesignMode)
				return;

			if (!Visible)
			{
				if (_ribbon != null)
					_ribbon.OnAppButtonMenuClosing(new CancelEventArgs());
			}
			else
			{
				Location = new Point(0, 0);
				Size = Parent.Size;
			}
		}
	}
}
