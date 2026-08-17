// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	/// <summary>
	/// This class manages the client of the local client-server connection (not cloud service).
	/// </summary>
	public static class SimulationAppClient
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 60.0;

		static ConnectionModeEnum connectionMode;
		static string serverAddress = "";
		static int serverPort;

		static SimulationAppClientNode connectionNode;
		static string LastError { get; set; } = "";

		static bool subscribedToEvents;

		//static DateTime updateLastTime;

		static string helloFromServerMessage;

		/////////////////////////////////////////

		public class SimulationAppClientNode : ClientNode
		{
			//services
			ClientNetworkService_CloudFunctions cloudFunctions;
			ClientNetworkService_Messages messages;
			ClientNetworkService_Users users;
			ClientNetworkService_Chat chat;
			ClientNetworkService_Components components;

			//

			public SimulationAppClientNode()
			{
				//register cloud functions service
				cloudFunctions = new ClientNetworkService_CloudFunctions();
				RegisterService( cloudFunctions );

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
			}

			public ClientNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
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
		}

		/////////////////////////////////////////

		public static bool Created
		{
			get { return connectionNode != null; }
		}

		public static ConnectionModeEnum ConnectionMode
		{
			get { return connectionMode; }
		}

		public static string ServerAddress
		{
			get { return serverAddress; }
		}

		public static int ServerPort
		{
			get { return serverPort; }
		}

		public static SimulationAppClientNode ConnectionNode
		{
			get { return connectionNode; }
		}

		public static string HelloFromServerMessage
		{
			get { return helloFromServerMessage; }
		}

		static bool ConnectToServer( string cloudModeVerificationCode, string directModeUsername, string directModePassword, out string error )
		{
			Destroy();

			error = "";

			if( !subscribedToEvents )
			{
				EngineApp.Tick += EngineApp_Tick;
				EngineApp.AppDestroy += EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI += SimulationApp_MainViewportRenderUI;
				subscribedToEvents = true;
			}

			connectionNode = new SimulationAppClientNode();
			connectionNode.ProtocolError += Client_ProtocolError;
			connectionNode.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			connectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
			connectionNode.Components.SceneCreateBegin += Components_SceneCreateBegin;
			connectionNode.Components.SceneCreateEnd += Components_SceneCreateEnd;
			connectionNode.Components.SceneDestroy += Components_SceneDestroy;

			var block = new TextBlock();

			if( connectionMode == ConnectionModeEnum.Cloud )
			{
				block.SetAttribute( "VerificationCode", cloudModeVerificationCode );
			}
			else
			{
				block.SetAttribute( "Username", directModeUsername );
				block.SetAttribute( "Password", directModePassword );
			}

			var loginData = block.DumpToString();

			if( !connectionNode.BeginConnect( false, serverAddress, serverPort, 0, EngineInfo.Version, loginData, out error ) )
			{
				Destroy();
				return false;
			}

			//configure the app
			EngineApp.EnginePauseWhenApplicationIsNotActive = false;

			return true;
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
					if( !Enum.TryParse( networkModeString, out connectionMode ) )
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

				Log.InvisibleInfo( "SimulationAppClient: GetInitSettings: Network mode: " + connectionMode.ToString() );
				Log.InvisibleInfo( "SimulationAppClient: GetInitSettings: Server address: " + serverAddress );
				Log.InvisibleInfo( "SimulationAppClient: GetInitSettings: Server port: " + serverPort.ToString() );

				if( connectionMode == ConnectionModeEnum.Cloud )
				{
					//get verificationCode
					if( !SystemSettings.CommandLineParameters.TryGetValue( "-verificationCode", out cloudModeVerificationCode ) )
					{
						Log.Fatal( "SimulationAppClient: Init: '-verificationCode' is not specified." );
						return false;
					}
				}

				if( connectionMode == ConnectionModeEnum.Direct )
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
				return;
			}
		}

		public static bool ConnectDirect( string address, int port, string username, string password, out string error )
		{
			Destroy();

			connectionMode = ConnectionModeEnum.Direct;
			serverAddress = address;
			serverPort = port;

			if( !ConnectToServer( "", username, password, out error ) )
				return false;

			return true;
		}

		private static void Client_ProtocolError( ClientNode sender, string message )
		{
			Log.Warning( "SimulationAppClient: Protocol error: " + message );
		}

		public static void Destroy()
		{
			if( connectionNode != null )
			{
				connectionNode.ProtocolError -= Client_ProtocolError;
				connectionNode.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				connectionNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;
				connectionNode.Components.SceneCreateBegin -= Components_SceneCreateBegin;
				connectionNode.Components.SceneCreateEnd -= Components_SceneCreateEnd;
				connectionNode.Components.SceneDestroy -= Components_SceneDestroy;

				connectionNode.Dispose();
				connectionNode = null;

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
			var utcNow = DateTime.UtcNow;
				connectionNode?.Update( utcNow );

			//var utcNow = DateTime.UtcNow;
			//if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			//{
			//	updateLastTime = utcNow;
			//	connectionNode?.Update( utcNow );
			//}
		}

		private static void SimulationApp_MainViewportRenderUI()
		{
			if( SimulationApp.NetworkLogging )
			{
				var viewport = SimulationApp.MainViewport;

				var lines = new List<string>();

				lines.Add( "SimulationAppClient is initialized." );
				if( connectionNode != null )
				{
					lines.Add( "SimulationAppClient is created." );
					lines.Add( "Connection status: " + connectionNode.Status.ToString() );
				}

				if( !string.IsNullOrEmpty( LastError ) )
				{
					lines.Add( "" );
					lines.Add( "Last error: " + LastError );
				}

				CanvasRendererUtility.AddTextLinesWithShadow( viewport, lines, new Rectangle( 0.02, 0.02, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			}
		}

		static void Client_ConnectionStatusChanged( ClientNode sender )
		{
			//ScreenMessages.Add( string.Format( "Connection status changed: {0}", status.ToString() ) );

			switch( sender.Status )
			{
			case NetworkStatus.Connected:
				{
					//client.FileSync.StartUpdate();
				}
				break;

			case NetworkStatus.Connecting:
				break;

			case NetworkStatus.Disconnected:
				break;
			}
		}

		private static void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", message ) );

			if( message == "HelloFromServerMessage" )
				helloFromServerMessage = data;

			if( message == "ScreenMessagesAdd" )
				ScreenMessages.Add( data );
		}

		private static void Components_SceneCreateBegin( ClientNetworkService_Components sender, string sceneInfo )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateBegin" ) );
		}

		private static void Components_SceneCreateEnd( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateEnd" ) );

			//set active scene to the app, load Play screen
			var scene = connectionNode.Components.Scene;
			if( scene != null )
				SimulationApp.NetworkClientSceneCreated( scene );
		}

		private static void Components_SceneDestroy( ClientNetworkService_Components sender, bool anotherSceneWillLoaded )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneDestroy" ) );

			SimulationApp.NetworkClientSceneDestroyed( anotherSceneWillLoaded );
		}

		internal static void RegisterEngineConsoleCommands()
		{
			EngineConsole.AddCommand( "StartNetworkProfiler", delegate ( string arguments )
			{
				var workingTime = float.MaxValue;
				if( !string.IsNullOrEmpty( arguments ) && float.TryParse( arguments, out var v ) )
					workingTime = v;
				ConnectionNode?.ProfilerStart( workingTime );

			}, "Starts the network profiler. Optionally, you can specify working time in seconds." );

			EngineConsole.AddCommand( "StopNetworkProfiler", delegate ( string arguments )
			{
				ConnectionNode?.ProfilerStop( true );
			}, "Stops the network profiler." );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SimulationAppClientAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsSimulation )
			{
				EngineApp.AppCreateAfter += delegate ()
				{
					SimulationAppClient.InitFromCommandLine();
					SimulationAppClient.RegisterEngineConsoleCommands();
				};
			}
		}
	}
}
