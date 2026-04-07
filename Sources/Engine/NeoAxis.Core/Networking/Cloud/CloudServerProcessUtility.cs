#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;

namespace NeoAxis.Networking
{
	public static class CloudServerProcessUtility
	{
		static bool initialized;
		static long requestToServerManagerCounter;

		static DateTime deleteOldRequestsLastTime;

		static GetServerInfoResult getServerInfoResultCached = new GetServerInfoResult();
		static DateTime getServerInfoResultCachedTime;

		static int currentServerWorkload = -1;

		////////////////////////////////////////////////

		public static class CommandLineParameters
		{
			internal static string serverAddress = "";
			internal static int serverPort;
			internal static string serverCheckCode = "";
			internal static string serverPasswordHash;
			internal static long userID;
			internal static long projectID;
			internal static string projectDirectory;
			internal static string projectName = "";
			//internal static string projectCurrency = "";
			internal static bool projectInAppPurchase;
			internal static bool projectInAppWithdraw;
			internal static bool projectInAppProfit;
			internal static int projectHorizontalServersMaxCount;
			internal static string processSettings = "";
			internal static TextBlock processSettingsTextBlock;
			internal static bool horizontalServer;

			//

			public static string ServerAddress
			{
				get
				{
					Init();
					return serverAddress;
				}
			}

			public static int ServerPort
			{
				get
				{
					Init();
					return serverPort;
				}
			}

			public static string ServerCheckCode
			{
				get
				{
					Init();
					return serverCheckCode;
				}
				//set method for Server Manager
				set
				{
					serverCheckCode = value;
				}
			}

			public static string ServerPasswordHash
			{
				get
				{
					Init();
					return serverPasswordHash;
				}
			}

			public static long UserID
			{
				get
				{
					Init();
					return userID;
				}
			}

			public static long ProjectID
			{
				get
				{
					Init();
					return projectID;
				}
			}

			public static string ProjectDirectory
			{
				get
				{
					Init();
					return projectDirectory;
				}
			}

			public static string ProjectName
			{
				get
				{
					Init();
					return projectName;
				}
			}

			//public static string ProjectCurrency
			//{
			//	get
			//	{
			//		Init();
			//		return projectCurrency;
			//	}
			//}

			public static bool ProjectInAppPurchase
			{
				get
				{
					Init();
					return projectInAppPurchase;
				}
			}

			public static bool ProjectInAppWithdraw
			{
				get
				{
					Init();
					return projectInAppWithdraw;
				}
			}

			public static bool ProjectInAppProfit
			{
				get
				{
					Init();
					return projectInAppProfit;
				}
			}

			public static int ProjectHorizontalServersMaxCount
			{
				get
				{
					Init();
					return projectHorizontalServersMaxCount;
				}
			}

			public static string ProcessSettings
			{
				get
				{
					Init();
					return processSettings;
				}
			}

			public static TextBlock ProcessSettingsTextBlock
			{
				get
				{
					if( processSettingsTextBlock == null )
					{
						processSettingsTextBlock = TextBlock.Parse( ProcessSettings, out _ );
						if( processSettingsTextBlock == null )
							processSettingsTextBlock = new TextBlock();
					}
					return processSettingsTextBlock;
				}
			}

			public static bool HorizontalServer
			{
				get
				{
					Init();
					return horizontalServer;
				}
			}
		}

		////////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error;
		}

		////////////////////////////////////////////////

