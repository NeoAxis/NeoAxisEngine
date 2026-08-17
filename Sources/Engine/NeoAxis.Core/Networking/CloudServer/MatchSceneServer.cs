#if !CLIENT
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis.Networking;

namespace NeoAxis.Cloud
{
	/// <summary>
	/// A server class that manages and synchronizes the scene of the match.
	/// </summary>
	public class MatchSceneServer : IDisposable
	{
		//settings
		public int ServerPortStart = 51000;
		public double ConnectionDefaultMaxLifetime = 31536000;
		public double ConnectionKeepAliveTime = 60;
		public bool ConnectionAllowReconnect = true;
		public string HelloFromServerMessage = "Hello from the server!";
		public bool ServerAllowWebSocket = true;
		public bool ServerAllowUDP = true;
		public double ServerNodeUpdateFrequency = 1.0 / 60.0;
		public bool ConsoleDebugOutput;

		//static fields
		static ESet<MatchSceneServer> instances = new ESet<MatchSceneServer>();

		//connection and clients data
		int serverPort;
		ClientConnectingGetUserByPasswordDelegate getUserByPassword;
		MatchSceneSyncServerNode serverNode;
		DateTime serverNodeUpdateLastTime;

		//time management
		double startTime;
		double lastUpdateTime;

		//scene
		Scene scene;
		string sceneName = "";

		///////////////////////////////////////////////

		public class ClientData
		{
			public ServerNode.Client Client;
		}

		///////////////////////////////////////////////

		public delegate bool ClientConnectingGetUserByPasswordDelegate( string password, out long userID );//, out string username );

		public delegate void ClientDisconnectedDelegate( ServerNode.Client client );
		public event ClientDisconnectedDelegate ClientDisconnected;

		public delegate void ClientConnectedDelegate( ServerNode.Client client );
		public event ClientConnectedDelegate ClientConnected;

		///////////////////////////////////////////////

		public class MatchSceneSyncServerNode : ServerNode
		{
			//services
			ServerNetworkService_CloudFunctions cloudFunctions;
			ServerNetworkService_Messages messages;
			ServerNetworkService_Users users;
			ServerNetworkService_Components components;

			//

			public MatchSceneSyncServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime, double keepAliveTime, bool allowReconnect, string fullPathToDatabase, bool databaseReadOnly, string projectDirectory, out string error )
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

			public ServerNetworkService_Components Components
			{
				get { return components; }
			}
		}

		///////////////////////////////////////////////

		public int ServerPort
		{
			get { return serverPort; }
		}

		public bool Created
		{
			get { return ServerNode != null; }
		}

		public MatchSceneSyncServerNode ServerNode
		{
			get { return serverNode; }
		}

		void AssignFreeServerPort()
		{
			lock( instances )
			{
				var usedPorts = new ESet<int>( instances.Count );
				foreach( var instance in instances )
				{
					if( instance.serverPort != 0 )
						usedPorts.AddWithCheckAlreadyContained( instance.ServerPort );
				}

				for( int n = ServerPortStart; ; n += 2 )
				{
					if( !usedPorts.Contains( n ) )
					{
						serverPort = n;
						return;
					}
				}
			}
		}

		bool LoadScene( string sceneName )
		{
			ResetScene( true );

			this.sceneName = sceneName;
			if( string.IsNullOrEmpty( sceneName ) )
			{
				ServerLogs.Write( "Cloud Functions", "MatchSceneServer: Scene name is empty." );
				Console.WriteLine( "MatchSceneServer: Scene name is empty." );
				return false;
			}

			ServerLogs.Write( "Cloud Functions", "MatchSceneServer: Loading scene \"" + sceneName + "\"." );
			if( ConsoleDebugOutput )
				Console.WriteLine( "MatchSceneServer: Loading scene \"" + sceneName + "\"." );

			scene = ResourceManager.LoadSeparateInstance<Scene>( sceneName, true, false, out var error );
			if( scene == null )
			{
				if( !string.IsNullOrEmpty( error ) )
				{
					ServerLogs.Write( "Cloud Functions", "MatchSceneServer: Load scene error: " + error );
					Console.WriteLine( "MatchSceneServer: Load scene error: " + error );
				}
				return false;
			}

			//!!!!exception when scene Dispose
			//scene.HierarchyController.FatalWhenHierarchyChangeFromNotControllerThread = true;

			SetScene( scene, sceneName, ProjectSettings.Get.General.SimulationStepsPerSecond );

			scene.Enabled = true;

			startTime = EngineApp.EngineTime;

			//////subscribe to simulation step event
			////scene.HierarchyController.SimulationStep += HierarchyController_SimulationStep;

			return true;
		}

