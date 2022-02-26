// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI.Input;
using Windows.Foundation;
using Internal;

namespace NeoAxis
{
	//[ComImport, Guid( "45D64A29-A63E-4CB6-B498-5781D298CB4F" )]
	//[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
	//interface ICoreWindowInterop
	//{
	//	IntPtr WindowHandle { get; }
	//	bool MessageHandled { set; }
	//}

	// application lifecycle:
	// https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle
	// https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.core.coreapplication

	public partial class PlatformFunctionalityUWP : PlatformFunctionality
	{
		static PlatformFunctionalityUWP instance;

		CoreWindowActivationState activationState = CoreWindowActivationState.Deactivated;
		bool windowClosed;
		bool windowVisible = true;
		bool isSizeChanged;

		CoreWindow coreWindow;
		ApplicationView applicationView;
		DisplayInformation displayInfo;

		double maxFPSLastRenderTime;

		double[] lastMouseButtonClickTimeForDoubleClickDetection = new double[ 5 ];

		/////////////////////////////////////////

		public PlatformFunctionalityUWP()
		{
			instance = this;
			SetInstance( instance );

			new PlatformSpecificUtilityUWP();
		}

		public override Vector2I GetScreenSize()
		{
			if( displayInfo == null )
				displayInfo = DisplayInformation.GetForCurrentView();
			return new Vector2I( (int)displayInfo.ScreenWidthInRawPixels, (int)displayInfo.ScreenHeightInRawPixels );
		}

		public override int GetScreenBitsPerPixel()
		{
			//TODO: implement it.
			int bpp = 32;
			return bpp;
		}

		public override Vector2I GetSmallIconSize()
		{
			//TODO: implement it.
			return new Vector2I( 16, 16 );
		}

		public override void Init( IntPtr mainModuleData )
		{
			base.Init( mainModuleData );
		}

		public override IntPtr CreatedWindow_CreateWindow()
		{
			applicationView = ApplicationView.GetForCurrentView();
			coreWindow = CoreWindow.GetForCurrentThread();
			displayInfo = DisplayInformation.GetForCurrentView();

			//var size = EngineApp.InitSettings.CreateWindowSize.Value;
			////var size = GetRequestedRawSize();

			//var minSize = new Size( size.X / displayInfo.RawPixelsPerViewPixel, size.Y / displayInfo.RawPixelsPerViewPixel );
			//applicationView.SetPreferredMinSize( minSize );
			//var canResize = applicationView.TryResizeView( minSize );

			//ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(size.X, size.Y);

			//ApplicationView.PreferredLaunchWindowingMode = EngineApp.FullscreenEnabled ? ApplicationViewWindowingMode.FullScreen : ApplicationViewWindowingMode.PreferredLaunchViewSize;

			coreWindow.SizeChanged += CoreWindow_SizeChanged;
			//coreWindow.KeyDown += CoreWindow_KeyDown;
			//coreWindow.KeyUp += CoreWindow_KeyUp;
			//coreWindow.CharacterReceived += CoreWindow_CharacterReceived;
			coreWindow.PointerPressed += CoreWindow_PointerPressed;
			coreWindow.PointerReleased += CoreWindow_PointerReleased;
			coreWindow.PointerMoved += CoreWindow_PointerMoved;
			coreWindow.PointerWheelChanged += CoreWindow_PointerWheelChanged;
			coreWindow.VisibilityChanged += CoreWindow_VisibilityChanged;
			coreWindow.Activated += CoreWindow_Activated;
			coreWindow.Closed += CoreWindow_Closed;

			coreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;

			displayInfo.DpiChanged += CurrentDisplayInformation_DpiChanged;
			displayInfo.OrientationChanged += CurrentDisplayInformation_OrientationChanged;

			SystemNavigationManager.GetForCurrentView().BackRequested += UWPFunctionality_BackRequested;

			//TODO: dont use IntPtr for crossplatform window handle.  Use type with IntPtr/object.
			//var interop = (ICoreWindowInterop)(object)coreWindow;
			//return interop.WindowHandle

			return Marshal.GetIUnknownForObject( coreWindow );
		}

