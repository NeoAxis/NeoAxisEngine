// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Internal;

namespace NeoAxis
{
	static class DDSWriter
	{
		static class Utility
		{
			public static void WriteUInt32( Stream stream, UInt32 uint32 )
			{
				stream.WriteByte( (byte)( uint32 & 0xff ) );
				stream.WriteByte( (byte)( ( uint32 >> 8 ) & 0xff ) );
				stream.WriteByte( (byte)( ( uint32 >> 16 ) & 0xff ) );
				stream.WriteByte( (byte)( ( uint32 >> 24 ) & 0xff ) );
			}
		}

		//////////////////////////////////////////////

		enum DdsFileFormat
		{
			DDS_FORMAT_DXT1,
			DDS_FORMAT_DXT3,
			DDS_FORMAT_DXT5,
			DDS_FORMAT_3DC,
			DDS_FORMAT_A8R8G8B8,
			DDS_FORMAT_X8R8G8B8,
			DDS_FORMAT_A8B8G8R8,
			DDS_FORMAT_X8B8G8R8,
			DDS_FORMAT_A1R5G5B5,
			DDS_FORMAT_A4R4G4B4,
			DDS_FORMAT_R8G8B8,
			DDS_FORMAT_R5G6B5,

			DDS_FORMAT_INVALID,
		}

		//////////////////////////////////////////////

		class DdsPixelFormat
		{
			public enum PixelFormatFlags
			{
				DDS_FOURCC = 0x00000004,
				DDS_RGB = 0x00000040,
				DDS_RGBA = 0x00000041,
			}

			public uint m_size;
			public uint m_flags;
			public uint m_fourCC;
			public uint m_rgbBitCount;
			public uint m_rBitMask;
			public uint m_gBitMask;
			public uint m_bBitMask;
			public uint m_aBitMask;

			public uint Size()
			{
				return 8 * 4;
			}

