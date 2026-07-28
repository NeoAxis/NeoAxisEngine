// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.CloudServer;

namespace Project
{
	public class MatchWindow : UIWindow
	{
		[Browsable( false )]
		public long MatchID { get; set; }

		//get match info once is enough
		Matches.Match matchInfo;
		DateTime getMatchInfoLastTime;

		//TextBlock matchSettingsBlock;
		string matchSettingsBlockText;
		DateTime getMatchSettingsBlockLastTime;

		string matchDetailsBlockText;
		DateTime getMatchDetailsLastTime;
		string notReadyReason;

		//settings
		List<UICombo> comboSubscribedSelectedIndex = new List<UICombo>();

		//chat
		bool chatNewMessagesAvailable = true;
		volatile bool chatGettingNewMessages;

		/////////////////////////////////////////

		UIControlList ControlListSettings { get { return GetComponent<UIControlList>( "Control List Settings" ); } }
		UIControl ControlSettingsCombo { get { return GetComponent<UIControl>( "Control Settings Combo" ); } }
		UIControl ControlSettingsCheck { get { return GetComponent<UIControl>( "Control Settings Check" ); } }

		UIButton ButtonStart { get { return GetComponent<UIButton>( "Button Start" ); } }
		UIButton ButtonDelete { get { return GetComponent<UIButton>( "Button Delete" ); } }
		UIButton ButtonKick { get { return GetComponent<UIButton>( "Button Kick" ); } }
		UIList ListParticipants { get { return GetComponent<UIList>( "List Participants" ); } }
		UIList ListChat { get { return GetComponent<UIList>( "List Chat" ); } }
		UIEdit EditChatMessage { get { return GetComponent<UIEdit>( "Edit Chat Message" ); } }
		UIButton ButtonChatSend { get { return GetComponent<UIButton>( "Button Chat Send" ); } }
		UIButton ButtonClose { get { return GetComponent<UIButton>( "Button Close" ); } }

		/////////////////////////////////////////

		public delegate void MatchStatusChangedToPlayDelegate( MatchWindow sender, Matches.Match matchInfo );
		public static event MatchStatusChangedToPlayDelegate MatchStatusChangedToPlay;

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//register [EngineConfig] fields, properties
			EngineConfig.RegisterClassParameters( typeof( MatchWindow ) );

			//reset background color
			ControlListSettings.BackgroundColor = new ColorValue( 0, 0, 0, 0 );
			ControlSettingsCombo.BackgroundColor = new ColorValue( 0, 0, 0, 0 );
			ControlSettingsCheck.BackgroundColor = new ColorValue( 0, 0, 0, 0 );

			//disable template controls
			ControlSettingsCombo.Enabled = false;
			ControlSettingsCheck.Enabled = false;

			ButtonStart.ReadOnly = true;
			EditChatMessage.ReadOnly = true;
			ButtonChatSend.ReadOnly = true;

			//register to receive messages from the server
			var client = CloudServiceClient.Client;
			if( client != null )
				client.ConnectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
		}

