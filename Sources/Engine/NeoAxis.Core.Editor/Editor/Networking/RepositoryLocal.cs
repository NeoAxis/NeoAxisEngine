#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis.Networking;
using System.Linq;
using System.Security.Cryptography;

namespace NeoAxis.Editor
{
	public static class RepositoryLocal
	{
		static string cloudProjectFolder = "";

		//key: ToLower project file names
		static Dictionary<string, FileItem> files = new Dictionary<string, FileItem>();
		static ESet<string> foldersContainFileItems = new ESet<string>();

		///////////////////////////////////////////////

		public enum Status
		{
			//NotAdded,
			//Default,
			Added,
			//Modified,
			Deleted,
			//Conflicted,
		}

		///////////////////////////////////////////////

		public class FileItem
		{
			public string FileName;
			public Status? Status;
			public RepositorySyncMode? SyncMode;
		}

		///////////////////////////////////////////////

		public static void Init( string cloudProjectFolder )
		{
			Unload();
			RepositoryLocal.cloudProjectFolder = cloudProjectFolder;
			LoadConfigFile();
		}

		static void Unload()
		{
			lock( files )
				files.Clear();
			lock( foldersContainFileItems )
				foldersContainFileItems.Clear();
		}

		static FileItem GetFileItem( string fileName )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: GetFileItem: The class is not initialized." );

			lock( files )
			{
				if( files.TryGetValue( fileName.ToLower(), out var item ) )
					return item;
			}

			return null;
		}

		static FileItem GetOrCreateFileItem( string fileName )
		{
			var item = GetFileItem( fileName );
			if( item == null )
			{
				item = new FileItem();
				item.FileName = fileName;
				lock( files )
					files[ fileName.ToLower() ] = item;
			}
			return item;
		}

		public static bool FileItemExists( string fileName )
		{
			return GetFileItem( fileName ) != null;
		}

		public static bool GetFileItemData( string fileName, out Status? status, out RepositorySyncMode? syncMode )
		{
			var item = GetFileItem( fileName );
			if( item != null )
			{
				status = item.Status;
				syncMode = item.SyncMode;
				return true;
			}
			status = null;
			syncMode = null;
			return false;
		}

		public static string[] GetAllFileItems()
		{
			var result = new List<string>( files.Count );
			lock( files )
			{
				foreach( var item in files.Values )
					result.Add( item.FileName );
			}
			return result.ToArray();
		}

		static string GetConfigFullPath()
		{
			return Path.Combine( cloudProjectFolder, "RepositoryLocal.config" );
		}

		static void LoadConfigFile()
		{
			if( File.Exists( GetConfigFullPath() ) )
			{
				var block = TextBlockUtility.LoadFromRealFile( GetConfigFullPath(), out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					ScreenNotifications2.Show( "Unable to load local repository file. " + error, true );
					return;
				}

				foreach( var itemBlock in block.Children )
				{
					if( itemBlock.Name == "Item" )
					{
						var item = new FileItem();

						var fileName = itemBlock.GetAttribute( "FileName" );
						if( !string.IsNullOrEmpty( fileName ) )
						{
							item.FileName = fileName;

							if( itemBlock.AttributeExists( "Status" ) )
								if( Enum.TryParse<Status>( itemBlock.GetAttribute( "Status" ), out var status ) )
									item.Status = status;

							if( itemBlock.AttributeExists( "SyncMode" ) )
								if( Enum.TryParse<RepositorySyncMode>( itemBlock.GetAttribute( "SyncMode" ), out var mode ) )
									item.SyncMode = mode;

							lock( files )
								files[ fileName.ToLower() ] = item;
						}
					}
				}

				ClearFoldersContainsFileItems();
			}
		}

