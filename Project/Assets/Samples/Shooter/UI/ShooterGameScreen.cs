// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using NeoAxis;
using NeoAxis.Cloud;

namespace Project
{
	public class ShooterGameScreen : BasicSceneScreen
	{
		//not used right now. synchronized via ShooterGameLogic. Can be used for additional info.
		//match settings
		bool matchSettingsNeedRequest = true;
		MatchSettings matchSettings;

		//match details
		volatile bool matchDetailsNeedRequest = true;
		volatile MatchDetails matchDetails;
		volatile MatchDetails matchDetailsPrevious;

		//various
		double lastOnUpdateTime;

		///////////////////////////////////////////////

		//In-game menu controls
		[Browsable( false )]
		public UIButton ButtonAddBot { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Add Bot" ); } }
		[Browsable( false )]
		public UIButton ButtonDeleteBot { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Delete Bot" ); } }

		///////////////////////////////////////////////

		public class MatchSettings
		{
			//public string SceneName = "";
			//public string/*GameTypeEnum*/ GameType = "Free For All";
			//public int MatchTimeLimit = 10; //in minutes

			//public string[] Bots = { "", "", "", "", "", "", "", "", "", "" };
			//public Dictionary<long, string> UserRoles = new Dictionary<long, string>();
		}

		///////////////////////////////////////////////

		//!!!!to basic or merge?
		public abstract class MatchDetailsBase
		{
			public List<Participant> Participants = new List<Participant>();
			public int GameCounter;

			/////////////////////

			public class Participant
			{
				public long UserID;
				public string Username = "";
				public string BotLevel = "";
				public int PlayerIndex = -1;

				//optimization
				public bool ThisUser;

				//

				public string GetDisplayName()
				{
					if( !string.IsNullOrEmpty( BotLevel ) )
						return $"Bot {BotLevel}";
					else
						return Username ?? "(No name)";
				}
			}

			/////////////////////

			public Participant GetParticipantByPlayerIndex( int playerIndex )
			{
				return Participants.FirstOrDefault( p => p.PlayerIndex == playerIndex );
			}

			public Participant GetParticipantByUserID( long userID )
			{
				return Participants.FirstOrDefault( p => p.UserID == userID );
			}
		}

		///////////////////////////////////////////////

		public class MatchDetails : MatchDetailsBase
		{
			//public float PreraceTimeDuration = 10;
			public Participant[] Players = Array.Empty<Participant>();
			public float GameTime;

			///////////////////////

			public int GetThisUserPlayerIndex()
			{
				var participant = GetParticipantByUserID( CloudServiceClient.ThisUserID );
				if( participant != null )
					return participant.PlayerIndex;
				return -1;
			}

			public bool ThisUserIsPlayer()
			{
				return GetThisUserPlayerIndex() != -1;
			}
		}

		///////////////////////////////////////////////

		public ShooterGameScreen()
		{
			EngineConfig.RegisterClassParameters( GetType() );
		}

		public new ShooterGameLogic GameLogic
		{
			get { return base.GameLogic as ShooterGameLogic; }
		}

		//public ShooterGameLogic GetShooterGameLogic()
		//{
		//	return Scene?.GetGameLogic() as ShooterGameLogic;
		//}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( Scene != null )
			{
				if( EngineApp.EngineTime - lastOnUpdateTime > 0.1 )
				{
					lastOnUpdateTime = EngineApp.EngineTime;

					//request match settings
					if( matchSettingsNeedRequest )
					{
						matchSettingsNeedRequest = false;
						Task.Run( GetMatchSettingsAsync );
					}

					//request match details
					if( matchDetailsNeedRequest )
					{
						matchDetailsNeedRequest = false;
						Task.Run( GetMatchDetailsAsync );
					}

					//update in-game menu controls
					var gameLogic = GameLogic;
					if( gameLogic != null && CloudServiceClient.Client == null )
					{
						//mutliplayer, single modes
						if( ButtonAddBot != null )
							ButtonAddBot.ReadOnly = false;
						if( ButtonDeleteBot != null )
						{
							if( gameLogic.NetworkIsClient && SimulationAppClient.Created )
							{
								//multiplayer mode
								ButtonDeleteBot.ReadOnly = gameLogic.Client_GetUsers().Count( u => u.Value.Bot ) < 1;
							}
							else if( gameLogic.NetworkIsSingle )
							{
								//single mode
								ButtonDeleteBot.ReadOnly = gameLogic.Single_GetUsers().Length < 2;
							}
						}
					}
					else
					{
						//cloud mode or no game logic component
						if( ButtonAddBot != null )
							ButtonAddBot.ReadOnly = true;
						if( ButtonDeleteBot != null )
							ButtonDeleteBot.ReadOnly = true;
					}
				}
			}
		}

		void DrawTextInRightTopCorner( CanvasRenderer renderer )
		{
			var lines = new List<(string, ColorValue)>();

			var gameLogic = GameLogic;
			if( gameLogic != null )
			{
				//get text lines
				if( MatchInfo != null && matchDetails != null )
				{
					//cloud mode

					//match name
					lines.Add( (MatchInfo.Name, new ColorValue( 0.95, 0.95, 0.95 )) );
					lines.Add( ("", new ColorValue( 1, 1, 1 )) );

					//players
					lines.Add( ("Players:", new ColorValue( 0.95, 0.95, 0.95 )) );
					for( int playerIndex = 0; ; playerIndex++ )
					{
						var participant = matchDetails.GetParticipantByPlayerIndex( playerIndex );
						if( participant != null )
						{
							//username
							var text = participant.GetDisplayName();

							//frags
							var user = gameLogic.Client_GetUser( participant.UserID );
							if( user != null )
								text += $" - {user.Frags}";

							lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
						}
						else
							break;
					}

					//spectators
					lines.Add( ("", new ColorValue( 1, 1, 1 )) );
					var addedSpectatorsTitle = false;
					foreach( var participant in matchDetails.Participants )
					{
						if( participant.PlayerIndex == -1 )
						{
							if( !addedSpectatorsTitle )
							{
								addedSpectatorsTitle = true;
								lines.Add( ("Spectators:", new ColorValue( 0.95, 0.95, 0.95 )) );
							}

							var text = participant.GetDisplayName();
							lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
						}
					}
				}
				else if( gameLogic.NetworkIsClient && SimulationAppClient.Created )
				{
					//multiplayer mode
					lines.Add( ("Players:", new ColorValue( 0.95, 0.95, 0.95 )) );
					foreach( var serverUserItem in gameLogic.Client_GetUsers() )
					{
						var text = serverUserItem.Value.Username;
						text += $" - {serverUserItem.Value.Frags}";
						lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
					}
				}
				else if( gameLogic.NetworkIsSingle )
				{
#if !CLIENT
					//single mode
					lines.Add( ("Players:", new ColorValue( 0.95, 0.95, 0.95 )) );
					foreach( var singleUserItem in gameLogic.Single_GetUsers() )
					{
						var text = singleUserItem.UserID == 0 ? "You" : $"Bot {singleUserItem.UserID}";
						text += $" - {gameLogic.Single_GetFrags( singleUserItem.UserID )}";
						lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
					}
#endif
				}
			}

			//visualize lines

			var fontHeight = renderer.DefaultFontSize;
			var position = ConvertOffset( new UIMeasureValueVector2( UIMeasure.Units, 10, 10 ), UIMeasure.Screen );
			position.X = 1 - position.X;
			if( ButtonSystemMenu != null && ButtonSystemMenu.Enabled )
				position.Y += ButtonSystemMenu.GetScreenRectangle().Bottom;
			else if( ButtonInGameMenu != null && ButtonInGameMenu.Enabled )
				position.Y += ButtonSystemMenu.GetScreenRectangle().Bottom;

			foreach( var line in lines )
			{
				var text = line.Item1;
				var color = line.Item2;

				if( !string.IsNullOrEmpty( text ) )
				{
					//!!!!bug. shadows have different distance because align. need change CanvasRendererImpl

					CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, text, position, EHorizontalAlignment.Right, EVerticalAlignment.Top, color );
				}

				position.Y += fontHeight;
			}
		}

