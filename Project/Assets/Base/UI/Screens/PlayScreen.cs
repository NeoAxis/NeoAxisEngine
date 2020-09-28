using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using NeoAxis;
using NeoAxis.Input;

namespace Project
{
	public class PlayScreen : UIControl
	{
		static PlayScreen instance;

		string playFileName = "";

		//load scene
		Component_Scene scene;
		Viewport sceneViewport;
		Component_GameMode gameMode;
		bool gameModeLastSentInputEnabled;
		bool gameModeLastSentMouseRelativeMode;

		//load UI control
		UIControl uiControl;

		UIWindow menuWindow;

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
		public Component_Scene Scene
		{
			get { return scene; }
		}

		[Browsable( false )]
		public Component_GameMode GameMode
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
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				instance = this;

				if( !SystemSettings.MobileDevice )
				{
					var button = GetComponent( "Button Menu" );
					if( button != null )
						button.Enabled = false;
				}
			}

			base.OnAddedToParent();
		}

		protected override void OnRemovedFromParent( NeoAxis.Component oldParent )
		{
			DestroyLoadedObject();

			base.OnRemovedFromParent( oldParent );

			if( instance == this )
				instance = null;
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
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageKeyDown( e.Key ) ) )
				return true;

			return base.OnKeyDown( e );
		}

		protected override bool OnKeyPress( KeyPressEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageKeyPress( e.KeyChar ) ) )
				return true;

			return base.OnKeyPress( e );
		}

		protected override bool OnKeyUp( KeyEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageKeyUp( e.Key ) ) )
				return true;

			return base.OnKeyUp( e );
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageMouseButtonDown( button ) ) )
				return true;

			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageMouseButtonUp( button ) ) )
				return true;

			return base.OnMouseUp( button );
		}

		protected override bool OnMouseDoubleClick( EMouseButtons button )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageMouseDoubleClick( button ) ) )
				return true;

			return base.OnMouseDoubleClick( button );
		}

		protected override void OnMouseMove( Vector2 mouse )
		{
			base.OnMouseMove( mouse );

			//Game mode
			gameMode?.ProcessInputMessage( this, new InputMessageMouseMove( mouse ) );
		}

		protected override bool OnMouseWheel( int delta )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageMouseWheel( delta ) ) )
				return true;

			return base.OnMouseWheel( delta );
		}

		protected override bool OnJoystickEvent( JoystickInputEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageJoystick( e ) ) )
				return true;

			return base.OnJoystickEvent( e );
		}

		protected override bool OnTouch( TouchData e )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageTouch( e ) ) )
				return true;

			return base.OnTouch( e );
		}

		protected override bool OnSpecialInputDeviceEvent( InputEvent e )
		{
			//Game mode
			if( gameMode != null && gameMode.ProcessInputMessage( this, new InputMessageSpecialInputDevice( e ) ) )
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

				//send input enabled changed
				if( gameMode != null && gameModeLastSentInputEnabled != inputEnabled )
				{
					gameModeLastSentInputEnabled = inputEnabled;
					gameMode.ProcessInputMessage( this, new InputMessageInputEnabledChanged( inputEnabled ) );
				}

				//update mouse relative mode
				{
					if( gameMode != null && inputEnabled )
						sceneViewport.MouseRelativeMode = gameMode.IsNeedMouseRelativeMode();
					else
						sceneViewport.MouseRelativeMode = false;

					//send message
					if( gameMode != null && gameModeLastSentMouseRelativeMode != sceneViewport.MouseRelativeMode )
					{
						gameModeLastSentMouseRelativeMode = sceneViewport.MouseRelativeMode;
						gameMode.ProcessInputMessage( this, new InputMessageMouseRelativeModeChanged( gameModeLastSentMouseRelativeMode ) );
					}
				}
			}

			// Update sound listener.
			if( scene != null && sceneViewport != null )
			{
				var settings = sceneViewport.CameraSettings;
				SoundWorld.SetListener( scene, settings.Position, Vector3.Zero, settings.Rotation );
			}
			else
				SoundWorld.SetListenerReset();

			// Scene simulation.
			if( scene != null && scene.HierarchyController != null )
				scene.HierarchyController.PerformSimulationSteps();
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//Game mode
			gameMode?.PerformRenderUI( this, renderer );
		}

		public void SetScene( Component_Scene scene, bool canChangeUIControl )
		{
			this.scene = scene;

			sceneViewport = ParentContainer.Viewport;
			scene.ViewportUpdateBegin += Scene_ViewportUpdateBegin;
			scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			scene.RenderEvent += Scene_RenderEvent;
			sceneViewport.AttachedScene = scene;

			//init GameMode
			gameMode = scene.GetComponent<Component_GameMode>( onlyEnabledInHierarchy: true );

			// Load UI screen of the scene.
			if( canChangeUIControl )
			{
				var uiScreen = scene.UIScreen.Value;
				if( uiScreen != null )
				{
					//if( uiScreen.ParentRoot != scene.ParentRoot )
					//{
					var fileName = uiScreen.HierarchyController?.CreatedByResource?.Owner.Name;
					if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
					{
						uiControl = ResourceManager.LoadSeparateInstance<UIControl>( fileName, false, null );
						if( uiControl != null )
							AddComponent( uiControl );
					}
					//}
					//else
					//{
					//	//!!!!impl?
					//}
				}
			}
		}

		bool LoadScene( bool canChangeUIControl )
		{
			DestroyScene();

			scene = ResourceManager.LoadSeparateInstance<Component_Scene>( PlayFileName, true, null );//, out var error );
			if( scene == null )
				return false;
			//if( !string.IsNullOrEmpty( error ) )
			//{
			//	Log.Error( error );
			//	return;
			//}

			SetScene( scene, canChangeUIControl );

			return true;
		}

		private void Scene_ViewportUpdateBegin( Component_Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			ProjectUtility.UpdateSceneAntialiasingByAppSettings( scene );
		}

		private void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			// Get default camera or the camera from the editor.
			Component_Camera camera = scene.CameraDefault;
			if( camera == null )
				camera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

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
			//camera = (Component_Camera)camera.Clone();
			////camera = new Component_Camera();
			//camera.Transform = new Transform( cameraPosition, Quaternion.LookAt( ( lookTo - cameraPosition ).GetNormalize(), up ) );
			//camera.FixedUp = up;

			if( camera != null )
			{
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );
				processed = true;
			}
		}

		private void Scene_RenderEvent( Component_Scene sender, Viewport viewport )
		{
			// Game mode.
			gameMode?.PerformRender( this, viewport );
		}

		public void DestroyScene()
		{
			if( sceneViewport != null )
			{
				scene.ViewportUpdateGetCameraSettings -= Scene_ViewportUpdateGetCameraSettings;
				scene.RenderEvent -= Scene_RenderEvent;
				sceneViewport = null;
			}
			if( scene != null )
			{
				scene.Dispose();
				scene = null;
			}
		}

		bool LoadUIControl()
		{
			DestroyUIControl();

			uiControl = ResourceManager.LoadSeparateInstance<UIControl>( PlayFileName, false, null );
			if( uiControl == null )
				return false;

			AddComponent( uiControl );

			return true;
		}

		public void DestroyUIControl()
		{
			if( uiControl != null )
			{
				RemoveComponent( uiControl, false );
				uiControl = null;
			}
		}

		public bool Load( string fileName, bool canChangeUIControl )
		{
			DestroyLoadedObject( canChangeUIControl );

			playFileName = fileName;

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

		[Browsable( false )]
		public bool InputEnabled
		{
			get
			{
				if( EngineConsole.Active )
					return false;
				if( GetComponent<UIWindow>( checkChildren: true, onlyEnabledInHierarchy: true ) != null )
					return false;
				return true;
				//return !EngineConsole.Active && menuWindow == null;
			}
		}

		public void ButtonMenu_Click( NeoAxis.UIButton sender )
		{
			OpenOrCloseMenu();
		}
	}
}