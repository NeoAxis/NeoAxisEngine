//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Drawing;
//using System.Runtime.InteropServices;
//using System.Threading;

//namespace NeoAxis
//{
//	class MacOSXPlatformFunctionality : PlatformFunctionality
//	{
//		static MacOSXPlatformFunctionality instance;

//		unsafe static MacAppNativeWrapper.CallbackMessageEvent messageEventdelegate = MessageEvent;
//		unsafe static MacAppNativeWrapper.CallbackLogInfo logInfoDelegate = LogInfo;
//		unsafe static MacAppNativeWrapper.CallbackLogWarning logWarningDelegate = LogWarning;
//		unsafe static MacAppNativeWrapper.CallbackLogFatal logFatalDelegate = LogFatal;

//		bool mustGoToFullscreenMinimizedMode;
//		int mustGoToFullscreenMinimizedModeStep;
//		bool fullscreenMinimizedMode;

//		bool goingToChangeWindowRectangle;

//		double maxFPSLastRenderTime;

//		///////////////////////////////////////////

//		struct Wrapper
//		{
//			public const string library = "NeoAxisCoreNative";
//			public const CallingConvention convention = CallingConvention.Cdecl;
//		}

//		internal struct MacAppNativeWrapper
//		{
//			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MessageBox", CallingConvention = Wrapper.convention,
//			//   CharSet = CharSet.Unicode )]
//			//public unsafe static extern void MessageBox( string text, string caption );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeOut", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void FullscreenFadeOut( [MarshalAs( UnmanagedType.U1 )]  bool exitApplication );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeIn", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void FullscreenFadeIn( [MarshalAs( UnmanagedType.U1 )]  bool exitApplication );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_InitApplicationWindow", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern IntPtr InitApplicationWindow( [MarshalAs( UnmanagedType.U1 )] bool fullscreen,
//				int windowSizeX, int windowSizeY, string title );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowVisible", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsWindowVisible();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowActive", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsWindowActive();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowFocused", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsWindowFocused();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowRectangle", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetWindowRectangle( out RectangleI rect );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowClientRect", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetWindowClientRect( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out RectangleI rect );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out int x, out int y );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void SetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, int x, int y );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetSystemTime", CallingConvention = Wrapper.convention )]
//			public unsafe static extern double GetSystemTime();
//			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTopMost", CallingConvention = Wrapper.convention )]
//			//public unsafe static extern void SetWindowTopMost( [MarshalAs( UnmanagedType.U1 )] bool value );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenSize", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetScreenSize( out int width, out int height );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenBitsPerPixel", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetScreenBitsPerPixel();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ProcessEvents", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool ProcessEvents();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShutdownApplicationWindow", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void ShutdownApplicationWindow();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowState", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetWindowState();
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowState", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void SetWindowState( int state );

//			[UnmanagedFunctionPointer( Wrapper.convention )]
//			public unsafe delegate void CallbackMessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC );

//			[UnmanagedFunctionPointer( Wrapper.convention )]
//			public unsafe delegate void CallbackLogInfo( IntPtr/*char* */text );
//			[UnmanagedFunctionPointer( Wrapper.convention )]
//			public unsafe delegate void CallbackLogWarning( IntPtr/*char* */text );
//			[UnmanagedFunctionPointer( Wrapper.convention )]
//			public unsafe delegate void CallbackLogFatal( IntPtr/*char* */text );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_Initialize", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool Initialize( CallbackMessageEvent messageEvent, CallbackLogInfo logInfo,
//				CallbackLogWarning logWarning, CallbackLogFatal logFatal );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ChangeVideoMode", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool ChangeVideoMode( int width, int height, int bpp );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreVideoMode", CallingConvention = Wrapper.convention )]
//			public unsafe static extern bool RestoreVideoMode();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetVideoModes", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetVideoModes( out int count, out Vector3I* array );

