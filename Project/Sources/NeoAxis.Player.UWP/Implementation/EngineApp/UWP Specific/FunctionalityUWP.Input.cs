// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.IO;
using DirectInput;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.System;
using Internal;

namespace NeoAxis
{
	partial class PlatformFunctionalityUWP : PlatformFunctionality
	{
		KeyInfo[] keysInfo = new KeyInfo[ EngineApp.GetEKeysMaxIndex() + 1 ];

		Vector2 lastMousePositionForMouseMoveDelta;
		Vector2 lastMousePositionForCheckMouseOutsideWindow;
		bool mustIgnoreOneMouseMoveAtRelativeMode;

		bool createdWindow_UpdateShowSystemCursor;

		/////////////////////////////////////////

		struct KeyInfo
		{
			public int keyCode;

			public KeyInfo( int keyCode )
			{
				this.keyCode = keyCode;
			}
		}

		static bool GetEKeyByVirtualKey( VirtualKey keyCode, out EKeys eKey )
		{
			if( Enum.IsDefined( typeof( EKeys ), (int)keyCode ) )
			{
				eKey = (EKeys)(int)keyCode;
				return true;
			}
			else
			{
				eKey = EKeys.Cancel;
				return false;
			}
		}


		public override Vector2 CreatedWindow_GetMousePosition()
		{
			var scaleFactor = displayInfo.RawPixelsPerViewPixel;

			var pointerPosition = coreWindow.PointerPosition;
			var x = ( pointerPosition.X - coreWindow.Bounds.X ) * scaleFactor;
			var y = ( pointerPosition.Y - coreWindow.Bounds.Y ) * scaleFactor;

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			return new Vector2F(
				(float)x / ( viewport.SizeInPixels.X - viewport.SizeInPixels.X % 2 ),
				(float)y / ( viewport.SizeInPixels.Y - viewport.SizeInPixels.Y % 2 ) );
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
			var scaleFactor = displayInfo.RawPixelsPerViewPixel;

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			Vector2I position = new Vector2I(
				(int)(float)( value.X * (float)viewport.SizeInPixels.X ),
				(int)(float)( value.Y * (float)viewport.SizeInPixels.Y ) );

			coreWindow.PointerPosition = new Windows.Foundation.Point(
				( position.X / scaleFactor ) + coreWindow.Bounds.X,
				( position.Y / scaleFactor ) + coreWindow.Bounds.Y );

			lastMousePositionForMouseMoveDelta = value;
			lastMousePositionForCheckMouseOutsideWindow = value;
		}

		//internal void SetCursor( bool visible )
		//{
		//	if( coreWindow == null )
		//		return;

