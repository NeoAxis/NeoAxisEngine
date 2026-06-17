// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.IO.Compression;
using NeoAxis;
using NeoAxis.Networking;

namespace CommandLineTools
{
	public static class PlatformServer
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 30.0;

		public static int ServerPort = 49876;
		public static bool AllowWebSocket = true; //WebSocket port: 49876 = ServerPort
		public static bool AllowUDP = true; //UDP port: 49877 = ServerPort + 1
		static string directModePassword = "";
		static PlatformServerNode serverNode;

		//new client initial settings
		public static double ConnectionDefaultMaxLifetime = 60 * 120;// 31536000;
		public static double ConnectionKeepAliveTime = 60;
		public static bool ConnectionAllowReconnect = true;
		public static string HelloFromServerMessage = "Hello from the server!";

		static DateTime updateLastTime;

		///////////////////////////////////////////////

		public class ClientData
		{
			public ConnectionModeEnum ConnectionMode;
			public string VerificationCode;
			public ServerNode.Client Client;

			//Direct mode
			public string Username;

			//custom data
			public string TaskID;
			public DateTime RequestTime;
		}

		///////////////////////////////////////////////

		public delegate void ClientDisconnectedDelegate( ServerNode.Client client );
		public static event ClientDisconnectedDelegate ClientDisconnected;

		public delegate void ClientConnectedDelegate( ServerNode.Client client );
		public static event ClientConnectedDelegate ClientConnected;

		///////////////////////////////////////////////

		public class PlatformServerNode : ServerNode
		{
			//services
			ServerNetworkService_CloudFunctions cloudFunctions;
			ServerNetworkService_Messages messages;
			ServerNetworkService_Users users;

			//

			public PlatformServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime, double keepAliveTime, bool allowReconnect, string fullPathToDatabase, bool databaseReadOnly, string projectDirectory, out string error )
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

		public static bool Created
		{
			get { return ServerNode != null; }
		}

		public static PlatformServerNode ServerNode
		{
			get { return serverNode; }
		}

		public static bool Init()
		{
			try
			{
				Console.WriteLine( "CommandLineTools: Platform Server." );
				Console.WriteLine( "System: " + Environment.OSVersion );

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-platformServer", out var password ) )
					return false;
				if( string.IsNullOrEmpty( password ) )
					return false;
				directModePassword = password;

				//create server
				if( !CreateServer( out var error ) )
				{
					Console.WriteLine( $"Error creating server: {error}" );
					return false;
				}

				Console.WriteLine( $"Server started." );
				if( AllowWebSocket )
					Console.WriteLine( $"WebSocket enabled on port {ServerPort}." );
				if( AllowUDP )
					Console.WriteLine( $"UDP enabled on port {ServerPort + 1}." );

				Console.WriteLine( "Press ESC to stop the server." );

				//loop
				while( true )
				{
					Tick();
					Thread.Sleep( 1 );

					if( Console.KeyAvailable )
					{
						var keyInfo = Console.ReadKey( true );
						if( keyInfo.Key == ConsoleKey.Escape )
							break;
					}
				}

				//exit
				Console.WriteLine( "Exiting..." );
				DestroyServer();
			}
			catch( Exception e )
			{
				Console.WriteLine( $"Error: {e.Message}" );
			}

			return false;
		}

		static bool CreateServer( out string error )
		{
			var fullPathToDatabase = "";
			var databaseReadOnly = true;

			//create or clear project directory
			var projectDirectory = Path.Combine( Path.GetTempPath(), "NeoAxisPlatformServer" );
			if( Directory.Exists( projectDirectory ) )
			{
				try
				{
					IOUtility.ClearDirectory( projectDirectory );
				}
				catch { }
			}
			else
				Directory.CreateDirectory( projectDirectory );

			serverNode = new PlatformServerNode( "NeoAxis Platform Server", EngineInfo.Version, 100000, ConnectionDefaultMaxLifetime, ConnectionKeepAliveTime, ConnectionAllowReconnect, fullPathToDatabase, databaseReadOnly, projectDirectory, out error );
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
			serverNode.CloudFunctions.CheckFileAccessEvent += CloudFunctions_CheckFileAccessEvent;

			serverNode.CloudFunctions.UploadFilesMaxBlockSize = 10 * 1024 * 1024;

			//register cloud methods
			serverNode.CloudFunctions.RegisterCloudMethods( typeof( PlatformServer ), out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				Console.WriteLine( "RegisterCallMethods error. " + error );
				return false;
			}

			if( !serverNode.BeginListen( false, null, AllowWebSocket ? ServerPort : 0, AllowUDP ? ServerPort + 1 : 0, out error ) )
			{
				serverNode.Dispose();
				serverNode = null;
				return false;
			}

			return true;
		}

		private static void Server_ProtocolError( ServerNode sender, ServerNode.Client client, string message )
		{
			Log.Warning( "PlatformServer: Protocol error: " + message );
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

			//send hello message
			serverNode.Messages.SendToClient( client, "HelloFromServerMessage", HelloFromServerMessage );

			//add to users service, with sending events to clients
			var user = serverNode.Users.AddUser( client );
		}

		static void Tick()
		{
			var utcNow = DateTime.UtcNow;
			if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			{
				updateLastTime = utcNow;
				serverNode?.Update( utcNow );
			}
		}