//			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayCount", CallingConvention = Wrapper.convention )]
//			//public unsafe static extern int GetDisplayCount();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShowSystemCursor", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void ShowSystemCursor( [MarshalAs( UnmanagedType.U1 )] bool show );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyPressed", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsKeyPressed( EKeys eKey );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyLocked", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsKeyLocked( EKeys eKey );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsSystemKey", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsSystemKey( EKeys eKey );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsMouseButtonPressed", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool IsMouseButtonPressed( int buttonCode );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeMemory", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void FreeMemory( IntPtr buffer );
//			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowPosition", CallingConvention = Wrapper.convention )]
//			//public unsafe static extern void SetWindowPosition( int x, int y );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowSize", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void SetWindowSize( int width, int height );
//			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowBorderStyle", CallingConvention = Wrapper.convention )]
//			//public unsafe static extern void SetWindowBorderStyle( int style );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowRectangle", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void SetWindowRectangle( int left, int top, int right, int bottom );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTitle", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern void SetWindowTitle( string title );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern void GetMouseMoveDelta( out int x, out int y );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ResetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern void ResetMouseMoveDelta( [MarshalAs( UnmanagedType.U1 )] bool resetIgnoreCounter );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_CallCustomPlatformSpecificMethod", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetLoadedBundleNames", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern void GetLoadedBundleNames( out IntPtr* list, out int count );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateAcceptsMouseMovedEventsFlag", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//			public unsafe static extern void UpdateAcceptsMouseMovedEventsFlag();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateWindowForProcessChangingVideoMode", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void UpdateWindowForProcessChangingVideoMode();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetGamma", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void SetGamma( float value );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeOutString", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void FreeOutString( IntPtr pointer );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ActivateFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void ActivateFullscreenMinimizedMode();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreFromFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void RestoreFromFullscreenMinimizedMode( int width, int height );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MinimizeWindow", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void MinimizeWindow();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetActiveDisplayList", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetActiveDisplayList( int bufferLength, uint* buffer );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayInfo", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetDisplayInfo( uint display, out IntPtr deviceName,
//				out RectangleI bounds, out RectangleI workingArea, [MarshalAs( UnmanagedType.U1 )] out bool primary );

//			public static string GetOutString( IntPtr pointer )
//			{
//				if( pointer != IntPtr.Zero )
//				{
//					string result = Marshal.PtrToStringUni( pointer );
//					FreeOutString( pointer );
//					return result;
//				}
//				else
//					return null;
//			}
//		}

//		public enum MessageTypes
//		{
//			MouseDown,
//			MouseUp,
//			MouseDoubleClick,
//			MouseWheel,
//			MouseMove,
//			KeyDown,
//			KeyUp,
//			WindowDidResize,
//			WindowDidBecomeKey,
//			WindowDidResignKey,
//			WindowWillMiniaturize,
//			WindowDidMiniaturize,
//			WindowDidDeminiaturize,
//			Periodic,
//		}

//		///////////////////////////////////////////

//		//public static EngineApp App
//		//{
//		//	get { return EngineApp.Instance; }
//		//}

//		unsafe public MacOSXPlatformFunctionality()
//		{
//			instance = this;
//			MacAppNativeWrapper.Initialize( messageEventdelegate, logInfoDelegate, logWarningDelegate,
//				logFatalDelegate );
//		}

//		public override bool ChangeVideoMode( Vector2I mode )
//		{
//			return MacAppNativeWrapper.ChangeVideoMode( mode.X, mode.Y, GetScreenBitsPerPixel() );
//		}

//		public override void RestoreVideoMode()
//		{
//			MacAppNativeWrapper.RestoreVideoMode();
//		}

//		public override void FullscreenFadeOut( bool exitApplication )
//		{
//			MacAppNativeWrapper.FullscreenFadeOut( exitApplication );
//		}

//		public override void FullscreenFadeIn( bool exitApplication )
//		{
//			MacAppNativeWrapper.FullscreenFadeIn( exitApplication );
//		}

//		public override System.IntPtr CreatedWindow_CreateWindow()
//		{
//			//!!!!!положение не указывается
//			//!!!!!стейты тоже
//			IntPtr handle = MacAppNativeWrapper.InitApplicationWindow( EngineApp.FullscreenEnabled,
//				EngineApp.InitSettings.CreateWindowSize.Value.X, EngineApp.InitSettings.CreateWindowSize.Value.Y,
//				EngineApp.CreatedInsideEngineWindow.Title );
//			return handle;
//		}

