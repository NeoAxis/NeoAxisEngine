#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// A basic server for the process in the cloud.
	/// </summary>
	public static class CloudFunctionsServer
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 30.0;
		public static double GCCollectFrequencyInMinutes { get; set; } = 10.0;

		//settings
		static bool ServerLogsEnabled;
		public static bool Database;
		public static bool DatabaseReadOnly;
		public static bool AccessCodes;
		public static bool Transactions;
		public static bool ChatsEnabled;
		public static bool MatchesEnabled;
		public static bool ServerWebSocket = true;
		public static bool ServerUDP = true;

		//client settings
		public static double ConnectionDefaultMaxLifetime = 31536000;
		public static double ConnectionKeepAliveTime = 60;
		public static bool ConnectionAllowReconnect = true;
		public static string HelloFromServerMessage = "Hello from the server!";

		//server node
		static CloudFunctionsServerNode serverNode;
		static DateTime lastSendInfoFromApp;

		//statistics of all connections of the process
		public static NetworkAggregateConnectionStatistics AggregatedConnectionStatistics = new NetworkAggregateConnectionStatistics();

		static DateTime updateLastTime;

		static DateTime gcCollectLastTime;

		///////////////////////////////////////////////

		public class CloudFunctionsServerNode : ServerNode
		{
			ServerNetworkService_CloudFunctions cloudFunctions;
			ServerNetworkService_Messages messages;
			ServerNetworkService_Users users;

			//

			public CloudFunctionsServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime, double keepAliveConnectionTime, bool allowReconnect, string fullPathToDatabase, bool databaseReadOnly, string projectDirectory, out string error )
				: base( serverName, serverVersion, maxConnections, defaultMaxLifetime, keepAliveConnectionTime, allowReconnect )
			{
				cloudFunctions = new ServerNetworkService_CloudFunctions( fullPathToDatabase, databaseReadOnly, projectDirectory, out error );
				RegisterService( cloudFunctions );

				messages = new ServerNetworkService_Messages();
				RegisterService( messages );

				users = new ServerNetworkService_Users();
				RegisterService( users );
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
		}

		///////////////////////////////////////////////

		public class ClientData
		{
			public ConnectionModeEnum ConnectionMode;
			public string VerificationCode;
			public ServerNode.Client Client;
		}

		///////////////////////////////////////////////

		////public delegate void ServerStartedDelegate();
		////public static event ServerStartedDelegate ServerStarted;

		////public delegate void ServerShuttingDownDelegate();
		////public static event ServerShuttingDownDelegate ServerShuttingDown;

		//useful events

		public delegate void ClientConnectedDelegate( ServerNode.Client client );
		public static event ClientConnectedDelegate ClientConnected;

		public delegate void ClientDisconnectedDelegate( ServerNode.Client client );
		public static event ClientDisconnectedDelegate ClientDisconnected;

		public delegate void ProcessAddSummaryBlockDelegate( TextBlock summary );
		public static event ProcessAddSummaryBlockDelegate ProcessAddSummaryBlock;

		///////////////////////////////////////////////

		public static string GetProjectDirectory()
		{
			var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
			return Path.Combine( CloudServerProcessUtility.ProjectsDirectory, projectID.ToString() );
		}

		public static bool Init( out string error )
		{
			DestroyServer();

			//get settings
			var settingsBlock = CloudServerProcessUtility.CommandLineParameters.ProcessSettingsTextBlock;
			if( bool.TryParse( settingsBlock.GetAttribute( "ServerLogs" ), out var serverLogs ) )
				ServerLogsEnabled = serverLogs;
			if( bool.TryParse( settingsBlock.GetAttribute( "Database" ), out var database ) )
				Database = database;
			if( bool.TryParse( settingsBlock.GetAttribute( "DatabaseReadOnly" ), out var databaseReadOnly ) )
				DatabaseReadOnly = databaseReadOnly;
			if( bool.TryParse( settingsBlock.GetAttribute( "AccessCodes" ), out var accessCodes ) )
				AccessCodes = accessCodes;
			if( bool.TryParse( settingsBlock.GetAttribute( "Transactions" ), out var transactions ) )
				Transactions = transactions;
			if( bool.TryParse( settingsBlock.GetAttribute( "Chats" ), out var chats ) )
				ChatsEnabled = chats;
			if( bool.TryParse( settingsBlock.GetAttribute( "Matches" ), out var matches ) )
				MatchesEnabled = matches;
			if( bool.TryParse( settingsBlock.GetAttribute( "ServerWebSocket" ), out var serverWebSocket ) )
				ServerWebSocket = serverWebSocket;
			if( bool.TryParse( settingsBlock.GetAttribute( "ServerUDP" ), out var serverUDP ) )
				ServerUDP = serverUDP;

			if( ServerLogsEnabled )
				ServerLogs.Init();

			//init database
			var fullPathToDatabase = "";
			if( Database )
			{
				fullPathToDatabase = Path.Combine( CloudServerProcessUtility.CommandLineParameters.ProjectDirectory, "CloudFunctionsData", "Database.litedb" );
			}

			//create a server node
			serverNode = new CloudFunctionsServerNode( "NeoAxis Server", EngineInfo.Version, 100000, ConnectionDefaultMaxLifetime, ConnectionKeepAliveTime, ConnectionAllowReconnect, fullPathToDatabase, databaseReadOnly, CloudServerProcessUtility.CommandLineParameters.ProjectDirectory, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			if( AccessCodes )
				AccessCodesClass.Initialize();
			if( Transactions )
				CloudServer.Transactions.Initialize();
			HorizontalServers.Initialize();
			if( ChatsEnabled )
				Chats.Initialize();
			if( MatchesEnabled )
				Matches.Initialize();

			//events for connection and basic functionality
			serverNode.ProtocolError += Server_ProtocolError;
			serverNode.IncomingConnectionApproval += Server_IncomingConnectionApproval;
			serverNode.ClientBeforeStatusChangeToConnected += ServerNode_ClientBeforeStatusChangeToConnected;
			serverNode.ClientStatusChanged += Server_ClientStatusChanged;
			serverNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;

			//register BasicCommands class
			serverNode.CloudFunctions.RegisterCloudMethods( typeof( BasicCommands ), out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				ServerLogs.Write( "Cloud Functions", "RegisterCallMethods error. " + error );
				Console.WriteLine( "RegisterCallMethods error. " + error );
			}

			//start the server
			var port = CloudServerProcessUtility.CommandLineParameters.ServerPort;
			if( !serverNode.BeginListen( false, CloudServerProcessUtility.CommandLineParameters.ServerAddress, ServerWebSocket ? port : 0, ServerUDP ? ( port + 1 ) : 0, out error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			ServerLogs.Write( "Cloud Functions", "Started." );

			return true;
		}

		private static void Server_ProtocolError( ServerNode sender, ServerNode.Client client, string message )
		{
			ServerLogs.Write( "Cloud Functions", $"ProtocolError; {client.GetAddressText()}; {message}" );
			Console.WriteLine( "Cloud Functions: Protocol error: " + message );
		}

		static void DestroyServer()
		{
			var serverNode2 = serverNode;
			if( serverNode2 != null )
			{
				serverNode2.ProtocolError -= Server_ProtocolError;
				serverNode2.IncomingConnectionApproval -= Server_IncomingConnectionApproval;
				serverNode2.ClientBeforeStatusChangeToConnected -= ServerNode_ClientBeforeStatusChangeToConnected;
				serverNode2.ClientStatusChanged -= Server_ClientStatusChanged;
				serverNode2.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;

				serverNode2.Dispose();
				serverNode = null;
			}
		}

		public static void Shutdown()
		{
			DestroyServer();
		}

		//static DateTime lastUpdate;

		public static void Update()
		{
			var utcNow = DateTime.UtcNow;
			if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			{
				updateLastTime = utcNow;

				try
				{
					serverNode?.Update( utcNow );
					WriteMessageToServerManagerTxt( utcNow );
					Actions.Update( utcNow );
					CloudServer.Transactions.Update( utcNow );
					HorizontalServers.Update( utcNow );
					Chats.Update( utcNow );
					Matches.Update( utcNow );
					UpdateGCCollect( utcNow );


					//if( false )
					//{
					//	var now = DateTime.UtcNow;
					//	if( ( now - lastUpdate ).TotalSeconds > 11 )
					//	{
					//		lastUpdate = now;

					//		Console.WriteLine( "LOGS:" );

					//		if( serverNode != null )
					//		{
					//			foreach( var client in serverNode.GetClientsArray() )
					//			{
					//				var realConnection = client.realConnection;
					//				var webSocket = realConnection?.WebSocketContext.WebSocket;

					//				var connectionCounter = realConnection?.DebugConnectionCounter.ToString() ?? "null";

					//				var closeStatus = "";
					//				if( webSocket?.CloseStatus != null )
					//					closeStatus = webSocket.CloseStatus.ToString();

					//				var lastRoundTrip = client.GetRoundtripLastInSeconds().ToString( "F2" );

					//				Console.WriteLine( $"Client={client.GetAddressText()}; Status={client.status}; GetNoRealSeconds={client.GetNoRealConnectionTimeInSeconds()}; webSocket.State={webSocket?.State.ToString() ?? ""}; Counter={connectionCounter}; Token={client.reconnectToken ?? "null"}; LastRoundTrip={lastRoundTrip}" );
					//			}
					//		}

					//		Console.WriteLine( "LOGS END" );
					//	}
					//}
				}
				catch( Exception e )
				{
					ServerLogs.Write( "Cloud Functions", "Update exception: " + e.ToString() );
				}
			}
		}

		public static CloudFunctionsServerNode ServerNode
		{
			get { return serverNode; }
		}

		public static int ConnectedNodesCount
		{
			get { return serverNode != null ? serverNode.ClientCount : 0; }
		}

		static string HashPassword2( string value )
		{
			using( var sha = SHA256.Create() )
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes( value );
				byte[] hashBytes = sha.ComputeHash( inputBytes );

				var sb = new StringBuilder();
				for( int i = 0; i < hashBytes.Length; i++ )
					sb.Append( hashBytes[ i ].ToString( "X2" ) );
				return sb.ToString().Substring( 0, 32 );
			}
		}

		static string HashPassword( string value )
		{
			return HashPassword2( HashPassword2( value + "sa" ) + "lt" );
		}

		private static void Server_IncomingConnectionApproval( ServerNode sender, ServerNode.Client client, ServerNode.IncomingConnectionApproveResult approveResult )
		{
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

					//start checking cloud verification code
					//Task.Run( async delegate ()
					TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "CloudFunctionsServer: Server_IncomingConnectionApproval: Start checking cloud verification code", async delegate ()
					{
						var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
						var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;

						using var cts = new CancellationTokenSource( new TimeSpan( 0, 0, 30 ) );
						var result = await CloudServiceFunctions.AccessGetUserByVerificationCodeAsync( projectID, client.UserRole, clientData.VerificationCode, false, serverCheckCode, cts.Token );

						if( !string.IsNullOrEmpty( result.Error ) )
						{
							approveResult.Reject( result.Error );
						}
						else
						{
							//set userID, username
							client.LoginDataUserID = result.UserID;
							client.LoginDataUsername = result.Username;
							approveResult.Approve();
						}
					} );
				}
				else
				{
					var password = block.GetAttribute( "Password" );

					//check access code
					long userID = 0;
					if( AccessCodesClass.Enabled )
						userID = AccessCodesClass.GetUserIDByAccessCode( password );
					if( userID != 0 )
						client.LoginDataUserID = userID;

					//1. code to execute on horizontal server. when connecting to horizontal server, Password property is used for server check code
					//2. web access from front server
					var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
					if( !string.IsNullOrEmpty( serverCheckCode ) && password == serverCheckCode )
						userID = CloudServerProcessUtility.CommandLineParameters.UserID;

					//check password
					if( userID == 0 )
					{
						var passwordHash = HashPassword( password );
						if( CloudServerProcessUtility.CommandLineParameters.ServerPasswordHash != passwordHash )
						{
							approveResult.Reject( "Invalid password or access code." );
							return;
						}
					}

					approveResult.Approve();
				}

				if( ServerLogsEnabled )
					ServerLogs.Write( "Cloud Functions", $"IncomingConnectionApproval; {client.GetAddressText()}" );
			}
			catch( Exception e )
			{
				ServerLogs.Write( "Cloud Functions", $"IncomingConnectionApproval reject; {client.GetAddressText()}; {e.Message}" );
				approveResult.Reject( "Exception: " + e.Message );
			}
		}

		private static void ServerNode_ClientBeforeStatusChangeToConnected( ServerNode sender, ServerNode.Client client )
		{
			var clientData = GetClientData( client );

			//update max lifetime
			client.MaxLifetime = ConnectionDefaultMaxLifetime;

			//configure collecting connection statistics
			client.AggregateConnectionStatistics = AggregatedConnectionStatistics;

			//send hello message
			serverNode.Messages.SendToClient( client, "HelloFromServerMessage", HelloFromServerMessage );

			//add to users service, with sending events to clients
			serverNode.Users.AddUser( client );
		}

		private static void Server_ClientStatusChanged( ServerNode sender, ServerNode.Client client, string message )
		{
			if( ServerLogsEnabled )
				ServerLogs.Write( "Cloud Functions", $"ClientStatusChanged; {client.GetAddressText()}; {client.Status}; {message}" );

			//Console.WriteLine( $"ClientStatusChanged; {client.GetAddressText()}; {client.Status}; {message}" );

			switch( client.Status )
			{
			case NetworkStatus.Connected:
				ClientConnected?.Invoke( client );
				break;

			case NetworkStatus.Disconnected:
				ClientDisconnected?.Invoke( client );
				break;
			}
		}

		//MessageToServerManager.txt file is used to send info to the server manager app.
		static void WriteMessageToServerManagerTxt( DateTime utcNow )
		{
			if( ServerNode != null && ( utcNow - lastSendInfoFromApp ).TotalSeconds > 10 )
			{
				lastSendInfoFromApp = utcNow;

				var rootBlock = new TextBlock();

				try
				{
					rootBlock.SetAttribute( "Clients", ConnectedNodesCount.ToString() );
					rootBlock.SetAttribute( "NetworkSent", AggregatedConnectionStatistics.GetSent().ToString() );
					rootBlock.SetAttribute( "NetworkReceived", AggregatedConnectionStatistics.GetReceived().ToString() );

					//add summary
					{
						var summary = new TextBlock();
						summary.SetAttribute( "Clients", ConnectedNodesCount.ToString() );

						var cloudFunctions = serverNode.CloudFunctions;
						if( cloudFunctions != null )
						{
							summary.SetAttribute( "Database", ( cloudFunctions.DatabaseImpl != null ).ToString() );
							if( cloudFunctions.DatabaseImpl != null )
								summary.SetAttribute( "DatabaseStringCount", cloudFunctions.DatabaseImpl.GetStringCount().ToString() );
						}
						summary.SetAttribute( "CallMethods", cloudFunctions.CloudMethodCount.ToString() );

						ProcessAddSummaryBlock?.Invoke( summary );
						rootBlock.SetAttribute( "Summary", summary.DumpToString() );
					}

					//add service CloudFunctions
					{
						var block = rootBlock.AddChild( "Service" );
						block.SetAttribute( "Name", "CloudFunctions" );
						block.SetAttribute( "SelectionMethod", "ProjectID" );
					}

					//add actions
					foreach( var action in Actions.GetActions() )
					{
						var actionBlock = rootBlock.AddChild( "Action" );
						actionBlock.SetAttribute( "ID", action.ID );
						actionBlock.SetAttribute( "Text", action.Text );
						actionBlock.SetAttribute( "Stoppable", action.Stoppable.ToString() );
						actionBlock.SetAttribute( "Status", action.Status.ToString() );
						actionBlock.SetAttribute( "Progress", action.Progress.ToString() );
						actionBlock.SetAttribute( "RemainingTimeInSeconds", action.GetRemainingTimeInSeconds().ToString() );
						actionBlock.SetAttribute( "StopCallMethodClassName", typeof( BasicCommands ).Name );
						actionBlock.SetAttribute( "StopCallMethodMethodName", "StopAction" );
					}
				}
				catch( Exception e )
				{
					ServerLogs.Write( "Cloud Functions", "WriteMessageToServerManagerTxt exception: " + e.ToString() );
					rootBlock.SetAttribute( "Summary", "Exception: " + e.ToString() );
				}

				try
				{
					File.WriteAllText( CloudServerProcessUtility.MessageToServerManagerTxtFullPath, rootBlock.DumpToString() + "[[!END!]]" );
				}
				catch { }
			}
		}

		private static void Messages_ReceiveMessageString( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data )
		{
			//var clientData = GetClientData( client );

			//if( ServerLogsEnabled )
			//	ServerLogs.Write( "Cloud Functions", $"ReceiveMessageString; {client.GetAddressText()}; {message}" );
		}

		public static ClientData GetClientData( ServerNode.Client client )
		{
			return client.Tag as ClientData;
		}

		public static void SendMessageToClientByLoginDataUserID( long userID, string message, string data )
		{
			var serverNode = ServerNode;
			if( serverNode != null )
			{
				var client = serverNode.GetClientByLoginDataUserID( userID );
				if( client != null )
					serverNode.Messages?.SendToClient( client, message, data );
			}
		}

		public static void SendMessageToClientByLoginDataUserID( long userID, string message, ArraySegment<byte> data )
		{
			var serverNode = ServerNode;
			if( serverNode != null )
			{
				var client = serverNode.GetClientByLoginDataUserID( userID );
				if( client != null )
					serverNode.Messages?.SendToClient( client, message, data );
			}
		}

		public static void SendMessageToClientByLoginDataUserID( long userID, string message, byte[] data )
		{
			SendMessageToClientByLoginDataUserID( userID, message, new ArraySegment<byte>( data ) );
		}

		public static void SendMessageToClientsByLoginDataUserID( IList<long> userIDs, string message, string data )
		{
			var serverNode = ServerNode;
			if( serverNode != null )
			{
				var clients = serverNode.GetClientsByLoginDataUserID( userIDs );
				if( clients.Count > 0 )
					serverNode.Messages?.SendToClients( clients, message, data );
			}
		}

		public static void SendMessageToClientsByLoginDataUserID( IList<long> userIDs, string message, ArraySegment<byte> data )
		{
			var serverNode = ServerNode;
			if( serverNode != null )
			{
				var clients = serverNode.GetClientsByLoginDataUserID( userIDs );
				if( clients.Count > 0 )
					serverNode.Messages?.SendToClients( clients, message, data );
			}
		}

		public static void SendMessageToClientsByLoginDataUserID( IList<long> userIDs, string message, byte[] data )
		{
			SendMessageToClientsByLoginDataUserID( userIDs, message, new ArraySegment<byte>( data ) );
		}

		//also can use <ServerGarbageCollection>false</ServerGarbageCollection> in the csproj file.
		static void UpdateGCCollect( DateTime utcNow )
		{
			if( GCCollectFrequencyInMinutes > 0 && ( utcNow - gcCollectLastTime ).TotalMinutes > GCCollectFrequencyInMinutes )
			{
				gcCollectLastTime = utcNow;

				try
				{
					GC.Collect( 2, GCCollectionMode.Forced, false );

					//GC.Collect();
					//GC.WaitForPendingFinalizers();
					//GC.Collect();
				}
				catch( Exception e )
				{
					ServerLogs.Write( "Cloud Functions", "UpdateGCCollect exception: " + e.ToString() );
				}
			}
		}
	}
}
#endif