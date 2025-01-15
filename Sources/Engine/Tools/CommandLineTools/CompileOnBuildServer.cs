//!!!!update networking

//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System.Text;
//using System.Xml;
//using System.Diagnostics;
//using NeoAxis;
//using NeoAxis.Networking;

//namespace CommandLineTools
//{
//	static class CompileOnBuildServer
//	{
//		public static List<FileItem> FilesToUpload = new List<FileItem>();

//		/////////////////////

//		public class FileItem
//		{
//			public string Name;
//			public long Length;
//			public uint CRC;

//			public FileItem() { }

//			public FileItem( string name, long length, uint crc )
//			{
//				Name = name;
//				Length = length;
//				CRC = crc;
//			}
//		}

//		/////////////////////

//		static string GetMD5( string input )
//		{
//			// Use input string to calculate MD5 hash
//			using( System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create() )
//			{
//				byte[] inputBytes = Encoding.ASCII.GetBytes( input );
//				byte[] hashBytes = md5.ComputeHash( inputBytes );

//				// Convert the byte array to hexadecimal string
//				var sb = new StringBuilder();
//				for( int i = 0; i < hashBytes.Length; i++ )
//					sb.Append( hashBytes[ i ].ToString( "X2" ) );
//				return sb.ToString();
//			}
//		}

//		//static string GetMachindGUID()
//		//{
//		//	var regPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Cryptography";
//		//	return (string)Registry.GetValue( regPath, "MachineGuid", "" );
//		//}



//		//class RequestServiceResult
//		//{
//		//	public string ServerAddress;
//		//	public int ServerPort;
//		//	public string VerificationCode;
//		//	public string Error;
//		//}

//		//static async Task<RequestServiceResult> RequestServiceAsync( string service )
//		//{
//		//	//request keys from general manager
//		//	GeneralManagerExecuteCommand.ResultClass requestResult;
//		//	{
//		//		var command = new GeneralManagerExecuteCommand();
//		//		command.FunctionName = "api/request_service";
//		//		command.RequireUserLogin = true;
//		//		command.Parameters.Add( ("service", service) );
//		//		requestResult = await command.ExecuteAsync();
//		//	}

//		//	if( !string.IsNullOrEmpty( requestResult.Error ) )
//		//	{
//		//		return new RequestServiceResult()
//		//		{
//		//			Error = requestResult.Error
//		//		};
//		//	}

//		//	var requestBlock = requestResult.Data;

//		//	return new RequestServiceResult()
//		//	{
//		//		ServerAddress = requestBlock.GetAttribute( "ServerAddress" ),
//		//		ServerPort = int.Parse( requestBlock.GetAttribute( "ServerPort" ) ),
//		//		VerificationCode = requestBlock.GetAttribute( "VerificationCode" ),
//		//	};
//		//}

//		public static async Task<bool> Process( Compile.CompileFileParser parser )
//		{
//			if( !SystemSettings.CommandLineParameters.TryGetValue( "-buildServer", out var buildServer ) )
//				return false;

//			string verificationCode = "";
//			if( buildServer.ToLower() != "cloud" )
//			{
//				if( !SystemSettings.CommandLineParameters.TryGetValue( "-verificationCode", out verificationCode ) )
//				{
//					Console.WriteLine( "Error: -verificationCode is not specified." );
//					return false;
//				}
//			}

//			//!!!!maybe add more special info to md5 to make unique compilation ID

//			//The CompilationID is a local folder name on the server
//			var md5 = GetMD5( GetMD5( parser.CompileFilePath ) + GetMD5( parser.RootPath ) + GetMD5( File.ReadAllText( parser.CompileFilePath ) ) );
//			var compilationID = Path.GetFileName( parser.CompileFilePath ).Replace( '.', '_' ) + "_" + md5;

//			//Log.Fatal( compilationID );
//			//return false;


//			try
//			{
//				//!!!!no non cloud support
//				var port = 0;

