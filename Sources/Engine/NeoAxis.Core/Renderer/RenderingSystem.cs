// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using SharpBgfx;

namespace NeoAxis
{
	struct OgreRoot
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_New", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void* New( [MarshalAs( UnmanagedType.LPWStr )] string nativeLibrariesDirectory );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_SetGeneralSetting", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void SetGeneralSetting( void* root, [MarshalAs( UnmanagedType.LPWStr )] string key,
		  [MarshalAs( UnmanagedType.LPWStr )] string value );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_Delete", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void Delete( void* root );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_initialise", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void initialise( void*/*OgreRoot*/ root );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_createSceneManager", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void*/*MyOgreSceneManager*/ createSceneManager(
			void*/*OgreRoot*/ root, string typeName );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_destroySceneManager", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void destroySceneManager( void*/*MyOgreSceneManager*/ sceneManager );

		//[DllImport( OgreWrapper.library, EntryPoint = "OgreRoot_setRenderingDevice", CallingConvention = OgreWrapper.convention )]
		//public unsafe static extern void setRenderingDevice( void*/*OgreRoot*/  root, string renderingDeviceName, int renderingDeviceIndex );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	enum OgreLogMessageLevel
	{
		LML_TRIVIAL = 1,
		LML_NORMAL = 2,
		LML_CRITICAL = 3
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	struct MyOgreLogListener
	{
		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate void messageLoggedDelegate( [MarshalAs( UnmanagedType.LPWStr )] string message,
			OgreLogMessageLevel lml, [MarshalAs( UnmanagedType.U1 )] bool maskDebug );

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreLogListener_New", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void*/*MyOgreLogListener*/ New( messageLoggedDelegate messageLogged );

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreLogListener_Delete", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void Delete( void*/*MyOgreLogListener*/ _this );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreLogManager
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreLogManager_getDefaultLog_addListener", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void getDefaultLog_addListener( void* root, void*/*OgreLogListener*/ listener );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreLogManager_getDefaultLog_removeListener", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void getDefaultLog_removeListener( void* root, void*/*OgreLogListener*/ listener );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreResourceGroupManager
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreResourceGroupManager_initialiseAllResourceGroups", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void initialiseAllResourceGroups( void* root, out IntPtr/*string*/ error );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreResourceGroupManager_addResourceLocation", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void addResourceLocation( void* root, string name, string locType,
			[MarshalAs( UnmanagedType.U1 )] bool recursive );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Various functionality for working with the rendering system.
	/// </summary>
	public static class RenderingSystem
	{
		public static Capabilities Capabilities;


		static internal unsafe OgreRoot* realRoot;

		static bool resourcesInitialized;
		static List<string> resourceInitializationErrors = new List<string>();
		static bool rendererAdditionInitialized;

		static OgreSceneManager sceneManager;

		//!!!!неудаленное может тут накапливаться в оконныъ приложениях
		static RenderWindow applicationRenderWindow;

		static internal ESet<RenderTarget> renderTargets = new ESet<RenderTarget>();
		//static internal Dictionary<IntPtr, RenderTarget> renderTargets = new Dictionary<IntPtr, RenderTarget>();
		static internal ESet<Viewport> viewports = new ESet<Viewport>();
		//static internal Dictionary<IntPtr, Viewport> viewports = new Dictionary<IntPtr, Viewport>();
		//static internal Dictionary<IntPtr, RenderCamera> cameras = new Dictionary<IntPtr, RenderCamera>();
		static internal ESet<CanvasRendererImpl> canvasRenderers = new ESet<CanvasRendererImpl>();

		//!!!!!было
		//для каждой карты своё получается, раз там step. как для тиков будет?
		//float updateViewportTime;//frameRenderTime;
		//float updateViewportTimeStep;//frameRenderTimeStep;
		//!!!!выставлять
		//!!!!!было
		//float previousUpdateViewportTime;

		//!!!!!
		//bool enableTimeProgress = true;

		//!!!!!надо
		//internal RenderStatisticsInfo statistics = new RenderStatisticsInfo();

		static unsafe MyOgreLogListener* logListener;
		static MyOgreLogListener.messageLoggedDelegate logListener_messageLoggedDelegate;
		static bool invisibleInternalLogMessages;
		static bool enableInternalLogMessages = true;

		//internal bool firstRenderOneFrame = true;
		//bool insideBeginRenderFrameEvent;
		//bool insideRenderOneFrame;

		//internal float irradianceVolumeLightPowerSpeed;

		//List<RenderTarget> lastUpdatedRenderTargetsForRenderTimeCalculation = new List<RenderTarget>();

		//static long renderWindowNameCounter = 1;
		//static long multiRenderTargetNameCounter = 1;

		//!!!!
		//static ShaderCacheManager shaderCacheManager = new OnlyForLoadShaderCacheManager();

		//OgreRoot.profilingToolBeginOperationDelegate profilingToolBeginOperationDelegate;
		//OgreRoot.profilingToolEndOperationDelegate profilingToolEndOperationDelegate;

		//!!!!!
		static internal List<Viewport> viewportsDuringUpdate = new List<Viewport>();

		static bool disposed;

		static Metadata.TypeInfo renderingPipelineDefault;

		//occlusion query
		static int canCreateOcclusionQueriesCounter;
		class OcclusionQueryItem
		{
			public OcclusionQuery query;
			public OcclusionQueryResult callback;
			public object callbackParameter;
		}
		static List<OcclusionQueryItem> occlusionQueries = new List<OcclusionQueryItem>();

		//static CallbackHandler callbackHandler;

		///////////////////////////////////////////

		//public delegate void BeginRenderFrameDelegate();
		//public event BeginRenderFrameDelegate BeginRenderFrame;

		///////////////////////////////////////////

		//public enum FilteringMode
		//{
		//	//[DisplayName( "Recommended setting", "Renderer" )]
		//	RecommendedSetting,

		//	//[DisplayName( "Trilinear", "Renderer" )]
		//	Trilinear,

		//	//[DisplayName( "Anisotropic 2x", "Renderer" )]
		//	Anisotropic2x,

		//	//[DisplayName( "Anisotropic 4x", "Renderer" )]
		//	Anisotropic4x,

		//	//[DisplayName( "Anisotropic 8x", "Renderer" )]
		//	Anisotropic8x,

		//	//[DisplayName( "Anisotropic 16x", "Renderer" )]
		//	Anisotropic16x,
		//}

		///////////////////////////////////////////////

		class CallbackHandler : ICallbackHandler
		{
			public void CaptureFinished() { }
			public void CaptureFrame( IntPtr data, int size ) { }
			public void CaptureStarted( int width, int height, int pitch, TextureFormat format, bool flipVertical ) { }
			public int GetCachedSize( long id ) { return 0; }
			public bool GetCacheEntry( long id, IntPtr data, int size ) { return false; }
			public void ProfilerBegin( string name, int color, string filePath, int line ) { }
			public void ProfilerEnd() { }

			public unsafe void ReportDebug( string fileName, int line, string format, IntPtr args )
			{
				if( EngineSettings.Init.RendererReportDebugToLog )
				{
					sbyte* buffer = stackalloc sbyte[ 1024 ];
					NativeMethods.bgfx_vsnprintf( buffer, new IntPtr( 1024 ), format, args );
					var str = Marshal.PtrToStringAnsi( new IntPtr( buffer ) );
					Log.Info( "Renderer: Bgfx: " + str );
				}
			}

			public void ReportError( string fileName, int line, ErrorType errorType, string message )
			{
				string text = $"{fileName} ({line})  {errorType}: {message}";

				Debug.Write( text );
				if( errorType == ErrorType.DebugCheck )
				{
					if( EngineSettings.Init.RendererReportDebugToLog ) //!!!! should we use RendererReportDebugToLog ? this is important warning/error.
						Log.Warning( "Renderer: Bgfx: " + text ); //TODO: replace to Log.Warn

					// Debug.Fail terminate app in UWP
					if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
					{
						Debug.Fail( text ); // thread independed, debug only message box.
					}
				}
				else
				{
					Log.Fatal( $"Renderer: Bgfx fatal error: " + text );
					Debug.Fail( text ); // thread independed, debug only message box.
										//Environment.Exit( 1 ); ?
				}
			}

			public void SaveScreenShot( string path, int width, int height, int pitch, IntPtr data, int size, bool flipVertical ) { }
			public void SetCacheEntry( long id, IntPtr data, int size ) { }
		}

		///////////////////////////////////////////////

		//!!!!!!
		[DllImport( "kernel32.dll", EntryPoint = "SetDllDirectory", CharSet = CharSet.Unicode )]
		static extern bool SetDllDirectory( string lpPathName );

		static bool nativeDLLsPreloaded;
		internal static void NativeDLLsPreload()
		{
			if( !nativeDLLsPreloaded )
			{
				NativeLibraryManager.PreLoadLibrary( "NeoAxisCoreNative" );
				NativeLibraryManager.PreLoadLibrary( SharpBgfx.NativeMethods.DllName );
				nativeDLLsPreloaded = true;
			}
		}

		//!!!!editor
		internal static bool Init( bool startedAtFullScreen, bool multiMonitorMode, string fontManagerDefaultLanguage )//, Vec2I mainRenderTargetSize )
		{
			//if( instance != null )
			//	Log.Fatal( "RendererWorld: _Init: The instance is already initialized." );
			//instance = new RendererWorld();

			NativeDLLsPreload();

			string saveCurrentDirectory = "";
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				saveCurrentDirectory = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory( VirtualFileSystem.Directories.PlatformSpecific );
			}

			bool result = InitInternal( startedAtFullScreen, multiMonitorMode, fontManagerDefaultLanguage );//, isEditor );//, mainRenderTargetSize );

			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				Directory.SetCurrentDirectory( saveCurrentDirectory );

			if( !result )
				Shutdown();
			return result;
		}

		///// <summary>
		///// Gets an instance of the <see cref="RendererWorld"/>.
		///// </summary>
		//public static RendererWorld Instance
		//{
		//	get { return instance; }
		//}

		internal static void _PostInitRendererAddition()
		{
			rendererAdditionInitialized = true;
		}

		/// <summary>Gets the main render target.</summary>
		public static RenderWindow ApplicationRenderTarget
		{
			get { return applicationRenderWindow; }
		}

		//public void UpdateRenderTargets( IList<RenderTarget> renderTargets, bool swapBuffers )
		//{
		//	//!!!!!про первое render time

		//	//!!!!!beginNewRenderFrame

		//!!!!!!!
		//	////no render if device lost
		//	//if( RenderSystem.Instance.IsDeviceLostByTestCooperativeLevel() )
		//	//	return;
		//	////try to restore device lost
		//	//if( RenderSystem.Instance.IsDeviceLost() )
		//	//{
		//	//	if( !RenderSystem.Instance.RestoreDeviceAfterLost() )
		//	//		return;
		//	//}

		//	//if ( beginNewRenderFrame && RendererWorld.insideRenderOneFrame )
		//		  //   if( RendererWorld.insideRenderOneFrame )
		//		  //      Log.Fatal( "RendererWorld: _RenderOneFrame: Unable to render frame, because right a now new frame is already during rendering." );


		//!!!!!взять из метода нужное. потом удалить
		//void UpdateRenderTargets223( RenderTarget renderTargetForUpdate, bool beginNewRenderFrame )
		//{
		//	//!!!!когда свапать?


		//	//bool needChangeInsideRenderOneFrameFlag = insideRenderOneFrame == false;

		//	//!!!!!
		//	//bool newFrame = forceBeginNewRenderFrame;
		//	//bool newFrame = false;
		//	////calculate newFrame and update lastUpdatedRenderTargetsForRenderTimeCalculation
		//	//if( !insideBeginRenderFrameEvent && !insideRenderOneFrame )
		//	//{
		//	//   if( forceBeginNewRenderFrame )
		//	//      newFrame = true;
		//	//   if( renderTargetForUpdate != null )
		//	//   {
		//	//      if( !lastUpdatedRenderTargetsForRenderTimeCalculation.Contains( renderTargetForUpdate ) )
		//	//         lastUpdatedRenderTargetsForRenderTimeCalculation.Add( renderTargetForUpdate );
		//	//      else
		//	//         newFrame = true;
		//	//   }
		//	//   if( frameRenderTime + 1.0f < time )
		//	//      newFrame = true;
		//	//   if( newFrame )
		//	//      lastUpdatedRenderTargetsForRenderTimeCalculation.Clear();
		//	//   if( firstRenderOneFrame )
		//	//      newFrame = true;
		//	//}

		//	if( beginNewRenderFrame )
		//	{
		//		////begin new frame
		//		//insideRenderOneFrame = true;

		//		////update render time
		//		//float time = EngineApp.Instance.Time;
		//		//previousFrameRenderTime = frameRenderTime;
		//		//float oldTime = frameRenderTime;
		//		//frameRenderTime = time;
		//		//frameRenderTimeStep = frameRenderTime - oldTime;

		//		//!!!!слишком большой step урезать?

		//		//!!!!!!надо
		//		//if( !EngineApp.Instance.EnginePaused )//!!!!!!так? ведь сейчас не включается setEnableTimeProgress
		//		//{
		//		//	unsafe
		//		//	{
		//		//		OgreRoot.setEnableTimeProgress( RendererWorld.realRoot, EnableTimeProgress );
		//		//		double t = (double)frameRenderTime * 1000.0;

		//		//		//!!!!!не так?
		//		//		OgreRoot.setTimerMilliseconds( RendererWorld.realRoot, (uint)t );
		//		//	}
		//		//}

		//		//!!!!иначе
		//		////reset statistics
		//		//statistics.BeginNewFrame();

		//		//!!!!!here?
		//		//!!!!!было
		//		//if( !LongOperationCallbackManager.DuringCallingCallback )
		//		//SceneManager.Instance.UpdateConstShadowSettings();

		//		//try
		//		//{
		//		//   insideBeginRenderFrameEvent = true;

		//		//for( int n = 0; n < Map.Instances.Count; n++ )
		//		//	Map.Instances[ n ].BeforeBeginRenderFrameEvent();

		//		//if( BeginRenderFrame != null )
		//		//	BeginRenderFrame();
		//		//}
		//		//finally
		//		//{
		//		//   insideBeginRenderFrameEvent = false;
		//		//}

		//		//!!!!!так? тут?
		//		//EngineApp.Instance.Call_MainViewport_OnUpdateCameraSettings();
		//		//EngineApp.Instance.Call_MainViewport_OnRenderUI();
		//	}

		//	//update render targets
		//	//unsafe
		//	//{
		//	//IntPtr errorPointer;

		//	//if( renderTargetForUpdate != null )
		//	//{
		//	//   OgreRoot.renderOneFrame( RendererWorld.realRoot, renderTargetForUpdate.realObject, newFrame, out errorPointer );
		//	//}
		//	//else
		//	//{
		//	//   OgreRoot.renderOneFrame( RendererWorld.realRoot, null, false, out errorPointer );
		//	//}

		//	//string error = OgreNativeWrapper.GetOutString( errorPointer );
		//	//if( error != null )
		//	//{
		//	//   Log.Fatal( "RendererWorld: Render frame failed ({0}).", error );
		//	//   return;
		//	//}
		//	//}

		//	//}
		//	//finally
		//	//{
		//	//   if( needChangeInsideRenderOneFrameFlag )
		//	//      insideRenderOneFrame = false;
		//	//}

		//	////reset current pass
		//	//SceneManager.Instance.ResetCurrentRenderPass();

		//	////тут ли? обновляется там, где тик. проверить всё по тикам и Simulation pause.
		//	////update particle systems bounds
		//	//if( newFrame )
		//	//{
		//	//	if( !RenderSystem.Instance.IsDeviceLost() )
		//	//		SceneManager.Instance.UpdateParticleSystemsBounds();
		//	//}

		//	//!!!!было
		//	////recreate viewport compositors if need
		//	//if( !RenderSystem.Instance.IsDeviceLost() )
		//	//{
		//	//   foreach( RenderTarget renderTarget in renderTargets.Values )
		//	//   {
		//	//      foreach( Viewport viewport in renderTarget.viewports )
		//	//      {
		//	//         if( viewport.needRecreateCompositors )
		//	//            viewport.RecreateCompositors();
		//	//      }
		//	//   }
		//	//}

		//	//if( beginNewRenderFrame )
		//	//{
		//	//!!!!!!need? maybe someone want to collect debug geometry and gui during some time.
		//	//if( !insideBeginRenderFrameEvent && !insideRenderOneFrame )
		//	//{
		//	//   //clear not rendered debug geometries
		//	//   foreach( RenderCamera camera in cameras.Values )
		//	//   {
		//	//      xx xx;//это как с GuiRenderer?

		//	//      if( !camera.lastFrameRendered )
		//	//      {
		//	//         if( camera.debugGeometry != null )
		//	//            camera.debugGeometry.Clear();
		//	//      }

		//	//      camera.lastFrameRendered = false;
		//	//   }
		//	//}

		//	//SceneManager.Instance._SceneGraph.UpdateAfterRendering();

		//	//insideRenderOneFrame = false;
		//	//}

		//	//firstRenderOneFrame = false;
		//}

		//internal void RenderOneFrame()
		//{
		//   if( RendererWorld.insideRenderOneFrame )
		//      Log.Fatal( "RendererWorld: _RenderOneFrame: Unable to render frame, because right a now new frame is already during rendering." );

		//   UpdateRenderTargets( null, true );
		//}

		//public string _GetParticleFilePathByName( string particleName )
		//{
		//	unsafe
		//	{
		//		OgreParticleSystem* realParticleSystem = (OgreParticleSystem*)
		//			OgreParticleSystemManager.getTemplate( RendererWorld.realRoot, particleName );

		//		string ret;
		//		if( realParticleSystem != null )
		//		{
		//			ret = OgreNativeWrapper.GetOutString(
		//				OgreParticleSystem.getOrigin( realParticleSystem ) );
		//		}
		//		else
		//			ret = "";

		//		return ret;
		//	}
		//}

		///// <summary>Gets the renderer driver name.</summary>
		//public string DriverName
		//{
		//	get
		//	{
		//		int major;
		//		int minor;
		//		int patch;
		//		IntPtr versionNamePointer;
		//		OgreRoot.getOgreVersion( out major, out minor, out patch, out versionNamePointer );
		//		string versionName = OgreNativeWrapper.GetOutString( versionNamePointer );

		//		return string.Format( "OGRE {0}.{1}.{2} {3}", major, minor, patch, versionName );
		//	}
		//}

		//!!!!!надо
		///// <summary>
		///// Gets the render statistics.
		///// </summary>
		//public RenderStatisticsInfo Statistics
		//{
		//   get { return statistics; }
		//}

		//!!!!было
		//public void _ResetFrameRenderTimeAndRenderTimeStep()
		//{
		//	frameRenderTime = EngineApp.Instance.Time;
		//	frameRenderTimeStep = 0;
		//}

		/// <summary>
		/// Gets the render targets collection.
		/// </summary>
		public static RenderTarget[] GetRenderTargets()
		{
			lock( renderTargets )
			{
				return renderTargets.ToArray();
				//RenderTarget[] a = new RenderTarget[ renderTargets.Count ];
				//renderTargets.Values.CopyTo( a, 0 );
				//return a;
			}
		}

		/// <summary>
		/// Gets the viewports collection.
		/// </summary>
		public static Viewport[] GetViewports()
		{
			lock( viewports )
			{
				return viewports.ToArray();
				//Viewport[] a = new Viewport[ viewports.Count ];
				//viewports.Values.CopyTo( a, 0 );
				//return a;
			}
		}

		public static bool _InvisibleInternalLogMessages
		{
			get { return invisibleInternalLogMessages; }
			set { invisibleInternalLogMessages = value; }
		}

		public static bool _EnableInternalLogMessages
		{
			get { return enableInternalLogMessages; }
			set { enableInternalLogMessages = value; }
		}

		public delegate void _InternalLogMessageDelegate( String message );
		public static event _InternalLogMessageDelegate _InternalLogMessage;

		internal static ResetFlags GetApplicationWindowResetFlags()
		{
			ResetFlags flags = 0;
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && EngineSettings.Init.SimulationVSync )
				flags |= ResetFlags.Vsync;
			if( EngineSettings.Init.AnisotropicFiltering )
				flags |= ResetFlags.MaxAnisotropy;

			//!!!!
			//flags |= ResetFlags.SrgbBackbuffer;

			//!!!!
			//flags |= ResetFlags.MSAA4x;

			return flags;
		}

		unsafe static bool InitInternal( bool startedAtFullScreen, bool multiMonitorMode, string fontManagerDefaultLanguage )//, bool isEditor )//, Vec2I mainRenderTargetSize )
		{
			OgreNativeWrapper.CheckNativeBridge( (int)ParameterType.TextureCube );// GpuProgramParameters.GetAutoConstantTypeCount() );

			//!!!!new UWP
			var path = VirtualFileSystem.Directories.PlatformSpecific;
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				path = VirtualFileSystem.MakePathRelative( path );

			Vector2I initialWindowSize = new Vector2I( 10, 10 );
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				initialWindowSize = EngineApp.platform.CreatedWindow_GetClientRectangle().Size;

			//set platform data
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
				Bgfx.SetPlatformData( new PlatformData { Context = (IntPtr)1 } );
			else
				Bgfx.SetPlatformData( new PlatformData { WindowHandle = EngineApp.ApplicationWindowHandle } );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && EngineSettings.Init.SimulationTripleBuffering )
				Bgfx.SetTripleBuffering();

			//set OpenGL ES for Android
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
			{
				EngineSettings.Init.RendererBackend = RendererBackend.OpenGLES;
				//EngineSettings.Init.RendererBackend = RendererBackend.Noop;
			}

			//Log.InvisibleInfo( "Renderer backend: " + EngineSettings.Init.RendererBackend.ToString() );

			Bgfx.Init( new InitSettings
			{
				Backend = EngineSettings.Init.RendererBackend,
				CallbackHandler = new CallbackHandler(),

				////!!!!в релизе можно включить. в NeoAxis.DefaultSettings.config
				//Debug = true
				//!!!!
				//ResetFlags = ResetFlags.MSAA8x,
			} );

			Bgfx.Reset( initialWindowSize.X, initialWindowSize.Y, GetApplicationWindowResetFlags() );

			realRoot = (OgreRoot*)OgreRoot.New( path );

			//!!!!before
			//realRoot = (OgreRoot*)OgreRoot.New( VirtualFileSystem.Directories.PlatformSpecific );

			//profilingToolBeginOperationDelegate = profilingToolBeginOperation;
			//profilingToolEndOperationDelegate = profilingToolEndOperation;
			//OgreRoot.setCallbackDelegates( realRoot,
			//   profilingToolBeginOperationDelegate, 
			//   profilingToolEndOperationDelegate );

			//this.isEditor = isEditor;

			logListener_messageLoggedDelegate = logListener_messageLogged;
			logListener = (MyOgreLogListener*)MyOgreLogListener.New(
				logListener_messageLoggedDelegate );
			OgreLogManager.getDefaultLog_addListener( realRoot, logListener );

			MyOgreVirtualFileSystem.Init();

			//TextBlock configBlock = null;
			//if( VirtualFile.Exists( "Base/Constants/RenderingSystem.config" ) )
			//	configBlock = TextBlockUtility.LoadFromVirtualFile( "Base/Constants/RenderingSystem.config" );

			////irradianceVolumeLightPowerSpeed
			//{
			//   irradianceVolumeLightPowerSpeed = 1;

			//   if( configBlock != null )
			//   {
			//      TextBlock staticLightingBlock = configBlock.FindChild( "staticLighting" );
			//      if( staticLightingBlock != null )
			//      {
			//         if( staticLightingBlock.IsAttributeExist( "irradianceVolumeLightPowerSpeed" ) )
			//         {
			//            irradianceVolumeLightPowerSpeed = float.Parse(
			//               staticLightingBlock.GetAttribute( "irradianceVolumeLightPowerSpeed" ) );
			//         }
			//      }
			//   }
			//}

			//if( !string.IsNullOrEmpty( EngineApp.InitSettings.RenderingDeviceName ) )
			//{
			//	OgreRoot.setRenderingDevice( realRoot, EngineApp.InitSettings.RenderingDeviceName,
			//		EngineApp.InitSettings.RenderingDeviceIndex );
			//}

			////load plugins
			//{
			//	//load render system
			//	{
			//		//fix EngineSettings.Init.renderingSystemDLL
			//		{
			//			//!!!!!если пустое или неправильное
			//			//EngineSettings.Init.renderingSystemDLL = "NeoAxis.Impl.Render.OpenGL.dll";
			//		}

			//		IntPtr errorPointer;
			//		OgreRoot.loadPlugin( realRoot, EngineSettings.Init.renderingSystemDLL, out errorPointer );
			//		string error = OgreNativeWrapper.GetOutString( errorPointer );
			//		if( error != null )
			//			Log.Fatal( "RendererWorld: Loading plugin \"{0}\" failed ({1}).", EngineSettings.Init.renderingSystemDLL, error );
			//	}

			//	//load addition plugins
			//	if( configBlock != null )
			//	{
			//		TextBlock pluginsBlock = configBlock.FindChild( "plugins" );
			//		if( pluginsBlock != null )
			//		{
			//			foreach( TextBlock pluginBlock in pluginsBlock.Children )
			//			{
			//				string pluginName = pluginBlock.GetAttribute( "name" );

			//				IntPtr errorPointer;
			//				OgreRoot.loadPlugin( realRoot, pluginName, out errorPointer );
			//				string error = OgreNativeWrapper.GetOutString( errorPointer );
			//				if( error != null )
			//				{
			//					Log.Fatal( "RendererWorld: Loading plugin \"{0}\" failed ({1}).", pluginName, error );
			//					return false;
			//				}
			//			}
			//		}
			//	}
			//}

			////Render system
			//RenderingSystem.Init();

			////!!!!!!всё таки выключать можно для NULL рендеринга?
			////renderSystem.MaxPixelShadersVersion = MaxPixelShadersVersions.PS30;
			////renderSystem.MaxVertexShadersVersion = MaxVertexShadersVersions.VS30;
			//RenderingSystem.Direct3DFPUPreserve = EngineApp.InitSettings.RenderingDirect3DFPUPreserve;

			//!!!!
			////hardware instancing settings
			//if( configBlock != null )
			//{
			//	TextBlock hardwareInstancingBlock = configBlock.FindChild( "hardwareInstancing" );
			//	if( hardwareInstancingBlock != null )
			//	{
			//		if( hardwareInstancingBlock.AttributeExists( "hardwareInstancingMaxObjectCountInBatch" ) )
			//		{
			//			RenderingSystem.HardwareInstancingMaxObjectCountInBatch = int.Parse(
			//				hardwareInstancingBlock.GetAttribute( "hardwareInstancingMaxObjectCountInBatch" ) );
			//		}

			//		if( hardwareInstancingBlock.AttributeExists( "hardwareInstancingMaxObjectVertexCount" ) )
			//		{
			//			RenderingSystem.HardwareInstancingMaxObjectVertexCount = int.Parse(
			//				hardwareInstancingBlock.GetAttribute( "hardwareInstancingMaxObjectVertexCount" ) );
			//		}
			//	}
			//}

			//!!!!
			////shaderCacheManager
			//if( shaderCacheManager != null )
			//	shaderCacheManager.RendererInit();

			unsafe
			{
				OgreRoot.initialise( realRoot );
			}

			////Render window
			//OgreRenderWindow* realRenderWindow;
			//{
			//	OgreNameValuePairList* _params = (OgreNameValuePairList*)OgreNameValuePairList.New();

			//	OgreNameValuePairList.insert( _params, "colourDepth", EngineApp.platform.GetScreenBitsPerPixel().ToString() );

			//	//if( verticalSyncInitialized )
			//	//   OgreNameValuePairList.insert( _params, "vsync", verticalSync ? "true" : "false" );
			//	OgreNameValuePairList.insert( _params, "vsync", EngineApp.InitSettings.RenderingVerticalSync ? "true" : "false" );

			//	//!!!! Code duplication !

			//	if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			//	{
			//		// dont set externalWindowHandle
			//	}
			//	else
			//	{
			//		IntPtr externalWindowHandle = EngineApp.ApplicationWindowHandle;
			//		if( externalWindowHandle != IntPtr.Zero )
			//		{
			//			if( IntPtr.Size == 8 )
			//			{
			//				UInt64 hwnd = (UInt64)externalWindowHandle.ToInt64();
			//				OgreNameValuePairList.insert( _params, "externalWindowHandle", hwnd.ToString() );
			//			}
			//			else
			//			{
			//				uint hwnd = (uint)externalWindowHandle.ToInt32();
			//				OgreNameValuePairList.insert( _params, "externalWindowHandle", hwnd.ToString() );
			//			}
			//		}
			//	}

			//	IntPtr glContext = EngineApp.platform.GetGLContext();
			//	if( glContext != IntPtr.Zero )
			//	{
			//		if( IntPtr.Size == 8 )
			//		{
			//			UInt64 context = (UInt64)glContext.ToInt64();
			//			OgreNameValuePairList.insert( _params, "externalGLContext", context.ToString() );
			//		}
			//		else
			//		{
			//			OgreNameValuePairList.insert( _params, "externalGLContext", glContext.ToString() );
			//		}
			//	}

			//	if( EngineApp.InitSettings.RenderingHardwareFullScreenAntialiasing != 0 )
			//		OgreNameValuePairList.insert( _params, "FSAA", EngineApp.InitSettings.RenderingHardwareFullScreenAntialiasing.ToString() );
			//	//if( !InitializationOptions.AllowSceneMRTRendering )
			//	//{
			//	//   int fsaa;
			//	//   if( !int.TryParse( InitializationOptions.FullSceneAntialiasing, out fsaa ) )
			//	//      fsaa = 0;
			//	//   if( fsaa != 0 )
			//	//      OgreNameValuePairList.insert( _params, "FSAA", fsaa.ToString() );
			//	//}

			//	if( multiMonitorMode )
			//		OgreNameValuePairList.insert( _params, "multiMonitorMode", "true" );
			//	if( Debugger.IsAttached )
			//		OgreNameValuePairList.insert( _params, "debugger", "true" );

			//	{
			//		IntPtr errorPointer;
			//		realRenderWindow = (OgreRenderWindow*)OgreRoot.createRenderWindow( realRoot,
			//			"MainWindow", 640, 480, startedAtFullScreen, _params, out errorPointer );
			//		string error = OgreNativeWrapper.GetOutString( errorPointer );
			//		if( error != null )
			//		{
			//			Log.Fatal( "RendererWorld: Initializing window failed ({0}).", error );
			//			return false;
			//		}
			//	}

			//	OgreNameValuePairList.Delete( _params );

			//	if( realRenderWindow == null )
			//	{
			//		Log.Fatal( "RendererWorld: Creation render window failed." );
			//		return false;
			//	}
			//}

			GpuProgramManager.Init();

			RenderingSystem.InitGPUSettingsAndCapabilities();

			applicationRenderWindow = new RenderWindow( FrameBuffer.Invalid, initialWindowSize, EngineApp.ApplicationWindowHandle, true );
			//applicationRenderWindow.WindowMovedOrResized( xxx );
			//!!!!!?, mainRenderTargetSize );

			//!!!!!!если это новый редактор, то не так.
			//было
			//applicationRenderWindow.AllowAdditionalMRTs = true;

			//Scene manager
			MyOgreSceneManager* realSceneManager = (MyOgreSceneManager*)OgreRoot.createSceneManager( realRoot, "NeoAxisSceneManager" );
			sceneManager = new OgreSceneManager( realSceneManager );

			EngineFontManager.Init( fontManagerDefaultLanguage );

			//Create viewport
			Viewport viewport = applicationRenderWindow.AddViewport( true, true );
			// RenderCamera.Purposes.UsualScene );// mainRenderTargetCamera );
			//viewport.Camera.AllowFrustumCullingTestMode = true;
			//mainRenderTargetViewport = mainRenderTarget.AddViewport( RenderCamera.Purposes.UsualScene );// mainRenderTargetCamera );
			//mainRenderTargetCamera = mainRenderTargetViewport.ViewportCamera;
			//mainRenderTargetCamera.AllowFrustumCullingTestMode = true;

			{
				IntPtr errorPointer;
				OgreResourceGroupManager.initialiseAllResourceGroups( realRoot, out errorPointer );
				string error = OgreNativeWrapper.GetOutString( errorPointer );
				if( error != null )
				{
					Log.Error( string.Format( "Renderer: {0}", error ) );
					return false;
				}
			}

			//Ogre initialization errors
			if( resourceInitializationErrors.Count != 0 )
			{
				string text = "Renderer initialization errors:\n\n";
				foreach( string message in resourceInitializationErrors )
					text += message + "\n";
				resourceInitializationErrors.Clear();
				Log.Error( text );
				return false;
			}
			resourcesInitialized = true;

			////animation defaults
			//OgreAnimation.setDefaultInterpolationMode( OgreAnimation.InterpolationMode.IM_LINEAR );
			//OgreAnimation.setDefaultRotationInterpolationMode(
			//   OgreAnimation.RotationInterpolationMode.RIM_LINEAR );

			GpuBufferManager.Init();
			//LowLevelMaterialManager.Init();
			//!!!!!
			//MeshManager_Old.Init();
			//MeshManager_Old.Init();
			//ModelImportConfigurationManager.Init();
			//!!!!!
			//SkeletonManager_Old.Init();

			//GpuProgramManager.Init();
			//CompositorManager.Init();
			//!!!!!было
			//ParticleSystemManager.Init();

			//_old_GpuProgramCacheManager.Init();
			//!!!!!HighLevelMaterialManager.Init();

			//!!!!!
			//ResourceLoadingManagerInBackground.Init();

			//!!!!!было
			//AnimationTreeManager.Init();

			//Rectangle2DRenderer.Init();

			//!!!!менять в real-time? кому нельзя менять?
			//filtering
			{
				//!!!!
				////get recommeded settings
				//if( EngineApp.InitSettings.RenderingFilteringMode == FilteringMode.RecommendedSetting )
				//{
				//	if( RenderingSystem.GPUIsGeForce() )
				//		EngineApp.InitSettings.RenderingFilteringMode = FilteringMode.Anisotropic8x;
				//	else if( RenderingSystem.GPUIsRadeon() )
				//		EngineApp.InitSettings.RenderingFilteringMode = FilteringMode.Anisotropic8x;
				//	else
				//		EngineApp.InitSettings.RenderingFilteringMode = FilteringMode.Anisotropic4x;
				//}

				//!!!!!
				////apply
				//switch( EngineApp.InitSettings.RenderingFilteringMode )
				//{
				//case FilteringMode.Trilinear:
				//	LowLevelMaterialManager.Instance.SetDefaultTextureFiltering( TextureFilterOption.Trilinear );
				//	break;

				//case FilteringMode.Anisotropic2x:
				//	LowLevelMaterialManager.Instance.SetDefaultTextureFiltering( TextureFilterOption.Anisotropic );
				//	LowLevelMaterialManager.Instance.DefaultAnisotropy = 2;
				//	break;

				//case FilteringMode.Anisotropic4x:
				//	LowLevelMaterialManager.Instance.SetDefaultTextureFiltering( TextureFilterOption.Anisotropic );
				//	LowLevelMaterialManager.Instance.DefaultAnisotropy = 4;
				//	break;

				//case FilteringMode.Anisotropic8x:
				//	LowLevelMaterialManager.Instance.SetDefaultTextureFiltering( TextureFilterOption.Anisotropic );
				//	LowLevelMaterialManager.Instance.DefaultAnisotropy = 8;
				//	break;

				//case FilteringMode.Anisotropic16x:
				//	LowLevelMaterialManager.Instance.SetDefaultTextureFiltering( TextureFilterOption.Anisotropic );
				//	LowLevelMaterialManager.Instance.DefaultAnisotropy = 16;
				//	break;
				//}
			}

			return true;
		}

		internal static unsafe void Shutdown()
		{
			foreach( var target in renderTargets.ToArray() )
				target.DisposeInternal();
			//while( renderTargets.Count != 0 )
			//{
			//	Dictionary<IntPtr, RenderTarget>.Enumerator enumerator = renderTargets.GetEnumerator();
			//	enumerator.MoveNext();
			//	enumerator.Current.Value.DisposeInternal();
			//}

			while( canvasRenderers.Count != 0 )
			{
				foreach( var r in canvasRenderers )
					r.Dispose();
			}

			//Rectangle2DRenderer.Shutdown();

			//!!!!!было
			//AnimationTreeManager.Shutdown();

			//!!!!!
			//ResourceLoadingManagerInBackground.Shutdown();

			//!!!!HighLevelMaterialManager.Shutdown();
			//_old_GpuProgramCacheManager.Shutdown();
			//!!!!!было
			//ParticleSystemManager.Shutdown();
			//CompositorManager.Shutdown();
			GpuProgramManager.Shutdown();
			//_old_GpuProgramManager.Shutdown();
			//!!!!!
			//SkeletonManager_Old.Shutdown();
			//ModelImportConfigurationManager.Shutdown();
			//!!!!!
			//MeshManager_Old.Shutdown();
			//MeshManager_Old.Shutdown();
			EngineFontManager.Shutdown();

			//!!!!!temp. как делать?
			//!!!!!какие еще ресурсы так удалять? и где?
			//delete resources of renderer
			//{
			//	List<Resource> toDelete = new List<Resource>();
			//	foreach( var v in ResourceManager.Instances )
			//	{
			//		var obj = v.ResultObject as Resource;

			//		if( obj is GpuTexture || obj is Material || obj is Mesh )
			//			toDelete.Add( obj );
			//	}
			//	foreach( var v in toDelete )
			//		v.Dispose();
			//}

			//LowLevelMaterialManager.Shutdown();
			GpuBufferManager.Shutdown();

			if( applicationRenderWindow != null )
			{
				applicationRenderWindow.Dispose();
				applicationRenderWindow = null;
			}
			//mainRenderTargetCamera = null;
			//if( mainRenderTargetCamera != null )
			//{
			//   mainRenderTargetCamera.Dispose();
			//   mainRenderTargetCamera = null;
			//}

			if( sceneManager != null )
			{
				sceneManager.Dispose();
				sceneManager = null;
			}

			//if( mainRenderTarget != null )
			//{
			//   mainRenderTarget.DisposeInternal();
			//   mainRenderTarget = null;
			//}

			//RenderingSystem.Dispose();

			if( logListener != null )
			{
				OgreLogManager.getDefaultLog_removeListener( realRoot, logListener );
				MyOgreLogListener.Delete( logListener );
				logListener = null;
			}

			//unload plugins
			//{
			//if( loadedRenderSystemPlugin != null )
			//{
			//   OgreRoot.unloadPlugin( realRoot, loadedRenderSystemPlugin );
			//   loadedRenderSystemPlugin = null;
			//}
			//}

			//!!!!
			//ShaderCacheManager = null;
			//ChangeShaderCacheManager( null );

			MyOgreVirtualFileSystem.Shutdown();

			//!!!!!валится
			//if( realRoot != null )
			//{
			//	OgreRoot.Delete( realRoot );
			//	realRoot = null;
			//}
			//OgreRoot.Delete();

			Bgfx.Shutdown();

			disposed = true;
		}

		static unsafe void logListener_messageLogged( string message, OgreLogMessageLevel lml, bool maskDebug )
		{
			//!!!!только из основного может прийти?
			EngineThreading.CheckMainThread();


			if( !_EnableInternalLogMessages )
				return;

			string text = "Renderer: " + message;

			bool logWrited = false;
			bool internalLogMessageCalled = false;

			if( lml == OgreLogMessageLevel.LML_CRITICAL || text.Contains( "Error" ) )
			{
				string t = message;

				//remove prefix
				{
					string check1 = "OGRE EXCEPTION(";
					string check2 = "): ";

					if( t.Contains( check1 ) && t.Contains( check2 ) )
					{
						int index = t.IndexOf( check2 ) + check2.Length;
						t = t.Substring( index );
					}
				}

				if( !_InvisibleInternalLogMessages )
				{
					if( !resourcesInitialized )
					{
						resourceInitializationErrors.Add( t );
					}
					else
					{
						if( !t.Contains( "Cannot locate resource " ) )
						{
							if( !rendererAdditionInitialized && resourcesInitialized )
							{
								//Specific for RendererAddition
								Log.Fatal( t );
							}
							else
								Log.Warning( t );
							logWrited = true;
						}
					}
				}

				if( _InternalLogMessage != null )
					_InternalLogMessage( t );

				internalLogMessageCalled = true;
			}

			if( !logWrited )
				Log.InvisibleInfo( text );

			if( !internalLogMessageCalled )
			{
				if( _InternalLogMessage != null )
					_InternalLogMessage( text );
			}
		}

		public static RenderWindow CreateRenderWindow( IntPtr parentWindowHandle, Vector2I size )
		{
			EngineThreading.CheckMainThread();

			var frameBuffer = new FrameBuffer( parentWindowHandle, size.X, size.Y );
			var renderWindow = new RenderWindow( frameBuffer, size, parentWindowHandle, false );
			return renderWindow;

			//unsafe
			//{
			//	OgreRenderWindow* realRenderWindow;

			//	OgreNameValuePairList* _params = (OgreNameValuePairList*)OgreNameValuePairList.New();

			//	OgreNameValuePairList.insert( _params, "colourDepth", EngineApp.platform.GetScreenBitsPerPixel().ToString() );

			//	//!!!! Code duplication !

			//	if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			//	{
			//		// dont set externalWindowHandle
			//	}
			//	else
			//	{
			//		if( IntPtr.Size == 8 )
			//		{
			//			UInt64 hwnd = (UInt64)parentWindowHandle.ToInt64();
			//			OgreNameValuePairList.insert( _params, "externalWindowHandle", hwnd.ToString() );
			//		}
			//		else
			//		{
			//			uint hwnd = (uint)parentWindowHandle.ToInt32();
			//			OgreNameValuePairList.insert( _params, "externalWindowHandle", hwnd.ToString() );
			//		}
			//	}

			//	if( EngineApp.InitSettings.RenderingHardwareFullScreenAntialiasing != 0 )
			//		OgreNameValuePairList.insert( _params, "FSAA", EngineApp.InitSettings.RenderingHardwareFullScreenAntialiasing.ToString() );
			//	//if( !InitializationOptions.AllowSceneMRTRendering )
			//	//{
			//	//   int fsaa;
			//	//   if( !int.TryParse( InitializationOptions.FullSceneAntialiasing, out fsaa ) )
			//	//      fsaa = 0;
			//	//   if( fsaa != 0 )
			//	//      OgreNameValuePairList.insert( _params, "FSAA", fsaa.ToString() );
			//	//}

			//	unchecked
			//	{
			//		renderWindowNameCounter++;
			//	}
			//	string name = string.Format( "RenderWindow{0}", renderWindowNameCounter );

			//	{
			//		IntPtr errorPointer;
			//		realRenderWindow = (OgreRenderWindow*)OgreRoot.createRenderWindow( realRoot, name,
			//			size.X, size.Y, false, _params, out errorPointer );
			//		string error = OgreNativeWrapper.GetOutString( errorPointer );
			//		if( error != null )
			//		{
			//			Log.Fatal( "RendererWorld: Initializing window failed ({0}).", error );
			//			return null;
			//		}
			//	}

			//	OgreNameValuePairList.Delete( _params );

			//	if( realRenderWindow == null )
			//	{
			//		Log.Warning( "RendererWorld: CreateRenderWindow: Creation render window failed." );
			//		return null;
			//	}

			//	var renderWindow = new RenderWindow( frameBuffer, size, parentWindowHandle, false );
			//	return renderWindow;
			//}
		}

		//!!!!тут?
		public static MultiRenderTarget CreateMultiRenderTarget( MultiRenderTarget.Item[] items )
		{
			//!!!!
			EngineThreading.CheckMainThread();

			Attachment[] attachments = new Attachment[ items.Length ];
			for( int n = 0; n < attachments.Length; n++ )
			{
				var item = items[ n ];
				var attachment = new Attachment();
				attachment.Texture = item.texture.Result.RealObject;
				attachment.Mip = item.mip;
				attachment.Layer = item.layer;
				attachment.Access = ComputeBufferAccess.Write;
				attachments[ n ] = attachment;
			}

			var frameBuffer = new FrameBuffer( attachments );
			var mrt = new MultiRenderTarget( frameBuffer, items );
			return mrt;

			//unchecked
			//{
			//	multiRenderTargetNameCounter++;
			//}
			//string name = string.Format( "MultiRenderTarget{0}", multiRenderTargetNameCounter );

			//unsafe
			//{
			//	OgreMultiRenderTarget* realMRT = (OgreMultiRenderTarget*)OgreRenderSystem.createMultiRenderTarget( realRoot, name );

			//	var mrt = new MultiRenderTarget( realMRT );
			//	return mrt;
			//}
		}

		//!!!!
		////!!!!!потом разрулится это всё
		//public static ShaderCacheManager ShaderCacheManager
		//{
		//	get { return shaderCacheManager; }
		//	set { shaderCacheManager = value; }
		//}

		//public static void ChangeShaderCacheManager( ShaderCacheManager newManager )
		//{
		//   if( shaderCacheManager != null )
		//      shaderCacheManager.Dispose();

		//   shaderCacheManager = newManager;
		//}

		//static unsafe void profilingToolBeginOperation( string operationName )
		//{
		//   ProfilingTool.ActiveTool_BeginOperation( operationName );
		//}

		//static unsafe void profilingToolEndOperation( string operationName )
		//{
		//   ProfilingTool.ActiveTool_EndOperation( operationName );
		//}

		//!!!!
		//public bool EnableTimeProgress
		//{
		//	get { return enableTimeProgress; }
		//	set { enableTimeProgress = value; }
		//}

		//!!!!
		//!!!!threading
		public static IList<Viewport> ViewportsDuringUpdate
		{
			get { return viewportsDuringUpdate; }
		}

		//public Viewport CurrentViewportInUpdate
		//{
		//	get
		//	{
		//		if( currentViewportsInUpdateStack.Count == 0 )
		//			return null;
		//		return currentViewportsInUpdateStack[ currentViewportsInUpdateStack.Count - 1 ];
		//	}
		//}

		//public bool InsideBeginRenderFrameEvent
		//{
		//   get { return insideBeginRenderFrameEvent; }
		//}

		//public bool InsideRenderOneFrame
		//{
		//	get { return insideRenderOneFrame; }
		//}

		public static bool Disposed
		{
			get { return disposed; }
		}

		public static Metadata.TypeInfo RenderingPipelineDefault
		{
			get
			{
				if( renderingPipelineDefault == null )
				{
					renderingPipelineDefault = MetadataManager.GetType( "NeoAxis.Component_RenderingPipeline_Default" );
					if( renderingPipelineDefault == null )
						Log.Fatal( "RendererWord: RenderingPipelineDefault: Get: No \'NeoAxis.Component_RenderingPipeline_Default\' type." );
				}
				return renderingPipelineDefault;
			}
		}




		public static bool BackendNull
		{
			get { return Capabilities.Backend == RendererBackend.Noop; }
		}

		public delegate void RenderSystemEventDelegate( RenderSystemEvent name );

		//!!!!threading
		//!!!!!!вызывать
		/// <summary>
		/// Occurs when the render system event is generated.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Some render systems have quite specific, internally generated events 
		/// that the application may wish to be notified of. Many applications
		/// don't have to worry about these events, and can just trust engine to 
		/// handle them, but if you want to know, you can add a listener here.
		/// </para>
		/// <para>
		/// Perhaps the most common example of a render system specific event is the 
		/// loss and restoration of a device in DirectX; which engine deals with, 
		/// but you may wish to know when it happens. 
		/// </para>
		/// </remarks>
		public static event RenderSystemEventDelegate RenderSystemEvent;

		//public delegate void RenderTargetPreUpdateEventDelegate( RenderTarget renderTarget );
		//public event RenderTargetPreUpdateEventDelegate RenderTargetPreUpdate;

		///// <summary>
		///// D3D specific method to return whether the device has been lost.
		///// </summary>
		internal static bool IsDeviceLost()
		{
			//!!!!!!
			EngineThreading.CheckMainThread();

			//!!!!
			return false;
			//unsafe
			//{
			//	if( OgreRenderSystem.isDeviceLost( RendererWorld.realRoot ) )
			//		return true;
			//	if( IsDeviceLostByTestCooperativeLevel() )
			//		return true;
			//	return false;
			//}
		}

		//!!!!!!!
		/// <summary>
		/// D3D specific method to return whether the device has been lost.
		/// </summary>
		public static bool IsDeviceLostByTestCooperativeLevel()
		{
			//!!!!!!
			EngineThreading.CheckMainThread();

			//!!!!
			return false;
			//unsafe
			//{
			//	return OgreRenderSystem.isDeviceLostByTestCooperativeLevel( RendererWorld.realRoot );
			//}
		}

		//!!!!!!!
		public static bool RestoreDeviceAfterLost()
		{
			EngineThreading.CheckMainThread();

			//!!!!
			return true;
			//unsafe
			//{
			//	return OgreRenderSystem.restoreDeviceAfterLost( RendererWorld.realRoot );
			//}
		}

		//!!!!
		static void Listener_eventOccurred( string name )
		{
			if( RenderSystemEvent != null )
			{
				RenderSystemEvent type;

				if( name == "DeviceLost" )
					type = NeoAxis.RenderSystemEvent.DeviceLost;
				else if( name == "DeviceRestored" )
					type = NeoAxis.RenderSystemEvent.DeviceRestored;
				else
				{
					Log.Fatal( "RenderSystem: Unknown render system event \"{0}\".", name );
					return;
				}

				RenderSystemEvent( type );
			}
		}

		internal static void InitGPUSettingsAndCapabilities()
		{
			Capabilities = Bgfx.GetCaps();

			Log.InvisibleInfo( "Renderer: Backend: " + Capabilities.Backend.ToString() );
			Log.InvisibleInfo( "Renderer: Adapter vendor: " + Capabilities.CurrentAdapter.Vendor.ToString() );
			Log.InvisibleInfo( "Renderer: Adapter device: " + Capabilities.CurrentAdapter.DeviceId.ToString() );
			Log.InvisibleInfo( "Renderer: Adapter description: " + Bgfx.GetGPUDescription() );
			Log.InvisibleInfo( "Renderer: -------------------------" );
		}

		/// <summary>
		/// Generates a packed data version of the passed in <see cref="NeoAxis.ColorValue"/>.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The uint value.</returns>
		///// <remarks>
		///// Since different render systems have different color data formats (eg
		///// RGBA for GL, ARGB for D3D) this method allows you to use 1 method for all.
		///// </remarks>
		public static uint ConvertColorValue( ref ColorValue color )
		{
			var packed = new ColorByte( ref color );
			return packed.PackedValue;
		}

		/// <summary>
		/// Generates a packed data version of the passed in <see cref="NeoAxis.ColorValue"/> suitable for use as with this RenderSystem.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The uint value.</returns>
		///// <remarks>
		///// Since different render systems have different color data formats (eg
		///// RGBA for GL, ARGB for D3D) this method allows you to use 1 method for all.
		///// </remarks>
		public static uint ConvertColorValue( ColorValue color )
		{
			return ConvertColorValue( ref color );
		}

		internal static int CallBgfxFrame()
		{
			int result = Bgfx.Frame();
			UpdateOcclusionQuery();
			return result;
		}

		public delegate void OcclusionQueryResult( object callbackParameter, int passingPixels );

		/// <summary>
		/// Tries to create an occlusion query. An occlusion query object cannot be created every frame, only in a specific frame (every fourth frame). Created object must used only in the current frame. If the pixels are drawn successfully, it will call callback.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="callbackParameter"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static bool TryCreateOcclusionQuery( OcclusionQueryResult callback, object callbackParameter, out OcclusionQuery query )
		{
			if( canCreateOcclusionQueriesCounter == 0 )
			{
				var item = new OcclusionQueryItem();
				item.query = OcclusionQuery.Create();
				item.query.SetCondition( true );
				item.callback = callback;
				item.callbackParameter = callbackParameter;
				occlusionQueries.Add( item );

				query = item.query;
				return true;
			}

			query = OcclusionQuery.Invalid;
			return false;
		}

		static void UpdateOcclusionQuery()
		{
			canCreateOcclusionQueriesCounter++;
			//!!!!3?
			if( canCreateOcclusionQueriesCounter > 3 )
			{
				foreach( var item in occlusionQueries )
				{
					if( item.query.Result == SharpBgfx.OcclusionQueryResult.Visible )
						item.callback( item.callbackParameter, item.query.PassingPixels );
				}

				foreach( var item in occlusionQueries.GetReverse() )
					item.query.Dispose();

				occlusionQueries.Clear();
				canCreateOcclusionQueriesCounter = 0;
			}
		}
	}
}
