// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using DirectInput;
using NeoAxis;
using System.Reflection;
using System.Linq;

namespace Internal
{
	class MacOSPlatformFunctionality : PlatformFunctionality
	{
		static MacOSPlatformFunctionality instance;

		KeyInfo[] keysInfo = new KeyInfo[ EngineApp.GetEKeysMaxIndex() + 1 ];

		//bool suspendModeTimerCreated;

		bool intoMenuLoop;
		//!!!!
		//bool resizingMoving;

		//!!!!
		//IntPtr hCursorArrow;

		Dictionary<string, IntPtr> loadedSystemCursors = new Dictionary<string, IntPtr>();

		Vector2 lastMousePositionForMouseMoveDelta;
		Vector2 lastMousePositionForCheckMouseOutsideWindow;
		bool mustIgnoreOneMouseMoveAtRelativeMode;

		double maxFPSLastRenderTime;

		WindowedModeEnum? goingToAnotherWindowedMode;
		//bool goingToWindowedMode;
		//bool goingToFullScreenMode;
		bool goingToChangeWindowRectangle;

		static List<SystemSettings.DisplayInfo> tempScreenList = new List<SystemSettings.DisplayInfo>();

		bool createdWindow_UpdateShowSystemCursor;

		///////////////////////////////////////////

		static MacAppNativeWrapper.CallbackMessageEvent messageEventdelegate = MessageEvent;
		static MacAppNativeWrapper.CallbackLogInfo logInfoDelegate = LogInfo;
		static MacAppNativeWrapper.CallbackLogWarning logWarningDelegate = LogWarning;
		static MacAppNativeWrapper.CallbackLogFatal logFatalDelegate = LogFatal;

		//!!!!
		bool mustGoToFullscreenMinimizedMode;
		int mustGoToFullscreenMinimizedModeStep;
		bool fullscreenMinimizedMode;

		///////////////////////////////////////////////

		struct Wrapper
		{
			public const string library = "libNeoAxisCoreNative";
			public const CallingConvention convention = CallingConvention.Cdecl;
		}

		///////////////////////////////////////////////

		internal struct MacAppNativeWrapper
		{
			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MessageBox", CallingConvention = Wrapper.convention,
			//   CharSet = CharSet.Unicode )]
			//public static extern void MessageBox( string text, string caption );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeOut", CallingConvention = Wrapper.convention )]
			public static extern void FullscreenFadeOut( [MarshalAs( UnmanagedType.U1 )] bool exitApplication );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeIn", CallingConvention = Wrapper.convention )]
			public static extern void FullscreenFadeIn( [MarshalAs( UnmanagedType.U1 )] bool exitApplication );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_CreateWindow", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern IntPtr CreateWindow( WindowedModeEnum windowedMode, string title, int positionX, int positionY, int sizeX, int sizeY );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_DestroyWindow", CallingConvention = Wrapper.convention )]
			public static extern void DestroyWindow();

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_InitApplicationWindow", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			//public static extern IntPtr InitApplicationWindow( [MarshalAs( UnmanagedType.U1 )] bool fullscreen, int windowSizeX, int windowSizeY, string title );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowVisible", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsWindowVisible();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowActive", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsWindowActive();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowFocused", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsWindowFocused();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowRectangle", CallingConvention = Wrapper.convention )]
			public static extern void GetWindowRectangle( out RectangleI rect );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowClientRect", CallingConvention = Wrapper.convention )]
			public static extern void GetWindowClientRect( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out RectangleI rect );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
			public static extern void GetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out int x, out int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
			public static extern void SetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, int x, int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetSystemTime", CallingConvention = Wrapper.convention )]
			public static extern double GetSystemTime();

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTopMost", CallingConvention = Wrapper.convention )]
			//public static extern void SetWindowTopMost( [MarshalAs( UnmanagedType.U1 )] bool value );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenSize", CallingConvention = Wrapper.convention )]
			public static extern void GetScreenSize( out int width, out int height );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenBitsPerPixel", CallingConvention = Wrapper.convention )]
			public static extern int GetScreenBitsPerPixel();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ProcessEvents", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool ProcessEvents();

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShutdownApplicationWindow", CallingConvention = Wrapper.convention )]
			//public static extern void ShutdownApplicationWindow();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowState", CallingConvention = Wrapper.convention )]
			public static extern int GetWindowState();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowState", CallingConvention = Wrapper.convention )]
			public static extern void SetWindowState( int state );