//				//request server address from cloud service
//				if( buildServer.ToLower() == "cloud" )
//				{
//					Console.WriteLine( "Connecting to the cloud service..." );

//					var requestResult = await GeneralManagerFunctions.RequestServiceAsync( "Build" );
//					//var requestResult = await RequestRequestBuildServerAsync( compilationID );
//					if( !string.IsNullOrEmpty( requestResult.Error ) )
//					{
//						Console.WriteLine( "Error: Unable to get server address from the cloud service. " + requestResult.Error );
//						return false;
//					}
//					buildServer = requestResult.ServerAddress;
//					port = requestResult.ServerPort;
//					verificationCode = requestResult.VerificationCode;
//				}

//				Console.WriteLine( "Connecting to " + buildServer + "..." );

//				//connect to the server
//				if( !BuildServerClient.BeginConnect( buildServer, port, compilationID, verificationCode, parser, out var error2 ) )
//				{
//					Console.WriteLine( "Error: " + error2 );
//					return false;
//				}

//				//get connection instance
//				var instances = BuildServerClient.GetInstances();
//				if( instances.Length == 0 )
//				{
//					Console.WriteLine( "Error: instances.Length == 0." );
//					return false;
//				}
//				var instance = instances[ 0 ];

//				//wait for establishing connection
//				while( instance.Client.Status == NetworkStatus.Connecting )
//				{
//					BuildServerClient.Update();
//					Thread.Sleep( 10 );
//				}
//				if( instance.Client.Status != NetworkStatus.Connected )
//				{
//					Console.WriteLine( "Error: instance.Client.Status != NetworkStatus.Connected." );
//					return false;
//				}

//				//now connected


//				//calculate FilesToUpload
//				{
//					FilesToUpload.Clear();


//					//!!!!на сервере нужно размещать файлы глубже в директориях, так как часть исходников выше из-за ".."
//					//тогда нужно указывать глубину. или подсчитать по путям
//					//или .compile файл выше положить


//					//!!!!temp


//					var files = new DirectoryInfo( parser.RootPath ).GetFiles( "*.*" );
//					foreach( var file in files )
//					{
//						file.Refresh();//sense?

//						var fileItem = new FileItem();
//						fileItem.Name = Path.GetRelativePath( parser.RootPath, file.FullName );
//						fileItem.Length = file.Length;

//						var bytes = File.ReadAllBytes( file.FullName );
//						var crc32 = new CRC32();
//						byte[] checksum = crc32.ComputeHash( bytes );
//						fileItem.CRC = BitConverter.ToUInt32( checksum, 0 );

//						FilesToUpload.Add( fileItem );
//					}

//					Console.WriteLine( $"Files to upload {FilesToUpload.Count}." );
//				}

//				//start from sending the message to get current a list of files
//				{
//					Console.WriteLine( "Requesting a list of files from the server..." );
//					instance.Client.Messages.SendToServer( "RequestFiles", "" );
//				}

//				//work
//				while( instance.Client.Status != NetworkStatus.Disconnected )
//				{
//					BuildServerClient.Update();
//					Thread.Sleep( 10 );
//				}

//				//end
//				if( instance.FinishedResult != null )
//				{
//					if( instance.FinishedResult.Value )
//					{
//						Console.WriteLine( "END SUCCESS." );
//						return true;
//					}
//					else
//					{
//						Console.WriteLine( "END FAILED." );
//						return false;
//					}
//				}
//				else
//				{
//					var text = "END NOT FINISHED.";
//					if( instance.Client != null )
//						text += " " + instance.Client.DisconnectionReason;
//					Console.WriteLine( text );
//					return false;
//				}
//			}
//			catch( Exception e )
//			{
//				Console.WriteLine( "Exception: " + e.Message );
//				return false;
//			}
//			finally
//			{
//				BuildServerClient.AppDestroy();
//			}
//		}
//	}
//}