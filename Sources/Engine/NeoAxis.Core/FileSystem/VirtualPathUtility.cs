// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with virtual file paths.
	/// </summary>
	public static class VirtualPathUtility
	{
		public static string NormalizePath( string path )
		{
			string result = path;
			if( result != null )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					result = result.Replace( '/', '\\' );
				else
					result = result.Replace( '\\', '/' );
			}
			return result;
		}

		///// <summary>
		///// Converts a file path of real file system to path of virtual file system.
		///// </summary>
		///// <param name="realPath">The real file path.</param>
		///// <returns>The virtual file path.</returns>
		public static bool GetVirtualPathByReal( string realPath, out string virtualPath )
		{
			if( realPath == null )
				Log.Fatal( "VirtualPathUtility: GetVirtualPathByReal: realPath == null." );

			realPath = NormalizePath( realPath );
			if( !Path.IsPathRooted( realPath ) )
				realPath = Path.Combine( VirtualFileSystem.Directories.Binaries, realPath );

			//project data
			{
				string dir = VirtualFileSystem.Directories.Assets;
				if( realPath.Length >= dir.Length )
				{
					if( string.Equals( realPath, dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = "";
						return true;
					}
					else if( string.Equals( realPath.Substring( 0, dir.Length ), dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = realPath.Substring( dir.Length + 1, realPath.Length - dir.Length - 1 );
						return true;
					}
				}
			}

			//user settings
			{
				string dir = VirtualFileSystem.Directories.UserSettings;
				if( realPath.Length > dir.Length )
				{
					if( string.Equals( realPath, dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = "user:";
						return true;
					}
					else if( string.Equals( realPath.Substring( 0, dir.Length ), dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = "user:" + realPath.Substring( dir.Length + 1, realPath.Length - dir.Length - 1 );
						return true;
					}
				}
			}

			//project directory
			{
				string dir = VirtualFileSystem.Directories.Project;
				if( realPath.Length > dir.Length )
				{
					if( string.Equals( realPath, dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = "project:";
						return true;
					}
					else if( string.Equals( realPath.Substring( 0, dir.Length ), dir, StringComparison.OrdinalIgnoreCase ) )
					{
						virtualPath = "project:" + realPath.Substring( dir.Length + 1, realPath.Length - dir.Length - 1 );
						return true;
					}
				}
			}

			virtualPath = "";
			return false;
		}

		/// <summary>
		/// Converts a file path of real file system to path of virtual file system.
		/// </summary>
		/// <param name="realPath">The real file path.</param>
		/// <returns>The virtual file path.</returns>
		public static string GetVirtualPathByReal( string realPath )
		{
			GetVirtualPathByReal( realPath, out var virtualPath );
			return virtualPath;
		}

		/// <summary>
		/// Converts a file path of virtual file system to path of real file system.
		/// </summary>
		/// <param name="virtualPath">The virtual file path.</param>
		/// <returns>The real file path.</returns>
		public static string GetRealPathByVirtual( string virtualPath )
		{
			virtualPath = NormalizePath( virtualPath );

			//user:
			if( virtualPath.Length >= 5 && virtualPath[ 4 ] == ':' )
			{
				string prefix = virtualPath.Substring( 0, 5 );
				if( prefix == "user:" )
					return Path.Combine( VirtualFileSystem.Directories.UserSettings, virtualPath.Substring( 5 ) );
			}

			//project:
			if( virtualPath.Length >= 8 && virtualPath[ 7 ] == ':' )
			{
				string prefix = virtualPath.Substring( 0, 8 );
				if( prefix == "project:" )
					return Path.Combine( VirtualFileSystem.Directories.Project, virtualPath.Substring( 8 ) );
			}

			return Path.Combine( VirtualFileSystem.Directories.Assets, virtualPath );
		}

		public static bool IsCorrectFileName( string fileName )
		{
			if( string.IsNullOrEmpty( fileName ) )
				return false;

			char[] invalidChars = Path.GetInvalidFileNameChars();
			foreach( char c in fileName )
			{
				if( Array.IndexOf<char>( invalidChars, c ) != -1 )
					return false;
			}

			if( Path.GetExtension( fileName ) != "" )
			{
				if( Path.GetFileNameWithoutExtension( fileName ) == "" )
					return false;
			}

			return true;
		}

		public static bool IsUserDirectoryPath( string path )
		{
			if( path.Length >= 5 && path[ 4 ] == ':' )
			{
				string prefix = path.Substring( 0, 5 );
				if( prefix == "user:" )
					return true;
			}
			return false;
		}

		//!!!!!
		//public static string ConvertRelativePathToFull( string ownerDirectoryName, string path )
		//{
		//	if( !string.IsNullOrEmpty( path ) )
		//	{
		//		if( path.Length > 1 && path[ 0 ] == '.' && ( path[ 1 ] == '\\' || path[ 1 ] == '/' ) )
		//		{
		//			if( ownerDirectoryName == null )
		//				ownerDirectoryName = "";
		//			return Path.Combine( ownerDirectoryName, path.Substring( 2 ) );
		//		}
		//	}
		//	return path;
		//}

		//!!!!!!
		//public static string ConvertFullPathToRelative( string ownerDirectoryName, string path )
		//{
		//	if( !string.IsNullOrEmpty( path ) )
		//	{
		//		string normalizedPath = NormalizePath( path );
		//		if( !string.IsNullOrEmpty( ownerDirectoryName ) )
		//		{
		//			string ownerDirectoryName2 = ownerDirectoryName;
		//			if( ownerDirectoryName2[ ownerDirectoryName2.Length - 1 ] != '\\' &&
		//				ownerDirectoryName2[ ownerDirectoryName2.Length - 1 ] != '/' )
		//			{
		//				ownerDirectoryName2 += "/";
		//			}

		//			string directoryName = NormalizePath( ownerDirectoryName2 );
		//			if( string.Compare( normalizedPath, 0, directoryName, 0, directoryName.Length, true ) == 0 )
		//				return Path.Combine( ".", normalizedPath.Substring( directoryName.Length ) );
		//		}
		//		else
		//			return Path.Combine( ".", normalizedPath );
		//	}
		//	return path;
		//}
	}
}