			public void Initialise( DDSTextureTools.DDSImage.FormatEnum format )
			{
				m_size = Size();
				switch( format )
				{
				case DDSTextureTools.DDSImage.FormatEnum.DXT1:
				case DDSTextureTools.DDSImage.FormatEnum.DXT3:
				case DDSTextureTools.DDSImage.FormatEnum.DXT5:
				case DDSTextureTools.DDSImage.FormatEnum.BC5:
					{
						// DXT1/DXT3/DXT5/3Dc
						m_flags = (int)PixelFormatFlags.DDS_FOURCC;
						m_rgbBitCount = 0;
						m_rBitMask = 0;
						m_gBitMask = 0;
						m_bBitMask = 0;
						m_aBitMask = 0;
						if( format == DDSTextureTools.DDSImage.FormatEnum.DXT1 ) m_fourCC = 0x31545844;
						if( format == DDSTextureTools.DDSImage.FormatEnum.DXT3 ) m_fourCC = 0x33545844;
						if( format == DDSTextureTools.DDSImage.FormatEnum.DXT5 ) m_fourCC = 0x35545844;
						if( format == DDSTextureTools.DDSImage.FormatEnum.BC5 ) m_fourCC = 0x32495441;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.R16G16B16A16:
					{
						m_flags = (int)PixelFormatFlags.DDS_FOURCC;
						m_rgbBitCount = 64;
						m_fourCC = 113;
						m_rBitMask = 0;
						m_gBitMask = 0;
						m_bBitMask = 0;
						m_aBitMask = 0;
						//m_rBitMask = 0x00ff0000;
						//m_gBitMask = 0x0000ff00;
						//m_bBitMask = 0x000000ff;
						//m_aBitMask = 0xff000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.A8R8G8B8:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGBA;
						m_rgbBitCount = 32;
						m_fourCC = 0;
						m_rBitMask = 0x00ff0000;
						m_gBitMask = 0x0000ff00;
						m_bBitMask = 0x000000ff;
						m_aBitMask = 0xff000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.X8R8G8B8:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGB;
						m_rgbBitCount = 32;
						m_fourCC = 0;
						m_rBitMask = 0x00ff0000;
						m_gBitMask = 0x0000ff00;
						m_bBitMask = 0x000000ff;
						m_aBitMask = 0x00000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.A8B8G8R8:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGBA;
						m_rgbBitCount = 32;
						m_fourCC = 0;
						m_rBitMask = 0x000000ff;
						m_gBitMask = 0x0000ff00;
						m_bBitMask = 0x00ff0000;
						m_aBitMask = 0xff000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.X8B8G8R8:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGB;
						m_rgbBitCount = 32;
						m_fourCC = 0;
						m_rBitMask = 0x000000ff;
						m_gBitMask = 0x0000ff00;
						m_bBitMask = 0x00ff0000;
						m_aBitMask = 0x00000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.A1R5G5B5:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGBA;
						m_rgbBitCount = 16;
						m_fourCC = 0;
						m_rBitMask = 0x00007c00;
						m_gBitMask = 0x000003e0;
						m_bBitMask = 0x0000001f;
						m_aBitMask = 0x00008000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.A4R4G4B4:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGBA;
						m_rgbBitCount = 16;
						m_fourCC = 0;
						m_rBitMask = 0x00000f00;
						m_gBitMask = 0x000000f0;
						m_bBitMask = 0x0000000f;
						m_aBitMask = 0x0000f000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.R8G8B8:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGB;
						m_fourCC = 0;
						m_rgbBitCount = 24;
						m_rBitMask = 0x00ff0000;
						m_gBitMask = 0x0000ff00;
						m_bBitMask = 0x000000ff;
						m_aBitMask = 0x00000000;
						break;
					}

				case DDSTextureTools.DDSImage.FormatEnum.R5G6B5:
					{
						m_flags = (int)PixelFormatFlags.DDS_RGB;
						m_fourCC = 0;
						m_rgbBitCount = 16;
						m_rBitMask = 0x0000f800;
						m_gBitMask = 0x000007e0;
						m_bBitMask = 0x0000001f;
						m_aBitMask = 0x00000000;
						break;
					}

				default:
					break;
				}
			}

			//public void Read( Stream input )
			//{
			//   this.m_size = (uint)Utility.ReadUInt32( input );
			//   this.m_flags = (uint)Utility.ReadUInt32( input );
			//   this.m_fourCC = (uint)Utility.ReadUInt32( input );
			//   this.m_rgbBitCount = (uint)Utility.ReadUInt32( input );
			//   this.m_rBitMask = (uint)Utility.ReadUInt32( input );
			//   this.m_gBitMask = (uint)Utility.ReadUInt32( input );
			//   this.m_bBitMask = (uint)Utility.ReadUInt32( input );
			//   this.m_aBitMask = (uint)Utility.ReadUInt32( input );
			//}

			public void Write( Stream output )
			{
				Utility.WriteUInt32( output, m_size );
				Utility.WriteUInt32( output, m_flags );
				Utility.WriteUInt32( output, m_fourCC );
				Utility.WriteUInt32( output, m_rgbBitCount );
				Utility.WriteUInt32( output, m_rBitMask );
				Utility.WriteUInt32( output, m_gBitMask );
				Utility.WriteUInt32( output, m_bBitMask );
				Utility.WriteUInt32( output, m_aBitMask );
			}
		}

		//////////////////////////////////////////////

		class DdsHeader
		{
			public enum HeaderFlags
			{
				DDS_HEADER_FLAGS_TEXTURE = 0x00001007,  // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
				DDS_HEADER_FLAGS_MIPMAP = 0x00020000,   // DDSD_MIPMAPCOUNT
				DDS_HEADER_FLAGS_VOLUME = 0x00800000,   // DDSD_DEPTH
				DDS_HEADER_FLAGS_PITCH = 0x00000008,    // DDSD_PITCH
				DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000,   // DDSD_LINEARSIZE
			}