		private static void Server_ClientStatusChanged( ServerNode sender, ServerNode.Client client, string message )
		{
			Console.WriteLine( $"Client connection status changed for {client.LoginDataUserID}; {client.Status}; {message}" );

			switch( client.Status )
			{
			case NetworkStatus.Connected:
				{
					var clientData = GetClientData( client );

					ClientConnected?.Invoke( client );
				}
				break;

			case NetworkStatus.Disconnected:
				{
					var clientData = GetClientData( client );

					//remove user
					var user = serverNode.Users.GetUser( client );
					if( user != null )
						serverNode.Users.RemoveUser( user );

					ClientDisconnected?.Invoke( client );

					//delete request
					if( clientData != null && !string.IsNullOrEmpty( clientData.TaskID ) )
						DeleteTask( clientData.TaskID );
				}
				break;
			}
		}

		private static void Messages_ReceiveMessageString( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data )
		{
			var clientData = GetClientData( client );

			Console.WriteLine( string.Format( "Message from {0}: {1}", client.LoginDataUserID, message ) );
		}

		public static ClientData GetClientData( ServerNode.Client client )
		{
			return client.Tag as ClientData;
		}

		static void CloudFunctions_CheckFileAccessEvent( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, ServerNetworkService_CloudFunctions.FileSource source, string[] filePaths, string anyData, ServerNetworkService_CloudFunctions.FileOperationAccess requiredAccess, ref bool allow, ref string error )
		{
			//by default the access is disabled

			var clientData = GetClientData( client );

			if( string.IsNullOrEmpty( clientData.TaskID ) )
			{
				allow = false;
				error = "Client has no active request.";
				return;
			}

			//!!!!more verifications

			var invalidStrings = new[] { "..", ":", "*", "?", "\"", "<", ">", "|" };

			foreach( var filePath in filePaths )
			{
				//check for invalid characters
				foreach( var invalidString in invalidStrings )
				{
					if( filePath.Contains( invalidString ) )
					{
						allow = false;
						error = $"Invalid character in file path: {filePath}";
						return;
					}
				}

				//check for task ID in path
				if( !filePath.StartsWith( clientData.TaskID + Path.DirectorySeparatorChar ) )
				{
					allow = false;
					error = $"File path does not start with task ID: {filePath}";
					return;
				}
			}

			allow = true;


			//////change paths
			////for( int n = 0; n < filePaths.Length; n++ )
			////	filePaths[ n ] = Path.Combine( $"Projects/{clientData.ProjectID}", filePaths[ n ] );
		}

		static void DeleteTask( string taskID )
		{
			try
			{
				var taskDirectory = Path.Combine( serverNode.CloudFunctions.ProjectDirectory, taskID );
				Directory.Delete( taskDirectory, true );
			}
			catch( Exception e )
			{
				Log.Info( "DeleteTask exception: " + e.Message );
			}
		}

		static string GetTaskFullPathDirectory( string taskID )
		{
			return Path.Combine( serverNode.CloudFunctions.ProjectDirectory, taskID );
		}

		[CloudMethod]
		public static string CompileCreate( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			var clientData = GetClientData( context.Client );

			//delete old request
			if( !string.IsNullOrEmpty( clientData.TaskID ) )
				DeleteTask( clientData.TaskID );

			var taskID = Guid.NewGuid().ToString();
			clientData.TaskID = taskID;
			clientData.RequestTime = DateTime.UtcNow;

			var taskDirectory = GetTaskFullPathDirectory( taskID );
			Console.WriteLine( $"Task created with ID: {taskID}" );
			Console.WriteLine( $"Task directory: {taskDirectory}" );

			return taskID;
		}

		static void SendShowMessageToClient( ClientData clientData, string message )
		{
			serverNode.Messages.SendToClient( clientData.Client, "ShowMessage", message );
		}

		[CloudMethod]
		public static void CompileStart( ServerNetworkService_CloudFunctions.CallMethodContext context, string taskID, string compileFilePath )
		{
			var clientData = GetClientData( context.Client );

			if( clientData.TaskID != taskID )
				throw new Exception( "Invalid task ID." );

			var taskDirectory = GetTaskFullPathDirectory( taskID );

			//extract source files
			{
				Console.WriteLine( "Extracting Source.zip..." );
				var sourceZipFile = Path.Combine( taskDirectory, "Source.zip" );
				if( !File.Exists( sourceZipFile ) )
				{
					Console.WriteLine( "Source.zip file not found: " + sourceZipFile );
					throw new Exception( "Source.zip file not found." );
				}
				try
				{
					ZipFile.ExtractToDirectory( sourceZipFile, taskDirectory );
				}
				catch( Exception e )
				{
					Console.WriteLine( "Error extracting Source.zip: " + e.Message );
					throw new Exception( "Error extracting Source.zip: " + e.Message );
				}
			}

			//load compile file
			Console.WriteLine( "File to compile: " + compileFilePath );
			var compileFileFullPath = Path.Combine( taskDirectory, compileFilePath );

			var parser = new CompileFileParser( compileFileFullPath, false );
			parser.Parse();
			parser.Print();


			//!!!!
			var singleTask = true;


			Task.Run( async delegate ()
			{
				try
				{
					SendShowMessageToClient( clientData, "Start compilation..." );

					var compiler = new Compile.LibCompiler( parser );
					var success = await compiler.CompileAsync( true, singleTask );

					serverNode.Messages.SendToClient( clientData.Client, "CompileResult", success.ToString() );
				}
				catch( Exception e )
				{
					try
					{
						SendShowMessageToClient( clientData, "EXCEPTION: " + e.Message );
					}
					catch { }
				}
			} );
		}

		[CloudMethod]
		public static void CompileCancel( ServerNetworkService_CloudFunctions.CallMethodContext context, string taskID )
		{
			var clientData = GetClientData( context.Client );

			if( clientData.TaskID != taskID )
				throw new Exception( "Invalid task ID." );

			DeleteTask( taskID );
		}
	}
}