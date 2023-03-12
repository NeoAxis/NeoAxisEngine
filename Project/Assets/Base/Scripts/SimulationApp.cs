// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// The class for general management of the project application.
	/// </summary>
	public static class SimulationApp
	{
		static UIControl currentUIScreen;
		static bool firstTick = true;

		//music
		static SoundChannelGroup musicChannelGroup;

		//debug info
		static double showEngineInfoLastTime;
		static long showEngineInfoManagedMemory;
		static long showEngineInfoNativeMemory;
		static long showEngineInfoGPUMemory;

		/////////////////////////////////////////

		static double soundVolume = 0.8;
		[EngineConfig]
		public static double SoundVolume
		{
			get { return soundVolume; }
			set
			{
				soundVolume = value;
				if( EngineApp.DefaultSoundChannelGroup != null )
					EngineApp.DefaultSoundChannelGroup.Volume = soundVolume;
			}
		}

		static double musicVolume = 0.8;
		[EngineConfig]
		public static double MusicVolume
		{
			get { return musicVolume; }
			set
			{
				musicVolume = value;
				if( musicChannelGroup != null )
					musicChannelGroup.Volume = musicVolume;
			}
		}

		static string antialiasingBasic = "";
		[EngineConfig]
		public static string AntialiasingBasic
		{
			get { return antialiasingBasic; }
			set { antialiasingBasic = value; }
		}

		static string antialiasingMotion = "";
		[EngineConfig]
		public static string AntialiasingMotion
		{
			get { return antialiasingMotion; }
			set { antialiasingMotion = value; }
		}

		static string resolutionUpscaleMode = "";
		[EngineConfig]
		public static string ResolutionUpscaleMode
		{
			get { return resolutionUpscaleMode; }
			set { resolutionUpscaleMode = value; }
		}

		static string resolutionUpscaleTechnique = "";
		[EngineConfig]
		public static string ResolutionUpscaleTechnique
		{
			get { return resolutionUpscaleTechnique; }
			set { resolutionUpscaleTechnique = value; }
		}

		static double sharpness = -1.0;
		[EngineConfig]
		public static double Sharpness
		{
			get { return sharpness; }
			set { sharpness = value; }
		}

		[EngineConfig]
		public static bool DisplayFrameInfo { get; set; }
		[EngineConfig]
		public static bool DisplaySceneInfo { get; set; }
		[EngineConfig]
		public static bool DisplayEngineInfo { get; set; }
		[EngineConfig]
		public static Vector2I VideoMode { get; set; }
		[EngineConfig]
		public static bool Fullscreen { get; set; } = true;
		[EngineConfig]
		public static bool VerticalSync { get; set; } = true;
		[EngineConfig]
		public static bool DisplayBackgroundScene { get; set; } = true;

		static double lodScale = 1.0;
		[EngineConfig]
		public static double LODScale
		{
			get { return lodScale; }
			set
			{
				lodScale = value;
				RenderingPipeline.GlobalLODScale = LODScale;
			}
		}

		static double textureQuality = 1.0;
		[EngineConfig]
		public static double TextureQuality
		{
			get { return textureQuality; }
			set
			{
				textureQuality = value;
				RenderingPipeline_Basic.GlobalTextureQuality = TextureQuality;
			}
		}

		static double shadowQuality = 1.0;
		[EngineConfig]
		public static double ShadowQuality
		{
			get { return shadowQuality; }
			set
			{
				shadowQuality = value;
				RenderingPipeline_Basic.GlobalShadowQuality = ShadowQuality;
			}
		}

		static double indirectLightingMultiplier = 1.0;
		[EngineConfig]
		public static double IndirectLightingMultiplier
		{
			get { return indirectLightingMultiplier; }
			set
			{
				indirectLightingMultiplier = value;
				RenderingEffect_IndirectLighting.GlobalMultiplier = IndirectLightingMultiplier;
			}
		}

		static double ambientOcclusionMultiplier = 1.0;
		[EngineConfig]
		public static double AmbientOcclusionMultiplier
		{
			get { return ambientOcclusionMultiplier; }
			set
			{
				ambientOcclusionMultiplier = value;
				RenderingEffect_AmbientOcclusion.GlobalMultiplier = AmbientOcclusionMultiplier;
			}
		}

		static double reflectionMultiplier = 1.0;
		[EngineConfig]
		public static double ReflectionMultiplier
		{
			get { return reflectionMultiplier; }
			set
			{
				reflectionMultiplier = value;
				RenderingEffect_Reflection.GlobalMultiplier = ReflectionMultiplier;
			}
		}

		static double motionBlurMultiplier = 1.0;
		[EngineConfig]
		public static double MotionBlurMultiplier
		{
			get { return motionBlurMultiplier; }
			set
			{
				motionBlurMultiplier = value;
				RenderingEffect_MotionBlur.GlobalMultiplier = MotionBlurMultiplier;
			}
		}

		static double depthOfFieldBlurFactor = 1.0;
		[EngineConfig]
		public static double DepthOfFieldBlurFactor
		{
			get { return depthOfFieldBlurFactor; }
			set
			{
				depthOfFieldBlurFactor = value;
				RenderingEffect_DepthOfField.GlobalBlurFactor = DepthOfFieldBlurFactor;
			}
		}

		static double bloomScale = 1.0;
		[EngineConfig]
		public static double BloomScale
		{
			get { return bloomScale; }
			set
			{
				bloomScale = value;
				RenderingEffect_Bloom.GlobalScale = BloomScale;
			}
		}

		static bool networkLogging;
		[EngineConfig]
		public static bool NetworkLogging
		{
			get { return networkLogging; }
			set
			{
				networkLogging = value;
				NeoAxis.Networking.NetworkCommonSettings.NetworkLogging = networkLogging;
			}
		}

		//[EngineConfig]
		//public static bool ShowEngineWatermark { get; set; }

		/////////////////////////////////////////

		public static event Action MainViewportRenderUI;

		/////////////////////////////////////////

		public static Viewport MainViewport
		{
			get { return RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ]; }
		}

		public static UIControl CurrentUIScreen
		{
			get { return currentUIScreen; }
		}

		public static bool ChangeUIScreen( string fileName, bool destroyNetworkClient )
		{
			if( destroyNetworkClient )
				SimulationAppClient.Destroy();

			//load
			UIControl newScreen = null;
			if( !string.IsNullOrEmpty( fileName ) )
			{
				newScreen = ResourceManager.LoadSeparateInstance<UIControl>( fileName, false, true );
				if( newScreen == null )
					return false;
			}

			//remove previous
			if( currentUIScreen != null )
			{
				currentUIScreen.RemoveFromParent( false );
				currentUIScreen = null;
			}

			//enable
			if( newScreen != null )
			{
				currentUIScreen = newScreen;
				MainViewport.UIContainer.AddComponent( currentUIScreen );

				currentUIScreen.ResetCreateTime();

				//reset mouse state
				MainViewport.MouseRelativeMode = false;
			}

			return true;
		}

		/////////////////////////////////////////

		public static void EngineApp_AppCreateBefore()
		{
			//register [EngineConfig] fields, properties
			EngineConfig.RegisterClassParameters( typeof( SimulationApp ) );

			//creation settings

			if( !Fullscreen )
				EngineApp.InitSettings.CreateWindowFullscreen = false;
			if( VideoMode != Vector2I.Zero && ( SystemSettings.VideoModeExists( VideoMode ) || !Fullscreen ) )
			{
				EngineApp.InitSettings.CreateWindowSize = VideoMode;
				if( !Fullscreen )
					EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Normal;
			}
			if( !VerticalSync )
				EngineApp.InitSettings.SimulationVSync = false;

			//get from project settings
			{
				var windowStateString = ProjectSettings.ReadParameterFromFile( "WindowState" );
				if( !string.IsNullOrEmpty( windowStateString ) )
				{
					if( Enum.TryParse<ProjectSettingsPage_General.WindowStateEnum>( windowStateString, out var windowState ) )
					{
						if( windowState != ProjectSettingsPage_General.WindowStateEnum.Auto )
						{
							switch( windowState )
							{
							case ProjectSettingsPage_General.WindowStateEnum.Normal:
								{
									EngineApp.InitSettings.CreateWindowFullscreen = false;
									EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Normal;

									var windowSizeString = ProjectSettings.ReadParameterFromFile( "WindowSize", ProjectSettingsPage_General.WindowSizeDefault.ToString() );
									if( !string.IsNullOrEmpty( windowSizeString ) )
									{
										try
										{
											EngineApp.InitSettings.CreateWindowSize = Vector2I.Parse( windowSizeString );
										}
										catch { }
									}
								}
								break;

							case ProjectSettingsPage_General.WindowStateEnum.Minimized:
								EngineApp.InitSettings.CreateWindowFullscreen = false;
								EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Minimized;
								break;

							case ProjectSettingsPage_General.WindowStateEnum.Maximized:
								EngineApp.InitSettings.CreateWindowFullscreen = false;
								EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Maximized;
								break;

							case ProjectSettingsPage_General.WindowStateEnum.Fullscreen:
								EngineApp.InitSettings.CreateWindowFullscreen = true;
								break;
							}
						}
					}
				}
			}
		}

		public static void EngineApp_AppCreateAfter()
		{
			Log.Handlers.InvisibleInfoHandler += InvisibleLog_Handlers_InfoHandler;
			Log.Handlers.InfoHandler += Log_Handlers_InfoHandler;
			Log.Handlers.WarningHandler += Log_Handlers_WarningHandler;
			Log.Handlers.ErrorHandler += Log_Handlers_ErrorHandler;
			Log.Handlers.FatalHandler += Log_Handlers_FatalHandler;

			EngineApp.RegisterConfigParameter += EngineApp_RegisterConfigParameter;

			EngineConsole.Init();
			//GameControlsManager.Init();

			//UIControl engineLoadingWindow = ResourceManager.LoadSeparateInstance<UIControl>( @"Base\UI\Windows\EngineLoadingWindow.ui", false, null );
			//if( engineLoadingWindow != null )
			//	MainViewport.UIContainer.AddComponent( engineLoadingWindow );

			////Subcribe to callbacks during engine loading. We will render scene from callback.
			//LongOperationCallbackManager.Subscribe( LongOperationCallbackManager_LoadingCallback, programLoadingWindow );

			EngineApp.Tick += EngineApp_Tick;

			////finish initialization of materials and hide loading window.
			////LongOperationCallbackManager.Unsubscribe();
			//if( engineLoadingWindow != null )
			//	engineLoadingWindow.RemoveFromParent( true );

			//subscribe to main viewport events
			MainViewport.KeyDown += MainViewport_KeyDown;
			MainViewport.KeyPress += MainViewport_KeyPress;
			MainViewport.KeyUp += MainViewport_KeyUp;
			MainViewport.MouseDown += MainViewport_MouseDown;
			MainViewport.MouseUp += MainViewport_MouseUp;
			MainViewport.MouseDoubleClick += MainViewport_MouseDoubleClick;
			MainViewport.MouseMove += MainViewport_MouseMove;
			MainViewport.MouseWheel += MainViewport_MouseWheel;
			MainViewport.JoystickEvent += MainViewport_JoystickEvent;
			MainViewport.Touch += MainViewport_Touch;
			MainViewport.SpecialInputDeviceEvent += MainViewport_SpecialInputDeviceEvent;
			MainViewport.UpdateBegin += MainViewport_UpdateBegin;
			MainViewport.UpdateBeforeOutput += MainViewport_UpdateBeforeOutput;
			MainViewport.UpdateEnd += MainViewport_UpdateEnd;

			//change application title
			if( EngineApp.CreatedInsideEngineWindow != null )
				EngineApp.CreatedInsideEngineWindow.Title = ProjectSettings.Get.General.ProjectName;

			//update sound volume
			if( EngineApp.DefaultSoundChannelGroup != null )
				EngineApp.DefaultSoundChannelGroup.Volume = soundVolume;

			//create music channel group
			musicChannelGroup = SoundWorld.CreateChannelGroup( "Music" );
			SoundWorld.MasterChannelGroup.AddGroup( musicChannelGroup );
			musicChannelGroup.Volume = musicVolume;

			EngineApp.EnginePausedChanged += EngineApp_EnginePausedChanged;
		}

		public static void EngineApp_AppDestroy()
		{
		}

		private static void EngineApp_RegisterConfigParameter( EngineConfig.Parameter parameter )
		{
			EngineConsole.RegisterConfigParameter( parameter );
		}

		static bool IsNeedDisableKeyboardAndMouseInput()
		{
			return false;
			//return IsScreenFadingOut();
		}

		private static void MainViewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			//engine console
			if( EngineConsole.PerformKeyDown( e ) )
			{
				handled = true;
				return;
			}

			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_KeyPress( Viewport viewport, KeyPressEvent e, ref bool handled )
		{
			//engine console
			if( EngineConsole.PerformKeyPress( e ) )
			{
				handled = true;
				return;
			}

			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_KeyUp( Viewport viewport, KeyEvent e, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_MouseMove( Viewport viewport, Vector2 mouse )//, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				//handled = true;
				return;
			}
		}

		private static void MainViewport_MouseWheel( Viewport viewport, int delta, ref bool handled )
		{
			//engine console
			if( EngineConsole.PerformMouseWheel( delta ) )
			{
				handled = true;
				return;
			}

			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_JoystickEvent( Viewport viewport, JoystickInputEvent e, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_Touch( Viewport viewport, TouchData e, ref bool handled )
		{
			//engine console
			if( EngineConsole.PerformTouch( e ) )
			{
				handled = true;
				return;
			}

			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void MainViewport_SpecialInputDeviceEvent( Viewport viewport, InputEvent e, ref bool handled )
		{
			//disable input processing
			if( IsNeedDisableKeyboardAndMouseInput() )
			{
				handled = true;
				return;
			}
		}

		private static void EngineApp_Tick( float delta )
		{
			//perform update viewport, attached scene, UI container
			MainViewport.PerformTick( delta );

			//process screen messages
			ScreenMessages.PerformTick( delta );

			//engine console
			EngineConsole.PerformTick( delta );

			RunServer.Update();

			if( firstTick )
				FirstTickActions();

			firstTick = false;
		}

		private static void MainViewport_UpdateBegin( Viewport viewport )
		{
		}

		static void MainViewport_RenderUI()
		{
			//configure cursor file name
			EngineApp.SystemCursorFileName = @"Base\UI\Cursors\DefaultSystem.cur";

			//Draw UI controls
			MainViewport.UIContainer.PerformRenderUI( MainViewport.CanvasRenderer );

			// Process screen messages.
			ScreenMessages.PerformRenderUI( MainViewport );

			//debug info on the screen
			if( DisplayFrameInfo || DisplaySceneInfo || DisplayEngineInfo )
			{
				var lines = new List<string>();

				if( DisplayFrameInfo )
				{
					var statistics = MainViewport.RenderingContext?.UpdateStatisticsPrevious;
					if( statistics != null )
					{
						lines.Add( SystemSettings.CPUDescription );
						lines.Add( Internal.SharpBgfx.Bgfx.GetGPUDescription() );
						lines.Add( "" );
						lines.Add( "FPS: " + statistics.FPS.ToString( "F1" ) );
						lines.Add( "Triangles: " + statistics.Triangles.ToString( "N0" ) );
						lines.Add( "Lines: " + statistics.Lines.ToString( "N0" ) );
						lines.Add( "Draw calls: " + statistics.DrawCalls.ToString( "N0" ) );
						lines.Add( "Instances: " + statistics.Instances.ToString( "N0" ) );
						lines.Add( "Compute dispatches: " + statistics.ComputeDispatches.ToString( "N0" ) );
						lines.Add( "Render targets: " + statistics.RenderTargets.ToString( "N0" ) );
						lines.Add( "Dynamic textures: " + statistics.DynamicTextures.ToString( "N0" ) );
						lines.Add( "Compute write images: " + statistics.ComputeWriteImages.ToString( "N0" ) );
						lines.Add( "Lights: " + statistics.Lights.ToString( "N0" ) );
						lines.Add( "Reflection probes: " + statistics.ReflectionProbes.ToString( "N0" ) );
						lines.Add( "Occlusion culling buffers: " + statistics.OcclusionCullingBuffers.ToString( "N0" ) );
					}
				}

				if( DisplaySceneInfo )
				{
					var playScreen = CurrentUIScreen as PlayScreen;
					var scene = playScreen?.Scene;
					if( scene == null && Scene.GetAllInstancesEnabled().Length != 0 )
						scene = Scene.GetAllInstancesEnabled()[ 0 ];

					if( scene != null )
					{
						if( lines.Count != 0 )
							lines.Add( "" );

						var components = 0;
						var meshesInSpace = 0;
						var rigidBodies = 0;
						var groupOfObjectsItems = 0L;
						scene.GetComponents( false, true, false, false, delegate ( Component c )
						{
							components++;
							if( c is MeshInSpace )
								meshesInSpace++;
							else if( c is RigidBody )
								rigidBodies++;
							else if( c is GroupOfObjects groupOfObjects )
								groupOfObjectsItems += groupOfObjects.ObjectsGetCount();
						} );
						lines.Add( "Components: " + components.ToString( "N0" ) );
						lines.Add( "MeshInSpace components: " + meshesInSpace.ToString( "N0" ) );
						lines.Add( "GroupOfObjects items: " + groupOfObjectsItems.ToString( "N0" ) );
						lines.Add( "RigidBody components: " + rigidBodies.ToString( "N0" ) );

						if( scene.PhysicsWorld != null )
						{
							scene.PhysicsWorld.GetStatistics( out var shapes, out var allBodies, out var kinematicDynamicBodies, out var activeBodies );
							if( allBodies != 0 )
							{
								lines.Add( "Physics shapes: " + shapes.ToString( "N0" ) );
								lines.Add( "Physics bodies: " + allBodies.ToString( "N0" ) );
								lines.Add( "Physics kinematic and dynamic bodies: " + kinematicDynamicBodies.ToString( "N0" ) );
								lines.Add( "Physics active bodies: " + activeBodies.ToString( "N0" ) );
							}
						}

						var physicsWorld2D = scene.Physics2DGetWorld( false );
						if( physicsWorld2D != null )
						{
							var rigidBodyCount = 0;
							var dynamicRigidBodyCount = 0;
							var activeRigidBodyCount = 0;

							foreach( var worldBody in physicsWorld2D.BodyList )
							{
								rigidBodyCount++;
								if( worldBody.BodyType == Internal.tainicom.Aether.Physics2D.Dynamics.BodyType.Dynamic )
								{
									dynamicRigidBodyCount++;
									if( worldBody.Awake )
										activeRigidBodyCount++;
								}
							}

							if( rigidBodyCount != 0 )
							{
								lines.Add( "Physics 2D rigid bodies: " + rigidBodyCount.ToString( "N0" ) );
								lines.Add( "Physics 2D dynamic rigid bodies: " + dynamicRigidBodyCount.ToString( "N0" ) );
								lines.Add( "Physics 2D active rigid bodies: " + activeRigidBodyCount.ToString( "N0" ) );
							}
						}

						if( scene.GetOctreeStatistics( out var objectCount, out var octreeBounds, out var octreeNodeCount, out var timeSinceLastFullRebuild ) )
						{
							lines.Add( "Octree objects: " + objectCount.ToString( "N0" ) );
							lines.Add( "Octree nodes: " + octreeNodeCount.ToString( "N0" ) );
							var size = octreeBounds.GetSize();
							lines.Add( "Octree bounds size: " + size.X.ToString( "N1" ) + " " + size.Y.ToString( "N1" ) + " " + size.Z.ToString( "N1" ) );
							lines.Add( "Octree since last full rebuild: " + timeSinceLastFullRebuild.ToString( "F1" ) );
						}
					}
				}

				if( DisplayEngineInfo )
				{
					if( lines.Count != 0 )
						lines.Add( "" );

					var stats = Internal.SharpBgfx.Bgfx.GetStats();

					if( showEngineInfoLastTime + 0.25 < Time.Current )
					{
						showEngineInfoLastTime = Time.Current;
						showEngineInfoManagedMemory = GC.GetTotalMemory( false );
						NativeUtility.GetCRTStatistics( out showEngineInfoNativeMemory, out _ );
						showEngineInfoGPUMemory = stats.GpuMemoryUsed;
					}

					lines.Add( "Managed memory: " + ( showEngineInfoManagedMemory / 1024 / 1024 ).ToString( "N0" ) + " MB" );
					lines.Add( "Native memory: " + ( showEngineInfoNativeMemory / 1024 / 1024 ).ToString( "N0" ) + " MB" );
					lines.Add( "GPU memory" + ": " + ( showEngineInfoGPUMemory / 1024 / 1024 ).ToString( "N0" ) + " / " + ( stats.MaxGpuMemory / 1024 / 1024 ).ToString( "N0" ) + " MB" );
				}

				var renderer = MainViewport.CanvasRenderer;
				var fontSize = renderer.DefaultFontSize;
				var offset = new Vector2( fontSize * renderer.AspectRatioInv * 0.8, fontSize * 0.6 );

				////draw background
				//{
				//	var maxLength = 0.0;
				//	foreach( var line in lines )
				//	{
				//		var length = renderer.DefaultFont.GetTextLength( fontSize, renderer, line );
				//		if( length > maxLength )
				//			maxLength = length;
				//	}
				//	var rect = offset + new Rectangle( 0, 0, maxLength, fontSize * lines.Count );
				//	rect.Expand( new Vector2( fontSize * 0.2, fontSize * 0.2 * renderer.AspectRatio ) );
				//	renderer.AddQuad( rect, new ColorValue( 0, 0, 0, 0.4 ) );
				//}

				//draw text
				CanvasRendererUtility.AddTextLinesWithShadow( MainViewport, null, fontSize, lines, new Rectangle( offset.X, offset.Y, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			}

			MainViewportRenderUI?.Invoke();

			//engine console
			EngineConsole.PerformRenderUI();

			//!!!!too blurry
			//if( ShowEngineWatermark )
			//{
			//	var texture = ResourceManager.LoadResource<ImageComponent>( @"Base\UI\Images\Watermark.png" );
			//	if( texture != null )
			//	{
			//		var renderer = MainViewport.CanvasRenderer;

			//		var imageSize = texture.Result.SourceSize;
			//		var aspect = (double)imageSize.X / (double)imageSize.Y;
			//		var size = 0.1;

			//		var screenSize = new Vector2( size * renderer.AspectRatioInv * aspect, size );

			//		renderer.AddQuad( new Rectangle( 1.0 - screenSize.X, 1.0 - screenSize.Y, 1, 1 ), new RectangleF( 0, 0, 1, 1 ), texture, new ColorValue( 1, 1, 1, 0.8 ), true );
			//	}
			//}
		}

		private static void MainViewport_UpdateBeforeOutput( Viewport viewport )
		{
			MainViewport_RenderUI();
		}

		private static void MainViewport_UpdateEnd( Viewport viewport )
		{
		}

		//protected override void OnSystemPause( bool pause )
		//{
		//	base.OnSystemPause( pause );

		//	if( EntitySystemWorld.Instance != null )
		//		EntitySystemWorld.Instance.SystemPauseOfSimulation = pause;
		//}

		//bool IsScreenFadingOut()
		//{
		//	//if( needMapLoadName != null || needRunExampleOfProceduralMapCreation || needWorldLoadName != null )
		//	//	return true;
		//	//if( needFadingOutAndExit )
		//	//	return true;
		//	return false;
		//}

		static void InvisibleLog_Handlers_InfoHandler( string text, ref bool dumpToLogFile )
		{
		}

		static void Log_Handlers_InfoHandler( string text, ref bool dumpToLogFile )
		{
			EngineConsole.Print( text );
		}

		static void Log_Handlers_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			handled = true;
			EngineConsole.Print( "Warning: " + text, new ColorValue( 1, 0, 0 ) );
			if( EngineConsole.AutoOpening )
				EngineConsole.Active = true;
		}

		static void Log_Handlers_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			handled = true;
			EngineConsole.Print( "Error: " + text, new ColorValue( 1, 0, 0 ) );
			if( EngineConsole.AutoOpening )
				EngineConsole.Active = true;

			//if( MainViewport != null && MainViewport.UIContainer != null )
			//{
			//	handled = true;

			//	//find already created MessageBoxWindow
			//	foreach( UIControl control in MainViewport.UIContainer.GetComponents<UIControl>( false ) )
			//	{
			//		if( control is MessageBoxWindow && !control.RemoveFromParentQueued )
			//			return;
			//	}

			//	bool insideTheGame = false;
			//	//bool insideTheGame = GameWindow.Instance != null;

			//	//if( insideTheGame )
			//	//{
			//	//	if( Map.Instance != null )
			//	//	{
			//	//		if( EntitySystemWorld.Instance.IsServer() || EntitySystemWorld.Instance.IsSingle() )
			//	//			EntitySystemWorld.Instance.Simulation = false;
			//	//	}

			//	//	EngineApp.Instance.MouseRelativeMode = false;

			//	//	DeleteAllGameWindows();

			//	//	MapSystemWorld.MapDestroy();
			//	//	if( EntitySystemWorld.Instance != null )
			//	//		EntitySystemWorld.Instance.WorldDestroy();
			//	//}

			//	//GameEngineApp.Instance.Server_DestroyServer( "Error on the server" );
			//	//GameEngineApp.Instance.Client_DisconnectFromServer();

			//	//show message box

			//	MessageBoxWindow messageBoxWindow = new MessageBoxWindow( text, "Error", delegate ( UIButton sender )
			//	{
			//		if( insideTheGame )
			//		{
			//			//close all windows
			//			foreach( UIControl control in MainViewport.UIContainer.GetComponents<UIControl>( false ) )
			//				control.RemoveFromParent( true );
			//		}
			//		else
			//		{
			//			////destroy Lobby Window
			//			//foreach( UIControl control in MainViewport.ControlManager.Controls )
			//			//{
			//			//	if( control is MultiplayerLobbyWindow )
			//			//	{
			//			//		control.SetShouldDetach();
			//			//		break;
			//			//	}
			//			//}
			//		}

			//		//if( EntitySystemWorld.Instance == null )
			//		//{
			//		//	EngineApp.Instance.NeedExit = true;
			//		//	return;
			//		//}

			//		////create main menu
			//		//if( MainMenuWindow.Instance == null )
			//		//	MainViewport.UIContainer.AddComponent( new MainMenuWindow() );

			//	} );

			//	MainViewport.UIContainer.AddComponent( messageBoxWindow );
			//}
		}

		static void Log_Handlers_FatalHandler( string text, string createdLogFilePath, ref bool handled )
		{
		}

		static void FirstTickActions()
		{
			if( !SimulationAppClient.Created )
			{
				string playFile;
				if( SystemSettings.CommandLineParameters.TryGetValue( "-play", out playFile ) )
				{
					try
					{
						if( Path.IsPathRooted( playFile ) )
							playFile = VirtualPathUtility.GetVirtualPathByReal( playFile );
					}
					catch { }
				}

				if( string.IsNullOrEmpty( playFile ) && ProjectSettings.Get.General.AutorunScene.ReferenceSpecified )
				{
					var res = ProjectSettings.Get.General.AutorunScene.Value;
					if( res != null && VirtualFile.Exists( res.ResourceName ) )
						playFile = res.ResourceName;
				}

				if( !string.IsNullOrEmpty( playFile ) && VirtualFile.Exists( playFile ) )
				{
					PlayFile( playFile );
				}
				else
				{
					//default start screen
					if( !string.IsNullOrEmpty( ProjectSettings.Get.General.InitialUIScreen.GetByReference ) )
						ChangeUIScreen( ProjectSettings.Get.General.InitialUIScreen.GetByReference, false );
				}
			}
		}

		public static void PlayFile( string virtualFileName )
		{
			if( !string.IsNullOrEmpty( virtualFileName ) && VirtualFile.Exists( virtualFileName ) && !SimulationAppClient.Created )
			{
				//load Play screen
				if( ChangeUIScreen( @"Base\UI\Screens\PlayScreen.ui", true ) )
				{
					var playScreen = CurrentUIScreen as PlayScreen;
					if( playScreen != null )
					{
						playScreen.Load( virtualFileName, true );

						GC.Collect();
						GC.WaitForPendingFinalizers();

						playScreen.ResetCreateTime();
					}
				}
			}
		}

		public static void NetworkClientSceneCreated( Scene scene )
		{
			//load Play screen
			if( ChangeUIScreen( @"Base\UI\Screens\PlayScreen.ui", false ) )
			{
				var playScreen = CurrentUIScreen as PlayScreen;
				if( playScreen != null )
				{
					playScreen.NetworkClientSetScene( scene, true );

					GC.Collect();
					GC.WaitForPendingFinalizers();

					playScreen.ResetCreateTime();
				}
			}
		}

		public static void NetworkClientSceneDestroyed()
		{
			if( PlayScreen.Instance != null )
				PlayScreen.Instance.DestroyScene();
			ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui", false );
		}

		private static void EngineApp_EnginePausedChanged( bool pause )
		{
			//reset motion blur
			if( RenderingSystem.ApplicationRenderTarget != null )
			{
				foreach( var viewport in RenderingSystem.ApplicationRenderTarget.Viewports )
					viewport.NotifyInstantCameraMovement();
			}
		}

		public static void UpdateSceneAntialiasingByAppSettings( Scene scene )
		{
			var pipeline = scene.RenderingPipeline.Value;
			if( pipeline != null )
			{
				var effect = pipeline.GetComponent<RenderingEffect_Antialiasing>( true );
				if( effect != null )
				{
					if( Enum.TryParse<RenderingEffect_Antialiasing.BasicTechniqueEnum>( AntialiasingBasic.Replace( " ", "" ), true, out var basic ) )
						effect.BasicTechnique = basic;
					else
						effect.BasicTechnique = effect.BasicTechniqueAfterLoading;

					if( Enum.TryParse<RenderingEffect_Antialiasing.MotionTechniqueEnum>( AntialiasingMotion.Replace( " ", "" ), true, out var motion ) )
						effect.MotionTechnique = motion;
					else
						effect.MotionTechnique = effect.MotionTechniqueAfterLoading;
				}
			}
		}

		public static void UpdateSceneResolutionUpscaleByAppSettings( Scene scene )
		{
			var pipeline = scene.RenderingPipeline.Value;
			if( pipeline != null )
			{
				var effect = pipeline.GetComponent<RenderingEffect_ResolutionUpscale>( true );
				if( effect != null )
				{
					if( Enum.TryParse<RenderingEffect_ResolutionUpscale.ModeEnum>( ResolutionUpscaleMode.Replace( " ", "" ), true, out var mode ) )
						effect.Mode = mode;
					else
						effect.Mode = effect.ModeAfterLoading;

					var techniqueString = ResolutionUpscaleTechnique;
					if( techniqueString == "Lanczos 2" )
						techniqueString = "Lanczos2";
					if( techniqueString == "AMD FSR 1.0" )
						techniqueString = "AMDFSR1";

					if( Enum.TryParse<RenderingEffect_ResolutionUpscale.TechniqueEnum>( techniqueString, true, out var technique ) )
						effect.Technique = technique;
					else
						effect.Technique = effect.TechniqueAfterLoading;
				}
			}
		}

		public static void UpdateSceneSharpnessByAppSettings( Scene scene )
		{
			var pipeline = scene.RenderingPipeline.Value;
			if( pipeline != null )
			{
				var effect = pipeline.GetComponent<RenderingEffect_Sharpen>( true );
				if( effect != null )
				{
					if( Sharpness >= 0 )
						effect.Strength = Sharpness;
					else
						effect.Strength = effect.StrengthAfterLoading;
				}
			}
		}

	}
}
