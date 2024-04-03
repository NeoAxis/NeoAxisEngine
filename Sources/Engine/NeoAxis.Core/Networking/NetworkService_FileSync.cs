// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if CLOUD
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace NeoAxis.Networking
{
	public static class CommonNetworkService_FileSync
	{
		public const int FileBlockSize = 1 * 1024 * 1024;

		//public const int MaxFileSizeDefault = 150 * 1024 * 1024;
		//public static int MaxFileSize = MaxFileSizeDefault;
	}

	public class ServerNetworkService_FileSync : ServerNetworkService
	{
		public int SendFilesMaxSize { get; set; } = 5/*10*/ * 1024 * 1024;
		public bool ZipData = true;

		public delegate void GetFileCacheForConnectedNodeDelegate( ServerNetworkService_FileSync sender, NetworkNode.ConnectedNode connectedNode, ref FileCache fileCache );
		public event GetFileCacheForConnectedNodeDelegate GetFileCacheForConnectedNode;

		public static List<string> SkipPathsServer { get; } = new List<string>();

		///////////////////////////////////////////

		public class FileInfo
		{
			public string FullPath;
			public string FileName;
			public DateTime TimeModified;
			public long Length;
			public string Hash;
			public RepositorySyncMode SyncMode;
		}

		///////////////////////////////////////////

		public class FileCache
		{
			public string ProjectDataFolder;
			public Dictionary<string, FileInfo> Files;
			//public string TotalHash = "";

			long? totalSizeCached;
			long? totalSizeToPlayCached;

			public FileCache( string projectDataFolder, Dictionary<string, FileInfo> files )//, string totalHash )
			{
				this.ProjectDataFolder = projectDataFolder;
				Files = files;
				//TotalHash = totalHash;

				//FileInfo q;
				//q.Length
			}

			public long TotalSize
			{
				get
				{
					if( !totalSizeCached.HasValue )
					{
						var result = 0L;
						foreach( var info in Files.Values )
							result += info.Length;
						totalSizeCached = result;
					}
					return totalSizeCached.Value;
				}
			}

			public long TotalSizeToPlay
			{
				get
				{
					if( !totalSizeToPlayCached.HasValue )
					{
						var projectSettingsCache = new CloudProjectProjectSettingsCache( ProjectDataFolder );

						var result = 0L;
						foreach( var fileInfo in Files.Values )
						{
							if( fileInfo.SyncMode != RepositorySyncMode.Synchronize )
								continue;

							string extension = "";
							try
							{
								extension = Path.GetExtension( fileInfo.FileName ).ToLower().Replace( ".", "" );
							}
							catch { }

							string newFileName = null;

							//rename exe files to prevent manual execution
							if( extension == "exe" )
								newFileName = Path.ChangeExtension( fileInfo.FileName, ".exe_secure" );

							//can't send exe, cmd and other files when play
							if( GetProhibitedExtensionsWhenPlay().Contains( extension ) )
								continue;

							//skip files depending project settings
							if( projectSettingsCache.SkipFilesWithExtensionWhenPlay.Contains( extension ) )
								continue;

							//clear files depending project settings
							if( projectSettingsCache.ClearFilesWithExtensionWhenPlay.Contains( extension ) )
								continue;

							result += fileInfo.Length;
						}
						totalSizeToPlayCached = result;
					}

					return totalSizeToPlayCached.Value;
				}
			}
		}

		///////////////////////////////////////////

		static ServerNetworkService_FileSync()
		{
			//server only private files
			SkipPathsServer.Add( "RepositoryServer.config" );
			SkipPathsServer.Add( "MessageToServerManager.txt" );
		}

		public ServerNetworkService_FileSync()
			: base( "FileSync", 5 )
		{
			//register message types
			RegisterMessageType( "RequestInfoToServer", 1, ReceiveMessage_RequestInfoToServer );
			RegisterMessageType( "SendInfoToClient", 2 );
			RegisterMessageType( "RequestFilesToServer", 3, ReceiveMessage_RequestFilesToServer );
			RegisterMessageType( "SendFilesToClient", 4 );
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		public static bool SkipFile( ESet<string> skipPaths, ESet<string> skipFolderNames, string fileName )
		{
			//!!!!slowly?

			//skip files depending path
			foreach( var skipPath in skipPaths )
			{
				if( fileName.Length >= skipPath.Length && string.Compare( fileName.Substring( 0, skipPath.Length ), skipPath, true ) == 0 )
					return true;
			}

			//skip files depending folder name
			try
			{
				var path = Path.GetDirectoryName( fileName );
				foreach( var folderName in path.Split( '\\', StringSplitOptions.RemoveEmptyEntries ) )
				{
					if( skipFolderNames.Contains( folderName ) )
						return true;
				}
			}
			catch { }

			return false;
		}


		public static ESet<string> GetProhibitedExtensionsWhenPlay()
		{
			if( prohibitedExtensionsWhenPlay == null )
			{
				var set = new ESet<string>();

				//set.Add( "exe" );//it is renamed to exe_secure
				set.Add( "com" );
				set.Add( "cmd" );
				set.Add( "bat" );
				set.Add( "job" );
				set.Add( "msi" );
				set.Add( "reg" );
				set.Add( "vb" );
				set.Add( "vbe" );
				set.Add( "vbscript" );
				set.Add( "ws" );
				set.Add( "wsf" );
				set.Add( "wsh" );
				set.Add( "scr" );
				set.Add( "sct" );
				set.Add( "ps1" );
				set.Add( "msp" );
				set.Add( "mst" );
				set.Add( "cpl" );

				prohibitedExtensionsWhenPlay = set;
			}

			return prohibitedExtensionsWhenPlay;
		}
		static ESet<string> prohibitedExtensionsWhenPlay;


		////clear source Import3D files
		//static ESet<string> GetClearFileExtensionsWhenPlay()
		//{
		//	if( clearFileExtensionsWhenPlay == null )
		//	{
		//		var set = new ESet<string>();

		//		var array = ResourceManager.Import3DFileExtensions;
		//		set.AddRange( array.Select( ext => "." + ext ) );

		//		clearFileExtensionsWhenPlay = set;
		//	}

		//	return clearFileExtensionsWhenPlay;
		//}
		//static ESet<string> clearFileExtensionsWhenPlay;


		//get hash of empty file
		static string GetMD5( MD5 md5, Stream stream )
		{
			var hashBytes = md5.ComputeHash( stream );

			var builder = new StringBuilder( ( hashBytes.Length + 1 ) * 2 );
			for( int i = 0; i < hashBytes.Length; i++ )
				builder.Append( hashBytes[ i ].ToString( "X2" ) );
			return builder.ToString();
		}

		static string GetHashOfEmptyFile()
		{
			if( hashOfEmptyFile == null )
			{
				using( var md5 = MD5.Create() )
				{
					using( var stream = new MemoryStream( new byte[ 0 ] ) )
						hashOfEmptyFile = GetMD5( md5, stream );
				}
			}
			return hashOfEmptyFile;
		}
		static string hashOfEmptyFile;


		public static bool CanSendFileToClient( CloudProjectProjectSettingsCache projectSettingsCache, bool edit, FileInfo fileInfo, out string newFileName, out bool clearFile )
		{
			newFileName = null;
			clearFile = false;

			if( !edit )// !clientData.Edit )
			{
				//skip server only files
				if( fileInfo.SyncMode != RepositorySyncMode.Synchronize )
					return false;

				string extension = "";
				try
				{
					extension = Path.GetExtension( fileInfo.FileName ).ToLower().Replace( ".", "" );
				}
				catch { }

				//rename exe files to prevent manual execution
				if( extension == "exe" )
					newFileName = Path.ChangeExtension( fileInfo.FileName, ".exe_secure" );

				//can't send exe, cmd and other files when play
				if( GetProhibitedExtensionsWhenPlay().Contains( extension ) )
					return false;


				//var projectSettingsCache = clientData.GetCloudProjectProjectSettingsCache();

				//skip files depending project settings
				if( projectSettingsCache.SkipFilesWithExtensionWhenPlay.Contains( extension ) )
					return false;

				//clear files depending project settings
				if( projectSettingsCache.ClearFilesWithExtensionWhenPlay.Contains( extension ) )
					clearFile = true;


				////!!!!slowly?

				////skip files depending folder name
				//try
				//{
				//	var path = Path.GetDirectoryName( fileInfo.FileName );
				//	foreach( var folderName in path.Split( '\\', StringSplitOptions.RemoveEmptyEntries ) )
				//	{
				//		if( projectSettingsCache.SkipFoldersWithName.Contains( folderName ) )
				//			return false;
				//	}
				//}
				//catch { }


				////maybe make settings for Import3D and blend files

				////clear source Import3D files
				//if( GetClearFileExtensionsWhenPlay().Contains( extension ) )
				//	clearFile = true;

				////skip .bin if .gltf exists with the same name
				//if( extension == ".bin" )
				//{
				//	if( File.Exists( Path.ChangeExtension( fileInfo.FullPath, ".gltf" ) ) )
				//		return false;
				//}

				////skip blend, blend1 files
				//if( extension == ".blend" || extension == ".blend1" )
				//	return false;

			}

			return true;
		}

		static bool CanSendFileToClient( CloudProjectClientData clientData, FileInfo fileInfo, out string newFileName, out bool clearFile )
		{
			var projectSettingsCache = clientData.GetCloudProjectProjectSettingsCache();
			return CanSendFileToClient( projectSettingsCache, clientData.Edit, fileInfo, out newFileName, out clearFile );
		}

		bool ReceiveMessage_RequestInfoToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var clientData = (CloudProjectClientData)sender.Tag;
			if( !clientData.Verified )
				return false;

			if( !reader.Complete() )
				return false;

			//!!!!в потоке, потом отправлять сообщение
			FileCache cache = null;
			GetFileCacheForConnectedNode?.Invoke( this, sender, ref cache );

			//send the file info to the client
			var writer = BeginMessage( sender, GetMessageType( "SendInfoToClient" ) );
			if( cache != null )
			{
				writer.WriteVariableInt32( CommonNetworkService_FileSync.FileBlockSize );

				var count = 0;
				foreach( var info in cache.Files.Values )
				{
					if( CanSendFileToClient( clientData, info, out _, out _ ) )
						count++;
				}
				writer.Write( count );

				foreach( var info in cache.Files.Values )
				{
					if( CanSendFileToClient( clientData, info, out var newFileName, out var clearFile ) )
					{
						writer.Write( newFileName ?? info.FileName );
						writer.Write( info.TimeModified );
						writer.Write( clearFile ? 0 : info.Length );
						writer.Write( clearFile ? GetHashOfEmptyFile() : info.Hash );
						writer.Write( (byte)info.SyncMode );
					}
				}

				//writer.Write( cache.TotalHash );
			}
			else
			{
				writer.Write( 0 );
				//writer.Write( cache.TotalHash );
			}
			EndMessage();

			return true;
		}

		bool ReceiveMessage_RequestFilesToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//!!!!везде такое во всех сервисах
			var clientData = (CloudProjectClientData)sender.Tag;
			if( !clientData.Verified )
				return false;

			//file names are checked by the cache

			var fileNamesToGet = new List<(string, int)>();
			var requiredCount = reader.ReadInt32();
			for( int n = 0; n < requiredCount; n++ )
			{
				var fileName = PathUtility.NormalizePath( reader.ReadString() );
				var requestedBlock = reader.ReadVariableInt32();

				if( Path.IsPathRooted( fileName ) || fileName.Contains( ".." ) )
					return false;

				//restore original .exe from .exe_secure
				if( !clientData.Edit )
				{
					string extension = "";
					try
					{
						extension = Path.GetExtension( fileName ).ToLower();
					}
					catch { }

					if( extension == ".exe_secure" )
						fileName = Path.ChangeExtension( fileName, ".exe" );
				}

				fileNamesToGet.Add( (fileName, requestedBlock) );
			}
			if( !reader.Complete() )
			{
				//!!!!как обрабатывать ошибки от юзера. везде так
				return false;
			}


			//send files to the client

			//!!!!в потоке, потом отправлять сообщение
			FileCache cache = null;
			GetFileCacheForConnectedNode?.Invoke( this, sender, ref cache );


			var writer = BeginMessage( sender, GetMessageType( "SendFilesToClient" ) );
			var totalSent = 0;

			foreach( var fileNameItem in fileNamesToGet )
			{
				var fileName = fileNameItem.Item1;
				var requestedBlock = fileNameItem.Item2;

				if( cache != null && cache.Files.TryGetValue( fileName, out var fileItem ) && CanSendFileToClient( clientData, fileItem, out var newFileName, out var clearFile ) )
				{
					var fullPath = fileItem.FullPath;

					try
					{
						byte[] data;
						if( clearFile )
							data = Array.Empty<byte>();
						else
						{
							using( FileStream fs = new FileStream( fullPath, FileMode.Open ) )
							{
								var offset = requestedBlock * CommonNetworkService_FileSync.FileBlockSize;
								fs.Seek( offset, SeekOrigin.Begin );

								var size = (int)Math.Min( fileItem.Length - offset, CommonNetworkService_FileSync.FileBlockSize );

								data = new byte[ size ];
								if( fs.Read( data, 0, size ) != size )
									throw new Exception( "File reading failed." );
							}

							//data = File.readReadAllBytes( fullPath );

							////check max file size limit
							//if( CommonNetworkService_FileSync.MaxFileSize != int.MaxValue )
							//{
							//	var fileInfo = new System.IO.FileInfo( fullPath );
							//	if( fileInfo.Length > CommonNetworkService_FileSync.MaxFileSize )
							//	{
							//		//!!!!check additionalErrorMessage
							//		additionalErrorMessage = $"File size is more than limit. Limit is {CommonNetworkService_FileSync.MaxFileSize}.";
							//		return false;
							//	}
							//}

							//data = File.ReadAllBytes( fullPath );
						}

						writer.Write( true );
						writer.Write( newFileName ?? fileName );
						writer.Write( data.Length );
						writer.Write( fileItem.TimeModified );//!!!!каждый блок необязательно
						writer.WriteVariableInt32( requestedBlock );

						var zipData = ZipData && data.Length > 200;
						writer.Write( zipData );
						if( zipData )
						{
							var zippedData = IOUtility.Zip( data, System.IO.Compression.CompressionLevel.Fastest );
							writer.Write( zippedData.Length );
							writer.Write( zippedData );
						}
						else
							writer.Write( data );

						totalSent += data.Length;
						if( totalSent > SendFilesMaxSize )
							break;
					}
					catch//( Exception e )
					{
						//no file or busy
						writer.Write( true );
						writer.Write( newFileName ?? fileName );
						writer.Write( -1 );
					}
				}
				else
				{
					//no cache, not in the cache or can't send
					writer.Write( true );
					writer.Write( fileName );
					writer.Write( -1 );
				}
			}

			writer.Write( false );
			EndMessage();

			return true;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_FileSync : ClientNetworkService
	{
		public static List<string> SkipPathsClient { get; } = new List<string>();

		public int SendFilesMaxSize { get; set; } = 5/*10*/ * 1024 * 1024;

		public string DestinationFolder { get; set; } = "";
		//public string LocalFilesFolder { get; set; } = "";

		public int ServerFileBlockSize { get; set; }

		volatile StatusClass status = new StatusClass( StatusEnum.NotInitialized, "" );
		bool remainsFilesToDownloadPrepared;
		//key: FileName.ToLower()
		EDictionary<string, FileInfo> remainsFilesToDownload = new EDictionary<string, FileInfo>();
		bool filesDuringRequest;
		public long RemainsToUpdate;

		//cached project settings
		CloudProjectProjectSettingsCache cloudProjectProjectSettingsCache;

		///////////////////////////////////////////

		public delegate void StatusChangedDelegate( ClientNetworkService_FileSync sender );
		public event StatusChangedDelegate StatusChanged;

		public delegate void BeforeUpdateFilesDelegate( ClientNetworkService_FileSync sender, Dictionary<string, FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel );
		public event BeforeUpdateFilesDelegate BeforeUpdateFiles;

		///////////////////////////////////////////

		public enum StatusEnum
		{
			NotInitialized,
			Updating,
			Success,
			Error,
		}

		///////////////////////////////////////////

		public class StatusClass
		{
			public StatusEnum Status { get; }
			public string Error { get; } = "";

			public StatusClass( StatusEnum status, string error )
			{
				Status = status;
				Error = error;
			}
		}

		///////////////////////////////////////////

		public class FileInfo
		{
			public string FileName;
			public long Length;
			public DateTime TimeModified;
			public string Hash;
			public RepositorySyncMode SyncMode;

			public ESet<int> DownloadedBlocks = new ESet<int>();

			//

			public int BlockCount
			{
				get { return (int)( ( Length + CommonNetworkService_FileSync.FileBlockSize - 1 ) / CommonNetworkService_FileSync.FileBlockSize ); }
			}

			public long RemainingLength
			{
				get { return Math.Min( Length, ( BlockCount - DownloadedBlocks.Count ) * CommonNetworkService_FileSync.FileBlockSize ); }
			}
		}

		///////////////////////////////////////////

		static ClientNetworkService_FileSync()
		{
			//!!!!что еще

			//SkipPathsClient.Add( "Caches" );
			//SkipPathsClient.Add( "User settings" );

			SkipPathsClient.Add( "CloudProject.info" );
			SkipPathsClient.Add( "RepositoryServerState.config" );
			SkipPathsClient.Add( "RepositoryLocal.config" );
		}

		public ClientNetworkService_FileSync()
			: base( "FileSync", 5 )
		{
			//register message types
			RegisterMessageType( "RequestInfoToServer", 1 );
			RegisterMessageType( "SendInfoToClient", 2, ReceiveMessage_SendInfoToClient );
			RegisterMessageType( "RequestFilesToServer", 3 );
			RegisterMessageType( "SendFilesToClient", 4, ReceiveMessage_SendFilesToClient );
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		public StatusClass Status
		{
			get { return status; }
		}

		void SetStatus( StatusClass status )
		{
			this.status = status;
			StatusChanged?.Invoke( this );
		}

		static string GetMD5( MD5 md5, Stream stream )
		{
			var hashBytes = md5.ComputeHash( stream );

			var builder = new StringBuilder( ( hashBytes.Length + 1 ) * 2 );
			for( int i = 0; i < hashBytes.Length; i++ )
				builder.Append( hashBytes[ i ].ToString( "X2" ) );
			return builder.ToString();
		}

		static string GetMD5( MD5 md5, byte[] input )
		{
			var hashBytes = md5.ComputeHash( input );

			var builder = new StringBuilder( ( hashBytes.Length + 1 ) * 2 );
			for( int i = 0; i < hashBytes.Length; i++ )
				builder.Append( hashBytes[ i ].ToString( "X2" ) );
			return builder.ToString();
		}

		public static bool SkipFile( ESet<string> skipPaths, ESet<string> skipFolderNames, string fileName )
		{
			//!!!!slowly?

			//skip files depending path
			foreach( var skipPath in skipPaths )
			{
				if( fileName.Length >= skipPath.Length && string.Compare( fileName.Substring( 0, skipPath.Length ), skipPath, true ) == 0 )
					return true;
			}

			//skip files depending folder name
			try
			{
				var path = Path.GetDirectoryName( fileName );
				foreach( var folderName in path.Split( '\\', StringSplitOptions.RemoveEmptyEntries ) )
				{
					if( skipFolderNames.Contains( folderName ) )
						return true;
				}
			}
			catch { }

			return false;
		}

		////call inside try/catch
		//List<FileInfo> GetLocalFiles()
		//{
		//	//!!!!кешировать? если много раз вызывается, то надо

		//	var result = new List<FileInfo>();

		//	if( !string.IsNullOrEmpty( LocalFilesFolder ) )
		//	{
		//		using( var md5 = System.Security.Cryptography.MD5.Create() )
		//		{
		//			var files = Directory.GetFiles( LocalFilesFolder, "*", SearchOption.AllDirectories );
		//			foreach( var fullPath in files )
		//			{
		//				var fileName = fullPath.Substring( LocalFilesFolder.Length + 1 );

		//				var fileInfo = new System.IO.FileInfo( fullPath );

		//				//!!!!поддержка long
		//				var bytes = File.ReadAllBytes( fullPath );
		//				var hash = GetMD5( md5, bytes );

		//				var info = new FileInfo();
		//				info.FileName = fileName;
		//				info.TimeModified = fileInfo.LastWriteTimeUtc;
		//				info.Length = fileInfo.Length;
		//				info.Hash = hash;

		//				result.Add( info );
		//			}
		//		}
		//	}

		//	return result;
		//}

		//call inside try/catch
		public static bool IsNeedUpdate( MD5 md5, long fileInfoLength, string fileInfoHash, string fullPath )
		{
			var update = false;

			if( File.Exists( fullPath ) )
			{
				var fileInfo = new System.IO.FileInfo( fullPath );

				if( fileInfoLength != fileInfo.Length )
					update = true;
				else
				{
					//!!!!slowly. use tasks. MD5.Create() for each thread

					string hash;
					if( fileInfo.Length > 10000000 )
					{
						using( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read ) )
							hash = GetMD5( md5, stream );
					}
					else
					{
						var bytes = File.ReadAllBytes( fullPath );
						hash = GetMD5( md5, bytes );
					}

					if( fileInfoHash != hash )
						update = true;
				}
			}
			else
				update = true;

			return update;
		}

		//call inside try/catch
		public static bool IsNeedUpdate( MD5 md5, FileInfo info, string fullPath )
		{
			return IsNeedUpdate( md5, info.Length, info.Hash, fullPath );
		}

		//call inside try/catch
		void GetFilesToDownloadAndDelete( Dictionary<string, FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete )
		{
			using( var md5 = MD5.Create() )
			{
				//var localFiles = GetLocalFiles();

				var allSkipPaths = new ESet<string>();
				var allSkipFolderNames = new ESet<string>();
				{
					allSkipPaths.AddRangeWithCheckAlreadyContained( SkipPathsClient );

					//apply settings from project settings
					if( cloudProjectProjectSettingsCache == null )
						cloudProjectProjectSettingsCache = new CloudProjectProjectSettingsCache( DestinationFolder );
					allSkipPaths.AddRangeWithCheckAlreadyContained( cloudProjectProjectSettingsCache.SkipPaths );
					allSkipFolderNames.AddRangeWithCheckAlreadyContained( cloudProjectProjectSettingsCache.SkipFoldersWithName );
				}

				//если на сервере и в локальном сторедже нет, а он есть, то удалять
				var filesToDelete2 = new List<string>();
				{
					var fileNamesOnServerOrLocalLower = new ESet<string>();
					foreach( var info in filesOnServer.Values )
						fileNamesOnServerOrLocalLower.AddWithCheckAlreadyContained( info.FileName.ToLower() );
					//foreach( var info in localFiles )
					//	fileNamesOnServerOrLocalLower.AddWithCheckAlreadyContained( info.FileName.ToLower() );

					foreach( var fullPath in Directory.GetFiles( DestinationFolder, "*", SearchOption.AllDirectories ) )
					{
						var fileName = fullPath.Substring( DestinationFolder.Length + 1 );

						if( !SkipFile( allSkipPaths, allSkipFolderNames, fileName ) )
						{
							if( !fileNamesOnServerOrLocalLower.Contains( fileName.ToLower() ) )
							{
								filesToDelete2.Add( fileName );

								//Log.Info( "File delete: " + fileName );
							}
						}
					}
				}


				//нужно в диалоге учитывать
				////copy files from local storage
				//{

				//	//!!!!дважды пробегать не нужно. где еще так


				//	var fileNamesOnServerLower = new ESet<string>();
				//	foreach( var info in filesOnServer )
				//		fileNamesOnServerLower.AddWithCheckAlreadyContained( info.FileName.ToLower() );

				//	foreach( var info in localFiles )
				//	{
				//		//if file exists on the server and in the local storage, then the server's file is used
				//		if( !fileNamesOnServerLower.Contains( info.FileName.ToLower() ) )
				//		{
				//			var fullPath = Path.Combine( DestinationFolder, info.FileName );

				//			if( IsNeedUpdate( md5, info, fullPath ) )
				//			{
				//				var directory = Path.GetDirectoryName( fullPath );
				//				Directory.CreateDirectory( directory );

				//				var sourceFullPath = Path.Combine( LocalFilesFolder, info.FileName );
				//				File.Copy( sourceFullPath, fullPath, true );

				//				////!!!!
				//				//Log.Info( "File local copy: " + info.FileName );
				//			}
				//		}
				//	}
				//}

				var filesToDownload2 = new List<string>();

				//если на сервере есть, а тут нет, или различаются, то скачивать
				foreach( var info in filesOnServer.Values )
				{
					var fullPath = Path.Combine( DestinationFolder, info.FileName );

					if( IsNeedUpdate( md5, info, fullPath ) )
						filesToDownload2.Add( info.FileName );
				}

				filesToDownload = filesToDownload2.ToArray();
				filesToDelete = filesToDelete2.ToArray();
			}
		}

		bool ReceiveMessage_SendInfoToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			ServerFileBlockSize = reader.ReadVariableInt32();

			var fileCountOnServer = reader.ReadInt32();
			var filesOnServer = new Dictionary<string, FileInfo>( fileCountOnServer );
			for( int n = 0; n < fileCountOnServer; n++ )
			{
				var info = new FileInfo();
				//!!!!check for "..". where else
				info.FileName = PathUtility.NormalizePath( reader.ReadString() );
				info.TimeModified = reader.ReadDateTime();
				info.Length = reader.ReadInt64();
				info.Hash = reader.ReadString();
				info.SyncMode = (RepositorySyncMode)reader.ReadByte();
				filesOnServer[ info.FileName.ToLower() ] = info;
			}

			//может дополнительно получать хеш из всего списка (сортированного). где еще
			//var totalHashOnServer = reader.ReadString();

			if( !reader.Complete() )
			{
				SetStatus( new StatusClass( StatusEnum.Error, "Error getting info about files." ) );
				return false;
			}


			try
			{
				string[] filesToDownload = null;
				string[] filesToDelete = null;

				//!!!!bool handled?

				//Editor
				var cancel = false;
				BeforeUpdateFiles?.Invoke( this, filesOnServer, ref filesToDownload, ref filesToDelete, ref cancel );
				if( cancel )
				{
					SetStatus( new StatusClass( StatusEnum.Error, "Cancelled by the user." ) );
					return true;
				}

				//Player
				if( EngineApp.IsSimulation && filesToDownload == null && filesToDelete == null )
					GetFilesToDownloadAndDelete( filesOnServer, ref filesToDownload, ref filesToDelete );


				//delete filesToDelete
				foreach( var fileName in filesToDelete )
				{
					var fullPath = Path.Combine( DestinationFolder, fileName );
					File.Delete( fullPath );
				}

				//start downloading
				remainsFilesToDownloadPrepared = true;
				foreach( var fileName in filesToDownload )
				{
					if( filesOnServer.TryGetValue( fileName.ToLower(), out var fileInfo ) )
					{
						//create empty files immediately without request
						if( fileInfo.BlockCount == 0 )
						{
							var fullPath = Path.Combine( DestinationFolder, fileInfo.FileName );
							File.WriteAllBytes( fullPath, new byte[ 0 ] );
							File.SetLastWriteTimeUtc( fullPath, fileInfo.TimeModified );
						}
						else
							remainsFilesToDownload[ fileInfo.FileName.ToLower() ] = fileInfo;
					}
				}

				UpdateRemainsToUpdate();


				//if( remainsFilesToDownload.Count == 0 )
				//	SetStatus( new StatusClass( StatusEnum.Success, "" ) );


				////request to download files
				//if( filesToDownload.Count != 0 )
				//{
				//	//!!!!можно не все запрашивать, только сколько влезет
				//	var writer = BeginMessage( GetMessageType( "RequestFilesToServer" ) );
				//	writer.Write( filesToDownload.Count );
				//	foreach( var fileName in filesToDownload )
				//		writer.Write( fileName );
				//	EndMessage();

				//	RemainsToUpdate = filesToDownloadSize;
				//}
				//else
				//{
				//	zzzzz;
				//	//!!!!проверять еще раз? хеш всех хешей может


				//	SetStatus( new StatusClass( StatusEnum.Success, "" ) );
				//}

			}
			catch( Exception e )
			{
				SetStatus( new StatusClass( StatusEnum.Error, "ReceiveMessage SendInfoToClient error. " + e.Message ) );
				return false;
			}

			return true;
		}

		class FileItem
		{
			public string FileName;
			public int Length;
			public DateTime TimeModified;
			public byte[] Data;
			public int RequestedBlock;
		}

		bool ReceiveMessage_SendFilesToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var fileItems = new List<FileItem>();
			while( reader.ReadBoolean() )
			{
				var fileItem = new FileItem();
				//!!!!check for "..". where else
				fileItem.FileName = PathUtility.NormalizePath( reader.ReadString() );
				fileItem.Length = reader.ReadInt32();

				if( fileItem.Length >= 0 )
				{
					fileItem.TimeModified = reader.ReadDateTime();
					fileItem.RequestedBlock = reader.ReadVariableInt32();

					var zipped = reader.ReadBoolean();
					if( zipped )
					{
						var zippedSize = reader.ReadInt32();
						var zippedData = new byte[ zippedSize ];
						reader.ReadBuffer( zippedData, 0, zippedData.Length );

						try
						{
							fileItem.Data = IOUtility.Unzip( zippedData );
							if( fileItem.Data.Length != fileItem.Length )
							{
								SetStatus( new StatusClass( StatusEnum.Error, "Invalid file size after unzip." ) );
								return false;
							}
						}
						catch( Exception e )
						{
							SetStatus( new StatusClass( StatusEnum.Error, e.Message ) );
							return false;
						}
					}
					else
					{
						fileItem.Data = new byte[ fileItem.Length ];
						reader.ReadBuffer( fileItem.Data, 0, fileItem.Length );
					}
				}

				fileItems.Add( fileItem );
			}

			////!!!!хеш получать из сортированного списка. везде так
			//var totalHash = reader.ReadString();

			if( !reader.Complete() )
			{
				SetStatus( new StatusClass( StatusEnum.Error, "Error getting files." ) );
				return false;
			}


			try
			{
				foreach( var fileItem in fileItems )
				{
					var fullPath = Path.Combine( DestinationFolder, fileItem.FileName );

					if( fileItem.Length >= 0 )
					{
						var directory = Path.GetDirectoryName( fullPath );
						if( !Directory.Exists( directory ) )
							Directory.CreateDirectory( directory );

						if( fileItem.RequestedBlock == 0 )
						{
							File.WriteAllBytes( fullPath, fileItem.Data );
						}
						else
						{
							using( var stream = new FileStream( fullPath, FileMode.Append ) )
								stream.Write( fileItem.Data, 0, fileItem.Data.Length );
						}
						File.SetLastWriteTimeUtc( fullPath, fileItem.TimeModified );

						if( remainsFilesToDownload.TryGetValue( fileItem.FileName.ToLower(), out var fileInfo ) )
						{
							fileInfo.DownloadedBlocks.AddWithCheckAlreadyContained( fileItem.RequestedBlock );

							if( fileInfo.DownloadedBlocks.Count == fileInfo.BlockCount )
								remainsFilesToDownload.Remove( fileItem.FileName.ToLower() );
						}
						else
							remainsFilesToDownload.Remove( fileItem.FileName.ToLower() );

						//var directory = Path.GetDirectoryName( fullPath );
						//Directory.CreateDirectory( directory );

						//File.WriteAllBytes( fullPath, fileItem.Data );

						////!!!!check
						//File.SetLastWriteTimeUtc( fullPath, fileItem.TimeModified );
					}
					else
					{
						if( File.Exists( fullPath ) )
							File.Delete( fullPath );
						remainsFilesToDownload.Remove( fileItem.FileName.ToLower() );
					}
				}
			}
			catch( Exception e )
			{
				SetStatus( new StatusClass( StatusEnum.Error, "ReceiveMessage SendFilesToClient error. " + e.Message ) );
				return false;
			}


			UpdateRemainsToUpdate();

			filesDuringRequest = false;


			//!!!!проверять еще раз? хеш всех хешей может
			//!!!!лочить файлы, чтобы нельзя было менять вручную в процессе работы?

			return true;
		}

		protected internal override void OnUpdate()
		{
			base.OnUpdate();

			if( remainsFilesToDownloadPrepared && !filesDuringRequest )
			{
				if( remainsFilesToDownload.Count != 0 )
				{
					var files = new List<FileInfo>();
					var totalSize = 0L;
					foreach( var fileInfo in remainsFilesToDownload.Values )
					{
						files.Add( fileInfo );

						//может влазить несколько блоков одного файла. сейчас получается несколько файлов одновременно будет качать

						totalSize += CommonNetworkService_FileSync.FileBlockSize;//fileInfo.Length;
						if( totalSize > SendFilesMaxSize )
							break;
					}

					var writer = BeginMessage( GetMessageType( "RequestFilesToServer" ) );
					writer.Write( files.Count );
					foreach( var fileInfo in files )
					{
						writer.Write( fileInfo.FileName );
						writer.WriteVariableInt32( fileInfo.DownloadedBlocks.Count );
					}
					EndMessage();

					filesDuringRequest = true;
				}
				else
					SetStatus( new StatusClass( StatusEnum.Success, "" ) );
			}
		}

		void UpdateRemainsToUpdate()
		{
			var value = 0L;
			foreach( var fileInfo in remainsFilesToDownload.Values )
				value += fileInfo.RemainingLength;
			RemainsToUpdate = value;
		}

		public void StartUpdate()
		{
			SetStatus( new StatusClass( StatusEnum.Updating, "" ) );

			BeginMessage( GetMessageType( "RequestInfoToServer" ) );
			EndMessage();
		}
	}
}
//#endif