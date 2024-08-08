
#if _

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !ANDROID && !IOS && !WEB && !UWP
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



//!!!!use from from 3.5 below



namespace Internal
{
	class MacOSPlatformFunctionality : PlatformFunctionality
	{
		static MacOSPlatformFunctionality instance;

		qqqq;

		KeyInfo[] keysInfo = new KeyInfo[ EngineApp.GetEKeysMaxIndex() + 1 ];

		bool suspendModeTimerCreated;

		bool intoMenuLoop;
		bool resizingMoving;

		IntPtr hCursorArrow;

		Dictionary<string, IntPtr> loadedSystemCursors = new Dictionary<string, IntPtr>();

		Vector2 lastMousePositionForMouseMoveDelta;
		Vector2 lastMousePositionForCheckMouseOutsideWindow;
		bool mustIgnoreOneMouseMoveAtRelativeMode;

		double maxFPSLastRenderTime;

		bool goingToWindowedMode;
		bool goingToFullScreenMode;
		bool goingToChangeWindowRectangle;

		static List<SystemSettings.DisplayInfo> tempScreenList = new List<SystemSettings.DisplayInfo>();

		bool createdWindow_UpdateShowSystemCursor;

		///////////////////////////////////////////

		qqqq;


		unsafe static MacAppNativeWrapper.CallbackMessageEvent messageEventdelegate = MessageEvent;
		unsafe static MacAppNativeWrapper.CallbackLogInfo logInfoDelegate = LogInfo;
		unsafe static MacAppNativeWrapper.CallbackLogWarning logWarningDelegate = LogWarning;
		unsafe static MacAppNativeWrapper.CallbackLogFatal logFatalDelegate = LogFatal;

		bool mustGoToFullscreenMinimizedMode;
		int mustGoToFullscreenMinimizedModeStep;
		bool fullscreenMinimizedMode;

		///////////////////////////////////////////

		struct Wrapper
		{
			public const string library = "NeoAxisCoreNative";
			public const CallingConvention convention = CallingConvention.Cdecl;
		}

		///////////////////////////////////////////

		internal struct MacAppNativeWrapper
		{
			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MessageBox", CallingConvention = Wrapper.convention,
			//   CharSet = CharSet.Unicode )]
			//public unsafe static extern void MessageBox( string text, string caption );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeOut", CallingConvention = Wrapper.convention )]
			public unsafe static extern void FullscreenFadeOut( [MarshalAs( UnmanagedType.U1 )] bool exitApplication );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FullscreenFadeIn", CallingConvention = Wrapper.convention )]
			public unsafe static extern void FullscreenFadeIn( [MarshalAs( UnmanagedType.U1 )] bool exitApplication );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_InitApplicationWindow", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern IntPtr InitApplicationWindow( [MarshalAs( UnmanagedType.U1 )] bool fullscreen,
				int windowSizeX, int windowSizeY, string title );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowVisible", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsWindowVisible();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowActive", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsWindowActive();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsWindowFocused", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsWindowFocused();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowRectangle", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetWindowRectangle( out RectangleI rect );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowClientRect", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetWindowClientRect( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out RectangleI rect );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, out int x, out int y );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
			public unsafe static extern void SetClientRectangleCursorPosition( [MarshalAs( UnmanagedType.U1 )] bool fullScreen, int x, int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetSystemTime", CallingConvention = Wrapper.convention )]
			public unsafe static extern double GetSystemTime();
			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTopMost", CallingConvention = Wrapper.convention )]
			//public unsafe static extern void SetWindowTopMost( [MarshalAs( UnmanagedType.U1 )] bool value );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenSize", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetScreenSize( out int width, out int height );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetScreenBitsPerPixel", CallingConvention = Wrapper.convention )]
			public unsafe static extern int GetScreenBitsPerPixel();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ProcessEvents", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool ProcessEvents();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShutdownApplicationWindow", CallingConvention = Wrapper.convention )]
			public unsafe static extern void ShutdownApplicationWindow();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetWindowState", CallingConvention = Wrapper.convention )]
			public unsafe static extern int GetWindowState();
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowState", CallingConvention = Wrapper.convention )]
			public unsafe static extern void SetWindowState( int state );

