// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Runtime.InteropServices;
using System.Text;
using Internal;

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb : PlatformFunctionality
	{
		const string CANVAS_PATH = "canvas";

		static PlatformFunctionalityWeb instance;
		public static Vector2I screenSize;

		nint canvasPathHandle;

		/////////////////////////////////////////

		public PlatformFunctionalityWeb()
		{
			instance = this;
			SetInstance( instance, SystemSettings.Platform.Web );

			new LogPlatformFunctionalityWeb();
			new PlatformSpecificUtilityWeb();
		}

		public override Vector2I GetScreenSize()
		{
			if( screenSize != Vector2I.Zero )
				return screenSize;
			else
				return new Vector2I( 100, 100 );
		}

		public override int GetScreenBitsPerPixel()
		{
			return 32;
		}

		public override Vector2I GetSmallIconSize()
		{
			return new Vector2I( 16, 16 );
		}

		public override void Init( IntPtr mainModuleData )
		{
			base.Init( mainModuleData );
		}

		public override IntPtr CreatedWindow_CreateWindow()
		{
			unsafe
			{
				if( canvasPathHandle == 0 )
				{
					canvasPathHandle = Marshal.AllocHGlobal( CANVAS_PATH.Length + 1 );
					var span = new Span<byte>( (void*)canvasPathHandle, CANVAS_PATH.Length + 1 );
					Encoding.ASCII.GetBytes( CANVAS_PATH, span );
					span[ CANVAS_PATH.Length ] = 0;
				}
				return canvasPathHandle;
			}
		}

		public override void CreatedWindow_DestroyWindow()
		{
			Marshal.FreeHGlobal( canvasPathHandle );
			canvasPathHandle = 0;
		}

		public override void CreatedWindow_ProcessMessageEvents()
		{
		}

		public override void MessageLoopWaitMessage()
		{
		}

		public override bool IsIntoMenuLoop()
		{
			return false;
		}

		public override void CreatedWindow_RunMessageLoop()
		{
		}

		public override bool IsWindowInitialized()
		{
			//!!!!
			return true;
		}

		public override void CreatedWindow_UpdateWindowTitle( string title )
		{
			//!!!!
		}

		//public override void CreatedWindow_UpdateWindowIcon( System.Drawing.Icon smallIcon, System.Drawing.Icon icon )
		//{
		//}

		public override RectangleI CreatedWindow_GetWindowRectangle()
		{
			//!!!!
			return CreatedWindow_GetClientRectangle();
		}

		public override void CreatedWindow_SetWindowRectangle( RectangleI rectangle )
		{
			//!!!!

			//!!!!
			//EngineApp._CreatedWindow_ProcessResize();
		}

		public override RectangleI CreatedWindow_GetClientRectangle()
		{
			var size = GetScreenSize();
			return new RectangleI( 0, 0, size.X, size.Y );
		}

		public override void CreatedWindow_SetWindowSize( Vector2I size )
		{
			//!!!!
		}

		public override bool ApplicationIsActive()
		{
			return CreatedWindow_IsWindowActive();
		}

		public override bool CreatedWindow_IsWindowActive()
		{
			//!!!!
			return true;
		}

		public override bool IsWindowVisible()
		{
			//!!!!
			return true;
		}

		public override WindowState GetWindowState()
		{
			//!!!!
			return WindowState.Normal;
		}

		public override void SetWindowState( WindowState value )
		{
			//!!!!
		}

		public override void SetWindowVisible( bool value )
		{
			//!!!!
		}

		public override bool IsFocused()
		{
			//!!!!
			return true;
		}

		public override InputDeviceManager CreateInputDeviceManager()
		{
			return null;
		}
	}
}