		static bool SaveConfigFile()
		{
			var block = new TextBlock();

			lock( files )
			{
				foreach( var item in files.Values )
				{
					if( item.Status.HasValue || item.SyncMode.HasValue )
					{
						var itemBlock = block.AddChild( "Item" );

						itemBlock.SetAttribute( "FileName", item.FileName );
						if( item.Status.HasValue )
							itemBlock.SetAttribute( "Status", item.Status.Value.ToString() );
						if( item.SyncMode.HasValue )
							itemBlock.SetAttribute( "SyncMode", item.SyncMode.Value.ToString() );
					}
				}
			}

			try
			{
				File.WriteAllText( GetConfigFullPath(), block.DumpToString() );
			}
			catch( Exception e )
			{
				ScreenNotifications2.Show( e.Message, true );
				return false;
			}

			return true;
		}

		public static bool Add( string[] fileNames, RepositorySyncMode mode )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: Add: The class is not initialized." );

			var count = 0;
			lock( files )
			{
				foreach( var fileName in fileNames )
				{
					var item = GetOrCreateFileItem( fileName );
					if( item.Status != Status.Added )
					{
						item.Status = Status.Added;
						item.SyncMode = mode;
						count++;
					}
				}
			}

			if( count == 0 )
			{
				ScreenNotifications2.Show( "No files added.", true );
				return false;
			}

			RemoveItemsWithDefaultValues();
			ClearFoldersContainsFileItems();

			if( !SaveConfigFile() )
				return false;

			if( fileNames.Length > 1 )
				ScreenNotifications2.Show( $"{fileNames.Length} files have been added." );
			else
				ScreenNotifications2.Show( "1 file has been added." );