//		public override void CreatedWindow_DestroyWindow()
//		{
//			MacAppNativeWrapper.ShutdownApplicationWindow();
//		}

//		public override void CreatedWindow_ProcessMessageEvents()
//		{
//		}

//		public override List<Vector2I> GetVideoModes()
//		{
//			List<Vector2I> videoModes = new List<Vector2I>();

//			int bpp = GetScreenBitsPerPixel();

//			unsafe
//			{
//				try
//				{
//					int count;
//					Vector3I* array;
//					MacAppNativeWrapper.GetVideoModes( out count, out array );

//					if( count != 0 )
//					{
//						for( int n = 0; n < count; n++ )
//						{
//							Vector3I item = array[ n ];

//							Vector2I mode = item.ToVector2I();
//							int modeBPP = item.Z;

//							if( bpp == modeBPP )
//							{
//								if( !videoModes.Contains( mode ) )
//									videoModes.Add( mode );
//							}
//						}
//					}

//					if( array != null )
//						MacAppNativeWrapper.FreeMemory( (IntPtr)array );
//				}
//				catch { }
//			}

//			return videoModes;
//		}

//		public override RectangleI CreatedWindow_GetClientRectangle()
//		{
//			RectangleI result;
//			MacAppNativeWrapper.GetWindowClientRect( EngineApp.FullscreenEnabled, out result );
//			return result;
//		}

//		public override Vector2 CreatedWindow_GetMousePosition()
//		{
//			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
//				return Vector2F.Zero;

//			int x, y;
//			MacAppNativeWrapper.GetClientRectangleCursorPosition( EngineApp.FullscreenEnabled, out x, out y );

//			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
//			return new Vector2F(
//				(float)x / (float)( viewport.SizeInPixels.X - viewport.SizeInPixels.X % 2 ),
//				(float)y / (float)( viewport.SizeInPixels.Y - viewport.SizeInPixels.Y % 2 ) );
//		}

//		public override void CreatedWindow_SetMousePosition( Vector2 value )
//		{
//			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
//			{
//				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
//				Vector2I position = new Vector2I(
//					(int)(float)( value.X * (float)viewport.SizeInPixels.X ),
//					(int)(float)( value.Y * (float)viewport.SizeInPixels.Y ) );
//				MacAppNativeWrapper.SetClientRectangleCursorPosition( EngineApp.FullscreenEnabled, position.X, position.Y );
//			}
//		}

//		//public override int GetMonitorCount()
//		//{
//		//   return MacAppNativeWrapper.GetDisplayCount();
//		//}

//		public unsafe override string[] GetNativeModuleNames()
//		{
//			IntPtr* list;
//			int count;
//			MacAppNativeWrapper.GetLoadedBundleNames( out list, out count );

//			string[] result = new string[ count ];

//			for( int n = 0; n < count; n++ )
//			{
//				IntPtr pointer = list[ n ];
//				string name = MacAppNativeWrapper.GetOutString( pointer );
//				result[ n ] = name;
//			}

//			MacAppNativeWrapper.FreeMemory( (IntPtr)list );

//			return result;
//		}

//		public override Vector2I GetScreenSize()
//		{
//			int width, height;
//			MacAppNativeWrapper.GetScreenSize( out width, out height );
//			return new Vector2I( width, height );
//		}

//		public override Vector2I GetSmallIconSize()
//		{
//			return new Vector2I( 0, 0 );
//		}

//		public override double GetSystemTime()
//		{
//			return MacAppNativeWrapper.GetSystemTime();
//		}

//		public override RectangleI CreatedWindow_GetWindowRectangle()
//		{
//			RectangleI result;
//			MacAppNativeWrapper.GetWindowRectangle( out result );
//			return result;
//		}

//		public override WindowState GetWindowState()
//		{
//			return (WindowState)MacAppNativeWrapper.GetWindowState();
//		}

//		public override bool CreatedWindow_IsWindowActive()
//		{
//			return MacAppNativeWrapper.IsWindowActive();
//		}

//		public override bool IsWindowVisible()
//		{
//			return MacAppNativeWrapper.IsWindowVisible();
//		}

//		public override void MessageLoopWaitMessage()
//		{
//			CreatedWindow_ProcessMessageEvents();
//		}

