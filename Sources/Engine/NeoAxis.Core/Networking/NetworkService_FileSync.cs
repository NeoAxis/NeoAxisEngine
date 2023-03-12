// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if CLOUD
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis.Networking;

namespace NeoAxis
{
	public class ServerNetworkService_FileSync : ServerNetworkService
	{
		public int SendFilesMaxSize { get; set; } = 10 * 1024 * 1024;

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
			public Dictionary<string, FileInfo> Files;
			//public string TotalHash = "";

			long totalSizeCached;

			public FileCache( Dictionary<string, FileInfo> files )//, string totalHash )
			{
				Files = files;
				//TotalHash = totalHash;

				//FileInfo q;
				//q.Length
			}

			public long TotalSize
			{
				get
				{
					if( totalSizeCached == 0 )
					{
						var result = 0L;
						foreach( var info in Files.Values )
							result += info.Length;
						totalSizeCached = result;
					}
					return totalSizeCached;
				}
			}
		}

		///////////////////////////////////////////

		static ServerNetworkService_FileSync()
		{
			//server only private files
			SkipPathsServer.Add( "RepositoryServer.config" );
			SkipPathsServer.Add( "MessageToServerManager.txt" );

			//SkipPathsDefault.Add( @"Caches\Files" );
			//SkipPathsDefault.Add( @"Caches\ShaderCache" );
			////SkipPathsDefault.Add( "Caches" );
			//SkipPathsDefault.Add( "User settings" );
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

		public static bool SkipFile( ESet<string> skipPaths, string fileName )
		{
			//!!!!slowly?

			foreach( var skipPath in skipPaths )
			{
				if( fileName.Length >= skipPath.Length && string.Compare( fileName.Substring( 0, skipPath.Length ), skipPath, true ) == 0 )
					return true;
			}
			return false;
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
				var count = 0;
				foreach( var info in cache.Files.Values )
				{
					if( clientData.Edit || info.SyncMode == RepositorySyncMode.Synchronize )
						count++;
				}
				writer.Write( count );

				foreach( var info in cache.Files.Values )
				{
					if( clientData.Edit || info.SyncMode == RepositorySyncMode.Synchronize )
					{
						writer.Write( info.FileName );
						writer.Write( info.TimeModified );
						writer.Write( info.Length );
						writer.Write( info.Hash );
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

			var fileNamesToGet = new List<string>();
			var requiredCount = reader.ReadInt32();
			for( int n = 0; n < requiredCount; n++ )
			{
				var fileName = reader.ReadString();

				if( Path.IsPathRooted( fileName ) || fileName.Contains( ".." ) )
					return false;

				fileNamesToGet.Add( fileName );
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

			foreach( var fileName in fileNamesToGet )
			{
				if( cache != null && cache.Files.TryGetValue( fileName, out var fileItem ) && ( clientData.Edit || fileItem.SyncMode == RepositorySyncMode.Synchronize ) )
				{
					var fullPath = fileItem.FullPath;

					try
					{
						//!!!!поддержка long
						var data = File.ReadAllBytes( fullPath );

						writer.Write( true );
						writer.Write( fileName );
						writer.Write( data.Length );
						writer.Write( fileItem.TimeModified );
						writer.Write( data );

						totalSent += data.Length;
						if( totalSent > SendFilesMaxSize )
							break;
					}
					catch//( Exception e )
					{
						//no file or busy
						writer.Write( true );
						writer.Write( fileName );
						writer.Write( -1 );
					}
				}
				else
				{
					//no cache or not in the cache
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_FileSync : ClientNetworkService
	{
		public static List<string> SkipPathsClient { get; } = new List<string>();

		public int SendFilesMaxSize { get; set; } = 10 * 1024 * 1024;

		public string DestinationFolder { get; set; } = "";
		//public string LocalFilesFolder { get; set; } = "";

		volatile StatusClass status = new StatusClass( StatusEnum.NotInitialized, "" );
		bool remainsFilesToDownloadPrepared;
		//key: FileName.ToLower()
		EDictionary<string, FileInfo> remainsFilesToDownload = new EDictionary<string, FileInfo>();
		bool filesDuringRequest;
		public long RemainsToUpdate;

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

		static string GetMD5( System.Security.Cryptography.MD5 md5, Stream stream )
		{
			var hashBytes = md5.ComputeHash( stream );

			var builder = new StringBuilder();
			for( int i = 0; i < hashBytes.Length; i++ )
				builder.Append( hashBytes[ i ].ToString( "X2" ) );
			return builder.ToString();
		}

		//static string GetMD5( System.Security.Cryptography.MD5 md5, byte[] input )
		//{
		//	var hashBytes = md5.ComputeHash( input );

		//	var builder = new StringBuilder();
		//	for( int i = 0; i < hashBytes.Length; i++ )
		//		builder.Append( hashBytes[ i ].ToString( "X2" ) );
		//	return builder.ToString();
		//}

		public static bool SkipFile( ESet<string> skipPaths, string fileName )
		{
			//!!!!slowly?

			foreach( var skipPath in skipPaths )
			{
				if( fileName.Length >= skipPath.Length && string.Compare( fileName.Substring( 0, skipPath.Length ), skipPath, true ) == 0 )
					return true;
			}
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
		public static bool IsNeedUpdate( System.Security.Cryptography.MD5 md5, long fileInfoLength, string fileInfoHash, string fullPath )
		{
			var update = false;

			if( File.Exists( fullPath ) )
			{
				var fileInfo = new System.IO.FileInfo( fullPath );

				if( fileInfoLength != fileInfo.Length )
					update = true;
				else
				{
					string hash;
					using( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read ) )
						hash = GetMD5( md5, stream );

					if( fileInfoHash != hash )
						update = true;
				}
			}
			else
				update = true;

			return update;
		}

		//call inside try/catch
		public static bool IsNeedUpdate( System.Security.Cryptography.MD5 md5, FileInfo info, string fullPath )
		{
			return IsNeedUpdate( md5, info.Length, info.Hash, fullPath );
		}

		//call inside try/catch
		void GetFilesToDownloadAndDelete( Dictionary<string, FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete )
		{
			using( var md5 = System.Security.Cryptography.MD5.Create() )
			{
				//var localFiles = GetLocalFiles();

				var allSkipPaths = new ESet<string>();
				{
					allSkipPaths.AddRangeWithCheckAlreadyContained( SkipPathsClient );

					//get ignore folders from Repository.settings
					var repositorySettingsFile = Path.Combine( DestinationFolder, "Repository.settings" );
					if( File.Exists( repositorySettingsFile ) )
					{
						if( !RepositorySettingsFile.Load( repositorySettingsFile, out var settings, out var error3 ) )
						{
							var error = "Unable to load Repository.settings. " + error3;
							throw new Exception( error );
							//return null;
						}
						allSkipPaths.AddRangeWithCheckAlreadyContained( settings.IgnoreFolders );
					}
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

						if( !SkipFile( allSkipPaths, fileName ) )
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
			var fileCountOnServer = reader.ReadInt32();
			var filesOnServer = new Dictionary<string, FileInfo>( fileCountOnServer );
			for( int n = 0; n < fileCountOnServer; n++ )
			{
				var info = new FileInfo();
				info.FileName = reader.ReadString();
				info.TimeModified = reader.ReadDateTime();
				info.Length = reader.ReadInt64();
				info.Hash = reader.ReadString();
				info.SyncMode = (RepositorySyncMode)reader.ReadByte();
				filesOnServer[ info.FileName.ToLower() ] = info;
			}

			//хеш получать из сортированного списка. везде так
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
						remainsFilesToDownload[ fileInfo.FileName.ToLower() ] = fileInfo;
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
		}

		bool ReceiveMessage_SendFilesToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var fileItems = new List<FileItem>();
			while( reader.ReadBoolean() )
			{
				var fileItem = new FileItem();
				fileItem.FileName = reader.ReadString();
				fileItem.Length = reader.ReadInt32();

				if( fileItem.Length >= 0 )
				{
					fileItem.TimeModified = reader.ReadDateTime();

					fileItem.Data = new byte[ fileItem.Length ];
					reader.ReadBuffer( fileItem.Data, 0, fileItem.Length );
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
						Directory.CreateDirectory( directory );

						File.WriteAllBytes( fullPath, fileItem.Data );

						//!!!!check
						File.SetLastWriteTimeUtc( fullPath, fileItem.TimeModified );
					}
					else
					{
						if( File.Exists( fullPath ) )
							File.Delete( fullPath );
					}

					remainsFilesToDownload.Remove( fileItem.FileName.ToLower() );

					////!!!!
					//Log.Info( "File update: " + fileItem.FileName );
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

						totalSize += fileInfo.Length;
						if( totalSize > SendFilesMaxSize )
							break;
					}


					var writer = BeginMessage( GetMessageType( "RequestFilesToServer" ) );
					writer.Write( files.Count );
					foreach( var fileInfo in files )
						writer.Write( fileInfo.FileName );
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
				value += fileInfo.Length;
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