// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		public unsafe static extern void CheckNativeBridge( int parameterTypeTextureCubeValue );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_FreeOutString", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern void FreeOutString( IntPtr pointer );

		public static string GetOutString( IntPtr pointer )
		{
			if( pointer != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringUni( pointer );
				FreeOutString( pointer );
				return result;
			}
			else
				return null;
		}
	}
}