//		unsafe static void MessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC )
//		{
//			//!!!!!применить в маке if( Log.FatalActivated ) также как в винде

//			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

//			switch( messageType )
//			{
//			case MessageTypes.MouseDown:
//				{
//					bool handled = false;
//					viewport.PerformMouseDown( (EMouseButtons)parameterA, ref handled );
//					break;
//				}
//			case MessageTypes.MouseUp:
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( (EMouseButtons)parameterA, ref handled );
//					break;
//				}
//			case MessageTypes.MouseDoubleClick:
//				{
//					bool handled = false;
//					viewport.PerformMouseDoubleClick( (EMouseButtons)parameterA, ref handled );
//					break;
//				}
//			case MessageTypes.MouseWheel:
//				{
//					bool handled = false;
//					viewport.PerformMouseWheel( (int)parameterA, ref handled );
//					break;
//				}
//			case MessageTypes.MouseMove:
//				EngineApp.CreatedInsideEngineWindow._ProcessMouseMoveEvent();
//				break;

//			case MessageTypes.KeyDown:
//				{
//					EKeys eKey = (EKeys)parameterA;
//					int character = parameterB;

//					bool handled = false;
//					bool suppressKeyPress = false;

//					if( eKey != (EKeys)0 )
//					{
//						KeyEvent keyEvent = new KeyEvent( eKey );
//						viewport.PerformKeyDown( keyEvent, ref handled );

//						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
//						{
//							keyEvent = new KeyEvent( EKeys.Shift );
//							viewport.PerformKeyDown( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
//						{
//							keyEvent = new KeyEvent( EKeys.Control );
//							viewport.PerformKeyDown( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
//						{
//							keyEvent = new KeyEvent( EKeys.Alt );
//							viewport.PerformKeyDown( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
//						{
//							keyEvent = new KeyEvent( EKeys.Command );
//							viewport.PerformKeyDown( keyEvent, ref handled );
//						}

//						//Cmd-Tab. Minimize for fullscreen mode.
//						if( !handled && eKey == EKeys.Tab && viewport.IsKeyPressed( EKeys.Command ) )
//						{
//							if( EngineApp.FullscreenEnabled )
//							{
//								if( !instance.fullscreenMinimizedMode )
//								{
//									instance.mustGoToFullscreenMinimizedMode = true;
//									instance.mustGoToFullscreenMinimizedModeStep = 1;
//									break;
//								}
//								handled = true;
//							}
//						}

//						//Command+M. Minimize.
//						if( !handled && eKey == EKeys.M && viewport.IsKeyPressed( EKeys.Command ) )
//						{
//							if( EngineApp.FullscreenEnabled )
//							{
//								if( !instance.fullscreenMinimizedMode )
//								{
//									instance.mustGoToFullscreenMinimizedMode = true;
//									instance.mustGoToFullscreenMinimizedModeStep = 1;
//									break;
//								}
//							}
//							else
//								MacAppNativeWrapper.MinimizeWindow();
//							handled = true;
//						}

//						//Command+Q. Quit.
//						if( !handled && eKey == EKeys.Q && viewport.IsKeyPressed( EKeys.Command ) )
//						{
//							EngineApp.NeedExit = true;
//							break;
//						}

//						if( keyEvent.SuppressKeyPress )
//							suppressKeyPress = true;
//					}

//					if( !suppressKeyPress && !MacAppNativeWrapper.IsSystemKey( eKey ) )
//					{
//						KeyPressEvent keyPressEvent = new KeyPressEvent( (char)character );
//						viewport.PerformKeyPress( keyPressEvent, ref handled );
//					}

//					if( handled )
//					{
//					}
//				}
//				break;

//			case MessageTypes.KeyUp:
//				{
//					EKeys eKey = (EKeys)parameterA;

//					bool handled = false;

//					if( eKey != (EKeys)0 )
//					{
//						KeyEvent keyEvent = new KeyEvent( eKey );
//						viewport.PerformKeyUp( keyEvent, ref handled );