		/// <summary>
		/// Creates a new game instance with the specified scene configuration and number of players.
		/// </summary>
		public bool Create( string sceneName, bool sceneInterpolation, ClientConnectingGetUserByPasswordDelegate getUserByPassword )
		{
			lock( instances )
				instances.Add( this );

			//server connection settings
			AssignFreeServerPort();

			//create server
			if( !CreateServer( sceneInterpolation, out var error ) )
			{
				ServerLogs.Write( "Cloud Functions", "MatchSceneServer: CreateServer error: " + error );
				Console.WriteLine( "MatchSceneServer: CreateServer error: " + error );
				return false;
			}
			if( getUserByPassword == null )
			{
				ServerLogs.Write( "Cloud Functions", "MatchSceneServer: getUserByPassword delegate is null." );
				Console.WriteLine( "MatchSceneServer: getUserByPassword delegate is null." );
				return false;
			}
			this.getUserByPassword = getUserByPassword;

			//load scene
			LoadScene( sceneName );

			return true;
		}

		public bool Reset( string sceneName )
		{
			return LoadScene( sceneName );
		}

		///// <summary>
		///// Performs a simulation step for the hierarchy controller, updating car objects based on the current player controls.
		///// </summary>
		//private void HierarchyController_SimulationStep( ComponentHierarchyController sender )
		//{
		//	double delta = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
		//}

		/// <summary>
		/// Releases all resources used by the current instance.
		/// </summary>
		public void Dispose()
		{
			DestroyServer();

			if( scene != null )
			{
				ServerLogs.Write( "Cloud Functions", "MatchSceneServer: Destroy scene \"" + sceneName + "\"." );
				if( ConsoleDebugOutput )
					Console.WriteLine( "MatchSceneServer: Destroy scene \"" + sceneName + "\"." );

				scene.Dispose();
				scene = null;
			}

			lock( instances )
				instances.Remove( this );
		}

		/// <summary>
		/// Advances the simulation state and sends updated car positions to all connected clients based on the provided match data.
		/// </summary>
		public void Update( DateTime utcNow )
		{
			try
			{
				//server node update
				if( utcNow - serverNodeUpdateLastTime > TimeSpan.FromSeconds( ServerNodeUpdateFrequency ) )
				{
					serverNodeUpdateLastTime = utcNow;
					serverNode?.Update( utcNow );
				}

				//simulation step

				var lastUpdateTime2 = lastUpdateTime;
				var delta = EngineApp.EngineTime - lastUpdateTime2;

				double deltaThreshold = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
				if( delta >= deltaThreshold )
				{
					//too big delta
					if( delta > 1 )
						delta = deltaThreshold;

					//update and simulate the scene
					var scene2 = scene;
					if( scene2 != null && !scene2.Disposed )
					{
						ComponentsHidePublic.PerformUpdate( scene2, (float)delta );
						scene2.HierarchyController.PerformSimulationSteps();
					}

					lastUpdateTime = lastUpdateTime2;
				}
			}
			catch( Exception e )
			{
				Log.Error( "GameInstanceClass: UpdateAndSimulate: " + e.ToString() );
			}
		}

