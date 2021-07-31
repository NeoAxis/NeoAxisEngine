// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Class for generating images in DDS format.
	/// </summary>
	public static class DDSTextureTools
	{
		/////////////////////////////////////////

		/// <summary>
		/// Data of an image for <see cref="DDSTextureTools"/>.
		/// </summary>
		public class DDSImage
		{
			FormatEnum format;
			Surface[] surfaces;
			bool cubemap;

			////////////

			public enum FormatEnum
			{
				DXT1,
				DXT3,
				DXT5,
				BC5,//_3DC
				A8R8G8B8,
				X8R8G8B8,
				A8B8G8R8,
				X8B8G8R8,
				A1R5G5B5,
				A4R4G4B4,
				R8G8B8,
				R5G6B5,
				R16G16B16A16,
			}

			////////////

			/// <summary>
			/// Surface data of an image for <see cref="DDSTextureTools"/>.
			/// </summary>
			public class Surface
			{
				Vector2I size;
				byte[] data;

				public Surface( Vector2I size, byte[] data )
				{
					this.size = size;
					this.data = data;
				}

				public Vector2I Size
				{
					get { return size; }
				}

				public byte[] Data
				{
					get { return data; }
				}
			}

			////////////

			public DDSImage( FormatEnum format, Surface[] surfaces, bool cubemap )
			{
				this.format = format;
				this.surfaces = surfaces;
				this.cubemap = cubemap;
			}

			public FormatEnum Format
			{
				get { return format; }
			}

			public Surface[] Surfaces
			{
				get { return surfaces; }
			}

			public bool Cubemap
			{
				get { return cubemap; }
			}
		}

		/////////////////////////////////////////

		//!!!!multithreading
		static List<DDSImage.Surface> generatingSurfaces;
		static int generatingSurfaceDataOffset;

		static void OutputOptions_BeginImage( int size, int width, int height, int depth, int face, int miplevel )
		{
			var surface = new DDSImage.Surface( new Vector2I( width, height ), new byte[ size ] );
			generatingSurfaces.Add( surface );
			generatingSurfaceDataOffset = 0;
		}

		static void OutputOptions_WriteData( IntPtr data, int size )
		{
			DDSImage.Surface surface = generatingSurfaces[ generatingSurfaces.Count - 1 ];
			Marshal.Copy( data, surface.Data, generatingSurfaceDataOffset, size );
			generatingSurfaceDataOffset += size;
		}

		static void OutputOptions_EndImage()
		{
		}

		public static DDSImage GenerateDDS( byte[] rgba, Vector2I size, DDSImage.FormatEnum format, bool normalMap, bool generateMipmaps, out string error )
		{
			//!!!!может не инициализировать каждый раз

			NvidiaTextureTools.Compressor compressor = null;
			NvidiaTextureTools.InputOptions inputOptions = null;
			NvidiaTextureTools.CompressionOptions compressionOptions = null;
			NvidiaTextureTools.OutputOptions outputOptions = null;

			try
			{
				NativeLibraryManager.PreLoadLibrary( "nvtt" );

				compressor = new NvidiaTextureTools.Compressor();
				inputOptions = new NvidiaTextureTools.InputOptions();
				compressionOptions = new NvidiaTextureTools.CompressionOptions();
				outputOptions = new NvidiaTextureTools.OutputOptions();

				inputOptions.SetTextureLayout( NvidiaTextureTools.TextureType.Texture2D, size.X, size.Y, 1 );

				byte[] bgra = new byte[ rgba.Length ];
				//bool containAlpha = false;
				{
					for( int y = 0; y < size.Y; y++ )
					{
						for( int x = 0; x < size.X; x++ )
						{
							int offset = ( y * size.X + x ) * 4;
							bgra[ offset + 0 ] = rgba[ offset + 2 ];
							bgra[ offset + 1 ] = rgba[ offset + 1 ];
							bgra[ offset + 2 ] = rgba[ offset + 0 ];
							bgra[ offset + 3 ] = rgba[ offset + 3 ];

							//byte alpha = bgra[ offset + 3 ];
							//if( alpha != 255 )
							//	containAlpha = true;
						}
					}
				}

				unsafe
				{
					fixed ( byte* pData = bgra )
						inputOptions.SetMipmapData( (IntPtr)pData, size.X, size.Y, 1, 0, 0 );
				}

				inputOptions.SetWrapMode( NvidiaTextureTools.WrapMode.Repeat );
				inputOptions.SetFormat( NvidiaTextureTools.InputFormat.BGRA_8UB );
				if( normalMap )
					inputOptions.SetNormalMap( true );
				inputOptions.SetMipmapGeneration( generateMipmaps );

				inputOptions.SetRoundMode( NvidiaTextureTools.RoundMode.ToNextPowerOfTwo );

				//sense?
				if( format == DDSImage.FormatEnum.DXT5 )
					inputOptions.SetAlphaMode( NvidiaTextureTools.AlphaMode.Transparency );
				//if( containAlpha )
				//{
				//	if( format == DDSImage.FormatEnum.DXT1 )
				//		inputOptions.SetAlphaMode( NvidiaTextureTools.AlphaMode.Premultiplied );
				//	else
				//		inputOptions.SetAlphaMode( NvidiaTextureTools.AlphaMode.Transparency );
				//}
				//else
				//	inputOptions.SetAlphaMode( NvidiaTextureTools.AlphaMode.None );

				//inputOptions.SetNormalMap( format == DDSImage.FormatTypes._3DC );
				//inputOptions.SetNormalMap( true );
				//inputOptions.SetConvertToNormalMap( true );
				//public void SetHeightEvaluation(float redScale, float greenScale, float blueScale, float alphaScale)
				//public void SetNormalFilter(float small, float medium, float big, float large)
				//inputOptions.SetNormalizeMipmaps( true );
				//public void SetColorTransform(ColorTransform t);
				//public void SetLinearTransfrom(int channel, float w0, float w1, float w2, float w3)

				switch( format )
				{
				case DDSImage.FormatEnum.DXT1:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.DXT1a );
					//compressionOptions.SetFormat( containAlpha ? NvidiaTextureTools.Format.DXT1a : NvidiaTextureTools.Format.DXT1 );
					break;
				case DDSImage.FormatEnum.DXT3:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.DXT3 );
					break;
				case DDSImage.FormatEnum.DXT5:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.DXT5 );
					break;
				case DDSImage.FormatEnum.BC5:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.BC5 );
					break;
				case DDSImage.FormatEnum.R8G8B8:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.RGB );
					break;
				case DDSImage.FormatEnum.A8R8G8B8:
					compressionOptions.SetFormat( NvidiaTextureTools.Format.RGBA );
					break;
				default:
					error = $"Format \'{format}\' is not supported.";
					break;
				}

				//!!!!зависает BC5 на Highest и Production
				if( format == DDSImage.FormatEnum.BC5 )
					compressionOptions.SetQuality( NvidiaTextureTools.Quality.Normal );
				else
					compressionOptions.SetQuality( NvidiaTextureTools.Quality.Highest );
				//compressionOptions.SetQuality( NvidiaTextureTools.Quality.Production );

				outputOptions.SetOutputHeader( false );
				outputOptions.SetOutputHandler( OutputOptions_BeginImage, OutputOptions_WriteData, OutputOptions_EndImage );

				generatingSurfaces = new List<DDSImage.Surface>();

				if( !compressor.Compress( inputOptions, compressionOptions, outputOptions ) )
				{
					error = "Compression failed.";
					return null;
				}

				DDSImage.FormatEnum resultFormat = format;
				if( resultFormat == DDSImage.FormatEnum.R8G8B8 )
					resultFormat = DDSImage.FormatEnum.A8R8G8B8;
				DDSImage ddsImage = new DDSImage( resultFormat, generatingSurfaces.ToArray(), false );

				error = "";
				return ddsImage;
			}
			catch( Exception e )
			{
				error = e.Message;
				return null;
			}
			finally
			{
				generatingSurfaces = null;
				generatingSurfaceDataOffset = 0;

				inputOptions?.Dispose();
				compressionOptions?.Dispose();
				outputOptions?.Dispose();
				compressor?.Dispose();
			}
		}

		public static void WriteToStream( Stream output, DDSImage image )
		{
			DDSWriter.WriteFile( output, image );
		}

		public static bool WriteToFile( string realFileName, DDSImage image, out string error )
		{
			try
			{
				using( var stream = new FileStream( realFileName, FileMode.Create ) )
					WriteToStream( stream, image );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
			error = "";
			return true;
		}

		public static bool Convert2DToDDS( string virtualFileName, string outputRealFileName, DDSImage.FormatEnum outputFormat, bool normalMap, bool generateMipmaps, out Vector2I sourceFileSize, out PixelFormat sourceFileFormat, out string error )
		{
			sourceFileSize = Vector2I.Zero;
			sourceFileFormat = PixelFormat.Unknown;

			if( !ImageUtility.LoadFromVirtualFile( virtualFileName, out var data, out var size, out var depth, out var format, out var numFaces, out var numMipmaps, out error ) )
				return false;

			sourceFileSize = size;
			sourceFileFormat = format;

			byte[] rgba = new byte[ size.X * size.Y * 4 ];

			if( format != PixelFormat.R8G8B8A8 )
			{
				for( int y = 0; y < size.Y; y++ )
				{
					for( int x = 0; x < size.X; x++ )
					{
						byte r;
						byte g;
						byte b;
						byte a = 255;
						int offset;

						switch( format )
						{
						case PixelFormat.R8G8B8:
							offset = ( y * size.X + x ) * 3;
							r = data[ offset + 2 ];
							g = data[ offset + 1 ];
							b = data[ offset + 0 ];
							break;

						case PixelFormat.X8R8G8B8:
							offset = ( y * size.X + x ) * 4;
							r = data[ offset + 2 ];
							g = data[ offset + 1 ];
							b = data[ offset + 0 ];
							break;

						case PixelFormat.A8R8G8B8:
							offset = ( y * size.X + x ) * 4;
							a = data[ offset + 3 ];
							r = data[ offset + 2 ];
							g = data[ offset + 1 ];
							b = data[ offset + 0 ];
							break;

						case PixelFormat.L8:
							offset = ( y * size.X + x );
							r = g = b = data[ offset ];
							break;

						case PixelFormat.ShortRGB:
							unsafe
							{
								fixed ( byte* pData = data )
								{
									ushort* pData2 = (ushort*)pData;
									offset = ( y * size.X + x ) * 3;
									r = (byte)( (float)pData2[ offset + 0 ] / ushort.MaxValue * 255.0f );
									g = (byte)( (float)pData2[ offset + 1 ] / ushort.MaxValue * 255.0f );
									b = (byte)( (float)pData2[ offset + 2 ] / ushort.MaxValue * 255.0f );
								}
							}
							break;

						case PixelFormat.ShortRGBA:
							unsafe
							{
								fixed ( byte* pData = data )
								{
									ushort* pData2 = (ushort*)pData;
									offset = ( y * size.X + x ) * 4;
									r = (byte)( (float)pData2[ offset + 0 ] / ushort.MaxValue * 255.0f );
									g = (byte)( (float)pData2[ offset + 1 ] / ushort.MaxValue * 255.0f );
									b = (byte)( (float)pData2[ offset + 2 ] / ushort.MaxValue * 255.0f );
									a = (byte)( (float)pData2[ offset + 3 ] / ushort.MaxValue * 255.0f );
								}
							}
							break;

						case PixelFormat.Float32RGB:
							unsafe
							{
								fixed ( byte* pData = data )
								{
									float* pData2 = (float*)pData;
									offset = ( y * size.X + x ) * 3;
									r = (byte)MathEx.Clamp( (float)pData2[ offset + 0 ] * 255.0f, 0.0f, 255.0f );
									g = (byte)MathEx.Clamp( (float)pData2[ offset + 1 ] * 255.0f, 0.0f, 255.0f );
									b = (byte)MathEx.Clamp( (float)pData2[ offset + 2 ] * 255.0f, 0.0f, 255.0f );
								}
							}
							break;

						case PixelFormat.Float32RGBA:
							unsafe
							{
								fixed ( byte* pData = data )
								{
									float* pData2 = (float*)pData;
									offset = ( y * size.X + x ) * 4;
									r = (byte)MathEx.Clamp( (float)pData2[ offset + 0 ] * 255.0f, 0.0f, 255.0f );
									g = (byte)MathEx.Clamp( (float)pData2[ offset + 1 ] * 255.0f, 0.0f, 255.0f );
									b = (byte)MathEx.Clamp( (float)pData2[ offset + 2 ] * 255.0f, 0.0f, 255.0f );
									a = (byte)MathEx.Clamp( (float)pData2[ offset + 3 ] * 255.0f, 0.0f, 255.0f );
								}
							}
							break;

						default:
							error = $"Conversion from \'{format}\' format is not supported.";
							return false;
						}

						//copy to rgba array
						offset = ( y * size.X + x ) * 4;
						rgba[ offset + 0 ] = r;
						rgba[ offset + 1 ] = g;
						rgba[ offset + 2 ] = b;
						rgba[ offset + 3 ] = a;
					}
				}
			}

			var image = GenerateDDS( rgba, size, outputFormat, normalMap, generateMipmaps, out error );
			if( image == null )
				return false;

			if( !WriteToFile( outputRealFileName, image, out error ) )
				return false;

			error = "";
			return true;

			//MapFormats mapFormat = textureData.normalMap ? NormalMapFormat : BaseMapFormat;

			//DDSImage.FormatTypes ddsFormat = DDSImage.FormatTypes.DXT1;
			//switch( mapFormat )
			//{
			//case MapFormats.DDS_DXT1:
			//	ddsFormat = DDSImage.FormatTypes.DXT1;
			//	break;
			//case MapFormats.DDS_DXT5:
			//	ddsFormat = DDSImage.FormatTypes.DXT5;
			//	break;
			//case MapFormats.DDS_3DC:
			//	ddsFormat = DDSImage.FormatTypes._3DC;
			//	break;
			//case MapFormats.DDS_ARGB:
			//	ddsFormat = DDSImage.FormatTypes.A8R8G8B8;
			//	break;
			//}

			//DDSImage ddsImage = DDSImageManager.Instance.Generate( rgba, size, ddsFormat, true );

			//bool oldExists = File.Exists( outputRealFullPath );
			//using( FileStream stream = new FileStream( outputRealFullPath, FileMode.Create ) )
			//{
			//	DDSImageManager.WriteFile( stream, ddsImage );
			//}
			//createdFiles.Add( new CreatedFileItem( outputRealFullPath, oldExists ) );

			//error = null;
			//return true;
		}
	}
}
