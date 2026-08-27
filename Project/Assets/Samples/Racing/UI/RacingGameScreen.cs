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
	public class RacingGameScreen : BasicSceneScreen
	{
		//not used right now. synchronized via RacingGameLogic. Can be used for additional info.
		//match settings
		bool matchSettingsNeedRequest = true;
		MatchSettings matchSettings;

		//!!!!
		//match details
		volatile bool matchDetailsNeedRequest = true;
		volatile MatchDetails matchDetails;
		volatile MatchDetails matchDetailsPrevious;

		//various
		double lastOnUpdateTime;
		RacingGameLogic.StatusInfo currentStatus = new RacingGameLogic.StatusInfo();
		int checkpointsPassedByThisPlayer;

		///////////////////////////////////////////////

		//In-game menu controls
		[Browsable( false )]
		public UIButton ButtonAddBot { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Add Bot" ); } }
		[Browsable( false )]
		public UIButton ButtonDeleteBot { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Delete Bot" ); } }

		///////////////////////////////////////////////

		//not used right now. synchronized via RacingGameLogic. Can be used for additional info.
		public class MatchSettings
		{
		}

		///////////////////////////////////////////////

		public class MatchDetails
		{
			//common
			public List<Participant> Participants = new List<Participant>();
			public int GameCounter;

			//game specific
			public Participant[] Players = Array.Empty<Participant>();

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

		public RacingGameScreen()
		{
			EngineConfig.RegisterClassParameters( GetType() );
		}

		public new RacingGameLogic GameLogic
		{
			get { return base.GameLogic as RacingGameLogic; }
		}

		protected override void OnEnabledInSimulationAndIsInstance()
		{
			base.OnEnabledInSimulationAndIsInstance();

			if( GameLogic != null )
				GameLogic.CheckpointsPassedTimeChanged += GameLogic_CheckpointsPassedTimeChanged;
		}

		public int GetPlayerIndex()
		{
			var gameLogic = GameLogic;

			if( MatchInfo != null && matchDetails != null )
			{
				//cloud mode
				return matchDetails.GetThisUserPlayerIndex();
			}
			else if( gameLogic.NetworkIsClient && SimulationAppClient.Created )
			{
				//multiplayer mode
				var thisUserID = SimulationAppClient.ConnectionNode?.Users.ThisUser?.UserID ?? 0;
				var users = gameLogic.Client_GetUsers().Values.ToArray();
				for( int n = 0; n < users.Length; n++ )
				{
					var user = users[ n ];
					if( user.UserID == thisUserID )
						return n;
				}
			}
			else if( gameLogic.NetworkIsSingle )
			{
				//single mode
				var users = gameLogic.Single_GetUsers();
				for( int n = 0; n < users.Length; n++ )
				{
					var user = users[ n ];
					if( !user.Bot )
						return n;
				}
			}

			return -1;
		}

		private void GameLogic_CheckpointsPassedTimeChanged( RacingGameLogic sender )
		{
			var gameLogic = GameLogic;

			//update checkpointsPassedByThisPlayer to play sound when checkpoint passed
			{
				var playerIndex = GetPlayerIndex();
				if( playerIndex != -1 )
				{
					if( checkpointsPassedByThisPlayer != gameLogic.CheckpointsPassed[ playerIndex ] )
					{
						checkpointsPassedByThisPlayer = gameLogic.CheckpointsPassed[ playerIndex ];

						//play sound checkpoint passed
						SoundPlay2D( @"Samples\Racing\Sounds\Checkpoint passed.ogg" );
					}
				}
			}

			//play sound game ended
			if( GameLogic.IsMatchOver() )
				SoundPlay2D( @"Samples\Racing\Sounds\Game ended.ogg" );
		}

		void ProcessGameEvents()
		{
			var newStatus = GameLogic.GetStatus();

			//play sound countdown 3
			if( currentStatus.PreRaceTimeRemaining >= 3 && newStatus.PreRaceTimeRemaining < 3 )
				SoundPlay2D( @"Samples\Racing\Sounds\Countdown.ogg" );

			//play sound countdown 2
			if( currentStatus.PreRaceTimeRemaining >= 2 && newStatus.PreRaceTimeRemaining < 2 )
				SoundPlay2D( @"Samples\Racing\Sounds\Countdown.ogg" );

			//play sound countdown 1
			if( currentStatus.PreRaceTimeRemaining >= 1 && newStatus.PreRaceTimeRemaining < 1 )
				SoundPlay2D( @"Samples\Racing\Sounds\Countdown.ogg" );

			//play sound go
			if( currentStatus.RaceTime == 0 && newStatus.RaceTime > 0 )
				SoundPlay2D( @"Samples\Racing\Sounds\Go.ogg" );

			currentStatus = newStatus;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( Scene != null )
			{
				if( EngineApp.EngineTime - lastOnUpdateTime > 0.05 )
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

					if( GameLogic != null )
					{
						ProcessGameEvents();
						UpdateCheckpointsDependsPassedState();
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
						lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
					}
				}
				else if( gameLogic.NetworkIsSingle )
				{
					//single mode
					lines.Add( ("Players:", new ColorValue( 0.95, 0.95, 0.95 )) );
					foreach( var singleUserItem in gameLogic.Single_GetUsers() )
					{
						var text = singleUserItem.UserID == 0 ? "You" : $"Bot {singleUserItem.UserID}";
						lines.Add( (text, new ColorValue( 0.95, 0.95, 0.95 )) );
					}
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
					CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, text, position, EHorizontalAlignment.Right, EVerticalAlignment.Top, color );
				}

				position.Y += fontHeight;
			}
		}

		void DrawGameStatus( CanvasRenderer renderer )
		{
			var text = "";
			var color = new ColorValue( 0.95, 0.95, 0.95 );

			if( currentStatus.PreRaceTimeRemaining > 0 )
			{
				var secondsToStart = (int)currentStatus.PreRaceTimeRemaining + 1;
				text = $"{secondsToStart} seconds before start.";
			}

			if( currentStatus.RaceTime > 0 && currentStatus.RaceTime < 3 )
			{
				text = "Go!";
				color = new ColorValue( 0, 1, 0 );
			}

			if( GameLogic.IsMatchOver() )
			{
				var thisUserPlayerIndex = GetPlayerIndex();
				if( thisUserPlayerIndex != -1 )
				{
					var endResult = GameLogic.GetEndedGamePlayerIndexesWithTime();

					var thisUserTime = -1.0f;
					foreach( var item in endResult )
					{
						if( item.Item1 == thisUserPlayerIndex )
						{
							thisUserTime = item.Item2;
							break;
						}
					}

					if( endResult.Length != 0 && endResult[ 0 ].Item1 == thisUserPlayerIndex )
					{
						text = "You win with time " + thisUserTime.ToString( "0.00" ) + " seconds!";
						color = new ColorValue( 0.1, 0.95, 0.1 );
					}
					else
					{
						text = "You didn't win. Finished in " + thisUserTime.ToString( "0.00" ) + " seconds.";
						color = new ColorValue( 0.95, 0.1, 0.1 );
					}
				}
				else
				{
					text = "Game over.";
					color = new ColorValue( 0.95, 0.95, 0.1 );
				}
			}

			if( !string.IsNullOrEmpty( text ) )
			{
				var fontHeight = renderer.DefaultFontSize * 1.5;
				var position = new Vector2( 0.5, 0.0 + fontHeight / 2 );
				CanvasRendererUtility.AddTextWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, text, position, EHorizontalAlignment.Center, EVerticalAlignment.Top, color );
			}
		}

		void DrawCurrentPlayerProgress( CanvasRenderer renderer )
		{
			var gameLogic = GameLogic;
			var laps = gameLogic.Laps.Value;

			var lines = new List<string>();

			lines.Add( currentStatus.RaceTime.ToString( "0.00" ) );

			var playerIndex = GetPlayerIndex();
			if( playerIndex != -1 )
			{
				var totalCheckpointsToPass = gameLogic.Checkpoints.Length * laps;

				var previousLapTime = 0.0;
				var raceTime = 0.0;

				for( int nLap = 0; nLap < laps; nLap++ )
				{
					var lapTime = 0.0;
					var index = playerIndex * totalCheckpointsToPass + gameLogic.Checkpoints.Length * ( nLap + 1 ) - 1;
					if( index >= 0 && index < gameLogic.CheckpointsPassedTime.Length )
						lapTime = gameLogic.CheckpointsPassedTime[ index ];

					if( lapTime != 0.0 )
					{
						var lapTime2 = lapTime - previousLapTime;
						previousLapTime = lapTime;
						if( nLap == laps - 1 )
							raceTime = lapTime;

						lines.Add( $"Lap {nLap + 1}: {lapTime2.ToString( "0.00" )}" );
					}
					else
						lines.Add( $"Lap {nLap + 1}: -" );
				}

				if( raceTime != 0.0 )
					lines.Add( $"Race: {raceTime.ToString( "0.00" )}" );
				else
					lines.Add( $"Race: -" );
			}

			var fontHeight = renderer.DefaultFontSize;
			var position = ConvertOffset( new UIMeasureValueVector2( UIMeasure.Units, 10, 10 ), UIMeasure.Screen );
			CanvasRendererUtility.AddTextLinesWithShadow( renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, fontHeight, lines, new Rectangle( position.X, position.Y, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			if( Scene != null && GameLogic != null )
			{
				DrawTextInRightTopCorner( renderer );
				DrawGameStatus( renderer );
				DrawCurrentPlayerProgress( renderer );
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
		}

		protected override void Scene_RenderEvent( Scene scene, Viewport viewport )
		{
			base.Scene_RenderEvent( scene, viewport );
		}

		protected override void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
			base.GameMode_GetCameraSettingsEvent( sender, viewport, cameraDefault, ref cameraSettings );

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

				//this data gets via RacingGameLogic

				//////SceneName
				////{
				////	var block = rootBlock.FindChild( "Scene Name" );
				////	if( block != null )
				////		settings.SceneName = block.GetAttribute( "CurrentValue" );
				////}

				//////GameType
				////{
				////	var block = rootBlock.FindChild( "Game Type" );
				////	if( block != null )
				////		settings.GameType = block.GetAttribute( "CurrentValue" );
				////}

				//////MatchTime
				////{
				////	var block = rootBlock.FindChild( "Match Time" );
				////	if( block != null )
				////		int.TryParse( block.GetAttribute( "CurrentValue" ), out settings.MatchTimeLimit );
				////}

				//////bots
				////for( int n = 0; n < 6; n++ )
				////{
				////	var block = rootBlock.FindChild( $"Bot {n + 1}" );
				////	if( block != null )
				////	{
				////		var valueString = block.GetAttribute( "CurrentValue" );

				////		if( valueString == "No Bot" || valueString == "" )
				////			settings.Bots[ n ] = "";
				////		else
				////			settings.Bots[ n ] = valueString;
				////	}
				////}

				//////user roles
				////foreach( var block in rootBlock.Children )
				////{
				////	if( block.Name.StartsWith( "UserRole " ) )
				////	{
				////		long.TryParse( block.Name.Substring( "UserRole ".Length ), out var userID );
				////		var valueString = block.GetAttribute( "CurrentValue" );

				////		settings.UserRoles[ userID ] = valueString;
				////	}
				////}

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
						var participant = new MatchDetails.Participant();

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
					var players = new List<MatchDetails.Participant>();
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

		void UpdateCheckpointsDependsPassedState()
		{
			var gameLogic = GameLogic;

			var playerIndex = GetPlayerIndex();
			if( playerIndex == -1 )
				return;
			if( gameLogic.Laps == 0 )
				return;

			var totalCheckpointsToPass = gameLogic.Laps * gameLogic.Checkpoints.Length;

			var checkpointsPassed = 0;
			if( playerIndex < gameLogic.CheckpointsPassed.Length )
				checkpointsPassed = gameLogic.CheckpointsPassed[ playerIndex ];

			var checkpointIndexToPass = -1;
			if( checkpointsPassed < totalCheckpointsToPass )
				checkpointIndexToPass = checkpointsPassed + 1;

			var currentCheckpointToPass = checkpointIndexToPass != -1 ? checkpointIndexToPass % gameLogic.Checkpoints.Length : -1;

			for( int n = 0; n < gameLogic.Checkpoints.Length; n++ )
			{
				var checkpoint = gameLogic.Checkpoints[ n ];

				foreach( var obj in checkpoint.MeshObjects )
				{
					if( currentCheckpointToPass == n )
					{
						var outline = new ObjectSpecialRenderingEffect_Outline() { Color = new ColorValue( 1, 1, 0 ), Scale = 1 };
						obj.SpecialEffects = new List<ObjectSpecialRenderingEffect> { outline };
					}
					else
						obj.SpecialEffects = null;
				}
			}
		}
	}
}