		//	var asyncResult = coreWindow.Dispatcher.RunIdleAsync( ( e ) =>
		//	{
		//		if( visible )
		//			coreWindow.PointerCursor = new CoreCursor( CoreCursorType.Arrow, 0 );
		//		else
		//			coreWindow.PointerCursor = null;
		//	} );
		//}

		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
		{
			bool show = EngineApp.IsRealShowSystemCursor();

			if( createdWindow_UpdateShowSystemCursor != show || forceUpdate )
			{
				createdWindow_UpdateShowSystemCursor = show;

				if( coreWindow != null )
				{
					if( show )
						coreWindow.PointerCursor = new CoreCursor( CoreCursorType.Arrow, 0 );
					else
						coreWindow.PointerCursor = null;
				}
			}

			//SetCursor( show );
			//coreWindow.PointerCursor = new CoreCursor( CoreCursorType.Arrow, 0 );

			// from SDL:

			// HACK ALERT: TL;DR - Hiding the cursor in WinRT/UWP apps is weird, and
			//   a Win32-style cursor resource file must be directly included in apps,
			//   otherwise hiding the cursor will cause mouse-motion data to never be
			//   received.
			//
			// Here's the lengthy explanation:
			//
			// There are two ways to hide a cursor in WinRT/UWP apps.
			// Both involve setting the WinRT CoreWindow's (which is somewhat analogous
			// to a Win32 HWND) 'PointerCursor' property.
			//
			// The first way to hide a cursor sets PointerCursor to nullptr.  This
			// is, arguably, the easiest to implement for an app.  It does have an
			// unfortunate side-effect: it'll prevent mouse-motion events from being
			// sent to the app (via CoreWindow).
			//
			// The second way to hide a cursor sets PointerCursor to a transparent
			// cursor.  This allows mouse-motion events to be sent to the app, but is
			// more difficult to set up, as:
			//   1. WinRT/UWP, while providing a few stock cursors, does not provide
			//      a completely transparent cursor.
			//   2. WinRT/UWP allows apps to provide custom-built cursors, but *ONLY*
			//      if they are linked directly inside the app, via Win32-style
			//      cursor resource files.  APIs to create cursors at runtime are
			//      not provided to apps, and attempting to link-to or use Win32
			//      cursor-creation APIs could cause an app to fail Windows Store
			//      certification.
			//
			// SDL can use either means of hiding the cursor.  It provides a Win32-style
			// set of cursor resource files in its source distribution, inside
			// src/main/winrt/.  If those files are linked to an SDL-for-WinRT/UWP app
			// (by including them in a MSVC project, for example), SDL will attempt to
			// use those, if and when the cursor is hidden via SDL APIs.  If those
			// files are not linked in, SDL will attempt to hide the cursor via the
			// 'set PointerCursor to nullptr' means (which, if you recall, causes
			// mouse-motion data to NOT be sent to the app!).
			//
			// Tech notes:
			//  - SDL's blank cursor resource uses a resource ID of 5000.
			//  - SDL's cursor resources consist of the following two files:
			//     - src/main/winrt/SDL2-WinRTResource_BlankCursor.cur -- cursor pixel data
			//     - src/main/winrt/SDL2-WinRTResources.rc             -- declares the cursor resource, and its ID (of 5000)
			//


			//TODO: implement. see WINRT_ShowCursor(SDL_Cursor * cursor)
		}

		public override void CreatedWindow_UpdateSystemCursorFileName()
		{
			string fileName = EngineApp.SystemCursorFileName;

			//TODO: implement it:

			// https://blogs.msdn.microsoft.com/devfish/2012/08/01/customcursors-in-windows-8-csharp-metro-applications/

			// https://github.com/spurious/SDL-mirror/blob/master/src/video/winrt/SDL_winrtmouse.cpp

			//coreWindow.PointerCursor = new CoreCursor( CoreCursorType.Custom, 101 );
		}

		public unsafe override bool InitDirectInputMouseDevice()
		{
			//TODO: implement it !
			// Do not use DirectInput in name. it's not cross-platform name.
			return false;
		}

		public override void ShutdownDirectInputMouseDevice()
		{
			//TODO: implement it !
			// Do not use DirectInput in name. it's not cross-platform name.
		}

