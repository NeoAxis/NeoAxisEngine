// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with native memory.
	/// </summary>
	public static class NativeUtility
	{
		internal const string library = "NeoAxisCoreNative";
		internal const CallingConvention convention = CallingConvention.Cdecl;

		static bool nativeWrapperLibraryLoaded;

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

		internal static void LoadUtilsNativeWrapperLibrary()
		{
			if( !nativeWrapperLibraryLoaded )
			{
				nativeWrapperLibraryLoaded = true;
				NativeLibraryManager.PreLoadLibrary( library );
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

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void NativeUtils_CopyMemory( IntPtr destination, IntPtr source, int length );
		public static void CopyMemory( IntPtr destination, IntPtr source, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_CopyMemory( destination, source, length );
		}
		public unsafe static void CopyMemory( void* destination, void* source, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_CopyMemory( (IntPtr)destination, (IntPtr)source, length );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void NativeUtils_MoveMemory( IntPtr destination, IntPtr source, int length );
		public static void MoveMemory( IntPtr destination, IntPtr source, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_MoveMemory( destination, source, length );
		}
		public unsafe static void MoveMemory( void* destination, void* source, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_MoveMemory( (IntPtr)destination, (IntPtr)source, length );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int NativeUtils_CompareMemory( IntPtr buffer1, IntPtr buffer2, int length );
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
		static extern void NativeUtils_ZeroMemory( IntPtr buffer, int length );
		public static void ZeroMemory( IntPtr buffer, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_ZeroMemory( buffer, length );
		}
		public unsafe static void ZeroMemory( void* buffer, int length )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_ZeroMemory( (IntPtr)buffer, length );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void NativeUtils_FillMemory( IntPtr buffer, int length, byte value );
		public static void FillMemory( IntPtr buffer, int length, byte value )
		{
			LoadUtilsNativeWrapperLibrary();
			NativeUtils_FillMemory( buffer, length, value );
		}

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
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
	}
}
