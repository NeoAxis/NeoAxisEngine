// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Windows.System;

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
		volatile bool windowClosed;
		volatile bool windowVisible = true;
		volatile bool isSizeChanged;

		CoreWindow coreWindow;
		ApplicationView applicationView;
		DisplayInformation displayInfo;

		double maxFPSLastRenderTime;

		double[] lastMouseButtonClickTimeForDoubleClickDetection = new double[ 5 ];

		//ConcurrentQueue<ActionItem> actionsToProcess = new ConcurrentQueue<ActionItem>();

		/////////////////////////////////////////

		public struct ActionItem
		{
			public string Name;
			public EMouseButtons Button;
			public CoreAcceleratorKeyEventType EventType;
			public VirtualKey VirtualKey;
		}

		/////////////////////////////////////////

		public PlatformFunctionalityUWP()
		{
			instance = this;
			SetInstance( instance, SystemSettings.Platform.UWP );
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
			return 32;
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

			coreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;

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
			args.Handled = true;

			windowClosed = true;
		}

		private void CoreWindow_PointerMoved( CoreWindow sender, PointerEventArgs args )
		{
			args.Handled = true;

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
			} );
		}

		private void CoreWindow_PointerWheelChanged( CoreWindow sender, PointerEventArgs args )
		{
			args.Handled = true;

			var wheelDelta = args.CurrentPoint.Properties.MouseWheelDelta;

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				bool handled = false;
				GetViewport()?.PerformMouseWheel( wheelDelta, ref handled );
			} );
		}

		private void CoreWindow_PointerPressed( CoreWindow sender, PointerEventArgs args )
		{
			args.Handled = true;

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

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				bool handled = false;
				GetViewport()?.PerformMouseDown( button, ref handled );

				//double click
				var time = EngineApp.GetSystemTime();
				if( time - lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] < 0.5 )
				{
					bool handled2 = false;
					GetViewport()?.PerformMouseDoubleClick( button, ref handled2 );
					lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = 0;
				}
				else
					lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = time;
			} );

			//actionsToProcess.Enqueue( new ActionItem() { Name = "PointerPressed", Button = button } );
		}

		private void CoreWindow_PointerReleased( CoreWindow sender, PointerEventArgs args )
		{
			args.Handled = true;

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

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				bool handled = false;
				GetViewport()?.PerformMouseUp( button, ref handled );
			} );

			//actionsToProcess.Enqueue( new ActionItem() { Name = "PointerReleased", Button = button } );
		}

		private void CoreWindow_SizeChanged( CoreWindow sender, WindowSizeChangedEventArgs args )
		{
			args.Handled = true;

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
			//args.Handled = true;

			var eventType = args.EventType;
			var virtualKey = args.VirtualKey;

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				var viewport = GetViewport();
				if( viewport == null )
					return;

				if( !GetEKeyByVirtualKey( virtualKey, out EKeys eKey ) )
					return;

				if( eventType == CoreAcceleratorKeyEventType.KeyDown || eventType == CoreAcceleratorKeyEventType.SystemKeyDown )
				{
					bool handled = false;
					var keyEvent = new KeyEvent( eKey );
					viewport.PerformKeyDown( keyEvent, ref handled );
					if( keyEvent.SuppressKeyPress )
					{
						//!!!!?

						//args.Handled = true;
					}

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
							if( EngineApp.WindowedMode == WindowedModeEnum.Fullscreen )
								EngineApp.SetWindowedMode( WindowedModeEnum.Windowed, EngineApp.WindowedModeSize );
							else
								EngineApp.SetWindowedMode( WindowedModeEnum.Fullscreen, EngineApp.WindowedModeSize );
							//EngineApp.SetFullscreenMode( !EngineApp.FullscreenEnabled, EngineApp.FullscreenSize );
							handled = true;
						}
					}

					if( handled )
						args.Handled = true;
				}
				else if( eventType == CoreAcceleratorKeyEventType.KeyUp || eventType == CoreAcceleratorKeyEventType.SystemKeyUp )
				{
					bool handled = false;
					viewport.PerformKeyUp( new KeyEvent( eKey ), ref handled );
					//args.Handled = handled;

					if( handled )
						args.Handled = true;
				}
				else if( eventType == CoreAcceleratorKeyEventType.Character || eventType == CoreAcceleratorKeyEventType.UnicodeCharacter )
				{
					char keyChar = (char)virtualKey;
					KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
					bool handled = false;
					GetViewport()?.PerformKeyPress( keyPressEvent, ref handled );
					//args.Handled = handled;

					if( handled )
						args.Handled = true;
				}
			} );

			//actionsToProcess.Enqueue( new ActionItem() { Name = "KeyActivated", EventType = args.EventType, VirtualKey = args.VirtualKey } );
		}

		//private void CoreWindow_KeyDown( CoreWindow sender, KeyEventArgs args )
		//{
		//}

		//private void CoreWindow_KeyUp( CoreWindow sender, KeyEventArgs args )
		//{
		//}

		//private void CoreWindow_CharacterReceived( CoreWindow sender, CharacterReceivedEventArgs args )
		//{
		//	//Log.Info( args.ToString() );

		//	//char keyChar = (char)args.KeyCode;

		//	//KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
		//	//bool handled = false;
		//	//GetViewport()?.PerformKeyPress( keyPressEvent, ref handled );
		//	////args.Handled = handled;
		//}

		private void CoreWindow_VisibilityChanged( CoreWindow sender, VisibilityChangedEventArgs args )
		{
			args.Handled = true;

			var visible = args.Visible;

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				windowVisible = visible;
				EngineApp.EnginePauseUpdateState( false, !visible );
			} );
		}

		private void CoreWindow_Activated( CoreWindow sender, WindowActivatedEventArgs args )
		{
			args.Handled = true;

			var windowActivationState = args.WindowActivationState;

			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				activationState = windowActivationState;

				if( windowActivationState == CoreWindowActivationState.Deactivated )
				{
					EngineApp.EnginePauseUpdateState( false, true );

					// do stuff
				}
				else
				{
					// do different stuff
				}

				instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
				instance.CreatedWindow_UpdateShowSystemCursor( true );
			} );
		}

		Viewport GetViewport()
		{
			if( !RenderingSystem.Disposed && RenderingSystem.ApplicationRenderTarget != null )
				return RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			else
				return null;
		}

		public override void CreatedWindow_DestroyWindow()
		{
			// empty for UWP?
		}


		public override void CreatedWindow_ProcessMessageEvents()
		{
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

		//void ProcessActions()
		//{
		//	while( actionsToProcess.TryDequeue( out var action ) )
		//	{
		//		switch( action.Name )
		//		{
		//		case "PointerPressed":
		//			{
		//				var button = action.Button;

		//				bool handled = false;
		//				GetViewport()?.PerformMouseDown( button, ref handled );

		//				//double click
		//				var time = EngineApp.GetSystemTime();//GetSystemTime();
		//				if( time - lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] < 0.5 )
		//				{
		//					bool handled2 = false;
		//					GetViewport()?.PerformMouseDoubleClick( button, ref handled2 );
		//					lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = 0;
		//				}
		//				else
		//					lastMouseButtonClickTimeForDoubleClickDetection[ (int)button ] = time;
		//			}
		//			break;

		//		case "PointerReleased":
		//			{
		//				var button = action.Button;

		//				bool handled = false;
		//				GetViewport()?.PerformMouseUp( button, ref handled );
		//			}
		//			break;

		//		case "KeyActivated":
		//			{
		//				var eventType = action.EventType;
		//				var virtualKey = action.VirtualKey;

		//				var viewport = GetViewport();
		//				if( viewport != null && GetEKeyByVirtualKey( virtualKey, out var eKey ) )
		//				{
		//					if( eventType == CoreAcceleratorKeyEventType.KeyDown || eventType == CoreAcceleratorKeyEventType.SystemKeyDown )
		//					{
		//						bool handled = false;
		//						var keyEvent = new KeyEvent( eKey );
		//						viewport.PerformKeyDown( keyEvent, ref handled );
		//						//!!!!
		//						if( keyEvent.SuppressKeyPress )
		//						{
		//							//args.Handled = true;
		//						}

		//						if( !handled && EngineApp.InitSettings.AllowChangeScreenVideoMode )
		//						{
		//							//support Alt+F4 in mouse relative mode. Alt+F4 is disabled during captured cursor.
		//							if( viewport.MouseRelativeMode )
		//							{
		//								if( eKey == EKeys.F4 && viewport.IsKeyPressed( EKeys.Alt ) )
		//								{
		//									EngineApp.NeedExit = true;
		//									return;
		//								}
		//							}

		//							if( viewport.IsKeyPressed( EKeys.Alt ) && eKey == EKeys.Return )
		//							{
		//								if( EngineApp.WindowedMode == WindowedModeEnum.Fullscreen )
		//									EngineApp.SetWindowedMode( WindowedModeEnum.Windowed, EngineApp.WindowedModeSize );
		//								else
		//									EngineApp.SetWindowedMode( WindowedModeEnum.Fullscreen, EngineApp.WindowedModeSize );
		//								//EngineApp.SetFullscreenMode( !EngineApp.FullscreenEnabled, EngineApp.FullscreenSize );
		//								handled = true;
		//							}
		//						}
		//					}
		//					else if( eventType == CoreAcceleratorKeyEventType.KeyUp || eventType == CoreAcceleratorKeyEventType.SystemKeyUp )
		//					{
		//						bool handled = false;
		//						viewport.PerformKeyUp( new KeyEvent( eKey ), ref handled );
		//					}
		//					else if( eventType == CoreAcceleratorKeyEventType.Character || eventType == CoreAcceleratorKeyEventType.UnicodeCharacter )
		//					{
		//						char keyChar = (char)virtualKey;
		//						KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
		//						bool handled = false;
		//						GetViewport()?.PerformKeyPress( keyPressEvent, ref handled );
		//					}
		//				}
		//			}
		//			break;
		//		}
		//	}
		//}

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

					////process actions
					//ProcessActions();

					EngineApp.UpdateEngineTime();
					double engineEime = EngineApp.EngineTime;
					bool needSleep = EngineApp.MaxFPS != 0 && engineEime < maxFPSLastRenderTime + 1.0f / EngineApp.MaxFPS;

					if( needSleep )
					{
						//!!!!0?
						Thread.Sleep( 1 );
					}
					else
					{
						maxFPSLastRenderTime = engineEime;

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
					//actionsToProcess.Clear();
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

		public override bool ApplicationIsActive()
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

		public override void SetWindowVisible( bool value )
		{
			//!!!!impl
		}

		public override InputDeviceManager CreateInputDeviceManager()
		{
			return new UWPInputDeviceManager( EngineApp.CreatedInsideEngineWindow.Handle );
		}
	}
}