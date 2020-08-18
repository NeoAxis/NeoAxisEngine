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
using System.Linq;
using NeoAxis.Input;
using NeoAxis.Editor;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Base class for the working the engine.
	/// </summary>
	public sealed class EngineApp
	{
		//general

		static EngineApp instance;
		static ApplicationTypeEnum applicationType;
		static internal PlatformFunctionality platform;

		//application window
		static IntPtr applicationWindowHandle;
		static CreatedInsideEngineWindowClass createdInsideEngineWindow;

		//!!!!
		static volatile bool created;
		static volatile bool closing;
		static volatile bool needExit;

		//config
		static volatile bool needSaveConfig;

		//fullscreen mode
		//lastValidWindowRectangleInWindowedMode
		//lastValisFullScreenSize
		//fullscreen mode
		//!!!!!
		static bool fullscreenEnabled;
		static Vector2I fullscreenSize;//when fullscreen mode is disabled this field is used to remember last fullscreen size.
		static bool mustChangeVideoMode;
		//Vec2I videoMode;
		//bool videoModeWasChangedOutside;
		//bool fullScreen;
		//internal Vec2I lastFullScreenWindowSize;
		//bool renderWindowInFullscreen;
		//Vec2I lastWindowSize;

		//cursor
		//это скорее App & Window Management
		static bool showCursor = true;
		static string systemCursorFileName = "";
		static Vector2 lastMousePositionForCursorUpdate;

		//rendering
		static bool duringRenderScene;

		//engine paused
		static bool enginePaused;
		static bool enginePausedFromOutsideEngineApp;
		static bool enginePauseWhenApplicationIsNotActive = true;
		//bool enginePauseWhenApplicationIsMinimized;

		//App & window management
		static SoundChannelGroup defaultSoundChannelGroup;
		static internal bool insideRunMessageLoop;

		//calculate and show FPS
		static int fpsCalcFrames;
		static uint fpsStartTime;
		static double fps;
		static volatile bool showFPS;
		static double lastEngineTimeToCalculateFPS;

		//time management
		static double startTime;
		static double addToResultTime;
		static double engineTimeScale = 1;
		static double engineTime;
		static bool engineTimeManualValueAndDisableAutoUpdate;
		static object timeLocker = new object();

		////gamma
		//double gamma = 1.0f;
		//bool gammChanged;

		//parameters
		static double maxFPS;

		//system video mode
		//!!!!!
		static bool videoModeChanged;

		//internal const double splashScreenTotalTime = 4;
		internal static double splashScreenStartTime;

		static string license = "Personal";
		static internal volatile bool needReadLicenseCertificate;

		static Assembly projectAssembly;

		//PerformanceCounter.TimeCounter renderPerformanceCounter = new PerformanceCounter.TimeCounter( "Render", false, new ColorValue( 0, 0, 1 ), 0 );
		//PerformanceCounter.TimeCounter soundPerformanceCounter = new PerformanceCounter.TimeCounter( "Sound", false, new ColorValue( 1, 1, 1 ), 1 );

		///////////////////////////////////////////

		//!!!!так и оставить?
		/// <summary>
		/// Represents engine's initialization settings.
		/// </summary>
		public static class InitSettings
		{
			//!!!!!все заюзать

			static string configVirtualFileName;

			static bool useDirectInputForMouseRelativeMode = true;
			static bool allowJoysticksAndSpecialInputDevices = true;
			//!!!!!странный флаг
			static bool allowChangeScreenVideoMode;
			static bool? multiMonitorMode;//!!!!!!тестить. смотреть где юзаются параметры

			static string language = "";//"Autodetect";
			static bool? localizeEngine = null;
			static bool? localizeToolset = null;

			static IntPtr useApplicationWindowHandle;
			static bool? createWindowFullscreen = null;
			static WindowStateEnum? createWindowState = null;
			static Vector2I? createWindowPosition = null;
			static Vector2I? createWindowSize = null;
			//static bool createWindowFullscreenAllowChangeDisplayFrequency = true;

			//static bool renderingVerticalSync = true;//!!!!что по дефолту? в редакторах включать?
			//static string renderingDeviceName = "";
			//static int renderingDeviceIndex;
			//static bool renderingDirect3DFPUPreserve = true;//!!!!!!выключив быстрее будет. проверить.
			//static RendererWorld.FilteringMode renderingFilteringMode = RendererWorld.FilteringMode.RecommendedSetting;
			//static bool renderingAllowDirectX9Ex;
			//!!!!для debug geometry потом. на гуй не влияет?
			//static int renderingHardwareFullscreenAntialiasing;

			//static bool allowWriteEngineConfigFile;

			///////////////

			public static string ConfigVirtualFileName
			{
				get { return configVirtualFileName; }
				set
				{
					if( instance != null )
						Log.Fatal( "EngineApp: InitializationParameters: set ConfigVirtualFileName: Can't change config file name after initialization (after EngineApp.Init())." );
					configVirtualFileName = VirtualPathUtility.NormalizePath( value );
				}
			}

			public static bool UseDirectInputForMouseRelativeMode
			{
				get { return useDirectInputForMouseRelativeMode; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					//	SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					useDirectInputForMouseRelativeMode = value;
				}
			}

			public static bool AllowJoysticksAndSpecialInputDevices
			{
				get { return allowJoysticksAndSpecialInputDevices; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					allowJoysticksAndSpecialInputDevices = value;
				}
			}

			public static bool AllowChangeScreenVideoMode
			{
				get { return allowChangeScreenVideoMode; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					allowChangeScreenVideoMode = value;
				}
			}

			public static bool? MultiMonitorMode
			{
				get { return multiMonitorMode; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					//	SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					multiMonitorMode = value;
				}
			}

			public static string Language
			{
				get { return language; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					language = value;
				}
			}

			public static bool? LocalizeEngine
			{
				get { return localizeEngine; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					localizeEngine = value;
				}
			}

			public static bool? LocalizeToolset
			{
				get { return localizeToolset; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					localizeToolset = value;
				}
			}

			public static IntPtr UseApplicationWindowHandle
			{
				get { return useApplicationWindowHandle; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					useApplicationWindowHandle = value;
				}
			}

			public static bool? CreateWindowFullscreen
			{
				get { return createWindowFullscreen; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					createWindowFullscreen = value;
				}
			}

			public static WindowStateEnum? CreateWindowState
			{
				get { return createWindowState; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					createWindowState = value;
				}
			}

			public static Vector2I? CreateWindowPosition
			{
				get { return createWindowPosition; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					createWindowPosition = value;
				}
			}

			public static Vector2I? CreateWindowSize
			{
				get { return createWindowSize; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					createWindowSize = value;
				}
			}

			//public static bool CreateWindowFullscreenAllowChangeDisplayFrequency
			//{
			//	get { return createWindowFullscreenAllowChangeDisplayFrequency; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		createWindowFullscreenAllowChangeDisplayFrequency = value;
			//	}
			//}

			//public static bool RenderingVerticalSync
			//{
			//	get { return renderingVerticalSync; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingVerticalSync = value;
			//	}
			//}

			//public static string RenderingDeviceName
			//{
			//	get { return renderingDeviceName; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingDeviceName = value;
			//	}
			//}

			//public static int RenderingDeviceIndex
			//{
			//	get { return renderingDeviceIndex; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingDeviceIndex = value;
			//	}
			//}

			//public static bool RenderingDirect3DFPUPreserve
			//{
			//	get { return renderingDirect3DFPUPreserve; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingDirect3DFPUPreserve = value;
			//	}
			//}

			//public static RendererWorld.FilteringMode RenderingFilteringMode
			//{
			//	get { return renderingFilteringMode; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingFilteringMode = value;
			//	}
			//}

			//public static bool RenderingAllowDirectX9Ex
			//{
			//	get { return renderingAllowDirectX9Ex; }
			//	set
			//	{
			//		if( EngineApp.Instance != null && EngineApp.Instance.IsCreated )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingAllowDirectX9Ex = value;
			//	}
			//}

			///// <summary>
			///// Standard engine's rendering pipeline is not using this mode. Full screen antialiasing is implemented as post effect (FXAA).
			///// </summary>
			//public static int RenderingHardwareFullscreenAntialiasing
			//{
			//	get { return renderingHardwareFullscreenAntialiasing; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		renderingHardwareFullscreenAntialiasing = value;
			//	}
			//}
		}

		///////////////////////////////////////////

		//!!!!!было надо
		//class MainViewportClassForCreatedWindow : MainViewportInterface
		//{
		//	public override Vec2 MousePosition
		//	{
		//		get
		//		{
		//			if( MouseRelativeMode )
		//				return new Vec2( .5f, .5f );
		//			return instance.platform.CreatedWindow_GetMousePosition();
		//		}
		//		set
		//		{
		//			if( instance.created && !instance.closing && instance.platform.IsFocused() )
		//				instance.platform.CreatedWindow_SetMousePosition( value );
		//		}
		//	}

		///////////////////////////////////////////

		/// <summary>
		/// Provides data for case when application window is created by the engine.
		/// </summary>
		public sealed class CreatedInsideEngineWindowClass
		{
			////!!!!инитить
			//Viewport viewport;

			string title = "NeoAxis Player";
			Icon icon;
			Icon iconSmall;//!!!!!так?

			//!!!!всё тут

			//public Viewport Viewport
			//{
			//	get { return viewport; }
			//}

			internal void Dispose()
			{
				if( iconSmall != null )
				{
					iconSmall.Dispose();
					iconSmall = null;
				}
			}

			public IntPtr Handle
			{
				get { return applicationWindowHandle; }
			}

			public bool Active
			{
				get { return platform.CreatedWindow_IsWindowActive(); }
			}

			public bool Focused
			{
				get { return platform.IsFocused(); }
			}

			public WindowStateEnum State
			{
				get { return (WindowStateEnum)platform.GetWindowState(); }
				set { platform.SetWindowState( (PlatformFunctionality.WindowState)value ); }
			}

			public RectangleI Rectangle
			{
				get { return platform.CreatedWindow_GetWindowRectangle(); }
				set
				{
					//!!!!!
					if( FullscreenEnabled )
					{
						Log.Warning( "EngineApp: ApplicationWindow: set Rectangle: Can't change in fullscreen mode." );
						return;
					}

					platform.CreatedWindow_SetWindowRectangle( value );
				}
			}

			public Icon Icon
			{
				get { return icon; }
				set
				{
					if( icon == value )
						return;
					icon = value;

					//!!!!IsCreated && !IsClosing
					if( Created && !Closing )
						UpdateIcon();
				}
			}

			public string Title
			{
				get
				{
					//!!!!!получать реальный? в других местах тоже? иконки?
					return title;
				}
				set
				{
					title = value;
					platform.CreatedWindow_UpdateWindowTitle( title );
				}
			}

			internal void UpdateIcon()
			{
				Icon oldSmallIcon = iconSmall;

				if( icon != null )
				{
					try
					{
#if !ANDROID
						Vector2I smallIconSize = platform.GetSmallIconSize();
						if( smallIconSize != Vector2I.Zero )
							iconSmall = new Icon( icon, new Size( smallIconSize.X, smallIconSize.Y ) );
#endif //!ANDROID
					}
					catch { }
				}
				else
					iconSmall = null;

				platform.CreatedWindow_UpdateWindowIcon( iconSmall, icon );

				if( oldSmallIcon != null )
				{
					oldSmallIcon.Dispose();
					oldSmallIcon = null;
				}
			}

			public /*internal */void _ProcessMouseMoveEvent()
			{
				//!!!!всё тут обновить с учетом изменений в Viewport

				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

				//!!!!обязательно IsFocused?
				if( viewport.MouseRelativeMode && platform.IsFocused() && !InitSettings.UseDirectInputForMouseRelativeMode )
				{
					//!!!!для мака

					Vector2 delta;
					platform.CreatedWindow_UpdateMouseRelativeMove( out delta );
					viewport.PerformMouseMove( delta );
				}

				if( !viewport.MouseRelativeMode )
				{
					Vector2 mouse = platform.CreatedWindow_GetMousePosition();

					//было, не надо
					//bool lastInside = new Rectangle( 0, 0, 1, 1 ).Contains( lastMousePositionForCursorUpdate );
					//bool inside = new Rectangle( 0, 0, 1, 1 ).Contains( mouse );
					//if( platform.IsFocused() && lastInside != inside )
					//	platform.UpdateShowSystemCursor();

					viewport.PerformMouseMove( mouse );

					lastMousePositionForCursorUpdate = mouse;
				}
			}
		}

		///////////////////////////////////////////

		public delegate void WindowsWndProcDelegate( uint message, IntPtr wParam, IntPtr lParam, ref bool processMessageByEngine );
		public static event WindowsWndProcDelegate WindowsWndProc;

		///////////////////////////////////////////

		public enum WindowStateEnum
		{
			Maximized,
			Minimized,
			Normal
		}

		///////////////////////////////////////////

		public enum ApplicationTypeEnum
		{
			Unknown,
			Simulation,
			Editor,
		}

		///////////////////////////////////////////

		//public enum UserCustomMethodResult
		//{
		//   Success,
		//   NoSuchMethod,
		//   IllegalArgument,
		//   IllegalAccess,
		//   InvocationTarget,
		//   InvalidMethodResult,
		//   NotImplemented,
		//   CallMethodNotFound,
		//}

		///////////////////////////////////////////

		//public enum MainModuleMessages
		//{
		//   Init,
		//   Shutdown,
		//   WindowMessage,
		//   IsNeedExit,
		//   UserCustomMessage,
		//}

		///////////////////////////////////////////

		//public static bool Init( EngineApp overridedObject, AndroidLauncherInitData androidLauncherInitData )
		//{
		//   Trace.Assert( overridedObject != null, "overridedObject != null" );
		//   Trace.Assert( instance == null, "instance == null" );
		//   instance = overridedObject;
		//   bool ret = instance.InitInternal( androidLauncherInitData );
		//   if( !ret )
		//      Shutdown();
		//   return ret;
		//}

		public static bool Init()// EngineApp overridedObject )//, IntPtr mainModuleData  )
		{
			StartupTiming.CounterStart( "EngineApp init" );
			try
			{
				IntPtr mainModuleData = IntPtr.Zero;

				//if( overridedObject == null )
				//	Log.Fatal( "EngineApp: Init: overridedObject == null." );
				if( instance != null )
					Log.Fatal( "EngineApp: Init: instance != null." );
				instance = new EngineApp();
				//instance = overridedObject;
				bool ret = instance.InitInternal( mainModuleData );
				if( !ret )
					Shutdown();
				return ret;
			}
			finally
			{
				StartupTiming.CounterEnd( "EngineApp init" );
			}
		}

		public static void Shutdown()
		{
			if( instance != null )
			{
				instance.ShutdownInternal();
				instance = null;
			}
		}

		public static EngineApp Instance
		{
			get { return instance; }
		}

		public EngineApp()
		{
		}

		public static ApplicationTypeEnum ApplicationType
		{
			get { return applicationType; }
			set
			{
				if( applicationType != ApplicationTypeEnum.Unknown )
					Log.Fatal( "EngineApp: ApplicationType: set: applicationType != ApplicationTypes.Unknown." );
				applicationType = value;
			}
		}

		bool InitInternal( IntPtr mainModuleData )
		{
			platform = PlatformFunctionality.Get();

			//Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );

			if( ApplicationType == ApplicationTypeEnum.Editor )
				EditorAssembly.Init();

			platform.Init( mainModuleData );

			//logs
			{
				Log.InvisibleInfo( "Powered by NeoAxis" );

				string operationSystemDisplayName = "";
				{
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
						operationSystemDisplayName = "Microsoft Windows";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
						operationSystemDisplayName = "Apple Mac OS X";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						operationSystemDisplayName = "Google Android";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
						//!!!!
						operationSystemDisplayName = "UWP";
						//#if W__!!__INDOWS_UWP
						//var deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
						//operationSystemDisplayName = "UWP - " + deviceFamily; // it can be "Mobile", "Xbox"
						//			//operationSystemDisplayName = "Universal Windows";
						//#endif
					}
					//else if( PlatformInfo.Platform == PlatformInfo.Platforms.Android )
					//   operationSystemDisplayName = "Google Android";
					else
					{
						Log.Fatal( "EngineApp: InitInternal: Unknown platform." );
						return false;
					}
				}

				Log.InvisibleInfo( "Operating System: {0} {1}", operationSystemDisplayName, SystemSettings.OSVersion );
				Log.InvisibleInfo( "Runtime Framework: " + SystemSettings.GetNetRuntimeDisplayName() );
				Log.InvisibleInfo( "NeoAxis version: " + EngineInfo.Version.ToString() );
				Log.InvisibleInfo( "Application type: " + ApplicationType.ToString() );
			}

			//Window
			//было. теперь в .._PostFix()
			//fullScreen = allowChangeVideoMode;
			//videoMode = platform.GetScreenSize();

			//Timer
			startTime = GetSystemTime();

			//FPS
			fpsStartTime = (uint)startTime;

			//initialize Config
			if( !string.IsNullOrEmpty( InitSettings.ConfigVirtualFileName ) )
			{
				string error;
				if( !EngineConfig.Init( InitSettings.ConfigVirtualFileName, out error ) )
				{
					if( applicationType == ApplicationTypeEnum.Editor )
					{
						//!!!!было
						//EditorMessageBox.Result result = EditorMessageBox.Show( error, "Warning", EditorMessageBox.Buttons.OKCancel,
						//	EditorMessageBox.Icon.Warning );
						//if( result == EditorMessageBox.Result.OK )
						//	return true;
					}
					return false;
				}
			}

			InitializationParameters_InitFromEngineConfig();

			//!!!!?
			//!!!!всем в сборке зарегать прост?
			//enable support field and properties serialization for GameEngineApp class.
			EngineConfig.RegisterClassParameters( GetType() );// typeof( SimulationApp ) );

			//!!!!
			//ReadLicenseCertificate();

			//if( !CreateEngineInterfaceImpl() )
			//	return false;

			return true;
		}

		void ShutdownInternal()
		{
			PlatformFunctionality.WindowState windowState = PlatformFunctionality.WindowState.Normal;
			if( platform.IsWindowInitialized() )
			{
				try
				{
					windowState = platform.GetWindowState();
				}
				catch { }
			}

			Destroy();

			if( needSaveConfig )
			{
				if( !string.IsNullOrEmpty( InitSettings.ConfigVirtualFileName ) )
					EngineConfig.Save();

				//!!!!!было. как теперь?
				//!!!!!!!!!теперь в SimulationApp?
				////Update Configs/Engine.config (videoMode)
				//if( allowWriteEngineConfigFile )
				//{
				//   string fileName = PathUtils.GetRealPathByVirtual( "user:Configs/Engine.config" );

				//   string error;
				//   TextBlock engineConfigBlock = TextBlockUtils.LoadFromRealFile( fileName, out error );
				//   if( engineConfigBlock == null )
				//      engineConfigBlock = new TextBlock();

				//   TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
				//   if( rendererBlock == null )
				//      rendererBlock = engineConfigBlock.AddChild( "Renderer" );

				//   Vec2I size = VideoMode;
				//   if( windowState == PlatformFunctionality.WindowStates.Minimized && lastWindowSize != Vec2I.Zero )
				//      size = lastWindowSize;

				//   if( allowChangeVideoMode )
				//   {
				//      rendererBlock.SetAttribute( "fullScreen", FullScreen.ToString() );

				//      if( windowState == PlatformFunctionality.WindowStates.Maximized && !FullScreen )
				//      {
				//         rendererBlock.DeleteAttribute( "videoMode" );
				//      }
				//      else
				//      {
				//         if( !FullScreen || videoModeWasChangedOutside )
				//            rendererBlock.SetAttribute( "videoMode", size.ToString() );
				//      }
				//   }

				//   try
				//   {
				//      string directoryName = Path.GetDirectoryName( fileName );
				//      if( directoryName != "" && !Directory.Exists( directoryName ) )
				//         Directory.CreateDirectory( directoryName );

				//      using( StreamWriter writer = new StreamWriter( fileName ) )
				//      {
				//         writer.Write( engineConfigBlock.DumpToString() );
				//      }
				//   }
				//   catch
				//   {
				//      Log.Warning( "Unable to save file \"{0}\".", fileName );
				//   }
				//}
			}

			//native memory manager detect leaks
			NativeMemoryManager.LogLeaks();
		}

		void InitializationParameters_InitFromEngineConfig()
		{
			//!!!!!

			var configPath = VirtualPathUtility.GetRealPathByVirtual( "user:Configs/Engine.config" );
			if( !File.Exists( configPath ) )
				return; // use default values.

			string error;
			TextBlock engineConfigBlock = TextBlockUtility.LoadFromRealFile( configPath, out error );
			if( engineConfigBlock != null )
			{
				//Renderer
				TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
				if( rendererBlock != null )
				{
					//!!!!было
					//if( string.IsNullOrEmpty( InitSettings.RenderingSystemComponent ) )
					//	InitSettings.RenderingSystemComponent = rendererBlock.GetAttribute( "implementationComponent" );

					//if( string.IsNullOrEmpty( InitSettings.RenderingDeviceName ) )
					//{
					//	if( rendererBlock.AttributeExists( "renderingDeviceName" ) )
					//		InitSettings.RenderingDeviceName = rendererBlock.GetAttribute( "renderingDeviceName" );
					//	if( rendererBlock.AttributeExists( "renderingDeviceIndex" ) )
					//		InitSettings.RenderingDeviceIndex = int.Parse( rendererBlock.GetAttribute( "renderingDeviceIndex" ) );
					//}

					//if( rendererBlock.IsAttributeExist( "fullSceneAntialiasing" ) )
					//{
					//   RendererWorld.InitializationOptions.FullSceneAntialiasing = rendererBlock.GetAttribute( "fullSceneAntialiasing" );
					//}

					//if( rendererBlock.AttributeExists( "filtering" ) )
					//{
					//	try
					//	{
					//		InitSettings.RenderingFilteringMode = (RendererWorld.FilteringMode)
					//			Enum.Parse( typeof( RendererWorld.FilteringMode ), rendererBlock.GetAttribute( "filtering" ) );
					//	}
					//	catch { }
					//}

					//if( rendererBlock.AttributeExists( "verticalSync" ) )
					//	InitSettings.RenderingVerticalSync = bool.Parse( rendererBlock.GetAttribute( "verticalSync" ) );

					//!!!!!!это выставлять из SimulationApp?
					//!!!!!!!!!!там и хранить в одном месте хранить размер экрана для симуляции?

					//if( InitializationParameters.AllowChangeScreenVideoMode )
					//{
					//   if( rendererBlock.IsAttributeExist( "fullScreen" ) )
					//      FullScreen = bool.Parse( rendererBlock.GetAttribute( "fullScreen" ) );
					//   if( InitializationParameters.MultiMonitorMode == null )
					//   {
					//      if( rendererBlock.IsAttributeExist( "multiMonitorMode" ) )
					//         InitializationParameters.MultiMonitorMode = bool.Parse( rendererBlock.GetAttribute( "multiMonitorMode" ) );
					//   }
					//   if( rendererBlock.IsAttributeExist( "videoMode" ) )
					//   {
					//      try
					//      {
					//         VideoMode = Vec2I.Parse( rendererBlock.GetAttribute( "videoMode" ) );
					//      }
					//      catch { }
					//   }
					//}
				}

				//SoundSystem
				TextBlock soundSystemBlock = engineConfigBlock.FindChild( "SoundSystem" );
				if( soundSystemBlock != null )
				{
					//!!!!было
					//if( string.IsNullOrEmpty( InitSettings.SoundSystemComponent ) )
					//	InitSettings.SoundSystemComponent = soundSystemBlock.GetAttribute( "implementationComponent" );
				}

				//localization
				TextBlock localizationBlock = engineConfigBlock.FindChild( "Localization" );
				if( localizationBlock != null )
				{
					if( string.IsNullOrEmpty( InitSettings.Language ) )
					{
						if( localizationBlock.AttributeExists( "language" ) )
							InitSettings.Language = localizationBlock.GetAttribute( "language" );
					}
					if( InitSettings.LocalizeEngine == null )
					{
						if( localizationBlock.AttributeExists( "localizeEngine" ) )
							InitSettings.LocalizeEngine = bool.Parse( localizationBlock.GetAttribute( "localizeEngine" ) );
					}
					if( InitSettings.LocalizeToolset == null )
					{
						if( localizationBlock.AttributeExists( "localizeToolset" ) )
							InitSettings.LocalizeToolset = bool.Parse( localizationBlock.GetAttribute( "localizeToolset" ) );
					}
				}

				//physics
				TextBlock physicsSystemBlock = engineConfigBlock.FindChild( "PhysicsSystem" );
				if( physicsSystemBlock != null )
				{
					//!!!!было
					//if( string.IsNullOrEmpty( InitSettings.PhysicsSystemComponent ) )
					//	InitSettings.PhysicsSystemComponent = physicsSystemBlock.GetAttribute( "implementationComponent" );
				}
			}
		}

		static void InitializationParameters_PostFix()
		{
			//!!!!!

			//!!!!было
			////Deployed: get language from deployment parameters
			//if( VirtualFileSystem.Deployed )
			//{
			//	if( string.IsNullOrEmpty( InitSettings.Language ) )
			//	{
			//		if( !string.IsNullOrEmpty( VirtualFileSystem.DeploymentParameters.DefaultLanguage ) )
			//			InitSettings.Language = VirtualFileSystem.DeploymentParameters.DefaultLanguage;
			//	}
			//}

			//detect language
			if( string.IsNullOrEmpty( InitSettings.Language ) || string.Compare( InitSettings.Language, "autodetect", true ) == 0 )
			{
				string systemLanguageName;
				string systemEnglishName;
				GetSystemLanguage( out systemLanguageName, out systemEnglishName );

				string name = systemLanguageName;//CultureInfo.CurrentUICulture.EnglishName;

				List<string> languages = new List<string>();
				{
					string[] directories = VirtualDirectory.GetDirectories( LanguageManager.LanguagesDirectory, "*.*", SearchOption.TopDirectoryOnly );
					foreach( string directory in directories )
					{
						string lang = Path.GetFileNameWithoutExtension( directory );
						languages.Add( lang );
					}
				}

				//find by exact name
				foreach( string lang in languages )
				{
					if( string.Compare( lang, name, true ) == 0 )
					{
						InitSettings.Language = lang;
						goto end;
					}
				}

				//find by including substring
				foreach( string lang in languages )
				{
					if( name.ToLower().Contains( lang.ToLower() ) )
					{
						InitSettings.Language = lang;
						goto end;
					}
				}

				//take English if available
				foreach( string lang in languages )
				{
					if( string.Compare( lang, "English", true ) == 0 )
					{
						InitSettings.Language = lang;
						goto end;
					}
				}

				//take first in list
				if( languages.Count > 0 )
				{
					InitSettings.Language = languages[ 0 ];
					goto end;
				}

				InitSettings.Language = "English";

				end:;
			}

			//init null parameters
			{
				if( InitSettings.MultiMonitorMode == null )
					InitSettings.MultiMonitorMode = false;
				if( InitSettings.LocalizeEngine == null )
					InitSettings.LocalizeEngine = true;
				if( InitSettings.LocalizeToolset == null )
					InitSettings.LocalizeToolset = true;

				if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero )
				{
					if( InitSettings.CreateWindowFullscreen == null )
					{
						//fullscreen
						if( SystemSettings.CommandLineParameters.TryGetValue( "-fullscreen", out var fullscreenStr ) )
						{
							try
							{
								InitSettings.CreateWindowFullscreen = (bool)SimpleTypes.ParseValue( typeof( bool ), fullscreenStr );
							}
							catch { }
						}

						//windowed
						if( SystemSettings.CommandLineParameters.TryGetValue( "-windowed", out var windowedStr ) )
						{
							try
							{
								InitSettings.CreateWindowFullscreen = !(bool)SimpleTypes.ParseValue( typeof( bool ), windowedStr );
							}
							catch { }
						}

						if( InitSettings.CreateWindowFullscreen == null )
						{
							if( ApplicationType == ApplicationTypeEnum.Simulation )
								InitSettings.CreateWindowFullscreen = true;
							else
								InitSettings.CreateWindowFullscreen = false;
						}
					}

					if( InitSettings.CreateWindowState == null )
						InitSettings.CreateWindowState = WindowStateEnum.Maximized;
					if( InitSettings.CreateWindowSize == null )
						InitSettings.CreateWindowSize = platform.GetScreenSize();

					if( InitSettings.CreateWindowPosition == null )
					{
						InitSettings.CreateWindowPosition = new Vector2I( 0, 0 );
						if( InitSettings.CreateWindowState.Value == WindowStateEnum.Normal )
						{
							InitSettings.CreateWindowPosition =
								( platform.GetScreenSize() - InitSettings.CreateWindowSize.Value ) / 2;
						}
					}

					if( InitSettings.CreateWindowFullscreen.Value )
						InitSettings.CreateWindowPosition = new Vector2I( 0, 0 );
				}
			}
		}

		public static bool Create()
		{
			StartupTiming.CounterStart( "EngineApp create" );
			try
			{
				if( created )
					Log.Fatal( "EngineApp: Create: The application is already created." );

				//Project.csproj. load cs files, compile.
				CompileAndLoadProjectAssembly( "Project" );

				AppCreateBefore?.Invoke();

				//instance.OnBeginAppCreation();
				needSaveConfig = true;

				InitializationParameters_PostFix();

				//change video mode
				if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero )
				{
					if( InitSettings.CreateWindowFullscreen.Value )
						platform.FullscreenFadeOut( false );

					//!!!!new: InitSettings.AllowChangeScreenVideoMode
					if( InitSettings.AllowChangeScreenVideoMode && InitSettings.CreateWindowFullscreen.Value && !InitSettings.MultiMonitorMode.Value )
					{
						if( SystemSettings.ChangeVideoMode( InitSettings.CreateWindowSize.Value ) )
						{
							fullscreenEnabled = true;
							fullscreenSize = InitSettings.CreateWindowSize.Value;
							//lastFullScreenWindowSize = videoMode;
						}
						else
						{
							//!!!!!что тут?
							//videoMode = platform.GetScreenSize();
						}
					}
				}

				if( !WindowCreateOrAttach() )
					return false;

				if( createdInsideEngineWindow != null )
				{
					//!!!!!?
					Log.Handlers.WarningHandler += Log_WarningHandler;
					Log.Handlers.ErrorHandler += Log_ErrorHandler;
					Log.Handlers.FatalHandler += Log_FatalHandler;
					Log.AfterFatal += Log_AfterFatal;
				}

				//physics
				{
					//!!!!temp
					//bool allowHardwareAcceleration = false;
					//PhysicsWorld.Init( allowHardwareAcceleration );

					BulletPhysicsUtility.InitLibrary();
				}

				//joysticks and special input devices
				if( InitSettings.AllowJoysticksAndSpecialInputDevices )
				{
					InputDeviceManager instance = null;

					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
					{
#if WINDOWS
						instance = new WindowsInputDeviceManager( applicationWindowHandle );
#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
					{
						Log.Fatal( "MacOSXInputDeviceManager impl." );
						//instance = new MacOSXInputDeviceManager();
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
#if UWP
						instance = new UWPInputDeviceManager( applicationWindowHandle );
#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					{
						//instance = new AndroidInputDeviceManager();
					}
					else
						Log.Fatal( "EngineApp: Init InputDeviceManager: Unknown platform." );

					if( instance != null )
					{
						if( !InputDeviceManager.Init( instance, InputDeviceManager_InputEventHandler ) )
						{
							//return false;
						}
					}
				}

				//DirectInput mouse device
				// not implemented for UWP now.
				if( ( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP ) &&
					InitSettings.UseDirectInputForMouseRelativeMode )
				{
					if( !platform.InitDirectInputMouseDevice() )
						InitSettings.UseDirectInputForMouseRelativeMode = false;
				}

				//Language initialize
				if( !LanguageManager.Init( InitSettings.Language, InitSettings.LocalizeEngine.Value ) )
					return false;

				////override rendering API
				//try
				//{
				//	var renderingAPI = ProjectSettings.Get.RenderingAPI.Value;
				//	switch( renderingAPI )
				//	{
				//	case Component_ProjectSettings.RenderingAPIEnum.DirectX11: EngineSettings.Init.RendererBackend = RendererBackend.Direct3D11; break;
				//	case Component_ProjectSettings.RenderingAPIEnum.DirectX12: EngineSettings.Init.RendererBackend = RendererBackend.Direct3D12; break;
				//	}
				//}
				//catch { }

				//OnBeforeRendererWorldInit();

				//renderWindowInFullscreen = startedAtFullScreen;

				//Renderer init
				StartupTiming.CounterStart( "Rendering system init" );
				{
					bool startedAtFullscreen = FullscreenEnabled;
					if( Debugger.IsAttached )
						startedAtFullscreen = false;

					if( !RenderingSystem.Init( startedAtFullscreen, InitSettings.MultiMonitorMode.Value, InitSettings.Language ) )
						return false;
				}
				StartupTiming.CounterEnd( "Rendering system init" );

				////check for DirectX debug version
				//if( RenderSystem.Instance.Name.Contains( "Direct3D" ) )
				//{
				//	bool debugVersion = Array.Exists( GetNativeModuleNames(), delegate ( string fileName )
				//	{
				//		string baseName = Path.GetFileName( fileName );
				//		if( string.Compare( baseName, "d3d9d.dll", true ) == 0 )
				//			return true;
				//		return false;
				//	} );
				//	if( debugVersion )
				//	{
				//		DoHideAnyEditorSplashForms();
				//		string text = "Debug version of Direct3D is selected in the system. Engine can work unstable.\n\nContinue?";
				//		if( !platform.ShowMessageBoxYesNoQuestion( text, "Warning" ) )
				//			return false;
				//	}
				//}

				//!!!!!
				//if( applicationWindowCreatedInsideEngine )
				//	MainViewport_Change( RendererWorld.ApplicationRenderTarget, new MainViewportClassForCreatedWindow() );

				//!!!!!откуда выставлять?
				//UpdateGamma();

				//!!!!было
				//if( !UIControlsWorld.Init() )
				//	return false;

				//instance.OnAfterRendererWorldInit();

				if( FullscreenEnabled )
					platform.FullscreenFadeIn( false );

				//!!!!!!было, но не надо
				//if( HighLevelMaterialManager.Instance.NeedLoadAllMaterialsAtStartup )
				//{
				//	if( !HighLevelMaterialManager.Instance.LoadAllMaterials() )
				//		return false;
				//}

				//RendererWorld.BeginRenderFrame += RendererWorld_BeginRenderFrame;

				RenderingSystem._PostInitRendererAddition();

				SoundWorld.Init( applicationWindowHandle );

				defaultSoundChannelGroup = SoundWorld.CreateChannelGroup( "Sound" );
				if( defaultSoundChannelGroup != null )
					SoundWorld.MasterChannelGroup.AddGroup( defaultSoundChannelGroup );

				//reset sound listener
				SoundWorld.SetListenerReset();

				//!!!! initialize here or on first use ?
				// scripting init.
				//Scripting.ScriptingCSharpEngine.Init();

				fpsStartTime = (uint)(float)( EngineTime * 1000.0f );

				//ShowCursor = true;

				//!!!!!было
				//if( !EngineInterface.instance.OnPostInitialize() )
				//   return false;

				//process message loop events
				if( createdInsideEngineWindow != null )
					_ProcessApplicationMessageEvents();

				//!!!!тут?
				//if( !SceneManager.Init() )
				//	return false;

				//!!!!!стало ниже
				//if( !OnCreate() )
				//   return false;

				created = true;

				//			//Project.csproj. load cs files, compile.
				//			//!!!!new
				//#if W__!!__INDOWS_UWP
				//			CompileAndLoadProjectAssembly( "Project.UWP" ); // move name to config ?
				//#else
				//			CompileAndLoadProjectAssembly( "Project" );
				//#endif
				//before
				//CSharpProjectFileUtility.GetProjectFileCSFiles( true, false );
				//CSharpProjectFileUtility.CheckToRemoveNotExistsFilesFromProject();
				//if( EngineSettings.Init.ScriptingCompileProjectSolutionAtStartup )
				//	CSharpProjectFileUtility.CompileProjectSolution( false, false, out bool skipped );

				VirtualFileSystem.RegisterAssemblies_IncludingFromDefaultSettingConfig();

				//!!!!было
				if( createdInsideEngineWindow != null && createdInsideEngineWindow.Focused )
					platform.CreatedWindow_UpdateShowSystemCursor( true );

				if( createdInsideEngineWindow != null )
					_CreatedWindow_ProcessResize();
				//DoResize();

				lastEngineTimeToCalculateFPS = EngineTime;

				//is not used
				//EngineConfig.RegisterClassParameters( typeof( EngineSettings ) );

				AppCreateAfter?.Invoke();
				//if( !instance.OnAppCreated() )
				//	return false;

			}
			finally
			{
				StartupTiming.CounterEnd( "EngineApp create" );
			}

			StartupTiming.TotalEnd();
			//write to logs
			foreach( var line in StartupTiming.GetStatisticsAsStringLines() )
				Log.InvisibleInfo( line );

			//Log.Info( "Net Types: " + MetadataManager.NetTypes.Count.ToString() );
			//string s = "";
			//int c = 0;
			//foreach( var n in MetadataManager.added )
			//{
			//	s += "\tNamespace { Name = " + n + " }\r\n";
			//	c++;
			//	if( c > 20 )
			//	{
			//		c = 0;
			//		Log.Info( s );
			//		s = "";
			//	}
			//}
			//Log.Info( s );
			//Log.Info( "---" );

			return true;
		}

		private static void CompileAndLoadProjectAssembly( string projectName, bool rebuild = false )
		{
#if !DEPLOY

			bool canCompile = true;

			//check dotnet available
			{
				var folder = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "dotnet" );
				if( !Directory.Exists( folder ) )
					canCompile = false;
			}

			if( canCompile )
			{
				CSharpProjectFileUtility.Init( projectName );
				CSharpProjectFileUtility.GetProjectFileCSFiles( true, false );
				CSharpProjectFileUtility.CheckToRemoveNotExistsFilesFromProject();

				// compile
				string outputAssemblyName = CSharpProjectFileUtility.OutputAssemblyName;
				if( EngineSettings.Init.ScriptingCompileProjectSolutionAtStartup )
					outputAssemblyName = CSharpProjectFileUtility.CompileIfRequired( rebuild, applicationType == ApplicationTypeEnum.Editor );

				// and load
				string fullPath = Path.Combine( CSharpProjectFileUtility.OutputDir, outputAssemblyName + ".dll" );
				projectAssembly = AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true, loadWithoutLocking: true );
			}
			else
			{
				string fullPath = Path.Combine( VirtualFileSystem.Directories.Binaries, projectName + ".dll" );
				projectAssembly = AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true, loadWithoutLocking: true );
			}