			return true;
		}

		public static bool Delete( string[] fileNames, bool keepLocal )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: Delete: The class is not initialized." );

			var count = 0;
			lock( files )
			{
				foreach( var fileName in fileNames )
				{
					var item = GetOrCreateFileItem( fileName );
					if( item.Status != Status.Deleted )
					{
						item.Status = Status.Deleted;
						count++;
					}
				}
			}

			if( count == 0 )
			{
				ScreenNotifications2.Show( "No files deleted.", true );
				return false;
			}

			RemoveItemsWithDefaultValues();
			ClearFoldersContainsFileItems();

			if( !SaveConfigFile() )
				return false;

			if( !keepLocal )
			{
				try
				{
					foreach( var fileName in fileNames )
					{
						var fullPath = Path.Combine( cloudProjectFolder, VirtualPathUtility.NormalizePath( fileName ) );
						//var fullPath = VirtualPathUtility.GetRealPathByProject( fileName );
						if( File.Exists( fullPath ) )
							File.Delete( fullPath );
					}
				}
				catch( Exception e )
				{
					ScreenNotifications2.Show( e.Message, true );
					return false;
				}
			}

			if( count > 1 )
				ScreenNotifications2.Show( $"{count} files have been deleted." );
			else
				ScreenNotifications2.Show( "1 file has been deleted." );

			return true;
		}

		public static bool Revert( string[] fileNames ) //RevertStatus
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: Revert: The class is not initialized." );

			var count = 0;
			lock( files )
			{
				foreach( var fileName in fileNames )
				{
					var item = GetFileItem( fileName );
					if( item != null )//&& item.Status.HasValue )
					{
						item.Status = null;
						item.SyncMode = null;
						count++;
					}
				}
			}

			if( count == 0 )
			{
				ScreenNotifications2.Show( "No files changed.", true );
				return false;
			}

			RemoveItemsWithDefaultValues();
			ClearFoldersContainsFileItems();

			if( !SaveConfigFile() )
				return false;

			if( fileNames.Length > 1 )
				ScreenNotifications2.Show( $"{fileNames.Length} files have been changed." );
			else
				ScreenNotifications2.Show( "1 file has been changed." );

			return true;
		}

		public static bool SetSyncMode( string[] fileNames, RepositorySyncMode? mode )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: SetSyncMode: The class is not initialized." );

			var count = 0;
			lock( files )
			{
				foreach( var fileName in fileNames )
				{
					var item = GetOrCreateFileItem( fileName );
					if( item.SyncMode != mode )
					{
						item.SyncMode = mode;
						count++;
					}
				}
			}

			if( count == 0 )
			{
				ScreenNotifications2.Show( "No files updated.", true );
				return false;
			}

			RemoveItemsWithDefaultValues();
			ClearFoldersContainsFileItems();

			if( !SaveConfigFile() )
				return false;

			if( count > 1 )
				ScreenNotifications2.Show( $"{count} files have been updated." );
			else
				ScreenNotifications2.Show( "1 file has been updated." );

			return true;
		}

		static void RemoveItemsWithDefaultValues()
		{
			lock( files )
			{
				var toRemove = new List<string>();

				foreach( var item in files.Values )
				{
					if( !item.Status.HasValue && !item.SyncMode.HasValue )
						toRemove.Add( item.FileName );
				}

				foreach( var fileName in toRemove )
					files.Remove( fileName.ToLower() );
			}
		}

		public static bool RemoveFileItems( ICollection<string> fileNames )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: RemoveFileItems: The class is not initialized." );

			lock( files )
			{
				foreach( var fileName in fileNames )
					files.Remove( fileName.ToLower() );
			}

			RemoveItemsWithDefaultValues();
			ClearFoldersContainsFileItems();

			if( !SaveConfigFile() )
				return false;

			return true;
		}

		public static bool GetFilesToGet( Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, out string[] filesToDownload, out string[] filesToDelete, out string error )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: GetFilesToGet: The class is not initialized." );

			error = "";
			filesToDownload = null;
			filesToDelete = null;


			//var filesOnServer = RepositoryServerState.GetAllFiles();

			try
			{
				using( var md5 = MD5.Create() )
				{
					var allSkipPaths = new ESet<string>();
					var allSkipFolderNames = new ESet<string>();
					{
						allSkipPaths.AddRangeWithCheckAlreadyContained( ClientNetworkService_FileSync.SkipPathsClient );

						//apply settings from project settings
						var projectSettingsCache = new CloudProjectProjectSettingsCache( cloudProjectFolder );
						allSkipPaths.AddRangeWithCheckAlreadyContained( projectSettingsCache.SkipPaths );
						allSkipFolderNames.AddRangeWithCheckAlreadyContained( projectSettingsCache.SkipFoldersWithName );
					}

					//если на сервере и в локальном сторедже нет, а он есть, то удалять
					var filesToDelete2 = new List<string>();
					{
						//var fileNamesOnServerOrLocalLower = new ESet<string>();
						//foreach( var info in filesOnServer.Values )
						//	fileNamesOnServerOrLocalLower.AddWithCheckAlreadyContained( info.FileName.ToLower() );

						foreach( var fullPath in Directory.GetFiles( cloudProjectFolder, "*", SearchOption.AllDirectories ) )
						{
							var fileName = fullPath.Substring( cloudProjectFolder.Length + 1 );

							//skip when Added status
							if( GetFileItemData( fileName, out var status, out _ ) && status.HasValue && status.Value == Status.Added )
								continue;

							if( !ClientNetworkService_FileSync.SkipFile( allSkipPaths, allSkipFolderNames, fileName ) )
							{
								if( !filesOnServer.ContainsKey( fileName.ToLower() ) )//if( !fileNamesOnServerOrLocalLower.Contains( fileName.ToLower() ) )
								{
									filesToDelete2.Add( fileName );

									//Log.Info( "File delete: " + fileName );
								}
							}
						}
					}


					var filesToDownload2 = new List<string>();

					//если на сервере есть, а тут нет, или различаются, то скачивать
					foreach( var info in filesOnServer.Values )
					{
						var fullPath = Path.Combine( cloudProjectFolder, info.FileName );
						var fileName = fullPath.Substring( cloudProjectFolder.Length + 1 );

						//skip when Deleted status
						if( GetFileItemData( fileName, out var status, out _ ) && status.HasValue && status.Value == Status.Deleted )
							continue;

						if( ClientNetworkService_FileSync.IsNeedUpdate( md5, info, fullPath ) )
						{
							//!!!!здесь может быть более сложная логика. определять что изменялось ли на сервере и что сейчас в local server state.

							//skip when modified and more recent time on the client
							if( File.Exists( fullPath ) && File.GetLastWriteTimeUtc( fullPath ) > info.TimeModified )
								continue;

							filesToDownload2.Add( info.FileName );
						}
					}

					//need?
					////sort filesToDownload and filesToDelete
					//{
					//}

					filesToDownload = filesToDownload2.ToArray();
					filesToDelete = filesToDelete2.ToArray();
				}

			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		public static bool GetFilesToCommit( out string[] filesToUpload, out string[] filesToDelete, out string error )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: GetFilesToCommit: The class is not initialized." );

			error = "";
			filesToUpload = null;
			filesToDelete = null;

			var filesOnServer = RepositoryServerState.GetAllFiles();
			//!!!!если не загрузился

			try
			{
				using( var md5 = MD5.Create() )
				{
					var allFileItems = GetAllFileItems();


					var filesToUpload2 = new ESet<string>();
					var filesToDelete2 = new ESet<string>();


					foreach( var fileName in allFileItems )
					{
						if( GetFileItemData( fileName, out var status, out var syncMode ) )
						{
							var fullPath = RepositoryUtility.GetRealPathByAllFiles( cloudProjectFolder, fileName );
							//var fullPath = VirtualPathUtility.GetRealPathByAllFiles( fileName );

							if( status.HasValue )
							{
								//if added
								if( status.Value == Status.Added && File.Exists( fullPath ) )
									filesToUpload2.AddWithCheckAlreadyContained( fileName );

								//if deleted
								if( status.Value == Status.Deleted && filesOnServer.ContainsKey( fileName.ToLower() ) )
									filesToDelete2.AddWithCheckAlreadyContained( fileName );
							}

							//if sync mode was changed
							if( syncMode.HasValue )
							{
								if( filesOnServer.TryGetValue( fileName.ToLower(), out var serverItem ) && serverItem.SyncMode != syncMode.Value )
									filesToUpload2.AddWithCheckAlreadyContained( fileName );
							}
						}
					}

					//if exists on the server, exists of the client, it is modified and it more recent on the client
					foreach( var fileItem in filesOnServer.Values )
					{
						var fullPath = RepositoryUtility.GetRealPathByAllFiles( cloudProjectFolder, fileItem.FileName );
						//var fullPath = VirtualPathUtility.GetRealPathByAllFiles( fileItem.FileName );

						if( File.Exists( fullPath ) )
						{
							if( ClientNetworkService_FileSync.IsNeedUpdate( md5, fileItem.Length, fileItem.Hash, fullPath ) )
							{

								//!!!!здесь может быть более сложная логика. определять что изменялось ли на сервере и что сейчас в local server state.

								if( File.GetLastWriteTimeUtc( fullPath ) > fileItem.TimeModified )
								{
									filesToUpload2.AddWithCheckAlreadyContained( fileItem.FileName );
								}
							}
						}
					}

					filesToUpload = filesToUpload2.ToArray();
					filesToDelete = filesToDelete2.ToArray();
				}

			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		static void ClearFoldersContainsFileItems()
		{
			lock( foldersContainFileItems )
				foldersContainFileItems.Clear();
		}

		static void UpdateFoldersContainsFileItems()
		{
			lock( files )
			{
				foreach( var fileItem in files )
				{
					var folder = Path.GetDirectoryName( fileItem.Key );
					while( true )
					{
						if( !foldersContainFileItems.AddWithCheckAlreadyContained( folder ) )
							break;

						if( string.IsNullOrEmpty( folder ) )
							break;
						folder = Path.GetDirectoryName( folder );
					}
				}
			}
		}

		public static bool FolderContainsFileItems( string folderName )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryLocal: FolderContainsFileItems: The class is not initialized." );

			lock( foldersContainFileItems )
			{
				if( foldersContainFileItems.Count == 0 )
					UpdateFoldersContainsFileItems();
				return foldersContainFileItems.Contains( folderName.ToLower() );
			}

			////slowly
			//var folderName2 = folderName.ToLower();
			//lock( files )
			//{
			//	foreach( var fileItem in files )
			//	{
			//		var fileName = fileItem.Key;

			//		if( fileName.Length >= folderName2.Length && fileName.Substring( 0, folderName2.Length ) == folderName2 )
			//			return true;
			//	}
			//}
			//return false;
		}
	}
}

#endif
#endif