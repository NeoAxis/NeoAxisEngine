// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using NeoAxis;
using NeoAxis.Networking;

namespace CommandLineTools
{
	public static class SynchronizeFiles
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 30.0;

		static string serverAddress = "";
		static PlatformClientNode connectionNode;
		static DateTime updateLastTime;

		static string sourceDirectoryFullPath;
		static string targetRemoteDirectory;
		static volatile bool exitRequested;

		///////////////////////////////////////////////

		public static bool Process()
		{
			try
			{
				Console.WriteLine( "CommandLineTools: Synchronize Files." );

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-synchronizeFiles", out var sourceDirectoryRelative ) )
					return false;
				if( string.IsNullOrEmpty( sourceDirectoryRelative ) )
				{
					Console.WriteLine( "Error: Source directory is not specified." );
					return false;
				}

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-targetRemoteDirectory", out targetRemoteDirectory ) )
					return false;
				if( string.IsNullOrEmpty( targetRemoteDirectory ) )
				{
					Console.WriteLine( "Error: Target remote directory is not specified." );
					return false;
				}

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-server", out serverAddress ) )
					return false;
				if( string.IsNullOrEmpty( serverAddress ) )
				{
					Console.WriteLine( "Error: Server address is not specified." );
					return false;
				}

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-password", out var password ) )
					return false;
				if( string.IsNullOrEmpty( password ) )
				{
					Console.WriteLine( "Error: Password is not specified." );
					return false;
				}

				sourceDirectoryFullPath = Path.GetFullPath( Path.Combine( AppContext.BaseDirectory, sourceDirectoryRelative ) );
				if( !Directory.Exists( sourceDirectoryFullPath ) )
				{
					Console.WriteLine( "Error: Source directory does not exist: " + sourceDirectoryFullPath );
					return false;
				}

				//connect to server
				Console.WriteLine( "Connecting to server " + serverAddress + "..." );
				var username = "None";
				if( !ConnectToServer( username, password, out var error ) )
				{
					Console.WriteLine( "Error connecting to server: " + error );
					return false;
				}

				//loop
				while( !exitRequested )
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
				Destroy();
			}
			catch( Exception ex )
			{
				Console.WriteLine( $"Error: {ex.Message}" );
			}

			return false;
		}

		///////////////////////////////////////////////

		public class PlatformClientNode : ClientNode
		{
			//services
			ClientNetworkService_CloudFunctions cloudFunctions;
			ClientNetworkService_Messages messages;
			ClientNetworkService_Users users;

			//

			public PlatformClientNode()
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
		}

		///////////////////////////////////////////////

		static bool ConnectToServer( string directModeUsername, string directModePassword, out string error )
		{
			Destroy();

			error = "";

			connectionNode = new PlatformClientNode();
			connectionNode.ProtocolError += Client_ProtocolError;
			connectionNode.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			connectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;

			var block = new TextBlock();
			block.SetAttribute( "Username", directModeUsername );
			block.SetAttribute( "Password", directModePassword );

			var loginData = block.DumpToString();

			var webSocketPort = PlatformServer.AllowWebSocket ? PlatformServer.ServerPort : 0;
			var udpPort = PlatformServer.AllowUDP ? PlatformServer.ServerPort + 1 : 0;

			if( !connectionNode.BeginConnect( false, serverAddress, webSocketPort, udpPort, EngineInfo.Version, loginData, out error ) )
			{
				Destroy();
				return false;
			}

			return true;
		}

		private static void Client_ProtocolError( ClientNode sender, string message )
		{
			Log.Warning( "PlatformClient: Protocol error: " + message );
		}

		static void Destroy()
		{
			if( connectionNode != null )
			{
				connectionNode.ProtocolError -= Client_ProtocolError;
				connectionNode.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				connectionNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;

				connectionNode.Dispose();
				connectionNode = null;
			}
		}

		static void Tick()
		{
			var utcNow = DateTime.UtcNow;
			if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			{
				updateLastTime = utcNow;
				connectionNode?.Update( utcNow );
			}
		}

		static void Client_ConnectionStatusChanged( ClientNode sender )
		{
			Console.WriteLine( "Connection status changed: " + sender.Status.ToString() );

			switch( sender.Status )
			{
			case NetworkStatus.Connected:
				{
					Task.Run( async delegate ()
					{
						await SynchronizeAsync();
					} );
				}
				break;
			}
		}