			[UnmanagedFunctionPointer( Wrapper.convention )]
			public unsafe delegate void CallbackMessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC );

			[UnmanagedFunctionPointer( Wrapper.convention )]
			public unsafe delegate void CallbackLogInfo( IntPtr/*char* */text );
			[UnmanagedFunctionPointer( Wrapper.convention )]
			public unsafe delegate void CallbackLogWarning( IntPtr/*char* */text );
			[UnmanagedFunctionPointer( Wrapper.convention )]
			public unsafe delegate void CallbackLogFatal( IntPtr/*char* */text );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_Initialize", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool Initialize( CallbackMessageEvent messageEvent, CallbackLogInfo logInfo,
				CallbackLogWarning logWarning, CallbackLogFatal logFatal );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ChangeVideoMode", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool ChangeVideoMode( int width, int height, int bpp );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreVideoMode", CallingConvention = Wrapper.convention )]
			public unsafe static extern bool RestoreVideoMode();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetVideoModes", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetVideoModes( out int count, out Vector3I* array );

			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayCount", CallingConvention = Wrapper.convention )]
			//public unsafe static extern int GetDisplayCount();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShowSystemCursor", CallingConvention = Wrapper.convention )]
			public unsafe static extern void ShowSystemCursor( [MarshalAs( UnmanagedType.U1 )] bool show );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyPressed", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsKeyPressed( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsKeyLocked", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsKeyLocked( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsSystemKey", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsSystemKey( EKeys eKey );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_IsMouseButtonPressed", CallingConvention = Wrapper.convention )]
			[return: MarshalAs( UnmanagedType.U1 )]
			public unsafe static extern bool IsMouseButtonPressed( int buttonCode );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeMemory", CallingConvention = Wrapper.convention )]
			public unsafe static extern void FreeMemory( IntPtr buffer );
			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowPosition", CallingConvention = Wrapper.convention )]
			//public unsafe static extern void SetWindowPosition( int x, int y );
			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowSize", CallingConvention = Wrapper.convention )]
			public unsafe static extern void SetWindowSize( int width, int height );
			//[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowBorderStyle", CallingConvention = Wrapper.convention )]
			//public unsafe static extern void SetWindowBorderStyle( int style );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowRectangle", CallingConvention = Wrapper.convention )]
			public unsafe static extern void SetWindowRectangle( int left, int top, int right, int bottom );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetWindowTitle", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void SetWindowTitle( string title );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void GetMouseMoveDelta( out int x, out int y );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ResetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void ResetMouseMoveDelta( [MarshalAs( UnmanagedType.U1 )] bool resetIgnoreCounter );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_CallCustomPlatformSpecificMethod", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetLoadedBundleNames", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void GetLoadedBundleNames( out IntPtr* list, out int count );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateAcceptsMouseMovedEventsFlag", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
			public unsafe static extern void UpdateAcceptsMouseMovedEventsFlag();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateWindowForProcessChangingVideoMode", CallingConvention = Wrapper.convention )]
			public unsafe static extern void UpdateWindowForProcessChangingVideoMode();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_SetGamma", CallingConvention = Wrapper.convention )]
			public unsafe static extern void SetGamma( float value );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_FreeOutString", CallingConvention = Wrapper.convention )]
			public unsafe static extern void FreeOutString( IntPtr pointer );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ActivateFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
			public unsafe static extern void ActivateFullscreenMinimizedMode();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_RestoreFromFullscreenMinimizedMode", CallingConvention = Wrapper.convention )]
			public unsafe static extern void RestoreFromFullscreenMinimizedMode( int width, int height );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_MinimizeWindow", CallingConvention = Wrapper.convention )]
			public unsafe static extern void MinimizeWindow();

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetActiveDisplayList", CallingConvention = Wrapper.convention )]
			public unsafe static extern int GetActiveDisplayList( int bufferLength, uint* buffer );

			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDisplayInfo", CallingConvention = Wrapper.convention )]
			public unsafe static extern void GetDisplayInfo( uint display, out IntPtr deviceName, out RectangleI bounds, out RectangleI workingArea, [MarshalAs( UnmanagedType.U1 )] out bool primary );

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

		//!!!!
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

		///////////////////////////////////////////

		qqqq;
		//!!!!
		struct KeyInfo
		{
			public int keyCode;

			public KeyInfo( int keyCode )
			{
				this.keyCode = keyCode;
			}
		}

		///////////////////////////////////////////

		public static EngineApp App
		{
			get { return EngineApp.Instance; }
		}

		public MacOSPlatformFunctionality()
		{
			instance = this;
			MacAppNativeWrapper.Initialize( messageEventdelegate, logInfoDelegate, logWarningDelegate, logFatalDelegate );
		}

		public override Vector2I GetScreenSize()
		{
			int width, height;
			MacAppNativeWrapper.GetScreenSize( out width, out height );
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

		static bool GetEKeyByKeyCode( int keyCode, out EKeys eKey )
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

		static int HIWORD( int n )
		{
			return ( ( n >> 0x10 ) & 0xffff );
		}

		static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}

		static void RemovePendingMessage( IntPtr hWnd, uint message )
		{
			MSG msg = new MSG();
			while( PeekMessage( ref msg, hWnd, message, message, PM_REMOVE ) )
			{
			}
		}

		static IntPtr CreatedWindow_ApplicationWindowProc( IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam )
		{
			//try
			//{

			if( EngineApp.Instance == null || Log.FatalActivated )
				return DefWindowProc( hWnd, message, wParam, lParam );

			bool processMessageByEngine = true;
			EngineApp.PerformWindowsWndProcEvent( message, wParam, lParam, ref processMessageByEngine );

			Viewport viewport = null;
			if( !RenderingSystem.Disposed && RenderingSystem.ApplicationRenderTarget != null )
				viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			if( processMessageByEngine )
			{
				switch( message )
				{
				case WM_ERASEBKGND:
					break;

				case WM_CLOSE:
					if( EngineApp.CreatedInsideEngineWindow.HideOnClose )
						ShowWindow( hWnd, SW_HIDE );
					else
						PostQuitMessage( 0 );
					return IntPtr.Zero;

				case WM_ENTERSIZEMOVE:
					instance.resizingMoving = true;
					return IntPtr.Zero;

				case WM_EXITSIZEMOVE:
					instance.resizingMoving = false;
					return IntPtr.Zero;

				case WM_SIZE:
					if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
					{
						EngineApp.CreatedWindowProcessResize();
						return IntPtr.Zero;
					}
					break;

				case WM_GETMINMAXINFO:
					unsafe
					{
						MINMAXINFO* info = (MINMAXINFO*)lParam;
						var size = new Vector2I( 100, 100 );
						if( ProjectSettings.Initialized )
						{
							size = ProjectSettings.Get.General.WindowSizeMinimal.Value;
							if( ProjectSettings.Get.General.WindowSizeApplySystemFontScale )
								size = ( size.ToVector2() * SystemSettings.DPIScale ).ToVector2I();
							info->ptMinTrackSize = size;
						}
						info->ptMinTrackSize = size;
					}
					return IntPtr.Zero;
				}

				if( EngineApp.Created && !EngineApp.Closing )
				{
					switch( message )
					{

					case WM_SETFOCUS:
						{
							instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
							instance.CreatedWindow_UpdateShowSystemCursor( true );
							return IntPtr.Zero;
						}
					//break;

					case WM_KILLFOCUS:
						{
							instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
							instance.CreatedWindow_UpdateShowSystemCursor( true );
							return IntPtr.Zero;
						}
					//break;

					case WM_ACTIVATE:
						{
							if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
							{
								bool activate = LOWORD( (int)wParam ) != WA_INACTIVE;

								instance.mustIgnoreOneMouseMoveAtRelativeMode = true;

								if( activate )
								{
									if( EngineApp.FullscreenEnabled )
									{
										if( EngineApp.FullscreenSize != instance.GetScreenSize() )
											EngineApp.MustChangeVideoMode();
									}
								}
								else
								{
									if( viewport.MouseRelativeMode )
									{
										ReleaseCapture();
										ClipCursor( IntPtr.Zero );
									}

									//!!!!просто обновляем. ведь внутри и так проверка есть. еще можно зафорсить. но не будет ли так, 
									//что отключится слишком быстро? нужно там где включает ставить флаг -> ChangeVUdeMode.
									EngineApp.EnginePauseUpdateState( false, true );
									//if( App.SuspendWorkingWhenApplicationIsNotActive )
									//   App.DoSystemPause( true, true );

									if( EngineApp.FullscreenEnabled )
									{
										instance.SetWindowState( WindowState.Minimized );

										if( !EngineApp.NeedExit )
											SystemSettings.RestoreVideoMode();
									}
								}
								return IntPtr.Zero;
							}
						}
						break;

					case WM_ENTERMENULOOP:
						{
							if( viewport.MouseRelativeMode )
							{
								ReleaseCapture();
								ClipCursor( IntPtr.Zero );
							}

							//if( App.SuspendWorkingWhenApplicationIsNotActive )
							//   App.DoSystemPause( true, true );
							instance.intoMenuLoop = true;
							EngineApp.EnginePauseUpdateState( false, true );

							return IntPtr.Zero;
						}
					//break;

					case WM_EXITMENULOOP:
						instance.intoMenuLoop = false;
						return IntPtr.Zero;

					case WM_LBUTTONDOWN:
					case WM_LBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Left;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_LBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_RBUTTONDOWN:
					case WM_RBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Right;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_RBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_MBUTTONDOWN:
					case WM_MBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Middle;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_MBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_XBUTTONDOWN:
					case WM_XBUTTONDBLCLK:
						{
							int b = HIWORD( (int)wParam );
							if( b == XBUTTON1 )
							{
								EMouseButtons button = EMouseButtons.XButton1;
								bool handled = false;
								viewport.PerformMouseDown( button, ref handled );
								if( message == WM_XBUTTONDBLCLK )
								{
									bool handled2 = false;
									viewport.PerformMouseDoubleClick( button, ref handled2 );
								}
								return new IntPtr( 1 );
							}
							else if( b == XBUTTON2 )
							{
								EMouseButtons button = EMouseButtons.XButton2;
								bool handled = false;
								viewport.PerformMouseDown( button, ref handled );
								if( message == WM_XBUTTONDBLCLK )
								{
									bool handled2 = false;
									viewport.PerformMouseDoubleClick( button, ref handled2 );
								}
								return new IntPtr( 1 );
							}
						}
						break;

					case WM_LBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
							return IntPtr.Zero;
						}

					case WM_RBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
							return IntPtr.Zero;
						}

					case WM_MBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
							return IntPtr.Zero;
						}

					case WM_XBUTTONUP:
						{
							int b = HIWORD( (int)wParam );
							if( b == XBUTTON1 )
							{
								bool handled = false;
								viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
								return new IntPtr( 1 );
							}
							else if( b == XBUTTON2 )
							{
								bool handled = false;
								viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
								return new IntPtr( 1 );
							}
						}
						break;

					case WM_MOUSEWHEEL:
						{
							int delta = (short)HIWORD( (int)wParam.ToInt64() );
							bool handled = false;
							viewport.PerformMouseWheel( delta, ref handled );
							return IntPtr.Zero;
						}
					//break;

					case WM_MOUSEMOVE:
					case WM_NCMOUSEMOVE:
						EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
						return IntPtr.Zero;

					case WM_SYSKEYDOWN:
					case WM_KEYDOWN:
						{
							int keyCode = (int)wParam;

							EKeys eKey;
							if( GetEKeyByKeyCode( keyCode, out eKey ) )
							{
								//support Alt+F4 in mouse relative mode. Alt+F4 is disabled during captured cursor.
								if( viewport.MouseRelativeMode )
								{
									if( eKey == EKeys.F4 && viewport.IsKeyPressed( EKeys.Alt ) )
									{
										EngineApp.NeedExit = true;
										return IntPtr.Zero;
									}
								}

								KeyEvent keyEvent = new KeyEvent( eKey );

								instance.keysInfo[ (int)keyEvent.Key ] = new KeyInfo( keyCode );

								bool handled = false;
								bool suppressKeyPress = false;

								//EKeys.LShift, EKeys.RShift
								if( eKey == EKeys.Shift )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LShift : EKeys.RShift;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								//EKeys.LControl, EKeys.RControl
								if( eKey == EKeys.Control )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LControl : EKeys.RControl;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								//EKeys.LAlt, EKeys.RAlt
								if( eKey == EKeys.Alt )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LAlt : EKeys.RAlt;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								viewport.PerformKeyDown( keyEvent, ref handled );
								if( keyEvent.SuppressKeyPress )
									suppressKeyPress = true;

								if( suppressKeyPress )
								{
									RemovePendingMessage( hWnd, WM_CHAR );
									RemovePendingMessage( hWnd, WM_SYSCHAR );
									RemovePendingMessage( hWnd, WM_IME_CHAR );
								}

								if( !handled && EngineApp.InitSettings.AllowChangeScreenVideoMode )
								{
									if( viewport.IsKeyPressed( EKeys.Alt ) && eKey == EKeys.Return )
									{
										EngineApp.SetFullscreenMode( !EngineApp.FullscreenEnabled, EngineApp.FullscreenSize );
										//App.FullScreen = !App.FullScreen;
										handled = true;
									}
								}

								if( handled )
									return IntPtr.Zero;
							}
						}
						break;

					case WM_SYSKEYUP:
					case WM_KEYUP:
						{
							int keyCode = (int)wParam;

							EKeys eKey;
							if( GetEKeyByKeyCode( keyCode, out eKey ) )
							{
								bool handled = false;

								//EKeys.LShift, EKeys.RShift
								if( eKey == EKeys.Shift )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LShift : EKeys.RShift;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								//EKeys.LControl, EKeys.RControl
								if( eKey == EKeys.Control )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LControl : EKeys.RControl;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								//EKeys.LAlt, EKeys.RAlt
								if( eKey == EKeys.Alt )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LAlt : EKeys.RAlt;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								KeyEvent keyEvent = new KeyEvent( eKey );
								viewport.PerformKeyUp( keyEvent, ref handled );

								if( handled )
									return IntPtr.Zero;
							}
						}
						break;

					case WM_SYSCHAR:
					case WM_CHAR:
						{
							char keyChar = (char)wParam;

							KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
							bool handled = false;
							viewport.PerformKeyPress( keyPressEvent, ref handled );
							if( handled )
								return IntPtr.Zero;
						}
						break;

					case WM_IME_COMPOSITION:
						if( ( (uint)lParam & GCS_RESULTSTR ) != 0 )
						{
							unsafe
							{
								IntPtr context = ImmGetContext( hWnd );
								if( context != IntPtr.Zero )
								{
									int bytes = ImmGetCompositionString( context, GCS_RESULTSTR, null, 0 );
									if( bytes > 0 )
									{
										char[] characters = new char[ bytes / 2 ];
										fixed( char* pCharacters = characters )
										{
											ImmGetCompositionString( context, GCS_RESULTSTR, pCharacters, (uint)bytes );
										}

										foreach( char character in characters )
										{
											if( character != 0 )
											{
												KeyPressEvent keyPressEvent = new KeyPressEvent( character );
												bool handled = false;
												viewport.PerformKeyPress( keyPressEvent, ref handled );
											}
										}
									}

									ImmReleaseContext( hWnd, context );
								}
							}

							return IntPtr.Zero;
						}
						break;

					case WM_TIMER:
						if( (int)wParam == suspendModeTimerID )
						{
							if( EngineApp.DrawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
							{
								unsafe
								{
									InvalidateRect( hWnd, null, false );
								}
							}

							if( !IsAllowApplicationIdle() )
								EngineApp.CreatedWindowApplicationIdle( true );

							return IntPtr.Zero;
						}
						break;

					case WM_PAINT:

#if !DEPLOY

						RECT MakeRECT( int left, int top, int width, int height )
						{
							RECT r = new RECT();
							r.Left = left;
							r.Top = top;
							r.Right = left + width;
							r.Bottom = top + height;
							return r;
						}

						////draw splash screen
						//var drawSplashScreen = EngineApp.DrawSplashScreen;

						//if( drawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
						//{
						//var hdc = BeginPaint( hWnd, out var ps );

						////int COLOR_WINDOW = 3;// 5;
						////FillRect( hdc, ref ps.rcPaint, (IntPtr)( COLOR_WINDOW + 1 ) );
						////FillRect( hdc, out ps.rcPaint, (HBRUSH)( COLOR_WINDOW + 1 ) );

						//using( var bitmap = EngineInfo.GetSplashLogoImage( drawSplashScreen ) )
						//{
						//	var screenRect = instance.CreatedWindow_GetClientRectangle();
						//	var screenSize = screenRect.Size;
						//	var center = screenRect.GetCenter();

						//	Bitmap bitmap2 = null;

						//	var imageSize = new Vector2I( bitmap.Size.Width, bitmap.Size.Height );
						//	if( imageSize.X > screenSize.X )
						//	{
						//		double factor = (double)screenSize.X / (double)imageSize.X;
						//		imageSize = ( imageSize.ToVector2() * factor ).ToVector2I();
						//		bitmap2 = Win32Utility.ResizeImage( bitmap, imageSize.X, imageSize.Y );
						//	}

						//	var splashBitmap = ( bitmap2 ?? bitmap ).GetHbitmap();
						//	if( splashBitmap != IntPtr.Zero )
						//	{
						//		var hdcMem = CreateCompatibleDC( hdc );
						//		var oldBitmap = SelectObject( hdcMem, splashBitmap );

						//		var destRect = new System.Drawing.Rectangle( center.X - imageSize.X / 2, center.Y - imageSize.Y / 2, imageSize.X, imageSize.Y );

						//		//draw background
						//		{
						//			var color = drawSplashScreen == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.WhiteBackground ? Color.White : Color.Black;

						//			//!!!!
						//			//color = Color.Gray;

						//			var brush = CreateSolidBrush( (uint)ColorTranslator.ToWin32( color ) );

						//			if( destRect.Left > 0 )
						//			{
						//				RECT r = MakeRECT( 0, 0, destRect.Left, screenSize.Y );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Right < screenSize.X )
						//			{
						//				RECT r = MakeRECT( destRect.Right, 0, screenSize.X - destRect.Right, screenSize.Y );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Top > 0 )
						//			{
						//				RECT r = MakeRECT( 0, 0, screenSize.X, destRect.Top );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Bottom < screenSize.Y )
						//			{
						//				RECT r = MakeRECT( 0, destRect.Bottom, screenSize.X, screenSize.Y - destRect.Bottom );
						//				FillRect( hdc, ref r, brush );
						//			}

						//			DeleteObject( brush );
						//		}

						//		////!!!!
						//		//{
						//		//	var text = new StringBuilder( "Test test" );

						//		//	var rect = MakeRECT( 0, 0, 500, 500 );
						//		//	var format = DT_CENTER | DT_VCENTER | DT_SINGLELINE;

						//		//	SetTextColor( hdc, ColorTranslator.ToWin32( Color.FromArgb( 255, 0, 0 ) ) );

						//		//	DrawTextEx( hdc, text, text.Length, ref rect, format, IntPtr.Zero );
						//		//}

						//		//if( stretch )
						//		//{
						//		//	StretchBlt( hdc, destRect.Left, destRect.Top, destRect.Width, destRect.Height, hdcMem, 0, 0, bitmap.Size.Width, bitmap.Size.Height, TernaryRasterOperations.SRCCOPY );
						//		//}
						//		//else
						//		//{

						//		BitBlt( hdc, destRect.Left, destRect.Top, destRect.Width, destRect.Height, hdcMem, 0, 0, TernaryRasterOperations.SRCCOPY );

						//		//}

						//		SelectObject( hdcMem, oldBitmap );
						//		DeleteDC( hdcMem );

						//	}

						//	bitmap2?.Dispose();
						//}

						//EndPaint( hWnd, ref ps );

						//	return IntPtr.Zero;
						//}

						if( SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) && projectServer == "1" && RenderingSystem.BackendNull )
						{
							int ToWin32( int r, int g, int b )
							{
								return ( b << 16 ) | ( g << 8 ) | r;
							}

							var hdc = BeginPaint( hWnd, out var ps );

							{
								var screenRect = instance.CreatedWindow_GetClientRectangle();
								var screenSize = screenRect.Size;

								//var textColor = Color.White;
								//var backColor = Color.Black;

								//draw background
								{
									var brush = CreateSolidBrush( (uint)ToWin32( 0, 0, 0 ) );
									//var brush = CreateSolidBrush( (uint)ColorTranslator.ToWin32( backColor ) );

									var rect = MakeRECT( 0, 0, screenSize.X, screenSize.Y );
									FillRect( hdc, ref rect, brush );

									DeleteObject( brush );
								}

								//draw text
								{
									var text = new StringBuilder( "Server mode" );

									var rect = MakeRECT( 10, 10, screenSize.X, screenSize.Y );
									var format = DT_LEFT | DT_TOP;
									//var format = DT_CENTER | DT_VCENTER;// | DT_SINGLELINE;

									SetTextColor( hdc, ToWin32( 255, 255, 255 ) );
									SetBkColor( hdc, ToWin32( 0, 0, 0 ) );

									//SetTextColor( hdc, ColorTranslator.ToWin32( textColor ) );
									//SetBkColor( hdc, ColorTranslator.ToWin32( backColor ) );

									DrawTextEx( hdc, text, text.Length, ref rect, format, IntPtr.Zero );
								}
							}

							EndPaint( hWnd, ref ps );

							return IntPtr.Zero;
						}


#endif

						if( EngineApp.insideRunMessageLoop && EngineApp.EnginePaused && !instance.resizingMoving &&
							!instance.intoMenuLoop && !instance.goingToWindowedMode &&
							!instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
						{
							EngineApp.CreatedWindowApplicationIdle( false );
						}
						break;
					}
				}
			}

			if( applicationWindowProcNeedCallDefWindowProc )
				return DefWindowProc( hWnd, message, wParam, lParam );
			else
				return IntPtr.Zero;
			//}
			//catch( Exception e )
			//{
			//	Log.Fatal( "WindowsPlatformFunctionality: CreatedWindow_ApplicationWindowProc: Exception: " + e.Message );
			//	return IntPtr.Zero;
			//}
		}

		public override void CreatedWindow_ProcessMessageEvents()
		{
			MSG msg = new MSG();

			while( PeekMessage( ref msg, IntPtr.Zero, 0, 0, PM_REMOVE ) )
			{
				if( msg.message == WM_QUIT )
				{
					EngineApp.NeedExit = true;
					break;
				}

				TranslateMessage( ref msg );
				DispatchMessage( ref msg );
			}
		}

		public override void CreatedWindow_RunMessageLoop()
		{
			while( true )
			{
				MSG msg = new MSG();

				if( PeekMessage( ref msg, IntPtr.Zero, 0, 0, PM_REMOVE ) )
				{
					if( msg.message == WM_QUIT )
						break;

					TranslateMessage( ref msg );
					DispatchMessage( ref msg );

					continue;
				}
				else
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

							//finish switching to windowed mode
							if( goingToWindowedMode )
							{
								SetWindowBorderStyle( PlatformFunctionality.WindowBorderStyle.Sizeable );
								//!!!!было .VideoMode
								Vector2I pos = ( GetScreenSize() - EngineApp.FullscreenSize ) / 2;
								//!!!!было .VideoMode
								Vector2I size = EngineApp.FullscreenSize - new Vector2I( 1, 1 );
								SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_NOTOPMOST, pos.X, pos.Y, size.X, size.Y, 0 );
								goingToWindowedMode = false;
								EngineApp.CreatedWindowProcessResize();
							}

							//finish switching to fullscreen mode
							if( goingToFullScreenMode )
							{
								bool topMost = !Debugger.IsAttached;
								SetWindowPos( EngineApp.ApplicationWindowHandle, topMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0,
									EngineApp.FullscreenSize.X, EngineApp.FullscreenSize.Y, 0 );
								goingToFullScreenMode = false;
								EngineApp.CreatedWindowProcessResize();
							}

							EngineApp.CreatedWindowApplicationIdle( false );
						}
					}
					else
						WaitMessage();
				}
			}

		}

		static bool IsAllowApplicationIdle()
		{
			bool needIdle = true;

			if( EngineApp.EnginePauseWhenApplicationIsNotActive ) //!!!!new
			{
				if( IsIconic( EngineApp.ApplicationWindowHandle ) )
					needIdle = false;
			}

			//!!!!!так?
			if( EngineApp.FullscreenEnabled && GetForegroundWindow() != EngineApp.ApplicationWindowHandle )
				needIdle = false;

			if( EngineApp.EnginePaused )
				needIdle = false;

			return needIdle;
		}

		public override bool IsIntoMenuLoop()
		{
			return intoMenuLoop;
		}

		public override void MessageLoopWaitMessage()
		{
			WaitMessage();
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

		//!!!!

		//static Assembly winFormsAssembly;

		//static Type GetIconType()
		//{
		//	if( winFormsAssembly == null )
		//		winFormsAssembly = GetAssemblyByName( "System.Drawing.Common" );
		//	return winFormsAssembly.GetType( "System.Drawing.Icon" );
		//}

		////save icon objects in memory. maybe no sense
		//static object currentIcon;
		//static object currentSmallIcon;

		public override void CreatedWindow_UpdateWindowIcon( object icon, string iconFilePath )
		{
			//!!!!

			////use icon object (System.Drawing.Icon)
			//if( icon != null )
			//{
			//	//make small icon
			//	object smallIcon = null;

			//	var smallIconSize = EngineApp.GetSystemSmallIconSize();

			//	try
			//	{
			//		//!!!!
			//		//temporary uses Winforms. can do resize by means native code

			//		var iconType = GetIconType();
			//		var constructor = iconType.GetConstructor( new Type[] { iconType, typeof( int ), typeof( int ) } );
			//		smallIcon = constructor.Invoke( new object[] { icon, smallIconSize.X, smallIconSize.Y } );

			//		//var smallIcon = new Icon( icon, new Size( smallIconSize.X, smallIconSize.Y ) );
			//	}
			//	catch { }

			//	if( smallIcon != null )
			//	{
			//		var handle = (IntPtr)ObjectEx.PropertyGet( smallIcon, "Handle" );
			//		SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, handle );
			//		//SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, smallIcon.Handle );
			//	}
			//	else
			//		SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, IntPtr.Zero );

			//	if( icon != null )
			//	{
			//		var handle = (IntPtr)ObjectEx.PropertyGet( icon, "Handle" );
			//		SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, handle );
			//		//SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, icon.Handle );
			//	}
			//	else
			//		SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, IntPtr.Zero );

			//	currentIcon = icon;
			//	currentSmallIcon = smallIcon;
			//}

			////use file path to icon
			//if( !string.IsNullOrEmpty( iconFilePath ) )
			//{
			//	try
			//	{
			//		{
			//			var smallIconSize = EngineApp.GetSystemSmallIconSize();

			//			var handle = LoadImage( IntPtr.Zero, iconFilePath, IMAGE_ICON, smallIconSize.X, smallIconSize.Y, LR_LOADFROMFILE );
			//			if( handle != IntPtr.Zero )
			//				SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, handle );
			//		}

			//		{
			//			var handle = LoadImage( IntPtr.Zero, iconFilePath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE );
			//			if( handle != IntPtr.Zero )
			//				SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, handle );
			//		}
			//	}
			//	catch { }
			//}
		}

		public override RectangleI CreatedWindow_GetWindowRectangle()
		{
			MacAppNativeWrapper.GetWindowRectangle( out var result );
			return result;
		}

		//!!!!?
		public override RectangleI CreatedWindow_GetClientRectangle()
		{
			qqqq;

			RectangleI clientRect;
			GetClientRect( EngineApp.ApplicationWindowHandle, out clientRect );
			return clientRect;

			qqq;

			RectI result;
			MacAppNativeWrapper.GetWindowClientRect( App.FullScreen, out result );
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
			return MacAppNativeWrapper.IsWindowActive();

			//!!!!что-то отсюда?
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
			return (WindowState)MacAppNativeWrapper.GetWindowState();
		}

		public override void SetWindowState( WindowState value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				//!!!!
				if( EngineApp.FullscreenEnabled && value == WindowState.Minimized )
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

		void SetWindowBorderStyle( WindowBorderStyle value )
		{
			if( IntPtr.Size == 8 )
			{
				ulong style = (ulong)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

				if( value == WindowBorderStyle.None )
				{
					unchecked
					{
						style &= ~(ulong)WS_OVERLAPPEDWINDOW;
						style |= ( (ulong)WS_POPUP );
					}
				}
				else if( value == WindowBorderStyle.Sizeable )
				{
					unchecked
					{
						style &= ~(ulong)WS_POPUP;
						style |= ( (ulong)WS_OVERLAPPEDWINDOW );
					}
				}

				SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)style );
			}
			else
			{
				uint style = (uint)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

				if( value == WindowBorderStyle.None )
				{
					unchecked
					{
						style &= ~(uint)WS_OVERLAPPEDWINDOW;
						style |= ( (uint)WS_POPUP );
					}
				}
				else if( value == WindowBorderStyle.Sizeable )
				{
					unchecked
					{
						style &= ~(uint)WS_POPUP;
						style |= ( (uint)WS_OVERLAPPEDWINDOW );
					}
				}

				SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)(int)style );
			}
		}

		public override void SetWindowVisible( bool value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				int show;
				if( value )
					show = SW_SHOW;
				else
					show = SW_HIDE;
				ShowWindow( EngineApp.ApplicationWindowHandle, show );
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

			qqqq;

			Vector2I position;
			GetCursorPos( out position );
			ScreenToClient( EngineApp.ApplicationWindowHandle, ref position );

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			return new Vector2F(
				(float)position.X / (float)( viewport.SizeInPixels.X - viewport.SizeInPixels.X % 2 ),
				(float)position.Y / (float)( viewport.SizeInPixels.Y - viewport.SizeInPixels.Y % 2 ) );

			qqqq;

			int x, y;
			MacAppNativeWrapper.GetClientRectangleCursorPosition( App.FullScreen, out x, out y );

			return new Vec2(
				(float)x / (float)( App.VideoMode.X - App.VideoMode.X % 2 ),
				(float)y / (float)( App.VideoMode.Y - App.VideoMode.Y % 2 ) );
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
				Vector2I position = new Vector2I(
					(int)(float)( value.X * (float)viewport.SizeInPixels.X ),
					(int)(float)( value.Y * (float)viewport.SizeInPixels.Y ) );

				qqqqq;

				Vector2I globalPosition = position;
				ClientToScreen( EngineApp.ApplicationWindowHandle, ref globalPosition );
				SetCursorPos( globalPosition.X, globalPosition.Y );

				qqqq;

				if( App.WindowHandle != IntPtr.Zero )
				{
					Vec2I position = new Vec2I(
						(int)(float)( value.X * (float)App.VideoMode.X ),
						(int)(float)( value.Y * (float)App.VideoMode.Y ) );
					MacAppNativeWrapper.SetClientRectangleCursorPosition( App.FullScreen, position.X, position.Y );
				}


				lastMousePositionForMouseMoveDelta = value;
				lastMousePositionForCheckMouseOutsideWindow = value;
			}
		}

		public override bool IsFocused()
		{
			qqqq;

			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
				return false;
			return GetFocus() == EngineApp.ApplicationWindowHandle;

			qqq;
			return MacAppNativeWrapper.IsWindowFocused();
		}

		public override bool ApplicationIsActivated()
		{
			var activatedHandle = GetForegroundWindow();
			if( activatedHandle == IntPtr.Zero )
				return false;// No window is currently activated

			var procId = Process.GetCurrentProcess().Id;
			int activeProcId;
			GetWindowThreadProcessId( activatedHandle, out activeProcId );

			if( activeProcId != procId )
				return false;

			if( GetWindowState() == WindowState.Minimized )
				return false;

			return true;
		}

		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
		{
			bool show = EngineApp.IsRealShowSystemCursor();

			if( createdWindow_UpdateShowSystemCursor != show || forceUpdate )
			{
				createdWindow_UpdateShowSystemCursor = show;

				if( show )
				{
					while( ShowCursor( 1 ) < 0 ) { }
				}
				else
				{
					while( ShowCursor( 0 ) >= 0 ) { }
				}
			}
		}

		public override IntPtr GetSystemCursorByFileName( string virtualFileName )
		{
			IntPtr hCursor = IntPtr.Zero;

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				if( !loadedSystemCursors.TryGetValue( virtualFileName, out hCursor ) )
				{
					hCursor = IntPtr.Zero;

					string realFileName;
					if( Path.IsPathRooted( virtualFileName ) )
						realFileName = virtualFileName;
					else
						realFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );

					if( File.Exists( realFileName ) )
					{
						//load from real file system
						hCursor = LoadCursorFromFile( realFileName );
					}
					else
					{
						//load from virtual file system

						string tempRealFileName = VirtualPathUtility.GetRealPathByVirtual(
							string.Format( "user:_Temp_{0}", Path.GetFileName( virtualFileName ) ) );

						try
						{
							string directoryName = Path.GetDirectoryName( tempRealFileName );
							if( !Directory.Exists( directoryName ) )
								Directory.CreateDirectory( directoryName );

							byte[] data;

							using( VirtualFileStream stream = VirtualFile.Open( virtualFileName ) )
							{
								data = new byte[ stream.Length ];
								if( stream.Read( data, 0, (int)stream.Length ) != stream.Length )
									throw new Exception();
							}

							File.WriteAllBytes( tempRealFileName, data );

							hCursor = LoadCursorFromFile( tempRealFileName );

							File.Delete( tempRealFileName );
						}
						catch { }
					}

					loadedSystemCursors.Add( virtualFileName, hCursor );
				}
			}

			return hCursor;
		}

		public override void CreatedWindow_UpdateSystemCursorFileName()
		{
			//!!!!

		}

		public unsafe override bool InitDirectInputMouseDevice()
		{
			IDirectInput* alreadyCreatedDirectInput = null;
			{
				WindowsInputDeviceManager windowsInputDeviceManager = InputDeviceManager.Instance as WindowsInputDeviceManager;
				if( windowsInputDeviceManager != null )
					alreadyCreatedDirectInput = windowsInputDeviceManager.DirectInput;
			}

			if( !DirectInputMouseDevice.Init( EngineApp.ApplicationWindowHandle, alreadyCreatedDirectInput ) )
				return false;

			return true;
		}

		public override void ShutdownDirectInputMouseDevice()
		{
			DirectInputMouseDevice.Shutdown();
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

				if( DirectInputMouseDevice.Instance != null )
				{
					DirectInputMouseDevice.State state = DirectInputMouseDevice.Instance.GetState();
					if( state.Position.X != 0 || state.Position.Y != 0 )
					{
						Vector2F offset = new Vector2F(
							(float)(int)state.Position.X / viewport.SizeInPixels.X,
							(float)(int)state.Position.Y / viewport.SizeInPixels.Y );

						viewport.PerformMouseMove( offset );

						if( !EngineApp.Closing && IsFocused() )
							CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
						//App.MousePosition = new Vec2( .5f, .5f );
					}
				}
				else
				{
					//!!!!надо ли

					if( !EngineApp.Closing && IsFocused() )
						CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
					//App.MousePosition = new Vec2( .5f, .5f );
				}
			}
		}

		public unsafe override string[] GetNativeModuleNames()
		{
			IntPtr* list;
			int count;
			MacAppNativeWrapper.GetLoadedBundleNames( out list, out count );

			string[] result = new string[ count ];

			for( int n = 0; n < count; n++ )
			{
				IntPtr pointer = list[ n ];
				string name = MacAppNativeWrapper.GetOutString( pointer );
				result[ n ] = name;
			}

			MacAppNativeWrapper.FreeMemory( (IntPtr)list );

			return result;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
			qqqqq;

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			if( viewport.MouseRelativeMode )
			{
				if( DirectInputMouseDevice.Instance != null )
					DirectInputMouseDevice.Instance.GetState();
			}
			else
			{
				ReleaseCapture();
				ClipCursor( IntPtr.Zero );
			}

			qqqq;//учесть
			mustIgnoreOneMouseMoveAtRelativeMode = true;

			qqqq;
			MacAppNativeWrapper.ResetMouseMoveDelta( true );
		}

		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
		{
			qqq;

			if( !mustIgnoreOneMouseMoveAtRelativeMode )
			{
				delta = CreatedWindow_GetMousePosition() - lastMousePositionForMouseMoveDelta;
			}
			else
			{
				mustIgnoreOneMouseMoveAtRelativeMode = false;
				delta = Vector2F.Zero;
			}

			qqqq;

			int x, y;
			MacAppNativeWrapper.GetMouseMoveDelta( out x, out y );
			if( App.VideoMode.X != 0 && App.VideoMode.Y != 0 )
				delta = new Vec2( x, y ) / App.VideoMode.ToVec2();
			else
				delta = Vec2.Zero;
			MacAppNativeWrapper.ResetMouseMoveDelta( false );

		}

		public override IntPtr CallPlatformSpecificMethod( string message, IntPtr param )
		{
			return IntPtr.Zero;
		}

		public override void ProcessChangingVideoMode()
		{


			qqq;

			//change video mode
			if( App.FullScreen )
			{
				if( !DisplaySettings.ChangeVideoMode( App.VideoMode ) )
					return;
				App.lastFullScreenWindowSize = App.VideoMode;
			}
			else
			{
				DisplaySettings.RestoreVideoMode();
			}

			MacAppNativeWrapper.UpdateWindowForProcessChangingVideoMode();

			App.DoResize();


			qq;


			if( EngineApp.FullscreenEnabled )
			{
				goingToFullScreenMode = true;

				//minimize window
				SetWindowState( WindowState.Minimized );

				//!!!!!так?
				//change video mode
				if( !SystemSettings.ChangeVideoMode( EngineApp.FullscreenSize ) )
					return;
				//было
				//App.lastFullScreenWindowSize = App.FullScreenSize;

				//update window
				bool topMost = !Debugger.IsAttached;
				SetWindowBorderStyle( WindowBorderStyle.None );
				SetWindowState( WindowState.Normal );
				SetWindowPos( EngineApp.ApplicationWindowHandle, topMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, EngineApp.FullscreenSize.X, EngineApp.FullscreenSize.Y, 0 );
			}
			else
			{
				goingToWindowedMode = true;

				//!!!!!так?
				//change video mode
				SystemSettings.RestoreVideoMode();
			}
		}

		unsafe public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
		{
			var result = new List<SystemSettings.DisplayInfo>();

			uint[] buffer = new uint[ 256 ];
			int count;

			fixed( uint* pBuffer = buffer )
			{
				count = MacAppNativeWrapper.GetActiveDisplayList( buffer.Length, pBuffer );
			}

			for( int n = 0; n < count; n++ )
			{
				uint display = buffer[ n ];

				IntPtr deviceNamePointer;
				RectangleI bounds;
				RectangleI workingArea;
				bool primary;

				MacAppNativeWrapper.GetDisplayInfo( display, out deviceNamePointer, out bounds, out workingArea, out primary );

				string deviceName = MacAppNativeWrapper.GetOutString( deviceNamePointer );

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
			MacAppNativeWrapper.SetWindowRectangle( rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom );
			goingToChangeWindowRectangle = false;

			qqqq;
			//!!!!!так?
			EngineApp.CreatedWindowProcessResize();
			qqqq;
			App.DoResize();
		}

		public override void GetSystemLanguage( out string name, out string englishName )
		{
			qqqq;

			name = MacAppNativeWrapper.GetOutString( CallCustomPlatformSpecificMethod( "GetSystemLanguageName", IntPtr.Zero ) );
			englishName = MacAppNativeWrapper.GetOutString( CallCustomPlatformSpecificMethod( "GetSystemLanguageEnglishName", IntPtr.Zero ) );

			//name = CultureInfo.CurrentUICulture.Name;
			//englishName = CultureInfo.CurrentUICulture.EnglishName;
		}

		public void SetDarkMode( IntPtr handle, bool enable )
		{
			//!!!!

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
			//!!!!было в 3.5
			//MacAppNativeWrapper.FullscreenFadeOut( exitApplication );
		}

		public override void FullscreenFadeIn( bool exitApplication )
		{
			//!!!!было в 3.5
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

		public override void SetGamma( float value )
		{
			MacAppNativeWrapper.SetGamma( value );
		}



		//!!!!from 3.5:
		//!!!!from 3.5:
		//!!!!from 3.5:
		//!!!!from 3.5:
		//!!!!from 3.5:
		//!!!!from 3.5:
		//!!!!from 3.5:




		public override System.IntPtr InitApplicationWindow()
		{
			IntPtr handle = MacAppNativeWrapper.InitApplicationWindow( App.FullScreen, App.VideoMode.X, App.VideoMode.Y, App.WindowTitle );
			return handle;
		}

		public override void ShutdownApplicationWindow()
		{
			MacAppNativeWrapper.ShutdownApplicationWindow();
		}

		public override void DoMessageEvents()
		{
		}

		//public override int GetMonitorCount()
		//{
		//   return MacAppNativeWrapper.GetDisplayCount();
		//}

		//public override double GetSystemTime()
		//{
		//	return MacAppNativeWrapper.GetSystemTime();
		//}

		public override void MessageLoopWaitMessage()
		{
			DoMessageEvents();
		}

		unsafe static void MessageEvent( MessageTypes messageType, int parameterA, int parameterB, int parameterC )
		{
			switch( messageType )
			{
			case MessageTypes.MouseDown:
				App.DoMouseDown( (EMouseButtons)parameterA );
				break;
			case MessageTypes.MouseUp:
				App.DoMouseUp( (EMouseButtons)parameterA );
				break;
			case MessageTypes.MouseDoubleClick:
				App.DoMouseDoubleClick( (EMouseButtons)parameterA );
				break;
			case MessageTypes.MouseWheel:
				App.DoMouseWheel( (int)parameterA );
				break;
			case MessageTypes.MouseMove:
				App.DoMouseMove();
				break;

			case MessageTypes.KeyDown:
				{
					EKeys eKey = (EKeys)parameterA;
					int character = parameterB;

					bool handled = false;
					bool suppressKeyPress = false;

					if( eKey != (EKeys)0 )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						handled = App.DoKeyDown( keyEvent );

						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
						{
							keyEvent = new KeyEvent( EKeys.Shift );
							if( App.DoKeyDown( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
						{
							keyEvent = new KeyEvent( EKeys.Control );
							if( App.DoKeyDown( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
						{
							keyEvent = new KeyEvent( EKeys.Alt );
							if( App.DoKeyDown( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
						{
							keyEvent = new KeyEvent( EKeys.Command );
							if( App.DoKeyDown( keyEvent ) )
								handled = true;
						}

						//Cmd-Tab. Minimize for fullscreen mode.
						if( !handled && eKey == EKeys.Tab && App.IsKeyPressed( EKeys.Command ) )
						{
							if( App.FullScreen )
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
						if( !handled && eKey == EKeys.M && App.IsKeyPressed( EKeys.Command ) )
						{
							if( App.FullScreen )
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
						if( !handled && eKey == EKeys.Q && App.IsKeyPressed( EKeys.Command ) )
						{
							App.SetNeedExit();
							break;
						}

						if( keyEvent.SuppressKeyPress )
							suppressKeyPress = true;
					}

					if( !suppressKeyPress && !MacAppNativeWrapper.IsSystemKey( eKey ) )
					{
						KeyPressEvent keyPressEvent = new KeyPressEvent( (char)character );
						App.DoKeyPress( keyPressEvent );
					}

					if( handled )
					{
					}
				}
				break;

			case MessageTypes.KeyUp:
				{
					EKeys eKey = (EKeys)parameterA;

					bool handled = false;

					if( eKey != (EKeys)0 )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						handled = App.DoKeyUp( keyEvent );

						if( eKey == EKeys.LShift || eKey == EKeys.RShift )
						{
							keyEvent = new KeyEvent( EKeys.Shift );
							if( App.DoKeyUp( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LControl || eKey == EKeys.RControl )
						{
							keyEvent = new KeyEvent( EKeys.Control );
							if( App.DoKeyUp( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
						{
							keyEvent = new KeyEvent( EKeys.Alt );
							if( App.DoKeyUp( keyEvent ) )
								handled = true;
						}
						if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
						{
							keyEvent = new KeyEvent( EKeys.Command );
							if( App.DoKeyUp( keyEvent ) )
								handled = true;
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
						App.DoResize();
					}
				}
				break;

			case MessageTypes.WindowDidBecomeKey:
				{
					//activated

					instance.UpdateShowSystemCursor();
				}
				break;

			case MessageTypes.WindowDidResignKey:
				{
					//deactivated

					if( App.SuspendWorkingWhenApplicationIsNotActive )
						App.DoSystemPause( true, true );

					instance.UpdateShowSystemCursor();
				}
				break;

			case MessageTypes.WindowWillMiniaturize:
				{
				}
				break;

			case MessageTypes.WindowDidMiniaturize:
				{
					if( App.SuspendWorkingWhenApplicationIsNotActive )
						App.DoSystemPause( true, true );
				}
				break;

			case MessageTypes.WindowDidDeminiaturize:
				{
					if( instance.fullscreenMinimizedMode && App.FullScreen )
					{
						instance.fullscreenMinimizedMode = false;
						MacAppNativeWrapper.RestoreFromFullscreenMinimizedMode( App.VideoMode.X, App.VideoMode.Y );
					}
				}
				break;

			case MessageTypes.Periodic:
				{
					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
					{
						if( !IsAllowApplicationIdle() )
							App.ApplicationIdle( true );
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
							if( instance.GetWindowState() != WindowStates.Minimized )
							{
								MacAppNativeWrapper.MinimizeWindow();

								if( instance.GetWindowState() == WindowStates.Minimized )
								{
									instance.mustGoToFullscreenMinimizedMode = false;
									instance.fullscreenMinimizedMode = true;
								}
							}
						}

						break;
					}

				}
				break;

			}
		}

		public override void RunMessageLoop()
		{
			while( MacAppNativeWrapper.ProcessEvents() )
			{
				if( App.IsNeedExit() )
					break;

				if( IsAllowApplicationIdle() )
				{
					if( !instance.mustGoToFullscreenMinimizedMode && !instance.fullscreenMinimizedMode )
					{
						float time = EngineApp.Instance.Time;
						bool needSleep = EngineApp.Instance.MaxFPS != 0 && time < maxFPSLastRenderTime + 1.0f / EngineApp.Instance.MaxFPS;

						if( needSleep )
						{
							Thread.Sleep( 1 );
						}
						else
						{
							maxFPSLastRenderTime = time;
							App.ApplicationIdle( false );
						}
					}
				}
				else
					System.Threading.Thread.Sleep( 50 );
			}
		}

		static bool IsAllowApplicationIdle()
		{
			bool needIdle = true;

			if( instance.GetWindowState() == WindowStates.Minimized )
				needIdle = false;

			if( App.FullScreen && !instance.IsWindowActive() )
				needIdle = false;

			if( App.SystemPause )
				needIdle = false;

			return needIdle;
		}

		unsafe static void LogInfo( IntPtr/*char* */text )
		{
			string result = MacAppNativeWrapper.GetOutString( text );
			Log.Info( result );
		}

		unsafe static void LogWarning( IntPtr/*char* */text )
		{
			string result = MacAppNativeWrapper.GetOutString( text );
			Log.Warning( result );
		}

		unsafe static void LogFatal( IntPtr/*char* */text )
		{
			string result = MacAppNativeWrapper.GetOutString( text );
			Log.Fatal( result );
		}

		public override void UpdateInputDevices()
		{
			MacAppNativeWrapper.UpdateAcceptsMouseMovedEventsFlag();

			if( MacAppNativeWrapper.IsWindowFocused() &&
				new Rect( 0, 0, 1, 1 ).IsContainsPoint( App.MousePosition ) )
			{
				UpdateShowSystemCursor();
			}

			//mouse
			{
				if( App.IsMouseButtonPressed( EMouseButtons.Left ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Left ) )
				{
					App.DoMouseUp( EMouseButtons.Left );
				}
				if( App.IsMouseButtonPressed( EMouseButtons.Right ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Right ) )
				{
					App.DoMouseUp( EMouseButtons.Right );
				}
				if( App.IsMouseButtonPressed( EMouseButtons.Middle ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Middle ) )
				{
					App.DoMouseUp( EMouseButtons.Middle );
				}
				if( App.IsMouseButtonPressed( EMouseButtons.XButton1 ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton1 ) )
				{
					App.DoMouseUp( EMouseButtons.XButton1 );
				}
				if( App.IsMouseButtonPressed( EMouseButtons.XButton2 ) && !MacAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton2 ) )
				{
					App.DoMouseUp( EMouseButtons.XButton2 );
				}
			}

			//keys
			foreach( EKeys eKey in App.AllKeys )
			{
				if( App.IsKeyPressed( eKey ) && eKey != EKeys.Shift && eKey != EKeys.Control && eKey != EKeys.Alt && eKey != EKeys.Command )
				{
					if( !MacAppNativeWrapper.IsKeyPressed( eKey ) )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						App.DoKeyUp( keyEvent );
					}
				}
			}

			//mouse relative mode
			if( App.MouseRelativeMode )
				App.MousePosition = new Vec2( .5f, .5f );
		}

		public override void UpdateShowSystemCursor()
		{
			bool show = App.IsRealShowSystemCursor();
			MacAppNativeWrapper.ShowSystemCursor( show );
		}

		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
		{
			Log.Fatal( "MacOSXPlatformFunctionality: ShowMessageBoxYesNoQuestion: method is not implemented." );
			return false;

			//int result = MessageBox( EngineApp.ApplicationWindowHandle, text, caption, MB_YESNO | MB_ICONEXCLAMATION );
			//if( result == IDYES )
			//	return true;
			//return false;

		}

		/*public override */
		IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param )
		{
			return MacAppNativeWrapper.CallCustomPlatformSpecificMethod( message, param );
		}

		public override IntPtr CreatedWindow_CreateWindow()
		{
			//!!!!
			throw new NotImplementedException();
		}

		public override void CreatedWindow_DestroyWindow()
		{
			//!!!!
			throw new NotImplementedException();
		}
	}
}
#endif


#endif

