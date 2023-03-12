#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace NeoAxis.Editor
{
	public static class EngineToolTipManager
	{
		const double initialDelay = 0.5;

		static Control currentControl;
		static string currentControlText;
		static double currentControlStartTime;
		static Point currentCursorPosition;
		static EngineToolTipForm currentControlForm;

		///////////////////////////////////////////////

		[System.Runtime.InteropServices.DllImport( "user32.dll" )]
		internal/*obfuscator*/ static extern IntPtr WindowFromPoint( Point pnt );

		///////////////////////////////////////////////

		static Control GetControlOverCursor()
		{
			IntPtr hWnd = WindowFromPoint( Control.MousePosition );
			if( hWnd != IntPtr.Zero )
				return Control.FromHandle( hWnd );
			return null;
		}

		public static void Update()
		{
			EngineToolTip.UpdateAllInstances();

			var control = GetControlOverCursor();

			if( currentControl != control )
			{
				//end old
				currentControl = null;
				currentControlForm?.Close();
				currentControlForm = null;

				//start new
				if( control != null )
				{
					(EngineToolTip toolTip, string text) tuple = EngineToolTip.GetToolTipByControl( control );
					if( tuple.toolTip != null && !string.IsNullOrEmpty( tuple.text ) )
					{
						currentControl = control;
						currentControlText = tuple.text;
						currentControlStartTime = EngineApp.GetSystemTime();
						currentCursorPosition = Control.MousePosition;
					}
				}
			}

			//show form
			if( currentControl != null && currentControlForm == null )
			{
				//reset counter when mouse moved
				if( currentCursorPosition != Control.MousePosition )
				{
					currentCursorPosition = Control.MousePosition;
					currentControlStartTime = EngineApp.GetSystemTime();
				}

				//show form
				if( EngineApp.GetSystemTime() > currentControlStartTime + initialDelay )
				{
					currentControlForm = new EngineToolTipForm( currentControlText );
					currentControlForm.StartPosition = FormStartPosition.Manual;
					var mouse = Control.MousePosition;
					currentControlForm.Location = new Point( mouse.X, mouse.Y + (int)( 16.0f * EditorAPI.DPIScale ) );
					currentControlForm.Show();
				}
			}
		}

		internal static void Hide( Control control )
		{
			if( currentControl == control )
			{
				currentControl = null;
				try
				{
					currentControlForm?.Close();
				}
				catch { }
				currentControlForm = null;
			}
		}
	}
}

#endif