		async static Task SynchronizeAsync()
		{
			try
			{
				//create target directory on server if it does not exist
				{
					Console.WriteLine( "Creating target directory on server if it does not exist..." );

					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var callResult = await connectionNode.CloudFunctions.CreateDirectoryAsync( ClientNetworkService_CloudFunctions.DataSource.Project, targetRemoteDirectory, cancellationToken: cts.Token );
					if( !string.IsNullOrEmpty( callResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + callResult.Error );
						exitRequested = true;
						return;
					}
				}

				//get current files on the server
				ClientNetworkService_CloudFunctions.GetDirectoryInfoResult.Item[] serverFileItems;
				ClientNetworkService_CloudFunctions.GetDirectoryInfoResult.Item[] serverDirectoryItems;
				{
					Console.WriteLine( "Getting directory info from server..." );

					var cts = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );
					var getDirectoryInfoResult = await connectionNode.CloudFunctions.GetDirectoryInfoAsync( ClientNetworkService_CloudFunctions.DataSource.Project, targetRemoteDirectory, "*", SearchOption.AllDirectories, cancellationToken: cts.Token );
					if( !string.IsNullOrEmpty( getDirectoryInfoResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + getDirectoryInfoResult.Error );
						exitRequested = true;
						return;
					}
					var serverItems = getDirectoryInfoResult.Items;
					serverFileItems = serverItems.Where( i => !i.IsDirectory ).ToArray();
					serverDirectoryItems = serverItems.Where( i => i.IsDirectory ).ToArray();
				}
				Console.WriteLine( $"Files on server: {serverFileItems.Length}" );

				//get server files in dictionary for quick access
				var serverFilesDictionary = new Dictionary<string, ClientNetworkService_CloudFunctions.GetDirectoryInfoResult.Item>();
				foreach( var serverFileItem in serverFileItems )
				{
					var relativePath = serverFileItem.PathNormalized.Substring( targetRemoteDirectory.Length ).TrimStart( '\\', '/' );
					serverFilesDictionary[ relativePath ] = serverFileItem;
				}

				//get local files
				var localDirectoryInfo = new DirectoryInfo( sourceDirectoryFullPath );
				var localFiles = localDirectoryInfo.GetFiles( "*", SearchOption.AllDirectories );
				Console.WriteLine( $"Files locally: {localFiles.Length}" );

				//get local files in dictionary for quick access
				var localFilesDictionary = new Dictionary<string, FileInfo>();
				foreach( var localFile in localFiles )
				{
					var relativePath = localFile.FullName.Substring( sourceDirectoryFullPath.Length ).TrimStart( '\\', '/' );
					localFilesDictionary[ relativePath ] = localFile;
				}

				//compare local and server files

				var filesToCopy = new List<string>();
				var filesToCopyLength = 0L;
				var filesToDelete = new List<string>();
				var directoriesToDelete = new List<string>();

				//get files to copy
				foreach( var localFileItem in localFiles )
				{
					var relativePath = localFileItem.FullName.Substring( sourceDirectoryFullPath.Length ).TrimStart( '\\', '/' );

					var exists = serverFilesDictionary.TryGetValue( relativePath, out var serverItem );
					if( !exists || serverItem.Size != localFileItem.Length || localFileItem.LastWriteTimeUtc > serverItem.LastModifiedUtc )
					{
						filesToCopy.Add( relativePath );
						filesToCopyLength += localFileItem.Length;
					}
				}

				//get files to delete
				foreach( var serverFileItem in serverFileItems )
				{
					var relativePath = serverFileItem.PathNormalized.Substring( targetRemoteDirectory.Length ).TrimStart( '\\', '/' );
					var exists = localFilesDictionary.TryGetValue( relativePath, out var localFile );
					if( !exists )
						filesToDelete.Add( Path.Combine( targetRemoteDirectory, relativePath ) );
				}

				//get empty directories to delete
				foreach( var serverDirectoryItem in serverDirectoryItems )
				{
					var relativePath = serverDirectoryItem.PathNormalized.Substring( targetRemoteDirectory.Length ).TrimStart( '\\', '/' );
					var exists = localFilesDictionary.Keys.Any( k => k.StartsWith( relativePath + "/" ) || k.StartsWith( relativePath + "\\" ) );
					if( !exists )
						directoriesToDelete.Add( Path.Combine( targetRemoteDirectory, relativePath ) );
				}

				Console.WriteLine( "Files to copy: " + filesToCopy.Count + " (" + StringUtility.FormatSize( filesToCopyLength ) + ")" );
				Console.WriteLine( "Files to delete: " + filesToDelete.Count );
				Console.WriteLine( "Directories to delete: " + directoriesToDelete.Count );

				Console.WriteLine( "Uploading files..." );

				//upload files
				{
					var sourceFullPaths = new List<string>();
					var targetFilePaths = new List<string>();
					foreach( var fileToCopy in filesToCopy )
					{
						sourceFullPaths.Add( Path.Combine( sourceDirectoryFullPath, fileToCopy ) );
						targetFilePaths.Add( Path.Combine( targetRemoteDirectory, fileToCopy ).Replace( '\\', '/' ) );
					}

					var cts = new CancellationTokenSource( new TimeSpan( 24, 0, 0 ) );
					var callResult = await connectionNode.CloudFunctions.UploadFilesAsync( ClientNetworkService_CloudFunctions.DataSource.Project, sourceFullPaths.ToArray(), targetFilePaths.ToArray(), cancellationToken: cts.Token );

					if( !string.IsNullOrEmpty( callResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + callResult.Error );
						exitRequested = true;
						return;
					}
				}

				Console.WriteLine( "Deleting files and directories..." );

				//delete files and empty directories
				{
					var itemsToDelete = new List<ClientNetworkService_CloudFunctions.DeleteObjectsItem>();
					foreach( var file in filesToDelete )
						itemsToDelete.Add( new ClientNetworkService_CloudFunctions.DeleteObjectsItem { Path = file, IsDirectory = false } );
					foreach( var directory in directoriesToDelete )
						itemsToDelete.Add( new ClientNetworkService_CloudFunctions.DeleteObjectsItem { Path = directory, IsDirectory = true } );

					var cts = new CancellationTokenSource( new TimeSpan( 0, 5, 0 ) );
					var callResult = await connectionNode.CloudFunctions.DeleteObjectsAsync( ClientNetworkService_CloudFunctions.DataSource.Project, itemsToDelete.ToArray(), cancellationToken: cts.Token );

					if( !string.IsNullOrEmpty( callResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + callResult.Error );
						exitRequested = true;
						return;
					}
				}

				Console.WriteLine( "Synchronization completed." );
				exitRequested = true;
			}
			catch( Exception e )
			{
				Console.WriteLine( "EXCEPTION: " + e.ToString() );
				exitRequested = true;
			}
		}

		private static void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			if( message == "ShowMessage" )
				Console.WriteLine( data );
		}
	}
}