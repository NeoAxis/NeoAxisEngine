// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	struct MyOgreVirtualArchiveFactory
	{
		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe delegate bool openDelegate( void*/*MyOgreVirtualDataStream*/ stream, [MarshalAs( UnmanagedType.LPWStr )] string fileName, ref int streamSize, [MarshalAs( UnmanagedType.U1 )] ref bool fileNotFound );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate void closeDelegate( void*/*MyOgreVirtualDataStream*/ stream );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate int readDelegate( void*/*MyOgreVirtualDataStream*/ stream,
			void* buf, int count );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate void skipDelegate( void*/*MyOgreVirtualDataStream*/ stream, int count );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate void seekDelegate( void*/*MyOgreVirtualDataStream*/ stream, int pos );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate int tellDelegate( void*/*MyOgreVirtualDataStream*/ stream );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe delegate bool findDelegate( [MarshalAs( UnmanagedType.LPWStr )] string pattern, [MarshalAs( UnmanagedType.U1 )] bool recursive, [MarshalAs( UnmanagedType.U1 )] bool dirs, void* userData );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe delegate bool findFileInfoDelegate( [MarshalAs( UnmanagedType.LPWStr )] string pattern, [MarshalAs( UnmanagedType.U1 )] bool recursive, [MarshalAs( UnmanagedType.U1 )] bool dirs, void* userData );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe delegate bool fileExistsDelegate( [MarshalAs( UnmanagedType.LPWStr )] string fileName );

		///////////////////////////////////////////

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreVirtualArchiveFactory_New", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void*/*MyOgreVirtualArchiveFactory*/ New( void* root, openDelegate open, closeDelegate close, readDelegate read, skipDelegate skip, seekDelegate seek, tellDelegate tell, findDelegate find, findFileInfoDelegate findFileInfo, fileExistsDelegate fileExists );

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreVirtualArchiveFactory_Delete", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void Delete( void*/*MyOgreVirtualArchiveFactory*/ _this );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreArchiveManager
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreArchiveManager_addArchiveFactory", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void addArchiveFactory( void*/*MyOgreVirtualArchiveFactory*/ factory );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	static class MyOgreVirtualFileSystem
	{
		static unsafe MyOgreVirtualArchiveFactory* virtualArchiveFactory;

		static MyOgreVirtualArchiveFactory.openDelegate _openDelegate;
		static MyOgreVirtualArchiveFactory.closeDelegate _closeDelegate;
		static MyOgreVirtualArchiveFactory.readDelegate _readDelegate;
		static MyOgreVirtualArchiveFactory.skipDelegate _skipDelegate;
		static MyOgreVirtualArchiveFactory.seekDelegate _seekDelegate;
		static MyOgreVirtualArchiveFactory.tellDelegate _tellDelegate;
		static MyOgreVirtualArchiveFactory.findDelegate _findDelegate;
		static MyOgreVirtualArchiveFactory.findFileInfoDelegate _findFileInfoDelegate;
		static MyOgreVirtualArchiveFactory.fileExistsDelegate _fileExistsDelegate;

		//key - MyOgreVirtualDataStream*
		static Dictionary<IntPtr, VirtualFileStream> openedStreams = new Dictionary<IntPtr, VirtualFileStream>();
		static unsafe void* lastOpenedOgreStream;
		static VirtualFileStream lastOpenedStream;

		//

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreVirtualFileSystem_findAddItem", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern void findAddItem( void* root, [MarshalAs( UnmanagedType.LPWStr )] string fileName, void* userData );

		[DllImport( OgreWrapper.library, EntryPoint = "MyOgreVirtualFileSystem_findFileInfoAddItem", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern void findFileInfoAddItem( void* root, [MarshalAs( UnmanagedType.LPWStr )] string fileName, [MarshalAs( UnmanagedType.LPWStr )] string path, [MarshalAs( UnmanagedType.LPWStr )] string baseName, int compressedSize, int uncompressedSize, void* userData );

		///////////////////////////////////////////

		unsafe public static void Init()
		{
			_openDelegate = open;
			_closeDelegate = close;
			_readDelegate = read;
			_skipDelegate = skip;
			_seekDelegate = seek;
			_tellDelegate = tell;
			_findDelegate = find;
			_findFileInfoDelegate = findFileInfo;
			_fileExistsDelegate = fileExists;

			virtualArchiveFactory = (MyOgreVirtualArchiveFactory*)MyOgreVirtualArchiveFactory.New( RenderingSystem.realRoot, _openDelegate, _closeDelegate, _readDelegate, _skipDelegate, _seekDelegate, _tellDelegate, _findDelegate, _findFileInfoDelegate, _fileExistsDelegate );
			OgreArchiveManager.addArchiveFactory( virtualArchiveFactory );

			OgreResourceGroupManager.addResourceLocation( RenderingSystem.realRoot, VirtualFileSystem.Directories.Assets, "VirtualFileSystem", true );
		}

		unsafe public static void Shutdown()
		{
			if( virtualArchiveFactory != null )
			{
				//crash
				//MyOgreVirtualArchiveFactory.Delete( virtualArchiveFactory );
				virtualArchiveFactory = null;
			}
		}

		unsafe static VirtualFileStream GetStreamByOgreStream( void*/*MyOgreVirtualDataStream*/ stream )
		{
			lock( openedStreams )
			{
				if( lastOpenedOgreStream == stream )
					return lastOpenedStream;

				var s = openedStreams[ (IntPtr)stream ];
				lastOpenedOgreStream = stream;
				lastOpenedStream = s;
				return s;
			}
		}

		//static double totalTime;
		//static double startTime;
		//static int openings;
		//static int calls;

		//static void BeginCounter()
		//{
		//   //xx xx;
		//   startTime = RendererWorld.renderTimerManager.GetSystemTime();
		//   calls++;
		//}

		//static void EndCounter()
		//{
		//   double t = RendererWorld.renderTimerManager.GetSystemTime() - startTime;
		//   totalTime += t;
		//}

		unsafe static bool open( void*/*MyOgreVirtualDataStream*/ stream, string fileName, ref int streamSize, ref bool fileNotFound )
		{
			//openings++;
			//Log.Info( "OPEN: " + fileName );

			streamSize = 0;
			fileNotFound = false;

			VirtualFileStream s;
			try
			{
				s = VirtualFile.Open( fileName );
			}
			catch( FileNotFoundException )
			{
				fileNotFound = true;
				return false;
			}
			catch
			{
				return false;
			}
			streamSize = (int)s.Length;

			lock( openedStreams )
			{
				openedStreams.Add( (IntPtr)stream, s );
				lastOpenedOgreStream = stream;
				lastOpenedStream = s;
			}

			return true;
		}

		unsafe static void close( void*/*MyOgreVirtualDataStream*/ stream )
		{
			var s = GetStreamByOgreStream( stream );

			s.Dispose();

			lock( openedStreams )
			{
				bool removed = openedStreams.Remove( (IntPtr)stream );
				if( lastOpenedOgreStream == stream )
				{
					lastOpenedOgreStream = null;
					lastOpenedStream = null;
				}
			}

			//Log.Info( "totalTime: {0}, openings: {1}, calls: {2}", totalTime, openings, calls );
		}

		unsafe static int read( void*/*MyOgreVirtualDataStream*/ stream, void* buf, int count )
		{
			//BeginCounter();
			//var s = GetStreamByOgreStream( stream );
			//int result = s.ReadUnmanaged( (IntPtr)buf, count );
			//EndCounter();
			//return result;

			var s = GetStreamByOgreStream( stream );
			return s.ReadUnmanaged( (IntPtr)buf, count );
		}

		unsafe static void skip( void*/*MyOgreVirtualDataStream*/ stream, int count )
		{
			//BeginCounter();

			var s = GetStreamByOgreStream( stream );
			s.Seek( count, SeekOrigin.Current );

			//EndCounter();
		}

		unsafe static void seek( void*/*MyOgreVirtualDataStream*/ stream, int pos )
		{
			//BeginCounter();

			var s = GetStreamByOgreStream( stream );
			s.Seek( pos, SeekOrigin.Begin );

			//EndCounter();
		}

		unsafe static int tell( void*/*MyOgreVirtualDataStream*/ stream )
		{
			//BeginCounter();
			//var s = GetStreamByOgreStream( stream );
			//int result = (int)s.Position;
			//EndCounter();
			//return result;

			var s = GetStreamByOgreStream( stream );
			return (int)s.Position;
		}

		unsafe static bool find( string pattern, bool recursive, bool dirs, void* userData )
		{
			try
			{
				string[] names;

				if( dirs )
					names = VirtualDirectory.GetDirectories( "", pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
				else
					names = VirtualDirectory.GetFiles( "", pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

				foreach( string name in names )
					findAddItem( RenderingSystem.realRoot, name, userData );
			}
			catch
			{
				return false;
			}
			return true;
		}

		unsafe static bool findFileInfo( string pattern, bool recursive, bool dirs, void* userData )
		{
			try
			{
				if( dirs )
				{
					var names = VirtualDirectory.GetDirectories( "", pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

					foreach( string name in names )
					{
						findFileInfoAddItem( RenderingSystem.realRoot, name, Path.GetDirectoryName( name ).Replace( '\\', '/' ) + "/", Path.GetFileName( name ), 0, 0, userData );
					}
				}
				else
				{
					var names = VirtualDirectory.GetFiles( "", pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

					foreach( string name in names )
					{
						int length = (int)VirtualFile.GetLength( name );
						findFileInfoAddItem( RenderingSystem.realRoot, name, Path.GetDirectoryName( name ).Replace( '\\', '/' ) + "/", Path.GetFileName( name ), length, length, userData );
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		static bool fileExists( string fileName )
		{
			return VirtualFile.Exists( fileName );
		}
	}
}
