// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	public class MultiplayerJoinWindow : UIWindow
	{
		[EngineConfig( "MultiplayerJoinWindow", "lastAddress" )]
		public static string lastAddress { get; set; } = "localhost";
		[EngineConfig( "MultiplayerJoinWindow", "lastPort" )]
		public static int lastPort { get; set; } = 52000;
		[EngineConfig( "MultiplayerJoinWindow", "lastUsername" )]
		public static string lastUsername { get; set; } = "";
		[EngineConfig( "MultiplayerJoinWindow", "lastPassword" )]
		public static string lastPassword { get; set; } = "";

		MessageBoxWindow.ResultData connectingWindowData;

		/////////////////////////////////////////

		UIEdit GetEditAddress() { return GetComponent<UIEdit>( "Edit Address" ); }
		UIEdit GetEditPort() { return GetComponent<UIEdit>( "Edit Port" ); }
		UIList GetListServers() { return GetComponent<UIList>( "List Servers" ); }
		UIEdit GetEditUsername() { return GetComponent<UIEdit>( "Edit Username" ); }
		UIEdit GetEditPassword() { return GetComponent<UIEdit>( "Edit Password" ); }
		UIButton GetButtonJoin() { return GetComponent<UIButton>( "Button Join" ); }
		//UIButton GetButtonClose() { return GetComponent<UIButton>( "Button Close" ); }

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//register [EngineConfig] fields, properties
			EngineConfig.RegisterClassParameters( typeof( MultiplayerJoinWindow ) );

			UpdateServerList();

			if( string.IsNullOrEmpty( lastUsername ) )
			{
				var random = new FastRandom();
				lastUsername = "Guest" + random.Next( 1000 ).ToString( "D03" );
			}

			GetEditAddress().Text = lastAddress;
			GetEditPort().Text = lastPort.ToString();
			GetEditUsername().Text = lastUsername;
			GetEditPassword().Text = lastPassword;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				GetButtonJoin().ReadOnly = string.IsNullOrEmpty( GetAddress() ) || string.IsNullOrEmpty( GetUsername() );

				//cancel connecting
				if( connectingWindowData != null && connectingWindowData.Result != EDialogResult.None )
				{
					connectingWindowData = null;

					if( SimulationAppClient.Client != null )
						SimulationAppClient.Client.ConnectionStatusChanged -= ClientConnectionStatusChanged;
					SimulationAppClient.Destroy();
				}
			}
		}

		public void ButtonClose_Click( NeoAxis.UIButton sender )
		{
			Dispose();
		}

		protected override void OnDisabledInSimulation()
		{
			if( SimulationAppClient.Client != null )
				SimulationAppClient.Client.ConnectionStatusChanged -= ClientConnectionStatusChanged;

			base.OnDisabledInSimulation();
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

		//!!!!call from OnUpdate
		void UpdateServerList()
		{
			var list = GetListServers();
			if( list != null )
			{
				//!!!!find servers in local network

				list.Items.Add( "localhost" );
			}
		}

		string GetAddress()
		{
			return GetEditAddress().Text.Value.Trim( ' ', '\t' );
		}

		int GetPort()
		{
			var text = GetEditPort().Text.Value.Trim( ' ', '\t' );
			if( int.TryParse( text, out var result ) )
				return result;
			return 0;
		}

		string GetUsername()
		{
			return GetEditUsername().Text.Value;
		}

		string GetPassword()
		{
			return GetEditPassword().Text.Value;
		}

		public void EditAddress_TextChanged( NeoAxis.UIControl obj )
		{
			//lastAddress = GetAddress();
		}

		public void EditPort_TextChanged( NeoAxis.UIControl obj )
		{
			//lastPort = GetPort();
		}

		public void EditUsername_TextChanged( NeoAxis.UIControl obj )
		{
			//lastUsername = GetUsername();
		}

		public void EditPassword_TextChanged( NeoAxis.UIControl obj )
		{
			//lastPassword = GetPassword();
		}

		public void ButtonJoin_Click( NeoAxis.UIButton sender )
		{
			lastAddress = GetAddress();
			lastPort = GetPort();
			lastUsername = GetUsername();
			lastPassword = GetPassword();

			//if( string.IsNullOrEmpty( Username ) )
			//{
			//	usernameEdit?.Focus();
			//	MessageBoxWindow.Show( this, "Username is not specified.", "Error", EMessageBoxButtons.OK );
			//	return;
			//}

			SimulationAppClient.ConnectDirect( GetAddress(), GetPort(), GetUsername(), GetPassword(), out var error );
			if( !string.IsNullOrEmpty( error ) )
			{
				MessageBoxWindow.Show( this, error, "Error", EMessageBoxButtons.OK, EMessageBoxIcon.Warning );
				return;
			}

			SimulationAppClient.Client.ConnectionStatusChanged += ClientConnectionStatusChanged;

			connectingWindowData = MessageBoxWindow.Show( this, "Connecting to the server...", "Info", EMessageBoxButtons.Cancel );
		}

		void ClientConnectionStatusChanged( NetworkClientNode sender, NetworkStatus status )
		{
			switch( status )
			{
			case NetworkStatus.Disconnected:
				{
					//close connecting message
					connectingWindowData?.Window.Dispose();
					connectingWindowData = null;

					//show error message
					string text = "Unable to connect.";
					if( !string.IsNullOrEmpty( sender.DisconnectionReason ) )
						text += " " + sender.DisconnectionReason;
					MessageBoxWindow.Show( this, text, "Error", EMessageBoxButtons.OK );

					//destroy the client
					SimulationAppClient.Destroy();
				}
				break;

			case NetworkStatus.Connected:
				{
					//close connecting message
					connectingWindowData?.Window.Dispose();
					connectingWindowData = null;

					//next, wait messages from the server (sync a scene)
				}
				break;
			}
		}
	}
}