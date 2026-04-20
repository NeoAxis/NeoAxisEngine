// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Defines a file for virtual file system.
	/// </summary>
	public static class VirtualFile
	{
		/// <summary>
		/// Determines whether the specified file exists. 
		/// </summary>
		/// <param name="path">The file to check.</param>
		/// <returns><b>true</b> if the file is exists; otherwise, <b>false</b>.</returns>
		public static bool Exists( string path )
		{
			//!!!!bool useCache/resetCache. можно же просто Dictionary сделать

			if( !VirtualFileSystem.initialized )
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualFile.Exists( \"{0}\" )", path );

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );
			if( File.Exists( realPath ) )
				return true;

			if( ArchiveManager.FileExists( path ) )
				return true;

			return false;
		}

		public static VirtualFileStream Open( string path )
		{
			if( !VirtualFileSystem.initialized )
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualFile.Open( \"{0}\" )", path );

			path = VirtualPathUtility.NormalizePath( path );

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );
			if( File.Exists( realPath ) )
			{
				VirtualFileStream stream = null;
				{
					//try
					//{

					//!!!!other platforms

					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
#if !ANDROID && !IOS && !WEB
						stream = new Win32HandleVirtualFileStream( realPath );
#endif
					}
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					//{
					//	//#if MACOS
					//	Log.Fatal( "new MacVirtualFileStream. impl." );
					//	//stream = new MacVirtualFileStream( realPath );
					//	//#endif
					//}
					else
						stream = new DefaultVirtualFileStream( realPath );

					//}
					//catch( FileNotFoundException )
					//{
					//}
					//catch( Exception e )
					//{
					//	throw e;
					//}

					//if( stream == null )
					//{
					//	stream = PackageManager.FileOpen( path );
					//	if( stream == null )
					//		throw new FileNotFoundException( "File not found.", path );
					//}
				}

				return stream;
			}

			{
				(var fileExists, var stream) = ArchiveManager.FileOpen( path );
				if( fileExists )
					return stream;
			}
			//if( ArchiveManager.FileExists( path ) )
			//	return ArchiveManager.FileOpen( path );

			throw new FileNotFoundException( "File not found.", path );
		}

		public static long GetLength( string path )
		{
			if( !VirtualFileSystem.initialized )
				Log.Fatal( "VirtualFileSystem: File system is not initialized." );

			if( VirtualFileSystem.LoggingFileOperations )
				Log.Info( "Logging file operations: VirtualFile.GetLength( \"{0}\" )", path );

			string realPath = VirtualPathUtility.GetRealPathByVirtual( path );
			if( File.Exists( realPath ) )
			{
				FileInfo fileInfo = new FileInfo( realPath );
				return fileInfo.Length;
			}

			{
				(var fileExists, var length) = ArchiveManager.FileGetLength( path );
				if( fileExists )
					return length;
			}
			//if( ArchiveManager.FileExists( path ) )
			//	return ArchiveManager.FileGetLength( path );

			throw new FileNotFoundException( "File not found.", path );
		}

		//!!!!
		//public static bool InsidePackage( string path )
		//{
		//	lock ( VirtualFileSystem.lockObject )
		//	{
		//		if( !VirtualFileSystem.initialized )
		//			Log.Fatal( "VirtualFileSystem: File system is not initialized." );

		//		if( VirtualFileSystem.LoggingFileOperations )
		//			Log.Info( "Logging file operations: VirtualFile.IsInArchive( \"{0}\" )", path );

		//		path = VirtualPathUtils.NormalizePath( path );

		//		path = VirtualFileSystem.GetRedirectedFileNameInternal( path, true );

		//		string realPath = VirtualPathUtils.GetRealPathByVirtual( path );
		//		if( File.Exists( realPath ) )
		//			return false;

		//		PackageManager.FileInfo fileInfo;
		//		return PackageManager.GetFileInfo( path, out fileInfo );
		//	}
		//}

		//!!!!!
		////!!!!!!remove?
		//public static bool IsPackage( string path )
		//{
		//	lock ( VirtualFileSystem.lockObject )
		//	{
		//		if( VirtualFileSystem.LoggingFileOperations )
		//			Log.Info( "Logging file operations: VirtualFile.IsArchive( \"{0}\" )", path );

		//		path = VirtualPathUtils.NormalizePath( path );

		//		path = VirtualFileSystem.GetRedirectedFileNameInternal( path, true );

		//		string realPath = VirtualPathUtils.GetRealPathByVirtual( path );
		//		return PackageManager.GetPackage( realPath ) != null;
		//	}
		//}

		public static byte[] ReadAllBytes( string path )
		{
			using( VirtualFileStream stream = Open( path ) )
			{
				byte[] result = new byte[ stream.Length ];
				if( stream.Read( result, 0, result.Length ) != result.Length )
				{
					throw new EndOfStreamException();
				}
				return result;
			}
		}

		public static string ReadAllText( string path, Encoding encoding )
		{
			using( var stream = Open( path ) )
			using( var streamReader = new StreamReader( stream, encoding ) )
				return streamReader.ReadToEnd();
		}

		public static string ReadAllText( string path )
		{
			return ReadAllText( path, Encoding.UTF8 );
		}

		public static string[] ReadAllLines( string path )
		{
			using( var stream = Open( path ) )
			{
				var list = new List<string>();
				using( var streamReader = new StreamReader( stream ) )
				{
					string item;
					while( ( item = streamReader.ReadLine() ) != null )
						list.Add( item );
				}
				return list.ToArray();
			}
		}

		//!!!!to VirtualFileUtils?
		/// <summary>
		/// Calculates the check sum of a file.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		public static uint CalculateChecksumCRC32( string path )
		{
			Stream stream = Open( path );
			CRC32 crc32 = new CRC32();
			byte[] checksum = crc32.ComputeHash( stream );
			Array.Reverse( checksum );
			uint value = BitConverter.ToUInt32( checksum, 0 );
			stream.Dispose();
			return value;
		}
	}
}
