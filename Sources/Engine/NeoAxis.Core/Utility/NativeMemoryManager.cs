// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with native memory. Used to get statistics on the use of native memory.
	/// </summary>
	public static class NativeMemoryManager
	{
		struct Wrapper
		{
			public const string library = "NeoAxisCoreNative";
			public const CallingConvention convention = CallingConvention.Cdecl;
		}

		///////////////////////////////////////////

		[DllImport( Wrapper.library, EntryPoint = "MemoryManager_GetStatistics", CallingConvention = Wrapper.convention )]
		static extern void MemoryManager_GetStatistics( NativeUtility.MemoryAllocationType allocationType, out long allocatedMemory, out int allocationCount );

		[DllImport( Wrapper.library, EntryPoint = "MemoryManager_GetCRTStatistics", CallingConvention = Wrapper.convention )]
		static extern void MemoryManager_GetCRTStatistics( out long allocatedMemory, out int allocationCount );

		[UnmanagedFunctionPointer( Wrapper.convention )]
		unsafe delegate void MemoryManager_GetAllocationInformationDelegate( NativeUtility.MemoryAllocationType allocationType, int size, sbyte* fileName, int lineNumber, int allocationCount );

		[DllImport( Wrapper.library, EntryPoint = "MemoryManager_GetAllocationInformation", CallingConvention = Wrapper.convention )]
		static extern void MemoryManager_GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate callback );

		///////////////////////////////////////////

		static bool utilsNativeWrapperLibraryLoaded;

		static void LoadUtilsNativeWrapperLibrary()
		{
			if( !utilsNativeWrapperLibraryLoaded )
			{
				utilsNativeWrapperLibraryLoaded = true;
				NativeLibraryManager.PreLoadLibrary( "NeoAxisCoreNative" );
			}
		}

		///////////////////////////////////////////

		public static void GetStatistics( NativeUtility.MemoryAllocationType allocationType, out long allocatedMemory, out int allocationCount )
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
		static ulong totalAllocationSize;

		unsafe static void EnumerateGetAllocationInformationCallback( NativeUtility.MemoryAllocationType allocationType, int size, sbyte* fileName,
			int lineNumber, int allocationCount )
		{
			string fileInfo;
			if( fileName != null )
				fileInfo = string.Format( "{0}:{1}", new string( fileName ), lineNumber );
			else
				fileInfo = "NULL";

			allocations.Add( string.Format( "{0} - type: {1}, size: {2} x {3} = {4} (file: {5})", allocationItemName, allocationType,
				size, allocationCount, size * allocationCount, fileInfo ) );

			totalAllocationSize += (ulong)( size * allocationCount );
		}

		internal static void LogStatistics( string allocationName )
		{
			LoadUtilsNativeWrapperLibrary();

			Log.InvisibleInfo( string.Format( "NativeMemoryManager: {0}s statistics begin",
				allocationName ) );

			allocationItemName = allocationName;
			allocations = new List<string>( 128 );
			totalAllocationSize = 0;

			unsafe
			{
				MemoryManager_GetAllocationInformation( EnumerateGetAllocationInformationCallback );
			}

			foreach( string leak in allocations )
				Log.InvisibleInfo( leak );

			Log.InvisibleInfo( "Total size: " + totalAllocationSize.ToString() );

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
