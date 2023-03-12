// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class BasicSceneScreen : NeoAxis.UIControl
	{
		Scene scene;
		GameMode gameMode;
		NetworkLogic networkLogic;

		EntranceScreen entranceScreen;
		InGameContextScreen inGameContextScreen;

		//

		static BasicSceneScreen()
		{
			EngineConfig.RegisterClassParameters( typeof( BasicSceneScreen ) );
		}

		public static BasicSceneScreen GetInstance()
		{
			return PlayScreen.Instance?.UIControl as BasicSceneScreen;
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
		public NetworkLogic NetworkLogic
		{
			get { return networkLogic; }
		}

		[Browsable( false )]
		public EntranceScreen EntranceScreen
		{
			get { return entranceScreen; }
		}

		[Browsable( false )]
		public InGameContextScreen InGameContextScreen
		{
			get { return inGameContextScreen; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EngineApp.IsSimulation && EnabledInHierarchyAndIsInstance )
			{
				scene = ClientUtility.GetScene();
				gameMode = ClientUtility.GetGameMode();
				networkLogic = ClientUtility.GetNetworkLogic();
			}

			if( EngineApp.IsSimulation )
			{
				//scene, game mode
				if( EnabledInHierarchyAndIsInstance )
				{
					if( scene != null && scene.NetworkIsClient )
					{
						if( PlayScreen.Instance != null )
							PlayScreen.Instance.InputEnabledEvent += PlayScreen_InputEnabledEvent;

						if( gameMode != null )
						{
							gameMode.GetInteractiveObjectInfoEvent += GameMode_GetInteractiveObjectInfoEvent;
							gameMode.GetCameraSettingsEvent += GameMode_GetCameraSettingsEvent;
							gameMode.ShowControlledObject += GameMode_ShowControlledObject;
							gameMode.RenderTargetImageBefore += GameMode_RenderTargetImageBefore;
						}
					}
				}
				else
				{
					if( PlayScreen.Instance != null )
						PlayScreen.Instance.InputEnabledEvent -= PlayScreen_InputEnabledEvent;

					if( gameMode != null )
					{
						gameMode.GetInteractiveObjectInfoEvent -= GameMode_GetInteractiveObjectInfoEvent;
						gameMode.GetCameraSettingsEvent -= GameMode_GetCameraSettingsEvent;
						gameMode.ShowControlledObject -= GameMode_ShowControlledObject;
						gameMode.RenderTargetImageBefore -= GameMode_RenderTargetImageBefore;
					}
				}

				//chat
				if( EnabledInHierarchyAndIsInstance )
				{
					if( SimulationAppClient.Client != null )
						SimulationAppClient.Client.Chat.ReceiveText += Chat_ReceiveText;
				}
				else
				{
					if( SimulationAppClient.Client != null )
						SimulationAppClient.Client.Chat.ReceiveText -= Chat_ReceiveText;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( scene != null && scene.NetworkIsClient )
			{
				UpdateEntranceScreen();

				//clear closed in-game screens
				if( inGameContextScreen != null && inGameContextScreen.Parent == null )
					InGameContextScreenDestroy();
			}
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );
		}

		protected virtual void GameMode_GetInteractiveObjectInfoEvent( GameMode sender, InteractiveObject obj, ref InteractiveObjectObjectInfo result )
		{
		}

		protected virtual void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
		}

		protected virtual void PlayScreen_InputEnabledEvent( PlayScreen sender, ref bool enabled )
		{
			if( inGameContextScreen != null )
				enabled = false;
		}

		protected virtual void GameMode_ShowControlledObject( GameMode sender, Viewport viewport, ref bool show )
		{
			//show = false;
		}

		protected virtual void GameMode_RenderTargetImageBefore( GameMode sender, ref bool show )
		{
			//show = false;
		}

		void UpdateEntranceScreen()
		{
			if( networkLogic != null )
			{
				if( networkLogic.EnteredToWorld )
				{
					if( entranceScreen != null )
						EntranceScreenDestroy();
				}
				else
				{
					if( entranceScreen == null )
						EntranceScreenCreate();
				}
			}
		}

		protected virtual void EntranceScreenCreate()
		{
			EntranceScreenDestroy();

			var fileName = @"Base\UI\Screens\EntranceScreen.ui";
			if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
			{
				var screen = ResourceManager.LoadSeparateInstance<EntranceScreen>( fileName, false, true );
				if( screen != null )
				{
					entranceScreen = screen;
					AddComponent( entranceScreen );
				}
			}
		}

		protected virtual void EntranceScreenDestroy()
		{
			entranceScreen?.Dispose();
			entranceScreen = null;
		}

		protected virtual void InGameContextScreenCreate()// bool focusEditMessage )
		{
			InGameContextScreenDestroy();

			//reset control keys
			PlayScreen.Instance?.ParentContainer.Viewport.KeysAndMouseButtonUpAll();

			{
				var fileName = @"Base\UI\Screens\InGameContextScreen.ui";
				if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
				{
					var screen = ResourceManager.LoadSeparateInstance<InGameContextScreen>( fileName, false, true );
					if( screen != null )
					{
						inGameContextScreen = screen;
						inGameContextScreen.ColorMultiplier = new ColorValue( 1, 1, 1, 0 );
						AddComponent( inGameContextScreen );
					}
				}
			}

			//focus edit message
			//if( focusEditMessage )
			inGameContextScreen?.GetEditMessage()?.Focus();
		}

		protected virtual void InGameContextScreenDestroy()
		{
			inGameContextScreen?.Dispose();
			inGameContextScreen = null;
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( scene != null && scene.NetworkIsClient )
			{
				if( e.Key == EKeys.F1 )
				{
					if( scene != null && entranceScreen == null )
					{
						if( inGameContextScreen == null )
							InGameContextScreenCreate();
						else
							InGameContextScreenDestroy();

						return true;
					}
				}

				if( e.Key == EKeys.Return )
				{
					if( scene != null && entranceScreen == null )
					{
						if( inGameContextScreen == null )
							InGameContextScreenCreate();

						return true;
					}
				}
			}

			return base.OnKeyDown( e );
		}

		protected virtual void Chat_ReceiveText( ClientNetworkService_Chat sender, ClientNetworkService_Users.UserInfo fromUser, string text )
		{
			var str = $"{fromUser.Username}: {text}";
			ScreenMessages.Add( str, false );
		}
	}
}