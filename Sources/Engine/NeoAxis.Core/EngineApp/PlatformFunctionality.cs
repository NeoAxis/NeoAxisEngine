// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NeoAxis
{
	/// <summary>
	/// Internal class for implementing the target platform.
	/// </summary>
	public abstract class PlatformFunctionality
	{
		static PlatformFunctionality instance;

		///////////////////////////////////////////

		public enum WindowState
		{
			Maximized,
			Minimized,
			Normal
		}

		///////////////////////////////////////////

		public enum WindowBorderStyle
		{
			None,
			Sizeable
		}

		///////////////////////////////////////////

		protected void SetInstance( PlatformFunctionality instance )
		{
			PlatformFunctionality.instance = instance;
		}

		public virtual void Init( IntPtr mainModuleData ) { }

		public virtual IntPtr GetGLContext() { return IntPtr.Zero; }

		public abstract Vector2I GetScreenSize();
		public abstract int GetScreenBitsPerPixel();
		//public abstract int GetMonitorCount();
		public abstract double GetSystemTime();
		public abstract Vector2I GetSmallIconSize();

		public virtual void FullscreenFadeOut( bool exitApplication ) { }
		public virtual void FullscreenFadeIn( bool exitApplication ) { }

		public abstract IntPtr CreatedWindow_CreateWindow();
		public abstract void CreatedWindow_DestroyWindow();

		public abstract void CreatedWindow_ProcessMessageEvents();
		public abstract void CreatedWindow_RunMessageLoop();
		public abstract void MessageLoopWaitMessage();

		public abstract bool IsWindowInitialized();

		public abstract void CreatedWindow_UpdateWindowTitle( string title );
		public virtual void CreatedWindow_UpdateWindowIcon( Icon smallIcon, Icon icon ) { }

		public abstract RectangleI CreatedWindow_GetWindowRectangle();
		public abstract RectangleI CreatedWindow_GetClientRectangle();
		//public abstract void SetWindowPosition( Vec2i position );
		//!!!!!!
		public abstract void CreatedWindow_SetWindowSize( Vector2I size );
		public abstract void CreatedWindow_SetWindowRectangle( RectangleI rectangle );
		public abstract bool CreatedWindow_IsWindowActive();

		public abstract void CreatedWindow_OnMouseRelativeModeChange();
		public abstract void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta );
		public abstract void CreatedWindow_UpdateInputDevices();

		public abstract bool IsIntoMenuLoop();

		//!!!!!CreatedWindow_?
		public abstract bool IsWindowVisible();
		//!!!!!CreatedWindow_?
		public abstract WindowState GetWindowState();
		public abstract void SetWindowState( WindowState value );
		//public abstract void SetWindowBorderStyle( WindowBorderStyles value );
		//public abstract void SetWindowTopMost( bool value );

		//!!!!!
		public abstract Vector2 CreatedWindow_GetMousePosition();
		public abstract void CreatedWindow_SetMousePosition( Vector2 value );


		//!!!!!
		public abstract void ProcessChangingVideoMode();

		//!!!!!
		public abstract bool IsFocused();

		//!!!!new
		public abstract bool ApplicationIsActivated();

		public abstract void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate );
		public abstract void CreatedWindow_UpdateSystemCursorFileName();

		public virtual bool InitDirectInputMouseDevice() { return false; }
		public virtual void ShutdownDirectInputMouseDevice() { }

		public abstract bool IsKeyLocked( EKeys key );

		public abstract string[] GetNativeModuleNames();

		//video modes
		public abstract List<Vector2I> GetVideoModes();
		public abstract bool ChangeVideoMode( Vector2I mode );
		public abstract void RestoreVideoMode();

		//gamma
		public abstract void SetGamma( float value );

		////Android
		//public virtual void MainModule_WindowMessage( IntPtr data ) { }

		//system messages
		//!!!!!
		public abstract bool ShowMessageBoxYesNoQuestion( string text, string caption );

		public abstract IntPtr CallSpecialPlatformSpecificMethod( string message, IntPtr param );

		public abstract IList<SystemSettings.DisplayInfo> GetAllDisplays();

		public abstract void GetSystemLanguage( out string name, out string englishName );

		///////////////////////////////////////////

		public static PlatformFunctionality Get()
		{
			if( instance == null )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
#if WINDOWS
					instance = new WindowsPlatformFunctionality();
#endif
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					Log.Fatal( "PlatformFunctionality: Get: Instance must be already initialized." );
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					Log.Fatal( "PlatformFunctionality: Get: Instance must be already initialized." );
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
				{
					Log.Fatal( "MacOSXInputDeviceManager impl." );
					//instance = new MacOSXPlatformFunctionality();
				}
				else
					Log.Fatal( "PlatformFunctionality: Get: Unknown platform." );
			}
			return instance;
		}
	}
}