			public enum SurfaceFlags
			{
				DDS_SURFACE_FLAGS_TEXTURE = 0x00001000, // DDSCAPS_TEXTURE
				DDS_SURFACE_FLAGS_MIPMAP = 0x00400008,  // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
				DDS_SURFACE_FLAGS_CUBEMAP = 0x00000008, // DDSCAPS_COMPLEX
			}

			public enum CubemapFlags
			{
				DDS_CUBEMAP_POSITIVEX = 0x00000600, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
				DDS_CUBEMAP_NEGATIVEX = 0x00000a00, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
				DDS_CUBEMAP_POSITIVEY = 0x00001200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
				DDS_CUBEMAP_NEGATIVEY = 0x00002200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
				DDS_CUBEMAP_POSITIVEZ = 0x00004200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
				DDS_CUBEMAP_NEGATIVEZ = 0x00008200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

				DDS_CUBEMAP_ALLFACES = ( DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX |
													DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY |
													DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ )
			}

			public enum VolumeFlags
			{
				DDS_FLAGS_VOLUME = 0x00200000,  // DDSCAPS2_VOLUME
			}

			public DdsHeader()
			{
				m_pixelFormat = new DdsPixelFormat();
			}

			public uint Size()
			{
				return ( 18 * 4 ) + m_pixelFormat.Size() + ( 5 * 4 );
			}

			public uint m_size;
			public uint m_headerFlags;
			public uint m_height;
			public uint m_width;
			public uint m_pitchOrLinearSize;
			public uint m_depth;
			public uint m_mipMapCount;
			public uint m_reserved1_0;
			public uint m_reserved1_1;
			public uint m_reserved1_2;
			public uint m_reserved1_3;
			public uint m_reserved1_4;
			public uint m_reserved1_5;
			public uint m_reserved1_6;
			public uint m_reserved1_7;
			public uint m_reserved1_8;
			public uint m_reserved1_9;
			public uint m_reserved1_10;
			public DdsPixelFormat m_pixelFormat;
			public uint m_surfaceFlags;
			public uint m_cubemapFlags;
			public uint m_reserved2_0;
			public uint m_reserved2_1;
			public uint m_reserved2_2;

			//public void Read( Stream input )
			//{
			//   this.m_size = (uint)Utility.ReadUInt32( input );
			//   this.m_headerFlags = (uint)Utility.ReadUInt32( input );
			//   this.m_height = (uint)Utility.ReadUInt32( input );
			//   this.m_width = (uint)Utility.ReadUInt32( input );
			//   this.m_pitchOrLinearSize = (uint)Utility.ReadUInt32( input );
			//   this.m_depth = (uint)Utility.ReadUInt32( input );
			//   this.m_mipMapCount = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_0 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_1 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_2 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_3 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_4 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_5 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_6 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_7 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_8 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_9 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved1_10 = (uint)Utility.ReadUInt32( input );
			//   this.m_pixelFormat.Read( input );
			//   this.m_surfaceFlags = (uint)Utility.ReadUInt32( input );
			//   this.m_cubemapFlags = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved2_0 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved2_1 = (uint)Utility.ReadUInt32( input );
			//   this.m_reserved2_2 = (uint)Utility.ReadUInt32( input );
			//}

