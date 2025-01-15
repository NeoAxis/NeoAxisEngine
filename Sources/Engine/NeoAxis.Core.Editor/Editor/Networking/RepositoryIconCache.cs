#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace NeoAxis.Editor
{
	public static class RepositoryIconCache
	{
		static string cloudProjectFolder;
		static RepositoryFileWatcher useSpecialFileWatcher;
		//static bool useEngineFileWatcher;

		//key: ToLower project file names
		static Dictionary<string, FileItem> cachedFiles = new Dictionary<string, FileItem>();
		static Dictionary<string, FolderItem> cachedFolders = new Dictionary<string, FolderItem>();

		static MD5 md5;

		///////////////////////////////////////////////

		public class FileItem
		{
			public string FileName;
			public bool Exists;
			public long Length;
			public DateTime TimeModified;
			public string Hash;
		}

		///////////////////////////////////////////////

		public class FolderItem
		{
			public bool Modified;
		}

		///////////////////////////////////////////////

		public static void Init( string cloudProjectFolder, RepositoryFileWatcher useSpecialFileWatcher )// bool useEngineFileWatcher )
		{
			RepositoryIconCache.cloudProjectFolder = cloudProjectFolder;
			RepositoryIconCache.useSpecialFileWatcher = useSpecialFileWatcher;
			//RepositoryIconCache.useEngineFileWatcher = useEngineFileWatcher;

			if( useSpecialFileWatcher != null )
				useSpecialFileWatcher.Update += VirtualFileWatcher_Update;
			else
				VirtualFileWatcher.Update += VirtualFileWatcher_Update;

			//if( useEngineFileWatcher )
			//{
			//	VirtualFileWatcher.Update += VirtualFileWatcher_Update;
			//}
			//else
			//{
			//	RepositoryIconCacheSpecialFileWatcher.Init( cloudProjectFolder );
			//	RepositoryIconCacheSpecialFileWatcher.Update += VirtualFileWatcher_Update;
			//}
		}

		public static void Shutdown()
		{
			if( useSpecialFileWatcher != null )
				useSpecialFileWatcher.Update -= VirtualFileWatcher_Update;
			else
				VirtualFileWatcher.Update -= VirtualFileWatcher_Update;

			//if( useEngineFileWatcher )
			//{
			//	VirtualFileWatcher.Update -= VirtualFileWatcher_Update;
			//}
			//else
			//{
			//	RepositoryIconCacheSpecialFileWatcher.Update -= VirtualFileWatcher_Update;
			//	RepositoryIconCacheSpecialFileWatcher.Shutdown();
			//}
		}

		static void RemoveFileAndParentFolders( string fileName )
		{
			var updated = false;

			var fileNameLower = fileName.ToLower();

			lock( cachedFiles )
			{
				if( cachedFiles.Remove( fileNameLower ) )
					updated = true;
			}

			lock( cachedFolders )
			{
				try
				{
					var current = fileNameLower;
					while( true )
					{
						current = Path.GetDirectoryName( current );

						if( cachedFolders.Remove( current ) )
							updated = true;

						if( string.IsNullOrEmpty( current ) )
							break;
					}
				}
				catch { }
			}

			//to update additional icons
			if( updated )
			{
				EditorAPI2.FindWindow<ResourcesWindow>()?.Invalidate( true );

				//Launcher specific
				LauncherEditRepositoryForm.Instance?.Invalidate( true );
			}
		}

		static void VirtualFileWatcher_Update( FileSystemEventArgs args )
		{
			switch( args.ChangeType )
			{
			case WatcherChangeTypes.Created:
			case WatcherChangeTypes.Changed:
			case WatcherChangeTypes.Deleted:
				{
					var fileName = RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, args.FullPath );
					//var fileName = VirtualPathUtility.GetAllFilesPathByReal( args.FullPath );
					if( !string.IsNullOrEmpty( fileName ) )
						RemoveFileAndParentFolders( fileName );
				}
				break;

			case WatcherChangeTypes.Renamed:
				{
					var args2 = (RenamedEventArgs)args;

					{
						var fileName = RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, args2.FullPath );
						//var fileName = VirtualPathUtility.GetAllFilesPathByReal( args2.FullPath );
						if( !string.IsNullOrEmpty( fileName ) )
							RemoveFileAndParentFolders( fileName );
					}

					{
						var fileName = RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, args.FullPath );
						//var fileName = VirtualPathUtility.GetAllFilesPathByReal( args.FullPath );
						if( !string.IsNullOrEmpty( fileName ) )
							RemoveFileAndParentFolders( fileName );
					}
				}
				break;
			}
		}

		static string GetMD5( MD5 md5, Stream stream )
		{
			var hashBytes = md5.ComputeHash( stream );

			var builder = new StringBuilder();
			for( int i = 0; i < hashBytes.Length; i++ )
				builder.Append( hashBytes[ i ].ToString( "X2" ) );
			return builder.ToString();
		}

		public static FileItem GetFileItem( string fileName )
		{
			FileItem item;
			lock( cachedFiles )
				cachedFiles.TryGetValue( fileName.ToLower(), out item );

			if( item == null )
			{
				try
				{
					var fullPath = RepositoryUtility.GetRealPathByAllFiles( cloudProjectFolder, fileName );
					//var fullPath = VirtualPathUtility.GetRealPathByAllFiles( fileName );

					item = new FileItem();
					item.FileName = fileName;

					if( File.Exists( fullPath ) )
					{
						if( md5 == null )
							md5 = MD5.Create();

						var fileInfo = new FileInfo( fullPath );

						string hash;
						using( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
							hash = GetMD5( md5, stream );

						item.Exists = true;
						item.TimeModified = fileInfo.LastWriteTimeUtc;
						item.Length = fileInfo.Length;
						item.Hash = hash;
					}

					lock( cachedFiles )
						cachedFiles[ fileName.ToLower() ] = item;
				}
				catch { }
			}

			return item;
		}

		static bool CheckFolderModified( string folderNameLower )
		{
			if( RepositoryLocal.FolderContainsFileItems( folderNameLower ) )
				return true;

			var folderRealPath = RepositoryUtility.GetRealPathByAllFiles( cloudProjectFolder, folderNameLower );
			//var folderRealPath = VirtualPathUtility.GetRealPathByAllFiles( folderNameLower );

			try
			{
				//check directories
				foreach( var childDirectoryRealPath in Directory.GetDirectories( folderRealPath ) )
				{
					var childDirectory = RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, childDirectoryRealPath );
					//var childDirectory = VirtualPathUtility.GetAllFilesPathByReal( childDirectoryRealPath );

					if( FolderModified( childDirectory ) )
						return true;
				}

				//check files
				foreach( var childFileRealPath in Directory.GetFiles( folderRealPath ) )
				{
					var childFile = RepositoryUtility.GetAllFilesPathByReal( cloudProjectFolder, childFileRealPath );
					//var childFile = VirtualPathUtility.GetAllFilesPathByReal( childFileRealPath );

					var serverStateItem = RepositoryServerState.GetFileItem( childFile );
					var localItemExists = RepositoryLocal.GetFileItemData( childFile, out var localStatus, out var localSyncMode );

					if( serverStateItem != null || localItemExists )
					{
						if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Added )
							return true;
						else if( localStatus.HasValue && localStatus.Value == RepositoryLocal.Status.Deleted )
							return true;
						else
						{
							if( serverStateItem != null )
							{
								//check for modified data of the file
								var iconsItem = RepositoryIconCache.GetFileItem( childFile );
								var equal = iconsItem != null && serverStateItem.Length == iconsItem.Length && serverStateItem.Hash == iconsItem.Hash;
								if( !equal )
									return true;

								//changed sync mode is also considered as modified
								if( localSyncMode.HasValue && serverStateItem.SyncMode != localSyncMode.Value )
									return true;
							}
						}
					}
				}
			}
			catch { }

			return false;
		}

		public static bool FolderModified( string folderName )
		{
			var folderNameLower = folderName.ToLower();

			lock( cachedFolders )
			{
				FolderItem item;
				if( !cachedFolders.TryGetValue( folderNameLower, out item ) )
				{
					item = new FolderItem();
					item.Modified = CheckFolderModified( folderNameLower );
					cachedFolders[ folderNameLower ] = item;
				}

				return item.Modified;
			}
		}
	}
}
#endif
#endif