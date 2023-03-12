// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;
using System.Linq;

namespace Project
{
	public class ShooterInGameContextScreen : NeoAxis.UIControl
	{
		Scene scene;
		GameMode gameMode;
		ShooterNetworkLogic networkLogic;

		//bool allowChangesByControls;

		/////////////////////////////////////////

		UIControl GetControlCurrentGame() { return GetComponent<UIControl>( "Control Current Game" ); }
		UIText GetTextGameInfo() { return GetControlCurrentGame().GetComponent( "Text Game Info" ) as UIText; }
		UIButton GetButtonAddBotLevel1() { return GetControlCurrentGame().GetComponent( "Button Add Bot Level 1" ) as UIButton; }
		UIButton GetButtonAddBotLevel2() { return GetControlCurrentGame().GetComponent( "Button Add Bot Level 2" ) as UIButton; }
		UIButton GetButtonAddBotLevel3() { return GetControlCurrentGame().GetComponent( "Button Add Bot Level 3" ) as UIButton; }
		UIButton GetButtonDeleteBot() { return GetControlCurrentGame().GetComponent( "Button Delete Bot" ) as UIButton; }

		/////////////////////////////////////////

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
		public ShooterNetworkLogic NetworkLogic
		{
			get { return networkLogic; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EngineApp.IsSimulation && EnabledInHierarchyAndIsInstance )
			{
				scene = ClientUtility.GetScene();
				gameMode = ClientUtility.GetGameMode();
				networkLogic = ClientUtility.GetNetworkLogic() as ShooterNetworkLogic;
			}

			if( networkLogic != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					networkLogic.ReceiveNetworkMessageFromServer += NetworkLogic_ReceiveNetworkMessageFromServer;
				else
					networkLogic.ReceiveNetworkMessageFromServer -= NetworkLogic_ReceiveNetworkMessageFromServer;
			}

			//if( EngineApp.IsSimulation && EnabledInHierarchyAndIsInstance )
			//	allowChangesByControls = true;
		}

		//void SendGetGamesToJoin()
		//{
		//	if( NetworkLogic != null )
		//	{
		//		var writer = NetworkLogic.BeginNetworkMessageToServer( "GetGamesToJoinForInGameContextScreen" );
		//		if( writer != null )
		//		{
		//			var list = GetListGameTypes();
		//			var specifiedGameType = list.SelectedIndex == 0 ? "" : list.SelectedItem;

		//			writer.Write( specifiedGameType );
		//			NetworkLogic.EndNetworkMessage();
		//		}
		//	}
		//}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( networkLogic != null )
			{
				var gameInfo = "";

				var gameType = networkLogic.GameType.Value;

				gameInfo += "Type: " + EnumUtility.GetValueDisplayName( gameType );
				gameInfo += "\nStatus: " + networkLogic.CurrentGameStatus.Value.ToString();
				gameInfo += "\nText status: " + networkLogic.GetGameTextStatus();

				gameInfo += "\n\nPlayers:";

				var users = networkLogic.ClientGetUsers().ToArray();

				//sort by points
				CollectionUtility.MergeSort( users, delegate ( ShooterNetworkLogic.ClientUserItem i1, ShooterNetworkLogic.ClientUserItem i2 )
				{
					return i2.Points - i1.Points;
				} );

				foreach( var user in users )
				{
					string text;
					var userInfo = SimulationAppClient.Client?.Users.GetUser( user.UserID );
					if( userInfo != null )
					{
						text = userInfo.Username;
						text += " " + user.Points.ToString();

						if( gameType == ShooterGameTypeEnum.TeamDeathmatch )
						{
							var teamNumber = user.Team + 1;
							text += $", team {teamNumber}";
						}
					}
					else
						text = "Unknown";

					gameInfo += "\n" + text;
				}


				if( gameType == ShooterGameTypeEnum.TeamDeathmatch )
				{
					gameInfo += "\n\nTeams:";

					var team1 = 0;
					var team2 = 0;
					foreach( var user in users )
					{
						if( user.Team == 0 )
							team1 += user.Points;
						else
							team2 += user.Points;
					}

					gameInfo += $"\nTeam 1: {team1}";
					gameInfo += $"\nTeam 2: {team2}";
				}

				GetTextGameInfo().Text = gameInfo;

				//GetButtonAddBotLevel1().Visible = isAdmin;
				//GetButtonAddBotLevel2().Visible = GetButtonAddBotLevel1().Visible;
				//GetButtonAddBotLevel3().Visible = GetButtonAddBotLevel1().Visible;
				GetButtonAddBotLevel1().ReadOnly = true;
				GetButtonAddBotLevel2().ReadOnly = GetButtonAddBotLevel1().ReadOnly;
				GetButtonAddBotLevel3().ReadOnly = GetButtonAddBotLevel1().ReadOnly;

				//GetButtonDeleteBot().Visible = isAdmin;
				GetButtonDeleteBot().ReadOnly = true;//game == null || game.GetPlayersOnClient( false ).FirstOrDefault( u => u.Bot ) == null;
			}
		}

		private void NetworkLogic_ReceiveNetworkMessageFromServer( NeoAxis.Component sender, string message, ArrayDataReader reader, ref bool error )
		{
			//if( message == "GamesToJoinForInGameContextScreen" )
			//{
			//	var blockString = reader.ReadString();
			//	if( !reader.Complete() )
			//		return;

			//	gamesToJoin.Clear();

			//	var block = TextBlock.Parse( blockString, out _ );
			//	if( block != null )
			//	{
			//	}
			//}
		}

		public void ButtonDeleteBot_Click( NeoAxis.UIButton sender )
		{
			//if( networkLogic == null )
			//	return;
			//networkLogic.BeginNetworkMessageToServer( "DeleteBot" );
			//networkLogic.EndNetworkMessage();
		}

		//void AddBot( int level )
		//{
		//	if( networkLogic == null )
		//		return;
		//	var writer = networkLogic.BeginNetworkMessageToServer( "AddBot" );
		//	if( writer != null )
		//	{
		//		writer.Write( level );
		//		networkLogic.EndNetworkMessage();
		//	}
		//}

		public void ButtonAddBotLevel1_Click( NeoAxis.UIButton sender )
		{
			//AddBot( 1 );
		}

		public void ButtonAddBotLevel2_Click( NeoAxis.UIButton sender )
		{
			//AddBot( 2 );
		}

		public void ButtonAddBotLevel3_Click( NeoAxis.UIButton sender )
		{
			//AddBot( 3 );
		}
	}
}