using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoAxis.Editor
{
	public class EngineToolStrip : ToolStrip
	{
		ToolStripItem mouseOverItem = null;
		Point mouseOverPoint;
		Timer timer;
		EngineToolTip toolTip;
		string toolTipText;
		//public bool ToolTipShowUp;

		//

		public EngineToolStrip()
			: base()
		{
			ShowItemToolTips = false;
			timer = new Timer();
			timer.Enabled = false;
			timer.Interval = SystemInformation.MouseHoverTime;
			timer.Tick += new EventHandler( timer_Tick );
			toolTip = new EngineToolTip();
		}

		protected override void OnMouseMove( MouseEventArgs mea )
		{
			base.OnMouseMove( mea );
			ToolStripItem newMouseOverItem = this.GetItemAt( mea.Location );
			if( mouseOverItem != newMouseOverItem ||
				( Math.Abs( mouseOverPoint.X - mea.X ) > SystemInformation.MouseHoverSize.Width || ( Math.Abs( mouseOverPoint.Y - mea.Y ) > SystemInformation.MouseHoverSize.Height ) ) )
			{
				mouseOverItem = newMouseOverItem;
				mouseOverPoint = mea.Location;
				if( toolTip != null )
					toolTip.Hide( this );
				timer.Stop();
				timer.Start();
			}
		}

		protected override void OnMouseClick( MouseEventArgs e )
		{
			base.OnMouseClick( e );
			ToolStripItem newMouseOverItem = this.GetItemAt( e.Location );
			if( newMouseOverItem != null && toolTip != null )
				toolTip.Hide( this );
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
			timer.Stop();
			if( toolTip != null )
				toolTip.Hide( this );
			mouseOverPoint = new Point( -50, -50 );
			mouseOverItem = null;
		}

		void timer_Tick( object sender, EventArgs e )
		{
			timer.Stop();
			try
			{
				Point currentMouseOverPoint;
				//if( ToolTipShowUp )
				//	currentMouseOverPoint = this.PointToClient( new Point( Control.MousePosition.X, Control.MousePosition.Y - Cursor.Current.Size.Height + Cursor.Current.HotSpot.Y ) );
				//else
				currentMouseOverPoint = this.PointToClient( new Point( Control.MousePosition.X, Control.MousePosition.Y + Cursor.Current.Size.Height - Cursor.Current.HotSpot.Y ) );

				if( mouseOverItem == null )
				{
					if( toolTipText != null && toolTipText.Length > 0 )
						toolTip.SetToolTip( this, toolTipText );
				}
				else if( ( !( mouseOverItem is ToolStripDropDownButton ) && !( mouseOverItem is ToolStripSplitButton ) ) ||
					( ( mouseOverItem is ToolStripDropDownButton ) && !( (ToolStripDropDownButton)mouseOverItem ).DropDown.Visible ) ||
					( ( ( mouseOverItem is ToolStripSplitButton ) && !( (ToolStripSplitButton)mouseOverItem ).DropDown.Visible ) ) )
				{
					if( mouseOverItem.ToolTipText != null && mouseOverItem.ToolTipText.Length > 0 && toolTip != null )
						toolTip.SetToolTip( this, mouseOverItem.ToolTipText );
				}
			}
			catch { }
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
			if( disposing )
			{
				timer.Dispose();
				toolTip.Dispose();
			}
		}
	}
}
