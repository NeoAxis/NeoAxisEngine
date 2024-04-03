// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class PlayScreen : UIControl
	{
		static PlayScreen instance;

		string playFileName = "";

		//load scene
		Scene scene;
		Viewport sceneViewport;
		GameMode gameMode;

		//load UI control
		UIControl uiControl;

		UIWindow menuWindow;

		bool firstRender = true;
		double fadeInTimer;

		///////////////////////////////////////////

		public PlayScreen()
		{
		}

		public static PlayScreen Instance
		{
			get { return instance; }
		}

		[Browsable( false )]
		public string PlayFileName
		{
			get { return playFileName; }
		}

		[Browsable( false )]
		public Scene Scene
		{
			get { return scene; }
		}

		[Browsable( false )]
		public GameMode GameMode
		{
			get { return gameMode; }
		}

		[Browsable( false )]
		public UIControl UIControl
		{
			get { return uiControl; }
		}

		protected override void OnAddedToParent()
		{
			if( EngineApp.IsSimulation )
			{
				instance = this;
				GameMode.UpdatePlayScreen( instance );

				//disable the Menu button on PC
				if( !SystemSettings.MobileDevice )
				{
					var button = GetComponent( "Button Menu" );
					if( button != null )
						button.Enabled = false;
				}

				//disable the Cutscene control
				var cutscene = GetComponent( "Cutscene" );
				if( cutscene != null )
					cutscene.Enabled = false;
			}

			base.OnAddedToParent();
		}

		protected override void OnRemovedFromParent( NeoAxis.Component oldParent )
		{
			DestroyLoadedObject();

			base.OnRemovedFromParent( oldParent );

			if( instance == this )
			{
				instance = null;
				GameMode.UpdatePlayScreen( instance );
			}
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				OpenOrCloseMenu();
				return true;
			}

			//!!!!SupressKeyPress
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageKeyDown( e.Key ) ) )
				return true;

			return base.OnKeyDown( e );
		}

		protected override bool OnKeyPress( KeyPressEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageKeyPress( e.KeyChar ) ) )
				return true;

			return base.OnKeyPress( e );
		}

		protected override bool OnKeyUp( KeyEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageKeyUp( e.Key ) ) )
				return true;

			return base.OnKeyUp( e );
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageMouseButtonDown( button ) ) )
				return true;

			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageMouseButtonUp( button ) ) )
				return true;

			return base.OnMouseUp( button );
		}

		protected override bool OnMouseDoubleClick( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageMouseDoubleClick( button ) ) )
				return true;

			return base.OnMouseDoubleClick( button );
		}

		protected override void OnMouseMove( Vector2 mouse )
		{
			base.OnMouseMove( mouse );

			//Game mode
			if( gameMode != null && gameMode.InputEnabled )
				gameMode?.ProcessInputMessage( new InputMessageMouseMove( mouse ) );
		}

		protected override bool OnMouseWheel( int delta )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageMouseWheel( delta ) ) )
				return true;

			return base.OnMouseWheel( delta );
		}

		protected override bool OnJoystickEvent( JoystickInputEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageJoystick( e ) ) )
				return true;

			return base.OnJoystickEvent( e );
		}

		protected override bool OnTouch( TouchData e )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageTouch( e ) ) )
				return true;

			return base.OnTouch( e );
		}

		protected override bool OnSpecialInputDeviceEvent( InputEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.InputEnabled && gameMode.ProcessInputMessage( new InputMessageSpecialInputDevice( e ) ) )
				return true;

			return base.OnSpecialInputDeviceEvent( e );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( menuWindow != null && menuWindow.Disposed )
				menuWindow = null;

			//Game mode
			if( sceneViewport != null )
			{
				var inputEnabled = InputEnabled;

				//send input enabled changed. sends each update
				gameMode?.ProcessInputMessage( new InputMessageInputEnabledChanged( inputEnabled ) );

				//update mouse relative mode
				{
					if( gameMode != null && inputEnabled && !RenderingSystem.BackendNull )
						sceneViewport.MouseRelativeMode = gameMode.IsNeedMouseRelativeMode();
					else
						sceneViewport.MouseRelativeMode = false;

					//send message. sends each update
					gameMode?.ProcessInputMessage( new InputMessageMouseRelativeModeChanged( sceneViewport.MouseRelativeMode ) );
				}
			}

			//update sound listener
			if( scene != null && sceneViewport != null )
			{
				var settings = sceneViewport.CameraSettings;
				SoundWorld.SetListener( scene, settings.Position, Vector3.Zero, settings.Rotation );
			}
			else
				SoundWorld.SetListenerReset();

			//scene simulation
			if( !SimulationAppClient.Created )
			{
				if( SimulationApp.Simulate )
					scene?.HierarchyController?.PerformSimulationSteps();
				ParentRoot.HierarchyController?.PerformSimulationSteps();
			}

			//Cutscene update
			if( EngineApp.IsSimulation && gameMode != null )
			{
				var cutscene = GetComponent( "Cutscene" ) as UIControl;
				if( cutscene != null )
				{
					cutscene.ColorMultiplier = new ColorValue( 1, 1, 1, gameMode.CutsceneGuiFadingFactor );
					cutscene.Enabled = gameMode.CutsceneGuiFadingFactor > 0;

					var textControl = cutscene.Components[ "Bottom\\Text" ] as UIText;
					if( textControl != null )
						textControl.Text = gameMode.CutsceneText;
				}
			}

			//screen fading
			if( EngineApp.IsSimulation && gameMode != null )
			{
				var screenFadingCurrentColor = gameMode.ScreenFadingCurrentColor;

				for( int n = 0; n < 4; n++ )
				{
					var current = gameMode.ScreenFadingCurrentColor[ n ];
					var target = gameMode.ScreenFadingTargetColor[ n ];

					if( current != target )
					{
						if( current < target )
						{
							current += (float)gameMode.ScreenFadingSpeed * delta;
							if( current > target )
								current = target;
						}
						else
						{
							current -= (float)gameMode.ScreenFadingSpeed * delta;
							if( current < target )
								current = target;
						}

						screenFadingCurrentColor[ n ] = current;
					}
				}

				gameMode.ScreenFadingCurrentColor = screenFadingCurrentColor;
			}

			if( EngineApp.IsSimulation )
				UpdateCursorVisibility();

			if( !firstRender )
				fadeInTimer += delta;
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//Game mode
			gameMode?.PerformRenderUI( renderer );

			//screen fading
			if( gameMode != null && gameMode.ScreenFadingCurrentColor.Alpha > 0 )
				renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), gameMode.ScreenFadingCurrentColor );
		}

		double GetFadeInAlpha()
		{
			var curve = new CurveLine();
			curve.AddPoint( 0, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 0.5, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 1.5, new Vector3( 0, 0, 0 ) );

			var value = curve.CalculateValueByTime( fadeInTimer );
			return MathEx.Saturate( value.X );
		}

		protected override void OnAfterRenderUIWithChildren( CanvasRenderer renderer )
		{
			base.OnAfterRenderUIWithChildren( renderer );

			//fade in at start
			if( EngineApp.IsSimulation && EngineApp.RenderVideoToFileData == null )
			{
				var alpha = GetFadeInAlpha();
				if( alpha != 0 )
					renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), null, new ColorValue( 0, 0, 0, alpha ) );
			}

			firstRender = false;
		}

		public void SetScene( Scene scene, bool canChangeUIControl )
		{
			this.scene = scene;

			//finish scene initialization
#if !CLIENT
			SimulationAppServer.SetScene( this.scene );
#endif
			scene.Enabled = true;

			//post scene initialization

			sceneViewport = ParentContainer.Viewport;
			scene.ViewportUpdateBegin += Scene_ViewportUpdateBegin;
			scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			//scene.RenderEvent += Scene_RenderEvent;
			sceneViewport.AttachedScene = scene;
			sceneViewport.NotifyInstantCameraMovement();

			//init GameMode
			gameMode = scene.GetComponent<GameMode>( onlyEnabledInHierarchy: true );

			// Load UI screen of the scene.
			if( canChangeUIControl )
			{
				var uiScreen = scene.UIScreen.Value;
				if( uiScreen != null )
				{
					var fileName = uiScreen.HierarchyController?.CreatedByResource?.Owner.Name;
					if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
					{
						uiControl = ResourceManager.LoadSeparateInstance<UIControl>( fileName, false, null );
						if( uiControl != null )
							AddComponent( uiControl );
					}
				}
				else
				{
					var fileName = @"Base\UI\Screens\BasicSceneScreen.ui";
					if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
					{
						uiControl = ResourceManager.LoadSeparateInstance<UIControl>( fileName, false, null );
						if( uiControl != null )
							AddComponent( uiControl );
					}
					//uiControl = CreateComponent<BasicSceneScreen>();
				}
			}
		}

		bool LoadScene( bool canChangeUIControl )
		{
			DestroyScene();

			Log.InvisibleInfo( $"Loading scene \'{PlayFileName}\'." );

			scene = ResourceManager.LoadSeparateInstance<Scene>( PlayFileName, true, false );//, out var error );
			if( scene == null )
				return false;

			//if( !string.IsNullOrEmpty( error ) )
			//{
			//	Log.Error( error );
			//	return;
			//}

			Log.InvisibleInfo( "Scene loaded successfully." );

			GC.Collect();
			GC.WaitForPendingFinalizers();

			SetScene( scene, canChangeUIControl );

			return true;
		}

		private void Scene_ViewportUpdateBegin( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			SimulationApp.UpdateSceneAntialiasingByAppSettings( scene );
			SimulationApp.UpdateSceneResolutionUpscaleByAppSettings( scene );
			SimulationApp.UpdateSceneSharpnessByAppSettings( scene );
		}

		private void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			// Get default camera or the camera from the editor.
			Camera camera = scene.CameraDefault;
			if( camera == null )
				camera = scene.Mode.Value == Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

			// Get camera settings by game mode.
			if( gameMode != null )
			{
				var cameraSettings = gameMode.GetCameraSettings( viewport, camera );
				if( cameraSettings != null )
				{
					viewport.CameraSettings = cameraSettings;
					processed = true;
					return;
				}
			}

			// Create new camera:
			//camera = (Camera)camera.Clone();
			////camera = new Camera();
			//camera.Transform = new Transform( cameraPosition, Quaternion.LookAt( ( lookTo - cameraPosition ).GetNormalize(), up ) );
			//camera.FixedUp = up;

			if( camera != null )
			{
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );
				processed = true;
			}
		}

		//private void Scene_RenderEvent( Scene sender, Viewport viewport )
		//{
		//	// Game mode.
		//	gameMode?.PerformRender( viewport );
		//}

		public void DestroyScene()
		{
			if( sceneViewport != null )
			{
				if( sceneViewport.AttachedScene == scene )
					sceneViewport.AttachedScene = null;

				scene.ViewportUpdateBegin -= Scene_ViewportUpdateBegin;
				scene.ViewportUpdateGetCameraSettings -= Scene_ViewportUpdateGetCameraSettings;
				//scene.RenderEvent -= Scene_RenderEvent;
				sceneViewport = null;
			}
			if( scene != null )
			{
#if !CLIENT
				SimulationAppServer.ResetScene();
#endif

				scene.Dispose();
				scene = null;

				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			//unload GPU resources. disable it when need faster switching between scenes
			GpuTexture.UnloadAllUnloadable();
			GpuBufferManager.DestroyNativeObjects();
		}

		bool LoadUIControl()
		{
			DestroyUIControl();

			Log.InvisibleInfo( $"Loading UI control {PlayFileName}." );

			uiControl = ResourceManager.LoadSeparateInstance<UIControl>( PlayFileName, false, null );
			if( uiControl == null )
				return false;

			AddComponent( uiControl );

			return true;
		}

		public void DestroyUIControl()
		{
			if( uiControl != null && uiControl.Parent == this )
			{
				RemoveComponent( uiControl, false );
				uiControl = null;
			}
		}

		public bool Load( string fileName, bool canChangeUIControl )
		{
			DestroyLoadedObject( canChangeUIControl );

			playFileName = PathUtility.NormalizePath( fileName );

			SoundWorld.SetListenerReset();

			if( !string.IsNullOrEmpty( playFileName ) && VirtualFile.Exists( playFileName ) )
			{
				string extension = Path.GetExtension( PlayFileName ).Replace( ".", "" ).ToLower();
				if( extension == "scene" )
					return LoadScene( canChangeUIControl );
				if( extension == "ui" )
					return LoadUIControl();
			}

			return false;
		}

		public bool NetworkClientSetScene( Scene scene, bool canChangeUIControl )
		{
			DestroyLoadedObject( canChangeUIControl );

			playFileName = "";

			SoundWorld.SetListenerReset();

			DestroyScene();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			SetScene( scene, canChangeUIControl );

			return true;
		}

		public void DestroyLoadedObject( bool canChangeUIControl = true )
		{
			if( canChangeUIControl )
				DestroyUIControl();
			DestroyScene();
			playFileName = "";
		}

		void OpenOrCloseMenu()
		{
			if( menuWindow != null && menuWindow.Disposed )
				menuWindow = null;

			if( menuWindow == null )
			{
				// If any UIWindow is opened, then can't open MenuWindow.
				if( ParentRoot.GetComponent<UIWindow>( true, true ) == null )
				{
					menuWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\MenuWindow.ui", false, true );
					if( menuWindow != null )
						AddComponent( menuWindow );
				}
			}
			else
			{
				menuWindow.Dispose();
				menuWindow = null;
			}
		}

		public delegate void InputEnabledEventDelegate( PlayScreen sender, ref bool enabled );
		public event InputEnabledEventDelegate InputEnabledEvent;

		[Browsable( false )]
		public bool InputEnabled
		{
			get
			{
				//disable when console
				if( EngineConsole.Active )
					return false;

				//disable when window on top
				if( GetComponent<UIWindow>( checkChildren: true, onlyEnabledInHierarchy: true ) != null )
					return false;

				//disable when cutscene
				if( gameMode != null && gameMode.CutsceneStarted )
					return false;

				//disable when continuous interaction (dialogue)
				var basicSceneScreen = GetComponent<BasicSceneScreen>( checkChildren: true, onlyEnabledInHierarchy: true );
				if( basicSceneScreen != null )
				{
					if( basicSceneScreen.IsContinuousInteractionEnabled() )
						return false;
					//var widget = basicSceneScreen.GetContinuousInteractionWidget();
					//if( widget != null && widget.Enabled )
					//	return false;
				}

				//custom
				var enabled = true;
				InputEnabledEvent?.Invoke( this, ref enabled );
				if( !enabled )
					return false;

				return true;
			}
		}

		public void ButtonMenu_Click( NeoAxis.UIButton sender )
		{
			OpenOrCloseMenu();
		}

		void UpdateCursorVisibility()
		{
			var show = true;

			if( gameMode != null && ( gameMode.CutsceneStarted || gameMode.CutsceneGuiFadingFactor > 0 ) )
				show = false;

			if( EngineConsole.Active )
				show = true;
			if( GetComponent<UIWindow>( checkChildren: true, onlyEnabledInHierarchy: true ) != null )
				show = true;

			EngineApp.ShowCursor = show;
		}
	}
}