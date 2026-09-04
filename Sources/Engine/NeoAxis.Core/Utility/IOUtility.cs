// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with files and folders.
	/// </summary>
	public static class IOUtility
	{
		//const int ErrorLockViolation = 33;
		//const int ErrorSharingViolation = 32;

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

		/// <summary>
		/// Determines whether the specified IOException is caused by a file being locked or in use by another process.
		/// </summary>
		/// <param name="exception">The IOException to check.</param>
		/// <returns>True if the exception is caused by a file being locked or in use by another process; otherwise, false.</returns>
		public static bool IsFileLockedException( IOException exception )
		{
			int hr = exception.HResult;

			if( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) )
			{
				int win32ErrorCode = hr & 0xFFFF;
				return win32ErrorCode == 0x0020 || win32ErrorCode == 0x0021;
			}
			else
			{
				// On Linux/macOS, POSIX error codes (errno) are mapped to the lower 16 bits of HResult
				int errno = hr & 0xFFFF;

				// Common POSIX codes for locked/busy files:
				// 11 (EAGAIN / EWOULDBLOCK) - Resource temporarily unavailable
				// 13 (EACCES) - Permission denied (often returned by fcntl locks)
				// 16 (EBUSY) - Device or resource busy
				return errno == 11 || errno == 13 || errno == 16;
			}
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
			catch( IOException e )
			{
				return IsFileLockedException( e );

				//int errorCode = e.HResult & 0xFFFF;
				//return errorCode == ErrorSharingViolation || errorCode == ErrorLockViolation;
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

		public static byte[] Unzip( byte[] data, long maxUncompressedSize = long.MaxValue )
		{
			using( var zippedStream = new MemoryStream( data ) )
			{
				using( var archive = new ZipArchive( zippedStream ) )
				{
					var entry = archive.Entries.FirstOrDefault();

					if( entry == null )
						throw new InvalidOperationException( "No entries in the zip archive." );
					if( entry.Length > maxUncompressedSize )
						throw new InvalidOperationException( "Uncompressed data size is too large." );

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

		public static void UnzipGetInfo( byte[] data, out long uncompressedSize )
		{
			using( var zippedStream = new MemoryStream( data ) )
			{
				using( var archive = new ZipArchive( zippedStream ) )
				{
					var entry = archive.Entries.FirstOrDefault();

					if( entry != null )
						uncompressedSize = entry.Length;
					else
						uncompressedSize = 0;
				}
			}
		}

		public static void DeleteEmptyDirectories( string targetDirectory, SearchOption searchOption, bool disableExceptions )
		{
			if( searchOption == SearchOption.AllDirectories )
			{
				if( Directory.Exists( targetDirectory ) )
				{
					try
					{
						foreach( string directory in Directory.GetDirectories( targetDirectory ) )
							DeleteEmptyDirectories( directory, SearchOption.AllDirectories, disableExceptions );
					}
					catch( Exception ) when( disableExceptions )
					{
					}

					try
					{
						if( Directory.GetFiles( targetDirectory ).Length == 0 && Directory.GetDirectories( targetDirectory ).Length == 0 )
						{
							try
							{
								Directory.Delete( targetDirectory );
							}
							catch( Exception ) when( disableExceptions )
							{
							}
						}
					}
					catch( Exception ) when( disableExceptions )
					{
					}
				}
			}
			else
			{
				if( Directory.Exists( targetDirectory ) )
				{
					try
					{
						foreach( string directory in Directory.GetDirectories( targetDirectory ) )
						{
							try
							{
								if( Directory.GetFiles( directory ).Length == 0 && Directory.GetDirectories( directory ).Length == 0 )
								{
									try
									{
										Directory.Delete( directory );
									}
									catch( Exception ) when( disableExceptions )
									{
									}
								}
							}
							catch( Exception ) when( disableExceptions )
							{
							}
						}
					}
					catch( Exception ) when( disableExceptions )
					{
					}
				}
			}
		}
	}
}
