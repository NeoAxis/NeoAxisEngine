// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

		//ZipArchive in .NET 6 now returns stream.Read not more 32kb per call
		public static int ReadGuaranteed( Stream stream, byte[] array )
		{
			var current = 0;
			while( current < array.Length )
			{
				var bytes = stream.Read( array, current, array.Length - current );
				if( bytes == 0 )
					break;
				current += bytes;
			}
			return current;
		}

		public static byte[] Zip( byte[] data, CompressionLevel compressionLevel )
		{
			using( var memoryStream = new MemoryStream( data.Length + 200 ) )
			{
				using( var zipArchive = new ZipArchive( memoryStream, ZipArchiveMode.Create, true ) )
				{
					var file = zipArchive.CreateEntry( "file", compressionLevel );
					using( var entryStream = file.Open() )
						entryStream.Write( data, 0, data.Length );
				}
				return memoryStream.ToArray();
			}
		}

		public static byte[] Unzip( byte[] data )
		{
			using( var zippedStream = new MemoryStream( data ) )
			{
				using( var archive = new ZipArchive( zippedStream ) )
				{
					var entry = archive.Entries.FirstOrDefault();
					using( var stream = entry.Open() )
					{
						var result = new byte[ entry.Length ];
						ReadGuaranteed( stream, result );
						return result;

						//it not work on .NET 6. Maximal size per call is 32kb
						//var result = new byte[ entry.Length ];
						//stream.Read( result, 0, result.Length );
						//return result;


						////using( var memoryStream = new MemoryStream( (int)entry.Length ) )
						////{
						////	stream.CopyTo( memoryStream );
						////	return memoryStream.ToArray();
						////}
					}
				}
			}
		}
	}
}

