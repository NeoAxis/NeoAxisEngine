#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	public static class RepositoryServerState
	{
		static string cloudProjectFolder = "";
		static bool loaded;
		//key: ToLower project file names
		static Dictionary<string, FileItem> files = new Dictionary<string, FileItem>();
		static Dictionary<string, FolderItem> folders = new Dictionary<string, FolderItem>();

		///////////////////////////////////////////////

		public class FileItem
		{
			public string FileName;
			public long Length;
			public DateTime TimeModified;
			public string Hash;
			public RepositorySyncMode SyncMode;
		}

		///////////////////////////////////////////////

		public class FolderItem
		{
			public bool ContainsSyncModeSynchronize;
			public bool ContainsSyncModeServerOnly;

			//public string FolderName;
			//public FolderItem Parent;
			//public List<FileItem> Files = new List<FileItem>();
			//public string Hash;
			//public RepositorySyncMode SyncMode;
		}

		///////////////////////////////////////////////

		public static void Init( string cloudProjectFolder )
		{
			Unload();
			RepositoryServerState.cloudProjectFolder = cloudProjectFolder;
		}

		public static FileItem GetFileItem( string fileName )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryServerState: GetFileItem: The class is not initialized." );

			LoadConfigFileIfNotLoaded();

			if( files.TryGetValue( fileName.ToLower(), out var item ) )
				return item;
			return null;
		}

		public static Dictionary<string, FileItem> GetAllFiles()
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryServerState: GetAllFiles: The class is not initialized." );

			LoadConfigFileIfNotLoaded();

			return files;
		}

		static string GetConfigFullPath()
		{
			return Path.Combine( cloudProjectFolder, "RepositoryServerState.config" );
		}

		static void LoadConfigFileIfNotLoaded()
		{
			if( !loaded )
				LoadConfigFile( out var error );
		}

		static void Unload()
		{
			loaded = false;
			files.Clear();
			folders.Clear();
		}

		static bool LoadConfigFile( out string error )
		{
			var files = new Dictionary<string, FileItem>();

			if( File.Exists( GetConfigFullPath() ) )
			{
				var block = TextBlockUtility.LoadFromRealFile( GetConfigFullPath(), out error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Unload();
					return false;
				}

				try
				{
					foreach( var itemBlock in block.Children )
					{
						if( itemBlock.Name == "Item" )
						{
							var item = new FileItem();
							item.FileName = itemBlock.GetAttribute( "FileName" );
							item.Length = long.Parse( itemBlock.GetAttribute( "Length", "0" ) );
							item.TimeModified = DateTime.ParseExact( itemBlock.GetAttribute( "TimeModified" ), "yyyy-MM-dd-HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture );
							item.Hash = itemBlock.GetAttribute( "Hash" );
							item.SyncMode = Enum.Parse<RepositorySyncMode>( itemBlock.GetAttribute( "SyncMode" ) );

							files[ item.FileName.ToLower() ] = item;
						}
					}
				}
				catch( Exception e )
				{
					error = e.Message;
					Unload();
					return false;
				}
			}

			RepositoryServerState.files = files;

			CalculateFolders();
			//folders.Clear();

			loaded = true;
			error = "";
			return true;
		}

		public static bool WriteConfigFile( ICollection<ClientNetworkService_FileSync.FileInfo> filesInfo, out string error )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryServerState: WriteConfigFile: The class is not initialized." );

			var block = new TextBlock();

			foreach( var fileInfo in filesInfo )
			{
				var itemBlock = block.AddChild( "Item" );
				itemBlock.SetAttribute( "FileName", fileInfo.FileName );
				itemBlock.SetAttribute( "Length", fileInfo.Length.ToString() );
				itemBlock.SetAttribute( "TimeModified", fileInfo.TimeModified.ToString( "yyyy-MM-dd-HH-mm-ss" ) );
				itemBlock.SetAttribute( "Hash", fileInfo.Hash );
				itemBlock.SetAttribute( "SyncMode", fileInfo.SyncMode.ToString() );
			}

			try
			{
				File.WriteAllText( GetConfigFullPath(), block.DumpToString() );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			Unload();

			error = "";
			return true;
		}

		static void CalculateFolders()
		{
			foreach( var fileItem in files.Values )
			{
				var folder = Path.GetDirectoryName( fileItem.FileName ).ToLower();
				while( true )
				{
					if( !folders.TryGetValue( folder, out var folderItem ) )
					{
						folderItem = new FolderItem();
						folders.Add( folder, folderItem );
					}

					if( fileItem.SyncMode == RepositorySyncMode.Synchronize )
						folderItem.ContainsSyncModeSynchronize = true;
					else //if( fileItem.SyncMode == RepositorySyncMode.ServerOnly )
						folderItem.ContainsSyncModeServerOnly = true;

					if( string.IsNullOrEmpty( folder ) )
						break;
					folder = Path.GetDirectoryName( folder );
				}
			}
		}

		public static FolderItem GetFolderItem( string folderName )
		{
			if( string.IsNullOrEmpty( cloudProjectFolder ) )
				Log.Fatal( "RepositoryServerState: GetFolderItem: The class is not initialized." );

			LoadConfigFileIfNotLoaded();

			if( folders.TryGetValue( folderName.ToLower(), out var item ) )
				return item;
			return null;
		}




		////static FolderItem GetFolderItemNoCalculate( string folderName )
		////{
		////}

		//static FolderItem GetOrCreateFolder( string folderName )
		//{
		//}

		////inside lock
		//static void CalculateFolders()
		//{
		//	//var filteredFiles = new List<FileItem>();

		//	//create tree
		//	foreach( var fileItem in files.Values )
		//	{
		//		var fileName = fileItem.FileName;

		//		var folder = Path.GetDirectoryName( fileName );
		//		if( !string.IsNullOrEmpty( folder ) )
		//		{
		//			var folderItem = GetOrCreateFolder( folder );
		//			folderItem.Files.Add( fileItem );

		//			//var f = folderItem;
		//			//do
		//			//{
		//			//	f.Hash |= 

		//			//	f = f.Parent;
		//			//} while( f != null );
		//		}

		//		//var words = fileName.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries );

		//		//var path = "";

		//		//foreach( var word in words )
		//		//{
		//		//	разный слеш на разных платформах

		//		//	path = Path.Combine( path, word );

		//		//	var folderItem = GetFolderItemNoCalculate( path );
		//		//}

		//		//if( fileName.Length >= folderName.Length && string.Compare( fileName.Substring( 0, folderName.Length ), folderName, true ) == 0 )
		//		//{
		//		//	filteredFiles.Add( fileItem );
		//		//}
		//	}

		//	//calculate states
		//	{
		//		zzzz;
		//	}

		//}

		//public static FolderItem GetFolderItem( string folderName )
		//{
		//	if( string.IsNullOrEmpty( projectFolder ) )
		//		Log.Fatal( "RepositoryServerState: GetFolderItem: The class is not initialized." );

		//	LoadConfigFileIfNotLoaded();

		//	lock( folders )
		//	{
		//		if( folders.Count == 0 )
		//			CalculateFolders();

		//		if( folders.TryGetValue( folderName.ToLower(), out var item ) )
		//			return item;
		//		return null;
		//	}
		//}
	}
}

#endif
#endif