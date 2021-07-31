// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	/////////////////////////////////////////////////////////////////////////////////////////////

	struct FreeType
	{
		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_Init", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern IntPtr/*FT_Library*/ Init();

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_Shutdown", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern void Shutdown( IntPtr/*FT_Library*/ library );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_CreateFace", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern IntPtr/*FT_Face*/ CreateFace( IntPtr/*FT_Library*/ library, IntPtr ttfData,
			int ttfDataSize );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_DestroyFace", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern void DestroyFace( IntPtr/*FT_Face*/ face );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_SetPixelSizes", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool SetPixelSizes( IntPtr/*FT_Face*/ face, int sizeX, int sizeY );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_IsGlyphExists", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool IsGlyphExists( IntPtr/*FT_Library*/ library, IntPtr/*FT_Face*/ face, int character );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_GetGlyphData", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool GetGlyphData( IntPtr/*FT_Library*/ library, IntPtr/*FT_Face*/ face, int character,
			int bufferSizeX, int bufferSizeY, IntPtr buffer, out int drawOffsetX,
			out int drawOffsetY, out int outSizeX, out int outSizeY, out int advance );

		[UnmanagedFunctionPointer( OgreWrapper.convention )]
		public unsafe delegate void GetGlyphContoursDelegate( int pointCount, IntPtr points );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_GetGlyphContours", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool GetGlyphContours( IntPtr/*FT_Library*/ library, IntPtr/*FT_Face*/ face, int character, GetGlyphContoursDelegate callback, out int drawOffsetX, out int drawOffsetY, out int advance );
	}
}