//						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
//						{
//							keyEvent = new KeyEvent( EKeys.Shift );
//							viewport.PerformKeyUp( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
//						{
//							keyEvent = new KeyEvent( EKeys.Control );
//							viewport.PerformKeyUp( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
//						{
//							keyEvent = new KeyEvent( EKeys.Alt );
//							viewport.PerformKeyUp( keyEvent, ref handled );
//						}
//						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
//						{
//							keyEvent = new KeyEvent( EKeys.Command );
//							viewport.PerformKeyUp( keyEvent, ref handled );
//						}

//					}

//					if( handled )
//					{
//					}
//				}
//				break;

//			case MessageTypes.WindowDidResize:
//				{
//					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode &&
//						!instance.goingToChangeWindowRectangle )
//					{
//						EngineApp._CreatedWindow_ProcessResize();
//					}
//				}
//				break;

//			case MessageTypes.WindowDidBecomeKey:
//				{
//					//activated

//					instance.CreatedWindow_UpdateShowSystemCursor( true );
//				}
//				break;

//			case MessageTypes.WindowDidResignKey:
//				{
//					//deactivated

//					if( EngineApp.EnginePauseWhenApplicationIsNotActive )
//						EngineApp._EnginePause_UpdateState( true, true );
//					//App.DoSystemPause( true, true );

//					instance.CreatedWindow_UpdateShowSystemCursor( true );
//				}
//				break;

//			case MessageTypes.WindowWillMiniaturize:
//				{
//				}
//				break;

//			case MessageTypes.WindowDidMiniaturize:
//				{
//					if( EngineApp.EnginePauseWhenApplicationIsNotActive )
//						EngineApp._EnginePause_UpdateState( true, true );
//					//App.DoSystemPause( true, true );
//				}
//				break;

//			case MessageTypes.WindowDidDeminiaturize:
//				{
//					if( instance.fullscreenMinimizedMode && EngineApp.FullscreenEnabled )
//					{
//						instance.fullscreenMinimizedMode = false;
//						//!!!!так?
//						MacAppNativeWrapper.RestoreFromFullscreenMinimizedMode( viewport.SizeInPixels.X, viewport.SizeInPixels.Y );
//						//MacAppNativeWrapper.RestoreFromFullscreenMinimizedMode( App.VideoMode.X, App.VideoMode.Y );
//					}
//				}
//				break;

//			case MessageTypes.Periodic:
//				{
//					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
//					{
//						if( !IsAllowApplicationIdle() )
//							EngineApp._CreatedWindow_ApplicationIdle( true );
//					}

//					//Alt+Tab
//					if( instance.mustGoToFullscreenMinimizedMode )
//					{
//						if( instance.mustGoToFullscreenMinimizedModeStep == 1 )
//						{
//							MacAppNativeWrapper.ActivateFullscreenMinimizedMode();
//							instance.mustGoToFullscreenMinimizedModeStep = 2;
//						}
//						else if( instance.mustGoToFullscreenMinimizedModeStep == 2 )
//						{
//							if( instance.GetWindowState() != WindowState.Minimized )
//							{
//								MacAppNativeWrapper.MinimizeWindow();

//								if( instance.GetWindowState() == WindowState.Minimized )
//								{
//									instance.mustGoToFullscreenMinimizedMode = false;
//									instance.fullscreenMinimizedMode = true;
//								}
//							}
//						}

//						break;
//					}

//				}
//				break;

//			}
//		}

//		public override void CreatedWindow_RunMessageLoop()
//		{
//			while( MacAppNativeWrapper.ProcessEvents() )
//			{
//				if( EngineApp.NeedExit )
//					break;

//				if( IsAllowApplicationIdle() )
//				{
//					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
//					{
//						double time = EngineApp.EngineTime;
//						bool needSleep = EngineApp.MaxFPS != 0 &&
//							time < maxFPSLastRenderTime + 1.0f / EngineApp.MaxFPS;

//						if( needSleep )
//						{
//							Thread.Sleep( 1 );
//						}
//						else
//						{
//							maxFPSLastRenderTime = time;
//							EngineApp._CreatedWindow_ApplicationIdle( false );
//						}
//					}
//				}
//				else
//					System.Threading.Thread.Sleep( 50 );
//			}
//		}

//		static bool IsAllowApplicationIdle()
//		{
//			bool needIdle = true;