		void DrawGameStatus( CanvasRenderer renderer )
		{
			var text = "";
			var color = new ColorValue( 0.95, 0.95, 0.95 );

			var gameLogic = GameLogic;
			if( gameLogic != null )
			{
				var remainingTimeSeconds = (int)gameLogic.GetRemainingTime();
				if( gameLogic.CurrentGameStatus.Value == ShooterGameLogic.GameStatusEnum.Preparing )
					text = $"Game starts in {remainingTimeSeconds} seconds.";
				else
					text = $"Game ends in {remainingTimeSeconds} seconds.";
			}
			else
				text = "ShooterGameLogic not found.";

			if( !string.IsNullOrEmpty( text ) )
			{
				var fontHeight = renderer.DefaultFontSize * 1.5;
				var position = new Vector2( 0.5, 0.0 + fontHeight / 2 );

				CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, text, position, EHorizontalAlignment.Center, EVerticalAlignment.Top, color );
			}
		}

		void DrawPlayerHealth( CanvasRenderer renderer )
		{
			//simple drawing health info by the text
			var gameLogic = GameLogic;
			if( GameMode != null && gameLogic != null )
			{
				var maxHealth = gameLogic.ObjectControlledByPlayerHealth.Value;

				var character = GameMode.ObjectControlledByPlayer.Value as Character;
				if( character != null && maxHealth > 0 )
				{
					var text = $"{(int)Math.Ceiling( character.Health.Value )} / {(int)maxHealth}";
					var fontHeight = renderer.DefaultFontSize;
					var position = new Vector2( fontHeight / 2 * renderer.AspectRatioInv, 1.0 - fontHeight / 2 );
					CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, text, position, EHorizontalAlignment.Left, EVerticalAlignment.Bottom, new ColorValue( 0.95, 0.95, 0.95 ) );
				}
			}
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			if( Scene != null )
			{
				DrawTextInRightTopCorner( renderer );
				DrawGameStatus( renderer );
				DrawPlayerHealth( renderer );
			}
		}

		protected override void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			base.Messages_ReceiveMessageString( sender, message, data );

			if( MatchInfo != null && message == "MatchUpdated" )
			{
				if( long.TryParse( data, out var matchID ) && MatchInfo.Id == matchID )
					matchDetailsNeedRequest = true; //by idea can get match details with MatchUpdated without additional request
			}
		}

		protected override void Messages_ReceiveMessageBinary( ClientNetworkService_Messages sender, string message, byte[] data )
		{
			base.Messages_ReceiveMessageBinary( sender, message, data );

			//var settings = matchSettings;
			//if( settings == null )
			//	return;
			//var details = matchDetails;
			//if( details == null )
			//	return;

			//if( message == "GameTime" )
			//{
			//	EngineThreading.ExecuteFromMainThreadLater( delegate ()
			//	{
			//		var previousGameTime = details.GameTime;

			//		var reader = new ArrayDataReader( data );
			//		details.GameTime = reader.ReadSingle();

			//		if( previousGameTime != 0 )
			//		{
			//			//play sound countdown 3
			//			if( previousGameTime <= details.PreraceTimeDuration - 3 && details.GameTime > details.PreraceTimeDuration - 3 )
			//				SoundPlay2D( @"Game\Sounds\Countdown.ogg" );

			//			//play sound countdown 2
			//			if( previousGameTime <= details.PreraceTimeDuration - 2 && details.GameTime > details.PreraceTimeDuration - 2 )
			//				SoundPlay2D( @"Game\Sounds\Countdown.ogg" );

			//			//play sound countdown 1
			//			if( previousGameTime <= details.PreraceTimeDuration - 1 && details.GameTime > details.PreraceTimeDuration - 1 )
			//				SoundPlay2D( @"Game\Sounds\Countdown.ogg" );

			//			//play sound go
			//			if( previousGameTime <= details.PreraceTimeDuration && details.GameTime > details.PreraceTimeDuration )
			//				SoundPlay2D( @"Game\Sounds\Go.ogg" );
			//		}

			//		return;
			//	} );
			//}
		}

		protected override void OnEnabledInSimulationAndIsInstance()
		{
			base.OnEnabledInSimulationAndIsInstance();

			//subscribe to ShooterGameLogic events
			var gameLogic = GameLogic;
			if( gameLogic != null )
				gameLogic.CurrentGameStatusChanged += GameLogic_CurrentGameStatusChanged;
		}

		private void GameLogic_CurrentGameStatusChanged( ShooterGameLogic gameLogic )
		{
			if( gameLogic.CurrentGameStatus.Value == ShooterGameLogic.GameStatusEnum.Playing )
			{
				ScreenMessages.Add( "The game has started!" );
				Scene?.SoundPlay2D( @"Samples\Shooter\Sounds\Game started.ogg" );
			}
			else
			{
				ScreenMessages.Add( "The game has ended." );
				Scene?.SoundPlay2D( @"Samples\Shooter\Sounds\Game ended.ogg" );
			}
		}

		protected override void Scene_RenderEvent( Scene scene, Viewport viewport )
		{
			base.Scene_RenderEvent( scene, viewport );

			//var settings = matchSettings;
			//if( settings == null )
			//	return;
			//var details = matchDetails;
			//if( details == null )
			//	return;

			//if( !IsOverlappedByOtherWindows() )
			//{
			//	var renderer = viewport.Simple3DRenderer;

			//	//renderer.AddRectangle( rectangle, Matrix4.Identity );

			//	//renderer.SetColor( new ColorValue( 0.1, 0.1, 0.1, 0.7 ) );
			//	//renderer.AddSphere( transform, 0.01, 32, true );
			//}
		}

		protected override void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
			base.GameMode_GetCameraSettingsEvent( sender, viewport, cameraDefault, ref cameraSettings );

			////var details = matchDetails;
			////if( details == null )
			////	return;
			////var playerIndex = details.GetThisUserPlayerIndex();

			//override default camera when player character is dead
			if( !GameMode.FreeCamera )
			{
				var character = sender.ObjectControlledByPlayer.Value as Character;
				if( character != null && character.LifeStatus.Value == Character.LifeStatusEnum.Dead )
					cameraSettings = sender.GetCameraSettingsDefaultFunction( viewport, cameraDefault, GameMode.BuiltInCameraEnum.ThirdPerson );
			}
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			//unfocus controls to prevent them from processing input and to allow the scene to receive input
			ParentContainer.FocusedControl?.Unfocus();

			//if( button == EMouseButtons.Left )
			//{
			//}

			return base.OnMouseDown( button );
		}

		async Task GetMatchSettingsAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<string>( "CloudServerImplementation", "GetMatchSettings", cts.Token, MatchInfo.Id );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					Log.Warning( "GetMatchSettingsAsync: CloudServerImplementation.GetMatchSettings error: " + result.Error );
					return;
				}

				var rootBlock = TextBlock.Parse( result.Value, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "GetMatchSettingsAsync: CloudServerImplementation.GetMatchSettings parse error: " + error );
					return;
				}

				var settings = new MatchSettings();

				//this data gets from ShooterGameLogic

				////SceneName
				//{
				//	var block = rootBlock.FindChild( "Scene Name" );
				//	if( block != null )
				//		settings.SceneName = block.GetAttribute( "CurrentValue" );
				//}

				////GameType
				//{
				//	var block = rootBlock.FindChild( "Game Type" );
				//	if( block != null )
				//		settings.GameType = block.GetAttribute( "CurrentValue" );
				//}

				////MatchTime
				//{
				//	var block = rootBlock.FindChild( "Match Time" );
				//	if( block != null )
				//		int.TryParse( block.GetAttribute( "CurrentValue" ), out settings.MatchTimeLimit );
				//}

				////bots
				//for( int n = 0; n < 6; n++ )
				//{
				//	var block = rootBlock.FindChild( $"Bot {n + 1}" );
				//	if( block != null )
				//	{
				//		var valueString = block.GetAttribute( "CurrentValue" );

				//		if( valueString == "No Bot" || valueString == "" )
				//			settings.Bots[ n ] = "";
				//		else
				//			settings.Bots[ n ] = valueString;
				//	}
				//}

				////user roles
				//foreach( var block in rootBlock.Children )
				//{
				//	if( block.Name.StartsWith( "UserRole " ) )
				//	{
				//		long.TryParse( block.Name.Substring( "UserRole ".Length ), out var userID );
				//		var valueString = block.GetAttribute( "CurrentValue" );

				//		settings.UserRoles[ userID ] = valueString;
				//	}
				//}

				matchSettings = settings;
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchSettingsAsync error: " + e.ToString() );
			}
		}

		async Task GetMatchDetailsAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<string>( "CloudServerImplementation", "GetMatchDetails", cts.Token, MatchInfo.Id );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					Log.Warning( "GetMatchDetailsAsync: CloudServerImplementation.GetMatchDetails error: " + result.Error );
					return;
				}

				var rootBlock = TextBlock.Parse( result.Value, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "GetMatchDetailsAsync: CloudServerImplementation.GetMatchDetails parse error: " + error );
					return;
				}

				var details = new MatchDetails();

				//participants
				var participantsBlock = rootBlock.FindChild( "Participants" );
				if( participantsBlock != null )
				{
					foreach( var itemBlock in participantsBlock.Children )
					{
						var participant = new MatchDetailsBase.Participant();

						if( int.TryParse( itemBlock.GetAttribute( "PlayerIndex" ), out var playerIndex ) )
							participant.PlayerIndex = playerIndex;
						long.TryParse( itemBlock.GetAttribute( "UserID" ), out participant.UserID );
						participant.Username = itemBlock.GetAttribute( "Username" );
						participant.BotLevel = itemBlock.GetAttribute( "BotLevel" );

						participant.ThisUser = participant.UserID == CloudServiceClient.ThisUserID;

						details.Participants.Add( participant );
					}
				}

				//Players
				{
					var players = new List<MatchDetailsBase.Participant>();
					foreach( var participant in details.Participants )
					{
						if( participant.PlayerIndex >= 0 )
						{
							while( players.Count <= participant.PlayerIndex )
								players.Add( null );
							players[ participant.PlayerIndex ] = participant;
						}
					}
					details.Players = players.ToArray();
				}

				var playerCount = details.Players.Length;

				int.TryParse( rootBlock.GetAttribute( "GameCounter" ), out details.GameCounter );

				matchDetailsPrevious = matchDetails;
				matchDetails = details;

				//update scene from the main thread
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					//play sound game started
					if( matchDetailsPrevious == null || matchDetailsPrevious.GameCounter != matchDetails.GameCounter )
						SoundPlay2D( @"Game\Sounds\Game started.ogg" );
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchDetailsAsync error: " + e.ToString() );
			}
		}

		public void ButtonAddBot_Click( NeoAxis.UIButton sender )
		{
			var gameLogic = GameLogic;
			if( gameLogic != null )
			{
				if( gameLogic.NetworkIsClient && SimulationAppClient.Created )
				{
					//multiplayer mode
					gameLogic.Client_SendRequestAddBot();
				}
				else if( gameLogic.NetworkIsSingle )
				{
					//single mode
					gameLogic.Single_AddBot();
				}
			}
		}

		public void ButtonDeleteBot_Click( NeoAxis.UIButton sender )
		{
			var gameLogic = GameLogic;
			if( gameLogic != null )
			{
				if( gameLogic.NetworkIsClient && SimulationAppClient.Created )
				{
					//multiplayer mode
					var array = gameLogic.Client_GetUsers().Where( u => u.Value.Bot ).ToArray();
					if( array.Length > 0 )
						gameLogic.Client_SendRequestDeleteBot( array[ array.Length - 1 ].Value.UserID );
				}
				else if( gameLogic.NetworkIsSingle )
				{
					//single mode
					var array = gameLogic.Single_GetUsers();
					if( array.Length > 0 )
						gameLogic.Single_DeleteBot( array[ array.Length - 1 ].UserID );
				}
			}
		}
	}
}