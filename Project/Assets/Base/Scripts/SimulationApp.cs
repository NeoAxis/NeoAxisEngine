// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
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
		public static bool DisplayViewportStatistics { get; set; }
		[EngineConfig]
		public static Vector2I VideoMode { get; set; }
		[EngineConfig]
		public static bool Fullscreen { get; set; } = true;
		[EngineConfig]
		public static bool VerticalSync { get; set; } = true;
		[EngineConfig]
		public static bool DisplayBackgroundScene { get; set; } = true;

		/////////////////////////////////////////

		public static Viewport MainViewport
		{
			get { return RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ]; }
		}

		public static UIControl CurrentUIScreen
		{
			get { return currentUIScreen; }
		}

		public static bool ChangeUIScreen( string fileName )
		{
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
			// Perform update viewport, attached scene, UI container.
			MainViewport.PerformTick( delta );

			// Process screen messages.
			ScreenMessages.PerformTick( delta );

			//engine console
			EngineConsole.PerformTick( delta );

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

			//viewport statistics
			if( DisplayViewportStatistics )
			{
				var statistics = MainViewport.RenderingContext?.UpdateStatisticsPrevious;
				if( statistics != null )
				{
					var lines = new List<string>();
					lines.Add( "FPS: " + statistics.FPS.ToString( "F1" ) );
					lines.Add( "Triangles: " + statistics.Triangles.ToString() );
					lines.Add( "Lines: " + statistics.Lines.ToString() );
					lines.Add( "Draw calls: " + statistics.DrawCalls.ToString() );
					lines.Add( "Render targets: " + statistics.RenderTargets.ToString() );
					lines.Add( "Dynamic textures: " + statistics.DynamicTextures.ToString() );
					lines.Add( "Compute write images: " + statistics.ComputeWriteImages.ToString() );
					lines.Add( "Lights: " + statistics.Lights.ToString() );
					lines.Add( "Reflection probes: " + statistics.ReflectionProbes.ToString() );
					lines.Add( "Occlusion culling buffers: " + statistics.OcclusionCullingBuffers.ToString() );

					var renderer = MainViewport.CanvasRenderer;
					var fontSize = renderer.DefaultFontSize;
					var offset = new Vector2( fontSize * renderer.AspectRatioInv * 0.8, fontSize * 0.6 );

					CanvasRendererUtility.AddTextLinesWithShadow( MainViewport, null, fontSize, lines, new Rectangle( offset.X, offset.Y, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
				}
			}

			//engine console
			EngineConsole.PerformRenderUI();
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
					ChangeUIScreen( ProjectSettings.Get.General.InitialUIScreen.GetByReference );
			}
		}

		public static void PlayFile( string virtualFileName )
		{
			if( !string.IsNullOrEmpty( virtualFileName ) && VirtualFile.Exists( virtualFileName ) )
			{
				//load Play screen
				if( ChangeUIScreen( @"Base\UI\Screens\PlayScreen.ui" ) )
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