			public void Write( Stream output )
			{
				Utility.WriteUInt32( output, this.m_size );
				Utility.WriteUInt32( output, this.m_headerFlags );
				Utility.WriteUInt32( output, this.m_height );
				Utility.WriteUInt32( output, this.m_width );
				Utility.WriteUInt32( output, this.m_pitchOrLinearSize );
				Utility.WriteUInt32( output, this.m_depth );
				Utility.WriteUInt32( output, this.m_mipMapCount );
				Utility.WriteUInt32( output, this.m_reserved1_0 );
				Utility.WriteUInt32( output, this.m_reserved1_1 );
				Utility.WriteUInt32( output, this.m_reserved1_2 );
				Utility.WriteUInt32( output, this.m_reserved1_3 );
				Utility.WriteUInt32( output, this.m_reserved1_4 );
				Utility.WriteUInt32( output, this.m_reserved1_5 );
				Utility.WriteUInt32( output, this.m_reserved1_6 );
				Utility.WriteUInt32( output, this.m_reserved1_7 );
				Utility.WriteUInt32( output, this.m_reserved1_8 );
				Utility.WriteUInt32( output, this.m_reserved1_9 );
				Utility.WriteUInt32( output, this.m_reserved1_10 );
				this.m_pixelFormat.Write( output );
				Utility.WriteUInt32( output, this.m_surfaceFlags );
				Utility.WriteUInt32( output, this.m_cubemapFlags );
				Utility.WriteUInt32( output, this.m_reserved2_0 );
				Utility.WriteUInt32( output, this.m_reserved2_1 );
				Utility.WriteUInt32( output, this.m_reserved2_2 );
			}

		}

		//////////////////////////////////////////////

