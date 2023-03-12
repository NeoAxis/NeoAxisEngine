// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class ShooterSceneScreen : BasicSceneScreen
	{
		ShooterInGameContextScreen shooterInGameContextScreen;

		//player settings
		//[EngineConfig( "ShooterSceneScreen", "seePeopleNotInGame" )]
		//public static bool seePeopleNotInGame = true;

		//

		static ShooterSceneScreen()
		{
			EngineConfig.RegisterClassParameters( typeof( ShooterSceneScreen ) );
		}

		public static new ShooterSceneScreen GetInstance()
		{
			return PlayScreen.Instance?.UIControl as ShooterSceneScreen;
		}

		public new ShooterNetworkLogic NetworkLogic
		{
			get { return base.NetworkLogic as ShooterNetworkLogic; }
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( Scene != null && Scene.NetworkIsClient )
			{
				//UpdateCurrentGame();
				//UpdateCharacterLookDirection();

				////update controls
				//if( currentGameSpecificScreen != null )
				//{
				//	var menuWindow = ParentContainer.GetComponent<MenuWindow>( true );
				//	currentGameSpecificScreen.ReadOnly = menuWindow != null;
				//	currentGameCommonScreen.ReadOnly = menuWindow != null;
				//}

				////update camera
				//if( GameMode != null )
				//	GameMode.UseBuiltInCamera = /*tpsCameraWhenNotInGame ? GameMode.BuiltInCameraEnum.ThirdPerson : */GameMode.BuiltInCameraEnum.FirstPerson;
			}
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//draw message in the bottom of the screen
			if( Scene != null && Scene.NetworkIsClient )
			{
				var text = "";
				var color = new ColorValue( 0.1, 0.9, 0.1 );

				if( NetworkLogic != null )
				{
					text = NetworkLogic.GetGameTextStatus();
					//if( NetworkLogic.CurrentGameStatus.Value == ShooterGameStatusEnum.Preparing )
					//	text = NetworkLogic.GameStatusText.Value;
				}

				if( !string.IsNullOrEmpty( text ) )
				{
					CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, 0.03, text, new Vector2( 0.5, 0.9 ), EHorizontalAlignment.Center, EVerticalAlignment.Center, color );
				}
			}
		}

		protected override void GameMode_GetInteractiveObjectInfoEvent( GameMode sender, InteractiveObject obj, ref InteractiveObjectObjectInfo result )
		{
			base.GameMode_GetInteractiveObjectInfoEvent( sender, obj, ref result );

			//if( scene != null && scene.NetworkIsClient && WorldClientManager.Client != null && !sender.FreeCamera )
			//{
			//	var chair = obj as Chair;
			//	if( chair != null )
			//	{
			//		if( currentGameSpecificScreen != null )
			//			result = null;
			//	}
			//}
		}

		protected override void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
			base.GameMode_GetCameraSettingsEvent( sender, viewport, cameraDefault, ref cameraSettings );
		}

		protected override void PlayScreen_InputEnabledEvent( PlayScreen sender, ref bool enabled )
		{
			base.PlayScreen_InputEnabledEvent( sender, ref enabled );

			//if( currentGameCommonScreen != null )
			//	enabled = false;
		}

		protected override void GameMode_ShowControlledObject( GameMode sender, Viewport viewport, ref bool show )
		{
			base.GameMode_ShowControlledObject( sender, viewport, ref show );

			//if( currentGameCommonScreen != null && !currentGameCommonScreen.Is2DMode() && !sender.FreeCamera )
			//	show = false;
		}

		protected override void GameMode_RenderTargetImageBefore( GameMode sender, ref bool show )
		{
			base.GameMode_RenderTargetImageBefore( sender, ref show );

			//if( currentGameCommonScreen != null )
			//	show = false;
		}

		//void UpdateCharacterLookDirection()
		//{
		//	if( gameMode != null && currentGameCommonScreen != null )
		//	{
		//		var character = gameMode.ObjectControlledByPlayer.Value as Character;
		//		if( character != null )
		//		{
		//			var inputProcessing = character.GetComponent<CharacterInputProcessing>();
		//			if( inputProcessing != null )
		//			{
		//				var gamePlace = currentGame.GamePlace.Value;
		//				if( gamePlace != null )
		//				{
		//					inputProcessing.LookDirection = SphericalDirection.FromVector( gamePlace.GetGamePosition() - character.TransformV.Position );
		//				}
		//			}
		//		}
		//	}
		//}

		protected override void InGameContextScreenCreate()
		{
			base.InGameContextScreenCreate();

			if( InGameContextScreen != null )
			{
				var fileName = @"Samples\Shooter\UI\ShooterInGameContextScreen.ui";
				if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
				{
					var screen = ResourceManager.LoadSeparateInstance<ShooterInGameContextScreen>( fileName, false, true );
					if( screen != null )
					{
						shooterInGameContextScreen = screen;
						InGameContextScreen.AddComponent( shooterInGameContextScreen );
					}
				}
			}
		}

		protected override void InGameContextScreenDestroy()
		{
			shooterInGameContextScreen?.Dispose();
			shooterInGameContextScreen = null;

			base.InGameContextScreenDestroy();
		}

	}
}