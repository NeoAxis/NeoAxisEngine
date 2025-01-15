//!!!!update networking

//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using System.Reflection;
//using System.Threading.Tasks;
//using NeoAxis;
//using NeoAxis.Networking;

//namespace CommandLineTools
//{
//	public class BuildServerClient
//	{
//		static List<BuildServerClient> instances = new List<BuildServerClient>();

//		BuildServerClientNode client;
//		bool? finishedResult;
//		Compile.CompileFileParser parser;

//		//////////////////////

//		public class BuildServerClientNode : ClientNode
//		{
//			//services
//			ClientNetworkService_Messages messages;

//			//

//			public BuildServerClientNode()
//			{
//				//register messages service
//				messages = new ClientNetworkService_Messages();
//				RegisterService( messages );
//			}

//			public ClientNetworkService_Messages Messages
//			{
//				get { return messages; }
//			}
//		}

//		//////////////////////

//		public static bool BeginConnect( string serverIP, int port, string compilationID, string verificationCode, Compile.CompileFileParser parser, out string error )
//		{
//			var instance = new BuildServerClient();
//			instance.parser = parser;

//			if( !instance.ConnectToServer( serverIP, port, compilationID, verificationCode, out error ) )
//				return false;

//			lock( instances )
//				instances.Add( instance );

//			return true;
//		}

//		bool ConnectToServer( string serverIP, int port, string compilationID, string verificationCode, out string error )
//		{
//			error = "";

//			DestroyClient();

//			client = new BuildServerClientNode();
//			client.ProtocolError += Client_ProtocolError;
//			client.ConnectionStatusChanged += Client_ConnectionStatusChanged;
//			client.Messages.ReceiveMessage += Messages_ReceiveMessage;

//			var block = new TextBlock();
//			block.SetAttribute( "CompilationID", compilationID );
//			block.SetAttribute( "VerificationCode", verificationCode );

//			if( !client.BeginConnect( serverIP, port, EngineInfo.Version, block.DumpToString(), 100, out error ) )
//			{
//				client.Dispose();
//				client = null;
//				return false;
//			}

//			return true;
//		}

//		private void Client_ProtocolError( NetworkClientNode sender, string message )
//		{
//			Console.WriteLine( "BuildServerClient: Protocol error: " + message );
//		}

//		void DestroyClient()
//		{
//			if( client != null )
//			{
//				client.ProtocolError -= Client_ProtocolError;
//				client.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
//				client.Messages.ReceiveMessage -= Messages_ReceiveMessage;

//				client.Dispose();
//				client = null;
//			}

//			lock( instances )
//				instances.Remove( this );
//		}

//		public static BuildServerClient[] GetInstances()
//		{
//			lock( instances )
//				return instances.ToArray();
//		}

//		public static void AppDestroy()
//		{
//			foreach( var instance in GetInstances() )
//				instance.DestroyClient();
//		}

//		public static void Update()
//		{
//			foreach( var instance in GetInstances() )
//				instance.client?.Update();
//		}

//		void Client_ConnectionStatusChanged( NetworkClientNode sender, NetworkStatus status )
//		{
//			switch( status )
//			{
//			case NetworkStatus.Disconnected:
//				lock( instances )
//					instances.Remove( this );
//				break;
//			}
//		}

//		private void Messages_ReceiveMessage( ClientNetworkService_Messages sender, string message, string data )
//		{
//			if( message == "InfoMessage" )
//			{
//				Console.WriteLine( "From the server: " + data );
//				return;
//			}

//			if( message == "Files" )
//			{
//				try
//				{
//					//parse files info on the server
//					var filesOnServer = new List<CompileOnBuildServer.FileItem>();
//					{
//						var rootBlock = TextBlock.Parse( data, out var error );
//						if( !string.IsNullOrEmpty( error ) )
//						{
//							Console.WriteLine( "Error: TextBlock.Parse failed: " + error );
//							return;
//						}

