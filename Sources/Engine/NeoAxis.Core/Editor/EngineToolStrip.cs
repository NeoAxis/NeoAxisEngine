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
		public ToolTip Tooltip;
		public int ToolTipInterval = 4000;
		public string ToolTipText;
		public bool ToolTipShowUp;

		//

		public EngineToolStrip()
			: base()
		{
			ShowItemToolTips = false;
			timer = new Timer();
			timer.Enabled = false;
			timer.Interval = SystemInformation.MouseHoverTime;
			timer.Tick += new EventHandler( timer_Tick );
			Tooltip = new ToolTip();
			EditorThemeUtility.ApplyDarkThemeToToolTip( Tooltip );
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
				if( Tooltip != null )
					Tooltip.Hide( this );
				timer.Stop();
				timer.Start();
			}
		}

		protected override void OnMouseClick( MouseEventArgs e )
		{
			base.OnMouseClick( e );
			ToolStripItem newMouseOverItem = this.GetItemAt( e.Location );
			if( newMouseOverItem != null && Tooltip != null )
			{
				Tooltip.Hide( this );
			}
		}

		protected override void OnMouseUp( MouseEventArgs mea )
		{
			base.OnMouseUp( mea );
			ToolStripItem newMouseOverItem = this.GetItemAt( mea.Location );
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
			timer.Stop();
			if( Tooltip != null )
				Tooltip.Hide( this );
			mouseOverPoint = new Point( -50, -50 );
			mouseOverItem = null;
		}

		void timer_Tick( object sender, EventArgs e )
		{
			timer.Stop();
			try
			{
				Point currentMouseOverPoint;
				if( ToolTipShowUp )
					currentMouseOverPoint = this.PointToClient( new Point( Control.MousePosition.X, Control.MousePosition.Y - Cursor.Current.Size.Height + Cursor.Current.HotSpot.Y ) );
				else
					currentMouseOverPoint = this.PointToClient( new Point( Control.MousePosition.X, Control.MousePosition.Y + Cursor.Current.Size.Height - Cursor.Current.HotSpot.Y ) );

				if( mouseOverItem == null )
				{
					if( ToolTipText != null && ToolTipText.Length > 0 )
					{
						if( Tooltip == null )
						{
							Tooltip = new ToolTip();
							EditorThemeUtility.ApplyDarkThemeToToolTip( Tooltip );
						}
						Tooltip.Show( ToolTipText, this, currentMouseOverPoint, ToolTipInterval );
					}
				}
				else if( ( !( mouseOverItem is ToolStripDropDownButton ) && !( mouseOverItem is ToolStripSplitButton ) ) ||
					( ( mouseOverItem is ToolStripDropDownButton ) && !( (ToolStripDropDownButton)mouseOverItem ).DropDown.Visible ) ||
					( ( ( mouseOverItem is ToolStripSplitButton ) && !( (ToolStripSplitButton)mouseOverItem ).DropDown.Visible ) ) )
				{
					if( mouseOverItem.ToolTipText != null && mouseOverItem.ToolTipText.Length > 0 && Tooltip != null )
					{
						if( Tooltip == null )
						{
							Tooltip = new ToolTip();
							EditorThemeUtility.ApplyDarkThemeToToolTip( Tooltip );
						}
						Tooltip.Show( mouseOverItem.ToolTipText, this, currentMouseOverPoint, ToolTipInterval );
					}
				}
			}
			catch
			{ }
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
			if( disposing )
			{
				timer.Dispose();
				Tooltip.Dispose();
			}
		}
	}
}