		private void UWPFunctionality_BackRequested( object sender, BackRequestedEventArgs e )
		{
			//throw new NotImplementedException();
		}

		//Vector2I GetRequestedRawSize()
		//{
		//	bool showMaximized = !EngineApp.FullscreenEnabled &&
		//		EngineApp.InitSettings.CreateWindowState.Value == EngineApp.WindowStateEnum.Maximized &&
		//		!EngineApp.InitSettings.MultiMonitorMode.Value;

		//	Vector2I position; // NOT USED
		//	Vector2I size;
		//	{
		//		if( showMaximized )
		//		{
		//			size = new Vector2I( 800, 600 );
		//			position = ( GetScreenSize() - size ) / 2;
		//		}
		//		else
		//		{
		//			//!!!!!EngineApp.InitializationParameters.MultiMonitorMode.Value? не False?
		//			if( !EngineApp.FullscreenEnabled || EngineApp.InitSettings.MultiMonitorMode.Value )
		//				position = EngineApp.InitSettings.CreateWindowPosition.Value;
		//			else
		//				position = Vector2I.Zero;
		//			size = EngineApp.InitSettings.CreateWindowSize.Value;
		//		}
		//	}
		//	return size;
		//}

		private void CoreWindow_Closed( CoreWindow sender, CoreWindowEventArgs args )
		{
			windowClosed = true;
		}

		private void CoreWindow_PointerMoved( CoreWindow sender, PointerEventArgs args )
		{
			EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
		}

		private void CoreWindow_PointerWheelChanged( CoreWindow sender, PointerEventArgs args )
		{
			bool handled = false;
			var wheelDelta = args.CurrentPoint.Properties.MouseWheelDelta;
			GetViewport()?.PerformMouseWheel( wheelDelta, ref handled );
			//args.Handled = handled;
		}

		private void CoreWindow_PointerPressed( CoreWindow sender, PointerEventArgs args )
		{
			var properties = args.CurrentPoint.Properties;

			EMouseButtons button;
			if( properties.IsLeftButtonPressed )
				button = EMouseButtons.Left;
			else if( properties.IsRightButtonPressed )
				button = EMouseButtons.Right;
			else if( properties.IsMiddleButtonPressed )
				button = EMouseButtons.Middle;
			else
				return;

			bool handled = false;
			GetViewport()?.PerformMouseDown( button, ref handled );
			//args.Handled = handled;

			//double click
			var time = GetSystemTime();
			if( time - lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] < 0.5 )
			{
				bool handled2 = false;
				GetViewport()?.PerformMouseDoubleClick( button, ref handled2 );
				lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = 0;
			}
			else
				lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = time;
		}

		private void CoreWindow_PointerReleased( CoreWindow sender, PointerEventArgs args )
		{
			var properties = args.CurrentPoint.Properties;

			EMouseButtons button;
			if( properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased )
				button = EMouseButtons.Left;
			else if( properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased )
				button = EMouseButtons.Right;
			else if( properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased )
				button = EMouseButtons.Middle;
			else
				return;

			bool handled = false;
			GetViewport()?.PerformMouseUp( button, ref handled );
			//args.Handled = handled;
		}

		private void CoreWindow_SizeChanged( CoreWindow sender, WindowSizeChangedEventArgs args )
		{
			isSizeChanged = true;
		}

		private void CurrentDisplayInformation_OrientationChanged( DisplayInformation sender, object args )
		{
			//NOT TESTED
			isSizeChanged = true;
		}

		private void CurrentDisplayInformation_DpiChanged( DisplayInformation sender, object args )
		{
			//NOT TESTED
			isSizeChanged = true;
		}