		public static void WriteFile( Stream output, DDSTextureTools.DDSImage image )
		{
			DdsHeader header = new DdsHeader();

			// For non-compressed textures, we need pixel width.
			//int pixelWidth = 0;

			// Identify if we're a compressed image
			bool isCompressed =
				image.Format == DDSTextureTools.DDSImage.FormatEnum.DXT1 ||
				image.Format == DDSTextureTools.DDSImage.FormatEnum.DXT3 ||
				image.Format == DDSTextureTools.DDSImage.FormatEnum.DXT5 ||
				image.Format == DDSTextureTools.DDSImage.FormatEnum.BC5;

			int width = image.Surfaces[ 0 ].Size.X;
			int height = image.Surfaces[ 0 ].Size.Y;

			//// Compute mip map count..
			int mipCount = image.Cubemap ? image.Surfaces.Length / 6 : image.Surfaces.Length;
			int mipWidth = width;// surface.Width;
			int mipHeight = height;// surface.Height;

			// Populate bulk of our DdsHeader
			header.m_size = header.Size();
			header.m_headerFlags = (uint)( DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_TEXTURE );

			if( isCompressed )
				header.m_headerFlags |= (uint)( DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_LINEARSIZE );
			else
				header.m_headerFlags |= (uint)( DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_PITCH );

			if( mipCount > 1 )
				header.m_headerFlags |= (uint)( DdsHeader.HeaderFlags.DDS_HEADER_FLAGS_MIPMAP );

			header.m_height = (uint)height;// surface.Height;
			header.m_width = (uint)width;// surface.Width;

			if( isCompressed )
			{
				// Compresssed textures have the linear flag set.So pitchOrLinearSize
				// needs to contain the entire size of the DXT block.
				int blockCount = ( ( width + 3 ) / 4 ) * ( ( height + 3 ) / 4 );
				int blockSize = ( image.Format == DDSTextureTools.DDSImage.FormatEnum.DXT1 ) ? 8 : 16;
				header.m_pitchOrLinearSize = (uint)( blockCount * blockSize );
			}
			else
			{
				int pixelWidth = 0;

				// Non-compressed textures have the pitch flag set. So pitchOrLinearSize
				// needs to contain the row pitch of the main image. DWORD aligned too.
				switch( image.Format )
				{
				case DDSTextureTools.DDSImage.FormatEnum.A8R8G8B8:
				case DDSTextureTools.DDSImage.FormatEnum.X8R8G8B8:
				case DDSTextureTools.DDSImage.FormatEnum.A8B8G8R8:
				case DDSTextureTools.DDSImage.FormatEnum.X8B8G8R8:
					pixelWidth = 4;// 32bpp
					break;

				case DDSTextureTools.DDSImage.FormatEnum.A1R5G5B5:
				case DDSTextureTools.DDSImage.FormatEnum.A4R4G4B4:
				case DDSTextureTools.DDSImage.FormatEnum.R5G6B5:
					pixelWidth = 2;// 16bpp
					break;

				case DDSTextureTools.DDSImage.FormatEnum.R8G8B8:
					pixelWidth = 3;// 24bpp
					break;

				case DDSTextureTools.DDSImage.FormatEnum.R16G16B16A16:
					pixelWidth = 8;// 64bpp
					break;
				}

				// Compute row pitch
				header.m_pitchOrLinearSize = (uint)( (int)header.m_width * pixelWidth );

				////#if	APPLY_PITCH_ALIGNMENT
				//// Align to DWORD, if we need to.. (see notes about pitch alignment all over this code)
				//header.m_pitchOrLinearSize = (uint)( ( (int)header.m_pitchOrLinearSize + 3 ) & ( ~3 ) );
				////#endif	//APPLY_PITCH_ALIGNMENT
			}

			header.m_depth = 0;
			header.m_mipMapCount = ( mipCount == 1 ) ? 0 : (uint)mipCount;
			header.m_reserved1_0 = 0;
			header.m_reserved1_1 = 0;
			header.m_reserved1_2 = 0;
			header.m_reserved1_3 = 0;
			header.m_reserved1_4 = 0;
			header.m_reserved1_5 = 0;
			header.m_reserved1_6 = 0;
			header.m_reserved1_7 = 0;
			header.m_reserved1_8 = 0;
			header.m_reserved1_9 = 0;
			header.m_reserved1_10 = 0;

			// Populate our DdsPixelFormat object
			header.m_pixelFormat.Initialise( image.Format );

			// Populate miscellanous header flags
			header.m_surfaceFlags = (uint)DdsHeader.SurfaceFlags.DDS_SURFACE_FLAGS_TEXTURE;

			if( mipCount > 1 )
				header.m_surfaceFlags |= (uint)DdsHeader.SurfaceFlags.DDS_SURFACE_FLAGS_MIPMAP;
			if( image.Cubemap )
				header.m_surfaceFlags |= (uint)DdsHeader.SurfaceFlags.DDS_SURFACE_FLAGS_CUBEMAP;

			header.m_cubemapFlags = image.Cubemap ? (uint)DdsHeader.CubemapFlags.DDS_CUBEMAP_ALLFACES : 0;
			header.m_reserved2_0 = 0;
			header.m_reserved2_1 = 0;
			header.m_reserved2_2 = 0;

			// Write out our DDS tag
			Utility.WriteUInt32( output, 0x20534444 ); // 'DDS '

			// Write out the header
			header.Write( output );

			//DDSSquish.SquishFlags squishFlags = /*ddsToken.*/GetSquishFlags( fileFormat );

			//// Our output data array will be sized as necessary
			//byte[] outputData;

			// Reset our mip width & height variables...
			mipWidth = width;// surface.Width;
			mipHeight = height;// surface.Height;

			//// Figure out how much total work each mip map is
			//Size[] writeSizes = new Size[ mipCount ];
			//int[] mipPixels = new int[ mipCount ];
			//int[] pixelsCompleted = new int[ mipCount ]; // # pixels completed once we have reached this mip
			//long totalPixels = 0;
			//for( int mipLoop = 0; mipLoop < mipCount; mipLoop++ )
			//{
			//   Size writeSize = new Size( ( mipWidth > 0 ) ? mipWidth : 1, ( mipHeight > 0 ) ? mipHeight : 1 );
			//   writeSizes[ mipLoop ] = writeSize;

			//   int thisMipPixels = writeSize.Width * writeSize.Height;
			//   mipPixels[ mipLoop ] = thisMipPixels;

			//   if( mipLoop == 0 )
			//   {
			//      pixelsCompleted[ mipLoop ] = 0;
			//   }
			//   else
			//   {
			//      pixelsCompleted[ mipLoop ] = pixelsCompleted[ mipLoop - 1 ] + mipPixels[ mipLoop - 1 ];
			//   }

			//   totalPixels += thisMipPixels;
			//   mipWidth /= 2;
			//   mipHeight /= 2;
			//}

			//mipWidth = width;// surface.Width;
			//mipHeight = height;// surface.Height;

			for( int nSurface = 0; nSurface < image.Surfaces.Length; nSurface++ )
			//for( int mipLoop = 0; mipLoop < mipCount; mipLoop++ )
			{
				DDSTextureTools.DDSImage.Surface surface = image.Surfaces[ nSurface ];
				//TextureTools.DDSImage.Surface surface = image.Surfaces[ mipLoop ];

				//Size writeSize = writeSizes[ mipLoop ];
				//Surface writeSurface = new Surface( writeSize );

				//if( mipLoop == 0 )
				//{
				//   // No point resampling the first level.. it's got exactly what we want.
				//   writeSurface = surface;
				//}
				//else
				//{
				//   // I'd love to have a UI component to select what kind of resampling, but
				//   // there's hardly any space for custom UI stuff in the Save Dialog. And I'm
				//   // not having any scrollbars in there..! 
				//   // Also, note that each mip level is formed from the main level, to reduce
				//   // compounded errors when generating mips. 
				//   writeSurface.SuperSamplingFitSurface( surface );
				//}

				//DdsSquish.ProgressFn progressFn =
				//    delegate( int workDone, int workTotal )
				//    {
				//       long thisMipPixelsDone = workDone * (long)mipWidth;
				//       long previousMipsPixelsDone = pixelsCompleted[ mipLoop ];
				//       double progress = (double)( (double)thisMipPixelsDone + (double)previousMipsPixelsDone ) / (double)totalPixels;
				//       progressCallback( this, new ProgressEventArgs( 100.0 * progress ) );
				//    };

				//if( ( /*ddsToken.m_*/fileFormat >= DdsFileFormat.DDS_FORMAT_DXT1 ) &&
				//   ( /*ddsToken.m_*/fileFormat <= DdsFileFormat.DDS_FORMAT_DXT5 ) )
				//{
				//   outputData = DDSSquish.CompressImage( writeSurface, squishFlags/*, 
				//      ( progressCallback == null ) ? null : progressFn*/
				//                                                         );
				//}
				//else
				//{
				//   Trace.Assert( false );

				//               int mipPitch = pixelWidth * writeSurface.Width;

				//               // From the DDS documents I read, I'd expected the pitch of each mip level to be
				//               // DWORD aligned. As it happens, that's not the case. Re-aligning the pitch of 
				//               // each level results in later mips getting sheared as the pitch is incorrect.
				//               // So, the following line is intentionally optional. Maybe the documentation
				//               // is referring to the pitch when accessing the mip directly.. who knows. 
				//               //
				//               // Infact, all the talk of non-compressed textures having DWORD alignment of pitch
				//               // seems to be bollocks.. If I apply alignment, then they fail to load in 3rd Party
				//               // or Microsoft DDS viewing applications.
				//               //

				//#if	APPLY_PITCH_ALIGNMENT
				//               mipPitch = ( mipPitch + 3 ) & ( ~3 );
				//#endif // APPLY_PITCH_ALIGNMENT

				//               outputData = new byte[ mipPitch * writeSurface.Height ];
				//               outputData.Initialize();

				//               for( int y = 0; y < writeSurface.Height; y++ )
				//               {
				//                  for( int x = 0; x < writeSurface.Width; x++ )
				//                  {
				//                     // Get colour from surface
				//                     ColorBgra pixelColour = writeSurface.GetPoint( x, y );
				//                     uint pixelData = 0;

				//                     switch( ddsToken.m_fileFormat )
				//                     {
				//                     case DdsFileFormat.DDS_FORMAT_A8R8G8B8:
				//                        {
				//                           pixelData = ( (uint)pixelColour.A << 24 ) |
				//                                    ( (uint)pixelColour.R << 16 ) |
				//                                    ( (uint)pixelColour.G << 8 ) |
				//                                    ( (uint)pixelColour.B << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_X8R8G8B8:
				//                        {
				//                           pixelData = ( (uint)pixelColour.R << 16 ) |
				//                                    ( (uint)pixelColour.G << 8 ) |
				//                                    ( (uint)pixelColour.B << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_A8B8G8R8:
				//                        {
				//                           pixelData = ( (uint)pixelColour.A << 24 ) |
				//                                    ( (uint)pixelColour.B << 16 ) |
				//                                    ( (uint)pixelColour.G << 8 ) |
				//                                    ( (uint)pixelColour.R << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_X8B8G8R8:
				//                        {
				//                           pixelData = ( (uint)pixelColour.B << 16 ) |
				//                                    ( (uint)pixelColour.G << 8 ) |
				//                                    ( (uint)pixelColour.R << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_A1R5G5B5:
				//                        {
				//                           pixelData = ( (uint)( ( pixelColour.A != 0 ) ? 1 : 0 ) << 15 ) |
				//                                    ( (uint)( pixelColour.R >> 3 ) << 10 ) |
				//                                    ( (uint)( pixelColour.G >> 3 ) << 5 ) |
				//                                    ( (uint)( pixelColour.B >> 3 ) << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_A4R4G4B4:
				//                        {
				//                           pixelData = ( (uint)( pixelColour.A >> 4 ) << 12 ) |
				//                                    ( (uint)( pixelColour.R >> 4 ) << 8 ) |
				//                                    ( (uint)( pixelColour.G >> 4 ) << 4 ) |
				//                                    ( (uint)( pixelColour.B >> 4 ) << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_R8G8B8:
				//                        {
				//                           pixelData = ( (uint)pixelColour.R << 16 ) |
				//                                    ( (uint)pixelColour.G << 8 ) |
				//                                    ( (uint)pixelColour.B << 0 );
				//                           break;
				//                        }

				//                     case DdsFileFormat.DDS_FORMAT_R5G6B5:
				//                        {
				//                           pixelData = ( (uint)( pixelColour.R >> 3 ) << 11 ) |
				//                                    ( (uint)( pixelColour.G >> 2 ) << 5 ) |
				//                                    ( (uint)( pixelColour.B >> 3 ) << 0 );
				//                           break;
				//                        }
				//                     }

				//                     // pixelData contains our target data.. so now set the pixel bytes
				//                     int pixelOffset = ( y * mipPitch ) + ( x * pixelWidth );
				//                     for( int loop = 0; loop < pixelWidth; loop++ )
				//                     {
				//                        outputData[ pixelOffset + loop ] = (byte)( ( pixelData >> ( 8 * loop ) ) & 0xff );
				//                     }
				//                  }

				//                  if( progressCallback != null )
				//                  {
				//                     long thisMipPixelsDone = ( y + 1 ) * (long)mipWidth;
				//                     long previousMipsPixelsDone = pixelsCompleted[ mipLoop ];
				//                     double progress = (double)( (double)thisMipPixelsDone + (double)previousMipsPixelsDone ) / (double)totalPixels;
				//                     progressCallback( this, new ProgressEventArgs( 100.0 * progress ) );
				//                  }
				//               }
				//}

				// Write the data for this mip level out.. 
				output.Write( surface.Data, 0, surface.Data.Length );
				//output.Write( outputData, 0, outputData.GetLength( 0 ) );

				//mipWidth = mipWidth / 2;
				//mipHeight = mipHeight / 2;
			}
		}
	}
}
