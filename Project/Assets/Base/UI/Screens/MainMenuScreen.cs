// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	public class MainMenuScreen : UIControl
	{
		static MainMenuScreen instance;

		bool currentDisplayBackgroundSceneOption;
		Scene scene;
		Viewport sceneViewport;

		bool firstRender = true;
		double fadeInTimer;

		UIWindow scenesWindow;
		UIWindow optionsWindow;
		UIWindow multiplayerCreateWindow;
		UIWindow multiplayerJoinWindow;

		///////////////////////////////////////////

		public delegate void EnabledInSimulationStaticDelegate( MainMenuScreen sender );
		/// <summary>
		/// Static event may be used to change the main menu without changing the code.
		/// </summary>
		public static event EnabledInSimulationStaticDelegate EnabledInSimulationStatic;

		///////////////////////////////////////////

		[DefaultValue( null )]
		[Serialize]
		public Reference<ReferenceValueType_Resource> BackgroundScene
		{
			get { if( _backgroundScene.BeginGet() ) BackgroundScene = _backgroundScene.Get( this ); return _backgroundScene.value; }
			set { if( _backgroundScene.BeginSet( this, ref value ) ) { try { BackgroundSceneChanged?.Invoke( this ); } finally { _backgroundScene.EndSet(); } } }
		}
		public event Action<MainMenuScreen> BackgroundSceneChanged;
		ReferenceField<ReferenceValueType_Resource> _backgroundScene;

		[DefaultValue( null )]
		[Serialize]
		public Reference<ReferenceValueType_Resource> BackgroundSceneLimitedDevice
		{
			get { if( _backgroundSceneLimitedDevice.BeginGet() ) BackgroundSceneLimitedDevice = _backgroundSceneLimitedDevice.Get( this ); return _backgroundSceneLimitedDevice.value; }
			set { if( _backgroundSceneLimitedDevice.BeginSet( this, ref value ) ) { try { BackgroundSceneLimitedDeviceChanged?.Invoke( this ); } finally { _backgroundSceneLimitedDevice.EndSet(); } } }
		}
		public event Action<MainMenuScreen> BackgroundSceneLimitedDeviceChanged;
		ReferenceField<ReferenceValueType_Resource> _backgroundSceneLimitedDevice;

		///////////////////////////////////////////

		[Browsable( false )]
		public UIButton ButtonPlayInCloud { get { return GetComponent<UIButton>( "Button Play In Cloud" ); } }
		[Browsable( false )]
		public UIButton ButtonPlaySingle { get { return GetComponent<UIButton>( "Button Play Single" ); } }
		[Browsable( false )]
		public UIButton ButtonScenes { get { return GetComponent<UIButton>( "Button Scenes" ); } }
		[Browsable( false )]
		public UIButton ButtonOptions { get { return GetComponent<UIButton>( "Button Options" ); } }
		[Browsable( false )]
		public UIButton ButtonExit { get { return GetComponent<UIButton>( "Button Exit" ); } }
		[Browsable( false )]
		public UIButton ButtonMultiplayerCreate { get { return GetComponent<UIButton>( "Button Multiplayer Create" ); } }
		[Browsable( false )]
		public UIButton ButtonMultiplayerJoin { get { return GetComponent<UIButton>( "Button Multiplayer Join" ); } }

		///////////////////////////////////////////

		public static MainMenuScreen Instance
		{
			get { return instance; }
		}

		protected override void OnEnabledInSimulation()
		{
			instance = this;

			base.OnEnabledInSimulation();

			EnabledInSimulationStatic?.Invoke( this );

			//Scenes button
			if( ButtonScenes != null )
			{
				ButtonScenes.Click += ButtonScenes_Click;
				ButtonScenes.ReadOnly = SimulationAppClient.Created;
			}

			//Options button
			if( ButtonOptions != null )
				ButtonOptions.Click += ButtonOptions_Click;

			//Exit button
			if( ButtonExit != null )
			{
				ButtonExit.Click += ButtonExit_Click;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
					ButtonExit.Enabled = false;
			}

			//play buttons
			if( Components[ "Button Play City Demo" ] != null )
			{
				var button = (UIButton)Components[ "Button Play City Demo" ];
				var fileName = @"Samples\City Demo\City Demo.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Nature Demo" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Nature Demo" ];
				var fileName = @"Samples\Nature Demo\Nature Demo.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play SciFi Demo" ] != null )
			{
				var button = (UIButton)Components[ "Button Play SciFi Demo" ];
				var fileName = @"Samples\Sci-fi Demo\Scenes\Sci-fi Demo.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Battle Demo" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Battle Demo" ];
				var fileName = @"Samples\Battle Demo\Battle Demo.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Shooter Game" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Shooter Game" ];
				var fileName = @"Samples\Shooter\Scenes\Shooter.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Simple Game" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Simple Game" ];
				var fileName = @"Samples\Simple Game\SimpleGameLevel1.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Spaceship Game" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Spaceship Game" ];
				var fileName = @"Samples\Spaceship Game\Spaceship Game.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}
			if( Components[ "Button Play Platform Game" ] != null )
			{
				var button = (UIButton)Components[ "Button Play Platform Game" ];
				var fileName = @"Samples\Platform Game\Platform Game.scene";
				button.AnyData = fileName;
				button.Click += ButtonPlay_Click;
				if( button.Visible )
					button.ReadOnly = !VirtualFile.Exists( fileName ) || SimulationAppClient.Created;
			}

			// Update sound listener.
			SoundWorld.SetListenerReset();

			// Load background scene.
			currentDisplayBackgroundSceneOption = SimulationApp.DisplayBackgroundScene;
			if( currentDisplayBackgroundSceneOption && EngineApp.IsSimulation )
			{
				var fileName = SystemSettings.LimitedDevice ? BackgroundSceneLimitedDevice.GetByReference : BackgroundScene.GetByReference;
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

			base.OnDisabledInSimulation();

			if( instance == this )
				instance = null;
		}

		public void ToggleScenesWindow()
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

		void ButtonScenes_Click( UIButton sender )
		{
			ToggleScenesWindow();
		}

		public void ToggleOptionsWindow()
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

		void ButtonOptions_Click( UIButton sender )
		{
			ToggleOptionsWindow();
		}

		void ButtonExit_Click( UIButton sender )
		{
			EngineApp.NeedExit = true;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				// Update background scene.
				if( currentDisplayBackgroundSceneOption != SimulationApp.DisplayBackgroundScene )
				{
					currentDisplayBackgroundSceneOption = SimulationApp.DisplayBackgroundScene;

					if( currentDisplayBackgroundSceneOption && EngineApp.IsSimulation )
					{
						var fileName = SystemSettings.LimitedDevice ? BackgroundSceneLimitedDevice.GetByReference : BackgroundScene.GetByReference;
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
				if( SimulationApp.SceneSimulate )
					scene?.HierarchyController?.PerformSimulationSteps();
				ParentRoot.HierarchyController?.PerformSimulationSteps();

				if( !firstRender )
					fadeInTimer += delta;

				if( ButtonMultiplayerCreate != null )
				{
					ButtonMultiplayerCreate.Highlighted = RunServer.Running;
					ButtonMultiplayerCreate.ReadOnly = SystemSettings.CurrentPlatform != SystemSettings.Platform.Windows;
				}

				//if( GetButtonMultiplayerJoin() != null )
				//	GetButtonMultiplayerJoin().ReadOnly = SystemSettings.CurrentPlatform != SystemSettings.Platform.Windows && SystemSettings.CurrentPlatform != SystemSettings.Platform.Android;
			}
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

			//fade in
			if( EngineApp.IsSimulation )
			{
				var alpha = GetFadeInAlpha();
				if( alpha != 0 )
					renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), null, new ColorValue( 0, 0, 0, alpha ) );
			}

			firstRender = false;
		}

		public void LoadScene( string fileName )
		{
			DestroyScene();

			if( SimulationAppClient.Created )
				return;

			if( !string.IsNullOrEmpty( fileName ) )
				scene = ResourceManager.LoadSeparateInstance<Scene>( fileName, true, null );

			if( scene == null )
			{
				scene = ComponentUtility.CreateComponent<Scene>( null, true, true );
				scene.BackgroundColor = new ColorValue( 0.4, 0.4, 0.4 );
			}

			sceneViewport = ParentContainer.Viewport;
			scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;
			sceneViewport.AttachedScene = scene;
			sceneViewport.NotifyInstantCameraMovement();

			GC.Collect( 2, GCCollectionMode.Forced, true );
		}

		private void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			Camera camera = scene.CameraDefault;
			if( camera == null )
				camera = scene.Mode.Value == Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

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
				if( sceneViewport.AttachedScene == scene )
					sceneViewport.AttachedScene = null;

				scene.ViewportUpdateGetCameraSettings -= Scene_ViewportUpdateGetCameraSettings;
				sceneViewport = null;
			}
			if( scene != null )
			{
				scene.Dispose();
				scene = null;

				GC.Collect( 2, GCCollectionMode.Forced, true );
			}
		}

		private void ButtonPlay_Click( UIButton sender )
		{
			var playFile = (string)sender.AnyData;
			SimulationApp.PlayFile( playFile );
		}

		public void ButtonMultiplayerCreate_Click( NeoAxis.UIButton sender )
		{
			if( multiplayerCreateWindow != null && multiplayerCreateWindow.Disposed )
				multiplayerCreateWindow = null;

			if( multiplayerCreateWindow == null )
			{
				multiplayerCreateWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\MultiplayerCreateWindow.ui", false, true );
				if( multiplayerCreateWindow != null )
					AddComponent( multiplayerCreateWindow );
			}
			else
			{
				multiplayerCreateWindow.Dispose();
				multiplayerCreateWindow = null;
			}
		}

		public void ButtonMultiplayerJoin_Click( NeoAxis.UIButton sender )
		{
			if( multiplayerJoinWindow != null && multiplayerJoinWindow.Disposed )
				multiplayerJoinWindow = null;

			if( multiplayerJoinWindow == null )
			{
				multiplayerJoinWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\MultiplayerJoinWindow.ui", false, true );
				if( multiplayerJoinWindow != null )
					AddComponent( multiplayerJoinWindow );
			}
			else
			{
				multiplayerJoinWindow.Dispose();
				multiplayerJoinWindow = null;
			}
		}
	}
}