		static void Init()
		{
			if( !initialized )
			{
				//get serverAddress
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverAddress", out CommandLineParameters.serverAddress ) )
				{
					//Logs.Write( "Common", "Init: '-serverAddress' is not specified." );
					//return false;
				}

				//get serverPort
				SystemSettings.CommandLineParameters.TryGetValue( "-serverPort", out var serverPortString );
				int.TryParse( serverPortString, out CommandLineParameters.serverPort );

				//get serverCheckCode
				if( SystemSettings.CommandLineParameters.ContainsKey( "-serverCheckCode" ) )
					SystemSettings.CommandLineParameters.TryGetValue( "-serverCheckCode", out CommandLineParameters.serverCheckCode );

				//get serverPasswordHash
				SystemSettings.CommandLineParameters.TryGetValue( "-serverPasswordHash", out CommandLineParameters.serverPasswordHash );

				//get userID
				SystemSettings.CommandLineParameters.TryGetValue( "-userID", out var userIDString );
				long.TryParse( userIDString, out CommandLineParameters.userID );

				//get projectID
				SystemSettings.CommandLineParameters.TryGetValue( "-projectID", out var projectIDString );
				long.TryParse( projectIDString, out CommandLineParameters.projectID );

				//get projectDirectory
				if( SystemSettings.CommandLineParameters.TryGetValue( "-projectDirectory", out var projectDirectoryEncoded ) )
					CommandLineParameters.projectDirectory = StringUtility.DecodeFromBase64URL( projectDirectoryEncoded );
				//SystemSettings.CommandLineParameters.TryGetValue( "-projectDirectory", out CommandLineParameters.projectDirectory );

				//get projectName
				if( SystemSettings.CommandLineParameters.TryGetValue( "-projectName", out var projectNameEncoded ) )
					CommandLineParameters.projectName = StringUtility.DecodeFromBase64URL( projectNameEncoded );

				////get projectCurrency
				//SystemSettings.CommandLineParameters.TryGetValue( "-projectCurrency", out CommandLineParameters.projectCurrency );

				//get projectInAppPurchase
				if( SystemSettings.CommandLineParameters.TryGetValue( "-inAppPurchase", out var inAppPurchaseString ) )
					bool.TryParse( inAppPurchaseString, out CommandLineParameters.projectInAppPurchase );

				//get projectInAppWithdraw
				if( SystemSettings.CommandLineParameters.TryGetValue( "-inAppWithdraw", out var inAppWithdrawString ) )
					bool.TryParse( inAppWithdrawString, out CommandLineParameters.projectInAppWithdraw );

				//get projectInAppProfit
				if( SystemSettings.CommandLineParameters.TryGetValue( "-inAppProfit", out var inAppProfitString ) )
					bool.TryParse( inAppProfitString, out CommandLineParameters.projectInAppProfit );

				//get projectHorizontalServersMaxCount
				if( SystemSettings.CommandLineParameters.TryGetValue( "-horizontalServersMaxCount", out var horizontalServersMaxCountString ) )
					int.TryParse( horizontalServersMaxCountString, out CommandLineParameters.projectHorizontalServersMaxCount );

				//get processSettings
				if( SystemSettings.CommandLineParameters.TryGetValue( "-processSettings", out var processSettingsEncoded ) )
					CommandLineParameters.processSettings = StringUtility.DecodeFromBase64URL( processSettingsEncoded );

				//get horizontalServer
				if( SystemSettings.CommandLineParameters.TryGetValue( "-horizontalServer", out var horizontalServerString ) )
					bool.TryParse( horizontalServerString, out CommandLineParameters.horizontalServer );

				initialized = true;
			}
		}

		public static string MessageToServerManagerTxtFullPath
		{
			get { return $"/home/Data/_messages/MessageToServerManager_{CommandLineParameters.ProjectID}.txt"; }
		}

		public static string ServerDataDirectory
		{
			get { return "/home/Data"; }
		}

		public static string ProjectsDirectory
		{
			get { return "/home/Data/Projects"; }
		}

		public static string TempFilesDirectory
		{
			get { return "/home/Data/_temp"; }
		}

		public static string StorageDirectory
		{
			get { return "/home/Data/Storage"; }
		}

		public static string GetUniqueTempFileName( string extension = null )
		{
			var ext = !string.IsNullOrEmpty( extension ) ? extension : ".tmp";
			if( ext[ 0 ] != '.' )
				ext = "." + ext;

			while( true )
			{
				var tempPath = Path.Combine( TempFilesDirectory, ( new Random().NextDouble() * 1000 ).ToString().Replace( ".", "" ).Replace( ",", "" ) + ext );
				if( !File.Exists( tempPath ) )
					return tempPath;
			}
		}


		//!!!! int maxBytesPerSecond = 0

		public class RequestToServerManagerResult
		{
			public TextBlock Answer;
			public string Error;
		}