		private void Dispatcher_AcceleratorKeyActivated( CoreDispatcher sender, AcceleratorKeyEventArgs args )
		{
			var viewport = GetViewport();
			if( viewport == null )
				return;

			if( !GetEKeyByVirtualKey( args.VirtualKey, out EKeys eKey ) )
				return;

			if( args.EventType == CoreAcceleratorKeyEventType.KeyDown || args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown )
			{
				bool handled = false;
				var keyEvent = new KeyEvent( eKey );
				viewport.PerformKeyDown( keyEvent, ref handled );
				if( keyEvent.SuppressKeyPress )
					args.Handled = true;

				if( !handled && EngineApp.InitSettings.AllowChangeScreenVideoMode )
				{
					//support Alt+F4 in mouse relative mode. Alt+F4 is disabled during captured cursor.
					if( viewport.MouseRelativeMode )
					{
						if( eKey == EKeys.F4 && viewport.IsKeyPressed( EKeys.Alt ) )
						{
							EngineApp.NeedExit = true;
							return;
						}
					}

					if( viewport.IsKeyPressed( EKeys.Alt ) && eKey == EKeys.Return )
					{
						EngineApp.SetFullscreenMode( !EngineApp.FullscreenEnabled, EngineApp.FullscreenSize );
						//App.FullScreen = !App.FullScreen;
						handled = true;
					}
				}

				//args.Handled = handled;
			}
			else if( args.EventType == CoreAcceleratorKeyEventType.KeyUp || args.EventType == CoreAcceleratorKeyEventType.SystemKeyUp )
			{
				bool handled = false;
				viewport.PerformKeyUp( new KeyEvent( eKey ), ref handled );
				//args.Handled = handled;
			}
			else if( args.EventType == CoreAcceleratorKeyEventType.Character || args.EventType == CoreAcceleratorKeyEventType.UnicodeCharacter )
			{
				char keyChar = (char)args.VirtualKey;
				KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
				bool handled = false;
				GetViewport()?.PerformKeyPress( keyPressEvent, ref handled );
				//args.Handled = handled;
			}
		}

		//private void CoreWindow_KeyDown( CoreWindow sender, KeyEventArgs args )
		//{
		//}

		//private void CoreWindow_KeyUp( CoreWindow sender, KeyEventArgs args )
		//{
		//}

		//private void CoreWindow_CharacterReceived( CoreWindow sender, CharacterReceivedEventArgs args )
		//{
		//	char keyChar = (char)args.KeyCode;

		//	KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
		//	bool handled = false;
		//	GetViewport()?.PerformKeyPress( keyPressEvent, ref handled );
		//	//args.Handled = handled;
		//}

		private void CoreWindow_VisibilityChanged( CoreWindow sender, VisibilityChangedEventArgs args )
		{
			windowVisible = args.Visible;

			EngineApp.EnginePauseUpdateState( false, !args.Visible );
		}

		private void CoreWindow_Activated( CoreWindow sender, WindowActivatedEventArgs args )
		{
			activationState = args.WindowActivationState;

			if( args.WindowActivationState == CoreWindowActivationState.Deactivated )
			{
				//!!!!просто обновляем. ведь внутри и так проверка есть. еще можно зафорсить. но не будет ли так, 
				//что отключится слишком быстро? нужно там где включает ставить флаг -> ChangeVUdeMode.
				EngineApp.EnginePauseUpdateState( false, true );

				// do stuff
			}
			else
			{
				// do different stuff
			}

			instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
			instance.CreatedWindow_UpdateShowSystemCursor( true );
		}

		private Viewport GetViewport()
		{
			if( !RenderingSystem.Disposed && RenderingSystem.ApplicationRenderTarget != null )
				return RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			else
				return null;
		}

		public override void CreatedWindow_DestroyWindow()
		{
			// empty for UWP?
		}


		public override void CreatedWindow_ProcessMessageEvents()
		{
			// empty for UWP?
			coreWindow.Dispatcher.ProcessEvents( CoreProcessEventsOption.ProcessAllIfPresent );
		}

		public override void MessageLoopWaitMessage()
		{
			// empty for UWP
		}

		public override bool IsIntoMenuLoop()
		{
			// empty for UWP
			return false;
		}