		protected override void OnDisabledInSimulation()
		{
			//unregister to receive messages from the server
			var client = CloudServiceClient.Client;
			if( client != null )
				client.ConnectionNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;

			base.OnDisabledInSimulation();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				var client = CloudServiceClient.Client;
				if( client != null )
				{
					//update controls size and position
					//{
					//	if( ControlListSettings != null )
					//	{
					//		var screenSize = ControlListSettings.GetScreenSize();
					//		var unitsSize = ConvertOffsetX( new UIMeasureValueDouble( UIMeasure.Screen, screenSize.X ), UIMeasure.Units );

					//		foreach( var control in ControlListSettings.GetItems() )
					//			control.Size = new UIMeasureValueVector2( UIMeasure.Units, unitsSize, control.Size.Value.Y );
					//	}
					//}

					//update controls state
					{
						//set window caption
						if( matchInfo != null )
							Text = matchInfo.Name;

						//only creator of the match can change settings and delete the match
						ControlListSettings.ReadOnly = matchInfo == null || matchInfo.UserID != CloudServiceClient.ThisUserID;

						//update ButtonDelete
						{
							var isCreator = matchInfo != null && matchInfo.UserID == CloudServiceClient.ThisUserID;
							ButtonDelete.ReadOnly = matchInfo == null;
							ButtonDelete.Text = isCreator ? "Delete" : "Leave";

							//update tooltip
							var tooltip = ButtonDelete.GetComponent<UITooltip>();
							if( tooltip != null )
								tooltip.Text = isCreator ? "Delete the match." : "Leave the match.";
						}

						//only creator of the match can kick players and only if some player is selected (and not himself)
						{
							GetSelectedParticipiant( out var selectedUserID, out var selectedText );
							ButtonKick.ReadOnly = !( matchInfo != null && selectedUserID > 0 && selectedUserID != CloudServiceClient.ThisUserID ) || matchInfo == null || matchInfo.UserID != CloudServiceClient.ThisUserID;
						}

						if( matchInfo != null && matchInfo.ChatID != 0 )
						{
							EditChatMessage.ReadOnly = false;
							ButtonChatSend.ReadOnly = false;
						}

						//update ButtonStart
						{
							//update read only state
							ButtonStart.ReadOnly = matchInfo == null || matchInfo.UserID != CloudServiceClient.ThisUserID || !string.IsNullOrEmpty( notReadyReason );

							//update tooltip
							var tooltip = ButtonStart.GetComponent<UITooltip>();
							if( tooltip != null )
							{
								if( !string.IsNullOrEmpty( notReadyReason ) )
									tooltip.Text = notReadyReason;
								else
									tooltip.Text = "Start the match.";
							}
						}
					}

					var utcNow = DateTime.UtcNow;

					//get match info
					if( matchInfo == null )
					{
						if( ( utcNow - getMatchInfoLastTime ).TotalSeconds > 5 )
						{
							getMatchInfoLastTime = utcNow;
							Task.Run( GetMatchInfoAsync );
						}
					}

					//!!!!update by events from server

					//get match settings
					if( ( utcNow - getMatchSettingsBlockLastTime ).TotalSeconds > 1 )
					{
						getMatchSettingsBlockLastTime = utcNow;
						Task.Run( GetMatchSettingsAsync );
					}

					//!!!!update by events from server

					//get match details
					if( ( utcNow - getMatchDetailsLastTime ).TotalSeconds > 1 )
					{
						getMatchDetailsLastTime = utcNow;
						Task.Run( GetMatchDetailsAsync );
					}

					//get new chat messages
					if( matchInfo != null && chatNewMessagesAvailable && !chatGettingNewMessages )
					{
						chatNewMessagesAvailable = false;
						chatGettingNewMessages = true;

						Chats.Message lastMessage = null;
						if( ListChat.Items.Count > 0 )
						{
							var lastItem = ListChat.Items[ ListChat.Items.Count - 1 ];
							lastMessage = lastItem.Tag as Chats.Message;
						}
						Task.Run( () => ChatGetNewMessagesAsync( lastMessage ) );
					}
				}
			}
		}

		public void ButtonClose_Click( NeoAxis.UIButton sender )
		{
			RemoveFromParent( true );
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				RemoveFromParent( true );
				return true;
			}

			return base.OnKeyDown( e );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			//draw background
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 0, 0, 0, 0.8 ) );

			base.OnRenderUI( renderer );
		}

		//disable all controls behind
		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.AllPreviousInHierarchy; }
		}

		public void ButtonDelete_Click( NeoAxis.UIButton sender )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			if( matchInfo == null )
				return;

			var isCreator = matchInfo.UserID == CloudServiceClient.ThisUserID;
			var text = isCreator ? "Delete the match?" : "Leave the match?";

			MessageBoxWindow.Show( this, text, "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender2, EDialogResult result2, object anyData )
			{
				if( result2 == EDialogResult.Yes )
				{
					Task.Run( async delegate ()
					{
						if( isCreator )
						{
							//delete the match
							var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
							var result = await client.CallMethodAsync( "Matches", "UpdateMatch", cts.Token, MatchID, "Deleted", null, null );
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
							var result = await client.CallMethodAsync( "Matches", "RemoveMatchUser", cts.Token, MatchID, CloudServiceClient.ThisUserID );
							if( !string.IsNullOrEmpty( result.Error ) )
							{
								Log.Warning( "Error: " + result.Error );
								return;
							}
						}
					} );
				}
			} );
		}

		async Task GetMatchInfoAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<Matches.Match>( "Matches", "GetMatch", cts.Token, MatchID );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					Log.Warning( "Error: " + result.Error );
					return;
				}

				matchInfo = result.Value;
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchInfoAsync error: " + e.ToString() );
			}
		}

		async Task GetMatchSettingsAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<string>( "Implementation", "GetMatchSettings", cts.Token, MatchID );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					if( !result.Error.Contains( "You are not a user of this match." ) )
						Log.Warning( "Error: " + result.Error );
					return;
				}

				if( matchSettingsBlockText == result.Value )
					return;
				matchSettingsBlockText = result.Value;

				var rootBlock = TextBlock.Parse( result.Value, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "Error: " + error );
					return;
				}

				//update controls
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					if( ControlListSettings == null )
						return;
					if( ControlListSettings.ParentContainer == null )
						return;

					var focusedControlName = ControlListSettings.ParentContainer.FocusedControl?.GetPathFromRoot() ?? "";

					//remove old items
					{
						foreach( var combo in comboSubscribedSelectedIndex )
							combo.SelectedIndexChanged -= Combo_SelectedIndexChanged;
						ControlListSettings.RemoveAllItems();
					}

					//create new controls
					foreach( var childBlock in rootBlock.Children )
					{
						//read settings

						var displayName = childBlock.GetAttribute( "DisplayName" );

						var settingName = childBlock.Name;
						var values = new List<string>();
						for( int n = 1; ; n++ )
						{
							var value = childBlock.GetAttribute( $"Value{n}" );
							if( string.IsNullOrEmpty( value ) )
								break;
							values.Add( value );
						}

						var currentValue = childBlock.GetAttribute( "CurrentValue" );

						//add control

						var control = (UIControl)ControlSettingsCombo.Clone();
						ControlListSettings.AddComponent( control );

						var text = control.GetComponent( "Text" ) as UIText;
						if( text != null )
						{
							if( !string.IsNullOrEmpty( displayName ) )
								text.Text = displayName;
							else
								text.Text = settingName;
						}

						var combo = control.GetComponent( "Combo" ) as UICombo;
						if( combo != null )
						{
							combo.AnyData = settingName;

							foreach( var value in values )
							{
								combo.AddItem( value );

								if( value == currentValue )
									combo.SelectedIndex = combo.Items.Count - 1;
							}

							combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
							comboSubscribedSelectedIndex.Add( combo );
						}

						control.Enabled = true;
					}

					if( !string.IsNullOrEmpty( focusedControlName ) )
					{
						var controlToFocus = ControlListSettings.ParentContainer.GetComponentByPath( focusedControlName ) as UIControl;
						if( controlToFocus != null )
							controlToFocus.Focus();
					}
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchSettingsAsync error: " + e.ToString() );
			}
		}

		void Combo_SelectedIndexChanged( UICombo sender )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			var newValue = sender.SelectedItem?.Value as string;
			if( newValue == null )
				return;

			var settingName = sender.AnyData as string;
			if( settingName == null )
				return;

			Task.Run( async delegate ()
			{
				try
				{
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var result = await client.CallMethodAsync( "Implementation", "SetMatchSetting", cts.Token, MatchID, settingName, newValue );
					if( !string.IsNullOrEmpty( result.Error ) )
						Log.Warning( "Error: " + result.Error );
				}
				catch( Exception e )
				{
					Log.Warning( "SetMatchSetting error: " + e.ToString() );
				}
			} );
		}

		async Task GetMatchDetailsAsync()
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var result = await client.CallMethodAsync<string>( "Implementation", "GetMatchDetails", cts.Token, MatchID );
				if( !string.IsNullOrEmpty( result.Error ) )
				{
					if( !result.Error.Contains( "You are not a user of this match." ) )
						Log.Warning( "Error: " + result.Error );
					return;
				}

				if( matchDetailsBlockText == result.Value )
					return;
				matchDetailsBlockText = result.Value;

				var rootBlock = TextBlock.Parse( result.Value, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "Error: " + error );
					return;
				}

				notReadyReason = rootBlock.GetAttribute( "NotReadyReason" );

				//update controls
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					if( ListParticipants != null )
					{
						var oldSelectedUserID = ListParticipants.SelectedItem?.Tag as long?;

						//remove old items
						ListParticipants.ClearItems();

						var participantsBlock = rootBlock.FindChild( "Participants" );
						if( participantsBlock != null )
						{
							foreach( var itemBlock in participantsBlock.Children )
							{
								long.TryParse( itemBlock.GetAttribute( "UserID" ), out var userID );
								var username = itemBlock.GetAttribute( "Username" );
								var botLevel = itemBlock.GetAttribute( "BotLevel" );

								var playerIndex = -1;
								if( itemBlock.AttributeExists( "PlayerIndex" ) )
									int.TryParse( itemBlock.GetAttribute( "PlayerIndex" ), out playerIndex );

								string text;

								if( userID > 0 )
									text = username;
								else if( userID < 0 )
									text = $"Bot {-userID} {botLevel}";
								else
									text = "Gap";

								if( playerIndex >= 0 )
									text += $" - Player {playerIndex + 1}";
								else
									text += " - Spectator";

								ListParticipants.AddItem( text, userID );
							}
						}

						if( oldSelectedUserID != null )
							ListParticipants.SelectItemByTag( oldSelectedUserID.Value );
					}
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "GetMatchDetailsAsync error: " + e.ToString() );
			}
		}

		bool GetSelectedParticipiant( out long participantID, out string text )
		{
			var selectedParticipantID = ListParticipants.SelectedItem?.Tag as long?;
			var selectedText = ListParticipants.SelectedItem?.Value as string;

			if( selectedParticipantID != null )
			{
				participantID = selectedParticipantID.Value;
				text = selectedText ?? "";
				return true;
			}
			else
			{
				participantID = 0;
				text = "";
				return false;
			}
		}

		public void ButtonStart_Click( NeoAxis.UIButton sender )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			Task.Run( async delegate ()
			{
				try
				{
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var result = await client.CallMethodAsync( "Matches", "UpdateMatch", cts.Token, MatchID, "Play", null, null );
					if( !string.IsNullOrEmpty( result.Error ) )
					{
						Log.Warning( "Error: " + result.Error );
						return;
					}
				}
				catch( Exception e )
				{
					Log.Warning( "UpdateMatch error: " + e.ToString() );
				}
			} );
		}

		public void ButtonKick_Click( NeoAxis.UIButton sender )
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			if( matchInfo == null )
				return;

			if( !GetSelectedParticipiant( out var selectedPartipicantID, out var selectedText ) )
				return;
			//can't be kick bot
			if( selectedPartipicantID < 0 )
				return;

			var text = $"Kick \"{selectedText}\" from the match?";

			MessageBoxWindow.Show( this, text, "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender2, EDialogResult result2, object anyData )
			{
				if( result2 == EDialogResult.Yes )
				{
					Task.Run( async delegate ()
					{
						//kick the player
						var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
						var result = await client.CallMethodAsync( "Matches", "RemoveMatchUser", cts.Token, MatchID, selectedPartipicantID );
						if( !string.IsNullOrEmpty( result.Error ) )
						{
							Log.Warning( "Error: " + result.Error );
							return;
						}
					} );
				}
			} );
		}

		void ChatSendMessage()
		{
			var client = CloudServiceClient.Client;
			if( client == null )
				return;

			var message = EditChatMessage.Text.Value.Trim();
			if( string.IsNullOrEmpty( message ) )
				return;

			if( matchInfo == null || matchInfo.ChatID == 0 )
				return;

			Task.Run( async delegate ()
			{
				try
				{
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var result = await client.CallMethodAsync<long>( "Chats", "NewMessage", cts.Token, matchInfo.ChatID, message, null, null );
					if( !string.IsNullOrEmpty( result.Error ) )
					{
						Log.Warning( "Error: " + result.Error );
						return;
					}

					EngineThreading.ExecuteFromMainThreadLater( delegate () { EditChatMessage.Text = ""; } );
				}
				catch( Exception e )
				{
					Log.Warning( "ChatSendMessage error: " + e.ToString() );
				}
			} );
		}

		public void EditChatMessage_KeyDownBefore( NeoAxis.UIControl sender, NeoAxis.KeyEvent e, ref bool handled )
		{
			if( e.Key == EKeys.Enter && EditChatMessage.Focused )
			{
				ChatSendMessage();
				handled = true;
			}
		}

		public void ButtonChatSend_Click( NeoAxis.UIButton sender )
		{
			ChatSendMessage();
		}

		private void Messages_ReceiveMessageString( ClientNetworkService_Messages sender2, string message, string data )
		{
			//handle messages from the server

			if( matchInfo == null )
				return;

			if( message == "Chat.NewMessage" )
			{
				if( long.TryParse( data, out var chatID ) )
				{
					if( matchInfo != null && matchInfo.ChatID == chatID )
						chatNewMessagesAvailable = true;
				}
			}

			if( message == "Match.StatusChanged" )
			{
				var rootBlock = TextBlock.Parse( data, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "Messages_ReceiveMessageString: Match.StatusChanged error: " + error );
					return;
				}

				long.TryParse( rootBlock.GetAttribute( "MatchID" ), out var matchID );
				var status = rootBlock.GetAttribute( "Status" );

				if( matchID == matchInfo.Id )
				{
					//fix status without receiving new match info
					matchInfo.Status = status;

					switch( status )
					{
					case "Deleted":
						{
							EngineThreading.ExecuteFromMainThreadLater( delegate ()
							{
								MessageBoxWindow.Show( this, "The match was deleted.", "Match Deleted", EMessageBoxButtons.OK, EMessageBoxIcon.None, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
								{
									RemoveFromParent( true );
								} );
							} );
						}
						break;

					case "Play":
						{
							EngineThreading.ExecuteFromMainThreadLater( delegate ()
							{
								//hide match window. remove after until subscribed to get events from the server about the match status
								Visible = false;

								//delete matches window
								//it's inside the event handler

								//open play screen
								MatchStatusChangedToPlay?.Invoke( this, matchInfo );

								//close window
								RemoveFromParent( true );
							} );
						}
						break;
					}
				}
			}

			if( message == "Match.UserRemoved" )
			{
				var rootBlock = TextBlock.Parse( data, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "Messages_ReceiveMessageString: Match.UserRemoved error: " + error );
					return;
				}

				long.TryParse( rootBlock.GetAttribute( "MatchID" ), out var matchID );
				long.TryParse( rootBlock.GetAttribute( "UserID" ), out var userID );

				if( matchInfo.Id == matchID && CloudServiceClient.ThisUserID == userID )
				{
					EngineThreading.ExecuteFromMainThreadLater( delegate ()
					{
						MessageBoxWindow.Show( this, "You are no longer in the match.", "Removed", EMessageBoxButtons.OK, EMessageBoxIcon.None, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
						{
							//close window
							RemoveFromParent( true );
						} );
					} );
				}
			}
		}

		async Task ChatGetNewMessagesAsync( object obj )
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var lastMessage = (Chats.Message)obj;
				var timeFrom = lastMessage != null ? lastMessage.CreationTime : DateTime.MinValue;
				var getFromEnd = lastMessage == null;

				var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var getMessagesResult = await client.CallMethodAsync<Chats.Message[]>( "Chats", "GetMessages", cts.Token, matchInfo.ChatID, new[] { "Enabled" }, timeFrom, DateTime.MaxValue, 200, getFromEnd );
				if( !string.IsNullOrEmpty( getMessagesResult.Error ) )
				{
					Log.Warning( "ChatGetNewMessagesAsync: Chats.GetMessages erorr: " + getMessagesResult.Error );
					chatGettingNewMessages = false;
					return;
				}
				var messages = getMessagesResult.Value;

				//update controls
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					if( ListChat != null )
					{
						foreach( var message in messages )
						{
							//find item with same message ID
							var found = false;
							foreach( var item in ListChat.Items )
							{
								var itemMessage = (Chats.Message)item.Tag;
								if( itemMessage.Id == message.Id )
								{
									found = true;
									break;
								}
							}

							if( !found )
							{
								ListChat.AddItem( message.Username + ": " + message.Text, message );
								ListChat.SelectedIndex = ListChat.Items.Count - 1;
								ListChat.EnsureVisible( ListChat.SelectedIndex );
							}
						}
					}

					chatGettingNewMessages = false;
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "ChatGetNewMessagesAsync error: " + e.ToString() );
				chatGettingNewMessages = false;
			}
		}
	}
}