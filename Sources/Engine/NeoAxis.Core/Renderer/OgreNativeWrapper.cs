// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	struct OgreWrapper
	{
		public const string library = "NeoAxisCoreNative";
		public const CallingConvention convention = CallingConvention.Cdecl;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreNativeWrapper
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_CheckNativeBridge", CallingConvention = OgreWrapper.convention )]
#if WEB
//!!!!
		[DefaultDllImportSearchPaths( DllImportSearchPath.SafeDirectories )]
#endif
		public unsafe static extern void CheckNativeBridge( int parameterTypeTextureCubeValue );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_FreeOutString", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern void FreeOutString( IntPtr pointer );


		public static string GetOutString( IntPtr pointer, bool free = true )
		{
			if( pointer != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringUni( pointer );
				if( free )
					FreeOutString( pointer );
				return result;
			}
			else
				return null;
		}


		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_GetGlobalParameter", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern IntPtr GetGlobalParameterNative( [MarshalAs( UnmanagedType.LPStr )] string name );
		//IntPtr parameter1, IntPtr parameter2, IntPtr parameter3, IntPtr parameter4 );

		public static string GetGlobalParameter( string name )
		{
			return GetOutString( GetGlobalParameterNative( name ) );
		}
	}
}
