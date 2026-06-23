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
	public static class CompileRemote
	{
		public static double UpdateFrequency { get; set; } = 1.0 / 30.0;

		static string serverAddress = "";
		static PlatformClientNode connectionNode;
		static DateTime updateLastTime;

		static CompileFileParser parser;

		static string taskID;
		static volatile bool exitRequested;

		///////////////////////////////////////////////

		public static bool Process()
		{
			try
			{
				Console.WriteLine( "CommandLineTools: Compile Remote." );

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-compileRemote", out var compileFilePath ) )
					return false;
				if( string.IsNullOrEmpty( compileFilePath ) )
				{
					Console.WriteLine( "Error: Compile file is not specified." );
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

				var executableDirectory = AppContext.BaseDirectory;
				var compileFileFullPath = Path.GetFullPath( Path.Combine( executableDirectory, compileFilePath ) );

				//parse compile file to get Source directories and source file extensions
				parser = new CompileFileParser( compileFileFullPath, true );
				parser.Parse();
				//parser.Print();
				Console.WriteLine( "File to compile: " + compileFileFullPath );

				//delete output file if exists
				{
					var outputFullPath = parser.GetFullSourcePath( parser.OutputFilePath );
					if( File.Exists( outputFullPath ) )
						File.Delete( outputFullPath );
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
						await CompileAsync();
					} );
				}
				break;
			}
		}

		async static Task CompileAsync()
		{
			try
			{
				//create task
				//string taskID;
				{
					var cts = new CancellationTokenSource( new TimeSpan( 0, 0, 15 ) );
					var callResult = await connectionNode.CloudFunctions.CallMethodAsync<string>( "PlatformServer", "CompileCreate", cts.Token );
					if( !string.IsNullOrEmpty( callResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + callResult.Error );
						exitRequested = true;
						return;
					}
					taskID = callResult.Value;
					Console.WriteLine( "Task was created: " + taskID );
				}

				//upload files
				{
					Console.WriteLine( "Source Root Folder: " + parser.Remote_SourceRootFolder );

					var sourceFullPaths = new List<string>();
					var targetFilePaths = new List<string>();
					var totalFileLength = 0L;

					foreach( var sourceDirectory in parser.Remote_SourceFolders )
					{
						var sourceDirectoryFullPath = Path.GetFullPath( Path.Combine( parser.Remote_SourceRootFolder, sourceDirectory ) );

						foreach( var sourceFileExtension in parser.Remote_SourceFileExtensions )
						{
							var directoryInfo = new DirectoryInfo( sourceDirectoryFullPath );

							var files = directoryInfo.GetFiles( "*." + sourceFileExtension, SearchOption.AllDirectories );
							foreach( var fileInfo in files )
							{
								var filePath = fileInfo.FullName;

								var relativePath = filePath.Substring( parser.Remote_SourceRootFolder.Length ).TrimStart( '\\', '/' );
								sourceFullPaths.Add( filePath );
								targetFilePaths.Add( relativePath ); ////targetFilePaths.Add( Path.Combine( taskID, relativePath ) );

								totalFileLength += fileInfo.Length;
							}
						}
					}

					Console.WriteLine( $"{sourceFullPaths.Count} files to upload ({StringUtility.FormatSize( totalFileLength )})..." );

					var tempFileName = Path.Combine( Path.GetTempPath(), "CompileRemote_" + taskID + ".zip" );

					try
					{
						//make zip archive
						using( var zip = ZipFile.Open( tempFileName, ZipArchiveMode.Create ) )
						{
							for( int n = 0; n < sourceFullPaths.Count; n++ )
								zip.CreateEntryFromFile( sourceFullPaths[ n ], targetFilePaths[ n ].Replace( '\\', '/' ) );
						}

						Console.WriteLine( $"Uploading zip archive ({StringUtility.FormatSize( new FileInfo( tempFileName ).Length )})..." );

						var targetZipFilePath = Path.Combine( taskID, "Source.zip" );

						var cts = new CancellationTokenSource( new TimeSpan( 0, 20, 0 ) );
						var callResult = await connectionNode.CloudFunctions.UploadFileAsync( ClientNetworkService_CloudFunctions.DataSource.Project, tempFileName, targetZipFilePath, cancellationToken: cts.Token );

						if( !string.IsNullOrEmpty( callResult.Error ) )
						{
							Console.WriteLine( "ERROR: " + callResult.Error );
							exitRequested = true;
							return;
						}

						////var cts = new CancellationTokenSource( new TimeSpan( 0, 10, 0 ) );
						////var callResult = await connectionNode.CloudFunctions.UploadFilesAsync( ClientNetworkService_CloudFunctions.DataSource.Project, sourceFullPaths.ToArray(), targetFilePaths.ToArray(), cancellationToken: cts.Token );
						////if( !string.IsNullOrEmpty( callResult.Error ) )
						////{
						////	Console.WriteLine( "ERROR: " + callResult.Error );
						////	exitRequested = true;
						////	return;
						////}
					}
					finally
					{
						if( File.Exists( tempFileName ) )
							File.Delete( tempFileName );
					}
				}

				//start compilation
				{
					Console.WriteLine( "Starting compilation..." );

					var compileFileFullPath = parser.CompileFileFullPath;
					var compileFilePath = compileFileFullPath.Substring( parser.Remote_SourceRootFolder.Length ).TrimStart( '\\', '/' );

					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var callResult = await connectionNode.CloudFunctions.CallMethodAsync<string>( "PlatformServer", "CompileStart", cts.Token, taskID, compileFilePath );
					if( !string.IsNullOrEmpty( callResult.Error ) )
					{
						Console.WriteLine( "ERROR: " + callResult.Error );
						exitRequested = true;
						return;
					}
					Console.WriteLine( "Compilation started." );
				}


				////!!!!test download
				//{
				//	Console.WriteLine( $"Test download zip archive" );

				//	var sourceZipFilePath = Path.Combine( taskID, "Source.zip" );
				//	var targetFullPath = @"C:\_______TestCompile\_Source.zip";

				//	var cts = new CancellationTokenSource( new TimeSpan( 0, 20, 0 ) );
				//	var callResult = await connectionNode.CloudFunctions.DownloadFileAsync( ClientNetworkService_CloudFunctions.DataSource.Project, sourceZipFilePath, targetFullPath, false, cancellationToken: cts.Token );

				//	if( !string.IsNullOrEmpty( callResult.Error ) )
				//	{
				//		Console.WriteLine( "ERROR: " + callResult.Error );
				//		exitRequested = true;
				//		return;
				//	}

				//	Console.WriteLine( "done" );
				//}


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

			if( message == "CompileResult" )
			{
				bool.TryParse( data, out var success );

				if( success )
				{
					Console.WriteLine( "Compilation completed successfully." );

					//copy output file from the server to local disk
					Task.Run( async delegate ()
					{
						try
						{
							var sourceFilePath = Path.Combine( taskID, Path.GetFileName( parser.OutputFilePath ) );

							//var sourceFilePath = Path.Combine( taskID, parser.CompileFileDirectory, parser.TempFolder, Path.GetFileName( parser.OutputFilePath ) );
							//var sourceFilePath = Path.Combine( taskID, parser.TempFolder, Path.GetFileName( parser.OutputFilePath ) );

							var outputFullPath = parser.GetFullSourcePath( parser.OutputFilePath );

							Console.WriteLine( "Downloading output file..." );

							var cts = new CancellationTokenSource( new TimeSpan( 0, 5, 0 ) );
							var callResult = await connectionNode.CloudFunctions.DownloadFileAsync( ClientNetworkService_CloudFunctions.DataSource.Project, sourceFilePath, outputFullPath, false, cancellationToken: cts.Token );

							if( !string.IsNullOrEmpty( callResult.Error ) )
							{
								Console.WriteLine( "ERROR: " + callResult.Error );
								exitRequested = true;
								return;
							}

							Console.WriteLine( "Download completed." );
							Console.WriteLine( "Output file: " + outputFullPath );
							exitRequested = true;
						}
						catch( Exception e )
						{
							Console.WriteLine( "EXCEPTION: " + e.ToString() );
						}
					} );
				}
				else
				{
					Console.WriteLine( "Compilation failed." );
					exitRequested = true;
					return;
				}
			}
		}
	}
}