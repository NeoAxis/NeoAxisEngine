#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis.Editor
{
	static class RepositoryUtility
	{
		public static bool GetAllFilesPathByReal( string cloudProjectFolder, string realPath, out string projectPath )
		{
			if( realPath == null )
				Log.Fatal( "RepositoryUtility: GetAllFilesPathByReal: realPath == null." );

			realPath = VirtualPathUtility.NormalizePath( realPath );
			//!!!!
			if( !Path.IsPathRooted( realPath ) )
				realPath = Path.Combine( VirtualFileSystem.Directories.Binaries, realPath );

			//project directory
			{
				string dir = cloudProjectFolder;//VirtualFileSystem.Directories.AllFiles;
				if( realPath.Length > dir.Length )
				{
					if( string.Equals( realPath, dir, StringComparison.OrdinalIgnoreCase ) )
					{
						projectPath = "";
						return true;
					}
					else if( string.Equals( realPath.Substring( 0, dir.Length ), dir, StringComparison.OrdinalIgnoreCase ) )
					{
						projectPath = realPath.Substring( dir.Length + 1, realPath.Length - dir.Length - 1 );
						return true;
					}
				}
			}

			projectPath = "";
			return false;
		}

		public static string GetAllFilesPathByReal( string cloudProjectFolder, string realPath )
		{
			GetAllFilesPathByReal( cloudProjectFolder, realPath, out var projectPath );
			return projectPath;
		}

		public static string GetRealPathByAllFiles( string cloudProjectFolder, string projectPath )
		{
			return Path.Combine( cloudProjectFolder, VirtualPathUtility.NormalizePath( projectPath ) );
		}
	}
}
#endif
#endif