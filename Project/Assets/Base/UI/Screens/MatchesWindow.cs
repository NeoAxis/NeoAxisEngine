// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.CloudServer;

namespace Project
{
	public class MatchesWindow : UIWindow
	{
		Matches.Match[] matchesFromServer = Array.Empty<Matches.Match>();
		DateTime getMatchesFromServerLastTime;

		Matches.Match[] matchesInList = Array.Empty<Matches.Match>();
		DateTime updateMatchesInListLastTime;

		int totalClients;
		int totalMatches;
		DateTime getProjectDetailsLastTime;

		bool getMatchPlayingCalled;

		/////////////////////////////////////////

		UIText TextCommonInfo { get { return GetComponent<UIText>( "Text Common Info" ); } }
		UIList ListMatches { get { return GetComponent<UIList>( "List Matches" ); } }
		UIButton ButtonNew { get { return GetComponent<UIButton>( "Button New" ); } }
		UIButton ButtonFind { get { return GetComponent<UIButton>( "Button Find" ); } }
		UIButton ButtonEnter { get { return GetComponent<UIButton>( "Button Enter" ); } }
		UIEdit EditMatchInfo { get { return GetComponent<UIEdit>( "Edit Match Info" ); } }
		UIButton ButtonClose { get { return GetComponent<UIButton>( "Button Close" ); } }

		/////////////////////////////////////////

		public delegate void MatchContinuePlayDelegate( MatchesWindow sender, Matches.Match matchInfo );
		public static event MatchContinuePlayDelegate MatchContinuePlay;

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//register [EngineConfig] fields, properties
			EngineConfig.RegisterClassParameters( typeof( MatchesWindow ) );


			//!!!!impl
			ButtonFind.Enabled = false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				//update controls size and position
				{
					var indentSize = ConvertOffsetX( new UIMeasureValueDouble( UIMeasure.Units, 10 ), UIMeasure.Screen );

					//update ListMatches
					ListMatches.Size = new UIMeasureValueVector2( UIMeasure.Screen, ButtonClose.GetScreenRectangle().Left - indentSize * 2 - ListMatches.GetScreenPosition().X, ButtonClose.GetScreenRectangle().Bottom - ListMatches.GetScreenPosition().Y );
					//ListMatches.Size = new UIMeasureValueVector2( UIMeasure.Screen, 0.5, ButtonClose.GetScreenRectangle().Bottom - ListMatches.GetScreenPosition().Y );

					//update EditMatchInfo

					//EditMatchInfo is disabled
					EditMatchInfo.Enabled = false;

					//var listMatchesScreenRectangle = ListMatches.GetScreenRectangle();
					//EditMatchInfo.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, listMatchesScreenRectangle.Right + indentSize, listMatchesScreenRectangle.Top, 0, 0 );
					//EditMatchInfo.Size = new UIMeasureValueVector2( UIMeasure.Screen, ButtonClose.GetScreenRectangle().Left - indentSize * 2 - EditMatchInfo.GetScreenPosition().X, ButtonClose.GetScreenRectangle().Bottom - EditMatchInfo.GetScreenPosition().Y );
				}

				//update controls state
				{
					ButtonEnter.ReadOnly = GetSelectedMatch() == null;
					TextCommonInfo.Text = FormatCommonInfo();
				}

				var utcNow = DateTime.UtcNow;

				//!!!!update when event. compare GetMathes in the server

				//get matches
				if( ( utcNow - getMatchesFromServerLastTime ).TotalSeconds > 5 )
				{
					getMatchesFromServerLastTime = utcNow;
					Task.Run( GetMatchesFromServerAsync );
				}

				//!!!!update when event

				//get project details
				if( ( utcNow - getProjectDetailsLastTime ).TotalSeconds > 10 )
				{
					getProjectDetailsLastTime = utcNow;
					Task.Run( GetProjectDetailsAsync );
				}

				//update list control
				if( ( utcNow - updateMatchesInListLastTime ).TotalSeconds > 0.1 )
				{
					updateMatchesInListLastTime = utcNow;
					UpdateMatchesList();
				}

