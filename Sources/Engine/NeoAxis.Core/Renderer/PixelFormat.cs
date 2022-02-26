// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	/// <summary>The pixel format used for images, textures, and render surfaces.</summary>
	public enum PixelFormat
	{
		/// <summary>Unknown pixel format.</summary>
		Unknown = 0,//Ogre::PF_UNKNOWN,

		/// <summary>8-bit pixel format, all bits luminance.</summary>
		L8 = 1,//Ogre::PF_L8,

		///// <summary>8-bit pixel format, all bits luminance.</summary>
		//ByteL = L8,

		/// <summary>16-bit pixel format, all bits luminace.</summary>
		L16 = 2,//Ogre::PF_L16,

		///// <summary>16-bit pixel format, all bits luminace.</summary>
		//ShortL = L16,

		/// <summary>8-bit pixel format, all bits alpha.</summary>
		A8 = 3,//Ogre::PF_A8,

		///// <summary>8-bit pixel format, all bits alpha.</summary>
		//ByteA = A8,

		/// <summary>8-bit pixel format, 4 bits alpha, 4 bits luminace.</summary>
		A4L4 = 4,//Ogre::PF_A4L4,

		/// <summary>2 byte pixel format, 1 byte luminance, 1 byte alpha.</summary>
		ByteLA = 5,//Ogre::PF_BYTE_LA,

		/// <summary>16-bit pixel format, 5 bits red, 6 bits green, 5 bits blue.</summary>
		R5G6B5 = 6,//Ogre::PF_R5G6B5,

		/// <summary>16-bit pixel format, 5 bits red, 6 bits green, 5 bits blue.</summary>
		B5G6R5 = 7,//Ogre::PF_B5G6R5,

		/// <summary>8-bit pixel format, 2 bits blue, 3 bits green, 3 bits red.</summary>
		R3G3B2 = 31,//Ogre::PF_R3G3B2,

		/// <summary>16-bit pixel format, 4 bits for alpha, red, green and blue.</summary>
		A4R4G4B4 = 8,//Ogre::PF_A4R4G4B4,

		/// <summary>16-bit pixel format, 5 bits for blue, green, red and 1 for alpha.</summary>
		A1R5G5B5 = 9,//Ogre::PF_A1R5G5B5,

		/// <summary>24-bit pixel format, 8 bits for red, green and blue.</summary>
		R8G8B8 = 10,//Ogre::PF_R8G8B8,

		/// <summary>24-bit pixel format, 8 bits for blue, green and red.</summary>
		B8G8R8 = 11,//Ogre::PF_B8G8R8,

		/// <summary>32-bit pixel format, 8 bits for alpha, red, green and blue.</summary>
		A8R8G8B8 = 12,//Ogre::PF_A8R8G8B8,

		/// <summary>32-bit pixel format, 8 bits for blue, green, red and alpha.</summary>
		A8B8G8R8 = 13,//Ogre::PF_A8B8G8R8,

		/// <summary>32-bit pixel format, 8 bits for blue, green, red and alpha.</summary>
		B8G8R8A8 = 14,//Ogre::PF_B8G8R8A8,

		/// <summary>32-bit pixel format, 8 bits for red, green, blue and alpha.</summary>
		R8G8B8A8 = 28,//Ogre::PF_R8G8B8A8,

		/// <summary>
		/// 32-bit pixel format, 8 bits for red, 8 bits for green, 8 bits for blue
		/// like A8R8G8B8, but alpha will get discarded.
		/// </summary>
		/// 
		X8R8G8B8 = 26,//Ogre::PF_X8R8G8B8,

		/// <summary>
		/// 32-bit pixel format, 8 bits for blue, 8 bits for green, 8 bits for red
		/// like A8B8G8R8, but alpha will get discarded.
		/// </summary>
		/// 
		X8B8G8R8 = 27,//Ogre::PF_X8B8G8R8,

		/*

		#if OGRE_ENDIAN == OGRE_ENDIAN_BIG
		/// 3 byte pixel format, 1 byte for red, 1 byte for green, 1 byte for blue
		ByteRGB = R8G8B8,
		/// 3 byte pixel format, 1 byte for blue, 1 byte for green, 1 byte for red
		ByteBGR = B8G8R8,
		/// 4 byte pixel format, 1 byte for blue, 1 byte for green, 1 byte for red and one byte for alpha
		ByteBGRA = B8G8R8A8,
		/// 4 byte pixel format, 1 byte for red, 1 byte for green, 1 byte for blue, and one byte for alpha
		ByteRGBA = R8G8B8A8,
		#else
		/// 3 byte pixel format, 1 byte for red, 1 byte for green, 1 byte for blue
		ByteRGB = B8G8R8,
		/// 3 byte pixel format, 1 byte for blue, 1 byte for green, 1 byte for red
		ByteBGR = R8G8B8,
		/// 4 byte pixel format, 1 byte for blue, 1 byte for green, 1 byte for red and one byte for alpha
		ByteBGRA = A8R8G8B8,
		/// 4 byte pixel format, 1 byte for red, 1 byte for green, 1 byte for blue, and one byte for alpha
		ByteRGBA = A8B8G8R8,
		#endif

		*/


		/// <summary>32-bit pixel format, 2 bits for alpha, 10 bits for red, green and blue.</summary>
		A2R10G10B10 = 15,//Ogre::PF_A2R10G10B10,

		/// <summary>32-bit pixel format, 10 bits for blue, green and red, 2 bits for alpha.</summary>
		A2B10G10R10 = 16,//Ogre::PF_A2B10G10R10,

		/// <summary>DDS (DirectDraw Surface) DXT1 format.</summary>
		DXT1 = 17,//Ogre::PF_DXT1,

		/// <summary>DDS (DirectDraw Surface) DXT2 format.</summary>
		DXT2 = 18,//Ogre::PF_DXT2,

		/// <summary>DDS (DirectDraw Surface) DXT3 format.</summary>
		DXT3 = 19,//Ogre::PF_DXT3,

		/// <summary>DDS (DirectDraw Surface) DXT4 format.</summary>
		DXT4 = 20,//Ogre::PF_DXT4,

		/// <summary>DDS (DirectDraw Surface) DXT5 format.</summary>
		DXT5 = 21,//Ogre::PF_DXT5,

		/// <summary>16-bit pixel format, 16 bits (float) for red.</summary>
		Float16R = 32,//Ogre::PF_FLOAT16_R,

		/// <summary>48-bit pixel format, 16 bits (float) for red, 16 bits (float) for green, 16 bits (float) for blue.</summary>
		Float16RGB = 22,//Ogre::PF_FLOAT16_RGB,

		/// <summary>64-bit pixel format, 16 bits (float) for red, 16 bits (float) for green, 16 bits (float) for blue, 16 bits (float) for alpha.</summary>
		Float16RGBA = 23,//Ogre::PF_FLOAT16_RGBA,

		/// <summary>32-bit pixel format, 32 bits (float) for red.</summary>
		Float32R = 33,//Ogre::PF_FLOAT32_R,

		/// <summary>96-bit pixel format, 32 bits (float) for red, 32 bits (float) for green, 32 bits (float) for blue.</summary>
		Float32RGB = 24,//Ogre::PF_FLOAT32_RGB,

		/// <summary>128-bit pixel format, 32 bits (float) for red, 32 bits (float) for green, 32 bits (float) for blue, 32 bits (float) for alpha.</summary>
		Float32RGBA = 25,//Ogre::PF_FLOAT32_RGBA,

		/// <summary>Depth texture format.</summary>
		Depth24S8 = 29,//!!!!Depth24 = 29,//Ogre::PF_DEPTH24,

		/// <summary>32-bit, 2-channel s10e5 floating point pixel format, 16-bit green, 16-bit red.</summary>
		Float16GR = 35,//Ogre::PF_FLOAT16_GR,

		/// <summary>64-bit, 2-channel floating point pixel format, 32-bit green, 32-bit red.</summary>
		Float32GR = 36,//Ogre::PF_FLOAT32_GR,

		/// <summary>64-bit pixel format, 16 bits for red, green, blue and alpha.</summary>
		ShortRGBA = 30,//Ogre::PF_SHORT_RGBA,

		/// <summary>32-bit pixel format, 16-bit green, 16-bit red.</summary>
		ShortGR = 34,//Ogre::PF_SHORT_GR,

		/// <summary>48-bit pixel format, 16 bits for red, green and blue.</summary>
		ShortRGB = 37,//Ogre::PF_SHORT_RGB,

		/// <summary>PVRTC (PowerVR) RGB 2 bpp</summary>
		PVRTC_RGB2 = 38,//Ogre::PF_PVRTC_RGB2,

		/// <summary>PVRTC (PowerVR) RGBA 2 bpp</summary>
		PVRTC_RGBA2 = 39,//Ogre::PF_PVRTC_RGBA2,

		/// <summary>PVRTC (PowerVR) RGB 4 bpp</summary>
		PVRTC_RGB4 = 40,//Ogre::PF_PVRTC_RGB4,

		/// <summary>PVRTC (PowerVR) RGBA 4 bpp</summary>
		PVRTC_RGBA4 = 41,//Ogre::PF_PVRTC_RGBA4,

		/// PVRTC (PowerVR) Version 2, 2 bpp
		PVRTC2_2BPP = 42,
		/// PVRTC (PowerVR) Version 2, 4 bpp
		PVRTC2_4BPP = 43,
		/// 32-bit pixel format, 11 bits (float) for red, 11 bits (float) for green, 10 bits (float) for blue
		R11G11B10_Float = 44,
		/// 8-bit pixel format, 8 bits red (unsigned int).
		R8_UInt = 45,
		/// 16-bit pixel format, 8 bits red (unsigned int), 8 bits blue (unsigned int).
		R8G8_UInt = 46,
		/// 24-bit pixel format, 8 bits red (unsigned int), 8 bits blue (unsigned int), 8 bits green (unsigned int).
		R8G8B8_UInt = 47,
		/// 32-bit pixel format, 8 bits red (unsigned int), 8 bits blue (unsigned int), 8 bits green (unsigned int), 8 bits alpha (unsigned int).
		R8G8B8A8_UInt = 48,
		/// 16-bit pixel format, 16 bits red (unsigned int).
		R16_UInt = 49,
		/// 32-bit pixel format, 16 bits red (unsigned int), 16 bits blue (unsigned int).
		R16G16_UInt = 50,
		/// 48-bit pixel format, 16 bits red (unsigned int), 16 bits blue (unsigned int), 16 bits green (unsigned int).
		R16G16B16_UInt = 51,
		/// 64-bit pixel format, 16 bits red (unsigned int), 16 bits blue (unsigned int), 16 bits green (unsigned int), 16 bits alpha (unsigned int).
		R16G16B16A16_UInt = 52,
		/// 32-bit pixel format, 32 bits red (unsigned int).
		R32_UInt = 53,
		/// 64-bit pixel format, 32 bits red (unsigned int), 32 bits blue (unsigned int).
		R32G32_UInt = 54,
		/// 96-bit pixel format, 32 bits red (unsigned int), 32 bits blue (unsigned int), 32 bits green (unsigned int).
		R32G32B32_UInt = 55,
		/// 128-bit pixel format, 32 bits red (unsigned int), 32 bits blue (unsigned int), 32 bits green (unsigned int), 32 bits alpha (unsigned int).
		R32G32B32A32_UInt = 56,
		/// 8-bit pixel format, 8 bits red (signed int).
		R8_SInt = 57,
		/// 16-bit pixel format, 8 bits red (signed int), 8 bits blue (signed int).
		R8G8_SInt = 58,
		/// 24-bit pixel format, 8 bits red (signed int), 8 bits blue (signed int), 8 bits green (signed int).
		R8G8B8_SInt = 59,
		/// 32-bit pixel format, 8 bits red (signed int), 8 bits blue (signed int), 8 bits green (signed int), 8 bits alpha (signed int).
		R8G8B8A8_SInt = 60,
		/// 16-bit pixel format, 16 bits red (signed int).
		R16_SInt = 61,
		/// 32-bit pixel format, 16 bits red (signed int), 16 bits blue (signed int).
		R16G16_SInt = 62,
		/// 48-bit pixel format, 16 bits red (signed int), 16 bits blue (signed int), 16 bits green (signed int).
		R16G16B16_SInt = 63,
		/// 64-bit pixel format, 16 bits red (signed int), 16 bits blue (signed int), 16 bits green (signed int), 16 bits alpha (signed int).
		R16G16B16A16_SInt = 64,
		/// 32-bit pixel format, 32 bits red (signed int).
		R32_SInt = 65,
		/// 64-bit pixel format, 32 bits red (signed int), 32 bits blue (signed int).
		R32G32_SInt = 66,
		/// 96-bit pixel format, 32 bits red (signed int), 32 bits blue (signed int), 32 bits green (signed int).
		R32G32B32_SInt = 67,
		/// 128-bit pixel format, 32 bits red (signed int), 32 bits blue (signed int), 32 bits green (signed int), 32 bits alpha (signed int).
		R32G32B32A32_SInt = 68,
		/// 32-bit pixel format, 9 bits for blue, green, red plus a 5 bit exponent.
		R9G9B9E5_ShareExp = 69,
		/// DDS (DirectDraw Surface) BC4 format (unsigned normalised)
		BC4_UNorm = 70,
		/// DDS (DirectDraw Surface) BC4 format (signed normalised)
		BC4_SNorm = 71,
		/// DDS (DirectDraw Surface) BC5 format (unsigned normalised)
		BC5_UNorm = 72,
		/// DDS (DirectDraw Surface) BC5 format (signed normalised)
		BC5_SNorm = 73,
		/// DDS (DirectDraw Surface) BC6H format (unsigned 16 bit float)
		BC6H_UF16 = 74,
		/// DDS (DirectDraw Surface) BC6H format (signed 16 bit float)
		BC6H_SF16 = 75,
		/// DDS (DirectDraw Surface) BC7 format (unsigned normalised)
		BC7_UNorm = 76,
		/// DDS (DirectDraw Surface) BC7 format (unsigned normalised sRGB)
		BC7_UNorm_SRGB = 77,
		/// 8-bit pixel format, all bits red.
		R8 = 78,
		/// 16-bit pixel format, 8 bits red, 8 bits green.
		RG8 = 79,
		/// 8-bit pixel format, 8 bits red (signed normalised int).
		R8_SNorm = 80,
		/// 16-bit pixel format, 8 bits red (signed normalised int), 8 bits blue (signed normalised int).
		R8G8_SNorm = 81,
		/// 24-bit pixel format, 8 bits red (signed normalised int), 8 bits blue (signed normalised int), 8 bits green (signed normalised int).
		R8G8B8_SNorm = 82,
		/// 32-bit pixel format, 8 bits red (signed normalised int), 8 bits blue (signed normalised int), 8 bits green (signed normalised int), 8 bits alpha (signed normalised int).
		R8G8B8A8_SNorm = 83,
		/// 16-bit pixel format, 16 bits red (signed normalised int).
		R16_SNorm = 84,
		/// 32-bit pixel format, 16 bits red (signed normalised int), 16 bits blue (signed normalised int).
		R16G16_SNorm = 85,
		/// 48-bit pixel format, 16 bits red (signed normalised int), 16 bits blue (signed normalised int), 16 bits green (signed normalised int).
		R16G16B16_SNorm = 86,
		/// 64-bit pixel format, 16 bits red (signed normalised int), 16 bits blue (signed normalised int), 16 bits green (signed normalised int), 16 bits alpha (signed normalised int).
		R16G16B16A16_SNorm = 87,
		/// ETC1 (Ericsson Texture Compression)
		ETC1_RGB8 = 88,
		/// ETC2 (Ericsson Texture Compression)
		ETC2_RGB8 = 89,
		/// ETC2 (Ericsson Texture Compression)
		ETC2_RGBA8 = 90,
		/// ETC2 (Ericsson Texture Compression)
		ETC2_RGB8A1 = 91,
		/// ATC (AMD_compressed_ATC_texture)
		ATC_RGB = 92,
		/// ATC (AMD_compressed_ATC_texture)
		ATC_RGBA_ExplicitAlpha = 93,
		/// ATC (AMD_compressed_ATC_texture)
		ATC_RGBA_InterpolatedAlpha = 94,

		//!!!!delete
		/// 3Dc pixel format.
		_3DC = 95,

		//// Number of pixel formats currently defined
		//Count = 96
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgrePixelUtil
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_getMemorySize", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern int getMemorySize( int width, int height, int depth, PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_getNumElemBytes", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern int getNumElemBytes( PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_hasAlpha", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool hasAlpha( PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_isFloatingPoint", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool isFloatingPoint( PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_isCompressed", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool isCompressed( PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_isDepth", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool isDepth( PixelFormat format );

		[DllImport( OgreWrapper.library, EntryPoint = "OgrePixelUtil_unpackColour", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern void unpackColour( out float r, out float g, out float b, out float a,
			PixelFormat pf, IntPtr src );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Auxiliary class for working with the <see cref="PixelFormat"/>.
	/// </summary>
	public static class PixelFormatUtility
	{
		public static int GetMemorySize( int width, int height, int depth, PixelFormat format )
		{
			return OgrePixelUtil.getMemorySize( width, height, depth, format );
		}

		public static int GetNumElemBytes( PixelFormat format )
		{
			return OgrePixelUtil.getNumElemBytes( format );
		}

		public static bool HasAlpha( PixelFormat format )
		{
			return OgrePixelUtil.hasAlpha( format );
		}

		public static bool IsFloatingPoint( PixelFormat format )
		{
			return OgrePixelUtil.isFloatingPoint( format );
		}

		public static bool IsCompressed( PixelFormat format )
		{
			return OgrePixelUtil.isCompressed( format );
		}

		public static bool IsDepth( PixelFormat format )
		{
			return OgrePixelUtil.isDepth( format );
		}

		//public static ColorValue UnpackColor( PixelFormat format, IntPtr source )
		//{
		//	float r = 0, g = 0, b = 0, a = 1;
		//	OgrePixelUtil.unpackColour( out r, out g, out b, out a, format, source );
		//	return new ColorValue( r, g, b, a );
		//}
	}
}
