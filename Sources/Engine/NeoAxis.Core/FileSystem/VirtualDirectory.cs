// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NeoAxis
{
	//!!!!!выключил optimize code в проекте для отладки

	/// <summary>
	/// Defines a directory for virtual file system.
	/// </summary>
	public static class VirtualDirectory
	{
		/// <summary>
		/// Determines whether the given path refers to an existing directory on virtual file system.
		/// </summary>
		/// <param name="path">The path to test.</param>
		/// <returns><b>true</b> if the directory is exists; otherwise, <b>false</b>.</returns>
		public static bool Exists( string path )
		{
			if( !VirtualFileSystem.initialized )
			{
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );
				return false;
			}

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualDirectory.Exists( \"{0}\" )", path );

			//!!!!как это всё переопределять и дополнять? и надо ли? может это на уровне ресурсов

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );
			if( Directory.Exists( realPath ) )
				return true;

			//!!!!
			//lock( VirtualFileSystem.lockObject )
			//{

			//!!!!
			//	if( InsidePackage( path ) )
			//		return true;

			//}

			return false;
		}

		//!!!!
		//public static bool InsidePackage( string path )
		//{
		//	lock( VirtualFileSystem.lockObject )
		//	{
		//		if( !VirtualFileSystem.initialized )
		//			Log.Fatal( "VirtualFileSystem: File system is not initialized." );

		//		if( VirtualFileSystem.LoggingFileOperations )
		//			Log.Info( "Logging file operations: VirtualDirectory.IsInArchive( \"{0}\" )", path );

		//		string realPath = VirtualPathUtils.GetRealPathByVirtual( path );
		//		if( Directory.Exists( realPath ) )
		//			return false;

		//		return PackageManager.IsDirectoryExists( path );
		//	}
		//}

		/// <summary>
		/// Returns the names of files in the specified directory that match the specified 
		/// search pattern, using a value to determine whether to search subdirectories.
		/// </summary>
		/// <param name="path">The directory from which to retrieve the files.</param>
		/// <param name="searchPattern">
		/// The search string to match against the names of files in path. The parameter
		/// cannot end in two periods ("..") or contain two periods ("..") followed by
		/// System.IO.Path.DirectorySeparatorChar or System.IO.Path.AltDirectorySeparatorChar,
		/// nor can it contain any of the characters in System.IO.Path.InvalidPathChars.
		/// 
		/// </param>
		/// <param name="searchOption">
		/// One of the System.IO.SearchOption values that specifies whether the search
		/// operation should include all subdirectories or only the current directory.
		/// </param>
		/// <returns>
		/// A <b>String</b> array containing containing the names of files in the 
		/// specified directory that match the specified search pattern.
		/// </returns>
		public static string[] GetFiles( string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly )
		{
			if( !VirtualFileSystem.initialized )
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualDirectory.GetFiles( \"{0}\", \"{1}\", \"{2}\" )", path, searchPattern, searchOption );

			if( searchPattern.IndexOfAny( new char[] { '\\', '/', '?' } ) != -1 )
				throw new ArgumentException( "searchPattern: following characters: \\, /, ? is not supported." );
			if( path.Contains( ".." ) )
				throw new ArgumentException( "path: \"..\" is not supported." );
			if( searchPattern.Contains( ".." ) )
				throw new ArgumentException( "searchPattern: \"..\" is not supported." );

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );

			//!!!!redirections

			string[] files;
			if( Directory.Exists( realPath ) )
				files = Directory.GetFiles( realPath, searchPattern, searchOption );
			else
				files = new string[ 0 ];

			if( VirtualPathUtility.IsUserDirectoryPath( path ) )
			{
				int prefixLength = VirtualFileSystem.Directories.UserSettings.Length + 1;
				for( int n = 0; n < files.Length; n++ )
					files[ n ] = "user:" + files[ n ].Substring( prefixLength );

				CollectionUtility.MergeSort( files, StringComparer.Ordinal );

				return files;
			}
			else
			{
				int prefixLength = VirtualFileSystem.Directories.Assets.Length + 1;
				for( int n = 0; n < files.Length; n++ )
					files[ n ] = files[ n ].Substring( prefixLength );

				//!!!!!
				//List<string> result = new List<string>( 64 );
				//PackageManager.GetFiles( path, searchPattern, searchOption, result );
				//if( result.Count != 0 )
				//{
				//	foreach( string name in files )
				//		result.Add( name );

				//	ListUtils.MergeSort( result, StringComparer.Ordinal );

				//	//remove replies
				//	for( int n = result.Count - 1; n >= 1; n-- )
				//	{
				//		if( string.Compare( result[ n ], result[ n - 1 ], true ) == 0 )
				//			result.RemoveAt( n );
				//	}

				//	files = result.ToArray();
				//}
				//else
				//{

				CollectionUtility.MergeSort( files, StringComparer.Ordinal );

				//}

				return files;
			}
		}

		/// <summary>
		/// Gets the names of subdirectories in the specified directory.
		/// </summary>
		/// <param name="path">The path for which an array of subdirectory names is returned.</param>
		/// <returns>An array of type <b>String</b> containing the names of subdirectories in path.</returns>
		public static string[] GetDirectories( string path )
		{
			return GetDirectories( path, "*" );
		}

		/// <summary>
		/// Gets an array of directories matching the specified search pattern from the 
		/// current directory.
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <param name="searchPattern">
		/// The search string to match against the names of files in path. The parameter cannot 
		/// end in two periods ("..") or contain two periods ("..") followed by 
		/// DirectorySeparatorChar or AltDirectorySeparatorChar, nor can it contain any 
		/// of the characters in InvalidPathChars.
		/// </param>
		/// <returns>
		/// A <b>String</b> array of directories matching the search pattern. 
		/// </returns>
		public static string[] GetDirectories( string path, string searchPattern )
		{
			return GetDirectories( path, searchPattern, SearchOption.TopDirectoryOnly );
		}

		/// <summary>
		/// Gets an array of directories matching the specified search pattern from the 
		/// current directory, using a value to determine whether to search subdirectories. 
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <param name="searchPattern">
		/// The search string to match against the names of files in path. The parameter 
		/// cannot end in two periods ("..") or contain two periods ("..") followed by 
		/// DirectorySeparatorChar or AltDirectorySeparatorChar, nor can it contain any 
		/// of the characters in InvalidPathChars.
		/// </param>
		/// <param name="searchOption">
		/// One of the SearchOption values that specifies whether the search operation 
		/// should include all subdirectories or only the current directory.
		/// </param>
		/// <returns>
		/// A <b>String</b> array of directories matching the search pattern. 
		/// </returns>
		public static string[] GetDirectories( string path, string searchPattern, SearchOption searchOption )
		{
			if( !VirtualFileSystem.initialized )
			{
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );
				return null;
			}

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualDirectory.GetDirectories( \"{0}\", \"{1}\", \"{2}\" )", path, searchPattern, searchOption );

			if( searchPattern.IndexOfAny( new char[] { '\\', '/', '?' } ) != -1 )
				throw new ArgumentException( "searchPattern: following characters: \\, /, ? is not supported." );
			if( path.Contains( ".." ) )
				throw new ArgumentException( "path: \"..\" is not supported." );
			if( searchPattern.Contains( ".." ) )
				throw new ArgumentException( "searchPattern: \"..\" is not supported." );

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );

			//!!!!redirections

			string[] directories;
			if( Directory.Exists( realPath ) )
				directories = Directory.GetDirectories( realPath, searchPattern, searchOption );
			else
				directories = new string[ 0 ];

			if( VirtualPathUtility.IsUserDirectoryPath( path ) )
			{
				int prefixLength = VirtualFileSystem.Directories.UserSettings.Length + 1;
				for( int n = 0; n < directories.Length; n++ )
					directories[ n ] = "user:" + directories[ n ].Substring( prefixLength );

				CollectionUtility.MergeSort( directories, StringComparer.Ordinal );

				return directories;
			}
			else
			{
				int prefixLength = VirtualFileSystem.Directories.Assets.Length + 1;
				for( int n = 0; n < directories.Length; n++ )
					directories[ n ] = directories[ n ].Substring( prefixLength );

				//!!!!!!
				//List<string> result = new List<string>( 64 );
				//PackageManager.GetDirectories( path, searchPattern, searchOption, result );
				//if( result.Count != 0 )
				//{
				//	foreach( string name in directories )
				//		result.Add( name );

				//	ListUtils.MergeSort( result, StringComparer.Ordinal );

				//	//remove replies
				//	for( int n = result.Count - 1; n >= 1; n-- )
				//	{
				//		if( string.Compare( result[ n ], result[ n - 1 ], true ) == 0 )
				//			result.RemoveAt( n );
				//	}

				//	directories = result.ToArray();
				//}
				//else
				//{

				CollectionUtility.MergeSort( directories, StringComparer.Ordinal );

				//}

				return directories;
			}
		}
	}
}