#else
			string fullPath = Path.Combine( VirtualFileSystem.Directories.Binaries, projectName + ".dll" );
			projectAssembly = AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
#endif
		}

		static void RestoreVideoModeAndMinimize()
		{
			if( !InitSettings.MultiMonitorMode.HasValue || !InitSettings.MultiMonitorMode.Value )
				SystemSettings.RestoreVideoMode();
			platform.SetWindowState( PlatformFunctionality.WindowState.Minimized );
		}

		static void Log_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				if( !created )
				{
					if( createdInsideEngineWindow != null && FullscreenEnabled )
						RestoreVideoModeAndMinimize();
				}
			}
		}

		static void Log_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				if( !created )
				{
					if( createdInsideEngineWindow != null && FullscreenEnabled )
						RestoreVideoModeAndMinimize();
				}
			}
		}

		static void Log_FatalHandler( string text, string createdLogFilePath, ref bool handled )
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				if( createdInsideEngineWindow != null && FullscreenEnabled )
					RestoreVideoModeAndMinimize();
			}
		}

		static void Log_AfterFatal()
		{
			if( Instance != null )
				Destroy();
		}

		static void InputDeviceManager_InputEventHandler( InputEvent e )
		{
			JoystickInputEvent joystickInputEvent = e as JoystickInputEvent;
			if( joystickInputEvent != null )
			{
				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

				bool handled = false;
				viewport.PerformJoystickEvent( joystickInputEvent, ref handled );

				//MainViewport._CallJoystickEvent( joystickInputEvent );

				return;
			}

			//!!!!
			//MainViewport._CallSpecialInputDeviceEvent( e );
		}

		static void ProcessChangingVideoMode()
		{
			if( EnginePauseWhenApplicationIsNotActive )
			{
				//disable simulation
				_EnginePause_UpdateState( true, true );
			}

			platform.ProcessChangingVideoMode();
		}

		static public /*internal */void _CreatedWindow_ApplicationIdle( bool doTickOnly )
		{
			if( created && !closing )
			{
				//change video mode
				if( mustChangeVideoMode && !doTickOnly && !InitSettings.MultiMonitorMode.Value )
				{
					mustChangeVideoMode = false;
					ProcessChangingVideoMode();
					return;
				}

				//system pause reset
				_EnginePause_UpdateState( false, true );
				//if( systemPause && applicationWindowCreated && platform.IsWindowActive() && !platform.IsIntoMenuLoop() )
				//   EnginePause_UpdateState( false, true );

				//tick and render scene
				if( !doTickOnly )
				{
					if( !RenderingSystem.IsDeviceLostByTestCooperativeLevel() )
					{
						//!!!!раньше тут не было. может и не будет
						//try to restore device lost
						if( RenderingSystem.IsDeviceLost() )
						{
							//!!!!
							RenderingSystem.RestoreDeviceAfterLost();
							return;
							//if( !RenderSystem.Instance.RestoreDeviceAfterLost() )
							//	return;
						}

						//PerformanceCounter.TotalTimeCounter.Start();

						DoTick();

						if( !needExit )
						{
							//renderPerformanceCounter.Start();
							RenderSceneInternal();
							//renderPerformanceCounter.End();
						}

						//PerformanceCounter.TotalTimeCounter.End();
					}
				}
				else
				{
					DoTick();
				}
			}
		}

		public static bool NeedExit
		{
			get { return needExit; }
			set { needExit = value; }
		}

		public static void Destroy()
		{
			if( instance == null )
				return;

			//if( FullScreen )
			//   platform.FullscreenFadeOut( true );

			_EnginePause_UpdateState( true, true );
			closing = true;

			//!!!!!было. надо?
			//if( createdInsideEngineWindow != null )
			//{
			//	Viewport viewport = RendererWorld.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			//	viewport.MouseRelativeMode = false;
			//}

			//!!!!
			SystemSettings.ResetGamma();
			//Gamma = 1.0f;

			AppDestroy?.Invoke();
			//instance.OnDestroy();

			//destroy all scenes
			do
			{
				foreach( var scene in Component_Scene.All )
					scene.Dispose();
			} while( Component_Scene.All.Length != 0 );
			//SceneManager.Shutdown();

			//!!!!было
			//UIControlsWorld.Shutdown();

			ResourceManager.DisposeAllResources();

			SoundWorld.Shutdown();

			ScriptingCSharpEngine.Shutdown();

			//Renderer
			//if( RendererWorld.Instance != null )
			//   RendererWorld.BeginRenderFrame -= RendererWorld_BeginRenderFrame;
			RenderingSystem.Shutdown();

			ShaderCache.Shutdown();

			LanguageManager.Shutdown();

			platform.ShutdownDirectInputMouseDevice();
			InputDeviceManager.Shutdown();

			BulletPhysicsUtility.ShutdownLibrary();
			//PhysicsWorld.Shutdown();

			WindowDestroyOrDetach();

			//!!!!
			SystemSettings.ResetGamma();
			//Gamma = 1.0f;

			//!!!!!тут?
			ResourceManager.Shutdown();

			created = false;
			closing = false;

			//if( FullScreen )
			//   platform.FullscreenFadeIn( true );
		}

		//!!!!!!
		static bool IsWindowVisibleAndValidSize()
		{
			if( !platform.IsWindowVisible() )
				return false;
			if( createdInsideEngineWindow != null && platform.GetWindowState() == PlatformFunctionality.WindowState.Minimized )
				return false;

			if( createdInsideEngineWindow != null )//!!!!new
			{
				RectangleI clientRect = platform.CreatedWindow_GetClientRectangle();
				if( clientRect.Size.X < 2 || clientRect.Size.Y < 2 )
					return false;
			}

			return true;
		}

		//void RendererWorld_BeginRenderFrame()
		//{
		//   //OnRenderFrame();

		//   //render screen UI
		//   if( needRenderScreenUI )
		//   {
		//      needRenderScreenUI = false;

		//      //!!!!!везде обновляется?
		//      //if( VideoMode.X != 0 && VideoMode.Y != 0 )
		//      //   screenGuiRenderer.AspectRatio = (float)VideoMode.X / (float)VideoMode.Y;
		//      OnRenderScreenUI( screenGuiRenderer );

		//   }
		//}

		static void RenderSceneInternal()
		{
			if( DrawSplashScreen )
				return;

			if( !platform.IsWindowInitialized() )
			{
				Log.Fatal( "EngineApp.RenderScene: !platform.IsWindowInitialized()." );
				return;
			}
			if( RenderingSystem.IsDeviceLostByTestCooperativeLevel() )
				return;
			if( !IsWindowVisibleAndValidSize() )
				return;

			if( duringRenderScene )
				return;
			try
			{
				duringRenderScene = true;

				//renderPerformanceCounter.Start();
				//needRenderScreenUI = true;
				//!!!!!!
				//Log.Fatal( "good?" );
				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

				viewport.Update( true );

				//!!!!так?
				//!!!!waitForVSync
				//SharpBgfx.Bgfx.Frame();
				//viewport.Parent.SwapBuffers( true );

				//RendererWorld._RenderOneFrame();
				//needRenderScreenUI = false;
				//renderPerformanceCounter.End();

				fpsCalcFrames++;
				if( fpsCalcFrames > 10 )
				{
					uint curtime = (uint)(float)( EngineTime * 1000.0f );
					if( curtime != fpsStartTime )
						fps = (float)fpsCalcFrames * 1000.0f / ( (float)curtime - (float)fpsStartTime );
					fpsStartTime = curtime;
					fpsCalcFrames = 0;
				}


			}
			finally
			{
				duringRenderScene = false;
			}
		}

		//!!!!!переименовать в RenderScene
		//!!!!!!!!или в CreatedWindow_XXx
		//public void RenderScene_T()
		//{
		//	xx xx;//DOTIck, queued

		//	RenderSceneInternal();
		//}

		public static double FPS
		{
			get { return fps; }
		}

		//!!!!тут ли? в MainViewport?
		public static bool ShowFPS
		{
			get { return showFPS; }
			set { showFPS = value; }
		}

		public static bool ShowCursor
		{
			get { return showCursor; }
			set
			{
				if( showCursor == value )
					return;

				showCursor = value;

				//!!!!было
				//!!!!!не так скорее всего всё это
				//if( platform != null && createdInsideEngineWindow != null && createdInsideEngineWindow.Focused /*IsWindowFocused()*/ &&
				//	new Rect( 0, 0, 1, 1 ).IsContainsPoint( RendererWorld.ApplicationRenderTarget.Viewports[ 0 ].MousePosition ) )
				//{
				//	//if( platform != null && created && !closing && ApplicationWindow.Focused )// IsWindowFocused() )
				//	platform.UpdateShowSystemCursor();
				//}
			}
		}

		public static string SystemCursorFileName
		{
			get { return systemCursorFileName; }
			set
			{
				systemCursorFileName = value;

				//!!!!!было
				//!!!!!не так скорее всего всё это
				if( platform != null && createdInsideEngineWindow != null && createdInsideEngineWindow.Focused /*IsWindowFocused()*/ &&
					new Rectangle( 0, 0, 1, 1 ).Contains( RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ].MousePosition ) )
				{
					platform.CreatedWindow_UpdateSystemCursorFileName();
				}
			}
		}

		public static event Action AppCreateBefore;
		public static event Action AppCreateAfter;
		public static event Action AppDestroy;

		//protected virtual void OnDestroy()
		//{
		//	//!!!!!!было
		//	//if( AllowChangeVideoMode )
		//	//{
		//	//   //get videoMode from windowControl.Size for window mode
		//	//   if( !fullScreen && platform.IsWindowInitialized() )
		//	//   {
		//	//      RectI windowRect = platform.GetWindowRectangle();
		//	//      videoMode = windowRect.Size;
		//	//   }
		//	//}
		//}

		//!!!!!!как юзают?
		//!!!!может лучше последовательно?
		public delegate void TickDelegate( float delta );
		public static event TickDelegate Tick;

		static void PerformTick( float delta )
		{
			//!!!!так?
			Tick?.Invoke( delta );
		}

		//!!!!!!
		//protected abstract void MainViewport_OnUpdateCameraSettings();

		//!!!!!!
		//internal void Call_MainViewport_OnUpdateCameraSettings()
		//{
		//	//reset main render target settings
		//	MainViewport.Viewport.Camera.PolygonMode = PolygonMode.Solid;
		//	MainViewport.Viewport.Camera.LodBias = 1;
		//	MainViewport.Viewport.UpdateAspectRatio();

		//	MainViewport_OnUpdateCameraSettings();
		//}

		//!!!!!объединить с MainViewport_OnUpdateCameraSettings?
		//protected abstract void MainViewport_OnRenderUI();

		//!!!!!!
		//internal void Call_MainViewport_OnRenderUI()
		//{
		//	MainViewport_OnRenderUI();

		//	//draw FPS counter
		//	if( ShowFPS )
		//	{
		//		Viewport viewport = mainViewport.Viewport;
		//		Vec2 position = new Vec2( .005f / viewport.GuiRenderer.AspectRatio, .005f );
		//		Vec2 shadowOffset = 2.0f / viewport.DimensionsInPixels.Size.ToVec2();
		//		string str = FPS.ToString( "F2" );
		//		viewport.GuiRenderer.AddText( str, position + shadowOffset, HorizontalAlign.Left, VerticalAlign.Top, new ColorValue( 0, 0, 0, .5f ) );
		//		viewport.GuiRenderer.AddText( str, position, HorizontalAlign.Left, VerticalAlign.Top, new ColorValue( 1, 1, 1 ) );
		//	}
		//}

		//Render
		//protected virtual void OnRenderFrame() { }
		//!!!!было;//когда вызывать?
		//protected virtual void OnRenderScreenUI( GuiRenderer renderer ) { }

		//!!!!!везде вызывается?
		public static void DoTick()
		{
			if( !created || closing )
				return;

			//update keyboard, mouse and input devices
			{
				if( CreatedInsideEngineWindow != null )
					platform.CreatedWindow_UpdateInputDevices();
				if( InputDeviceManager.Instance != null )
					InputDeviceManager.Instance.UpdateDeviceState();
			}

			//!!!где еще?
			_UpdateEngineTime();

			//!!!!!тут?
			//!!!!когда еще вызывать?
			EngineThreading.ExecuteQueuedActionsFromMainThread();

			//!!!!тут?
			//!!!!!где-то еще?
			VirtualFileWatcher.ProcessEvents();

			//update sound world
			//soundPerformanceCounter.Start();
			//!!!!
			SoundWorld._Update();// EngineTime );
								 //soundPerformanceCounter.End();

			Log.FlushCachedLog();

			double time = EngineTime;
			double delta = time - lastEngineTimeToCalculateFPS;
			if( delta > 1.0f )
			{
				if( !enginePaused )
				{
					_EnginePause_UpdateState( true, false );
					_EnginePause_UpdateState( false, false );
				}
				delta = 0;
				//!!!!new
				lastEngineTimeToCalculateFPS = time;
			}
			if( delta != 0 )
			{
				lastEngineTimeToCalculateFPS = time;
				if( !DrawSplashScreen )
					PerformTick( (float)delta );
			}

			//!!!!!
			EngineThreading.ExecuteQueuedActionsFromMainThread();

			if( needReadLicenseCertificate )
			{
				needReadLicenseCertificate = false;
				//!!!!
				//ReadLicenseCertificate();
			}
		}

		static bool WindowCreateOrAttach()
		{
			//lastFullScreenWindowSize = platform.GetScreenSize();

			////change video mode
			//if( FullScreenEnabled && !InitializationParameters.MultiMonitorMode.Value )
			//{
			//   if( videoMode == Vec2I.Zero )
			//      videoMode = platform.GetScreenSize();

			//   if( DisplaySettings.ChangeVideoMode( FullScreenSize ) )
			//      lastFullScreenWindowSize = videoMode;
			//   else
			//      videoMode = platform.GetScreenSize();
			//}

			//if( initialWindowPosition.X == -1 || initialWindowPosition.Y == -1 )
			//   initialWindowPosition = ( platform.GetScreenSize() - videoMode ) / 2;

			//if( InitializationParameters.MultiMonitorMode.Value )
			//{
			//   RectI totalBounds = RectI.Cleared;
			//   foreach( DisplayInfo display in AllDisplays )
			//      totalBounds.Add( display.Bounds );

			//   initialWindowPosition = totalBounds.LeftTop;
			//   videoMode = totalBounds.Size;
			//}

			if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero )
			{
				//create window by the engine
				//!!!!может выше создавать, чтобы раньше можно было параметры менять?
				createdInsideEngineWindow = new CreatedInsideEngineWindowClass();
				applicationWindowHandle = platform.CreatedWindow_CreateWindow();
			}
			else
			{
				//use already created
				applicationWindowHandle = InitSettings.UseApplicationWindowHandle;
			}

			if( !platform.IsWindowInitialized() )
				Log.Fatal( "EngineApp: WindowCreateOrAttach: !platform.IsWindowInitialized()." );

			//set icon
			if( createdInsideEngineWindow != null && createdInsideEngineWindow.Icon != null )
				createdInsideEngineWindow.UpdateIcon();

			return true;
		}

		static void WindowDestroyOrDetach()
		{
			if( platform.IsWindowInitialized() )
			{
				if( createdInsideEngineWindow != null )
					platform.CreatedWindow_DestroyWindow();

				applicationWindowHandle = IntPtr.Zero;

				if( fullscreenEnabled && !InitSettings.MultiMonitorMode.Value )
					SystemSettings.RestoreVideoMode();
			}

			if( createdInsideEngineWindow != null )
			{
				createdInsideEngineWindow.Dispose();
				createdInsideEngineWindow = null;
			}
		}

		public static bool FullscreenEnabled
		{
			get { return fullscreenEnabled; }
		}

		public static Vector2I FullscreenSize
		{
			get { return fullscreenSize; }
		}

		public static void SetFullscreenMode( bool enable, Vector2I screenResolution )
		{
			if( !InitSettings.AllowChangeScreenVideoMode )
				return;
			if( InitSettings.MultiMonitorMode.Value && created )
				return;

			if( createdInsideEngineWindow == null )
			{
				//!!!!!
				//Log.Fatal( "EngineApp: Fullscreen mode are not supported for window not created by the EngineApp." );
				return;
			}

			if( created && SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
			{
				Log.Warning( "Switching fullscreen/windowed mode during application work on Mac OS X is not supported." );
				return;
			}

			if( fullscreenEnabled != enable || ( fullscreenEnabled && fullscreenSize != screenResolution ) )
			{
				bool modeChanged = fullscreenEnabled != enable;

				fullscreenEnabled = enable;
				fullscreenSize = screenResolution;

				if( created && !closing )
				{
					//!!!!!так?
					//if( !fullScreen )
					//   platform.SetWindowSize( videoMode );
					//else
					mustChangeVideoMode = true;
					//!!!!было
					//videoModeWasChangedOutside = true;
				}
			}

			//!!!!!!?
			//if( fullScreen )
			//   VideoMode = LastFullScreenWindowSize;
		}

		public static SoundChannelGroup DefaultSoundChannelGroup
		{
			get { return defaultSoundChannelGroup; }
		}

		public static bool Created
		{
			get { return instance != null && created; }
		}

		public static bool Closing
		{
			get { return closing; }
		}

		//bool CreateEngineInterfaceImpl()
		//{
		//EngineComponentManager.ComponentInfo[] components = EngineComponentManager.Instance.GetComponentsByType(
		//	EngineComponentManager.ComponentTypeFlags.Classes );
		//foreach( EngineComponentManager.ComponentInfo component in components )
		//{
		//	foreach( EngineComponentManager.ComponentInfo.PathInfo path in component.GetAllEntryPointsForThisPlatform() )
		//	{
		//		string assemblyFileName = path.Path;
		//		if( !string.IsNullOrEmpty( assemblyFileName ) )
		//		{
		//			Assembly assembly;
		//			try
		//			{
		//				assembly = AssemblyUtils.LoadAssemblyByRealFileName( assemblyFileName, false );
		//			}
		//			catch( Exception e )
		//			{
		//				Log.Fatal( "EngineApp: CreateEngineInterfaceImpl: Loading assembly failed \"{0}\" ({1})", assemblyFileName, e.Message );
		//				return false;
		//			}

		//			foreach( Type type in assembly.GetTypes() )
		//			{
		//				//!!!!good?
		//				if( typeof( EngineInterface ).IsAssignableFrom( type ) && !type.IsAbstract )
		//				{
		//					ConstructorInfo constructor = type.GetConstructor( new Type[ 0 ] { } );
		//					EngineInterface engineInitialization = (EngineInterface)constructor.Invoke( null );
		//					return true;
		//				}
		//			}
		//		}
		//	}
		//}

		//Log.Fatal( "EngineApp: CreateEngineInterfaceImpl: Implementation class for EngineInterface class is not found." );
		//	return false;
		//}

		////!!!!!
		//protected virtual void OnHideAnyEditorSplashForms() { }
		//public void DoHideAnyEditorSplashForms() { OnHideAnyEditorSplashForms(); }

		static internal void PerformWindowsWndProcEvent( uint message, IntPtr wParam, IntPtr lParam, ref bool processMessageByEngine )
		{
			WindowsWndProc?.Invoke( message, wParam, lParam, ref processMessageByEngine );
		}

		static internal void MustChangeVideoMode()
		{
			mustChangeVideoMode = true;
		}

		static public /*internal */bool _IsRealShowSystemCursor()
		{
			bool show;

			if( platform.ApplicationIsActivated() )//if( platform.IsFocused() )
			{
				show = ShowCursor;

				if( createdInsideEngineWindow != null )
				{
					Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

					if( viewport.MouseRelativeMode )
						show = false;
					else
					{
						if( !new Rectangle( 0, 0, 1, 1 ).Contains( viewport.MousePosition ) )
							show = true;
					}
				}
			}
			else
				show = true;

			return show;
		}

		///////////////////////////////////////////////////////////////// General ///////////////////////////////////////////////////////////////////

		public delegate void EnginePausedChangedDelegate( bool pause );
		public static event EnginePausedChangedDelegate EnginePausedChanged;

		//protected virtual void OnEnginePausedChanged( bool pause ) { }

		public static bool EnginePaused
		{
			get { return enginePaused; }
		}

		public static bool EnginePausedFromOutsideEngineApp
		{
			get { return enginePausedFromOutsideEngineApp; }
			set
			{
				if( enginePausedFromOutsideEngineApp == value )
					return;
				enginePausedFromOutsideEngineApp = value;

				_EnginePause_UpdateState( false, true );
			}
		}

		public static bool EnginePauseWhenApplicationIsNotActive
		{
			get { return enginePauseWhenApplicationIsNotActive; }
			set
			{
				if( enginePauseWhenApplicationIsNotActive == value )
					return;
				enginePauseWhenApplicationIsNotActive = value;

				SoundWorld._SuspendWorkingWhenApplicationIsNotActive = value;
			}
		}

		//!!!!!!
		static public /*internal */void _EnginePause_UpdateState( bool tempPauseByEngine, bool updateSoundWorldAndKeysUpAll )
		{
			if( Created )
			{
				//!!!!в паузе обновлять экран будет? когда окно поверх двигать будем, что будет с обновлением?

				bool newValue = tempPauseByEngine || enginePausedFromOutsideEngineApp || platform.IsIntoMenuLoop();
				if( enginePauseWhenApplicationIsNotActive && createdInsideEngineWindow != null && !platform.CreatedWindow_IsWindowActive() )
					newValue = true;

				if( newValue != enginePaused )
				{
					enginePaused = newValue;

					//update

					{
						foreach( Viewport viewport in RenderingSystem.GetViewports() )
							viewport.ResetLastUpdateTime();
						//!!!!!!
						//foreach( Component_Map map in Component_Map.Instances )
						//	map.ResetExecutedTimeForTicks();

						//RendererWorld._ResetFrameRenderTimeAndRenderTimeStep();
					}

					//!!!!?
					_UpdateEngineTime();
					lastEngineTimeToCalculateFPS = EngineTime;

					//instance.OnEnginePausedChanged( newValue );
					EnginePausedChanged?.Invoke( newValue );

					if( updateSoundWorldAndKeysUpAll )
					{
						//!!!!было
						//SoundWorld._UpdateAfterEnginePause( EngineTime );

						if( SoundWorld.MasterChannelGroup != null )
							SoundWorld.MasterChannelGroup.Pause = newValue;

						//!!!!!!тут?
						if( createdInsideEngineWindow != null )//!!!!!!что еще проверить?
						{
							Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
							viewport.KeysAndMouseButtonUpAll();
						}
					}

					//!!!!?
					////notify components
					//foreach( var viewport in RendererWorld.viewports.ToArray() )
					//{
					//	if( viewport.AttachedScene != null && viewport.AttachedScene.EnabledInHierarchy )
					//		viewport.AttachedScene.PerformApplicationPause( newValue );
					//	if( viewport.UIContainer != null && viewport.UIContainer.EnabledInHierarchy )
					//		viewport.UIContainer.PerformApplicationPause( newValue );
					//}
				}
			}
		}

		//////////////////////////////////////////////////////// App & Window Management ////////////////////////////////////////////////////////////

		//void UpdateGamma()
		//{
		//	if( gamma != 1 )
		//	{
		//		platform.SetGamma( gamma );
		//		gammChanged = true;
		//	}
		//	else
		//	{
		//		if( gammChanged )
		//		{
		//			platform.SetGamma( 1 );
		//			gammChanged = false;
		//		}
		//	}
		//}

		//!!!!!
		//public float Gamma
		//{
		//	get { return gamma; }
		//	set
		//	{
		//		gamma = value;
		//		if( created )
		//			UpdateGamma();
		//	}
		//}

		public static void _ProcessApplicationMessageEvents()
		{
			if( createdInsideEngineWindow == null )
				Log.Fatal( "EngineApp: _ProcessApplicationMessageEvents: createdInsideEngineWindow == null." );
			platform.CreatedWindow_ProcessMessageEvents();
		}

		public static void Run()
		{
			if( !Created )
				Log.Fatal( "EngineApp: Run: Window is not created." );
			if( createdInsideEngineWindow == null )
				Log.Fatal( "EngineApp: Run: Cannot run message loop, because window is created not from EngineApp." );

			insideRunMessageLoop = true;
			platform.CreatedWindow_RunMessageLoop();
			insideRunMessageLoop = false;

			needExit = false;
		}

		//!!!!!
		static public /*internal */void _CreatedWindow_ProcessResize()
		{
			if( createdInsideEngineWindow == null )
				Log.Fatal( "EngineApp: CreatedWindow_ProcessResize: createdInsideEngineWindow == null." );

			if( !created || closing )
				return;

			if( !platform.IsWindowInitialized() )
			{
				Log.Fatal( "EngineApp.OnResize: !platform.IsWindowInitialized()." );
				return;
			}

			//было
			//if( !fullScreen )
			//{
			//   if( platform.GetWindowState() != PlatformFunctionality.WindowStates.Minimized )
			//   {
			//      RectI clientRect = platform.GetClientRectangle();
			//      videoMode = clientRect.Size;
			//   }
			//}

			if( IsWindowVisibleAndValidSize() )
			{
				//было
				//if( RendererWorld.RenderWindow.Size != videoMode || renderWindowInFullscreen != fullScreen )
				//{
				//   renderWindowInFullscreen = fullScreen;

				//!!!!так?
				var rect = platform.CreatedWindow_GetClientRectangle();
				RenderingSystem.ApplicationRenderTarget.WindowMovedOrResized( rect.Size );// FullScreenEnabled );//, videoMode );
																						  //}

				//lastWindowSize = platform.GetWindowRectangle().GetSize();

				//MainViewport_OnResize();
			}
		}

		public static void MessageLoopWaitMessage()
		{
			platform.MessageLoopWaitMessage();
		}

		/////////////////////////////////////////////////////////////// Main Viewport /////////////////////////////////////////////////////////////////

		//public MainViewportInterface MainViewport
		//{
		//	get { return mainViewport; }
		//}

		//!!!!!
		//!!!!можно null? или всегда не null, но и менять можно
		//!!!!name? SetMainViewport?
		//public void MainViewport_Change( RenderTarget target, MainViewportInterface mainViewportImplementation )
		//{
		//	//!!!!!можно менять потом? если нет, то ошибку тут
		//	//!!!!!что для этого обновить?

		//	mainViewport = mainViewportImplementation;
		//	mainViewport.renderTarget = target;
		//	mainViewport.viewport = mainViewport.renderTarget.Viewports[ 0 ];

		//	//!!!!что тут?
		//}

		//MainViewport events
		//!!!!!
		//protected virtual void MainViewport_OnResize() { }

		/////////////////////////////////////////////////////////////// Time Management ///////////////////////////////////////////////////////////////

		public static double EngineTimeScale
		{
			get { return engineTimeScale; }
			set
			{
				lock( timeLocker )
				{
					if( engineTimeScale == value )
						return;

					double systemTime = platform.GetSystemTime();
					double newAddResultTimeValue = addToResultTime + ( systemTime - startTime ) * engineTimeScale;

					startTime = systemTime;
					addToResultTime = newAddResultTimeValue;

					engineTimeScale = value;
				}
			}
		}

		public /*internal */static void _UpdateEngineTime( double? setManualValueAndDisableAutoUpdate = null )
		{
			lock( timeLocker )
			{
				if( setManualValueAndDisableAutoUpdate != null )
				{
					engineTime = setManualValueAndDisableAutoUpdate.Value;
					engineTimeManualValueAndDisableAutoUpdate = true;
				}
				else
				{
					engineTime = addToResultTime + ( platform.GetSystemTime() - startTime ) * engineTimeScale;
					engineTimeManualValueAndDisableAutoUpdate = false;
				}
			}
		}

		/// <summary>
		/// Gets the current time in the engine.
		/// </summary>
		public static double EngineTime
		{
			get
			{
				lock( timeLocker )
				{
					return engineTime;
				}
			}
		}

		public static double GetSystemTime()
		{
			lock( timeLocker )
			{
				return platform.GetSystemTime();
			}
		}

		///////////////////////////////////////////////////////////////// Get Info ////////////////////////////////////////////////////////////////////

		//!!!SystemSettings
		public static void GetSystemLanguage( out string languageName, out string languageEnglishName )
		{
			platform.GetSystemLanguage( out languageName, out languageEnglishName );
		}

		public static string[] GetNativeModuleNames()
		{
			return platform.GetNativeModuleNames();
		}

		/////////////////////////////////////////////////////////////////// Config ////////////////////////////////////////////////////////////////////

		////!!!!
		//public static EngineConfig Config
		//{
		//	get { return config; }
		//}

		//!!!!
		public static bool NeedSaveConfig
		{
			get { return needSaveConfig; }
			set { needSaveConfig = value; }
		}

		////!!!!
		//protected internal virtual void OnRegisterConfigParameter( EngineConfig.Parameter parameter ) { }

		public delegate void RegisterConfigParameterDelegate( EngineConfig.Parameter parameter );
		public static event RegisterConfigParameterDelegate RegisterConfigParameter;

		public static void PerformRegisterConfigParameter( EngineConfig.Parameter parameter )
		{
			RegisterConfigParameter?.Invoke( parameter );
		}

		/////////////////////////////////////////////////////////////////// Other /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The ability to set the limit for maximal framerate.
		/// </summary>
		public static double MaxFPS
		{
			get { return maxFPS; }
			set { maxFPS = value; }
		}

		public static IntPtr _CallSpecialPlatformSpecificMethod( string message, IntPtr param )
		{
			return platform.CallSpecialPlatformSpecificMethod( message, param );
		}

		//public UserCustomMethodResult CallUserCustomMethod( string methodName, out object returnValue, params object[] arguments )
		//{
		//   return platform.CallUserCustomMethod( methodName, out returnValue, arguments );
		//}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!!может для удобства таки добавить. но потом

		//!!!!!
		//public bool IsKeyPressed( EKeys key )
		//{
		//	return MainViewport.IsKeyPressed( key );
		//}

		//!!!!!
		//public static bool IsKeyLocked( EKeys key )
		//{
		//	if( key != EKeys.Insert && key != EKeys.NumLock && key != EKeys.Capital && key != EKeys.Scroll )
		//		Log.Fatal( "EngineApp: IsKeyLocked: Invalid key value. Next keys can be checked by this method: EKeys.Insert, EKeys.NumLock, EKeys.Capital, EKeys.Scroll." );
		//	return instance.platform.IsKeyLocked( key );
		//}

		//!!!!!
		//public bool IsMouseButtonPressed( EMouseButtons button )
		//{
		//	return MainViewport.IsMouseButtonPressed( button );
		//}

		//!!!!!
		//public bool IsJoystickButtonPressed( JoystickButtons button )
		//{
		//	return MainViewport.IsJoystickButtonPressed( button );
		//}

		//!!!!!!
		internal static bool ChangeVideoMode( Vector2I mode )
		{
			PlatformFunctionality platform = PlatformFunctionality.Get();

			if( !platform.ChangeVideoMode( mode ) )
			{
				string text = string.Format( "Cannot change screen resolution to \"{0}x{1}\".", mode.X, mode.Y );
				if( !SystemSettings.VideoModeExists( mode ) )
					text += " This resolution is not supported by the system.";
				Log.Warning( text );
				return false;
			}

			videoModeChanged = true;
			return true;
		}

		//!!!!!!
		internal static void RestoreVideoMode()
		{
			if( videoModeChanged )
			{
				PlatformFunctionality platform = PlatformFunctionality.Get();
				platform.RestoreVideoMode();
				videoModeChanged = false;
			}
		}

		//!!!!а на какое changed?
		//!!!!!!в SystemSettings?
		public static bool VideoModeChanged
		{
			get { return videoModeChanged; }
		}


		public static CreatedInsideEngineWindowClass CreatedInsideEngineWindow
		{
			get { return createdInsideEngineWindow; }
		}

		public /*internal */static int GetEKeysMaxIndex()
		{
			int maxIndex = 0;
			foreach( EKeys eKey in Enum.GetValues( typeof( EKeys ) ) )
			{
				int index = (int)eKey;
				if( index > maxIndex )
					maxIndex = index;
			}
			return maxIndex;
		}

		public static IntPtr ApplicationWindowHandle
		{
			get { return applicationWindowHandle; }
		}

		//public double LastTickTime
		//{
		//	get { return lastTickTime; }
		//}

		//!!!!!тут ли. может в SystemSettings
		public static Vector2I GetScreenSize()
		{
			return platform.GetScreenSize();
		}

		//!!!!Debug
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		static extern short GetKeyState( int keyCode );
		[Browsable( false )]
		public static bool _DebugCapsLock
		{
			get
			{
				try
				{
					return ( ( (ushort)GetKeyState( 0x14 ) ) & 0xffff ) != 0;
				}
				catch { return false; }
			}
		}

		public static string License
		{
			get { return license; }
		}

		//public static bool IsProPlan
		//{
		//	get { return License == "Pro"; }
		//}

		[Browsable( false )]
		internal static bool DrawSplashScreen
		{
			get
			{
				//if( EngineTime != 0 )
				//{

				if( splashScreenStartTime == 0 )
					splashScreenStartTime = EngineTime;

				double totalTime = ProjectSettings.Get.EngineSplashScreenTime.Value;
				//double totalTime = IsProPlan ? ProjectSettings.Get.EngineSplashScreenTime.Value : ProjectSettings.Get.EngineSplashScreenTimeReadOnly;
				return EngineTime - splashScreenStartTime < totalTime;

				//}
				//else
				//	return true;

				//if( !IsProPlan )
				//{
				//	if( EngineTime != 0 )
				//	{
				//		if( splashScreenStartTime == 0 )
				//			splashScreenStartTime = EngineTime;
				//		return EngineTime - splashScreenStartTime < splashScreenTotalTime;
				//	}
				//	else
				//		return true;
				//}
				//else
				//{
				//	if( !ProjectSettings.Get.CustomizeSplashScreen.Value )
				//		return true;
				//	return false;
				//}
			}
		}

		//internal static void ReadLicenseCertificate()
		//{
		//	license = "Personal";

		//	try
		//	{
		//		var fileName = Path.Combine( VirtualFileSystem.Directories.Project, "License.cert" );
		//		if( File.Exists( fileName ) )
		//		{
		//			if( LoginUtility.ReadLicenseCertificate( fileName, out var error, out var email, out var engineVersion, out var license2, out var machineId, out var expirationDate ) )
		//			{
		//				if( !string.IsNullOrEmpty( machineId ) )
		//				{
		//					var machineId2 = LoginUtility.GetMachineId();//GetMacAddress();
		//					if( !string.IsNullOrEmpty( machineId2 ) && machineId != machineId2 )
		//					{
		//						Log.Info( "License certificate 'License.cert': Invalid machine identifier." );
		//						goto skip;
		//					}
		//				}

		//				if( DateTime.Compare( DateTime.UtcNow, expirationDate ) > 0 )
		//				{
		//					Log.Info( "License certificate 'License.cert': Date exprired." );
		//					goto skip;
		//				}

		//				if( !string.IsNullOrEmpty( engineVersion ) && engineVersion != EngineInfo.Version )
		//				{
		//					Log.Info( "License certificate 'License.cert': Different engine version." );
		//					goto skip;
		//				}

		//				license = license2;

		//				skip:;
		//			}
		//			else
		//				Log.Warning( "Reading license certificate 'License.cert' failed. " + error );
		//		}
		//	}
		//	catch { }
		//}

		public static Assembly ProjectAssembly
		{
			get { return projectAssembly; }
		}
	}
}