		public override void CreatedWindow_RunMessageLoop()
		{
			while( !windowClosed )
			{
				if( EngineApp.NeedExit )
					break;

				if( IsAllowApplicationIdle() )
				{
					// Process events incoming to the window.
					coreWindow.Dispatcher.ProcessEvents( CoreProcessEventsOption.ProcessAllIfPresent );


					EngineApp.UpdateEngineTime();
					double time = EngineApp.EngineTime;
					bool needSleep = EngineApp.MaxFPS != 0 && time < maxFPSLastRenderTime + 1.0f / EngineApp.MaxFPS;

					if( needSleep )
					{
						//!!!!0?
						Thread.Sleep( 1 );
					}
					else
					{
						maxFPSLastRenderTime = time;

						EngineApp.CreatedWindowApplicationIdle( false );

						if( isSizeChanged )
						{
							isSizeChanged = false;
							EngineApp.CreatedWindowProcessResize();
						}
					}

				}
				else
				{
					coreWindow.Dispatcher.ProcessEvents( CoreProcessEventsOption.ProcessOneAndAllPending );
				}
			}
		}

		bool IsAllowApplicationIdle()
		{
			if( !windowVisible ) // or coreWindow.Visible?
				return false;

			//if( EngineApp.EnginePaused )
			//	return false;

			return true;
		}

		public override bool IsWindowInitialized()
		{
			return coreWindow != null;
		}

		public override void CreatedWindow_UpdateWindowTitle( string title )
		{
			// we can't do it in UWP
		}

		//public override void CreatedWindow_UpdateWindowIcon( System.Drawing.Icon smallIcon, System.Drawing.Icon icon )
		//{
		//	//TODO: implement it.
		//}

		public override RectangleI CreatedWindow_GetWindowRectangle()
		{
			//TODO: implement it.

			return CreatedWindow_GetClientRectangle();
		}

		public override void CreatedWindow_SetWindowRectangle( RectangleI rectangle )
		{
			//TODO: implement it.

			var scaleFactor = displayInfo.RawPixelsPerViewPixel;

			applicationView.TryResizeView( new Size(
				( rectangle.Right - rectangle.Left ) / scaleFactor,
				( rectangle.Bottom - rectangle.Top ) / scaleFactor ) );

			//!!!!!так?
			EngineApp.CreatedWindowProcessResize();
		}

		//!!!!?
		public override RectangleI CreatedWindow_GetClientRectangle()
		{
			var scaleFactor = displayInfo.RawPixelsPerViewPixel;
			var b = applicationView.VisibleBounds; // or coreWindow.Bounds ?
			return new RectangleI(
				(int)( b.Left * scaleFactor ), (int)( b.Top * scaleFactor ),
				(int)( b.Right * scaleFactor ), (int)( b.Bottom * scaleFactor ) );
		}

		//!!!!!
		public override void CreatedWindow_SetWindowSize( Vector2I size )
		{
			var scaleFactor = displayInfo.RawPixelsPerViewPixel;
			var viewSize = new Size( size.X / scaleFactor, size.Y / scaleFactor );
			if( !applicationView.TryResizeView( viewSize ) )
			{
				// ?
			}
		}

		public override bool ApplicationIsActivated()
		{
			//TEST IT:
			return CreatedWindow_IsWindowActive();
		}

		public override bool CreatedWindow_IsWindowActive()
		{
			if( coreWindow.ActivationMode == CoreWindowActivationMode.None )
				return false;

			// remove this:
			//if( coreWindow.ActivationMode != CoreWindowActivationMode.Deactivated )
			//	Debug.Assert( activationState != CoreWindowActivationState.Deactivated );
			//else
			//	Debug.Assert( activationState == CoreWindowActivationState.Deactivated );
			//

			return coreWindow.ActivationMode != CoreWindowActivationMode.Deactivated;
		}

		public override bool IsWindowVisible()
		{
			return coreWindow.Visible;
		}

		public override WindowState GetWindowState()
		{
			//TODO: implement it.

			// we can determine the difference between the maximized and fullscreen window
			// but not minimized
			// see SDL WINRT_DetectWindowFlags

			return WindowState.Normal;
		}

		public override void SetWindowState( WindowState value )
		{
			// we can't do it in UWP
		}

		public override bool IsFocused()
		{
			return coreWindow.ActivationMode == CoreWindowActivationMode.ActivatedInForeground;
		}
	}
}