				//get match where player is playing
				if( !getMatchPlayingCalled )
				{
					getMatchPlayingCalled = true;
					Task.Run( GetMatchPlayingAsync );
				}
			}
		}

		public void ButtonClose_Click( NeoAxis.UIButton sender )
		{
			Dispose();
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				Dispose();
				return true;
			}

			return base.OnKeyDown( e );
		}

		public void ButtonNew_Click( NeoAxis.UIButton sender )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			MessageBoxWindow.Show( this, "Create a new match?", "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender2, EDialogResult result2, object anyData )
			{
				if( result2 == EDialogResult.Yes )
				{
					Task.Run( async delegate ()
					{
						//create match on the server
						var matchID = 0L;
						{
							var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
							var result = await client.CallMethodAsync<long>( "Matches", "NewMatch", cts.Token, null, null );
							if( !string.IsNullOrEmpty( result.Error ) )
							{
								EngineThreading.ExecuteFromMainThreadLater( delegate ()
								{
									var text = result.Error;
									//clamp up to return line
									if( text.IndexOfAny( new[] { '\r', '\n' } ) != -1 )
										text = text.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries )[ 0 ];
									MessageBoxWindow.Show( this, text, "Can't create match", EMessageBoxButtons.OK, EMessageBoxIcon.Warning );
								} );

								//Log.Warning( "Error: " + result.Error );
								//matchesFromServer = Array.Empty<Matches.Match>();

								return;
							}

							matchID = result.Value;
						}

						//open match window
						EngineThreading.ExecuteFromMainThreadLater( delegate ()
						{
							var matchWindow = ResourceManager.LoadSeparateInstance<MatchWindow>( @"Base\UI\Screens\MatchWindow.ui", false, true );
							matchWindow.MatchID = matchID;
							Parent.AddComponent( matchWindow );
						} );
					} );
				}
			} );
		}

		public void ButtonFind_Click( NeoAxis.UIButton sender )
		{
		}

		Matches.Match GetSelectedMatch()
		{
			var selectedMatch = ListMatches.SelectedItem?.Tag as Matches.Match;
			return selectedMatch;
		}

		async Task EnterToMatchAsync( long matchID )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			//call EnterMatch on the server
			var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
			var result = await client.CallMethodAsync<long>( "Matches", "EnterMatch", cts.Token, matchID, null );
			if( !string.IsNullOrEmpty( result.Error ) )
			{
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					var text = result.Error;
					//clamp up to return line
					if( text.IndexOfAny( new[] { '\r', '\n' } ) != -1 )
						text = text.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries )[ 0 ];
					MessageBoxWindow.Show( this, text, "Error", EMessageBoxButtons.OK, EMessageBoxIcon.Warning );
				} );
				return;
			}

			//open match window
			EngineThreading.ExecuteFromMainThreadLater( delegate ()
			{
				var matchWindow = ResourceManager.LoadSeparateInstance<MatchWindow>( @"Base\UI\Screens\MatchWindow.ui", false, true );
				matchWindow.MatchID = matchID;
				Parent.AddComponent( matchWindow );
			} );
		}

		void EnterToSelectedMatch()
		{
			var selectedMatch = GetSelectedMatch();
			if( selectedMatch == null )
				return;

			Task.Run( async delegate ()
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var isUserOfMatch = false;
				{
					//call GetMatchUserOfCaller on the server
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var result = await client.CallMethodAsync<Matches.MatchUser>( "Matches", "GetMatchUserOfCaller", cts.Token, selectedMatch.Id );
					if( !string.IsNullOrEmpty( result.Error ) )
					{
						Log.Warning( "Error: " + result.Error );
						return;
					}

					if( result.Value != null && result.Value.MatchID == selectedMatch.Id )
						isUserOfMatch = true;
				}

				if( !isUserOfMatch )
				{
					//ask to enter
					EngineThreading.ExecuteFromMainThreadLater( delegate ()
					{
						var text = $"You are not a player of \"{selectedMatch.Name}\". Do you want to enter?";
						MessageBoxWindow.Show( this, text, "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
						{
							if( result == EDialogResult.Yes )
							{
								Task.Run( () => EnterToMatchAsync( selectedMatch.Id ) );
							}
						} );
					} );
				}
				else
				{
					//open match window
					EngineThreading.ExecuteFromMainThreadLater( delegate ()
					{
						var matchWindow = ResourceManager.LoadSeparateInstance<MatchWindow>( @"Base\UI\Screens\MatchWindow.ui", false, true );
						matchWindow.MatchID = selectedMatch.Id;
						Parent.AddComponent( matchWindow );
					} );
				}
			} );
		}

		public void ButtonEnter_Click( NeoAxis.UIButton sender )
		{
			EnterToSelectedMatch();
		}

		async Task GetMatchesFromServerAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				//get matches with Lobby status
				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<Matches.Match[]>( "Matches", "GetMatches", cts.Token, null, null, null, new[] { "Lobby" } );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					Log.Warning( "Error: " + result.Error );
					matchesFromServer = Array.Empty<Matches.Match>();
					return;
				}
				matchesFromServer = result.Value;
			}
			catch( Exception e )
			{
				Log.Warning( e.ToString() );
			}
		}

		void UpdateMatchesList()
		{
			var changed = false;

			var matches = matchesFromServer;
			if( matches.Length == matchesInList.Length )
			{
				for( int n = 0; n < matchesFromServer.Length; n++ )
				{
					var match = matchesFromServer[ n ];
					var matchInList = matchesInList[ n ];

					if( match.Id != matchInList.Id || match.Name != matchInList.Name || match.AnyData != matchInList.AnyData )
					{
						changed = true;
						break;
					}
				}
			}
			else
				changed = true;

			if( changed )
			{
				var list = ListMatches;

				long selectedMatchID = 0;
				{
					var selectedMatch = list.SelectedItem?.Tag as Matches.Match;
					if( selectedMatch != null )
						selectedMatchID = selectedMatch.Id;
				}

				list.ClearItems();

				foreach( var match in matches )
				{
					var text = match.Name;
					if( !string.IsNullOrEmpty( match.AnyData ) )
						text += " - " + match.AnyData;

					if( CloudServiceClient.ThisUserID == match.UserID )
						text += " (Your)";

					list.AddItem( text, match );

					if( selectedMatchID != 0 && match.Id == selectedMatchID || selectedMatchID == 0 && CloudServiceClient.ThisUserID == match.UserID )
					{
						list.SelectedIndex = list.Items.Count - 1;
						list.EnsureVisible( list.SelectedIndex );
					}
				}

				matchesInList = matches;
			}
		}

		public void ListMatches_ItemMouseDoubleClick( NeoAxis.UIControl sender, NeoAxis.EMouseButtons button, ref bool handled )
		{
			EnterToSelectedMatch();
		}

		public void ListMatches_KeyDown( NeoAxis.UIControl sender, NeoAxis.KeyEvent e, ref bool handled )
		{
			if( e.Key == EKeys.Enter )
			{
				EnterToSelectedMatch();
				handled = true;
			}
		}

		async Task GetProjectDetailsAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				TextBlock rootBlock = null;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<string>( "Implementation", "GetProjectDetails", cts.Token );
				if( !string.IsNullOrEmpty( result.Error ) )
					Log.Warning( "Error: " + result.Error );
				else
				{
					rootBlock = TextBlock.Parse( result.Value, out var error );
					if( !string.IsNullOrEmpty( error ) )
						Log.Warning( "Error: " + error );
				}

				if( rootBlock == null )
					rootBlock = new TextBlock();

				int.TryParse( rootBlock.GetAttribute( "Clients" ), out totalClients );
				int.TryParse( rootBlock.GetAttribute( "Matches" ), out totalMatches );

			}
			catch( Exception e )
			{
				Log.Warning( "GetProjectDetailsAsync error: " + e.ToString() );
			}
		}

		string FormatCommonInfo()
		{
			string Pluralize( int value, string singular, string plural )
			{
				return value == 1 ? singular : plural;
			}

			var clientsText = totalClients.ToString();
			var matchesText = totalMatches.ToString();

			if( totalMatches == 0 )
				return $"No active matches. {clientsText} {Pluralize( totalClients, "player", "players" )} online.";

			return $"{clientsText} {Pluralize( totalClients, "player", "players" )} across {matchesText} {Pluralize( totalMatches, "match", "matches" )}.";
		}

		async Task GetMatchPlayingAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<Matches.Match>( "Implementation", "GetMatchPlayingByCaller", cts.Token );

				if( !string.IsNullOrEmpty( result.Error ) )
				{
					Log.Warning( "Error: " + result.Error );
					return;
				}

				var match = result.Value;
				//!!!!null is not supported
				if( match == null || match.Id == 0 )
					return;

				//found match where player is playing

				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					//ask to enter

					var isCreator = match.UserID == CloudServiceClient.ThisUserID;

					var text = $"You are playing \"{match.Name}\". Do you want to continue?";

					var windowData = MessageBoxWindow.Show( this, text, "Confirm", EMessageBoxButtons.YesNoCancel, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender, EDialogResult result2, object anyData )
					{
						if( result2 == EDialogResult.Yes )
						{
							MatchContinuePlay?.Invoke( this, match );
						}
						else if( result2 == EDialogResult.No )
						{
							Task.Run( async delegate ()
							{
								if( isCreator )
								{
									//delete the match
									var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
									var result = await client.CallMethodAsync( "Matches", "UpdateMatch", cts.Token, match.Id, "Deleted", null, null );
									if( !string.IsNullOrEmpty( result.Error ) )
									{
										Log.Warning( "Error: " + result.Error );
										return;
									}
								}
								else
								{
									//leave the match
									var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
									var result = await client.CallMethodAsync( "Matches", "RemoveMatchUser", cts.Token, match.Id, CloudServiceClient.ThisUserID );
									if( !string.IsNullOrEmpty( result.Error ) )
									{
										Log.Warning( "Error: " + result.Error );
										return;
									}
								}
							} );
						}
					} );

					windowData.Window.SetButtonName( EDialogResult.No, isCreator ? "Delete" : "Leave" );
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchPlayingAsync error: " + e.ToString() );
			}
		}
	}
}