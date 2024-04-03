// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.IO;
using Internal;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with native memory and libraries.
	/// </summary>
	public static class NativeUtility
	{
		internal const string library = "NeoAxisCoreNative";
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
			LoadUtilsNativeWrapperLibrary();
			return NativeUtils_CompareMemory( buffer1, buffer2, length );
		}
		public unsafe static int CompareMemory( void* buffer1, void* buffer2, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			return NativeUtils_CompareMemory( (IntPtr)buffer1, (IntPtr)buffer2, length );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern void NativeUtils_ZeroMemory( IntPtr buffer, int length );
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void ZeroMemory( IntPtr buffer, int length )
		{
#if !UWP && !ANDROID
			unsafe
			{
				Unsafe.InitBlockUnaligned( (void*)buffer, 0, (uint)length );
			}
#else
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_ZeroMemory( buffer, length );
#endif
		}
		public unsafe static void ZeroMemory( void* buffer, int length )
		{
#if !UWP && !ANDROID
			Unsafe.InitBlockUnaligned( buffer, 0, (uint)length );
#else
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_ZeroMemory( (IntPtr)buffer, length );
#endif
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		static extern void NativeUtils_FillMemory( IntPtr buffer, int length, byte value );
		public static void FillMemory( IntPtr buffer, int length, byte value )
		{
#if !UWP && !ANDROID
			unsafe
			{
				Unsafe.InitBlockUnaligned( (void*)buffer, value, (uint)length );
			}
#else
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_FillMemory( buffer, length, value );
#endif
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

		//Linux
#if !UWP && !ANDROID
		static bool dllImportResolverInitialized;

		static IntPtr DllImportResolver( string libraryName, Assembly assembly, DllImportSearchPath? searchPath )
		{
			IntPtr libHandle = IntPtr.Zero;

			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
			{
				//!!!!maybe rename libraries to remove adding "lib" prefix

				if( libraryName == "NeoAxisCoreNative" || libraryName == "bgfx" || libraryName == "shaderc" )
				{
					var path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "lib" + libraryName + ".so" );
					NativeLibrary.TryLoad( path, out libHandle );
				}
				else if( libraryName == "OpenAL32" )
				{
					var path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "libOpenAL.so" );
					NativeLibrary.TryLoad( path, out libHandle );
				}
			}
			return libHandle;

			//IntPtr libHandle = IntPtr.Zero;
			////you can add here different loading logic
			//if( libraryName == NativeLib && RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) && Environment.Is64BitProcess )
			//{
			//	NativeLibrary.TryLoad( "./runtimes/win-x64/native/somelib.dll", out libHandle );
			//}
			//else
			//if( libraryName == NativeLib )
			//{
			//	NativeLibrary.TryLoad( "libsomelibrary.so", assembly, DllImportSearchPath.ApplicationDirectory, out libHandle );
			//}
			//return libHandle;
		}

		static void InitDllImportResolver()
		{
			if( !dllImportResolverInitialized )
			{
				dllImportResolverInitialized = true;
				NativeLibrary.SetDllImportResolver( typeof( NativeUtility ).Assembly, DllImportResolver );
			}
		}
#endif

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
#if !UWP && !ANDROID
					InitDllImportResolver();
#endif

					//!!!!
					return IntPtr.Zero;

					////remove ".dll"
					//if( Path.GetExtension( baseName ) != ".dll" )
					//	baseName = Path.ChangeExtension( baseName, null );

					//baseName += ".so";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
				{
					//no preloading on Android
					return IntPtr.Zero;

					//if( Path.GetExtension( baseName ) != ".so" )
					//	baseName += ".so";

					//string prefix = "lib";
					//if( baseName.Length > 3 && baseName.Substring( 0, 3 ) == "lib" )
					//	prefix = "";
					//baseName = prefix + baseName + ".so";
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
				//if( loadedNativeLibraries.ContainsKey( baseName ) )
				//	return;

				loadedNativeLibraries[ baseName ] = IntPtr.Zero;

				string saveCurrentDirectory = Directory.GetCurrentDirectory();

				//Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, additionalPath )
				if( !string.IsNullOrEmpty( overrideSetCurrentDirectory ) )
					Directory.SetCurrentDirectory( overrideSetCurrentDirectory );
				else
					Directory.SetCurrentDirectory( VirtualFileSystem.Directories.PlatformSpecific );

				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				{
					try
					{
						SetDllDirectory( VirtualFileSystem.Directories.PlatformSpecific );
					}
					catch { }
				}

				string fullPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, baseName );

				var pointer = PlatformSpecificUtility.Instance.LoadLibrary( fullPath );
				if( pointer == IntPtr.Zero )
				{
					if( errorFatal )
						Log.Fatal( "NativeLibraryManager: PreloadLibrary: Loading native library failed ({0}).", fullPath );
					return IntPtr.Zero;
				}

				loadedNativeLibraries[ baseName ] = pointer;

				Directory.SetCurrentDirectory( saveCurrentDirectory );

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
