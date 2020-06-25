using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	//!!!!name
	static class ExtensionMethods
	{
		public static DockWindow GetDockWindow( this KryptonPage page )
		{
			Debug.Assert( page.Controls.Count <= 1 );
			if( page.Controls.Count == 0 )
				return null;
			return (DockWindow)page.Controls[ 0 ];
		}

		public static bool HasDockWindow( this KryptonPage page )
		{
			Debug.Assert( page.Controls.Count <= 1 );
			return page.Controls.Count != 0;
		}

		//public static DockWindow GetParentDockWindow( this Control control )
		//{
		//	var parent = control.Parent;

		//	while( parent != null )
		//	{
		//		if( parent is DockWindow )
		//			return (DockWindow)parent;
		//		parent = parent.Parent;
		//	}

		//	return null;
		//}

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool GetWindowRect( IntPtr hWnd, out RectangleI rect );

		public static bool IsPhysicalVisibleCheckBy5Points( this Control control )
		{
			bool CheckControlFromPoint( int x, int y )
			{
				var other = Control.FromChildHandle( PI.WindowFromPoint( new PI.POINT( x, y ) ) );
				if( other == null )
					return false;
				if( control == other || control.Contains( other ) )
					return true;
				return false;
			}

			if( !GetWindowRect( control.Handle, out var rect ) )
				return true;

			if( CheckControlFromPoint( rect.Left + 2, rect.Top + 2 ) )
				return true;
			if( CheckControlFromPoint( rect.Right - 2, rect.Top + 2 ) )
				return true;
			if( CheckControlFromPoint( rect.Left + 2, rect.Bottom - 2 ) )
				return true;
			if( CheckControlFromPoint( rect.Right - 2, rect.Bottom - 2 ) )
				return true;
			if( CheckControlFromPoint( rect.GetCenter().X, rect.GetCenter().Y ) )
				return true;

			//var pos = control.PointToScreen( control.Location );

			//if( CheckControlFromPoint( pos.X, pos.Y ) )
			//	return true;
			//if( CheckControlFromPoint( pos.X + control.Width - 1, pos.Y ) )
			//	return true;
			//if( CheckControlFromPoint( pos.X, pos.Y + control.Height - 1 ) )
			//	return true;
			//if( CheckControlFromPoint( pos.X + control.Width - 1, pos.Y + control.Height - 1 ) )
			//	return true;
			//if( CheckControlFromPoint( pos.X + control.Width / 2, pos.Y + control.Height / 2 ) )
			//	return true;

			return false;
		}

		internal static int GetTotalControlsCount( this Control control )
		{
			int count = 0;
			foreach( Control child in control.Controls )
				count += 1 + child.GetTotalControlsCount();
			return count;
		}
	}
}
