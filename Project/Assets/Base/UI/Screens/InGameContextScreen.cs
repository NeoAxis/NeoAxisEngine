// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		//!!!!
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
				GetListMessages().Items.Clear();

				if( SimulationAppClient.Client != null )
				{
					if( EnabledInHierarchyAndIsInstance )
					{
						//!!!!
						//colorAlpha = 0;

						foreach( var message in SimulationAppClient.Client.Chat.LastMessages )
							AddListMessageChatMessage( message.FromUser, message.Text );

						SimulationAppClient.Client.Chat.ReceiveText += Chat_ReceiveText;
					}
					else
						SimulationAppClient.Client.Chat.ReceiveText -= Chat_ReceiveText;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//update controls
			if( EngineApp.IsSimulation )
			{
				//!!!!
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
				NetworkLogic.BeginNetworkMessageToServer( "TryLeaveWorld" );
				NetworkLogic.EndNetworkMessage();
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

			SimulationAppClient.Client?.Chat.SayToEveryone( text );

			GetEditMessage().Text = "";

			return true;
		}

		public void AddListMessage( string text )
		{
			var list = GetListMessages();

			list.Items.Add( text );

			if( list.Items.Count > ClientNetworkService_Chat.MaxLastMessages )
				list.Items.RemoveAt( 0 );

			list.SelectedIndex = list.Items.Count - 1;
			list.EnsureVisible( list.Items.Count - 1 );
		}

		public delegate void AddListMessageChatMessageBeforeDelegate( InGameContextScreen sender, ClientNetworkService_Users.UserInfo fromUser, string text, ref bool skip );
		public event AddListMessageChatMessageBeforeDelegate AddListMessageChatMessageBefore;

		void AddListMessageChatMessage( ClientNetworkService_Users.UserInfo fromUser, string text )
		{
			var skip = false;
			AddListMessageChatMessageBefore?.Invoke( this, fromUser, text, ref skip );
			if( skip )
				return;

			var str = $"{fromUser.Username}: {text}";
			AddListMessage( str );
		}

		private void Chat_ReceiveText( ClientNetworkService_Chat sender, ClientNetworkService_Users.UserInfo fromUser, string text )
		{
			AddListMessageChatMessage( fromUser, text );
		}
	}
}