//						foreach( var block in rootBlock.Children )
//						{
//							var name = block.GetAttribute( "Name" );
//							var length = long.Parse( block.GetAttribute( "Length" ) );
//							var crc = uint.Parse( block.GetAttribute( "CRC" ) );

//							var fileItem = new CompileOnBuildServer.FileItem( name, length, crc );
//							filesOnServer.Add( fileItem );
//						}

//						Console.WriteLine( $"Got files info from the server. Files {filesOnServer.Count}." );
//					}

//					var filesOnClient = CompileOnBuildServer.FilesToUpload;


//					//compare what files need to send, what to delete
//					var filesToUpload = new List<CompileOnBuildServer.FileItem>(); //var filesToUpload = new List<string>();
//					var filesToDelete = new List<string>();


//					// Convert lists to dictionaries for quick lookup
//					var serverFileDict = filesOnServer.ToDictionary( f => f.Name, f => f );
//					var clientFileDict = filesOnClient.ToDictionary( f => f.Name, f => f );

//					// Determine files to upload
//					foreach( var clientFile in filesOnClient )
//					{
//						// Check if the file exists on the server
//						if( serverFileDict.TryGetValue( clientFile.Name, out var serverFile ) )
//						{
//							// File exists on both client and server
//							if( clientFile.Length != serverFile.Length || clientFile.CRC != serverFile.CRC )
//							{
//								// If lengths are different or CRCs don't match, we need to upload the client file
//								filesToUpload.Add( clientFile );//.Name );
//							}
//						}
//						else
//						{
//							// If the file does not exist on the server, we should upload it
//							filesToUpload.Add( clientFile );//.Name );
//						}
//					}

//					// Determine files to delete
//					foreach( var serverFile in filesOnServer )
//					{
//						// Check if the file exists on the client
//						if( !clientFileDict.ContainsKey( serverFile.Name ) )
//						{
//							// If the file does not exist on the client, we should delete it from the server
//							filesToDelete.Add( serverFile.Name );
//						}
//					}


//					//send to the server
//					Console.WriteLine( "Sending files to the server..." );

//					Client.Messages.SendToServer( "DeleteFiles", string.Join( "|", filesToDelete ) );


//					byte[] filesToUploadData;
//					using( var memoryStream = new MemoryStream() )
//					{
//						using( var zipArchive = new ZipArchive( memoryStream, ZipArchiveMode.Create, true ) )
//						{
//							foreach( var file in filesToUpload )
//							{
//								var fullPath = Path.Combine( parser.RootPath, file.Name );
//								var fileContent = File.ReadAllBytes( fullPath );

//								var zipEntry = zipArchive.CreateEntry( file.Name, CompressionLevel.Optimal );
//								using( var entryStream = zipEntry.Open() )
//									entryStream.Write( fileContent, 0, fileContent.Length );
//							}
//						}

//						filesToUploadData = memoryStream.ToArray();
//					}

//					//need better uploader for bigger size of files. like NetworkService_FileSync or ServerNetworkService_Commit
//					if( filesToUploadData.Length > 200 * 1024 * 1024 )
//					{
//						Console.WriteLine( "filesToUploadData.Length > 200 * 1024 * 1024" );
//						return;
//					}

//					Client.Messages.SendToServer( "UploadFiles", filesToUploadData );
//				}
//				catch( Exception e )
//				{
//					Console.WriteLine( "Exception: " + e.Message );
//					return;
//				}

//				return;
//			}

//			if( message == "UploadFilesFinished" )
//			{

//				//!!!!send command to execute on server

//				Console.WriteLine( "IMPL" );


//				return;
//			}

//			if( message == "Finished" )
//			{
//				finishedResult = bool.Parse( data );
//				return;
//			}
//		}

//		public BuildServerClientNode Client
//		{
//			get { return client; }
//		}

//		public bool? FinishedResult
//		{
//			get { return finishedResult; }
//		}

//		public Compile.CompileFileParser Parser
//		{
//			get { return parser; }
//		}
//	}
//}
