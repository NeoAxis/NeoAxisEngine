// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace NeoAxis
{
	//!!!!!!threading
	//!!!!!!

	struct OgreImageManager
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_loadFromFile", CallingConvention = OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool loadFromFile( void* root, [MarshalAs( UnmanagedType.LPWStr )] string fileName, out IntPtr data,
			out int dataSise, out int width, out int height, out int depth, out PixelFormat format,
			out int numFaces, out int numMipmaps, out IntPtr/*string*/ error );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_loadFromBuffer", CallingConvention = OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool loadFromBuffer( void* root, IntPtr sourceBuffer, int sourceBufferSize,
			string fileType, out IntPtr data, out int dataSise, out int width, out int height,
			out int depth, out PixelFormat format, out int numFaces, out int numMipmaps,
			out IntPtr/*string*/ error );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_freeData", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void freeData( void* root, IntPtr data );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_save", CallingConvention = OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool save( void* root, [MarshalAs( UnmanagedType.LPWStr )] string fileName, IntPtr data, int width,
			int height, int depth, PixelFormat format, int numFaces, int numMipmaps,
			out IntPtr/*string*/ error );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_scale", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void scale( void* root, IntPtr data, int width, int height,
			PixelFormat format, int newWidth, int newHeight, ImageUtility.Filters filter,
			IntPtr newData );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreImageManager_getImageFlags", CallingConvention = OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public unsafe static extern bool getImageFlags( void* root, [MarshalAs( UnmanagedType.LPWStr )] string fileName, out uint flags, out IntPtr/*string*/ error );
	}

	/// <summary>
	/// An auxiliary class for working with images.
	/// </summary>
	public static class ImageUtility
	{
		/// <summary>
		/// Represents a 2D image data for <see cref="ImageUtility"/>.
		/// </summary>
		public class Image2D
		{
			PixelFormat format;
			Vector2I size;
			byte[] data;

			//

			public Image2D( PixelFormat format, Vector2I size, byte[] data = null )
			{
				this.format = format;
				this.size = size;
				this.data = data;

				var sizeInBytes = TotalPixels * PixelFormatUtility.GetNumElemBytes( format );
				if( data != null )
				{
					if( data.Length != sizeInBytes )
						Log.Fatal( "ImageUtility: Image2D: Constructor: data.Length != TotalPixels * PixelFormatUtility.GetNumElemBytes( format )." );
				}
				else
					this.data = new byte[ sizeInBytes ];
			}

			public Image2D( PixelFormat format, Vector2I size, IntPtr data )
			{
				this.format = format;
				this.size = size;

				this.data = new byte[ SizeInBytes ];
				if( data != IntPtr.Zero )
				{
					unsafe
					{
						fixed( byte* pData = this.data )
							NativeUtility.CopyMemory( (IntPtr)pData, data, SizeInBytes );
					}
				}
			}

			public PixelFormat Format
			{
				get { return format; }
			}

			public Vector2I Size
			{
				get { return size; }
			}

			public byte[] Data
			{
				get { return data; }
			}

			public int TotalPixels
			{
				get { return size.X * size.Y; }
			}

			public int SizeInBytes
			{
				get { return TotalPixels * PixelFormatUtility.GetNumElemBytes( Format ); }
			}

			public unsafe Vector4F GetPixel( Vector2I position )
			{
				if( position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y )
					return Vector4F.Zero;

				Vector4F value = new Vector4F( 1, 1, 1, 1 );

				fixed( byte* pData = data )
				{
					switch( format )
					{
					case PixelFormat.Float32RGBA:
						{
							var p = (Vector4F*)pData + position.Y * size.X + position.X;
							value.X = p->X;
							value.Y = p->Y;
							value.Z = p->Z;
							value.W = p->W;
						}
						break;

					case PixelFormat.Float32RGB:
						{
							var p = (Vector3F*)pData + position.Y * size.X + position.X;
							value.X = p->X;
							value.Y = p->Y;
							value.Z = p->Z;
						}
						break;

					case PixelFormat.R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 3;
							value.X = (float)p[ 2 ] / 255.0f;
							value.Y = (float)p[ 1 ] / 255.0f;
							value.Z = (float)p[ 0 ] / 255.0f;
						}
						break;

					//!!!!check

					case PixelFormat.X8R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 4;
							value.X = (float)p[ 2 ] / 255.0f;
							value.Y = (float)p[ 1 ] / 255.0f;
							value.Z = (float)p[ 0 ] / 255.0f;
						}
						break;

					case PixelFormat.A8R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 4;
							value.W = (float)p[ 3 ] / 255.0f;
							value.X = (float)p[ 2 ] / 255.0f;
							value.Y = (float)p[ 1 ] / 255.0f;
							value.Z = (float)p[ 0 ] / 255.0f;
						}
						break;

					case PixelFormat.L8:
						{
							var p = pData + ( position.Y * size.X + position.X );
							var v = (float)p[ 0 ] / 255.0f;
							value.X = v;
							value.Y = v;
							value.Z = v;
						}
						break;

					//!!!!

					default:
						throw new Exception( $"ImageUtility: GetPixel: Format \"{format}\" is not supported." );
						//Log.Fatal( "ImageUtility: SetPixel: Format is not supported." );
						//break;
					}
				}

				return value;
			}

			public unsafe ColorByte GetPixelByte( Vector2I position )
			{
				if( position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y )
					return ColorByte.Zero;

				ColorByte value = ColorByte.One;

				fixed( byte* pData = data )
				{
					switch( format )
					{
					//case PixelFormat.Float32RGBA:
					//	{
					//		var p = (Vector4F*)pData + position.Y * size.X + position.X;
					//		value.X = p->X;
					//		value.Y = p->Y;
					//		value.Z = p->Z;
					//		value.W = p->W;
					//	}
					//	break;

					//case PixelFormat.Float32RGB:
					//	{
					//		var p = (Vector3F*)pData + position.Y * size.X + position.X;
					//		value.X = p->X;
					//		value.Y = p->Y;
					//		value.Z = p->Z;
					//	}
					//	break;

					//case PixelFormat.R8G8B8:
					//	{
					//		var p = pData + ( position.Y * size.X + position.X ) * 3;
					//		value.X = (float)p[ 2 ] / 255.0f;
					//		value.Y = (float)p[ 1 ] / 255.0f;
					//		value.Z = (float)p[ 0 ] / 255.0f;
					//	}
					//	break;

					////!!!!check

					//case PixelFormat.X8R8G8B8:
					//	{
					//		var p = pData + ( position.Y * size.X + position.X ) * 4;
					//		value.X = (float)p[ 2 ] / 255.0f;
					//		value.Y = (float)p[ 1 ] / 255.0f;
					//		value.Z = (float)p[ 0 ] / 255.0f;
					//	}
					//	break;

					case PixelFormat.A8R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 4;
							value = new ColorByte( p[ 2 ], p[ 1 ], p[ 0 ], p[ 3 ] );
							//value.W = (float)p[ 3 ] / 255.0f;
							//value.X = (float)p[ 2 ] / 255.0f;
							//value.Y = (float)p[ 1 ] / 255.0f;
							//value.Z = (float)p[ 0 ] / 255.0f;
						}
						break;

					//case PixelFormat.L8:
					//	{
					//		var p = pData + ( position.Y * size.X + position.X );
					//		var v = (float)p[ 0 ] / 255.0f;
					//		value.X = v;
					//		value.Y = v;
					//		value.Z = v;
					//	}
					//	break;

					//!!!!

					default:
						throw new Exception( $"ImageUtility: GetPixel: Format \"{format}\" is not supported." );
						//Log.Fatal( "ImageUtility: SetPixel: Format is not supported." );
						//break;
					}
				}

				return value;
			}

			public unsafe void SetPixel( Vector2I position, Vector4F value )
			{
				if( position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y )
					return;

				fixed( byte* pData = data )
				{
					switch( format )
					{
					case PixelFormat.Float32RGBA:
						{
							var p = (Vector4F*)pData + position.Y * size.X + position.X;
							p->X = value.X;
							p->Y = value.Y;
							p->Z = value.Z;
							p->W = value.W;
						}
						break;

					case PixelFormat.Float32RGB:
						{
							var p = (Vector3F*)pData + position.Y * size.X + position.X;
							p->X = value.X;
							p->Y = value.Y;
							p->Z = value.Z;
						}
						break;

					case PixelFormat.A8R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 4;
							p[ 3 ] = (byte)MathEx.Clamp( (int)( value.W * 255.0 ), 0, 255 );
							p[ 2 ] = (byte)MathEx.Clamp( (int)( value.X * 255.0 ), 0, 255 );
							p[ 1 ] = (byte)MathEx.Clamp( (int)( value.Y * 255.0 ), 0, 255 );
							p[ 0 ] = (byte)MathEx.Clamp( (int)( value.Z * 255.0 ), 0, 255 );
						}
						break;

					default:
						throw new Exception( $"ImageUtility: SetPixel: Format \"{format}\" is not supported." );
						//Log.Fatal( "ImageUtility: SetPixel: Format is not supported." );
						//break;
					}
				}
			}

			public unsafe void SetPixel( Vector2I position, ColorByte value )
			{
				if( position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y )
					return;

				fixed( byte* pData = data )
				{
					switch( format )
					{
					//case PixelFormat.Float32RGBA:
					//	{
					//		var p = (Vector4F*)pData + position.Y * size.X + position.X;
					//		p->X = value.X;
					//		p->Y = value.Y;
					//		p->Z = value.Z;
					//		p->W = value.W;
					//	}
					//	break;

					//case PixelFormat.Float32RGB:
					//	{
					//		var p = (Vector3F*)pData + position.Y * size.X + position.X;
					//		p->X = value.X;
					//		p->Y = value.Y;
					//		p->Z = value.Z;
					//	}
					//	break;

					case PixelFormat.A8R8G8B8:
						{
							var p = pData + ( position.Y * size.X + position.X ) * 4;
							p[ 3 ] = value.Alpha;
							p[ 2 ] = value.Red;
							p[ 1 ] = value.Green;
							p[ 0 ] = value.Blue;
							//p[ 3 ] = (byte)MathEx.Clamp( (int)( value.W * 255.0 ), 0, 255 );
							//p[ 2 ] = (byte)MathEx.Clamp( (int)( value.X * 255.0 ), 0, 255 );
							//p[ 1 ] = (byte)MathEx.Clamp( (int)( value.Y * 255.0 ), 0, 255 );
							//p[ 0 ] = (byte)MathEx.Clamp( (int)( value.Z * 255.0 ), 0, 255 );
						}
						break;

					default:
						throw new Exception( $"ImageUtility: SetPixel: Format \"{format}\" is not supported." );
						//Log.Fatal( "ImageUtility: SetPixel: Format is not supported." );
						//break;
					}
				}
			}

			public void Blit( Vector2I writePosition, Image2D image, Vector2I readPosition )
			{
				//!!!!slowly

				for( int y = 0; y < image.Size.Y; y++ )
					for( int x = 0; x < image.Size.X; x++ )
						SetPixel( writePosition + new Vector2I( x, y ), image.GetPixel( readPosition + new Vector2I( x, y ) ) );
			}

			public void Blit( Vector2I writePosition, Image2D image )
			{
				Blit( writePosition, image, Vector2I.Zero );
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Loads an image file.
		/// </summary>
		/// <param name="virtualFileName">The virtual file name.</param>
		/// <param name="data">The image data.</param>
		/// <param name="size">The image size.</param>
		/// <param name="depth">The image depth (in 3d images, numbers of layers, otherwhise 1).</param>
		/// <param name="format">Pixel format.</param>
		/// <param name="numFaces">The number of faces the image data has inside (6 for cubemaps, 1 otherwise).</param>
		/// <param name="numMipmaps">The number of mipmaps the image data has inside.</param>
		/// <param name="error">Output error string.</param>
		/// <returns><b>true</b> if image is loaded; otherwise, <b>false</b>.</returns>
		public static bool LoadFromVirtualFile( string virtualFileName, out byte[] data, out Vector2I size, out int depth,
			out PixelFormat format, out int numFaces, out int numMipmaps, out string error )
		{
			unsafe
			{
				IntPtr pData;
				int dataSize;
				int width;
				int height;
				IntPtr errPointer;

				bool result = OgreImageManager.loadFromFile( RenderingSystem.realRoot, virtualFileName, out pData,
					out dataSize, out width, out height, out depth, out format, out numFaces,
					out numMipmaps, out errPointer );
				string err = OgreNativeWrapper.GetOutString( errPointer );

				error = null;
				data = null;
				size = new Vector2I( width, height );

				if( err != null )
					error = string.Format( "Loading file failed \"{0}\" ({1}).", virtualFileName, err );

				if( pData != IntPtr.Zero )
				{
					data = new byte[ dataSize ];
					Marshal.Copy( pData, data, 0, data.Length );
					OgreImageManager.freeData( RenderingSystem.realRoot, pData );
				}

				return result;
			}
		}

		/// <summary>
		/// Loads an image from buffer.
		/// </summary>
		/// <param name="sourceBuffer">The source buffer.</param>
		/// <param name="fileType">The file type (file extension).</param>
		/// <param name="data">The image data.</param>
		/// <param name="size">The image size.</param>
		/// <param name="depth">The image depth (in 3d images, numbers of layers, otherwhise 1).</param>
		/// <param name="format">Pixel format.</param>
		/// <param name="numFaces">The number of faces the image data has inside (6 for cubemaps, 1 otherwise).</param>
		/// <param name="numMipmaps">The number of mipmaps the image data has inside.</param>
		/// <param name="error">Output error string.</param>
		/// <returns><b>true</b> if image is loaded; otherwise, <b>false</b>.</returns>
		public static bool LoadFromBuffer( byte[] sourceBuffer, string fileType, out byte[] data,
			out Vector2I size, out int depth, out PixelFormat format, out int numFaces,
			out int numMipmaps, out string error )
		{
			unsafe
			{
				IntPtr pData;
				int dataSize;
				int width;
				int height;
				IntPtr errPointer;

				bool result;
				fixed( byte* pSourceBuffer = sourceBuffer )
				{
					result = OgreImageManager.loadFromBuffer( RenderingSystem.realRoot,
						(IntPtr)pSourceBuffer, sourceBuffer.Length, fileType, out pData, out dataSize,
						out width, out height, out depth, out format, out numFaces, out numMipmaps,
						out errPointer );
				}
				string err = OgreNativeWrapper.GetOutString( errPointer );

				error = null;
				data = null;
				size = new Vector2I( width, height );

				if( err != null )
					error = string.Format( "Loading file from buffer failed ({0}).", err );

				if( pData != IntPtr.Zero )
				{
					data = new byte[ dataSize ];
					Marshal.Copy( pData, data, 0, data.Length );
					OgreImageManager.freeData( RenderingSystem.realRoot, pData );
				}

				return result;
			}
		}

		/// <summary>
		/// Loads an image file.
		/// </summary>
		/// <param name="realFileName">The real file name.</param>
		/// <param name="data">The image data.</param>
		/// <param name="size">The image size.</param>
		/// <param name="depth">The image depth (in 3d images, numbers of layers, otherwhise 1).</param>
		/// <param name="format">Pixel format.</param>
		/// <param name="numFaces">The number of faces the image data has inside (6 for cubemaps, 1 otherwise).</param>
		/// <param name="numMipmaps">The number of mipmaps the image data has inside.</param>
		/// <param name="error">Output error string.</param>
		/// <returns><b>true</b> if image is loaded; otherwise, <b>false</b>.</returns>
		public static bool LoadFromRealFile( string realFileName, out byte[] data, out Vector2I size, out int depth,
			out PixelFormat format, out int numFaces, out int numMipmaps, out string error )
		{
			byte[] buffer = null;
			string fileType = null;
			try
			{
				buffer = File.ReadAllBytes( realFileName );
				fileType = Path.GetExtension( realFileName ).Replace( ".", "" );
			}
			catch( Exception e )
			{
				data = null;
				size = Vector2I.Zero;
				depth = 0;
				format = PixelFormat.Unknown;
				numFaces = 0;
				numMipmaps = 0;
				error = e.Message;
				return false;
			}

			return LoadFromBuffer( buffer, fileType, out data, out size, out depth, out format, out numFaces, out numMipmaps, out error );
		}

		/// <summary>
		/// Save the image as a file.
		/// </summary>
		/// <param name="realFileName">The real file name.</param>
		/// <param name="data">The image data.</param>
		/// <param name="size">The image size.</param>
		/// <param name="depth">The image depth (in 3d images, numbers of layers, otherwhise 1).</param>
		/// <param name="format">Pixel format.</param>
		/// <param name="numFaces">The number of faces the image data has inside (6 for cubemaps, 1 otherwise).</param>
		/// <param name="numMipmaps">The number of mipmaps the image data has inside.</param>
		/// <param name="error">Output error string.</param>
		/// <returns><b>true</b> if image is currently serialized; otherwise, <b>false</b>.</returns>
		public static bool Save( string realFileName, IntPtr data, Vector2I size, int depth, PixelFormat format,
		int numFaces, int numMipmaps, out string error )
		{
			error = null;

			unsafe
			{
				IntPtr errPointer;
				bool result = OgreImageManager.save( RenderingSystem.realRoot, realFileName, data,
					size.X, size.Y, depth, format, numFaces, numMipmaps, out errPointer );
				string err = OgreNativeWrapper.GetOutString( errPointer );
				if( err != null )
					error = string.Format( "Saving file failed \"{0}\" ({1}).", realFileName, err );
				return result;
			}
		}

		/// <summary>
		/// Save the image as a file.
		/// </summary>
		/// <param name="realFileName">The real file name.</param>
		/// <param name="data">The image data.</param>
		/// <param name="size">The image size.</param>
		/// <param name="depth">The image depth (in 3d images, numbers of layers, otherwhise 1).</param>
		/// <param name="format">Pixel format.</param>
		/// <param name="numFaces">The number of faces the image data has inside (6 for cubemaps, 1 otherwise).</param>
		/// <param name="numMipmaps">The number of mipmaps the image data has inside.</param>
		/// <param name="error">Output error string.</param>
		/// <returns><b>true</b> if image is currently serialized; otherwise, <b>false</b>.</returns>
		public static bool Save( string realFileName, byte[] data, Vector2I size, int depth, PixelFormat format,
			int numFaces, int numMipmaps, out string error )
		{
			unsafe
			{
				fixed( byte* pData = data )
				{
					return Save( realFileName, (IntPtr)pData, size, depth, format, numFaces, numMipmaps, out error );
				}
			}
		}

		public enum Filters
		{
			Nearest,
			Linear,
			Bilinear,
			Box,
			Triangle,
			Bicubic
		};

		public static void Scale( byte[] data, Vector2I size, PixelFormat format, Vector2I newSize, Filters filter, out byte[] newData )
		{
			newData = new byte[ newSize.X * newSize.Y * PixelFormatUtility.GetNumElemBytes( format ) ];

			unsafe
			{
				fixed( byte* pData = data, pNewData = newData )
				{
					OgreImageManager.scale( RenderingSystem.realRoot, (IntPtr)pData, size.X, size.Y, format, newSize.X, newSize.Y, filter, (IntPtr)pNewData );
				}
			}
		}

		public static void Scale( IntPtr data, Vector2I size, PixelFormat format, Vector2I newSize, Filters filter, IntPtr newData )
		{
			unsafe
			{
				OgreImageManager.scale( RenderingSystem.realRoot, data, size.X, size.Y, format, newSize.X, newSize.Y, filter, newData );
			}
		}

		[Flags]
		public enum ImageFlags
		{
			Compressed = 0x0001,
			Cubemap = 0x0002,
			Texture3D = 0x0004
		}

		public static bool GetImageFlags( string realFileName, out ImageFlags flags, out string error )
		{
			error = null;

			unsafe
			{
				bool result = OgreImageManager.getImageFlags( RenderingSystem.realRoot, realFileName, out uint uflags, out IntPtr errPointer );
				string err = OgreNativeWrapper.GetOutString( errPointer );
				if( err != null )
					error = string.Format( "Error in file \"{0}\" ({1}).", realFileName, err );

				flags = (ImageFlags)uflags;

				return result;
			}
		}

		public static bool DetectTextureType( byte[] data, Vector2I size, PixelFormat format, out bool hasAlpha, out bool normalMap )
		{
			hasAlpha = false;
			normalMap = size.X > 64 && size.Y > 64;

			int normalMapCount = 0;
			int totalCount = 0;

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
					case PixelFormat.R8G8B8A8:
						offset = ( y * size.X + x ) * 4;
						r = data[ offset + 3 ];
						g = data[ offset + 2 ];
						b = data[ offset + 1 ];
						a = data[ offset + 0 ];
						break;
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
					default:
						hasAlpha = false;
						normalMap = false;
						return false;
					}

					if( a != 255 )
						hasAlpha = true;

					if( normalMap )
					{
						var v = new Vector3F( r, g, b ) / 255.0f;
						v *= 2;
						v -= new Vector3F( 1, 1, 1 );
						var l = v.Length();
						if( l > 0.75f && l < 1.25f )
						{
							normalMapCount++;
						}
						else
						{
							if( totalCount > 10000 )
							{
								float p = (float)normalMapCount / totalCount;
								if( p < .95f )
									normalMap = false;
							}
						}
					}

					totalCount++;
				}
			}

			if( normalMap )
			{
				float p = (float)normalMapCount / totalCount;
				if( p < .95f )
					normalMap = false;
			}

			return true;
		}

		static bool IsPowerOfTwo( ulong x )
		{
			return ( x != 0 ) && ( ( x & ( x - 1 ) ) == 0 );
		}

		public delegate void ImageAutoCompressionDetectTypeOverrideDelegate( byte[] data, Vector2I size, PixelFormat format, ref string useCompression );
		public static event ImageAutoCompressionDetectTypeOverrideDelegate ImageAutoCompressionDetectTypeOverride;

		public static string ImageAutoCompressionDetectType( byte[] data, Vector2I size, PixelFormat format )
		{
			//override
			string useCompression = null;
			ImageAutoCompressionDetectTypeOverride?.Invoke( data, size, format, ref useCompression );
			if( useCompression != null )
				return useCompression;

			//!!!!какие-то еще?
			//!!!!ShortRGB по сути можно было компрессировать, но тогда нужно определять не будет ли потери точности.
			if( format == PixelFormat.L8 || format == PixelFormat.L16 || format == PixelFormat.A8 || format == PixelFormat.ByteLA ||
				format == PixelFormat.ShortGR || format == PixelFormat.ShortRGB || format == PixelFormat.ShortRGBA ||
				PixelFormatUtility.IsFloatingPoint( format ) )
				return "NoCompression";

			//default implementation
			if( size.X > 256 && size.Y > 256 && IsPowerOfTwo( (ulong)size.X ) && IsPowerOfTwo( (ulong)size.Y ) )
			{
				if( DetectTextureType( data, size, format, out var hasAlpha, out var normalMap ) )
				{
					if( normalMap && !hasAlpha )
						return "NormalMap";
					else if( hasAlpha )
						return "DXT5";
					else
						return "DXT1";
				}
				else
				{
					if( PixelFormatUtility.HasAlpha( format ) )
						return "DXT5";
					else
						return "DXT1";
				}
			}
			else
				return "NoCompression";
		}
	}
}