		public static async Task<RequestToServerManagerResult> RequestToServerManagerAsync( string requestText, CancellationToken cancellationToken = default )
		{
			var requestsDirectory = "/home/Data/_requestsToServerManager";
			var answersDirectory = "/home/Data/_answersFromServerManager";

			var now = DateTime.UtcNow;

			//delete old requests to prevent overflow
			if( ( now - deleteOldRequestsLastTime ).TotalSeconds > 10 )
			{
				deleteOldRequestsLastTime = now;

				try
				{
					var directoryInfo = new DirectoryInfo( requestsDirectory );
					if( directoryInfo.Exists )
					{
						foreach( var fileInfo in directoryInfo.GetFiles( "*.txt", SearchOption.TopDirectoryOnly ) )
						{
							if( ( now - fileInfo.LastWriteTimeUtc ).TotalSeconds > 30 )
							{
								try
								{
									fileInfo.Delete();
								}
								catch { }
							}
						}
					}
				}
				catch { }
			}

			//add request
			string requestName;
			try
			{
				var id = Interlocked.Increment( ref requestToServerManagerCounter );
				var processId = Process.GetCurrentProcess().Id;
				requestName = $"{CommandLineParameters.ProjectID}_{processId}_{id}.txt";

				Directory.CreateDirectory( requestsDirectory );
				var path = Path.Combine( requestsDirectory, requestName );
				File.WriteAllText( path, requestText + "[[!END!]]" );
			}
			catch( Exception e )
			{
				return new RequestToServerManagerResult() { Error = e.Message };
			}


			//wait for answer
			var answerPath = Path.Combine( answersDirectory, requestName );
			try
			{
				while( true )
				{
					try
					{
						if( File.Exists( answerPath ) )
						{
							var text = File.ReadAllText( answerPath );
							var index = text.IndexOf( "[[!END!]]" );
							if( index >= 0 )
							{
								text = text.Substring( 0, index );

								//read answer
								var rootBlock = TextBlock.Parse( text, out var error );
								if( !string.IsNullOrEmpty( error ) )
									return new RequestToServerManagerResult() { Error = "Invalid answer. " + error };
								if( rootBlock != null )
								{
									error = rootBlock.GetAttribute( "Error" );
									if( !string.IsNullOrEmpty( error ) )
										return new RequestToServerManagerResult() { Error = error };
									else
										return new RequestToServerManagerResult() { Answer = rootBlock };
								}
							}
						}
					}
					catch { }

					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
					{
						//add request to cancel another request

						var requestBlock = new TextBlock();
						requestBlock.SetAttribute( "Command", "CancelRequest" );
						requestBlock.SetAttribute( "AnswerPath", answerPath );
						var requestText2 = requestBlock.DumpToString( false );

						try
						{
							var id = Interlocked.Increment( ref requestToServerManagerCounter );
							var processId = Process.GetCurrentProcess().Id;
							requestName = $"{CommandLineParameters.ProjectID}_{processId}_{id}.txt";

							Directory.CreateDirectory( requestsDirectory );
							var path = Path.Combine( requestsDirectory, requestName );
							File.WriteAllText( path, requestText2 + "[[!END!]]" );
						}
						catch( Exception e )
						{
							return new RequestToServerManagerResult() { Error = e.Message };
						}

						return new RequestToServerManagerResult() { Error = "Operation was cancelled." };
					}
				}
			}
			catch( Exception e )
			{
				return new RequestToServerManagerResult() { Error = e.Message };
			}
			finally
			{
				//delete answer file
				try
				{
					if( File.Exists( answerPath ) )
						File.Delete( answerPath );
				}
				catch { }
			}
		}

		///////////////////////////////////////////////
		//storage functions from GeneralManagerFunctions with local storage directory management

		public static string GetFilePathByStorageFileName( string storageFileName )
		{
			return Path.Combine( StorageDirectory, VirtualPathUtility.NormalizePath( storageFileName ) );
		}

		///////////////////////////////////////////////
		//storage get files info