//			if( instance.GetWindowState() == WindowState.Minimized )
//				needIdle = false;

//			if( EngineApp.FullscreenEnabled && !instance.CreatedWindow_IsWindowActive() )
//				needIdle = false;

//			if( EngineApp.EnginePaused )
//				needIdle = false;

//			return needIdle;
//		}

//		public override bool IsIntoMenuLoop()
//		{
//			return false;
//		}

//		unsafe static void LogInfo( IntPtr/*char* */text )
//		{
//			string result = MacAppNativeWrapper.GetOutString( text );
//			Log.Info( result );
//		}

//		unsafe static void LogWarning( IntPtr/*char* */text )
//		{
//			string result = MacAppNativeWrapper.GetOutString( text );
//			Log.Warning( result );
//		}

//		unsafe static void LogFatal( IntPtr/*char* */text )
//		{
//			string result = MacAppNativeWrapper.GetOutString( text );
//			Log.Fatal( result );
//		}

//		public override void SetGamma( float value )
//		{
//			MacAppNativeWrapper.SetGamma( value );
//		}

//		public override void CreatedWindow_SetWindowSize( Vector2I size )
//		{
//			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
//				MacAppNativeWrapper.SetWindowSize( size.X, size.Y );
//		}

//		public override void SetWindowState( WindowState value )
//		{
//			//the method is used only for Engine.WindowState set.

//			if( EngineApp.FullscreenEnabled && value == WindowState.Minimized )
//			{
//				if( !instance.fullscreenMinimizedMode )
//				{
//					instance.mustGoToFullscreenMinimizedMode = true;
//					instance.mustGoToFullscreenMinimizedModeStep = 1;
//				}
//				return;
//			}

//			MacAppNativeWrapper.SetWindowState( (int)value );
//		}

//		public override void CreatedWindow_UpdateInputDevices()
//		{
//			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

//			//!!!!!

//			MacAppNativeWrapper.UpdateAcceptsMouseMovedEventsFlag();

//			if( MacAppNativeWrapper.IsWindowFocused() && new Rectangle( 0, 0, 1, 1 ).Contains( viewport.MousePosition ) )
//				CreatedWindow_UpdateShowSystemCursor( false );

//			//mouse
//			{
//				if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) &&
//					!MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Left ) )
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
//				}
//				if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) &&
//					!MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Right ) )
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
//				}
//				if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) &&
//					!MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Middle ) )
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
//				}
//				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton1 ) &&
//					!MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton1 ) )
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
//				}
//				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton2 ) &&
//					!MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton2 ) )
//				{
//					bool handled = false;
//					viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
//				}
//			}

//			//keys
//			foreach( EKeys eKey in Viewport.AllKeys )
//			{
//				if( viewport.IsKeyPressed( eKey ) && eKey != EKeys.Shift && eKey != EKeys.Control &&
//					eKey != EKeys.Alt && eKey != EKeys.Command )
//				{
//					if( !MacAppNativeWrapper.IsKeyPressed( eKey ) )
//					{
//						KeyEvent keyEvent = new KeyEvent( eKey );
//						bool handled = false;
//						viewport.PerformKeyUp( keyEvent, ref handled );
//					}
//				}
//			}

//			//mouse relative mode
//			if( viewport.MouseRelativeMode )
//				viewport.MousePosition = new Vector2F( .5f, .5f );
//		}

//		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
//		{
//			bool show = EngineApp._IsRealShowSystemCursor();
//			MacAppNativeWrapper.ShowSystemCursor( show );
//		}

//		public override void CreatedWindow_UpdateSystemCursorFileName()
//		{
//		}

//		public override void CreatedWindow_UpdateWindowIcon( Icon smallIcon, Icon icon )
//		{
//		}

//		public override bool IsWindowInitialized()
//		{
//			return EngineApp.ApplicationWindowHandle != IntPtr.Zero;
//		}

//		public override void CreatedWindow_UpdateWindowTitle( string title )
//		{
//			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
//				MacAppNativeWrapper.SetWindowTitle( title );
//		}

//		public override bool IsFocused()
//		{
//			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
//				return false;
//			return MacAppNativeWrapper.IsWindowFocused();
//		}

