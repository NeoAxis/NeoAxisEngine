// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.IO;
using Internal;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Xml.Schema;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with native memory and libraries.
	/// </summary>
	public static class NativeUtility
	{
		internal const string library = "libNeoAxisCoreNative";
		internal const CallingConvention convention = CallingConvention.Cdecl;

		static bool nativeWrapperLibraryLoaded;

		static object preLoadLibraryLockObject = new object();
		static Dictionary<string, IntPtr> loadedNativeLibraries = new Dictionary<string, IntPtr>();

		///////////////////////////////////////////

		[DllImport( library, EntryPoint = "MemoryManager_GetStatistics", CallingConvention = convention )]
		static extern void MemoryManager_GetStatistics( MemoryAllocationType allocationType, out long allocatedMemory, out int allocationCount );

		[DllImport( library, EntryPoint = "MemoryManager_GetCRTStatistics", CallingConvention = convention )]
		static extern void MemoryManager_GetCRTStatistics( out long allocatedMemory, out int allocationCount );

		[UnmanagedFunctionPointer( convention )]
		unsafe delegate void MemoryManager_GetAllocationInformationDelegate( MemoryAllocationType allocationType, int size, sbyte* fileName, int lineNumber, int allocationCount );

		[DllImport( library, EntryPoint = "MemoryManager_GetAllocationInformation", CallingConvention = convention )]
		static extern long MemoryManager_GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate callback );

		///////////////////////////////////////////

		[DllImport( "kernel32.dll", EntryPoint = "SetDllDirectory", CharSet = CharSet.Unicode )]
		static extern bool SetDllDirectory( string lpPathName );

		/////////////////////////////////////////

		public enum MemoryAllocationType
		{
			Renderer,
			Physics,
			SoundAndVideo,
			Utility,
			//Other,

			Count,
		}

		/////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal static void LoadUtilsNativeWrapperLibrary()
		{
			if( !nativeWrapperLibraryLoaded )
			{
				nativeWrapperLibraryLoaded = true;
				PreloadLibrary( library );
			}
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr NativeUtils_Alloc( MemoryAllocationType allocationType, int size );
		public static IntPtr Alloc( MemoryAllocationType allocationType, int size )
		{
			LoadUtilsNativeWrapperLibrary();
			return NativeUtils_Alloc( allocationType, size );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void NativeUtils_Free( IntPtr pointer );
		public static void Free( IntPtr pointer )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_Free( pointer );
		}
		public unsafe static void Free( void* pointer )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_Free( (IntPtr)pointer );
		}

		//

		//[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		//static extern void NativeUtils_CopyMemory( IntPtr destination, IntPtr source, int length );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe static void CopyMemory( void* destination, void* source, int length )
		{
			Buffer.MemoryCopy( source, destination, length, length );

			//if( MoveMemorySmall( destination, source, length ) )
			//	return;

			//LoadUtilsNativeWrapperLibrary();
			//NativeUtils_CopyMemory( (IntPtr)destination, (IntPtr)source, length );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void CopyMemory( IntPtr destination, IntPtr source, int length )
		{
			unsafe
			{
				CopyMemory( (void*)destination, (void*)source, length );
			}
		}

		//[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		//static extern void NativeUtils_MoveMemory( IntPtr destination, IntPtr source, int length );

		//public unsafe static void MoveMemory( void* destination, void* source, int length )
		//{
		//	Buffer.MemoryCopy( source, destination, length, length );

		//	//if( MoveMemorySmall( destination, source, length ) )
		//	//	return;

		//	//LoadUtilsNativeWrapperLibrary();
		//	//NativeUtils_MoveMemory( (IntPtr)destination, (IntPtr)source, length );
		//}

		//public static void MoveMemory( IntPtr destination, IntPtr source, int length )
		//{
		//	unsafe
		//	{
		//		MoveMemory( (void*)destination, (void*)source, length );
		//	}
		//}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern int NativeUtils_CompareMemory( IntPtr buffer1, IntPtr buffer2, int length );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static int CompareMemory( IntPtr buffer1, IntPtr buffer2, int length )
		{
			//#if !UWP
			unsafe
			{
				return new ReadOnlySpan<byte>( (void*)buffer1, length ).SequenceCompareTo( new ReadOnlySpan<byte>( (void*)buffer2, length ) );
			}
			//#else
			//			LoadUtilsNativeWrapperLibrary();
			//			return NativeUtils_CompareMemory( buffer1, buffer2, length );
			//#endif
		}

		public unsafe static int CompareMemory( void* buffer1, void* buffer2, int length )
		{
			//#if !UWP
			return new ReadOnlySpan<byte>( buffer1, length ).SequenceCompareTo( new ReadOnlySpan<byte>( buffer2, length ) );
			//#else
			//			LoadUtilsNativeWrapperLibrary();
			//			return NativeUtils_CompareMemory( (IntPtr)buffer1, (IntPtr)buffer2, length );
			//#endif
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern void NativeUtils_ZeroMemory( IntPtr buffer, int length );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void ZeroMemory( IntPtr buffer, int length )
		{
			//#if !UWP
			unsafe
			{
				if( buffer != IntPtr.Zero && length > 0 )
					new Span<byte>( (void*)buffer, length ).Clear();
			}
			//#else
			//			LoadUtilsNativeWrapperLibrary();
			//			NativeUtils_ZeroMemory( buffer, length );
			//#endif

			////#if !UWP && !ANDROID && !NETSTANDARD2_1
			////			unsafe
			////			{
			////				Unsafe.InitBlockUnaligned( (void*)buffer, 0, (uint)length );
			////			}
			////#else
			////			LoadUtilsNativeWrapperLibrary();
			////			NativeUtils_ZeroMemory( buffer, length );
			////#endif
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe static void ZeroMemory( void* buffer, int length )
		{
			//#if !UWP
			unsafe
			{
				if( buffer != null && length > 0 )
					new Span<byte>( buffer, length ).Clear();
			}
			//#else
			//			LoadUtilsNativeWrapperLibrary();
			//			NativeUtils_ZeroMemory( (IntPtr)buffer, length );
			//#endif

			////#if !UWP && !ANDROID && !NETSTANDARD2_1
			////			Unsafe.InitBlockUnaligned( buffer, 0, (uint)length );
			////#else
			////			LoadUtilsNativeWrapperLibrary();
			////			NativeUtils_ZeroMemory( (IntPtr)buffer, length );
			////#endif
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern void NativeUtils_FillMemory( IntPtr buffer, int length, byte value );

		public static void FillMemory( IntPtr buffer, int length, byte value )
		{
			//#if !UWP
			unsafe
			{
				if( buffer != IntPtr.Zero && length > 0 )
					new Span<byte>( (void*)buffer, length ).Fill( value );
			}
			//#else
			//			LoadUtilsNativeWrapperLibrary();
			//			NativeUtils_FillMemory( buffer, length, value );
			//#endif

			////#if !UWP && !ANDROID && !NETSTANDARD2_1
			////			unsafe
			////			{
			////				Unsafe.InitBlockUnaligned( (void*)buffer, value, (uint)length );
			////			}
			////#else
			////			LoadUtilsNativeWrapperLibrary();
			////			NativeUtils_FillMemory( buffer, length, value );
			////#endif
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern int NativeUtils_CalculateHash( IntPtr buffer, int length );
		public static int CalculateHash( IntPtr buffer, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			return NativeUtils_CalculateHash( buffer, length );
		}
		public unsafe static int CalculateHash( void* buffer, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			return NativeUtils_CalculateHash( (IntPtr)buffer, length );
		}

		///////////////////////////////////////////

		//for macOS, Linux
		static bool dllImportResolverInitialized;

		static IntPtr DllImportResolver( string libraryName, Assembly assembly, DllImportSearchPath? searchPath )
		{
			var handle = IntPtr.Zero;

			if( libraryName.Contains( "NeoAxisCoreNative" ) )
			{
				var path = "";
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "libNeoAxisCoreNative.dylib" );
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
					path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "libNeoAxisCoreNative.so" );

				NativeLibrary.TryLoad( path, out handle );
			}

			//!!!!other libs?

			////if( libraryName == "NeoAxisCoreNative" )
			////{
			////	var path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "lib" + libraryName + ".so" );
			////	TryLoadReflection( path, out libHandle );
			////	//NativeLibrary.TryLoad( path, out libHandle );
			////}

			return handle;
		}

		static void InitDllImportResolver()
		{
			if( !dllImportResolverInitialized )
			{
				dllImportResolverInitialized = true;
				NativeLibrary.SetDllImportResolver( typeof( NativeUtility ).Assembly, DllImportResolver );
			}
		}

		public static IntPtr PreloadLibrary( string baseName, string overrideSetCurrentDirectory = "", bool errorFatal = true )
		{
			lock( preLoadLibraryLockObject )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				{
					if( Path.GetExtension( baseName ) != ".dll" )
						baseName += ".dll";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
				{
					InitDllImportResolver();

					//remove ".dll"
					if( Path.GetExtension( baseName ) != ".dll" )
						baseName = Path.ChangeExtension( baseName, null );

					string checkPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, baseName + ".bundle" );
					if( Directory.Exists( checkPath ) )
						baseName += ".bundle";
					else
						baseName += ".dylib";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
				{
					InitDllImportResolver();
					//return IntPtr.Zero;

					//remove ".dll"
					if( Path.GetExtension( baseName ) != ".dll" )
						baseName = Path.ChangeExtension( baseName, null );

					baseName += ".so";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
				{
					//no preloading on Android
					return IntPtr.Zero;
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
				{
					//no preloading on iOS
					return IntPtr.Zero;
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
				{
					//no preloading on Web
					return IntPtr.Zero;
				}
				else
				{
					if( errorFatal )
						Log.Fatal( "NativeLibraryManager: PreloadLibrary: no code." );
					return IntPtr.Zero;
				}

				if( loadedNativeLibraries.TryGetValue( baseName, out var pointer2 ) )
					return pointer2;

				loadedNativeLibraries[ baseName ] = IntPtr.Zero;

				var saveCurrentDirectory = Directory.GetCurrentDirectory();
				var pointer = IntPtr.Zero;

				try
				{
					//set current directory to the platform specific directory. This is needed for loading dependent libraries.
					if( !string.IsNullOrEmpty( overrideSetCurrentDirectory ) )
						Directory.SetCurrentDirectory( overrideSetCurrentDirectory );
					else
						Directory.SetCurrentDirectory( VirtualFileSystem.Directories.PlatformSpecific );

					//set dll directory for dependent libraries
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
						try
						{
							SetDllDirectory( VirtualFileSystem.Directories.PlatformSpecific );
						}
						catch { }
					}


					var fullPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, baseName );

					//////in macOS the library must be in the same directory as the executable
					////if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					////	fullPath = Path.Combine( VirtualFileSystem.Directories.Binaries, baseName );


					//standard way to load library
					var errorMessage = "";
					try
					{
						pointer = NativeLibrary.Load( fullPath );
					}
					catch( Exception e )
					{
						errorMessage = e.Message;
					}

					////second way to load library, because first way doesn't work on macOS
					//if( pointer == IntPtr.Zero )
					//	pointer = PlatformSpecificUtility.Instance.LoadLibrary( fullPath );

					//can't load library
					if( pointer == IntPtr.Zero )
					{
						if( errorFatal )
						{
							var text = $"NativeLibraryManager: PreloadLibrary: Loading native library failed ({fullPath}).";
							////if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
							////	text += " Note: On macOS, the library must be in the same directory as the executable.";
							if( !string.IsNullOrEmpty( errorMessage ) )
								text += $" Exception: {errorMessage}";

							Log.Fatal( text );
						}
						return IntPtr.Zero;
					}

					loadedNativeLibraries[ baseName ] = pointer;
				}
				finally
				{
					Directory.SetCurrentDirectory( saveCurrentDirectory );
				}

				return pointer;
			}
		}

		///////////////////////////////////////////

		public static void GetStatistics( MemoryAllocationType allocationType, out long allocatedMemory, out int allocationCount )
		{
			LoadUtilsNativeWrapperLibrary();
			MemoryManager_GetStatistics( allocationType, out allocatedMemory, out allocationCount );
		}

		public static void GetCRTStatistics( out long allocatedMemory, out int allocationCount )
		{
			LoadUtilsNativeWrapperLibrary();
			MemoryManager_GetCRTStatistics( out allocatedMemory, out allocationCount );
		}

		static string allocationItemName;
		static List<string> allocations;
		//static ulong totalAllocationSize;

		unsafe static void EnumerateGetAllocationInformationCallback( MemoryAllocationType allocationType, int size, sbyte* fileName,
			int lineNumber, int allocationCount )
		{
			string fileInfo;
			if( fileName != null )
				fileInfo = string.Format( "{0}:{1}", new string( fileName ), lineNumber );
			else
				fileInfo = "NULL";

			allocations.Add( string.Format( "{0} - type: {1}, size: {2} x {3} = {4} (file: {5})", allocationItemName, allocationType,
				size, allocationCount, size * allocationCount, fileInfo ) );

			//totalAllocationSize += (ulong)( size * allocationCount );
		}

		internal static void LogStatistics( string allocationName )
		{
			LoadUtilsNativeWrapperLibrary();

			Log.InvisibleInfo( string.Format( "NativeMemoryManager: {0}s statistics begin", allocationName ) );

			allocationItemName = allocationName;
			allocations = new List<string>( 128 );
			long totalAllocationSize = 0;

			unsafe
			{
				totalAllocationSize = MemoryManager_GetAllocationInformation( EnumerateGetAllocationInformationCallback );
			}

			foreach( string leak in allocations )
				Log.InvisibleInfo( leak );

			Log.InvisibleInfo( "Total current allocation size: " + totalAllocationSize.ToString() + " bytes" );

			allocationItemName = null;
			allocations = null;
			totalAllocationSize = 0;

			Log.InvisibleInfo( string.Format( "NativeMemoryManager: {0}s statistics end", allocationName ) );
		}

		internal static void LogLeaks()
		{
			LogStatistics( "Leak" );
		}

		public static void LogAllocationStatistics()
		{
			LogStatistics( "Allocation" );
		}
	}
}