		public static async Task<CloudServiceFunctions.StorageGetFilesInfoResult> StorageGetFilesInfoAsync( string[] storageFileNames, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				return new CloudServiceFunctions.StorageGetFilesInfoResult() { Error = "Server check code is not configured." };
			return await CloudServiceFunctions.StorageGetFilesInfoAsync( storageFileNames, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.StorageGetFileInfoResult> StorageGetFileInfoAsync( string storageFileName, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				return new CloudServiceFunctions.StorageGetFileInfoResult() { Error = "Server check code is not configured." };
			return await CloudServiceFunctions.StorageGetFileInfoAsync( storageFileName, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		///////////////////////////////////////////////
		//storage get directory info

		public static async Task<CloudServiceFunctions.StorageGetDirectoryInfoResult> StorageGetDirectoryInfoAsync( string storageDirectory, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );
			return await CloudServiceFunctions.StorageGetDirectoryInfoAsync( storageDirectory, searchPattern, searchOption, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		///////////////////////////////////////////////
		//storage get content urls

		public static async Task<CloudServiceFunctions.StorageGetContentUrlsResult> StorageGetContentUrlsAsync( string[] storageFileNames, bool upload, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );
			return await CloudServiceFunctions.StorageGetContentUrlsAsync( storageFileNames, upload, false, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.StorageGetContentUrlResult> StorageGetContentUrlAsync( string storageFileName, bool upload, bool makePublic = false, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );
			return await CloudServiceFunctions.StorageGetContentUrlAsync( storageFileName, upload, makePublic, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		///////////////////////////////////////////////
		//storage download files

		public class StorageDownloadFilesResult
		{
			public bool WasAlreadyDownloaded;
			public string Error;
		}

		//!!!!add progress callback

		public static async Task<StorageDownloadFilesResult> StorageDownloadFilesAsync( string[] storageFileNames, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				return new StorageDownloadFilesResult() { Error = "Server check code is not configured." };

			var getFilesInfoResult = await CloudServiceFunctions.StorageGetFilesInfoAsync( storageFileNames, CommandLineParameters.ServerCheckCode, cancellationToken );
			if( !string.IsNullOrEmpty( getFilesInfoResult.Error ) )
				return new StorageDownloadFilesResult() { Error = getFilesInfoResult.Error };

			var filesOnStorage = getFilesInfoResult.Items;

			if( getFilesInfoResult.Items.Length != storageFileNames.Length )
				return new StorageDownloadFilesResult() { Error = "Invalid answer from StorageGetFilesInfoAsync." };

			//check all files exist
			for( int n = 0; n < filesOnStorage.Length; n++ )
			{
				if( !filesOnStorage[ n ].Exists )
					return new StorageDownloadFilesResult() { Error = "File is not exists in the Storage." };
			}

			//compare
			var result = new StorageDownloadFilesResult();
			result.WasAlreadyDownloaded = true;

			var filesToDownload = new bool[ storageFileNames.Length ];
			{
				for( int n = 0; n < storageFileNames.Length; n++ )
				{
					var storageFileName = storageFileNames[ n ];
					var fileOnStorage = filesOnStorage[ n ];

					var filePath = GetFilePathByStorageFileName( storageFileName );

					//!!!!compare hashes? then need add hashes to StorageGetFilesInfoResult.Item

					var equal = false;
					try
					{
						var fileInfo = new FileInfo( filePath );
						equal = fileInfo.Exists && fileInfo.Length == fileOnStorage.Size && fileInfo.LastWriteTimeUtc > fileOnStorage.LastModified;
					}
					catch { }

					if( !equal )
					{
						filesToDownload[ n ] = true;
						result.WasAlreadyDownloaded = false;
					}
				}
			}

			if( !result.WasAlreadyDownloaded )
			{
				var storageFileNames2 = storageFileNames.Where( ( f, i ) => filesToDownload[ i ] ).ToArray();

				var getContentUrlsResult = await StorageGetContentUrlsAsync( storageFileNames2, false, cancellationToken );
				if( !string.IsNullOrEmpty( getContentUrlsResult.Error ) )
					return new StorageDownloadFilesResult() { Error = getContentUrlsResult.Error };

				//using( var httpClient = new HttpClient() )
				//{
				for( int n = 0; n < storageFileNames2.Length; n++ )
				{
					var storageFileName = storageFileNames2[ n ];
					var url = getContentUrlsResult.Urls[ n ];
					var targetFullPath = GetFilePathByStorageFileName( storageFileName );


					//check free space before download

					//if( driveInfoCached == null )
					//	driveInfoCached = new DriveInfo( "/" );
					//var freeBytes = driveInfoCached.AvailableFreeSpace;

					//Logs.Write( "General Client", $"DownloadObject; {storageFileName}; {size}; Free space {freeBytes}" );

					//if( freeBytes * 1.1 < size )
					//{
					//	var b = new TextBlock();
					//	b.SetAttribute( "Error", "No free space on the disk." );
					//	AddAnswer( b.DumpToString( false ), answerPath );
					//	return;
					//}


					var lastProgressUpdate = DateTime.UtcNow;
					void Progress( int downloadedIncrement, long totalDownloaded, long totalSize )// int progressPercentage, ref bool mustBreak )
					{
						lastProgressUpdate = DateTime.UtcNow;
					}

					using var cancellationToken2 = new CancellationTokenSource( new TimeSpan( 100, 0, 0 ) );

					var downloadResultTask = NetworkUtility.DownloadFileByUrlAsync( /*httpClient,*/ url, targetFullPath, Progress, cancellationToken );

					while( !downloadResultTask.IsCompleted )
					{
						if( ( DateTime.UtcNow - lastProgressUpdate ).TotalMinutes > 10 /*|| RemoveCancelledRequest( answerPath )*/ )
							cancellationToken2.Cancel();
						Thread.Sleep( 1 );
					}
					var downloadResult = downloadResultTask.Result;

					if( !string.IsNullOrEmpty( downloadResult.Error ) )
						return new StorageDownloadFilesResult() { Error = downloadResult.Error };


					////connection statistics
					//{
					//	var projectItem = ProjectManagement.GetProjectItem( projectID );
					//	projectItem?.AggregatedConnectionStatistics?.AddReceived( size );
					//}

				}
				//}
			}

			return result;
		}

		public static async Task<StorageDownloadFilesResult> StorageDownloadFileAsync( string storageFileName, CancellationToken cancellationToken = default )
		{
			return await StorageDownloadFilesAsync( new string[] { storageFileName }, cancellationToken );
		}

		///////////////////////////////////////////////
		//storage download directory

		public class StorageDownloadDirectoryResult
		{
			public Item[] Items;
			public bool WasAlreadyDownloaded;
			public string Error;

			public struct Item
			{
				public string Name;
				public long Size;
				public DateTime LastModified;
				public bool IsDirectory;
			}
		}

		//!!!!add progress callback

		public static async Task<StorageDownloadDirectoryResult> StorageDownloadDirectoryAsync( string storageDirectory, string searchPattern, SearchOption searchOption, bool deleteExcessEntries, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				return new StorageDownloadDirectoryResult() { Error = "Server check code is not configured." };

			//!!!!where else
			if( !ServerNetworkService_CloudFunctions.IsValidVirtualPath( storageDirectory, true ) )
				throw new Exception( "Invalid storage directory path." );

			var getDirectoryInfoResult = await CloudServiceFunctions.StorageGetDirectoryInfoAsync( storageDirectory, searchPattern, searchOption, CommandLineParameters.ServerCheckCode, cancellationToken );
			if( !string.IsNullOrEmpty( getDirectoryInfoResult.Error ) )
				return new StorageDownloadDirectoryResult() { Error = getDirectoryInfoResult.Error };

			//compare
			var filesToDownload = new List<CloudServiceFunctions.StorageGetDirectoryInfoResult.Item>();
			var fileNamesToDownload = new List<string>();
			//!!!!var directoriesToCreate = new List<GeneralManagerFunctions.StorageGetDirectoryInfoResult.Item>();
			{
				foreach( var item in getDirectoryInfoResult.Items )
				{
					if( !item.IsDirectory )
					{
						var filePath = GetFilePathByStorageFileName( item.Name );

						//!!!!compare hashes? then need add hashes to StorageGetFilesInfoResult.Item

						var equal = false;
						try
						{
							var fileInfo = new FileInfo( filePath );
							equal = fileInfo.Exists && fileInfo.Length == item.Size && fileInfo.LastWriteTimeUtc > item.LastModified;
						}
						catch { }
						if( !equal )
						{
							filesToDownload.Add( item );
							fileNamesToDownload.Add( item.Name );
						}
					}
				}
			}

			var wasAlreadyDownloaded = filesToDownload.Count == 0;

			if( filesToDownload.Count != 0 )
			{
				var getContentUrlsResult = await StorageGetContentUrlsAsync( fileNamesToDownload.ToArray(), false, cancellationToken );
				if( !string.IsNullOrEmpty( getContentUrlsResult.Error ) )
					return new StorageDownloadDirectoryResult() { Error = getContentUrlsResult.Error };

				//using( var httpClient = new HttpClient() )
				//{
				for( int n = 0; n < filesToDownload.Count; n++ )
				{
					var fileToDownload = filesToDownload[ n ];
					var storageFileName = fileToDownload.Name;
					var url = getContentUrlsResult.Urls[ n ];

					var filePath = GetFilePathByStorageFileName( storageFileName );
					var size = fileToDownload.Size;

					{
						var targetFullPath = GetFilePathByStorageFileName( storageFileName );


						////check free space before download

						//if( driveInfoCached == null )
						//	driveInfoCached = new DriveInfo( "/" );
						//var freeBytes = driveInfoCached.AvailableFreeSpace;

						////Logs.Write( "General Client", $"DownloadObject; {fileItem.Key}; {size}; Free space {freeBytes}" );

						//if( freeBytes * 1.05 < size ) 
						//{
						//	var b = new TextBlock();
						//	b.SetAttribute( "Error", "No free space on the disk." );
						//	AddAnswer( b.DumpToString( false ), answerPath );
						//	return;
						//}


						//download

						var lastProgressUpdate = DateTime.UtcNow;
						void Progress( int downloadedIncrement, long totalDownloaded, long totalSize )
						{
							lastProgressUpdate = DateTime.UtcNow;
						}

						using var cancellationToken2 = new CancellationTokenSource( new TimeSpan( 100, 0, 0 ) );

						var downloadResultTask = NetworkUtility.DownloadFileByUrlAsync( /*httpClient,*/ url, targetFullPath, Progress, cancellationToken );

						while( !downloadResultTask.IsCompleted )
						{
							if( ( DateTime.UtcNow - lastProgressUpdate ).TotalMinutes > 10 /*|| RemoveCancelledRequest( answerPath )*/ )
								cancellationToken2.Cancel();
							Thread.Sleep( 1 );
						}
						var downloadResult = downloadResultTask.Result;

						if( !string.IsNullOrEmpty( downloadResult.Error ) )
							return new StorageDownloadDirectoryResult() { Error = downloadResult.Error };


						//var lastProgressUpdate = DateTime.UtcNow;
						//void Progress( int progressPercentage, ref bool mustBreak )
						//{
						//	lastProgressUpdate = DateTime.UtcNow;
						//}

						//using var cancellationToken2 = new CancellationTokenSource( new TimeSpan( 100, 0, 0 ) );

						//var downloadResultTask = StorageUtility.DownloadFileAsync( StorageUtility.CreateS3Client( keys.ServiceURL, keys.PublicKey, keys.SecretKey ), true, keys.Bucket, fileItem.Key, filePath, callbackProgressPercentage: Progress, cancellationToken: cancellationToken2.Token );

						//while( !downloadResultTask.IsCompleted )
						//{
						//	if( ( DateTime.UtcNow - lastProgressUpdate ).TotalMinutes > 10 || RemoveCancelledRequest( answerPath ) )
						//		cancellationToken.Cancel();
						//	Thread.Sleep( 1 );
						//}
						//var downloadResult = downloadResultTask.Result;

						//if( !string.IsNullOrEmpty( downloadResult.Error ) )
						//{
						//	var b = new TextBlock();
						//	b.SetAttribute( "Error", downloadResult.Error );
						//	AddAnswer( b.DumpToString( false ), answerPath );
						//	return;
						//}


						////connection statistics
						//{
						//	var projectItem = ProjectManagement.GetProjectItem( projectID );
						//	projectItem?.AggregatedConnectionStatistics?.AddReceived( size );
						//}

					}
				}
				//}
			}

			if( deleteExcessEntries )
			{
				var needFiles = new ESet<string>();
				foreach( var fileItem in getDirectoryInfoResult.Items )
				{
					var filePath = GetFilePathByStorageFileName( fileItem.Name );
					needFiles.AddWithCheckAlreadyContained( filePath );
				}

				var directoryPath = GetFilePathByStorageFileName( storageDirectory );

				var directoryInfo = new DirectoryInfo( directoryPath );
				if( directoryInfo.Exists )
				{
					//delete files
					foreach( var fileInfo in directoryInfo.GetFiles( "*.*", searchOption ) )
					{
						if( !needFiles.Contains( fileInfo.FullName ) )
						{
							try
							{
								if( File.Exists( fileInfo.FullName ) )
									File.Delete( fileInfo.FullName );
							}
							catch { }
						}
					}

					//delete empty directories
					IOUtility.DeleteEmptyDirectories( directoryPath, searchOption, true );
				}
			}

			var result = new StorageDownloadDirectoryResult();
			result.WasAlreadyDownloaded = wasAlreadyDownloaded;
			result.Items = new StorageDownloadDirectoryResult.Item[ getDirectoryInfoResult.Items.Length ];
			for( int n = 0; n < result.Items.Length; n++ )
			{
				var item = getDirectoryInfoResult.Items[ n ];
				result.Items[ n ] = new StorageDownloadDirectoryResult.Item() { Name = item.Name, Size = item.Size, LastModified = item.LastModified, IsDirectory = item.IsDirectory };
			}
			return result;
		}

		///////////////////////////////////////////////
		//storage upload files

		public delegate void StorageUploadFilesProgressCallback( int uploadedIncrement, long totalUploaded, long totalSize );

		public static async Task<SimpleResult> StorageUploadFilesAsync( string[] storageFileNames, StorageUploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			var getContentUrlsResult = await StorageGetContentUrlsAsync( storageFileNames, true, cancellationToken );
			if( !string.IsNullOrEmpty( getContentUrlsResult.Error ) )
				return new SimpleResult() { Error = getContentUrlsResult.Error };

			var totalSizeToUpload = 0L;
			{
				for( int n = 0; n < storageFileNames.Length; n++ )
				{
					var storageFileName = storageFileNames[ n ];
					var sourceFullPath = GetFilePathByStorageFileName( storageFileName );
					if( !File.Exists( sourceFullPath ) )
						return new SimpleResult() { Error = "File to upload not found." }; //: " + storageFileName };
					var fileInfo = new FileInfo( sourceFullPath );
					totalSizeToUpload += fileInfo.Length;
				}
			}

			progressCallback?.Invoke( 0, 0, totalSizeToUpload );
			var totalUploadedSize = 0L;

			//using( var httpClient = new HttpClient() )
			{
				//httpClient.Timeout = TimeSpan.FromMinutes( 100 );

				for( int n = 0; n < storageFileNames.Length; n++ )
				{
					var storageFileName = storageFileNames[ n ];
					var url = getContentUrlsResult.Urls[ n ];
					var sourceFullPath = GetFilePathByStorageFileName( storageFileName );

					if( !File.Exists( sourceFullPath ) )
						return new SimpleResult() { Error = "File to upload not found." }; //: " + storageFileName };

					void Progress( int uploadedIncrement, long totalUploaded, long totalSize )
					{
						totalUploadedSize += uploadedIncrement;
						progressCallback?.Invoke( uploadedIncrement, totalUploadedSize, totalSizeToUpload );
					}

					var uploadResult = await NetworkUtility.UploadFileByUrlAsync( /*httpClient, */url, sourceFullPath, true, Progress, cancellationToken );
					if( !string.IsNullOrEmpty( uploadResult.Error ) )
						return new SimpleResult() { Error = uploadResult.Error };
				}
			}

			return new SimpleResult();
		}

		public static async Task<SimpleResult> StorageUploadFileAsync( string storageFileName, StorageUploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await StorageUploadFilesAsync( new[] { storageFileName }, progressCallback, cancellationToken );
		}

		public static string MoveFileToLocalStorageDirectory( string sourceFilePath, string storageFileName )
		{
			var newFilePath = GetFilePathByStorageFileName( storageFileName );
			if( sourceFilePath != newFilePath )
			{
#if UWP || NETSTANDARD2_1
				// .NET Standard 2.1 doesn't support File.Move(source, dest, overwrite). Emulate overwrite behavior.
				var destDir = Path.GetDirectoryName( newFilePath );
				if( !string.IsNullOrEmpty( destDir ) )
					Directory.CreateDirectory( destDir );
				if( File.Exists( newFilePath ) )
					File.Delete( newFilePath );
				File.Move( sourceFilePath, newFilePath );
#else
				File.Move( sourceFilePath, newFilePath, true );
#endif
			}
			return newFilePath;
		}

		public static async Task<SimpleResult> MoveFileToLocalStorageDirectoryAndUploadAsync( string sourceFilePath, string storageFileName, StorageUploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			var newFilePath = MoveFileToLocalStorageDirectory( sourceFilePath, storageFileName );
			return await StorageUploadFilesAsync( new string[] { storageFileName }, progressCallback, cancellationToken );
		}

		///////////////////////////////////////////////
		//storage create directory

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCreateDirectoriesAsync( string[] storageDirectoryNames, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );
			return await CloudServiceFunctions.StorageCreateDirectoriesAsync( storageDirectoryNames, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCreateDirectoryAsync( string storageDirectoryName, CancellationToken cancellationToken = default )
		{
			return await StorageCreateDirectoriesAsync( new string[] { storageDirectoryName }, cancellationToken );
		}

		///////////////////////////////////////////////
		//delete objects

		public static async Task<CloudServiceFunctions.SimpleResult> StorageDeleteObjectsAsync( CloudServiceFunctions.DeleteObjectsItem[] objects, bool deleteLocalFiles, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );

			var result = await CloudServiceFunctions.StorageDeleteObjectsAsync( objects, CommandLineParameters.ServerCheckCode, cancellationToken );

			if( string.IsNullOrEmpty( result.Error ) && deleteLocalFiles )
			{
				foreach( var obj in objects )
				{
					var filePath = GetFilePathByStorageFileName( obj.Name );

					try
					{
						if( obj.IsDirectory )
						{
							if( Directory.Exists( filePath ) )
								Directory.Delete( filePath, true );
						}
						else
						{
							if( File.Exists( filePath ) )
								File.Delete( filePath );
						}
					}
					catch( Exception e )
					{
						return new CloudServiceFunctions.SimpleResult { Error = e.Message };
					}
				}
			}

			return result;
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageDeleteDirectoryAsync( string storageDirectoryName, bool deleteLocalFiles, CancellationToken cancellationToken = default )
		{
			return await StorageDeleteObjectsAsync( new CloudServiceFunctions.DeleteObjectsItem[] { new CloudServiceFunctions.DeleteObjectsItem { Name = storageDirectoryName, IsDirectory = true } }, deleteLocalFiles, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageDeleteFilesAsync( string[] storageFileNames, bool deleteLocalFiles, CancellationToken cancellationToken = default )
		{
			var objects = new CloudServiceFunctions.DeleteObjectsItem[ storageFileNames.Length ];
			for( int n = 0; n < storageFileNames.Length; n++ )
				objects[ n ] = new CloudServiceFunctions.DeleteObjectsItem { Name = storageFileNames[ n ], IsDirectory = false };

			return await StorageDeleteObjectsAsync( objects, deleteLocalFiles, cancellationToken );

			//return await StorageDeleteObjectsAsync( await Task.Run( () => storageFileNames.Select( fn => new CloudServiceFunctions.DeleteObjectsItem { Name = fn, IsDirectory = false } ).ToArray() ), deleteLocalFiles, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageDeleteFileAsync( string storageFileName, bool deleteLocalFiles, CancellationToken cancellationToken = default )
		{
			return await StorageDeleteObjectsAsync( new CloudServiceFunctions.DeleteObjectsItem[] { new CloudServiceFunctions.DeleteObjectsItem { Name = storageFileName, IsDirectory = false } }, deleteLocalFiles, cancellationToken );
		}

		///////////////////////////////////////////////
		//copy objects

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCopyObjectsAsync( CloudServiceFunctions.CopyObjectsItem[] objects, bool move, CancellationToken cancellationToken = default )
		{
			if( string.IsNullOrEmpty( CommandLineParameters.ServerCheckCode ) )
				throw new Exception( "Server check code is not configured." );
			return await CloudServiceFunctions.StorageCopyObjectsAsync( objects, move, CommandLineParameters.ServerCheckCode, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCopyDirectoryAsync( string sourceDirectory, string targetDirectory, bool move, CancellationToken cancellationToken = default )
		{
			return await StorageCopyObjectsAsync( new CloudServiceFunctions.CopyObjectsItem[] { new CloudServiceFunctions.CopyObjectsItem { Name = sourceDirectory, TargetName = targetDirectory, IsDirectory = true } }, move, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCopyFilesAsync( string[] sourceFileNames, string[] targetFileNames, bool move, CancellationToken cancellationToken = default )
		{
			var objects = new CloudServiceFunctions.CopyObjectsItem[ sourceFileNames.Length ];
			for( int n = 0; n < sourceFileNames.Length; n++ )
				objects[ n ] = new CloudServiceFunctions.CopyObjectsItem { Name = sourceFileNames[ n ], TargetName = targetFileNames[ n ] };
			return await StorageCopyObjectsAsync( objects, move, cancellationToken );
		}

		public static async Task<CloudServiceFunctions.SimpleResult> StorageCopyFileAsync( string sourceFileName, string targetFileName, bool move, CancellationToken cancellationToken = default )
		{
			return await StorageCopyObjectsAsync( new CloudServiceFunctions.CopyObjectsItem[] { new CloudServiceFunctions.CopyObjectsItem { Name = sourceFileName, TargetName = targetFileName } }, move, cancellationToken );
		}

		///////////////////////////////////////////////
		//get main server info

		public class GetServerInfoResult
		{
			public int CPUUsage;
			public long MemoryInUse;
			public long MemoryCapacity;
			public int GPUUsage;
			public long GPUMemoryInUse;
			public long GPUMemoryCapacity;
			public long DiskInUse;
			public long DiskCapacity;
			public long TrafficOutbound;
			public long TrafficInbound;
			public long TrafficOutboundSpeed;
			public long TrafficInboundSpeed;
			public string Error;

			//

			public int MemoryUsage
			{
				get
				{
					if( MemoryCapacity == 0 )
						return 0;
					return (int)( MemoryInUse * 100 / MemoryCapacity );
				}
			}

			public int GPUMemoryUsage
			{
				get
				{
					if( GPUMemoryCapacity == 0 )
						return 0;
					return (int)( GPUMemoryInUse * 100 / GPUMemoryCapacity );
				}
			}

			public int DiskUsage
			{
				get
				{
					if( DiskCapacity == 0 )
						return 0;
					return (int)( DiskInUse * 100 / DiskCapacity );
				}
			}
		}

		public static async Task<GetServerInfoResult> GetServerInfoAsync( CancellationToken cancellationToken = default )
		{
			var now = DateTime.UtcNow;
			if( ( now - getServerInfoResultCachedTime ).TotalSeconds > 1 )
			{
				var input = new TextBlock();
				input.SetAttribute( "Command", "ServerInfo" );

				var requestResult = await RequestToServerManagerAsync( input.DumpToString( false ), cancellationToken );
				if( !string.IsNullOrEmpty( requestResult.Error ) )
					return new GetServerInfoResult() { Error = requestResult.Error };

				var block = requestResult.Answer;

				var result = new GetServerInfoResult();
				int.TryParse( block.GetAttribute( "CPUUsage", "0" ), out result.CPUUsage );
				long.TryParse( block.GetAttribute( "MemoryInUse", "0" ), out result.MemoryInUse );
				long.TryParse( block.GetAttribute( "MemoryCapacity", "0" ), out result.MemoryCapacity );
				int.TryParse( block.GetAttribute( "GPUUsage", "0" ), out result.GPUUsage );
				long.TryParse( block.GetAttribute( "GPUMemoryInUse", "0" ), out result.GPUMemoryInUse );
				long.TryParse( block.GetAttribute( "GPUMemoryCapacity", "0" ), out result.GPUMemoryCapacity );
				long.TryParse( block.GetAttribute( "DiskInUse" ), out result.DiskInUse );
				long.TryParse( block.GetAttribute( "DiskCapacity" ), out result.DiskCapacity );
				long.TryParse( block.GetAttribute( "TrafficOutbound", "0" ), out result.TrafficOutbound );
				long.TryParse( block.GetAttribute( "TrafficInbound", "0" ), out result.TrafficInbound );
				long.TryParse( block.GetAttribute( "TrafficOutboundSpeed", "0" ), out result.TrafficOutboundSpeed );
				long.TryParse( block.GetAttribute( "TrafficInboundSpeed", "0" ), out result.TrafficInboundSpeed );

				getServerInfoResultCached = result;
				getServerInfoResultCachedTime = now;
			}

			return getServerInfoResultCached;
		}

		/// <summary>
		/// Sets the user-specified server workload value from 0 to 100. Use it for manual management of balancing between servers for the Web Access feature.
		/// </summary>
		/// <param name="workload"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<SimpleResult> SetServerWorkloadAsync( int workload, CancellationToken cancellationToken = default )
		{
			if( currentServerWorkload != workload )
			{
				currentServerWorkload = workload;

				var input = new TextBlock();
				input.SetAttribute( "Command", "SetWorkload" );
				input.SetAttribute( "Workload", workload.ToString() );

				var requestResult = await RequestToServerManagerAsync( input.DumpToString( false ), cancellationToken );
				if( !string.IsNullOrEmpty( requestResult.Error ) )
					return new SimpleResult() { Error = requestResult.Error };
			}

			return new SimpleResult();
		}

		/// <summary>
		/// Resets the user-specified server workload value.
		/// </summary>
		/// <param name="workload"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<SimpleResult> ResetServerWorkloadAsync( CancellationToken cancellationToken = default )
		{
			return await SetServerWorkloadAsync( -1, cancellationToken );
		}
	}
}
#endif