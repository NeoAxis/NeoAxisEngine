// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Linq;
using NeoAxis.Editor;
using Internal;
using Internal.SharpBgfx;
using System.Threading;

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
		static bool isSimulation;
		static bool isEditor;
		static internal PlatformFunctionality platform;

		//application window
		static IntPtr applicationWindowHandle;
		static CreatedInsideEngineWindowClass createdInsideEngineWindow;

		//!!!!
		static volatile bool created;
		static volatile bool closing;
		static volatile bool needExit;
		static volatile bool afterFatalOperations;

		//config
		static volatile bool needSaveConfig;

		//windowed mode
		static WindowedModeEnum windowedMode = WindowedModeEnum.Fullscreen; //static bool fullscreenEnabled;
		static Vector2I windowedModeSize;//when fullscreen mode is disabled this field is used to remember last fullscreen size.
		static bool mustChangeWindowedModeOrVideoMode;

		//cursor
		//это скорее App & Window Management
		static bool showCursor = true;
		static string customCursorFileName = "";
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
		//static volatile bool showFPS;
		static double lastEngineTimeToCalculateFPS;

		//time management
		static double startTime;
		static double addToResultTime;
		static double engineTimeScale = 1;
		static double engineTime;
		//static bool engineTimeManualValueAndDisableAutoUpdate;
		//static object timeLocker = new object();

		//auto unload textures
		static double lastEngineTimeToAutoUnloadGpuResources;

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

		//static string license = "Free";
		//static internal volatile bool needReadLicenseCertificate;

		static Assembly projectAssembly;

		//render video to file
		static RenderVideoToFileData renderVideoToFileData;

		static bool insideDoTick;

		//PerformanceCounter.TimeCounter renderPerformanceCounter = new PerformanceCounter.TimeCounter( "Render", false, new ColorValue( 0, 0, 1 ), 0 );
		//PerformanceCounter.TimeCounter soundPerformanceCounter = new PerformanceCounter.TimeCounter( "Sound", false, new ColorValue( 1, 1, 1 ), 1 );

		///////////////////////////////////////////

		/// <summary>
		/// Represents engine's initialization settings.
		/// </summary>
		public static class InitSettings
		{
			static string configVirtualFileName;

			static bool useDirectInputForMouseRelativeMode = true;
			static bool allowJoysticksAndSpecialInputDevices = true;
			static bool allowChangeScreenVideoMode;
			static bool? multiMonitorMode;

			static string language = "";
			static bool? localizeEngine;
			static bool? localizeToolset;

			static IntPtr useApplicationWindowHandle;
			static WindowedModeEnum? createWindowedMode; //static bool? createWindowFullscreen = null;
			static WindowStateEnum? createWindowState;
			static Vector2I? createWindowPosition;
			static Vector2I? createWindowSize;
			//static bool createWindowFullscreenAllowChangeDisplayFrequency = true;

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

			public static WindowedModeEnum? CreateWindowedMode
			{
				get { return createWindowedMode; }
				set
				{
					if( Created )
						Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
					createWindowedMode = value;
				}
			}

			//public static bool? CreateWindowFullscreen
			//{
			//	get { return createWindowFullscreen; }
			//	set
			//	{
			//		if( Created )
			//			Log.Fatal( "EngineApp: InitializationParameters: Can't change initialization parameters after creation." );
			//		createWindowFullscreen = value;
			//	}
			//}

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

			public static RendererBackend RendererBackend = RendererBackend.Default;
			public static bool RendererReportDebugToLog;
			public static bool SimulationVSync = true;//for garbage collector is better to enable vsync
			public static bool SimulationTripleBuffering;
			public static bool UseShaderCache = true;
			//public static bool AnisotropicFiltering = true;

			public static string SoundSystem = "";
			public static int SoundMaxReal2DChannels = 32;
			public static int SoundMaxReal3DChannels = 50;

			public static bool ScriptingCompileProjectSolutionAtStartup = true;

			public static double AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInEditor = 300;
			public static double AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInSimulation = 300;

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
			string title = "NeoAxis Player";
			object/*Icon*/ icon;
			//object/*Icon*/ smallIcon;//!!!!!так?
			string iconFilePath;

			/////////////////////

			internal void Dispose()
			{
				//if( smallIcon != null )
				//{
				//	smallIcon.Dispose();
				//	smallIcon = null;
				//}
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
					if( WindowedMode != WindowedModeEnum.Windowed )
					{
						Log.Warning( "EngineApp: ApplicationWindow: set Rectangle: Can't change in fullscreen or borderless mode." );
						return;
					}

					platform.CreatedWindow_SetWindowRectangle( value );
				}
			}

			public object/*Icon*/ Icon
			{
				get { return icon; }
				set
				{
					if( icon == value )
						return;
					icon = value;

					if( Created && !Closing )
						UpdateIcon();
				}
			}

			public string IconFilePath
			{
				get { return iconFilePath; }
				set
				{
					if( iconFilePath == value )
						return;
					iconFilePath = value;

					if( Created && !Closing )
						UpdateIcon();
				}
			}

			//public object SmallIcon
			//{
			//	get { return smallIcon; }
			//}

			//public void SetIcon( object icon, object smallIcon )
			//{
			//	if( this.icon == icon && this.smallIcon == smallIcon )
			//		return;
			//	this.icon = icon;
			//	this.smallIcon = smallIcon;

			//	if( Created && !Closing )
			//		UpdateIcon();
			//}

			public string Title
			{
				get { return title; }
				set
				{
					title = value;
					platform.CreatedWindow_UpdateWindowTitle( title );
				}
			}

			internal void UpdateIcon()
			{
				platform.CreatedWindow_UpdateWindowIcon( /*smallIcon, */icon, iconFilePath );

				//				IDisposable/*Icon*/ oldSmallIcon = smallIcon;

				//				if( icon != null )
				//				{
				//					try
				//					{
				//#if !ANDROID && !IOS && !WEB && !UWP
				//						Vector2I smallIconSize = platform.GetSmallIconSize();
				//						if( smallIconSize != Vector2I.Zero )
				//						{
				//							smallIcon = new Icon( icon, new Size( smallIconSize.X, smallIconSize.Y ) );
				//						}
				//#endif
				//					}
				//					catch { }
				//				}
				//				else
				//					smallIcon = null;

				//				platform.CreatedWindow_UpdateWindowIcon( smallIcon, icon );

				//				if( oldSmallIcon != null )
				//				{
				//					oldSmallIcon.Dispose();
				//					oldSmallIcon = null;
				//				}
			}

			public void ProcessMouseMoveEvent()
			{
				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

				//!!!!must be IsFocused?
				if( viewport.MouseRelativeMode && platform.IsFocused() && !InitSettings.UseDirectInputForMouseRelativeMode )
				{
					//!!!!what about mac

					platform.CreatedWindow_UpdateMouseRelativeMove( out var delta );
					viewport.PerformMouseMove( delta );
				}

				if( !viewport.MouseRelativeMode )
				{
					var mouse = platform.CreatedWindow_GetMousePosition();
					viewport.PerformMouseMove( mouse );

					lastMousePositionForCursorUpdate = mouse;
				}
			}

			public bool Visible
			{
				get { return platform.IsWindowVisible(); }
				set { platform.SetWindowVisible( value ); }
			}

			public bool HideOnClose { get; set; }
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

		///////////////////////////////////////////

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
				applicationType = ApplicationTypeEnum.Unknown;
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
				isSimulation = applicationType == ApplicationTypeEnum.Simulation;
				isEditor = applicationType == ApplicationTypeEnum.Editor;
			}
		}

		public static bool IsSimulation
		{
			get { return isSimulation; }
		}

		public static bool IsEditor
		{
			get { return isEditor; }
		}

		bool InitInternal( IntPtr mainModuleData )
		{
			platform = PlatformFunctionality.Instance;

			//Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );

#if !DEPLOY
			if( ApplicationType == ApplicationTypeEnum.Editor )
				EditorAssembly.Init();
#endif

			platform.Init( mainModuleData );

			//logs
			{
				Log.InvisibleInfo( "Powered by NeoAxis" );

				string operationSystemDisplayName = "";
				{
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
						operationSystemDisplayName = "Microsoft Windows";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
						operationSystemDisplayName = "Apple macOS";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
						operationSystemDisplayName = "Linux";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						operationSystemDisplayName = "Google Android";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
						operationSystemDisplayName = "Apple iOS";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
						operationSystemDisplayName = "Web";
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
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
				//Log.InvisibleInfo( "Engine mode: " + EngineInfo.EngineMode.ToString() );
				//Log.InvisibleInfo( "AppContainer: " + SystemSettings.AppContainer.ToString() );
			}

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

			//enable support field and properties serialization for GameEngineApp class.
			EngineConfig.RegisterClassParameters( GetType() );// typeof( SimulationApp ) );

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
			}

			//native memory manager detect leaks
			NativeUtility.LogLeaks();
		}

		void InitializationParameters_InitFromEngineConfig()
		{
			var configPath = VirtualPathUtility.GetRealPathByVirtual( "user:Configs/Engine.config" );
			if( !File.Exists( configPath ) )
				return; // use default values.

			string error;
			TextBlock engineConfigBlock = TextBlockUtility.LoadFromRealFile( configPath, out error );
			if( engineConfigBlock != null )
			{
				////Renderer
				//TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
				//if( rendererBlock != null )
				//{
				//	//if( string.IsNullOrEmpty( InitSettings.RenderingSystemComponent ) )
				//	//	InitSettings.RenderingSystemComponent = rendererBlock.GetAttribute( "implementationComponent" );

				//	//if( string.IsNullOrEmpty( InitSettings.RenderingDeviceName ) )
				//	//{
				//	//	if( rendererBlock.AttributeExists( "renderingDeviceName" ) )
				//	//		InitSettings.RenderingDeviceName = rendererBlock.GetAttribute( "renderingDeviceName" );
				//	//	if( rendererBlock.AttributeExists( "renderingDeviceIndex" ) )
				//	//		InitSettings.RenderingDeviceIndex = int.Parse( rendererBlock.GetAttribute( "renderingDeviceIndex" ) );
				//	//}

				//	//if( rendererBlock.IsAttributeExist( "fullSceneAntialiasing" ) )
				//	//{
				//	//   RendererWorld.InitializationOptions.FullSceneAntialiasing = rendererBlock.GetAttribute( "fullSceneAntialiasing" );
				//	//}

				//	//if( rendererBlock.AttributeExists( "filtering" ) )
				//	//{
				//	//	try
				//	//	{
				//	//		InitSettings.RenderingFilteringMode = (RendererWorld.FilteringMode)
				//	//			Enum.Parse( typeof( RendererWorld.FilteringMode ), rendererBlock.GetAttribute( "filtering" ) );
				//	//	}
				//	//	catch { }
				//	//}

				//	//if( rendererBlock.AttributeExists( "verticalSync" ) )
				//	//	InitSettings.RenderingVerticalSync = bool.Parse( rendererBlock.GetAttribute( "verticalSync" ) );

				//	//!!!!!!это выставлять из SimulationApp?
				//	//!!!!!!!!!!там и хранить в одном месте хранить размер экрана для симуляции?

				//	//if( InitializationParameters.AllowChangeScreenVideoMode )
				//	//{
				//	//   if( rendererBlock.IsAttributeExist( "fullScreen" ) )
				//	//      FullScreen = bool.Parse( rendererBlock.GetAttribute( "fullScreen" ) );
				//	//   if( InitializationParameters.MultiMonitorMode == null )
				//	//   {
				//	//      if( rendererBlock.IsAttributeExist( "multiMonitorMode" ) )
				//	//         InitializationParameters.MultiMonitorMode = bool.Parse( rendererBlock.GetAttribute( "multiMonitorMode" ) );
				//	//   }
				//	//   if( rendererBlock.IsAttributeExist( "videoMode" ) )
				//	//   {
				//	//      try
				//	//      {
				//	//         VideoMode = Vec2I.Parse( rendererBlock.GetAttribute( "videoMode" ) );
				//	//      }
				//	//      catch { }
				//	//   }
				//	//}
				//}

				////SoundSystem
				//TextBlock soundSystemBlock = engineConfigBlock.FindChild( "SoundSystem" );
				//if( soundSystemBlock != null )
				//{
				//	//if( string.IsNullOrEmpty( InitSettings.SoundSystemComponent ) )
				//	//	InitSettings.SoundSystemComponent = soundSystemBlock.GetAttribute( "implementationComponent" );
				//}

				//localization
				var localizationBlock = engineConfigBlock.FindChild( "Localization" );
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

				////physics
				//var physicsSystemBlock = engineConfigBlock.FindChild( "PhysicsSystem" );
				//if( physicsSystemBlock != null )
				//{
				//	//!!!!было
				//	//if( string.IsNullOrEmpty( InitSettings.PhysicsSystemComponent ) )
				//	//	InitSettings.PhysicsSystemComponent = physicsSystemBlock.GetAttribute( "implementationComponent" );
				//}
			}
		}

		static void InitializationParameters_PostFix()
		{
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

			//init parameters
			{
				if( InitSettings.MultiMonitorMode == null )
					InitSettings.MultiMonitorMode = false;
				if( InitSettings.LocalizeEngine == null )
					InitSettings.LocalizeEngine = true;
				if( InitSettings.LocalizeToolset == null )
					InitSettings.LocalizeToolset = true;

				if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero )
				{
					//if( InitSettings.CreateWindowFullscreen == null )
					{
						//windowedMode
						if( SystemSettings.CommandLineParameters.TryGetValue( "-windowedMode", out var windowedModeString ) )
						{
							if( Enum.TryParse<WindowedModeEnum>( windowedModeString, true, out var windowedMode ) )
								InitSettings.CreateWindowedMode = windowedMode;
						}

						if( InitSettings.CreateWindowedMode == null )
						{
							if( ApplicationType == ApplicationTypeEnum.Simulation )
								InitSettings.CreateWindowedMode = WindowedModeEnum.Fullscreen;
							else
								InitSettings.CreateWindowedMode = WindowedModeEnum.Windowed;
						}

						////fullscreen
						//if( SystemSettings.CommandLineParameters.TryGetValue( "-fullscreen", out var fullscreenStr ) )
						//{
						//	try
						//	{
						//		InitSettings.CreateWindowFullscreen = (bool)SimpleTypes.ParseValue( typeof( bool ), fullscreenStr );
						//	}
						//	catch { }
						//}

						////windowed
						//if( SystemSettings.CommandLineParameters.TryGetValue( "-windowed", out var windowedStr ) )
						//{
						//	try
						//	{
						//		InitSettings.CreateWindowFullscreen = !(bool)SimpleTypes.ParseValue( typeof( bool ), windowedStr );
						//	}
						//	catch { }
						//}

						//if( InitSettings.CreateWindowFullscreen == null )
						//{
						//	if( ApplicationType == ApplicationTypeEnum.Simulation )
						//		InitSettings.CreateWindowFullscreen = true;
						//	else
						//		InitSettings.CreateWindowFullscreen = false;
						//}
					}

					//if( InitSettings.CreateWindowState == null )
					{
						//windowState
						if( SystemSettings.CommandLineParameters.TryGetValue( "-windowState", out var str ) )
						{
							try
							{
								if( Enum.TryParse( typeof( WindowStateEnum ), str, true, out var windowState ) )
									InitSettings.CreateWindowState = (WindowStateEnum)windowState;
							}
							catch { }
						}
					}

					//if( InitSettings.CreateWindowPosition == null )
					{
						//windowPosition
						if( SystemSettings.CommandLineParameters.TryGetValue( "-windowPosition", out var str ) )
						{
							try
							{
								InitSettings.CreateWindowPosition = (Vector2I)SimpleTypes.ParseValue( typeof( Vector2I ), str );
								if( InitSettings.CreateWindowState == null )
									InitSettings.CreateWindowState = WindowStateEnum.Normal;
							}
							catch { }
						}
					}

					//if( InitSettings.CreateWindowSize == null )
					{
						//windowSize
						if( SystemSettings.CommandLineParameters.TryGetValue( "-windowSize", out var str ) )
						{
							try
							{
								InitSettings.CreateWindowSize = (Vector2I)SimpleTypes.ParseValue( typeof( Vector2I ), str );
								if( InitSettings.CreateWindowState == null )
									InitSettings.CreateWindowState = WindowStateEnum.Normal;
							}
							catch { }
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
							InitSettings.CreateWindowPosition = ( platform.GetScreenSize() - InitSettings.CreateWindowSize.Value ) / 2;
					}

					if( InitSettings.CreateWindowedMode != null && InitSettings.CreateWindowedMode.Value != WindowedModeEnum.Windowed )
						InitSettings.CreateWindowPosition = new Vector2I( 0, 0 );
					//if( InitSettings.CreateWindowFullscreen.Value )
					//	InitSettings.CreateWindowPosition = new Vector2I( 0, 0 );

					//rendererBackend
					{
						if( SystemSettings.CommandLineParameters.TryGetValue( "-rendererBackend", out var str ) )
						{
							try
							{
								InitSettings.RendererBackend = (RendererBackend)Enum.Parse( typeof( RendererBackend ), str );
							}
							catch { }
						}
					}

					//soundSystem
					{
						if( SystemSettings.CommandLineParameters.TryGetValue( "-soundSystem", out var str ) )
							InitSettings.SoundSystem = str;
					}
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
				CompileAndLoadProjectAssembly();

				AppCreateBefore?.Invoke();

				//instance.OnBeginAppCreation();
				needSaveConfig = true;

				InitializationParameters_PostFix();

				//change video mode
				if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero && InitSettings.CreateWindowedMode != null )
				{
					if( InitSettings.CreateWindowedMode.Value == WindowedModeEnum.Fullscreen )
					{
						platform.FullscreenFadeOut( false );

						if( InitSettings.AllowChangeScreenVideoMode && !InitSettings.MultiMonitorMode.Value )
						{
							if( SystemSettings.ChangeVideoMode( InitSettings.CreateWindowSize.Value ) )
							{
								windowedMode = WindowedModeEnum.Fullscreen;
								windowedModeSize = InitSettings.CreateWindowSize.Value;
							}
						}
					}
					else if( InitSettings.CreateWindowedMode.Value == WindowedModeEnum.Borderless )
					{
						windowedMode = WindowedModeEnum.Borderless;
						windowedModeSize = InitSettings.CreateWindowSize.Value;
					}
					else if( InitSettings.CreateWindowedMode.Value == WindowedModeEnum.Windowed )
					{
						windowedMode = WindowedModeEnum.Windowed;
						windowedModeSize = InitSettings.CreateWindowSize.Value;
					}


					//if( InitSettings.CreateWindowFullscreen.Value )
					//	platform.FullscreenFadeOut( false );

					//if( InitSettings.AllowChangeScreenVideoMode && InitSettings.CreateWindowFullscreen.Value && !InitSettings.MultiMonitorMode.Value )
					//{
					//	if( SystemSettings.ChangeVideoMode( InitSettings.CreateWindowSize.Value ) )
					//	{
					//		fullscreenEnabled = true;
					//		fullscreenSize = InitSettings.CreateWindowSize.Value;
					//	}
					//}
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

				////physics
				//{
				//	//!!!!temp
				//	//bool allowHardwareAcceleration = false;
				//	//PhysicsWorld.Init( allowHardwareAcceleration );

				//	Internal.BulletSharp.BulletPhysicsUtility.InitLibrary();
				//}

				//joysticks and special input devices
				if( InitSettings.AllowJoysticksAndSpecialInputDevices )
				{
					InputDeviceManager instance = null;

					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
					{
#if !ANDROID && !IOS && !WEB
						instance = new WindowsInputDeviceManager( applicationWindowHandle );
#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					{
#if !ANDROID && !IOS && !WEB && !UWP
						instance = new MacOSInputDeviceManager( applicationWindowHandle );
#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
#if UWP
						instance = new UWPInputDeviceManager( applicationWindowHandle );
#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					{
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
					{
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
					{
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
				//	case ProjectSettingsComponent.RenderingAPIEnum.DirectX11: EngineSettings.Init.RendererBackend = RendererBackend.Direct3D11; break;
				//	case ProjectSettingsComponent.RenderingAPIEnum.DirectX12: EngineSettings.Init.RendererBackend = RendererBackend.Direct3D12; break;
				//	}
				//}
				//catch { }

				//OnBeforeRendererWorldInit();

				//renderWindowInFullscreen = startedAtFullScreen;

				//Renderer init
				StartupTiming.CounterStart( "Rendering system init" );
				{
					bool startedAtFullscreen = WindowedMode == WindowedModeEnum.Fullscreen;
					if( Debugger.IsAttached && !SystemSettings.CommandLineParameters.TryGetValue( "-renderVideoToFile", out _ ) )
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

				//!!!!!where to set
				//UpdateGamma();

				if( WindowedMode == WindowedModeEnum.Fullscreen )
					platform.FullscreenFadeIn( false );

				RenderingSystem.PostInitRendererAddition();

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

				//process message loop events
				if( createdInsideEngineWindow != null )
					ProcessApplicationMessageEvents();

				created = true;

				VirtualFileSystem.RegisterAssembliesIncludingFromDefaultSettingConfig();

				if( createdInsideEngineWindow != null && createdInsideEngineWindow.Focused )
					platform.CreatedWindow_UpdateShowSystemCursor( true );

				if( createdInsideEngineWindow != null )
					CreatedWindowProcessResize();
				//DoResize();

				lastEngineTimeToCalculateFPS = EngineTime;

				AppCreateAfter?.Invoke();
			}
			finally
			{
				StartupTiming.CounterEnd( "EngineApp create" );
			}

			StartupTiming.TotalEnd();
			//write to logs
			foreach( var line in StartupTiming.GetStatisticsAsStringLines() )
				Log.InvisibleInfo( line );

			//render video to file
			RenderVideoToFileData.Init();

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

		static void CompileAndLoadProjectAssembly()// string projectName, bool rebuild = false )
		{
			var clientDll = false;
			//use Project.Client.dll on a client in network mode
			if( SystemSettings.CommandLineParameters.TryGetValue( "-client", out var projectClient ) )
				clientDll = true;

			var projectName = "Project";
			if( clientDll )
				projectName += ".Client";

#if !DEPLOY

			var server = false;
			if( SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) )
				server = true;

			//check dotnet available
			bool canCompile = true;
			{
				var folder = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "dotnet" );
				if( !Directory.Exists( folder ) )
					canCompile = false;

				var projectSln = Path.Combine( VirtualFileSystem.Directories.Project, projectName + ".sln" );
				if( !File.Exists( projectSln ) )
					canCompile = false;

				//the compilation on the client is disabled
				if( clientDll )
					canCompile = false;

				if( server )
					canCompile = false;
			}

			//compile
			if( canCompile )
			{
				CSharpProjectFileUtility.Init();
				CSharpProjectFileUtility.GetProjectFileCSFiles( true, false );
				CSharpProjectFileUtility.CheckToRemoveNotExistsFilesFromProject();

				if( InitSettings.ScriptingCompileProjectSolutionAtStartup )
					CSharpProjectFileUtility.ClearAndCompileIfRequiredAtStart( clientDll );
			}

			//load
			string fullPath = Path.Combine( VirtualFileSystem.Directories.Binaries, projectName + ".dll" );
			projectAssembly = AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true, loadWithoutLocking: true );

#else
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			//{
			//	string fullPath = Path.Combine( VirtualFileSystem.Directories.Binaries, projectName + ".dll" );
			//	projectAssembly = AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
			//}
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
					if( createdInsideEngineWindow != null && WindowedMode == WindowedModeEnum.Fullscreen )
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
					if( createdInsideEngineWindow != null && WindowedMode == WindowedModeEnum.Fullscreen )
						RestoreVideoModeAndMinimize();
				}
			}
		}

		static void Log_FatalHandler( string text, string createdLogFilePath, ref bool handled )
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				if( createdInsideEngineWindow != null && WindowedMode == WindowedModeEnum.Fullscreen )
					RestoreVideoModeAndMinimize();
			}
		}

		static void Log_AfterFatal()
		{
			if( Instance != null && !AfterFatalOperations )
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

		static void ProcessChangingWindowedModeOrVideoMode()
		{
			if( EnginePauseWhenApplicationIsNotActive )
			{
				//disable simulation
				EnginePauseUpdateState( true, true );
			}

			platform.ProcessChangingVideoMode();
		}

		static public void CreatedWindowApplicationIdle( bool doTickOnly )
		{
			if( created && !closing && !afterFatalOperations )
			{
				//change video mode
				if( mustChangeWindowedModeOrVideoMode && !doTickOnly && !InitSettings.MultiMonitorMode.Value )
				{
					mustChangeWindowedModeOrVideoMode = false;
					ProcessChangingWindowedModeOrVideoMode();
					return;
				}

				//system pause reset
				EnginePauseUpdateState( false, true );
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

		public static bool AfterFatalOperations
		{
			get { return afterFatalOperations; }
			set { afterFatalOperations = value; }
		}

		public static void Destroy()
		{
			if( instance == null )
				return;

			//if( FullScreen )
			//   platform.FullscreenFadeOut( true );

			EnginePauseUpdateState( true, true );
			closing = true;

			//if( createdInsideEngineWindow != null )
			//{
			//	Viewport viewport = RendererWorld.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			//	viewport.MouseRelativeMode = false;
			//}

			SystemSettings.ResetGamma();
			//Gamma = 1.0f;

			AppDestroy?.Invoke();
			//instance.OnDestroy();

			//destroy all scenes
			do
			{
				foreach( var scene in Scene.GetAllInstancesEnabled() )
					scene.Dispose();
			} while( Scene.GetAllInstancesEnabled().Length != 0 );
			//SceneManager.Shutdown();

			RenderVideoToFileData?.Close();
			RenderVideoToFileData = null;

			//UIWebBrowser.ShutdownCefRuntime();

			ResourceManager.DisposeAllResources();

			SoundWorld.Shutdown();

			ScriptingCSharpEngine.Shutdown();

			//Renderer
			RenderingSystem.Shutdown();

#if !NO_LITE_DB
			ShaderCache.Shutdown();
#endif

			LanguageManager.Shutdown();

			platform.ShutdownDirectInputMouseDevice();
			InputDeviceManager.Shutdown();

			PhysicsNative.JDestroy();

			WindowDestroyOrDetach();

			SystemSettings.ResetGamma();

			ResourceManager.Shutdown();

			created = false;
			closing = false;

			//if( FullScreen )
			//   platform.FullscreenFadeIn( true );
		}

		static bool IsWindowVisibleAndValidSize()
		{
			if( !platform.IsWindowVisible() )
				return false;

			if( createdInsideEngineWindow != null )
			{
				if( platform.GetWindowState() == PlatformFunctionality.WindowState.Minimized )
					return false;

				var clientRect = platform.CreatedWindow_GetClientRectangle();
				if( clientRect.Size.X < 2 || clientRect.Size.Y < 2 )
					return false;
			}

			return true;
		}

		static void RenderSceneInternal()
		{
			if( DrawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
				return;
			if( RenderingSystem.BackendNull )
				return;

			if( !platform.IsWindowInitialized() )
			{
				Log.Fatal( "EngineApp: RenderSceneInternal: !platform.IsWindowInitialized()." );
				return;
			}
			if( RenderingSystem.IsDeviceLostByTestCooperativeLevel() )
				return;
			if( !IsWindowVisibleAndValidSize() )
				return;

			//update cursor for the first time when using the created window mode
			if( platform != null && createdInsideEngineWindow != null && createdInsideEngineWindow.Focused && new Rectangle( 0, 0, 1, 1 ).Contains( RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ].MousePosition ) )
			{
				platform.CreatedWindow_UpdateSystemCursorFileName();
			}

			if( duringRenderScene )
				return;
			try
			{
				duringRenderScene = true;

				//renderPerformanceCounter.Start();

				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				viewport.Update( true );

				RenderVideoToFileData?.AddFrame();

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

		public static double FPS
		{
			get { return fps; }
		}

		//public static bool ShowFPS
		//{
		//	get { return showFPS; }
		//	set { showFPS = value; }
		//}

		public static bool ShowCursor
		{
			get { return showCursor; }
			set
			{
				if( showCursor == value )
					return;
				showCursor = value;

				//update cursor
				if( platform != null && createdInsideEngineWindow != null && createdInsideEngineWindow.Focused &&
					new Rectangle( 0, 0, 1, 1 ).Contains( RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ].MousePosition ) )
				{
					platform.CreatedWindow_UpdateSystemCursorFileName();
				}
			}
		}

		public static string CustomCursorFileName
		{
			get { return customCursorFileName; }
			set
			{
				if( customCursorFileName == value )
					return;
				customCursorFileName = value;

				//update cursor
				if( platform != null && createdInsideEngineWindow != null && createdInsideEngineWindow.Focused &&
					new Rectangle( 0, 0, 1, 1 ).Contains( RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ].MousePosition ) )
				{
					platform.CreatedWindow_UpdateSystemCursorFileName();
				}
			}
		}

		public static event Action AppCreateBefore;
		public static event Action AppCreateAfter;
		public static event Action AppDestroy;

		public delegate void TickDelegate( float delta );
		public static event TickDelegate Tick;

		static void PerformTick( float delta )
		{
			Tick?.Invoke( delta );
		}

		public static void DoTick()
		{
			if( !created || closing )
				return;
			if( insideDoTick )
				return;

			insideDoTick = true;
			try
			{
				//update keyboard, mouse and input devices
				{
					if( CreatedInsideEngineWindow != null )
						platform.CreatedWindow_UpdateInputDevices();
					if( InputDeviceManager.Instance != null )
						InputDeviceManager.Instance.UpdateDeviceState();
				}

				UpdateEngineTime();

				EngineThreading.ExecuteQueuedActionsFromMainThread();

				VirtualFileWatcher.ProcessEvents();

				//auto unload textures
				if( EngineTime - lastEngineTimeToAutoUnloadGpuResources > 1.0 )
				{
					double interval;
					if( ApplicationType == ApplicationTypeEnum.Editor )
						interval = InitSettings.AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInEditor;
					else
						interval = InitSettings.AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInSimulation;

					//interval = 100000000;
					//if( EngineApp._DebugCapsLock )
					//	interval = -1;

					GpuTexture.UnloadNotUsedForLongTime( interval );
					GpuBufferManager.DestroyNativeObjectsNotUsedForLongTime( interval );

					lastEngineTimeToAutoUnloadGpuResources = EngineTime;
				}

				Log.FlushCachedLog();

				double time = EngineTime;
				double delta = time - lastEngineTimeToCalculateFPS;
				if( delta > 1.0f && renderVideoToFileData == null )
				{
					if( !enginePaused )
					{
						EnginePauseUpdateState( true, false );
						EnginePauseUpdateState( false, false );
					}
					delta = 0;
					//!!!!new
					lastEngineTimeToCalculateFPS = time;
				}
				if( delta != 0 )
				{
					lastEngineTimeToCalculateFPS = time;
					if( DrawSplashScreen == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
						PerformTick( (float)delta );
				}

				//EngineThreading.ExecuteQueuedActionsFromMainThread();

				Flow.ExecuteGlobalSleepingFlows();

				//update sound world
				//soundPerformanceCounter.Start();
				SoundWorld.UpdateInternal();
				//soundPerformanceCounter.End();

				//if( needReadLicenseCertificate )
				//{
				//	needReadLicenseCertificate = false;
				//	//ReadLicenseCertificate();
				//}
			}
			finally
			{
				insideDoTick = false;
			}
		}

		static bool WindowCreateOrAttach()
		{
			if( InitSettings.UseApplicationWindowHandle == IntPtr.Zero )
			{
				//create window by the engine
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

				if( windowedMode == WindowedModeEnum.Fullscreen && !InitSettings.MultiMonitorMode.Value )
					SystemSettings.RestoreVideoMode();
				//if( fullscreenEnabled && !InitSettings.MultiMonitorMode.Value )
				//	SystemSettings.RestoreVideoMode();
			}

			if( createdInsideEngineWindow != null )
			{
				createdInsideEngineWindow.Dispose();
				createdInsideEngineWindow = null;
			}
		}

		public static WindowedModeEnum WindowedMode
		{
			get { return windowedMode; }
		}

		public static Vector2I WindowedModeSize
		{
			get { return windowedModeSize; }
		}

		public static void SetWindowedMode( WindowedModeEnum mode, Vector2I size )
		{
			if( !InitSettings.AllowChangeScreenVideoMode )
				return;
			if( InitSettings.MultiMonitorMode.Value && created )
				return;
			if( createdInsideEngineWindow == null )
				return;

			//if( created && SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
			//{
			//	Log.Warning( "Switching fullscreen/windowed mode during application work on macOS is not supported." );
			//	return;
			//}

			if( windowedMode != mode || windowedModeSize != size )
			{
				windowedMode = mode;
				windowedModeSize = size;

				if( created && !closing )
					mustChangeWindowedModeOrVideoMode = true;
			}
		}

		//public static void SetFullscreenMode( bool enable, Vector2I screenResolution )
		//{
		//	if( !InitSettings.AllowChangeScreenVideoMode )
		//		return;
		//	if( InitSettings.MultiMonitorMode.Value && created )
		//		return;
		//	if( createdInsideEngineWindow == null )
		//		return;

		//	if( created && SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
		//	{
		//		Log.Warning( "Switching fullscreen/windowed mode during application work on Mac OS X is not supported." );
		//		return;
		//	}

		//	if( fullscreenEnabled != enable || ( fullscreenEnabled && fullscreenSize != screenResolution ) )
		//	{
		//		bool modeChanged = fullscreenEnabled != enable;

		//		fullscreenEnabled = enable;
		//		fullscreenSize = screenResolution;

		//		if( created && !closing )
		//		{
		//			mustChangeVideoMode = true;
		//		}
		//	}
		//}

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

		static internal void PerformWindowsWndProcEvent( uint message, IntPtr wParam, IntPtr lParam, ref bool processMessageByEngine )
		{
			WindowsWndProc?.Invoke( message, wParam, lParam, ref processMessageByEngine );
		}

		static internal void MustChangeWindowedModeOrVideoMode()
		{
			mustChangeWindowedModeOrVideoMode = true;
		}

		static public bool IsRealShowSystemCursor()
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

				EnginePauseUpdateState( false, true );
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

				SoundWorld.Internal_SuspendWorkingWhenApplicationIsNotActive = value;
			}
		}

		//!!!!!!
		static public void EnginePauseUpdateState( bool tempPauseByEngine, bool updateSoundWorldAndKeysUpAll )
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
						foreach( Viewport viewport in RenderingSystem.GetAllViewports() )
							viewport.ResetLastUpdateTime();
					}

					if( RenderVideoToFileData == null )
					{
						//!!!!?
						UpdateEngineTime();
						lastEngineTimeToCalculateFPS = EngineTime;
					}

					//instance.OnEnginePausedChanged( newValue );
					EnginePausedChanged?.Invoke( newValue );

					if( updateSoundWorldAndKeysUpAll )
					{
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

		////////////////////////////////////////////////////// App & Window Management //////////////////////////////////////////////////////////

		public static void ProcessApplicationMessageEvents()
		{
			if( createdInsideEngineWindow == null )
				Log.Fatal( "EngineApp: ProcessApplicationMessageEvents: createdInsideEngineWindow == null." );
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

		public static void CreatedWindowProcessResize()
		{
			if( createdInsideEngineWindow == null )
				Log.Fatal( "EngineApp: CreatedWindowProcessResize: createdInsideEngineWindow == null." );

			if( !created || closing )
				return;

			if( !platform.IsWindowInitialized() )
			{
				Log.Fatal( "EngineApp: CreatedWindowProcessResize: !platform.IsWindowInitialized()." );
				return;
			}

			if( IsWindowVisibleAndValidSize() )
			{
				var rect = platform.CreatedWindow_GetClientRectangle();
				RenderingSystem.ApplicationRenderTarget.WindowMovedOrResized( rect.Size );
			}
		}

		public static void MessageLoopWaitMessage()
		{
			platform.MessageLoopWaitMessage();
		}

		//////////////////////////////////////////////////////////// Time Management ////////////////////////////////////////////////////////////

		public static double EngineTimeScale
		{
			get { return engineTimeScale; }
			set
			{
				//lock( timeLocker )
				//{

				if( engineTimeScale == value )
					return;

				double systemTime = GetSystemTime();
				double newAddResultTimeValue = addToResultTime + ( systemTime - startTime ) * engineTimeScale;

				startTime = systemTime;
				addToResultTime = newAddResultTimeValue;

				engineTimeScale = value;

				//}
			}
		}

		public static void UpdateEngineTime( double? setManualValueAndDisableAutoUpdate = null )
		{
			//lock( timeLocker )
			//{

			if( renderVideoToFileData != null )
			{
				Interlocked.Exchange( ref engineTime, EngineTime + 1.0 / (double)renderVideoToFileData.FramesPerSecond );
				//engineTime += 1.0 / (double)renderVideoToFileData.FramesPerSecond;
			}
			else if( setManualValueAndDisableAutoUpdate != null )
			{
				Interlocked.Exchange( ref engineTime, setManualValueAndDisableAutoUpdate.Value );
				//engineTime = setManualValueAndDisableAutoUpdate.Value;
				//engineTimeManualValueAndDisableAutoUpdate = true;
			}
			else
			{
				Interlocked.Exchange( ref engineTime, addToResultTime + ( GetSystemTime() - startTime ) * engineTimeScale );
				//engineTime = addToResultTime + ( GetSystemTime() - startTime ) * engineTimeScale;
				//engineTimeManualValueAndDisableAutoUpdate = false;
			}

			//}
		}

		/// <summary>
		/// Gets the current time in the engine. The engine time is updated once before a simulation step or before a frame update if it is an editor.
		/// </summary>
		public static double EngineTime
		{
			get
			{
				return Interlocked.CompareExchange( ref engineTime, -1.0, -1.0 );
			}
		}

		public static double GetSystemTime()
		{
			long time = Stopwatch.GetTimestamp();
			double elapsedSeconds = time * ( 1.0 / Stopwatch.Frequency );
			return elapsedSeconds;
		}

		////////////////////////////////////////////////////////////// Get Info /////////////////////////////////////////////////////////////////

		//!!!SystemSettings
		public static void GetSystemLanguage( out string languageName, out string languageEnglishName )
		{
			platform.GetSystemLanguage( out languageName, out languageEnglishName );
		}

		public static string[] GetNativeModuleNames()
		{
			return platform.GetNativeModuleNames();
		}

		//////////////////////////////////////////////////////////////// Config /////////////////////////////////////////////////////////////////

		public static bool NeedSaveConfig
		{
			get { return needSaveConfig; }
			set { needSaveConfig = value; }
		}

		public delegate void RegisterConfigParameterDelegate( EngineConfig.Parameter parameter );
		public static event RegisterConfigParameterDelegate RegisterConfigParameter;

		public static void PerformRegisterConfigParameter( EngineConfig.Parameter parameter )
		{
			RegisterConfigParameter?.Invoke( parameter );
		}

		//////////////////////////////////////////////////////////////// Other //////////////////////////////////////////////////////////////////

		/// <summary>
		/// The ability to set the limit for maximal framerate.
		/// </summary>
		public static double MaxFPS
		{
			get { return maxFPS; }
			set { maxFPS = value; }
		}

		public static IntPtr CallPlatformSpecificMethod( string message, IntPtr param )
		{
			return platform.CallPlatformSpecificMethod( message, param );
		}

		//public UserCustomMethodResult CallUserCustomMethod( string methodName, out object returnValue, params object[] arguments )
		//{
		//   return platform.CallUserCustomMethod( methodName, out returnValue, arguments );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static bool ChangeVideoMode( Vector2I mode )
		{
			if( !PlatformFunctionality.Instance.ChangeVideoMode( mode ) )
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

		internal static void RestoreVideoMode()
		{
			if( videoModeChanged )
			{
				PlatformFunctionality platform = PlatformFunctionality.Instance;
				platform.RestoreVideoMode();
				videoModeChanged = false;
			}
		}

		public static bool VideoModeChanged
		{
			get { return videoModeChanged; }
		}


		public static CreatedInsideEngineWindowClass CreatedInsideEngineWindow
		{
			get { return createdInsideEngineWindow; }
		}

		public static int GetEKeysMaxIndex()
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

		public static Vector2I GetScreenSize()
		{
			return platform.GetScreenSize();
		}

		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		static extern short GetKeyState( int keyCode );
		[Browsable( false )]
		public static bool _DebugCapsLock
		{
			get
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						return ( ( (ushort)GetKeyState( 0x14 ) ) & 0xffff ) != 0;
					}
					catch { }
				}
				return false;
			}
		}

		[Browsable( false )]
		public static ProjectSettingsPage_General.EngineSplashScreenStyleEnum DrawSplashScreen
		{
			get
			{
				var result = (ProjectSettingsPage_General.EngineSplashScreenStyleEnum)ProjectSettings.Get.General.EngineSplashScreenStyle.Value;

				//if( EngineTime != 0 )
				//{

				//if( ProjectSettings.Get.CustomizeSplashScreen )
				//	return false;

				if( splashScreenStartTime == 0 )
					splashScreenStartTime = EngineTime;

				double totalTime = 3.0;
				//double totalTime = ProjectSettings.Get.EngineSplashScreenTime.Value;

				//double totalTime = IsProPlan ? ProjectSettings.Get.EngineSplashScreenTime.Value : ProjectSettings.Get.EngineSplashScreenTimeReadOnly;
				if( EngineTime - splashScreenStartTime > totalTime )
					result = ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled;

				if( RenderingSystem.BackendNull )
					result = ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled;

				return result;

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
			set { projectAssembly = value; }
		}

		//public enum RestartEngineEventEnum
		//{
		//	Destroyed,
		//	Created,
		//}
		//public delegate void RestartEngineEventDelegate( RestartEngineEventEnum name );
		//public static event RestartEngineEventDelegate RestartEngineEvent;

		//public static void PerformRestartEngineEvent( RestartEngineEventEnum name )
		//{
		//	RestartEngineEvent?.Invoke( name );
		//}

		public static RenderVideoToFileData RenderVideoToFileData
		{
			get { return renderVideoToFileData; }
			set { renderVideoToFileData = value; }
		}

		public static Vector2I GetSystemSmallIconSize()
		{
			return platform.GetSmallIconSize();
		}
	}
}
