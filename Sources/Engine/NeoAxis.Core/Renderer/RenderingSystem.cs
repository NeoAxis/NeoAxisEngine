// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Internal.SharpBgfx;
using System.Runtime.CompilerServices;
using NeoAxis.Editor;

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

		//OgreRoot.profilingToolBeginOperationDelegate profilingToolBeginOperationDelegate;
		//OgreRoot.profilingToolEndOperationDelegate profilingToolEndOperationDelegate;

		//!!!!!
		static internal List<Viewport> viewportsDuringUpdate = new List<Viewport>();

		static bool disposed;

		//occlusion query
		static int canCreateOcclusionQueriesCounter;
		class OcclusionQueryItem
		{
			public OcclusionQuery query;
			public OcclusionQueryResult callback;
			public object callbackParameter;
		}
		static List<OcclusionQueryItem> occlusionQueries = new List<OcclusionQueryItem>();

		internal static int currentViewNumber = -1;
		static int lastFrameNumber;

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
				if( EngineApp.InitSettings.RendererReportDebugToLog )
				{
					sbyte* buffer = stackalloc sbyte[ 1024 ];
					NativeMethods.bgfx_vsnprintf( buffer, new IntPtr( 1024 ), format, args );
					var str = Marshal.PtrToStringAnsi( new IntPtr( buffer ) );
					Log.Info( "Renderer: Bgfx: " + str );
				}
			}

			public void ReportError( string fileName, int line, ErrorType errorType, string message )
			{
				if( EngineApp.AfterFatalOperations )
					return;

				string text = $"{fileName} ({line})  {errorType}: {message}";

				Debug.Write( text );
				Log.InvisibleInfo( text );

				var skipLogFatal = false;
				if( EngineApp.IsEditor && !EngineApp.Closing && EditorAssemblyInterface.Instance != null )
				{
					//switch to after crash mode
					EngineApp.AfterFatalOperations = true;
					EditorAssemblyInterface.Instance.AfterFatalShowDialogAndSaveDocuments( text, ref skipLogFatal );
				}

				if( !skipLogFatal )
					Log.Fatal( $"Renderer: Fatal error: " + text );


				//if( errorType == ErrorType.DebugCheck )
				//{
				//	if( EngineApp.InitSettings.RendererReportDebugToLog )
				//		Log.Warning( "Renderer: Bgfx: " + text );

				//	if( EngineApp.IsEditor && !EngineApp.Closing && EditorAssemblyInterface.Instance != null )
				//	{
				//		//switch to after crash mode
				//		EngineApp.AfterFatalOperations = true;
				//		EditorAssemblyInterface.Instance.AfterFatalShowDialogAndSaveDocuments( text );
				//	}
				//	else
				//	{
				//		//!!!!

				//		// Debug.Fail terminate app in UWP
				//		if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
				//			Debug.Fail( text ); // thread independed, debug only message box.
				//	}
				//}
				//else
				//{
				//	Log.Fatal( $"Renderer: Bgfx fatal error: " + text );
				//	Debug.Fail( text ); // thread independed, debug only message box.
				//						//Environment.Exit( 1 ); ?
				//}
			}

			public void SaveScreenShot( string path, int width, int height, int pitch, IntPtr data, int size, bool flipVertical ) { }
			public void SetCacheEntry( long id, IntPtr data, int size ) { }
		}

		///////////////////////////////////////////////

		//[DllImport( "kernel32.dll", EntryPoint = "SetDllDirectory", CharSet = CharSet.Unicode )]
		//static extern bool SetDllDirectory( string lpPathName );

		static bool nativeDLLsPreloaded;
		internal static void NativeDLLsPreload()
		{
			if( !nativeDLLsPreloaded )
			{
				NativeUtility.PreloadLibrary( "NeoAxisCoreNative" );
				NativeUtility.PreloadLibrary( Internal.SharpBgfx.NativeMethods.DllName );
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

		internal static void PostInitRendererAddition()
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
		/// Gets all the render targets collection.
		/// </summary>
		public static RenderTarget[] GetAllRenderTargets()
		{
			lock( renderTargets )
				return renderTargets.ToArray();
		}

		/// <summary>
		/// Gets the viewports collection.
		/// </summary>
		public static Viewport[] GetAllViewports()
		{
			lock( viewports )
				return viewports.ToArray();
		}

		internal static bool InvisibleInternalLogMessages
		{
			get { return invisibleInternalLogMessages; }
			set { invisibleInternalLogMessages = value; }
		}

		internal static bool EnableInternalLogMessages
		{
			get { return enableInternalLogMessages; }
			set { enableInternalLogMessages = value; }
		}

		internal delegate void _InternalLogMessageDelegate( string message );
		internal static event _InternalLogMessageDelegate InternalLogMessage;

		internal static ResetFlags GetApplicationWindowResetFlags()
		{
			ResetFlags flags = 0;
			if( EngineApp.IsSimulation && EngineApp.InitSettings.SimulationVSync )
				flags |= ResetFlags.Vsync;
			//if( EngineApp.InitSettings.AnisotropicFiltering )
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

			var path = VirtualFileSystem.Directories.PlatformSpecific;
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				path = VirtualFileSystem.MakePathRelative( path );

			Vector2I initialWindowSize = new Vector2I( 10, 10 );
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				initialWindowSize = EngineApp.platform.CreatedWindow_GetClientRectangle().Size;

			//set backend for Android
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
			{
				EngineApp.InitSettings.RendererBackend = RendererBackend.OpenGLES;
				//EngineSettings.Init.RendererBackend = RendererBackend.Vulkan;
				//EngineSettings.Init.RendererBackend = RendererBackend.Noop;
			}
			//set backend for iOS
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
			{
				EngineApp.InitSettings.RendererBackend = RendererBackend.OpenGLES;
				//EngineSettings.Init.RendererBackend = RendererBackend.Metal;
				//EngineSettings.Init.RendererBackend = RendererBackend.Noop;
			}
			//set backend for Web
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
				EngineApp.InitSettings.RendererBackend = RendererBackend.OpenGLES;

			unsafe
			{
				NativeMethods.bgfx_check_wrapper( sizeof( InitSettings.Native ), sizeof( PlatformData ), sizeof( FrameBuffer.NativeAttachment ) );
			}

			//set platform data
			if( ( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android || SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web ) && EngineApp.InitSettings.RendererBackend == RendererBackend.OpenGLES )
			{
				//Android, OpenGLES
				Bgfx.SetPlatformData( new PlatformData { Context = (IntPtr)1 } );
			}
			else
				Bgfx.SetPlatformData( new PlatformData { WindowHandle = EngineApp.ApplicationWindowHandle } );

			if( EngineApp.IsSimulation && EngineApp.InitSettings.SimulationTripleBuffering )
				Bgfx.SetTripleBuffering();

			//Log.InvisibleInfo( "Renderer backend: " + EngineSettings.Init.RendererBackend.ToString() );

			var initSettings = new InitSettings
			{
				Backend = EngineApp.InitSettings.RendererBackend,
				CallbackHandler = new CallbackHandler(),

				////!!!!в релизе можно включить. в NeoAxis.DefaultSettings.config
				//Debug = true
				//!!!!
				//ResetFlags = ResetFlags.MSAA8x,
			};

			//!!!!configurable to NeoAxis.DefaultSettings.config?
			//initSettings.BackBufferCount = ;
			//initSettings.MaxFrameLatency = ;


			if( !Bgfx.Init( initSettings ) )
			{
				Log.Error( "Unable to initialize Bgfx." );
				return false;
			}

			Bgfx.Reset( initialWindowSize.X, initialWindowSize.Y, GetApplicationWindowResetFlags() );

			realRoot = (OgreRoot*)OgreRoot.New( path );

			//profilingToolBeginOperationDelegate = profilingToolBeginOperation;
			//profilingToolEndOperationDelegate = profilingToolEndOperation;
			//OgreRoot.setCallbackDelegates( realRoot,
			//   profilingToolBeginOperationDelegate, 
			//   profilingToolEndOperationDelegate );

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

			////!!!!!!всё таки выключать можно для NULL рендеринга?
			////renderSystem.MaxPixelShadersVersion = MaxPixelShadersVersions.PS30;
			////renderSystem.MaxVertexShadersVersion = MaxVertexShadersVersions.VS30;
			//RenderingSystem.Direct3DFPUPreserve = EngineApp.InitSettings.RenderingDirect3DFPUPreserve;

			unsafe
			{
				OgreRoot.initialise( realRoot );
			}

			GpuProgramManager.Init();

			InitGPUSettingsAndCapabilities();

			applicationRenderWindow = new RenderWindow( FrameBuffer.Invalid, initialWindowSize, EngineApp.ApplicationWindowHandle, true );
			//applicationRenderWindow.WindowMovedOrResized( xxx );
			//!!!!!?, mainRenderTargetSize );

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

			GpuBufferManager.Init();

			//!!!!!
			//ResourceLoadingManagerInBackground.Init();

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

			//!!!!!
			//ResourceLoadingManagerInBackground.Shutdown();

			GpuProgramManager.Shutdown();

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

			if( sceneManager != null )
			{
				sceneManager.Dispose();
				sceneManager = null;
			}

			if( logListener != null )
			{
				OgreLogManager.getDefaultLog_removeListener( realRoot, logListener );
				MyOgreLogListener.Delete( logListener );
				logListener = null;
			}

			MyOgreVirtualFileSystem.Shutdown();

			try
			{
				Bgfx.Shutdown();
			}
			catch { }

			disposed = true;
		}

		static unsafe void logListener_messageLogged( string message, OgreLogMessageLevel lml, bool maskDebug )
		{
			//!!!!только из основного может прийти?
			EngineThreading.CheckMainThread();


			if( !EnableInternalLogMessages )
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

				if( !InvisibleInternalLogMessages )
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

				if( InternalLogMessage != null )
					InternalLogMessage( t );

				internalLogMessageCalled = true;
			}

			if( !logWrited )
				Log.InvisibleInfo( text );

			if( !internalLogMessageCalled )
			{
				if( InternalLogMessage != null )
					InternalLogMessage( text );
			}
		}

		public static RenderWindow CreateRenderWindow( IntPtr parentWindowHandle, Vector2I size )
		{
			EngineThreading.CheckMainThread();

			var frameBuffer = new FrameBuffer( parentWindowHandle, size.X, size.Y );
			var renderWindow = new RenderWindow( frameBuffer, size, parentWindowHandle, false );
			return renderWindow;
		}

		public static MultiRenderTarget CreateMultiRenderTarget( MultiRenderTarget.Item[] items )
		{
			//!!!!
			EngineThreading.CheckMainThread();

			Attachment[] attachments = new Attachment[ items.Length ];
			for( int n = 0; n < attachments.Length; n++ )
			{
				var item = items[ n ];
				var attachment = new Attachment();
				attachment.Texture = item.texture.Result.GetNativeObject( true );
				attachment.Mip = item.mip;
				attachment.Layer = item.layer;
				//!!!!
				attachment.NumLayers = 1;
				attachment.Access = ComputeBufferAccess.Write;
				attachments[ n ] = attachment;
			}

			var frameBuffer = new FrameBuffer( attachments );
			var mrt = new MultiRenderTarget( frameBuffer, items );
			return mrt;
		}

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

		public static Metadata.TypeInfo RenderingPipelineBasic
		{
			get { return MetadataManager.GetTypeOfNetType( typeof( RenderingPipeline_Basic ) ); }
		}

		public static bool BackendNull
		{
			get { return Capabilities.Backend == RendererBackend.Noop; }
		}

		public delegate void RenderSystemEventDelegate( RenderSystemEvent name );

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

		//!!!!
		internal static void PerformRenderSystemEvent( RenderSystemEvent name )
		{
			RenderSystemEvent?.Invoke( name );
		}

		///// <summary>
		///// D3D specific method to return whether the device has been lost.
		///// </summary>
		public static bool IsDeviceLost()
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
		//static void Listener_eventOccurred( string name )
		//{
		//	if( RenderSystemEvent != null )
		//	{
		//		RenderSystemEvent type;

		//		if( name == "DeviceLost" )
		//			type = NeoAxis.RenderSystemEvent.DeviceLost;
		//		else if( name == "DeviceRestored" )
		//			type = NeoAxis.RenderSystemEvent.DeviceRestored;
		//		else
		//		{
		//			Log.Fatal( "RenderSystem: Unknown render system event \"{0}\".", name );
		//			return;
		//		}

		//		RenderSystemEvent( type );
		//	}
		//}

		static void InitGPUSettingsAndCapabilities()
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

		public static int CurrentViewNumber
		{
			get { return currentViewNumber; }
			set { currentViewNumber = value; }
		}

		//public static void ResetViews()
		//{
		//	for( int n = 0; n < currentViewNumber; n++ )
		//		Bgfx.ResetView( (ushort)n );
		//	currentViewNumber = -1;
		//}

		public static int CallFrame()
		{
			int result = Bgfx.Frame();
			lastFrameNumber = result;
			UpdateOcclusionQuery();

			//reset views
			for( int n = 0; n < currentViewNumber; n++ )
				Bgfx.ResetView( (ushort)n );
			currentViewNumber = -1;

			return result;
		}

		public static int LastFrameNumber
		{
			get { return lastFrameNumber; }
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
					if( item.query.Result == Internal.SharpBgfx.OcclusionQueryResult.Visible )
						item.callback( item.callbackParameter, item.query.PassingPixels );
				}

				foreach( var item in occlusionQueries.GetReverse() )
					item.query.Dispose();

				occlusionQueries.Clear();
				canCreateOcclusionQueriesCounter = 0;
			}
		}

		//the parameters must be cached

		static ProjectSettingsPage_Rendering.ShadowTechniqueEnum? shadowTechnique;
		public static ProjectSettingsPage_Rendering.ShadowTechniqueEnum ShadowTechnique
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !shadowTechnique.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						shadowTechnique = ProjectSettings.Get.Rendering.ShadowTechniqueLimitedDevice;
					else
						shadowTechnique = ProjectSettings.Get.Rendering.ShadowTechnique;
				}
				return shadowTechnique.Value;
			}
		}

		static ProjectSettingsPage_Rendering.ShadowTextureFormatEnum? shadowTextureFormat;
		public static ProjectSettingsPage_Rendering.ShadowTextureFormatEnum ShadowTextureFormat
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !shadowTextureFormat.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						shadowTextureFormat = ProjectSettings.Get.Rendering.ShadowTextureFormatLimitedDevice;
					else
						shadowTextureFormat = ProjectSettings.Get.Rendering.ShadowTextureFormat;
				}
				return shadowTextureFormat.Value;
			}
		}

		static bool? staticShadows;
		public static bool StaticShadows
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !staticShadows.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						staticShadows = ProjectSettings.Get.Rendering.StaticShadowsLimitedDevice;
					else
						staticShadows = ProjectSettings.Get.Rendering.StaticShadows;
				}
				return staticShadows.Value;
			}
		}

		static ShadowTextureSizeEnum? shadowMaxTextureSizeDirectionalLight;
		public static ShadowTextureSizeEnum ShadowMaxTextureSizeDirectionalLight
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !shadowMaxTextureSizeDirectionalLight.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						shadowMaxTextureSizeDirectionalLight = ProjectSettings.Get.Rendering.ShadowMaxTextureSizeDirectionalLightLimitedDevice;
					else
						shadowMaxTextureSizeDirectionalLight = ShadowTextureSizeEnum._8192;// ProjectSettings.Get.Rendering.ShadowMaxTextureSizeDirectionalLight;
				}
				return shadowMaxTextureSizeDirectionalLight.Value;
			}
		}

		static ShadowTextureSizeEnum? shadowMaxTextureSizePointLight;
		public static ShadowTextureSizeEnum ShadowMaxTextureSizePointLight
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !shadowMaxTextureSizePointLight.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						shadowMaxTextureSizePointLight = ProjectSettings.Get.Rendering.ShadowMaxTextureSizePointLightLimitedDevice;
					else
						shadowMaxTextureSizePointLight = ShadowTextureSizeEnum._8192;// ProjectSettings.Get.Rendering.ShadowMaxTextureSizePointLight;
				}
				return shadowMaxTextureSizePointLight.Value;
			}
		}

		static ShadowTextureSizeEnum? shadowMaxTextureSizeSpotLight;
		public static ShadowTextureSizeEnum ShadowMaxTextureSizeSpotLight
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !shadowMaxTextureSizeSpotLight.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						shadowMaxTextureSizeSpotLight = ProjectSettings.Get.Rendering.ShadowMaxTextureSizeSpotLightLimitedDevice;
					else
						shadowMaxTextureSizeSpotLight = ShadowTextureSizeEnum._8192;// ProjectSettings.Get.Rendering.ShadowMaxTextureSizeSpotLight;
				}
				return shadowMaxTextureSizeSpotLight.Value;
			}
		}

		//static ProjectSettingsPage_Rendering.CompressVerticesEnum? compressVertices;
		//public static ProjectSettingsPage_Rendering.CompressVerticesEnum CompressVertices
		//{
		//	get
		//	{
		//		if( compressVertices == null )
		//		{
		//			if( SystemSettings.LimitedDevice )
		//				compressVertices = ProjectSettings.Get.Rendering.CompressVerticesLimitedDevice;
		//			else
		//				compressVertices = ProjectSettings.Get.Rendering.CompressVertices;
		//		}
		//		return compressVertices.Value;
		//	}
		//}

		static bool? debugMode;
		public static bool DebugMode
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !debugMode.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						debugMode = ProjectSettings.Get.Rendering.DebugModeLimitedDevice;
					else
						debugMode = ProjectSettings.Get.Rendering.DebugMode;
				}
				return debugMode.Value;
			}
		}

		static bool? lightMask;
		public static bool LightMask
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !lightMask.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						lightMask = ProjectSettings.Get.Rendering.LightMaskLimitedDevice;
					else
						lightMask = ProjectSettings.Get.Rendering.LightMask;
				}
				return lightMask.Value;
			}
		}

		static bool? lightGrid;
		public static bool LightGrid
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !lightGrid.HasValue )
				{
					if( SystemSettings.LimitedDevice )
					{
						//!!!!disabled because samplers limit
						lightGrid = false;
						//lightGrid = ProjectSettings.Get.Rendering.LightGridLimitedDevice;
					}
					else
						lightGrid = ProjectSettings.Get.Rendering.LightGrid;
				}
				return lightGrid.Value;
			}
		}

		static int? displacementMaxSteps;
		public static int DisplacementMaxSteps
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !displacementMaxSteps.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						displacementMaxSteps = ProjectSettings.Get.Rendering.DisplacementMaxStepsLimitedDevice;
					else
						displacementMaxSteps = ProjectSettings.Get.Rendering.DisplacementMaxSteps;
				}
				return displacementMaxSteps.Value;
			}
		}

		static bool? tessellation;
		public static bool Tessellation
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !tessellation.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						tessellation = ProjectSettings.Get.Rendering.TessellationLimitedDevice;
					else
						tessellation = ProjectSettings.Get.Rendering.Tessellation;
				}
				return tessellation.Value;
			}
		}

		static bool? removeTextureTiling;
		public static bool RemoveTextureTiling
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !removeTextureTiling.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						removeTextureTiling = ProjectSettings.Get.Rendering.RemoveTextureTilingLimitedDevice;
					else
						removeTextureTiling = ProjectSettings.Get.Rendering.RemoveTextureTiling;
				}
				return removeTextureTiling.Value;
			}
		}

		static bool? motionVector;
		public static bool MotionVector
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !motionVector.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						motionVector = false;//ProjectSettings.Get.Rendering.MotionVectorLimitedDevice;
					else
						motionVector = ProjectSettings.Get.Rendering.MotionVector;
				}
				return motionVector.Value;
			}
		}

		//static bool? indirectLightingFullMode;
		//public static bool IndirectLightingFullMode
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get
		//	{
		//		if( indirectLightingFullMode == null )
		//		{
		//			if( SystemSettings.LimitedDevice )
		//				indirectLightingFullMode = ProjectSettings.Get.Rendering.IndirectLightingFullModeLimitedDevice;
		//			else
		//				indirectLightingFullMode = ProjectSettings.Get.Rendering.IndirectLightingFullMode;
		//		}
		//		return indirectLightingFullMode.Value;
		//	}
		//}

		static int? cutVolumeMaxAmount;
		public static int CutVolumeMaxAmount
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !cutVolumeMaxAmount.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						cutVolumeMaxAmount = ProjectSettings.Get.Rendering.CutVolumeMaxAmountLimitedDevice;
					else
						cutVolumeMaxAmount = ProjectSettings.Get.Rendering.CutVolumeMaxAmount;
				}
				return cutVolumeMaxAmount.Value;
			}
		}

		static bool? fadeByVisibilityDistance;
		public static bool FadeByVisibilityDistance
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !fadeByVisibilityDistance.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						fadeByVisibilityDistance = ProjectSettings.Get.Rendering.FadeByVisibilityDistanceLimitedDevice;
					else
						fadeByVisibilityDistance = ProjectSettings.Get.Rendering.FadeByVisibilityDistance;
				}
				return fadeByVisibilityDistance.Value;
			}
		}

		static bool? fog;
		public static bool Fog
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !fog.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						fog = ProjectSettings.Get.Rendering.FogLimitedDevice;
					else
						fog = ProjectSettings.Get.Rendering.Fog;
				}
				return fog.Value;
			}
		}

		static bool? smoothLOD;
		public static bool SmoothLOD
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !smoothLOD.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						smoothLOD = ProjectSettings.Get.Rendering.SmoothLODLimitedDevice;
					else
						smoothLOD = ProjectSettings.Get.Rendering.SmoothLOD;
				}
				return smoothLOD.Value;
			}
		}

		static bool? normalMapping;
		public static bool NormalMapping
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !normalMapping.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						normalMapping = ProjectSettings.Get.Rendering.NormalMappingLimitedDevice;
					else
						normalMapping = ProjectSettings.Get.Rendering.NormalMapping;
				}
				return normalMapping.Value;
			}
		}

		static bool? skeletalAnimation;
		public static bool SkeletalAnimation
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !skeletalAnimation.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						skeletalAnimation = ProjectSettings.Get.Rendering.SkeletalAnimationLimitedDevice;
					else
						skeletalAnimation = ProjectSettings.Get.Rendering.SkeletalAnimation;
				}
				return skeletalAnimation.Value;
			}
		}

		static bool? voxelLOD;
		public static bool VoxelLOD
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !voxelLOD.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						voxelLOD = false;//ProjectSettings.Get.Rendering.VoxelLODLimitedDevice;
					else
						voxelLOD = ProjectSettings.Get.Rendering.VoxelLOD;
				}
				return voxelLOD.Value;
			}
		}

		static int? voxelLODMaxSteps;
		public static int VoxelLODMaxSteps
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !voxelLODMaxSteps.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						voxelLODMaxSteps = 1;//ProjectSettings.Get.Rendering.VoxelLODMaxStepsLimitedDevice;
					else
						voxelLODMaxSteps = ProjectSettings.Get.Rendering.VoxelLODMaxSteps;
				}
				return voxelLODMaxSteps.Value;
			}
		}

		//static bool? virtualizedGeometry;
		//public static bool VirtualizedGeometry
		//{
		//	get
		//	{
		//		if( virtualizedGeometry == null )
		//		{
		//			if( SystemSettings.LimitedDevice )
		//				virtualizedGeometry = ProjectSettings.Get.Rendering.VirtualizedGeometryLimitedDevice;
		//			else
		//				virtualizedGeometry = ProjectSettings.Get.Rendering.VirtualizedGeometry;
		//		}
		//		return virtualizedGeometry.Value;
		//	}
		//}

		static ProjectSettingsPage_Rendering.MaterialShadingEnum? materialShading;
		public static ProjectSettingsPage_Rendering.MaterialShadingEnum MaterialShading
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !materialShading.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						materialShading = ProjectSettings.Get.Rendering.MaterialShadingLimitedDevice;
					else
						materialShading = ProjectSettings.Get.Rendering.MaterialShading;
				}
				return materialShading.Value;
			}
		}

		static bool? anisotropicFiltering;
		public static bool AnisotropicFiltering
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				//!!!!slowly проверять HasValue? везде так

				if( !anisotropicFiltering.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						anisotropicFiltering = ProjectSettings.Get.Rendering.AnisotropicFilteringLimitedDevice;
					else
						anisotropicFiltering = ProjectSettings.Get.Rendering.AnisotropicFiltering;
				}
				return anisotropicFiltering.Value;
			}
		}

		static bool? deferredShading;
		public static bool DeferredShading
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !deferredShading.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						deferredShading = false;//ProjectSettings.Get.Rendering.DeferredShadingLimitedDevice;
					else
						deferredShading = ProjectSettings.Get.Rendering.DeferredShading;
				}
				return deferredShading.Value;
			}
		}

		//static bool? globalIllumination;
		public static bool GlobalIllumination
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				//!!!!temp gi
				return false;

				//if( !globalIllumination.HasValue )
				//{
				//	if( SystemSettings.LimitedDevice )
				//		globalIllumination = ProjectSettings.Get.Rendering.GlobalIlluminationLimitedDevice;
				//	else
				//		globalIllumination = ProjectSettings.Get.Rendering.GlobalIllumination;
				//}
				//return globalIllumination.Value;
			}
		}

		static bool? environmentMapMixing;
		public static bool EnvironmentMapMixing
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !environmentMapMixing.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						environmentMapMixing = ProjectSettings.Get.Rendering.EnvironmentMapMixingLimitedDevice;
					else
						environmentMapMixing = ProjectSettings.Get.Rendering.EnvironmentMapMixing;
				}
				return environmentMapMixing.Value;
			}
		}

		static int? limitTextureSize;
		public static int LimitTextureSize
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( !limitTextureSize.HasValue )
				{
					if( SystemSettings.LimitedDevice )
						limitTextureSize = ProjectSettings.Get.Rendering.LimitTextureSizeLimitedDevice;
					else
						limitTextureSize = ProjectSettings.Get.Rendering.LimitTextureSize;
				}
				return limitTextureSize.Value;
			}
		}

		public static bool ReversedZ
		{
			get { return false; }
			//get { return Capabilities.Backend == RendererBackend.Direct3D11 || Capabilities.Backend == RendererBackend.Direct3D12; }
		}

		public static bool DepthBuffer32Float
		{
			get { return Capabilities.Backend == RendererBackend.Direct3D11 || Capabilities.Backend == RendererBackend.Direct3D12; }
		}
	}
}