			[UnmanagedFunctionPointer( Wrapper.convention )]
			public delegate void CallbackMessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC );

			[UnmanagedFunctionPointer( Wrapper.convention )]
			public delegate void CallbackLogInfo( IntPtr/*char* */text );
			[UnmanagedFunctionPointer( Wrapper.convention )]
			public delegate void CallbackLogWarning( IntPtr/*char* */text );
			[UnmanagedFunctionPointer( Wrapper.convention )]
			public delegate void CallbackLogFatal( IntPtr/*char* */text );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_Initialize", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool Initialize( CallbackMessageEvent messageEvent, CallbackLogInfo logInfo, CallbackLogWarning logWarning, CallbackLogFatal logFatal );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ChangeVideoMode", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool ChangeVideoMode( int width, int height, int bpp );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreVideoMode", CallingConvention = Wrapper.convention )]
			public static extern bool RestoreVideoMode();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetVideoModes", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetVideoModes( out int count, out Vector3I* array );

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayCount", CallingConvention = Wrapper.convention )]
			//public static extern int GetDisplayCount();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShowSystemCursor", CallingConvention = Wrapper.convention )]
			public static extern void ShowSystemCursor( [MarshalAs( UnmanagedType.U1 )] bool show );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyPressed", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsKeyPressed( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyLocked", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsKeyLocked( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsSystemKey", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsSystemKey( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsMouseButtonPressed", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public static extern bool IsMouseButtonPressed( int buttonCode );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeMemory", CallingConvention = Wrapper.convention )]
			public static extern void FreeMemory( IntPtr buffer );

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowPosition", CallingConvention = Wrapper.convention )]
			//public static extern void SetWindowPosition( int x, int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowSize", CallingConvention = Wrapper.convention )]
			public static extern void SetWindowSize( int width, int height );

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowBorderStyle", CallingConvention = Wrapper.convention )]
			//public static extern void SetWindowBorderStyle( int style );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowRectangle", CallingConvention = Wrapper.convention )]
			public static extern void SetWindowRectangle( int left, int top, int right, int bottom );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTitle", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern void SetWindowTitle( string title );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern void GetMouseMoveDelta( out int x, out int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ResetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern void ResetMouseMoveDelta( [MarshalAs( UnmanagedType.U1 )] bool resetIgnoreCounter );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_CallCustomPlatformSpecificMethod", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetLoadedBundleNames", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void GetLoadedBundleNames( out IntPtr* list, out int count );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateAcceptsMouseMovedEventsFlag", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public static extern void UpdateAcceptsMouseMovedEventsFlag();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateWindowForProcessChangingVideoMode", CallingConvention = Wrapper.convention )]
			public static extern void UpdateWindowForProcessChangingVideoMode();

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetGamma", CallingConvention = Wrapper.convention )]
			//public static extern void SetGamma( float value );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeOutString", CallingConvention = Wrapper.convention )]
			public static extern void FreeOutString( IntPtr pointer );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ActivateFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
			public static extern void ActivateFullscreenMinimizedMode();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreFromFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
			public static extern void RestoreFromFullscreenMinimizedMode( int width, int height );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MinimizeWindow", CallingConvention = Wrapper.convention )]
			public static extern void MinimizeWindow();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetActiveDisplayList", CallingConvention = Wrapper.convention )]
			public unsafe static extern int GetActiveDisplayList( int bufferLength, uint* buffer );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayInfo", CallingConvention = Wrapper.convention )]
			public static extern void GetDisplayInfo( uint display, out IntPtr deviceName, out RectangleI bounds, out RectangleI workingArea, [MarshalAs( UnmanagedType.U1 )] out bool primary );

			public static string GetOutString( IntPtr pointer )
			{
				if( pointer != IntPtr.Zero )
				{
					string result = Marshal.PtrToStringUni( pointer );
					FreeOutString( pointer );
					return result;
				}
				else
					return null;
			}
		}

		public enum MessageTypes
		{
			MouseDown,
			MouseUp,
			MouseDoubleClick,
			MouseWheel,
			MouseMove,
			KeyDown,
			KeyUp,
			WindowDidResize,
			WindowDidBecomeKey,
			WindowDidResignKey,
			WindowWillMiniaturize,
			WindowDidMiniaturize,
			WindowDidDeminiaturize,
			Periodic,
		}

		///////////////////////////////////////////////

		struct KeyInfo
		{
			public int keyCode;

			public KeyInfo( int keyCode )
			{
				this.keyCode = keyCode;
			}
		}

		///////////////////////////////////////////////

		public static EngineApp App
		{
			get { return EngineApp.Instance; }
		}

		public MacOSPlatformFunctionality()
		{
			instance = this;
			SetInstance( this, SystemSettings.Platform.macOS );
			new PlatformSpecificUtilityMacOS();

			MacAppNativeWrapper.Initialize( messageEventdelegate, logInfoDelegate, logWarningDelegate, logFatalDelegate );
		}

		public override Vector2I GetScreenSize()
		{
			MacAppNativeWrapper.GetScreenSize( out var width, out var height );
			return new Vector2I( width, height );
		}

		public override int GetScreenBitsPerPixel()
		{
			return MacAppNativeWrapper.GetScreenBitsPerPixel();
		}

		public override Vector2I GetSmallIconSize()
		{
			return Vector2I.Zero;
		}

		static int HIWORD( int n )
		{
			return ( ( n >> 0x10 ) & 0xffff );
		}

		static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}

		static void CreatedWindow_ApplicationWindowProc( IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam )
		{

			//!!!!what useful?


			//switch( message )
			//{
			//case WM_ENTERSIZEMOVE:
			//	instance.resizingMoving = true;
			//	return IntPtr.Zero;

			//case WM_EXITSIZEMOVE:
			//	instance.resizingMoving = false;
			//	return IntPtr.Zero;

			//case WM_SIZE:
			//	if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
			//	{
			//		EngineApp.CreatedWindowProcessResize();
			//		return IntPtr.Zero;
			//	}
			//	break;

			//case WM_GETMINMAXINFO:
			//	unsafe
			//	{
			//		MINMAXINFO* info = (MINMAXINFO*)lParam;
			//		var size = new Vector2I( 100, 100 );
			//		if( ProjectSettings.Initialized )
			//		{
			//			size = ProjectSettings.Get.General.WindowSizeMinimal.Value;
			//			if( ProjectSettings.Get.General.WindowSizeApplySystemFontScale )
			//				size = ( size.ToVector2() * SystemSettings.DPIScale ).ToVector2I();
			//			info->ptMinTrackSize = size;
			//		}
			//		info->ptMinTrackSize = size;
			//	}
			//	return IntPtr.Zero;
			//}

			//if( EngineApp.Created && !EngineApp.Closing )
			//{
			//	switch( message )
			//	{

			//	case WM_SETFOCUS:
			//		{
			//			instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
			//			instance.CreatedWindow_UpdateShowSystemCursor( true );
			//			return IntPtr.Zero;
			//		}
			//	//break;

			//	case WM_KILLFOCUS:
			//		{
			//			instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
			//			instance.CreatedWindow_UpdateShowSystemCursor( true );
			//			return IntPtr.Zero;
			//		}
			//	//break;

			//	case WM_ACTIVATE:
			//		{
			//			if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
			//			{
			//				bool activate = LOWORD( (int)wParam ) != WA_INACTIVE;

			//				instance.mustIgnoreOneMouseMoveAtRelativeMode = true;

			//				if( activate )
			//				{
			//					if( EngineApp.FullscreenEnabled )
			//					{
			//						if( EngineApp.FullscreenSize != instance.GetScreenSize() )
			//							EngineApp.MustChangeVideoMode();
			//					}
			//				}
			//				else
			//				{
			//					if( viewport.MouseRelativeMode )
			//					{
			//						ReleaseCapture();
			//						ClipCursor( IntPtr.Zero );
			//					}

			//					//!!!!просто обновл€ем. ведь внутри и так проверка есть. еще можно зафорсить. но не будет ли так, 
			//					//что отключитс€ слишком быстро? нужно там где включает ставить флаг -> ChangeVUdeMode.
			//					EngineApp.EnginePauseUpdateState( false, true );
			//					//if( App.SuspendWorkingWhenApplicationIsNotActive )
			//					//   App.DoSystemPause( true, true );

			//					if( EngineApp.FullscreenEnabled )
			//					{
			//						instance.SetWindowState( WindowState.Minimized );

			//						if( !EngineApp.NeedExit )
			//							SystemSettings.RestoreVideoMode();
			//					}
			//				}
			//				return IntPtr.Zero;
			//			}
			//		}
			//		break;

			//	case WM_ENTERMENULOOP:
			//		{
			//			if( viewport.MouseRelativeMode )
			//			{
			//				ReleaseCapture();
			//				ClipCursor( IntPtr.Zero );
			//			}

			//			//if( App.SuspendWorkingWhenApplicationIsNotActive )
			//			//   App.DoSystemPause( true, true );
			//			instance.intoMenuLoop = true;
			//			EngineApp.EnginePauseUpdateState( false, true );

			//			return IntPtr.Zero;
			//		}
			//	//break;

			//	case WM_EXITMENULOOP:
			//		instance.intoMenuLoop = false;
			//		return IntPtr.Zero;

			//	case WM_MOUSEMOVE:
			//	case WM_NCMOUSEMOVE:
			//		EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
			//		return IntPtr.Zero;


			//case WM_TIMER:
			//	if( (int)wParam == suspendModeTimerID )
			//	{
			//		if( EngineApp.DrawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
			//		{
			//			unsafe
			//			{
			//				InvalidateRect( hWnd, null, false );
			//			}
			//		}

			//		if( !IsAllowApplicationIdle() )
			//			EngineApp.CreatedWindowApplicationIdle( true );

			//		return IntPtr.Zero;
			//	}
			//	break;


			//case WM_PAINT:

			//	if( EngineApp.insideRunMessageLoop && EngineApp.EnginePaused && !instance.resizingMoving && !instance.intoMenuLoop && !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
			//	{
			//		EngineApp.CreatedWindowApplicationIdle( false );
			//	}
			//	break;
			//	}
			//}
		}

		public override void CreatedWindow_ProcessMessageEvents()
		{
			//!!!!need process events?			
		}

		public override bool IsIntoMenuLoop()
		{
			return intoMenuLoop;
		}

		public override void MessageLoopWaitMessage()
		{
			//!!!!for editor, widget

			//WaitMessage();
		}

		public override bool IsWindowInitialized()
		{
			return EngineApp.ApplicationWindowHandle != IntPtr.Zero;
		}

		public override void CreatedWindow_UpdateWindowTitle( string title )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				MacAppNativeWrapper.SetWindowTitle( title );
		}

		static Assembly GetAssemblyByName( string name )
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault( assembly => assembly.GetName().Name == name );
		}

		public override void CreatedWindow_UpdateWindowIcon( object icon, string iconFilePath )
		{
			//!!!!impl
		}

		public override RectangleI CreatedWindow_GetWindowRectangle()
		{
			MacAppNativeWrapper.GetWindowRectangle( out var result );
			return result;
		}

		public override RectangleI CreatedWindow_GetClientRectangle()
		{
			MacAppNativeWrapper.GetWindowClientRect( EngineApp.WindowedMode != WindowedModeEnum.Windowed, out var result );
			return result;
		}

		//void SetWindowPosition( Vec2i position )
		//{
		//   SetWindowPos( App.ApplicationWindow.Handle, IntPtr.Zero, position.X, position.Y, 0, 0, SWP_NOSIZE );
		//}

		public override void CreatedWindow_SetWindowSize( Vector2I size )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				MacAppNativeWrapper.SetWindowSize( size.X, size.Y );
		}

		public override bool CreatedWindow_IsWindowActive()
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				return MacAppNativeWrapper.IsWindowActive();
			return false;

			//!!!!need?
			//if( GetForegroundWindow() != EngineApp.ApplicationWindowHandle )
			//	return false;
			//if( GetWindowState() == WindowState.Minimized )
			//	return false;
			//return true;
		}

		public override bool IsWindowVisible()
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				return MacAppNativeWrapper.IsWindowVisible();
			return true;
		}

		public override WindowState GetWindowState()
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				return (WindowState)MacAppNativeWrapper.GetWindowState();
			return WindowState.Normal;
		}

		public override void SetWindowState( WindowState value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				if( EngineApp.WindowedMode != WindowedModeEnum.Windowed && value == WindowState.Minimized )
				{
					if( !instance.fullscreenMinimizedMode )
					{
						instance.mustGoToFullscreenMinimizedMode = true;
						instance.mustGoToFullscreenMinimizedModeStep = 1;
					}
					return;
				}

				MacAppNativeWrapper.SetWindowState( (int)value );
			}
		}

		//void SetWindowBorderStyle( WindowBorderStyle value )
		//{
		//	if( IntPtr.Size == 8 )
		//	{
		//		ulong style = (ulong)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

		//		if( value == WindowBorderStyle.None )
		//		{
		//			unchecked
		//			{
		//				style &= ~(ulong)WS_OVERLAPPEDWINDOW;
		//				style |= ( (ulong)WS_POPUP );
		//			}
		//		}
		//		else if( value == WindowBorderStyle.Sizeable )
		//		{
		//			unchecked
		//			{
		//				style &= ~(ulong)WS_POPUP;
		//				style |= ( (ulong)WS_OVERLAPPEDWINDOW );
		//			}
		//		}

		//		SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)style );
		//	}
		//	else
		//	{
		//		uint style = (uint)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

		//		if( value == WindowBorderStyle.None )
		//		{
		//			unchecked
		//			{
		//				style &= ~(uint)WS_OVERLAPPEDWINDOW;
		//				style |= ( (uint)WS_POPUP );
		//			}
		//		}
		//		else if( value == WindowBorderStyle.Sizeable )
		//		{
		//			unchecked
		//			{
		//				style &= ~(uint)WS_POPUP;
		//				style |= ( (uint)WS_OVERLAPPEDWINDOW );
		//			}
		//		}

		//		SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)(int)style );
		//	}
		//}

		public override void SetWindowVisible( bool value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				//!!!!impl

				//int show;
				//if( value )
				//	show = SW_SHOW;
				//else
				//	show = SW_HIDE;
				//ShowWindow( EngineApp.ApplicationWindowHandle, show );
			}
		}

		//void SetWindowTopMost( bool value )
		//{
		//   SetWindowPos( App.WindowHandle, value ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0,
		//      0, 0, SWP_NOMOVE | SWP_NOSIZE );
		//}

		public override Vector2 CreatedWindow_GetMousePosition()
		{
			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
				return Vector2F.Zero;

			MacAppNativeWrapper.GetClientRectangleCursorPosition( EngineApp.WindowedMode != WindowedModeEnum.Windowed, out var x, out var y );

			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			return new Vector2(
				(float)x / (float)( viewport.SizeInPixels.X - viewport.SizeInPixels.X % 2 ),
				(float)y / (float)( viewport.SizeInPixels.Y - viewport.SizeInPixels.Y % 2 ) );

			//int x, y;
			//MacAppNativeWrapper.GetClientRectangleCursorPosition( App.FullScreen, out x, out y );

			//return new Vec2(
			//	(float)x / (float)( App.VideoMode.X - App.VideoMode.X % 2 ),
			//	(float)y / (float)( App.VideoMode.Y - App.VideoMode.Y % 2 ) );
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var position = new Vector2I(
					(int)(float)( value.X * (float)viewport.SizeInPixels.X ),
					(int)(float)( value.Y * (float)viewport.SizeInPixels.Y ) );

				MacAppNativeWrapper.SetClientRectangleCursorPosition( EngineApp.WindowedMode != WindowedModeEnum.Windowed, position.X, position.Y );


				//if( App.WindowHandle != IntPtr.Zero )
				//{
				//	Vec2I position = new Vec2I(
				//		(int)(float)( value.X * (float)App.VideoMode.X ),
				//		(int)(float)( value.Y * (float)App.VideoMode.Y ) );
				//	MacAppNativeWrapper.SetClientRectangleCursorPosition( EngineApp.WindowedMode != WindowedModeEnum.Windowed, position.X, position.Y );
				//}

				lastMousePositionForMouseMoveDelta = value;
				lastMousePositionForCheckMouseOutsideWindow = value;
			}
		}

		public override bool IsFocused()
		{
			return MacAppNativeWrapper.IsWindowFocused();
		}

		public override bool ApplicationIsActive()
		{
			//!!!!good? it is same as CreatedWindow_IsWindowActive

			if( !MacAppNativeWrapper.IsWindowActive() )
				return false;

			if( GetWindowState() == WindowState.Minimized )
				return false;

			return true;
		}

		public override IntPtr GetSystemCursorByFileName( string virtualFileName )
		{
			//!!!!impl
			return nint.Zero;

			//IntPtr hCursor = IntPtr.Zero;

			//if( !string.IsNullOrEmpty( virtualFileName ) )
			//{
			//	if( !loadedSystemCursors.TryGetValue( virtualFileName, out hCursor ) )
			//	{
			//		hCursor = IntPtr.Zero;

			//		string realFileName;
			//		if( Path.IsPathRooted( virtualFileName ) )
			//			realFileName = virtualFileName;
			//		else
			//			realFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );

			//		if( File.Exists( realFileName ) )
			//		{
			//			//load from real file system
			//			hCursor = LoadCursorFromFile( realFileName );
			//		}
			//		else
			//		{
			//			//load from virtual file system

			//			string tempRealFileName = VirtualPathUtility.GetRealPathByVirtual( string.Format( "user:_Temp_{0}", Path.GetFileName( virtualFileName ) ) );

			//			try
			//			{
			//				string directoryName = Path.GetDirectoryName( tempRealFileName );
			//				if( !Directory.Exists( directoryName ) )
			//					Directory.CreateDirectory( directoryName );

			//				byte[] data;

			//				using( VirtualFileStream stream = VirtualFile.Open( virtualFileName ) )
			//				{
			//					data = new byte[ stream.Length ];
			//					if( stream.Read( data, 0, (int)stream.Length ) != stream.Length )
			//						throw new Exception();
			//				}

			//				File.WriteAllBytes( tempRealFileName, data );

			//				hCursor = LoadCursorFromFile( tempRealFileName );

			//				File.Delete( tempRealFileName );
			//			}
			//			catch { }
			//		}

			//		loadedSystemCursors.Add( virtualFileName, hCursor );
			//	}
			//}

			//return hCursor;
		}

		public override void CreatedWindow_UpdateSystemCursorFileName()
		{
			//!!!!impl
		}

		public unsafe override bool InitDirectInputMouseDevice()
		{
			//!!!!impl mouse relative mode
			return false;
		}

		public override void ShutdownDirectInputMouseDevice()
		{
			//!!!!impl mouse relative mode
		}

		public unsafe override void CreatedWindow_UpdateInputDevices()
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			MacAppNativeWrapper.UpdateAcceptsMouseMovedEventsFlag();

			if( EngineApp.CreatedInsideEngineWindow != null && IsFocused() )
				CreatedWindow_UpdateShowSystemCursor( false );

			//!!!!old code. need?
			//if( MacAppNativeWrapper.IsWindowFocused() && new Rectangle( 0, 0, 1, 1 ).Contains( App.MousePosition ) )
			//	UpdateShowSystemCursor();

			//mouse buttons
			{
				if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Left ) )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Right ) )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Middle ) )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton1 ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton1 ) )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton2 ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton2 ) )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
				}
			}

			//keys
			foreach( EKeys eKey in Viewport.AllKeys )
			{
				//!!!!need eKey != EKeys.Shift && eKey != EKeys.Control && eKey != EKeys.Alt && eKey != EKeys.Command?

				//if( viewport.IsKeyPressed( eKey ) && eKey != EKeys.Shift && eKey != EKeys.Control && eKey != EKeys.Alt && eKey != EKeys.Command )
				if( viewport.IsKeyPressed( eKey ) )
				{
					var keyInfo = keysInfo[ (int)eKey ];
					if( keyInfo.keyCode != 0 )
					{
						if( !MacAppNativeWrapper.IsKeyPressed( eKey ) )
						{
							var keyEvent = new KeyEvent( eKey );
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

					//!!!!impl

					//SetCapture( EngineApp.ApplicationWindowHandle );

					//RectangleI rectangle = CreatedWindow_GetWindowRectangle();
					//rectangle.Left += 1;
					//rectangle.Top += 1;
					//rectangle.Right -= 1;
					//rectangle.Bottom -= 1;
					//ClipCursor( (IntPtr)( &rectangle ) );
				}

				//if( DirectInputMouseDevice.Instance != null )
				//{
				//	DirectInputMouseDevice.State state = DirectInputMouseDevice.Instance.GetState();
				//	if( state.Position.X != 0 || state.Position.Y != 0 )
				//	{
				//		Vector2F offset = new Vector2F(
				//			(float)(int)state.Position.X / viewport.SizeInPixels.X,
				//			(float)(int)state.Position.Y / viewport.SizeInPixels.Y );

				//		viewport.PerformMouseMove( offset );

				//		if( !EngineApp.Closing && IsFocused() )
				//			CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
				//		//App.MousePosition = new Vec2( .5f, .5f );
				//	}
				//}
				//else
				{
					//!!!!need?

					if( !EngineApp.Closing && IsFocused() )
						CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
				}
			}
		}

		public unsafe override string[] GetNativeModuleNames()
		{
			MacAppNativeWrapper.GetLoadedBundleNames( out var list, out var count );

			var result = new string[ count ];
			for( int n = 0; n < count; n++ )
				result[ n ] = MacAppNativeWrapper.GetOutString( list[ n ] );

			MacAppNativeWrapper.FreeMemory( (IntPtr)list );

			return result;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			//!!!!impl

			//if( viewport.MouseRelativeMode )
			//{
			//	if( DirectInputMouseDevice.Instance != null )
			//		DirectInputMouseDevice.Instance.GetState();
			//}
			//else
			//{
			//	ReleaseCapture();
			//	ClipCursor( IntPtr.Zero );
			//}

			mustIgnoreOneMouseMoveAtRelativeMode = true;

			MacAppNativeWrapper.ResetMouseMoveDelta( true );
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

			//!!!!

			//MacAppNativeWrapper.GetMouseMoveDelta( out var x, out var y );
			//if( App.VideoMode.X != 0 && App.VideoMode.Y != 0 )
			//	delta = new Vec2( x, y ) / App.VideoMode.ToVec2();
			//else
			//	delta = Vec2.Zero;
			//MacAppNativeWrapper.ResetMouseMoveDelta( false );

		}

		public override IntPtr CallPlatformSpecificMethod( string message, IntPtr param )
		{
			return IntPtr.Zero;
		}

		public override void ProcessChangingVideoMode()
		{

			//!!!!good?

			if( EngineApp.WindowedMode == WindowedModeEnum.Fullscreen )
			{
				goingToAnotherWindowedMode = EngineApp.WindowedMode;

				SetWindowState( WindowState.Minimized );

				//change video mode
				if( !SystemSettings.ChangeVideoMode( EngineApp.WindowedModeSize ) )
					return;
			}
			else
			{
				goingToAnotherWindowedMode = EngineApp.WindowedMode;

				//change video mode
				SystemSettings.RestoreVideoMode();
			}




			////change video mode
			//if( App.FullScreen )
			//{
			//	if( !DisplaySettings.ChangeVideoMode( App.VideoMode ) )
			//		return;
			//	App.lastFullScreenWindowSize = App.VideoMode;
			//}
			//else
			//{
			//	DisplaySettings.RestoreVideoMode();
			//}

			//MacAppNativeWrapper.UpdateWindowForProcessChangingVideoMode();

			//App.DoResize();





			//if( EngineApp.FullscreenEnabled )
			//{
			//	goingToFullScreenMode = true;

			//	//minimize window
			//	SetWindowState( WindowState.Minimized );

			//	//!!!!!так?
			//	//change video mode
			//	if( !SystemSettings.ChangeVideoMode( EngineApp.FullscreenSize ) )
			//		return;
			//	//было
			//	//App.lastFullScreenWindowSize = App.FullScreenSize;

			//	//update window
			//	bool topMost = !Debugger.IsAttached;
			//	SetWindowBorderStyle( WindowBorderStyle.None );
			//	SetWindowState( WindowState.Normal );
			//	SetWindowPos( EngineApp.ApplicationWindowHandle, topMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, EngineApp.FullscreenSize.X, EngineApp.FullscreenSize.Y, 0 );
			//}
			//else
			//{
			//	goingToWindowedMode = true;

			//	//!!!!!так?
			//	//change video mode
			//	SystemSettings.RestoreVideoMode();
			//}
		}

		unsafe public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
		{
			var result = new List<SystemSettings.DisplayInfo>();

			var buffer = new uint[ 256 ];
			int count;
			fixed( uint* pBuffer = buffer )
				count = MacAppNativeWrapper.GetActiveDisplayList( buffer.Length, pBuffer );

			for( int n = 0; n < count; n++ )
			{
				var display = buffer[ n ];
				MacAppNativeWrapper.GetDisplayInfo( display, out var deviceNamePointer, out var bounds, out var workingArea, out var primary );
				var deviceName = MacAppNativeWrapper.GetOutString( deviceNamePointer );
				var displayInfo = new SystemSettings.DisplayInfo( deviceName, bounds, workingArea, primary );
				result.Add( displayInfo );
			}

			if( result.Count == 0 )
			{
				var area = new RectangleI( Vector2I.Zero, GetScreenSize() );
				var info = new SystemSettings.DisplayInfo( "Primary", area, area, true );
				result.Add( info );
			}

			return result;
		}

		public override void CreatedWindow_SetWindowRectangle( RectangleI rectangle )
		{
			goingToChangeWindowRectangle = true;
			//!!!!need?
			//if( GetWindowState() == WindowState.Maximized )
			//	SetWindowState( WindowState.Normal );
			MacAppNativeWrapper.SetWindowRectangle( rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom );
			goingToChangeWindowRectangle = false;

			EngineApp.CreatedWindowProcessResize();
		}

		public override void GetSystemLanguage( out string name, out string englishName )
		{
			//!!!!maybe now ok?

			name = MacAppNativeWrapper.GetOutString( CallCustomPlatformSpecificMethod( "GetSystemLanguageName", IntPtr.Zero ) );
			englishName = MacAppNativeWrapper.GetOutString( CallCustomPlatformSpecificMethod( "GetSystemLanguageEnglishName", IntPtr.Zero ) );

			//name = CultureInfo.CurrentUICulture.Name;
			//englishName = CultureInfo.CurrentUICulture.EnglishName;
		}

		public void SetDarkMode( IntPtr handle, bool enable )
		{
			//!!!!impl

			//!!!!defaults write <Bundle-Identifier> NSRequiresAquaSystemAppearance -bool yes

			//try
			//{
			//	int value = enable ? 1 : 0;
			//	DwmSetWindowAttribute( handle, DwmWindowAttribute.UseImmersiveDarkMode, ref value, 4 );
			//}
			//catch { }
		}

		public override bool ChangeVideoMode( Vector2I mode )
		{
			return MacAppNativeWrapper.ChangeVideoMode( mode.X, mode.Y, GetScreenBitsPerPixel() );
		}

		public override void RestoreVideoMode()
		{
			MacAppNativeWrapper.RestoreVideoMode();
		}

		public override void FullscreenFadeOut( bool exitApplication )
		{
			//!!!!was in 3.5
			//MacAppNativeWrapper.FullscreenFadeOut( exitApplication );
		}

		public override void FullscreenFadeIn( bool exitApplication )
		{
			//!!!!was in 3.5
			//MacAppNativeWrapper.FullscreenFadeIn( exitApplication );
		}

		public override List<Vector2I> GetVideoModes()
		{
			var videoModes = new List<Vector2I>();

			int bpp = GetScreenBitsPerPixel();

			unsafe
			{
				try
				{
					int count;
					Vector3I* array;
					MacAppNativeWrapper.GetVideoModes( out count, out array );

					if( count != 0 )
					{
						for( int n = 0; n < count; n++ )
						{
							var item = array[ n ];

							var mode = item.ToVector2I();
							int modeBPP = item.Z;

							if( bpp == modeBPP )
							{
								if( !videoModes.Contains( mode ) )
									videoModes.Add( mode );
							}
						}
					}

					if( array != null )
						MacAppNativeWrapper.FreeMemory( (IntPtr)array );
				}
				catch { }
			}

			return videoModes;
		}

		public override bool IsKeyLocked( EKeys key )
		{
			return MacAppNativeWrapper.IsKeyLocked( key );
		}

		//public override void SetGamma( float value )
		//{
		//	//MacAppNativeWrapper.SetGamma( value );
		//}

		public override IntPtr CreatedWindow_CreateWindow()
		{
			bool showMaximized = EngineApp.WindowedMode == WindowedModeEnum.Windowed && EngineApp.InitSettings.CreateWindowState.Value == EngineApp.WindowStateEnum.Maximized && !EngineApp.InitSettings.MultiMonitorMode.Value;
			bool showMinimized = EngineApp.InitSettings.CreateWindowState.Value == EngineApp.WindowStateEnum.Minimized;

			Vector2I position;
			Vector2I size;
			{
				if( showMaximized )
				{
					size = new Vector2I( 800, 600 );
					position = ( GetScreenSize() - size ) / 2;
				}
				else
				{
					if( EngineApp.WindowedMode == WindowedModeEnum.Windowed || EngineApp.InitSettings.MultiMonitorMode.Value )
						position = EngineApp.InitSettings.CreateWindowPosition.Value;
					else
						position = Vector2I.Zero;
					size = EngineApp.InitSettings.CreateWindowSize.Value;
				}
			}

			//uint style = 0;
			//uint exStyle = 0;
			//{
			//	if( !showMaximized )
			//		style |= WS_VISIBLE;

			//	if( EngineApp.WindowedMode == WindowedModeEnum.Fullscreen || EngineApp.WindowedMode == WindowedModeEnum.Borderless )
			//		style |= WS_POPUP;
			//	else
			//		style |= WS_OVERLAPPEDWINDOW;

			//	if( EngineApp.WindowedMode == WindowedModeEnum.Fullscreen )
			//		exStyle |= WS_EX_TOPMOST;

			//	if( IsServerWithoutRenderingBackend() )
			//		exStyle |= WS_EX_COMPOSITED;
			//}


			//!!!!impl WindowedMode.Borderless


			var windowHandle = MacAppNativeWrapper.CreateWindow( EngineApp.WindowedMode, EngineApp.CreatedInsideEngineWindow.Title, position.X, position.Y, size.X, size.Y );
			if( windowHandle == IntPtr.Zero )
				Console.WriteLine( "MacAppNativeWrapper: CreateWindow: Failed to create window." );

			if( SystemSettings.DarkMode )
				SetDarkMode( windowHandle, true );

			//!!!!impl

			//if( showMaximized )
			//	ShowWindow( windowHandle, SW_SHOWMAXIMIZED );
			//if( showMinimized )
			//	ShowWindow( windowHandle, SW_SHOWMINIMIZED );

			//SetForegroundWindow( windowHandle );
			//SetFocus( windowHandle );

			////SetTimer( windowHandle, (IntPtr)suspendModeTimerID, 10, IntPtr.Zero );
			////suspendModeTimerCreated = true;

			return windowHandle;


			//var handle = MacAppNativeWrapper.CreateWindow( App.FullScreen, App.VideoMode.X, App.VideoMode.Y, App.WindowTitle );

			//var handle = MacAppNativeWrapper.InitApplicationWindow( App.FullScreen, App.VideoMode.X, App.VideoMode.Y, App.WindowTitle );
			//return handle;
		}

		public override void CreatedWindow_DestroyWindow()
		{
			MacAppNativeWrapper.DestroyWindow();

			//MacAppNativeWrapper.ShutdownApplicationWindow();
		}


		//public override int GetMonitorCount()
		//{
		//   return MacAppNativeWrapper.GetDisplayCount();
		//}

		//public override double GetSystemTime()
		//{
		//	return MacAppNativeWrapper.GetSystemTime();
		//}

		unsafe static void MessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC )
		{
			Viewport viewport = null;
			if( !RenderingSystem.Disposed && RenderingSystem.ApplicationRenderTarget != null )
				viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			switch( messageType )
			{
			case MessageTypes.MouseDown:
				{
					var handled = false;
					viewport.PerformMouseDown( (EMouseButtons)parameterA, ref handled );

					//!!!!double click?
				}
				break;

			case MessageTypes.MouseUp:
				{
					var handled = false;
					viewport.PerformMouseUp( (EMouseButtons)parameterA, ref handled );
				}
				break;

			case MessageTypes.MouseDoubleClick:
				{
					var handled = false;
					viewport.PerformMouseDoubleClick( (EMouseButtons)parameterA, ref handled );
				}
				break;

			case MessageTypes.MouseWheel:
				{
					var handled = false;
					viewport.PerformMouseWheel( (int)parameterA, ref handled );
				}
				break;

			case MessageTypes.MouseMove:
				{
					//!!!!

					EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
					//viewport.PerformMouseMove();
				}
				break;

			case MessageTypes.KeyDown:
				{
					var eKey = (EKeys)parameterA;
					var character = parameterB;

					bool handled = false;
					bool suppressKeyPress = false;

					if( eKey != 0 )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						viewport.PerformKeyDown( keyEvent, ref handled );

						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
						{
							keyEvent = new KeyEvent( EKeys.Shift );
							viewport.PerformKeyDown( keyEvent, ref handled );
						}
						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
						{
							keyEvent = new KeyEvent( EKeys.Control );
							viewport.PerformKeyDown( keyEvent, ref handled );
						}
						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
						{
							keyEvent = new KeyEvent( EKeys.Alt );
							viewport.PerformKeyDown( keyEvent, ref handled );
						}
						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
						{
							keyEvent = new KeyEvent( EKeys.Command );
							viewport.PerformKeyDown( keyEvent, ref handled );
						}

						//Cmd-Tab. Minimize for fullscreen mode.
						if( !handled && eKey == EKeys.Tab && viewport.IsKeyPressed( EKeys.Command ) )
						{
							if( EngineApp.WindowedMode != WindowedModeEnum.Windowed )
							{
								if( !instance.fullscreenMinimizedMode )
								{
									instance.mustGoToFullscreenMinimizedMode = true;
									instance.mustGoToFullscreenMinimizedModeStep = 1;
									break;
								}
								handled = true;
							}
						}

						//Command+M. Minimize.
						if( !handled && eKey == EKeys.M && viewport.IsKeyPressed( EKeys.Command ) )
						{
							if( EngineApp.WindowedMode != WindowedModeEnum.Windowed )
							{
								if( !instance.fullscreenMinimizedMode )
								{
									instance.mustGoToFullscreenMinimizedMode = true;
									instance.mustGoToFullscreenMinimizedModeStep = 1;
									break;
								}
							}
							else
								MacAppNativeWrapper.MinimizeWindow();
							handled = true;
						}

						//Command+Q. Quit.
						if( !handled && eKey == EKeys.Q && viewport.IsKeyPressed( EKeys.Command ) )
						{
							EngineApp.NeedExit = true;
							break;
						}

						if( keyEvent.SuppressKeyPress )
							suppressKeyPress = true;
					}

					if( !suppressKeyPress && !MacAppNativeWrapper.IsSystemKey( eKey ) )
					{
						KeyPressEvent keyPressEvent = new KeyPressEvent( (char)character );
						viewport.PerformKeyPress( keyPressEvent, ref handled );
					}

					if( handled )
					{
					}
				}
				break;

			case MessageTypes.KeyUp:
				{
					var eKey = (EKeys)parameterA;

					bool handled = false;

					if( eKey != 0 )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						viewport.PerformKeyUp( keyEvent, ref handled );

						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
						{
							keyEvent = new KeyEvent( EKeys.Shift );
							viewport.PerformKeyUp( keyEvent, ref handled );
						}
						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
						{
							keyEvent = new KeyEvent( EKeys.Control );
							viewport.PerformKeyUp( keyEvent, ref handled );
						}
						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
						{
							keyEvent = new KeyEvent( EKeys.Alt );
							viewport.PerformKeyUp( keyEvent, ref handled );
						}
						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
						{
							keyEvent = new KeyEvent( EKeys.Command );
							viewport.PerformKeyUp( keyEvent, ref handled );
						}

					}

					if( handled )
					{
					}
				}
				break;

			case MessageTypes.WindowDidResize:
				{
					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode && !instance.goingToChangeWindowRectangle )
					{
						EngineApp.CreatedWindowProcessResize();
					}
				}
				break;

			case MessageTypes.WindowDidBecomeKey:
				{
					//activated

					instance.CreatedWindow_UpdateShowSystemCursor( true );
					//instance.UpdateShowSystemCursor();
				}
				break;

			case MessageTypes.WindowDidResignKey:
				{
					//deactivated

					EngineApp.EnginePauseUpdateState( true, true );
					//if( App.SuspendWorkingWhenApplicationIsNotActive )
					//	App.DoSystemPause( true, true );

					instance.CreatedWindow_UpdateShowSystemCursor( true );
					//instance.UpdateShowSystemCursor();
				}
				break;

			case MessageTypes.WindowWillMiniaturize:
				{
				}
				break;

			case MessageTypes.WindowDidMiniaturize:
				{
					EngineApp.EnginePauseUpdateState( true, true );
					//if( App.SuspendWorkingWhenApplicationIsNotActive )
					//	App.DoSystemPause( true, true );
				}
				break;

			case MessageTypes.WindowDidDeminiaturize:
				{
					if( instance.fullscreenMinimizedMode && EngineApp.WindowedMode != WindowedModeEnum.Windowed )
					{
						instance.fullscreenMinimizedMode = false;

						MacAppNativeWrapper.RestoreFromFullscreenMinimizedMode( viewport.SizeInPixels.X, viewport.SizeInPixels.Y );

						//MacAppNativeWrapper.RestoreFromFullscreenMinimizedMode( App.VideoMode.X, App.VideoMode.Y );
					}
				}
				break;

			case MessageTypes.Periodic:
				{
					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
					{
						if( !IsAllowApplicationIdle() )
							EngineApp.CreatedWindowApplicationIdle( true );
					}

					//Alt+Tab
					if( instance.mustGoToFullscreenMinimizedMode )
					{
						if( instance.mustGoToFullscreenMinimizedModeStep == 1 )
						{
							MacAppNativeWrapper.ActivateFullscreenMinimizedMode();
							instance.mustGoToFullscreenMinimizedModeStep = 2;
						}
						else if( instance.mustGoToFullscreenMinimizedModeStep == 2 )
						{
							if( instance.GetWindowState() != WindowState.Minimized )
							{
								MacAppNativeWrapper.MinimizeWindow();

								if( instance.GetWindowState() == WindowState.Minimized )
								{
									instance.mustGoToFullscreenMinimizedMode = false;
									instance.fullscreenMinimizedMode = true;
								}
							}
						}
					}
				}
				break;
			}
		}

		public override void CreatedWindow_RunMessageLoop()
		{
			while( MacAppNativeWrapper.ProcessEvents() )
			{
				if( EngineApp.NeedExit )
					break;

				if( IsAllowApplicationIdle() )
				{
					if( EngineApp.RenderVideoToFileData == null )
						EngineApp.UpdateEngineTime();
					double time = EngineApp.EngineTime;
					bool needSleep = EngineApp.MaxFPS != 0 && time < maxFPSLastRenderTime + 1.0f / EngineApp.MaxFPS;

					if( needSleep )
					{
						Thread.Sleep( 1 );
					}
					else
					{
						maxFPSLastRenderTime = time;

						//finish switching to another windowed mode
						var goingToAnotherWindowedMode2 = goingToAnotherWindowedMode;
						if( goingToAnotherWindowedMode2 != null )
						{

							//!!!!

							////windowed
							//if( goingToAnotherWindowedMode2.Value == WindowedModeEnum.Windowed )
							//{
							//	SetWindowBorderStyle( WindowBorderStyle.Sizeable );

							//	var pos = ( GetScreenSize() - EngineApp.WindowedModeSize ) / 2;
							//	var size = EngineApp.WindowedModeSize - new Vector2I( 1, 1 );
							//	SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_NOTOPMOST, pos.X, pos.Y, size.X, size.Y, SWP_FRAMECHANGED | SWP_SHOWWINDOW );
							//}

							////borderless
							//if( goingToAnotherWindowedMode2.Value == WindowedModeEnum.Borderless )
							//{
							//	SetWindowBorderStyle( WindowBorderStyle.None );

							//	var monitorRect = GetWindowMonitorRectangle();

							//	SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_NOTOPMOST, monitorRect.Left, monitorRect.Top, monitorRect.Size.X, monitorRect.Size.Y, SWP_FRAMECHANGED | SWP_SHOWWINDOW );

							//	//SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_NOTOPMOST, 0, 0, EngineApp.WindowedModeSize.X, EngineApp.WindowedModeSize.Y, SWP_FRAMECHANGED | SWP_SHOWWINDOW );
							//}

							////fullscreen
							//if( goingToAnotherWindowedMode2.Value == WindowedModeEnum.Fullscreen )
							//{
							//	SetWindowBorderStyle( WindowBorderStyle.None );

							//	var monitorRect = GetWindowMonitorRectangle();

							//	SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_TOPMOST, monitorRect.Left, monitorRect.Top, monitorRect.Size.X, monitorRect.Size.Y, SWP_FRAMECHANGED | SWP_SHOWWINDOW );

							//	//SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_TOPMOST, 0, 0, EngineApp.WindowedModeSize.X, EngineApp.WindowedModeSize.Y, SWP_FRAMECHANGED | SWP_SHOWWINDOW );
							//}

							////SetWindowState( WindowState.Normal );
							//SetForegroundWindow( EngineApp.ApplicationWindowHandle );
							//SetFocus( EngineApp.ApplicationWindowHandle );

							//goingToAnotherWindowedMode = null;
							//EngineApp.CreatedWindowProcessResize();

						}


						EngineApp.CreatedWindowApplicationIdle( false );

						//!!!!useful?
						//if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
						//{
						//	float time = EngineApp.Instance.Time;
						//	bool needSleep = EngineApp.Instance.MaxFPS != 0 && time < maxFPSLastRenderTime + 1.0f / EngineApp.Instance.MaxFPS;

						//	if( needSleep )
						//	{
						//		Thread.Sleep( 1 );
						//	}
						//	else
						//	{
						//		maxFPSLastRenderTime = time;
						//		App.ApplicationIdle( false );
						//	}
						//}
					}
				}
				else
					Thread.Sleep( 50 );
			}
		}

		static bool IsAllowApplicationIdle()
		{
			bool needIdle = true;

			if( EngineApp.EnginePauseWhenApplicationIsNotActive )
			{
				if( instance.GetWindowState() == WindowState.Minimized )
					needIdle = false;
			}

			if( EngineApp.WindowedMode != WindowedModeEnum.Windowed && !MacAppNativeWrapper.IsWindowActive() )
				needIdle = false;
			//if( App.FullScreen && !instance.IsWindowActive() )
			//	needIdle = false;

			if( EngineApp.EnginePaused )
				needIdle = false;

			return needIdle;
		}

		static void LogInfo( IntPtr/*char* */text )
		{
			var result = MacAppNativeWrapper.GetOutString( text );
			Log.Info( result );
		}

		static void LogWarning( IntPtr/*char* */text )
		{
			var result = MacAppNativeWrapper.GetOutString( text );
			Log.Warning( result );
		}

		static void LogFatal( IntPtr/*char* */text )
		{
			var result = MacAppNativeWrapper.GetOutString( text );
			Log.Fatal( result );
		}

		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
		{
			bool show = EngineApp.IsRealShowSystemCursor();

			if( createdWindow_UpdateShowSystemCursor != show || forceUpdate )
			{
				createdWindow_UpdateShowSystemCursor = show;
				MacAppNativeWrapper.ShowSystemCursor( show );
			}
		}

		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
		{
			Log.Fatal( "MacOSPlatformFunctionality: ShowMessageBoxYesNoQuestion: method is not implemented." );
			return false;

			//int result = MessageBox( EngineApp.ApplicationWindowHandle, text, caption, MB_YESNO | MB_ICONEXCLAMATION );
			//if( result == IDYES )
			//	return true;
			//return false;

		}

		IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param )
		{
			return MacAppNativeWrapper.CallCustomPlatformSpecificMethod( message, param );
		}

		public override InputDeviceManager CreateInputDeviceManager()
		{
			if( EngineApp.CreatedInsideEngineWindow != null )
				return new MacOSInputDeviceManager( EngineApp.CreatedInsideEngineWindow.Handle );
			else
				return null;
		}
	}
}