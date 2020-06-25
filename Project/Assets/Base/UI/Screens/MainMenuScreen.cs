using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class MainMenuScreen : UIControl
	{
		static MainMenuScreen instance;

		bool currentDisplayBackgroundSceneOption;
		Component_Scene scene;
		Viewport sceneViewport;

		bool firstRender = true;
		double fadeInTimer;

		UIWindow scenesWindow;
		UIWindow optionsWindow;

		///////////////////////////////////////////

		[DefaultValue( null )]
		[Serialize]
		public Reference<ReferenceValueType_Resource> BackgroundScene
		{
			get { if( _backgroundScene.BeginGet() ) BackgroundScene = _backgroundScene.Get( this ); return _backgroundScene.value; }
			set { if( _backgroundScene.BeginSet( ref value ) ) { try { BackgroundSceneChanged?.Invoke( this ); } finally { _backgroundScene.EndSet(); } } }
		}
		public event Action<MainMenuScreen> BackgroundSceneChanged;
		ReferenceField<ReferenceValueType_Resource> _backgroundScene;

		///////////////////////////////////////////

		public static MainMenuScreen Instance
		{
			get { return instance; }
		}

		protected override void OnEnabledInSimulation()
		{
			instance = this;

			if( Components[ "Button Scenes" ] != null )
				( (UIButton)Components[ "Button Scenes" ] ).Click += ButtonScenes_Click;
			if( Components[ "Button Options" ] != null )
				( (UIButton)Components[ "Button Options" ] ).Click += ButtonOptions_Click;
			if( Components[ "Button Exit" ] != null )
				( (UIButton)Components[ "Button Exit" ] ).Click += ButtonExit_Click;

			//play buttons
			if( Components[ "Button Play Nature Demo" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Nature Demo" ];
				var fileName = @"Samples\Nature Demo\Scenes\Nature Demo.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.Visible = VirtualFile.Exists( fileName );
			}
			if( Components[ "Button Play Simple Game" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Simple Game" ];
				var fileName = @"Samples\Simple Game\SimpleGameLevel1.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.Visible = VirtualFile.Exists( fileName );
			}
			if( Components[ "Button Play Character Scene" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Character Scene" ];
				var fileName = @"Samples\Starter Content\Scenes\Character.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.Visible = VirtualFile.Exists( fileName );
			}
			if( Components[ "Button Play Spaceship 2D" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Spaceship 2D" ];
				var fileName = @"Samples\Starter Content\Scenes\Spaceship control 2D.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.Visible = VirtualFile.Exists( fileName );
			}
			if( Components[ "Button Play Character 2D" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Character 2D" ];
				var fileName = @"Samples\Starter Content\Scenes\Character 2D.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.Visible = VirtualFile.Exists( fileName );
			}

			// Update sound listener.
			SoundWorld.SetListenerReset();

			// Load background scene.
			currentDisplayBackgroundSceneOption = SimulationApp.DisplayBackgroundScene;
			if( currentDisplayBackgroundSceneOption && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				var fileName = BackgroundScene.GetByReference;
				if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
					LoadScene( fileName );
				else
					LoadScene( "" );
			}
			else
				LoadScene( "" );
		}

		protected override void OnDisabledInSimulation()
		{
			DestroyScene();

			if( instance == this )
				instance = null;
		}

		void ButtonScenes_Click( UIButton sender )
		{
			if( scenesWindow != null && scenesWindow.Disposed )
				scenesWindow = null;

			if( scenesWindow == null )
			{
				scenesWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\ScenesWindow.ui", false, true );
				if( scenesWindow != null )
					AddComponent( scenesWindow );
			}
			else
			{
				scenesWindow.Dispose();
				scenesWindow = null;
			}
		}

		void ButtonOptions_Click( UIButton sender )
		{
			if( optionsWindow != null && optionsWindow.Disposed )
				optionsWindow = null;

			if( optionsWindow == null )
			{
				optionsWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\OptionsWindow.ui", false, true );
				if( optionsWindow != null )
					AddComponent( optionsWindow );
			}
			else
			{
				optionsWindow.Dispose();
				optionsWindow = null;
			}
		}

		void ButtonExit_Click( UIButton sender )
		{
			EngineApp.NeedExit = true;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			// Update background scene.
			if( currentDisplayBackgroundSceneOption != SimulationApp.DisplayBackgroundScene )
			{
				currentDisplayBackgroundSceneOption = SimulationApp.DisplayBackgroundScene;

				if( currentDisplayBackgroundSceneOption && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
				{
					var fileName = BackgroundScene.GetByReference;
					if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
						LoadScene( fileName );
					else
						LoadScene( "" );
				}
				else
					LoadScene( "" );
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

			if( !firstRender )
				fadeInTimer += delta;
		}

		double GetFadeInAlpha()
		{
			var curve = new CurveLine();
			curve.AddPoint( 0, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 1.0, new Vector3( 1, 0, 0 ) );
			curve.AddPoint( 1.0 + 1.0, new Vector3( 0, 0, 0 ) );

			var value = curve.CalculateValueByTime( fadeInTimer );
			return MathEx.Saturate( value.X );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );
		}

		protected override void OnAfterRenderUIWithChildren( CanvasRenderer renderer )
		{
			base.OnAfterRenderUIWithChildren( renderer );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				//fade in
				var alpha = GetFadeInAlpha();
				if( alpha != 0 )
					renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), null, new ColorValue( 0, 0, 0, alpha ) );
			}

			firstRender = false;
		}

		public void LoadScene( string fileName )
		{
			DestroyScene();

			if( string.IsNullOrEmpty( fileName ) )
			{
				scene = ComponentUtility.CreateComponent<Component_Scene>( null, true, true );
				scene.BackgroundColor = new ColorValue( 0.4, 0.4, 0.4 );
			}
			else
				scene = ResourceManager.LoadSeparateInstance<Component_Scene>( fileName, true, null );
			if( scene == null )
				return;

			sceneViewport = ParentContainer.Viewport;
			scene.ViewportUpdateBegin += Scene_ViewportUpdateBegin;
			scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			sceneViewport.AttachedScene = scene;
		}

		private void Scene_ViewportUpdateBegin( Component_Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			ProjectUtility.UpdateSceneAntialiasingByAppSettings( scene );
		}

		private void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			Component_Camera camera = scene.CameraDefault;
			if( camera == null )
				camera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

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
			else
			{
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, 0.1, 1000, Vector3.Zero, Vector3.XAxis, Vector3.ZAxis, ProjectionType.Perspective, 1, 1, 1 );
				processed = true;
			}
		}

		/// <summary>
		/// Destroys background scene.
		/// </summary>
		public void DestroyScene()
		{
			if( sceneViewport != null )
			{
				scene.ViewportUpdateGetCameraSettings -= Scene_ViewportUpdateGetCameraSettings;
				sceneViewport = null;
			}
			if( scene != null )
			{
				scene.Dispose();
				scene = null;
			}
		}

		private void ButtonPlay_Click( UIButton sender )
		{
			var playFile = (string)sender.AnyData;
			SimulationApp.PlayFile( playFile );
		}

	}
}