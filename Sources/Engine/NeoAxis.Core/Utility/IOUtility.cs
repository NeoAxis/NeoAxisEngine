// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with files and folders.
	/// </summary>
	public static class IOUtility
	{
		const int ErrorLockViolation = 33;
		const int ErrorSharingViolation = 32;

		public static bool IsDirectoryEmpty( string path )
		{
			return !Directory.EnumerateFileSystemEntries( path ).Any();
		}

		public static void CopyDirectory( string sourcePath, string destinationPath )
		{
			Directory.CreateDirectory( destinationPath );
			foreach( string dirPath in Directory.GetDirectories( sourcePath, "*", SearchOption.AllDirectories ) )
				Directory.CreateDirectory( dirPath.Replace( sourcePath, destinationPath ) );
			foreach( string newPath in Directory.GetFiles( sourcePath, "*.*", SearchOption.AllDirectories ) )
				File.Copy( newPath, newPath.Replace( sourcePath, destinationPath ), true );
		}

		public static void ClearDirectory( string path )
		{
			var info = new DirectoryInfo( path );
			foreach( var file in info.GetFiles() )
				file.Delete();
			foreach( var dir in info.GetDirectories() )
				dir.Delete( true );
		}

		public static bool IsFileLocked( string fileName )
		{
			Debug.Assert( !string.IsNullOrEmpty( fileName ) );

			try
			{
				if( File.Exists( fileName ) )
				{
					using( FileStream fs = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.None ) )
						fs.ReadByte();
				}

				return false;
			}
			catch( IOException ex )
			{
				int errorCode = ex.HResult & 0xFFFF;
				return errorCode == ErrorSharingViolation || errorCode == ErrorLockViolation;
			}
		}
	}
}

