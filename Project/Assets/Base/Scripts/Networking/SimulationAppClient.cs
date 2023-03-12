// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	public class SimulationAppClientNode : NetworkClientNode
	{
		//services
		ClientNetworkService_Messages messages;
		ClientNetworkService_Users users;
		ClientNetworkService_Chat chat;
		ClientNetworkService_Components components;
		ClientNetworkService_FileSync fileSync;

		//

		public SimulationAppClientNode()
		{
			//register messages service
			messages = new ClientNetworkService_Messages();
			RegisterService( messages );

			//register users service
			users = new ClientNetworkService_Users();
			RegisterService( users );

			//register chat service
			chat = new ClientNetworkService_Chat( users );
			RegisterService( chat );

			//register components service
			components = new ClientNetworkService_Components( users );
			RegisterService( components );

			//register file sync service
			fileSync = new ClientNetworkService_FileSync();
			RegisterService( fileSync );
		}

		public ClientNetworkService_Messages Messages
		{
			get { return messages; }
		}

		public ClientNetworkService_Users Users
		{
			get { return users; }
		}

		public ClientNetworkService_Chat Chat
		{
			get { return chat; }
		}

		public ClientNetworkService_Components Components
		{
			get { return components; }
		}

		public ClientNetworkService_FileSync FileSync
		{
			get { return fileSync; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SimulationAppClientAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsSimulation )
			{
				EngineApp.AppCreateAfter += delegate ()
				{
					SimulationAppClient.InitFromCommandLine();
				};
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The class for general management of the client.
	/// </summary>
	public static class SimulationAppClient
	{
		static NetworkModeEnum networkMode;
		static string serverAddress = "";
		static int serverPort;

		static SimulationAppClientNode client;
		static string LastError { get; set; } = "";

		static bool subscribedToEvents;

		/////////////////////////////////////////

		public enum NetworkModeEnum
		{
			CloudProject,
			Direct,
		}

		/////////////////////////////////////////

		public static bool Created
		{
			get { return client != null; }
		}

		public static NetworkModeEnum NetworkMode
		{
			get { return networkMode; }
		}

		public static string ServerAddress
		{
			get { return serverAddress; }
		}

		public static int ServerPort
		{
			get { return serverPort; }
		}

		public static SimulationAppClientNode Client
		{
			get { return client; }
		}

		static bool GetInitSettings( out string cloudModeVerificationCode, out string directModeUsername, out string directModePassword )
		{
			cloudModeVerificationCode = "";
			directModeUsername = "";
			directModePassword = "";

			if( SystemSettings.CommandLineParameters.TryGetValue( "-client", out var projectClient ) && projectClient == "1" )
			{
				//run Player from Launcher

				//get network mode
				if( SystemSettings.CommandLineParameters.TryGetValue( "-networkMode", out var networkModeString ) )
				{
					if( !Enum.TryParse( networkModeString, out networkMode ) )
					{
						Log.Fatal( "SimulationAppClient: Init: '-networkMode' unknown mode." );
						return false;
					}
				}

				//get serverAddress
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverAddress", out serverAddress ) )
				{
					Log.Fatal( "SimulationAppClient: Init: '-serverAddress' is not specified." );
					return false;
				}

				//get serverPort
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverPort", out var serverPortString ) )
				{
					Log.Fatal( "SimulationAppClient: Init: '-serverPort' is not specified." );
					return false;
				}
				if( !int.TryParse( serverPortString, out serverPort ) )
				{
					Log.Fatal( "SimulationAppClient: Init: '-serverPort' invalid data." );
					return false;
				}

				//get appContainer
				if( SystemSettings.CommandLineParameters.TryGetValue( "-appContainer", out var appContainer ) && appContainer == "1" )
					SystemSettings.AppContainer = true;

				if( networkMode == NetworkModeEnum.CloudProject )
				{
					//get verificationCode
					if( !SystemSettings.CommandLineParameters.TryGetValue( "-verificationCode", out cloudModeVerificationCode ) )
					{
						Log.Fatal( "SimulationAppClient: Init: '-verificationCode' is not specified." );
						return false;
					}
				}

				if( networkMode == NetworkModeEnum.Direct )
				{
					//get username
					if( SystemSettings.CommandLineParameters.TryGetValue( "-username", out var username ) )
						directModeUsername = username;

					//get password
					if( SystemSettings.CommandLineParameters.TryGetValue( "-password", out var passwordBase64 ) )
						directModePassword = Encoding.UTF8.GetString( Convert.FromBase64String( passwordBase64 ) );
					else
						directModePassword = "";
				}

				return true;
			}

			return false;
		}

		public static void InitFromCommandLine()
		{
			if( !GetInitSettings( out var cloudModeVerificationCode, out var directModeUsername, out var directModePassword ) )
				return;

			if( !ConnectToServer( cloudModeVerificationCode, directModeUsername, directModePassword, out var error ) )
			{
				LastError = error;
				//return;
			}
		}

		static bool ConnectToServer( string cloudModeVerificationCode, string directModeUsername, string directModePassword, out string error )
		{

			//!!!!связь может прерываться, может не сразу подключиться. постоянный "Connecting..." как в ланчере


			Destroy();

			error = "";

			if( !subscribedToEvents )
			{
				EngineApp.Tick += EngineApp_Tick;
				EngineApp.AppDestroy += EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI += SimulationApp_MainViewportRenderUI;
				subscribedToEvents = true;
			}

			client = new SimulationAppClientNode();
			client.ProtocolError += Client_ProtocolError;
			client.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			client.Messages.ReceiveMessage += Messages_ReceiveMessage;
			client.Components.SceneCreateBegin += Components_SceneCreateBegin;
			client.Components.SceneCreateEnd += Components_SceneCreateEnd;
			client.Components.SceneDestroy += Components_SceneDestroy;

			var block = new TextBlock();

			if( networkMode == NetworkModeEnum.CloudProject )
			{
				block.SetAttribute( "VerificationCode", cloudModeVerificationCode );
			}
			else
			{
				block.SetAttribute( "Username", directModeUsername );
				block.SetAttribute( "Password", directModePassword );
			}

			var loginData = block.DumpToString();

			//!!!!connectionTimeout
			if( !client.BeginConnect( serverAddress, serverPort, EngineInfo.Version, loginData, 10, out error ) )
			{
				Destroy();
				return false;
			}

			//configure the app
			EngineApp.EnginePauseWhenApplicationIsNotActive = false;

			return true;
		}

		public static bool ConnectDirect( string address, int port, string username, string password, out string error )
		{
			Destroy();

			networkMode = NetworkModeEnum.Direct;
			serverAddress = address;
			serverPort = port;

			if( !ConnectToServer( "", username, password, out error ) )
				return false;

			return true;
		}

		private static void Client_ProtocolError( NetworkClientNode sender, string message )
		{
			Log.Warning( "SimulationAppClient: Protocol error: " + message );
		}

		public static void Destroy()
		{
			if( client != null )
			{
				client.ProtocolError -= Client_ProtocolError;
				client.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				client.Messages.ReceiveMessage -= Messages_ReceiveMessage;
				client.Components.SceneCreateBegin -= Components_SceneCreateBegin;
				client.Components.SceneCreateEnd -= Components_SceneCreateEnd;
				client.Components.SceneDestroy -= Components_SceneDestroy;

				client.Dispose();
				client = null;

				//restore the app settings
				EngineApp.EnginePauseWhenApplicationIsNotActive = true;
			}

			if( subscribedToEvents )
			{
				EngineApp.Tick -= EngineApp_Tick;
				EngineApp.AppDestroy -= EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI -= SimulationApp_MainViewportRenderUI;
				subscribedToEvents = false;
			}
		}

		private static void EngineApp_AppDestroy()
		{
			Destroy();
		}

		private static void EngineApp_Tick( float delta )
		{
			client?.Update();
		}

		private static void SimulationApp_MainViewportRenderUI()
		{
			if( SimulationApp.NetworkLogging )
			{
				var viewport = SimulationApp.MainViewport;

				var lines = new List<string>();

				lines.Add( "SimulationAppClient is initialized." );
				if( client != null )
				{
					lines.Add( "SimulationAppClient is created." );
					lines.Add( "Connection status: " + client.Status.ToString() );

					var fileSyncStatus = client.FileSync.Status;
					lines.Add( "File sync status: " + fileSyncStatus.Status.ToString() );
					if( fileSyncStatus.Status == ClientNetworkService_FileSync.StatusEnum.Error )
						lines.Add( "File sync error: " + fileSyncStatus.Error );
				}

				if( !string.IsNullOrEmpty( LastError ) )
				{
					lines.Add( "" );
					lines.Add( "Last error: " + LastError );
				}

				CanvasRendererUtility.AddTextLinesWithShadow( viewport, lines, new Rectangle( 0.02, 0.02, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			}
		}

		static void Client_ConnectionStatusChanged( NetworkClientNode sender, NetworkStatus status )
		{
			//ScreenMessages.Add( string.Format( "Connection status changed: {0}", status.ToString() ) );

			switch( status )
			{
			case NetworkStatus.Connected:
				{

					//!!!!не обновлять если это в dev папке
					//!!!!это sync в процессе работы
					//client.FileSync.StartUpdate();

				}
				break;

			case NetworkStatus.Connecting:
				break;

			case NetworkStatus.Disconnected:
				break;
			}
		}

		private static void Messages_ReceiveMessage( ClientNetworkService_Messages sender, string message, string data )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", message ) );

			if( message == "Verified" )
			{
				//do something useful after verification of a new connected user
			}
			else if( message == "ScreenMessagesAdd" )
				ScreenMessages.Add( data );
		}

		private static void Components_SceneCreateBegin( ClientNetworkService_Components sender, string sceneInfo )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateBegin" ) );
		}

		private static void Components_SceneCreateEnd( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateEnd" ) );

			var scene = client.Components.Scene;
			if( scene != null )
				SimulationApp.NetworkClientSceneCreated( scene );
		}

		private static void Components_SceneDestroy( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneDestroy" ) );

			SimulationApp.NetworkClientSceneDestroyed();
		}
	}
}
