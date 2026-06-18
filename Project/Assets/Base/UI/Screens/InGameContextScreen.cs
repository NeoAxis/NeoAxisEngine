// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// A basic depending on context UI screen.
	/// </summary>
	public class InGameContextScreen : NeoAxis.UIControl
	{
		Scene scene;
		GameMode gameMode;
		NetworkLogic networkLogic;

		//double colorAlpha;

		/////////////////////////////////////////

		UIControl GetWindow() { return GetComponent( "Window" ) as UIControl; }
		UIList GetListMessages() { return GetWindow().GetComponent( "List Messages" ) as UIList; }
		public UIEdit GetEditMessage() { return GetWindow().GetComponent( "Edit Message" ) as UIEdit; }
		UIButton GetButtonSendMessage() { return GetWindow().GetComponent( "Button Send Message" ) as UIButton; }

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
		public NetworkLogic NetworkLogic
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
				networkLogic = ClientUtility.GetNetworkLogic();
			}

			if( EngineApp.IsSimulation )
			{
				GetListMessages().ClearItems();

				if( SimulationAppClient.ConnectionNode?.Chat != null )
				{
					if( EnabledInHierarchyAndIsInstance )
					{
						//colorAlpha = 0;

						var defaultRoom = SimulationAppClient.ConnectionNode.Chat.GetRoom( "Default" );
						if( defaultRoom != null )
						{
							foreach( var message in defaultRoom.Messages )
								AddListMessageChatMessage( message );
						}

						SimulationAppClient.ConnectionNode.Chat.ReceivedRoomMessage += Chat_ReceivedRoomMessage;
					}
					else
						SimulationAppClient.ConnectionNode.Chat.ReceivedRoomMessage -= Chat_ReceivedRoomMessage;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//update controls
			if( EngineApp.IsSimulation )
			{
				ColorMultiplier = new ColorValue( 1, 1, 1, 1 );
				//fading
				//colorAlpha += delta * 2;
				//if( colorAlpha > 1 )
				//	colorAlpha = 1;
				//ColorMultiplier = new ColorValue( 1, 1, 1, colorAlpha );

				GetButtonSendMessage().ReadOnly = GetSendMessageText() == "";
			}
		}

		public void ButtonLeave_Click( NeoAxis.UIButton sender )
		{
			if( NetworkLogic != null )
			{
				var m = NetworkLogic.BeginNetworkMessageToServer( "TryLeaveWorld" );
				if( m != null )
					m.End();
			}

			RemoveFromParent( true );
		}

		public void ButtonClose_Click( NeoAxis.UIButton sender )
		{
			RemoveFromParent( true );
		}

		public void ButtonSendMessage_Click( NeoAxis.UIButton sender )
		{
			SendMessage();
		}

		public void EditMessage_KeyDownBefore( NeoAxis.UIControl sender, NeoAxis.KeyEvent e, ref bool handled )
		{
			if( e.Key == EKeys.Return )
			{
				var text = GetSendMessageText();
				if( text == "" )
				{
					RemoveFromParent( true );
					handled = true;
				}
				else if( SendMessage() )
					handled = true;
			}
		}

		string GetSendMessageText()
		{
			var text = GetEditMessage().Text.Value;
			return text.Trim( ' ', '\t' );
		}

		bool SendMessage()
		{
			var text = GetSendMessageText();
			if( text == "" )
				return false;

			var defaultRoom = SimulationAppClient.ConnectionNode?.Chat?.GetRoom( "Default" );
			if( defaultRoom == null )
				return false;

			SimulationAppClient.ConnectionNode.Chat.SayInRoom( defaultRoom, text );
			GetEditMessage().Text = "";

			return true;
		}

		public void AddListMessage( string text )
		{
			var list = GetListMessages();

			list.AddItem( text );

			if( SimulationAppClient.ConnectionNode != null )
			{
				while( list.Items.Count > SimulationAppClient.ConnectionNode.Chat.MaxMessagesInRoom )
					list.RemoveItem( 0 );
			}

			list.SelectedIndex = list.Items.Count - 1;
			list.EnsureVisible( list.Items.Count - 1 );
		}

		public delegate void AddListMessageChatMessageBeforeDelegate( InGameContextScreen sender, ClientNetworkService_Chat.RoomMessage message, ref bool skip );
		public event AddListMessageChatMessageBeforeDelegate AddListMessageChatMessageBefore;

		void AddListMessageChatMessage( ClientNetworkService_Chat.RoomMessage message )
		{
			var skip = false;
			AddListMessageChatMessageBefore?.Invoke( this, message, ref skip );
			if( skip )
				return;

			var chatService = SimulationAppClient.ConnectionNode?.Chat;

			var user = chatService.UsersService.GetUser( message.UserID );
			var userString = user != null ? user.Username : message.UserID.ToString();

			var str = $"{userString}: {message.Text}";
			AddListMessage( str );
		}

		private void Chat_ReceivedRoomMessage( ClientNetworkService_Chat sender, ClientNetworkService_Chat.RoomMessage message )
		{
			AddListMessageChatMessage( message );
		}
	}
}