		public unsafe override void CreatedWindow_UpdateInputDevices()
		{
			//!!!!!всё тут проверить

			//!!!!//ничего не обновлять, если отрублены?

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			//!!!!new
			if( EngineApp.CreatedInsideEngineWindow != null && IsFocused() )
				CreatedWindow_UpdateShowSystemCursor( false );

			//mouse buttons
			{
				//!!!!

				//List<ValueTuple<EMouseButtons, int>> buttons = new List<(EMouseButtons, int)>();
				//buttons.Add( (EMouseButtons.Left, VK_LBUTTON) );
				//buttons.Add( (EMouseButtons.Right, VK_RBUTTON) );
				//buttons.Add( (EMouseButtons.Middle, VK_MBUTTON) );
				//buttons.Add( (EMouseButtons.XButton1, VK_XBUTTON1) );
				//buttons.Add( (EMouseButtons.XButton2, VK_XBUTTON2) );
				//foreach( var tuple in buttons )
				//{
				//	if( viewport.IsMouseButtonPressed( tuple.Item1 ) && GetKeyState( tuple.Item2 ) >= 0 )
				//	{
				//		bool handled = false;
				//		viewport.PerformMouseUp( tuple.Item1, ref handled );
				//	}
				//}


				if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) && GetKeyState( VK_LBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && GetKeyState( VK_RBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) && GetKeyState( VK_MBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton1 ) && GetKeyState( VK_XBUTTON1 ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton2 ) && GetKeyState( VK_XBUTTON2 ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
				}
			}

			//keys
			foreach( EKeys eKey in Viewport.AllKeys )
			{
				if( viewport.IsKeyPressed( eKey ) )
				{
					KeyInfo keyInfo = keysInfo[ (int)eKey ];

					if( keyInfo.keyCode != 0 )
					{
						if( GetKeyState( keyInfo.keyCode ) >= 0 )
						{
							KeyEvent keyEvent = new KeyEvent( eKey );
							bool handled = false;
							viewport.PerformKeyUp( keyEvent, ref handled );
						}
					}
				}
			}

			//mouse outside window client rectangle
			if( !viewport.MouseRelativeMode )
			{
				Vector2 mouse = viewport.MousePosition;

				if( mouse.X < 0 || mouse.X >= 1 || mouse.Y < 0 || mouse.Y >= 1 )
				{
					if( !mouse.Equals( lastMousePositionForCheckMouseOutsideWindow, .0001f ) )
					{
						lastMousePositionForCheckMouseOutsideWindow = mouse;
						EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
					}
				}
			}

			//mouse relative mode
			if( viewport.MouseRelativeMode )
			{
				//!!!!в EngineViewportControl тоже?
				//clip cursor by window rectangle
				if( IsFocused() )
				{
					SetCapture( EngineApp.ApplicationWindowHandle );

					RectangleI rectangle = CreatedWindow_GetWindowRectangle();
					rectangle.Left += 1;
					rectangle.Top += 1;
					rectangle.Right -= 1;
					rectangle.Bottom -= 1;
					ClipCursor( (IntPtr)( &rectangle ) );
				}

				//!!!!
				//if( DirectInputMouseDevice.Instance != null )
				//{
				//	////!!!!!
				//	//Log.Warning( "temp" );

				//	DirectInputMouseDevice.State state = DirectInputMouseDevice.Instance.GetState();
				//	if( state.Position.X != 0 || state.Position.Y != 0 )
				//	{
				//		//!!!!wrong
				//		Vector2F offset = new Vector2F(
				//			(float)(int)state.Position.X / viewport.SizeInPixels.X,
				//			(float)(int)state.Position.Y / viewport.SizeInPixels.Y );

				//		//!!!!
				//		viewport.PerformMouseMove( offset );

				//		if( !EngineApp.Closing && IsFocused() )
				//			CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
				//		//App.MousePosition = new Vec2( .5f, .5f );
				//	}
				//}
				//else
				{
					//!!!!надо ли

					if( !EngineApp.Closing && IsFocused() )
						CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
					//App.MousePosition = new Vec2( .5f, .5f );
				}
			}
		}

		public override bool IsKeyLocked( EKeys key )
		{
			int keyState = GetKeyState( (int)key );
			if( key != EKeys.Insert && key != EKeys.Capital )
				return ( keyState & 0x8001 ) != 0;
			return ( keyState & 1 ) != 0;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			if( viewport.MouseRelativeMode )
			{
				//!!!!
				//if( DirectInputMouseDevice.Instance != null )
				//	DirectInputMouseDevice.Instance.GetState();
			}
			else
			{
				ReleaseCapture();
				ClipCursor( IntPtr.Zero );
			}

			mustIgnoreOneMouseMoveAtRelativeMode = true;
		}

		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
		{
			if( !mustIgnoreOneMouseMoveAtRelativeMode )
			{
				delta = CreatedWindow_GetMousePosition() - lastMousePositionForMouseMoveDelta;
			}
			else
			{
				mustIgnoreOneMouseMoveAtRelativeMode = false;
				delta = Vector2F.Zero;
			}
		}
	}
}
