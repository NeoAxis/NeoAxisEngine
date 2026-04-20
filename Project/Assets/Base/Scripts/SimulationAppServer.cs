// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !CLIENT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	/// <summary>
	/// The class for general management of the server.
	/// </summary>
	public static class SimulationAppServer
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 60.0;

		static int serverPort;
		//!!!!static long projectID;
		static string directModePassword = "";

		static SimulationAppServerNode serverNode;
		static string LastError { get; set; } = "";

		//!!!!
		//static DateTime touchUserVerificationCodesLastTime;

		//new client initial settings
		public static double ConnectionDefaultMaxLifetime = 31536000;
		public static double ConnectionKeepAliveTime = 60;
		public static bool ConnectionAllowReconnect = false;//!!!!true;
		public static string HelloFromServerMessage = "Hello from the server!";

		static DateTime updateLastTime;

		/////////////////////////////////////////

		public class ClientData
		{
			public ConnectionModeEnum ConnectionMode;
			public string VerificationCode;
			//public bool Verified;
			public ServerNode.Client Client;

			//!!!!
			////the ability to update user settings
			//public DateTime TouchUserVerificationCodeLastTime;

			//Direct mode
			public string Username;
		}

		/////////////////////////////////////////

		//useful events

		public delegate void ClientDisconnectedDelegate( ServerNode.Client client );
		public static event ClientDisconnectedDelegate ClientDisconnected;

		public delegate void ClientConnectedDelegate( ServerNode.Client client );
		public static event ClientConnectedDelegate ClientConnected;

		/////////////////////////////////////////

		public class SimulationAppServerNode : ServerNode
		{
			//services
			ServerNetworkService_CloudFunctions cloudFunctions;
			ServerNetworkService_Messages messages;
			ServerNetworkService_Users users;
			ServerNetworkService_Chat chat;
			ServerNetworkService_Components components;

			//

			public SimulationAppServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime, double keepAliveTime, bool allowReconnect, string fullPathToDatabase, bool databaseReadOnly, string projectDirectory, out string error )
				: base( serverName, serverVersion, maxConnections, defaultMaxLifetime, keepAliveTime, allowReconnect )
			{
				//register cloud functions service
				cloudFunctions = new ServerNetworkService_CloudFunctions( fullPathToDatabase, databaseReadOnly, projectDirectory, out error );
				RegisterService( cloudFunctions );

				//register messages service
				messages = new ServerNetworkService_Messages();
				RegisterService( messages );

				//register users service
				users = new ServerNetworkService_Users();
				RegisterService( users );

				//register chat service
				chat = new ServerNetworkService_Chat( users );
				RegisterService( chat );

				//register components service
				components = new ServerNetworkService_Components( users );
				RegisterService( components );
			}

			public ServerNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
			}

			public ServerNetworkService_Messages Messages
			{
				get { return messages; }
			}

			public ServerNetworkService_Users Users
			{
				get { return users; }
			}

			public ServerNetworkService_Chat Chat
			{
				get { return chat; }
			}

			public ServerNetworkService_Components Components
			{
				get { return components; }
			}
		}

		/////////////////////////////////////////

		public static int ServerPort
		{
			get { return serverPort; }
		}

		//!!!!
		//public static long ProjectID
		//{
		//	get { return projectID; }
		//}

		public static string DirectModePassword
		{
			get { return directModePassword; }
		}

		public static bool Created
		{
			get { return ServerNode != null; }
		}

		public static SimulationAppServerNode ServerNode
		{
			get { return serverNode; }
		}

		public static void Init()
		{
			if( SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) && projectServer == "1" )
			{
				//!!!!new commented
				//EngineInfo.SetEngineMode( EngineInfo.EngineModeEnum.CloudServer, null );

				EngineApp.AppDestroy += EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI += SimulationApp_MainViewportRenderUI;


				//!!!!

				////get network mode
				//if( SystemSettings.CommandLineParameters.TryGetValue( "-networkMode", out var networkModeString ) )
				//{
				//	if( !Enum.TryParse( networkModeString, out connectionMode ) )
				//	{
				//		Log.Fatal( "SimulationAppServer: Init: '-networkMode' unknown mode." );
				//		return;
				//	}
				//}


				//get server port
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverPort", out var serverPortString ) )
				{
					Log.Fatal( "SimulationAppServer: Init: '-serverPort' is not specified." );
					return;
				}
				if( !int.TryParse( serverPortString, out serverPort ) )
				{
					Log.Fatal( "SimulationAppServer: Init: '-serverPort' invalid value." );
					return;

				}

				//!!!!

				//if( connectionMode == ConnectionModeEnum.Cloud )
				//{
				//	//get project ID
				//	if( !SystemSettings.CommandLineParameters.TryGetValue( "-projectID", out var projectIDString ) )
				//	{
				//		Log.Fatal( "SimulationAppServer: Init: '-projectID' is not specified." );
				//		return;
				//	}
				//	if( !long.TryParse( projectIDString, out projectID ) )
				//	{
				//		Log.Fatal( "SimulationAppServer: Init: '-projectID' invalid value." );
				//		return;
				//	}
				//}

				//if( connectionMode == ConnectionModeEnum.Direct )
				{
					//get password
					if( SystemSettings.CommandLineParameters.TryGetValue( "-password", out var passwordBase64 ) )
						directModePassword = Encoding.UTF8.GetString( Convert.FromBase64String( passwordBase64 ) );
					else
						directModePassword = "";
				}

				if( !RunOneSceneConfiguration( serverPort, out var error ) )
					LastError = error;

				EngineApp.Tick += EngineApp_Tick;
			}
		}

		private static void EngineApp_AppDestroy()
		{
			DestroyServer();
		}

		static bool CreateServer( out string error )
		{

			//!!!!
			var fullPathToDatabase = "";
			var databaseReadOnly = true;
			var projectDirectory = "";

			serverNode = new SimulationAppServerNode( "NeoAxis Project Server", EngineInfo.Version, 100000, ConnectionDefaultMaxLifetime, ConnectionKeepAliveTime, ConnectionAllowReconnect, fullPathToDatabase, databaseReadOnly, projectDirectory, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			serverNode.ProtocolError += Server_ProtocolError;
			serverNode.IncomingConnectionApproval += Server_IncomingConnectionApproval;
			serverNode.ClientBeforeStatusChangeToConnected += ServerNode_ClientBeforeStatusChangeToConnected;
			serverNode.ClientStatusChanged += Server_ClientStatusChanged;
			serverNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;

			//!!!!need?
			//configure Chat service
			//if( ChatDefaultRoom )
			serverNode.Chat.CreateRoom( "Default" );
			//serverNode.Chat.AllowPrivateMessages = ChatPrivateMessages;

			if( !serverNode.BeginListen( false, null, serverPort, 0, out error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			return true;
		}

		private static void Server_ProtocolError( ServerNode sender, ServerNode.Client client, string message )
		{
			Log.Warning( "SimulationAppServer: Protocol error: " + message );
		}

		static void DestroyServer()
		{
			if( serverNode != null )
			{
				serverNode.ProtocolError -= Server_ProtocolError;
				serverNode.IncomingConnectionApproval -= Server_IncomingConnectionApproval;
				serverNode.ClientBeforeStatusChangeToConnected -= ServerNode_ClientBeforeStatusChangeToConnected;
				serverNode.ClientStatusChanged -= Server_ClientStatusChanged;
				serverNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;

				serverNode.Dispose();
				serverNode = null;
			}
		}

		private static void Server_IncomingConnectionApproval( ServerNode sender, ServerNode.Client client, ServerNode.IncomingConnectionApproveResult approveResult )
		{
			//if( !AllowToConnectNewClients )
			//{
			//	rejectReason = "The server does not allow new users to connect now.";
			//	return false;
			//}

			try
			{
				var block = TextBlock.Parse( client.LoginData, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					approveResult.Reject( "Invalid text block format. " + error );
					return;
				}

				if( Enum.TryParse<CloudUserRole>( block.GetAttribute( "UserRole" ), out var userRole ) )
					client.UserRole = userRole;

				var clientData = new ClientData();
				clientData.Client = client;
				client.Tag = clientData;

				clientData.ConnectionMode = block.AttributeExists( "VerificationCode" ) ? ConnectionModeEnum.Cloud : ConnectionModeEnum.Direct;
				if( clientData.ConnectionMode == ConnectionModeEnum.Cloud )
				{
					clientData.VerificationCode = block.GetAttribute( "VerificationCode" );

					//start checking Cloud verification code
					Task.Run( async delegate ()
					{
						var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
						var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;

						using var cts = new CancellationTokenSource( new TimeSpan( 0, 0, 30 ) );
						var result = await CloudServiceFunctions.AccessGetUserByVerificationCodeAsync( projectID, client.UserRole, clientData.VerificationCode, false, serverCheckCode, cts.Token );

						if( string.IsNullOrEmpty( result.Error ) )
						{
							approveResult.Reject( result.Error );
						}
						else
						{
							//set userID, username
							client.LoginDataUserID = result.UserID;
							client.LoginDataUsername = result.Username;

							approveResult.Approve();

							//!!!!
							//clientData.TouchUserVerificationCodeLastTime = DateTime.UtcNow;
						}
					} );
				}
				else
				{
					clientData.Username = block.GetAttribute( "Username" );
					if( directModePassword != block.GetAttribute( "Password" ) )
					{
						approveResult.Reject( "Invalid password." );
						return;
					}

					client.LoginDataUserID = serverNode.Users.GetDirectConnectionFreeUserID();

					var username = clientData.Username;
					if( string.IsNullOrEmpty( username ) )
						username = "User" + client.LoginDataUserID.ToString();
					client.LoginDataUsername = username;

					approveResult.Approve();
				}
			}
			catch( Exception e )
			{
				approveResult.Reject( "Exception: " + e.Message );
				return;
			}
		}

		private static void ServerNode_ClientBeforeStatusChangeToConnected( ServerNode sender, ServerNode.Client client )
		{
			var clientData = GetClientData( client );

			//update max lifetime
			client.MaxLifetime = ConnectionDefaultMaxLifetime;

			////configure collecting connection statistics
			//client.AggregateConnectionStatistics = AggregatedConnectionStatistics;

			//send hello message
			serverNode.Messages.SendToClient( client, "HelloFromServerMessage", HelloFromServerMessage );

			//add to users service, with sending events to clients
			var user = serverNode.Users.AddUser( client );

			//!!!!?
			//add user to Default chat room
			var defaultRoom = serverNode.Chat.GetRoom( "Default" );
			if( defaultRoom != null )
				serverNode.Chat.AddUserToRoom( defaultRoom, user );
		}

		static bool RunOneSceneConfiguration( int port, out string error )
		{
			error = "";

			DestroyServer();

			//configure the app
			EngineApp.EnginePauseWhenApplicationIsNotActive = false;

			//initialize networking to connect clients

			if( !CreateServer( out error ) )
				return false;

			return true;
		}

		private static void EngineApp_Tick( float delta )
		{
			var utcNow = DateTime.UtcNow;
			if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			{
				updateLastTime = utcNow;

				serverNode?.Update( utcNow );

				//!!!!
				//if( serverNode != null && connectionMode == ConnectionModeEnum.Cloud )
				//	TouchUserVerificationCodes();
			}
		}

		private static void SimulationApp_MainViewportRenderUI()
		{
			if( SimulationApp.NetworkLogging )
			{
				var viewport = SimulationApp.MainViewport;

				var lines = new List<string>();

				lines.Add( "Project server manager is initialized." );
				if( serverNode != null )
					lines.Add( "Project server is created." );

				if( !string.IsNullOrEmpty( LastError ) )
				{
					lines.Add( "" );
					lines.Add( "Last error: " + LastError );
				}

				CanvasRendererUtility.AddTextLinesWithShadow( viewport, lines, new Rectangle( 0.02, 0.02, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			}
		}

		private static void Server_ClientStatusChanged( ServerNode sender, ServerNode.Client client, string message )
		{
			if( SimulationApp.NetworkLogging )
				Log.Info( $"Client connection status changed for {client.LoginDataUserID}; {client.Status}; {message}" );

			switch( client.Status )
			{
			case NetworkStatus.Connected:
				{
					var clientData = GetClientData( client );

					//send initial scene state to the client
					if( serverNode.Components.Scene != null )
					{
						var clientItem = serverNode.Components.GetClientItem( client );
						if( clientData != null )
							serverNode.Components.SendSceneCreate( clientItem );
					}

					ClientConnected?.Invoke( client );
				}
				break;

			case NetworkStatus.Disconnected:
				{
					var clientData = GetClientData( client );

					//notify components service
					var clientItem = serverNode.Components.GetClientItem( client );
					if( clientData != null )
						serverNode.Components.ClientDisconnected( clientItem );

					//remove user
					var user = serverNode.Users.GetUser( client );
					if( user != null )
					{
						serverNode.Chat.RemoveUser( user );
						serverNode.Users.RemoveUser( user );
					}

					ClientDisconnected?.Invoke( client );
				}
				break;
			}
		}

		//!!!!
		//static void GeneralManagerExecuteRequestAvatarSettingsProcessed( GeneralManagerExecuteCommand command )
		//{
		//	var client = (ServerNode.Client)command.Tag;

		//	if( !string.IsNullOrEmpty( command.Result.Error ) )
		//	{
		//		serverNode.DisconnectClient( client, command.Result.Error );
		//		return;
		//	}

		//	var block = command.Result.Data;
		//	var avatar = block.GetAttribute( "Avatar" );

		//	if( serverNode != null )
		//		serverNode.Messages.SendToClient( client, "AvatarSettings", avatar );
		//}

		private static void Messages_ReceiveMessageString( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data )
		{
			var clientData = GetClientData( client );
			//if( !clientData.Verified )
			//	return;

			if( SimulationApp.NetworkLogging )
				Log.Info( string.Format( "Message from {0}: {1}", client.LoginDataUserID, message ) );

			if( serverNode != null )
			{
				if( clientData.ConnectionMode == ConnectionModeEnum.Cloud )
				{
					//cloud mode


					//!!!!

					//if( message == "RequestAvatarSettings" )
					//{
					//	var user = serverNode.Users.GetUser( source );
					//	if( user != null )
					//	{
					//		serverNode?.Messages.SendToClient( source, "AvatarSettings", user.DirectServerAvatar );
					//	}
					//}
					//else if( message == "SetAvatarSettings" )
					//{
					//	var user = serverNode.Users.GetUser( source );
					//	if( user != null )
					//		user.DirectServerAvatar = data;
					//}


					////if( message == "RequestAvatarSettings" )
					////{
					////	var user = server.Users.GetUser( source );
					////	if( user != null )
					////	{
					////		var command = new GeneralManagerExecuteCommand();
					////		command.FunctionName = "api/get_user_settings";
					////		command.Parameters.Add( ("user", user.UserID.ToString()) );
					////		command.Parameters.Add( ("property", "Avatar") );
					////		command.Parameters.Add( ("defaultValue", "") );
					////		command.Tag = source;
					////		command.Processed += GeneralManagerExecuteRequestAvatarSettingsProcessed;
					////		command.BeginExecution( true );
					////	}
					////}
					////else if( message == "SetAvatarSettings" )
					////{
					////	var block = TextBlock.Parse( data, out var error );
					////	//if( !string.IsNullOrEmpty( error ) )
					////	//	Log.Warning( "Unable to parse avatar settings. " + error );

					////	if( block != null )
					////	{
					////		var clientData = (ClientData)source.Tag;
					////		if( clientData != null )
					////		{
					////			var command = new GeneralManagerExecuteCommand();
					////			command.FunctionName = "api/set_user_settings";
					////			command.Parameters.Add( ("project", projectID.ToString()) );
					////			command.Parameters.Add( ("purpose", "Enter") );
					////			command.Parameters.Add( ("code", clientData.VerificationCode) );
					////			command.Parameters.Add( ("property", "Avatar") );
					////			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );
					////			//!!!!check Processed?
					////			command.BeginExecution( true );
					////		}
					////	}
					////}
				}
				else
				{
					//usual multiplayer mode

					//!!!!

					//if( message == "RequestAvatarSettings" )
					//{
					//	var user = serverNode.Users.GetUser( source );
					//	if( user != null )
					//	{
					//		serverNode?.Messages.SendToClient( source, "AvatarSettings", user.DirectServerAvatar );
					//	}
					//}
					//else if( message == "SetAvatarSettings" )
					//{
					//	var user = serverNode.Users.GetUser( source );
					//	if( user != null )
					//		user.DirectServerAvatar = data;
					//}
				}
			}
		}

		public static ClientData GetClientData( ServerNode.Client client )
		{
			return client.Tag as ClientData;
		}

		public static void SetScene( Scene scene, string sceneInfo )
		{
			ResetScene();

			if( serverNode != null )
				serverNode.Components.SetScene( scene, sceneInfo );
		}

		public static void ResetScene()
		{
			if( serverNode != null && serverNode.Components.Scene != null )
				serverNode.Components.ResetScene();
		}

		//!!!!
		//static void TouchUserVerificationCodes()
		//{
		//	var now = DateTime.UtcNow;
		//	if( ( now - touchUserVerificationCodesLastTime ).TotalSeconds > 10 )
		//	{
		//		foreach( var client in serverNode.GetClientsArray() )
		//		{

		//			//!!!!check


		//			var clientData = client.Tag as ClientData;
		//			if( clientData != null && clientData.Verified && ( now - clientData.TouchUserVerificationCodeLastTime ).TotalMinutes > 5 )
		//			{
		//				var command = new GeneralManagerExecuteCommand();
		//				command.FunctionName = "api/get_user_by_verification_code";
		//				command.AddParameter( "project", projectID.ToString(), true );

		//				//!!!!
		//				Log.Fatal( "userRole" );

		//				command.AddParameter( "purpose", "Enter", true );
		//				command.AddParameter( "code", clientData.VerificationCode, true );
		//				command.Tag = client;
		//				//command.Processed += GeneralManagerExecuteCommandProcessed;
		//				command.BeginExecution( false );

		//				clientData.TouchUserVerificationCodeLastTime = now;
		//			}
		//		}

		//		touchUserVerificationCodesLastTime = now;
		//	}
		//}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SimulationAppServerAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsSimulation )
			{
				EngineApp.AppCreateAfter += delegate ()
				{
					SimulationAppServer.Init();
				};
			}
		}
	}

}
#endif