		///// <summary>
		///// Saves the current match details to the text block.
		///// </summary>
		//public void SaveMatchDetails( TextBlock rootBlock )
		//{
		//	//var sceneBlock = rootBlock.AddChild( "Scene" );
		//}

		bool CreateServer( bool sceneInterpolation, out string error )
		{
			var projectDirectory = "";

			serverNode = new MatchSceneSyncServerNode( "NeoAxis Game Instance Server", EngineInfo.Version, 100000, ConnectionDefaultMaxLifetime, ConnectionKeepAliveTime, ConnectionAllowReconnect, "", true, projectDirectory, out error );
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
			serverNode.Interpolation = sceneInterpolation;

			var serverPortWebSocket = ServerAllowWebSocket ? serverPort : 0;
			var serverPortUDP = ServerAllowUDP ? serverPort + 1 : 0;

			if( !serverNode.BeginListen( false, null, serverPortWebSocket, serverPortUDP, out error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			return true;
		}

		private void Server_ProtocolError( ServerNode sender, ServerNode.Client client, string message )
		{
			ServerLogs.Write( "Cloud Functions", "MatchSceneServer: ServerNode: Protocol error: " + message );
			Console.WriteLine( "MatchSceneServer: ServerNode: Protocol error: " + message );
		}

		void DestroyServer()
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

		private void Server_IncomingConnectionApproval( ServerNode sender, ServerNode.Client client, ServerNode.IncomingConnectionApproveResult approveResult )
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

				var password = block.GetAttribute( "Password" );
				if( string.IsNullOrEmpty( password ) )
				{
					approveResult.Reject( "Password is empty." );
					return;
				}

				if( !getUserByPassword( password, out var userID ) ) //, out var username ) )
				{
					approveResult.Reject( "Invalid password." );
					return;
				}

				//if( !userGameInstanceServerPasswords.TryGetValue( password, out var userID ) )
				//{
				//	approveResult.Reject( "Invalid password." );
				//	return;
				//}

				client.LoginDataUserID = userID;
				//client.LoginDataUsername = username;
				if( string.IsNullOrEmpty( client.LoginDataUsername ) )
					client.LoginDataUsername = "User" + userID.ToString(); //client.LoginDataUsername = "User" + client.LoginDataUserID.ToString();

				approveResult.Approve();
			}
			catch( Exception e )
			{
				approveResult.Reject( "Exception: " + e.Message );
				return;
			}
		}

		private void ServerNode_ClientBeforeStatusChangeToConnected( ServerNode sender, ServerNode.Client client )
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
		}

		private void Server_ClientStatusChanged( ServerNode sender, ServerNode.Client client, string message )
		{
			//if( SimulationApp.NetworkLogging )
			//	Log.Info( $"Client connection status changed for {client.LoginDataUserID}; {client.Status}; {message}" );

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
							serverNode.Components.SendSceneCreate( clientItem, ProjectSettings.Get.General.SimulationStepsPerSecond );
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
						serverNode.Users.RemoveUser( user );

					ClientDisconnected?.Invoke( client );
				}
				break;
			}
		}

		private void Messages_ReceiveMessageString( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data )
		{
			var clientData = GetClientData( client );

			//if( SimulationApp.NetworkLogging )
			//	Log.Info( string.Format( "Message from {0}: {1}", client.LoginDataUserID, message ) );
		}

		public ClientData GetClientData( ServerNode.Client client )
		{
			return client.Tag as ClientData;
		}

		void SetScene( Scene scene, string sceneInfo, double simulationStepsPerSecond )
		{
			if( serverNode != null )
				serverNode.Components.SetScene( scene, sceneInfo, simulationStepsPerSecond );
		}

		void ResetScene( bool anotherSceneWillLoaded )
		{
			serverNode?.Components?.ResetScene( anotherSceneWillLoaded );
		}

		public double StartTime
		{
			get { return startTime; }
		}

		public double CurrentTime
		{
			get { return EngineApp.EngineTime - StartTime; }
		}

		public Scene Scene
		{
			get { return scene; }
		}
	}
}
#endif