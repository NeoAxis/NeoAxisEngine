// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace NeoAxis
{
	//!!!!public
	static class ArchiveManager
	{
		static List<BakingArchive> archives = new List<BakingArchive>();
		static Dictionary<string, BakingArchive> files = new Dictionary<string, BakingArchive>();

		/////////////////////////////////////////

		internal static bool Init()
		{
			//!!!!error handling

			try
			{
				foreach( var path in Directory.GetFiles( VirtualFileSystem.Directories.Assets, "*.neoaxisbaking", SearchOption.AllDirectories ) )
					LoadBakingFile( path );
			}
			catch( Exception e )
			{
				Log.Error( e.Message );
				return false;
			}

			return true;
		}

		/////////////////////////////////////////

		class BakingArchive
		{
			ZipArchive zipArchive;
			Dictionary<string, ZipArchiveEntry> entries = new Dictionary<string, ZipArchiveEntry>();

			//

			public BakingArchive( string path )
			{
				zipArchive = ZipFile.Open( path, ZipArchiveMode.Read );

				var virtualFolder = VirtualPathUtility.GetVirtualPathByReal( Path.GetDirectoryName( path ) );

				foreach( var entry in zipArchive.Entries )
				{
					var fileKey = VirtualPathUtility.NormalizePath( Path.Combine( virtualFolder, entry.FullName ) ).ToLower();

					entries[ fileKey ] = entry;
					files[ fileKey ] = this;
				}
			}

			public VirtualFileStream FileOpen( string fileKey )
			{
				entries.TryGetValue( fileKey, out var entry );

				using( var stream = entry.Open() )
				{
					var bytes = new byte[ entry.Length ];
					stream.Read( bytes, 0, bytes.Length );

					//decode
					for( int n = 0; n < bytes.Length; n++ )
						bytes[ n ] = (byte)~bytes[ n ];

					return new MemoryVirtualFileStream( bytes );
				}
			}

			public long FileGetLength( string fileKey )
			{
				entries.TryGetValue( fileKey, out var entry );
				return entry.Length;
			}
		}

		/////////////////////////////////////////

		static void LoadBakingFile( string path )
		{
			var archive = new BakingArchive( path );
			archives.Add( archive );
		}

		public static bool FileExists( string path )
		{
			if( archives.Count != 0 )
			{
				string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
				return files.ContainsKey( fileKey );
			}
			return false;
		}

		public static (bool fileExists, VirtualFileStream stream) FileOpen( string path )
		{
			string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
			if( files.TryGetValue( fileKey, out var archive ) )
				return (true, archive.FileOpen( fileKey ));
			else
				return (false, null);
		}

		public static (bool fileExists, long length) FileGetLength( string path )
		{
			string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
			if( files.TryGetValue( fileKey, out var archive ) )
				return (true, archive.FileGetLength( fileKey ));
			else
				return (false, 0);
		}
	}

	//!!!!!скорее нужен некий abstract virtual resource container
	//!!!!threading

	//static class PackageManager
	//{
	//	//Key: lower normalized real file path
	//	static Dictionary<string, Package> packages = new Dictionary<string, Package>();
	//	//Key: lower normalized file name
	//	static Dictionary<string, FileInfo> packagesFiles = new Dictionary<string, FileInfo>( 256 );
	//	static DirectoryInfo packagesRootDirectory = new DirectoryInfo( null, true );

	//	///////////////////////////////////////////

	//	public class FileInfo
	//	{
	//		string fileName;
	//		Package package;
	//		string inPackageFileName;
	//		long length;

	//		//

	//		public FileInfo( string fileName, Package package, string inArchiveFileName, long length )
	//		{
	//			this.fileName = fileName;
	//			this.package = package;
	//			this.inPackageFileName = inArchiveFileName;
	//			this.length = length;
	//		}

	//		public string FileName
	//		{
	//			get { return fileName; }
	//		}

	//		public Package Package
	//		{
	//			get { return package; }
	//		}

	//		public string InArchiveFileName
	//		{
	//			get { return inPackageFileName; }
	//		}

	//		public long Length
	//		{
	//			get { return length; }
	//		}
	//	}

	//	///////////////////////////////////////////

	//	class DirectoryInfo
	//	{
	//		internal string name;
	//		internal bool realFileSystemDirectory;
	//		internal List<string> files;
	//		//Key: lower name
	//		internal Dictionary<string, DirectoryInfo> directories;

	//		//

	//		public DirectoryInfo( string name, bool realFileSystemDirectory )
	//		{
	//			this.name = name;
	//			this.realFileSystemDirectory = realFileSystemDirectory;
	//		}

	//		public string Name
	//		{
	//			get { return name; }
	//		}

	//		public bool RealFileSystemDirectory
	//		{
	//			get { return realFileSystemDirectory; }
	//		}

	//		public List<string> Files
	//		{
	//			get { return files; }
	//		}

	//		public Dictionary<string, DirectoryInfo> Directories
	//		{
	//			get { return directories; }
	//		}
	//	}

	//	///////////////////////////////////////////

	//	public static bool Init()
	//	{
	//		if( !LoadPackagesAtStartup() )
	//			return false;
	//		return true;
	//	}

	//	public static void Shutdown()
	//	{
	//		foreach( Package package in packages.Values )
	//			package.Dispose();
	//		packages.Clear();

	//		packagesRootDirectory = new DirectoryInfo( null, true );
	//	}

	//	static List<string> GetSortedPackagesToLoadAtStartup()
	//	{
	//		//!!!!!тестить
	//		string[] fileNames = Directory.GetFiles( VirtualFileSystem.ProjectDataDirectory, "*.neoaxispackage", SearchOption.AllDirectories );

	//		List<Pair<string, float>> toLoad = new List<Pair<string, float>>();
	//		foreach( string fileName in fileNames )
	//		{
	//			string error;
	//			Package.InfoClass info = LoadPackageInfo( fileName, out error );
	//			//!!!!!или без фаталов?
	//			if( !string.IsNullOrEmpty( error ) )
	//				Log.Fatal( "PackageManager: LoadPackageInfo: Unable to load package info from \'{0}\'. Error: {1}", fileName, error );
	//			if( info.LoadAtStartup )
	//				toLoad.Add( new Pair<string, float>( fileName, info.LoadingPriority ) );
	//		}

	//		ListUtils.SelectionSort( toLoad, delegate ( Pair<string, float> item1, Pair<string, float> item2 )
	//		{
	//			if( item1.Second > item2.Second )
	//				return -1;
	//			if( item1.Second < item2.Second )
	//				return 1;
	//			return string.Compare( item1.First, item2.First );
	//		} );

	//		List<string> result = new List<string>();
	//		foreach( Pair<string, float> pair in toLoad )
	//			result.Add( pair.First );
	//		return result;
	//	}

	//	static bool LoadPackagesAtStartup()
	//	{
	//		//!!!!откуда и как еще загружать?

	//		foreach( string fileName in GetSortedPackagesToLoadAtStartup() )
	//		{
	//			string error;
	//			LoadPackage( fileName, out error );
	//			//!!!!!или без фаталов?
	//			if( !string.IsNullOrEmpty( error ) )
	//				Log.Fatal( "PackageManager: LoadPackage: Unable to load package \'{0}\'. Error: {1}", fileName, error );
	//		}

	//		return true;
	//	}

	//	public static VirtualFileStream FileOpen( string virtualPath )
	//	{
	//		//!!!!lock object
	//		lock ( VirtualFileSystem.lockObject )
	//		{
	//			FileInfo fileInfo;
	//			if( !GetFileInfo( virtualPath, out fileInfo ) )
	//				return null;
	//			return fileInfo.Package.OnFileOpen( fileInfo.InArchiveFileName );
	//		}
	//	}

	//	public static bool GetFileInfo( string virtualPath, out FileInfo fileInfo )
	//	{
	//		//!!!!lock object
	//		lock( VirtualFileSystem.lockObject )
	//		{
	//			string fileKey = VirtualPathUtils.NormalizePath( virtualPath ).ToLower();
	//			if( !packagesFiles.TryGetValue( fileKey, out fileInfo ) )
	//				return false;
	//			return true;
	//		}
	//	}

	//	static DirectoryInfo GetDirectory( string path, bool createInNotExists )
	//	{
	//		string[] names = path.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries );

	//		string walkedPath = "";

	//		DirectoryInfo directory = packagesRootDirectory;
	//		foreach( string name in names )
	//		{
	//			walkedPath = Path.Combine( walkedPath, name );

	//			DirectoryInfo child = null;
	//			string key = name.ToLower();
	//			if( directory.Directories != null )
	//				directory.Directories.TryGetValue( key, out child );

	//			if( child == null )
	//			{
	//				if( createInNotExists )
	//				{
	//					bool realFileSystemDirectory;
	//					{
	//						string realPath = Path.Combine( VirtualFileSystem.ProjectDataDirectory,
	//							walkedPath );
	//						realFileSystemDirectory = Directory.Exists( realPath );
	//					}

	//					child = new DirectoryInfo( name, realFileSystemDirectory );

	//					if( directory.directories == null )
	//						directory.directories = new Dictionary<string, DirectoryInfo>();
	//					directory.directories.Add( key, child );
	//				}
	//				else
	//					return null;
	//			}

	//			directory = child;
	//		}

	//		return directory;
	//	}

	//	static internal bool IsDirectoryExists( string path )
	//	{
	//		//lock on top level

	//		DirectoryInfo directory = GetDirectory( path, false );
	//		return directory != null && !directory.RealFileSystemDirectory;
	//	}

	//	static bool IsSearchPatternMatch( string str, string searchPattern )
	//	{
	//		if( searchPattern == "*" || searchPattern == "*.*" )
	//			return true;

	//		string tempStr = str;
	//		string tempPattern = searchPattern;
	//		//if (!caseSensitive)
	//		{
	//			tempStr = tempStr.ToLower();
	//			tempPattern = tempPattern.ToLower();
	//		}

	//		int strIndex = 0;
	//		int patIndex = 0;
	//		int lastWildCardIt = tempPattern.Length;
	//		while( strIndex != tempStr.Length && patIndex != tempPattern.Length )
	//		{
	//			if( tempPattern[ patIndex ] == '*' )
	//			{
	//				lastWildCardIt = patIndex;
	//				// Skip over looking for next character
	//				patIndex++;
	//				if( patIndex == tempPattern.Length )
	//				{
	//					// Skip right to the end since * matches the entire rest of the string
	//					strIndex = tempStr.Length;
	//				}
	//				else
	//				{
	//					// scan until we find next pattern character
	//					while( strIndex != tempStr.Length && tempStr[ strIndex ] != tempPattern[ patIndex ] )
	//						strIndex++;
	//				}
	//			}
	//			else
	//			{
	//				if( tempPattern[ patIndex ] != tempStr[ strIndex ] )
	//				{
	//					if( lastWildCardIt != tempPattern.Length )
	//					{
	//						// The last wildcard can match this incorrect sequence
	//						// rewind pattern to wildcard and keep searching
	//						patIndex = lastWildCardIt;
	//						lastWildCardIt = tempPattern.Length;
	//					}
	//					else
	//					{
	//						// no wildwards left
	//						return false;
	//					}
	//				}
	//				else
	//				{
	//					patIndex++;
	//					strIndex++;
	//				}
	//			}

	//		}
	//		// If we reached the end of both the pattern and the string, we succeeded
	//		if( patIndex == tempPattern.Length && strIndex == tempStr.Length )
	//			return true;
	//		else
	//			return false;
	//	}

	//	static void GetFiles( string directoryPath, DirectoryInfo directory, string searchPattern, SearchOption searchOption, List<string> outList )
	//	{
	//		if( searchOption == SearchOption.AllDirectories )
	//		{
	//			if( directory.Directories != null )
	//			{
	//				foreach( DirectoryInfo child in directory.Directories.Values )
	//				{
	//					string childPath = Path.Combine( directoryPath, child.Name );
	//					GetFiles( childPath, child, searchPattern, searchOption, outList );
	//				}
	//			}
	//		}

	//		if( directory.Files != null )
	//		{
	//			for( int n = 0; n < directory.Files.Count; n++ )
	//			{
	//				string name = directory.Files[ n ];

	//				if( IsSearchPatternMatch( name, searchPattern ) )
	//				{
	//					string fullName = Path.Combine( directoryPath, name );
	//					outList.Add( fullName );
	//				}
	//			}
	//		}
	//	}

	//	static void GetDirectories( string directoryPath, DirectoryInfo directory, string searchPattern, SearchOption searchOption,
	//		List<string> outList )
	//	{
	//		if( directory.Directories != null )
	//		{
	//			if( searchOption == SearchOption.AllDirectories )
	//			{
	//				foreach( DirectoryInfo child in directory.Directories.Values )
	//				{
	//					string childPath = Path.Combine( directoryPath, child.Name );
	//					GetDirectories( childPath, child, searchPattern, searchOption, outList );
	//				}
	//			}

	//			{
	//				foreach( DirectoryInfo child in directory.Directories.Values )
	//				{
	//					if( !child.RealFileSystemDirectory )
	//					{
	//						string name = child.Name;
	//						if( IsSearchPatternMatch( name, searchPattern ) )
	//						{
	//							string fullName = Path.Combine( directoryPath, name );
	//							outList.Add( fullName );
	//						}
	//					}
	//				}
	//			}
	//		}
	//	}

	//	internal static void GetFiles( string path, string searchPattern, SearchOption searchOption, List<string> outList )
	//	{
	//		//lock on top level

	//		DirectoryInfo directory = GetDirectory( path, false );
	//		if( directory != null )
	//			GetFiles( path, directory, searchPattern, searchOption, outList );
	//	}

	//	internal static void GetDirectories( string path, string searchPattern, SearchOption searchOption, List<string> outList )
	//	{
	//		//lock on top level

	//		DirectoryInfo directory = GetDirectory( path, false );
	//		if( directory != null )
	//			GetDirectories( path, directory, searchPattern, searchOption, outList );
	//	}

	//	public static ICollection<Package> Packages
	//	{
	//		get { return packages.Values; }
	//	}

	//	public static Package GetPackage( string realFileName )
	//	{
	//		//lock on top level

	//		string key = VirtualPathUtils.NormalizePath( realFileName ).ToLower();
	//		Package package;
	//		packages.TryGetValue( key, out package );
	//		return package;
	//	}

	//	static Package LoadPackageInternal( string realFileName, bool loadInfoOnly, out string error )
	//	{
	//		Package package = null;
	//		error = "";
	//		VirtualFileSystem.CallPackageLoading( realFileName, loadInfoOnly, ref package, ref error );
	//		if( !string.IsNullOrEmpty( error ) )
	//			return null;

	//		if( package == null )
	//		{
	//			package = new ZipPackage( realFileName, loadInfoOnly, out error );
	//			if( !string.IsNullOrEmpty( error ) )
	//				return null;
	//		}

	//		return package;
	//	}

	//	public static Package.InfoClass LoadPackageInfo( string realFileName, out string error )
	//	{
	//		realFileName = VirtualPathUtils.NormalizePath( realFileName );

	//		//check already loaded
	//		{
	//			Package package2 = GetPackage( realFileName );
	//			if( package2 != null )
	//			{
	//				error = "";
	//				return package2.Info;
	//			}
	//		}

	//		Package package = LoadPackageInternal( realFileName, true, out error );
	//		if( package == null )
	//			return null;
	//		Package.InfoClass info = package.Info;
	//		package.Dispose();
	//		return info;
	//	}

	//	public static Package LoadPackage( string realFileName, out string error )
	//	{
	//		realFileName = VirtualPathUtils.NormalizePath( realFileName );

	//		//check already loaded
	//		{
	//			Package package2 = GetPackage( realFileName );
	//			if( package2 != null )
	//				Log.Fatal( "VirtualFileSystem: LoadPackage: The package \'{0}\' is already loaded.", realFileName );
	//		}

	//		//load
	//		Package package = LoadPackageInternal( realFileName, false, out error );
	//		if( package == null )
	//			return null;
	//		packages.Add( realFileName.ToLower(), package );

	//		//get files and directories
	//		string archiveDirectoryName = VirtualPathUtils.GetVirtualPathByReal( Path.GetDirectoryName( package.RealFileName ) );

	//		string[] directoryNames;
	//		Package.FileInfo[] fileInfos;
	//		package.OnGetDirectoryAndFileList( out directoryNames, out fileInfos );

	//		foreach( string directoryName in directoryNames )
	//		{
	//			string correctedName = directoryName.Trim( '\\', '/' );
	//			string fullDirectoryName = Path.Combine( archiveDirectoryName, correctedName );
	//			GetDirectory( fullDirectoryName, true );
	//		}

	//		foreach( Package.FileInfo fileInfo in fileInfos )
	//		{
	//			string normalizedFileName = VirtualPathUtils.NormalizePath( fileInfo.FileName );

	//			//add directory if it is not exists
	//			{
	//				string directoryName = Path.GetDirectoryName( normalizedFileName );
	//				string correctedName = directoryName.Trim( '\\', '/' );
	//				string fullDirectoryName = Path.Combine( archiveDirectoryName, correctedName );
	//				GetDirectory( fullDirectoryName, true );
	//			}

	//			string fullFileName = Path.Combine( archiveDirectoryName, normalizedFileName );

	//			//add to directory infos
	//			{
	//				DirectoryInfo directory = GetDirectory( Path.GetDirectoryName( fullFileName ), false );
	//				if( directory.files == null )
	//					directory.files = new List<string>();
	//				directory.files.Add( Path.GetFileName( fullFileName ) );
	//			}

	//			//add files to filesDictionary
	//			{
	//				FileInfo existFileInfo;
	//				string fileKey = fullFileName.ToLower();
	//				if( packagesFiles.TryGetValue( fileKey, out existFileInfo ) )
	//				{
	//					//already exists file info. old file will be replaced by new archive
	//					packagesFiles.Remove( fileKey );
	//				}
	//				//add file info
	//				FileInfo info = new FileInfo( fullFileName, package, fileInfo.FileName, fileInfo.Length );
	//				packagesFiles.Add( fileKey, info );
	//			}
	//		}

	//		VirtualFileSystem.CallPackageLoaded( package );

	//		return package;
	//	}
	//}
}