//		public override bool ApplicationIsActivated()
//		{
//			//!!!!
//			Log.Fatal( "impl" );
//			return true;
//		}

//		public override int GetScreenBitsPerPixel()
//		{
//			return MacAppNativeWrapper.GetScreenBitsPerPixel();
//		}

//		public override bool IsKeyLocked( EKeys key )
//		{
//			return MacAppNativeWrapper.IsKeyLocked( key );
//		}

//		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
//		{
//			Log.Fatal( "MacOSXPlatformFunctionality: ShowMessageBoxYesNoQuestion: method is not implemented." );
//			return false;
//		}

//		public override void CreatedWindow_OnMouseRelativeModeChange()
//		{
//			MacAppNativeWrapper.ResetMouseMoveDelta( true );
//		}

//		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
//		{
//			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

//			int x, y;
//			MacAppNativeWrapper.GetMouseMoveDelta( out x, out y );
//			if( viewport.SizeInPixels.X != 0 && viewport.SizeInPixels.Y != 0 )
//				delta = new Vector2F( x, y ) / viewport.SizeInPixels.ToVector2F();
//			else
//				delta = Vector2F.Zero;
//			MacAppNativeWrapper.ResetMouseMoveDelta( false );
//		}

//		public override IntPtr CallSpecialPlatformSpecificMethod( string message, IntPtr param )
//		{
//			return MacAppNativeWrapper.CallCustomPlatformSpecificMethod( message, param );
//		}

//		public override void ProcessChangingVideoMode()
//		{
//			//change video mode
//			if( EngineApp.FullscreenEnabled )
//			{
//				if( !ChangeVideoMode( EngineApp.FullscreenSize ) )
//					return;
//				//App.lastFullScreenWindowSize = App.VideoMode;
//			}
//			else
//			{
//				RestoreVideoMode();
//			}

//			MacAppNativeWrapper.UpdateWindowForProcessChangingVideoMode();

//			EngineApp._CreatedWindow_ProcessResize();
//		}

//		unsafe public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
//		{
//			List<SystemSettings.DisplayInfo> result = new List<SystemSettings.DisplayInfo>();

//			uint[] buffer = new uint[ 256 ];
//			int count;

//			fixed ( uint* pBuffer = buffer )
//			{
//				count = MacAppNativeWrapper.GetActiveDisplayList( buffer.Length, pBuffer );
//			}

//			for( int n = 0; n < count; n++ )
//			{
//				uint display = buffer[ n ];

//				IntPtr deviceNamePointer;
//				RectangleI bounds;
//				RectangleI workingArea;
//				bool primary;

//				MacAppNativeWrapper.GetDisplayInfo( display, out deviceNamePointer, out bounds,
//					out workingArea, out primary );

//				string deviceName = MacAppNativeWrapper.GetOutString( deviceNamePointer );

//				SystemSettings.DisplayInfo displayInfo = new SystemSettings.DisplayInfo( deviceName, bounds, workingArea, primary );
//				result.Add( displayInfo );
//			}

//			if( result.Count == 0 )
//			{
//				RectangleI area = new RectangleI( Vector2I.Zero, GetScreenSize() );
//				SystemSettings.DisplayInfo info = new SystemSettings.DisplayInfo( "Primary", area, area, true );
//				result.Add( info );
//			}

//			return result;
//		}

//		public override void CreatedWindow_SetWindowRectangle( RectangleI rectangle )
//		{
//			goingToChangeWindowRectangle = true;
//			MacAppNativeWrapper.SetWindowRectangle( rectangle.Left, rectangle.Top, rectangle.Right,
//				rectangle.Bottom );
//			goingToChangeWindowRectangle = false;

//			EngineApp._CreatedWindow_ProcessResize();
//		}

//		public override void GetSystemLanguage( out string name, out string englishName )
//		{
//			name = MacAppNativeWrapper.GetOutString(
//				CallSpecialPlatformSpecificMethod( "GetSystemLanguageName", IntPtr.Zero ) );
//			englishName = MacAppNativeWrapper.GetOutString(
//				CallSpecialPlatformSpecificMethod( "GetSystemLanguageEnglishName", IntPtr.Zero ) );
//		}
//	}
//}
