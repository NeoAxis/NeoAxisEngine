// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Replacement for Path class to support case insensitive paths.
	/// </summary>
	public static class PathUtility
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

		static bool Fix
		{
			get { return SystemSettings.CurrentPlatform == SystemSettings.Platform.Android; }
		}

		public static string GetFileNameWithoutExtension( string path )
		{
			if( Fix )
				return Path.GetFileNameWithoutExtension( NormalizePath( path ) );
			else
				return Path.GetFileNameWithoutExtension( path );
		}

		public static string GetFileName( string path )
		{
			if( Fix )
				return Path.GetFileName( NormalizePath( path ) );
			else
				return Path.GetFileName( path );
		}

		public static string GetDirectoryName( string path )
		{
			if( Fix )
				return Path.GetDirectoryName( NormalizePath( path ) );
			else
				return Path.GetDirectoryName( path );
		}

		public static string Combine( string path1, string path2 )
		{
			if( Fix )
				return Path.Combine( NormalizePath( path1 ), NormalizePath( path2 ) );
			else
				return Path.Combine( path1, path2 );
		}

		public static string Combine( string path1, string path2, string path3 )
		{
			if( Fix )
				return Path.Combine( NormalizePath( path1 ), NormalizePath( path2 ), NormalizePath( path3 ) );
			else
				return Path.Combine( path1, path2, path3 );
		}

		public static string Combine( string path1, string path2, string path3, string path4 )
		{
			if( Fix )
				return Path.Combine( NormalizePath( path1 ), NormalizePath( path2 ), NormalizePath( path3 ), NormalizePath( path4 ) );
			else
				return Path.Combine( path1, path2, path3, path4 );
		}

		public static string Combine( params string[] paths )
		{
			if( Fix )
			{
				var paths2 = new string[ paths.Length ];
				for( int n = 0; n < paths.Length; n++ )
					paths2[ n ] = NormalizePath( paths[ n ] );
				return Path.Combine( paths2 );
			}
			else
				return Path.Combine( paths );
		}
	}
}
