// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using Internal.SharpBgfx;
using System.Security.Cryptography;
using NeoAxis.Editor;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Represents a texture.
	/// </summary>
	public class GpuTexture : ThreadSafeDisposable
	{
		static ESet<GpuTexture> all = new ESet<GpuTexture>();

		//constants
		public enum ModeEnum
		{
			Load,
			Create,
			RenderTarget,
		}
		ModeEnum mode;
		string[] loadFileNames;
		bool loadFileCube4x3;
		string componentTextureVirtualFileName;

		ImageComponent.TypeEnum textureType = ImageComponent.TypeEnum._2D;
		Vector2I sourceSize;
		Vector2I resultSize;
		int sourceDepth;
		int resultDepth;
		PixelFormat sourceFormat = PixelFormat.Unknown;
		PixelFormat resultFormat = PixelFormat.Unknown;
		int resultMipLevels;
		bool mipmaps = true;
		int arrayLayers;
		Usages usage;
		int fullSceneAntialiasing;

		Texture nativeObject;
		bool nativeObjectUnloadSupport;
		bool nativeObjectUnloaded;
		double nativeObjectLastUsedTime;

		//!!!!опционально не хранить данные. восстанавливать когда device lost.
		SurfaceData[] data;
		bool needUpdateNative;

		List<RenderTargetItem> renderTargets = new List<RenderTargetItem>();

		////bug fix for Intel
		//internal bool _renderTargetCleared;

		/////////////////////////////////////////

		/// <summary>Enum describing buffer usage; not mutually exclusive.</summary>
		[Flags]
		public enum Usages
		{
			//!!!!сложна

			/// <summary>
			/// Static buffer which the application rarely modifies once created. Modifying 
			/// the contents of this buffer will involve a performance hit.
			/// </summary>
			Static = 1,//GpuBuffer._LowLevelUsages.Static,//Ogre::TU_STATIC,

			/// <summary>
			/// Indicates the application would like to modify this buffer with the CPU
			/// fairly often. 
			/// Buffers created with this flag will typically end up in AGP memory rather 
			/// than video memory.
			/// </summary>
			Dynamic = 2,//GpuBuffer._LowLevelUsages.Dynamic,//Ogre::TU_DYNAMIC,

			/// <summary>
			/// Indicates the application will never read the contents of the buffer back, 
			/// it will only ever write data. Locking a buffer with this flag will ALWAYS 
			/// return a pointer to new, blank memory rather than the memory associated 
			/// with the contents of the buffer; this avoids DMA stalls because you can 
			/// write to a new memory area while the previous one is being used. 
			/// </summary>
			WriteOnly = 4,//GpuBuffer._LowLevelUsages.WriteOnly,//Ogre::TU_WRITE_ONLY,

			//!!!!может убрать комбинированные

			/// <summary>
			/// Combination of <b>Static</b> and <b>WriteOnly</b>.
			/// </summary>
			StaticWriteOnly = 5,//GpuBuffer._LowLevelUsages.StaticWriteOnly,//Ogre::TU_STATIC_WRITE_ONLY, 

			/// <summary>
			/// Combination of <b>Dynamic</b> and <b>WriteOnly</b>. If you use 
			/// this, strongly consider using <b>DynamicWriteOnlyDiscardable</b>
			/// instead if you update the entire contents of the buffer very 
			/// regularly. 
			/// </summary>
			DynamicWriteOnly = 6,//GpuBuffer._LowLevelUsages.DynamicWriteOnly,//Ogre::TU_DYNAMIC_WRITE_ONLY,

			/// <summary>
			/// Texture can be used as the destination of a blit operation.
			/// </summary>
			BlitDestination = 7,

			//!!!!Discardable - такого нет
			/// <summary>
			/// Combination of <b>Dynamic</b>, <b>WriteOnly</b> and <b>Discardable</b>.
			/// </summary>
			DynamicWriteOnlyDiscardable = 14,//GpuBuffer._LowLevelUsages.DynamicWriteOnlyDiscardable,//Ogre::TU_DYNAMIC_WRITE_ONLY_DISCARDABLE,

			/// <summary>
			/// Mipmaps will be automatically generated for this texture.
			/// </summary>
			AutoMipmap = 0x100,//Ogre::TU_AUTOMIPMAP,

			/// <summary>
			/// This texture will be a render target, ie. used as a target for render to texture
			/// setting this flag will ignore all other texture usages except <b>AutoMipmap</b>.
			/// </summary>
			RenderTarget = 0x200,//Ogre::TU_RENDERTARGET,

			/// <summary>
			/// Texture data can be read back.
			/// </summary>
			ReadBack = 0x400,

			ComputeWrite = 0x800,

			///// <summary>
			///// Default to automatic mipmap generation static textures.
			///// </summary>
			//Default = AutoMipmap | StaticWriteOnly,//Ogre::TU_DEFAULT,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a surface data of <see cref="GpuTexture"/>.
		/// </summary>
		public class SurfaceData
		{
			public ArraySegment<byte> Data { get; }
			public int ArrayLayer { get; }
			public int Face { get; }
			public int MipLevel { get; }

			public SurfaceData( ArraySegment<byte> data, int arrayLayer = 0, int face = 0, int mipLevel = 0 )
			{
				Data = data;
				ArrayLayer = arrayLayer;
				Face = face;
				MipLevel = mipLevel;
			}

			public SurfaceData( byte[] data, int arrayLayer = 0, int face = 0, int mipLevel = 0 )
			{
				Data = new ArraySegment<byte>( data );
				ArrayLayer = arrayLayer;
				Face = face;
				MipLevel = mipLevel;
			}
		}

		/////////////////////////////////////////

		class RenderTargetItem
		{
			int mip;
			int slice;
			RenderTexture renderTarget;

			//

			internal RenderTargetItem( int mip, int slice, RenderTexture renderTarget )
			{
				this.mip = mip;
				this.slice = slice;
				this.renderTarget = renderTarget;
			}

			public int Mip
			{
				get { return mip; }
			}

			public int Slice
			{
				get { return slice; }
			}

			public RenderTexture RenderTarget
			{
				get { return renderTarget; }
			}
		}

		/////////////////////////////////////////

		unsafe internal GpuTexture( ImageComponent.TypeEnum type, Vector2I size, int depth, bool mipmaps, int arrayLayers, PixelFormat format,
			Usages usage, int fullSceneAntialiasing, out string error )
		{
			error = "";

			this.textureType = type;
			sourceSize = size;
			resultSize = size;
			sourceDepth = depth;
			resultDepth = depth;
			sourceFormat = format;
			resultFormat = format;
			this.mipmaps = mipmaps;
			this.arrayLayers = arrayLayers;
			this.usage = usage;
			this.fullSceneAntialiasing = fullSceneAntialiasing;

			if( ( usage & Usages.RenderTarget ) != 0 )
			{
				mode = ModeEnum.RenderTarget;
				CreateNativeObject();

				if( nativeObject == null )
				{
					//!!!!
					error = "Unable to create render target.";
				}
			}
			else
			{
				mode = ModeEnum.Create;

				//native object will created later
			}

			lock( all )
				all.Add( this );
		}

		void CreateNativeObject()
		{
			//commented to optimize
			//EngineThreading.CheckMainThread();

			if( nativeObject != null )
				Log.Fatal( "Texture: Create: The texture data is already initialized. nativeObject != null." );

			TextureFormat realFormat = ConvertFormat( ResultFormat );

			TextureFlags flags = TextureFlags.None;
			if( ( usage & Usages.RenderTarget ) != 0 )
			{
				//if( ( usage & Usages.WriteOnly ) != 0 )
				//flags |= TextureFlags.RenderTargetWriteOnly;
				//else
				flags |= TextureFlags.RenderTarget;
			}

			if( ( usage & Usages.ComputeWrite ) != 0 )
				flags |= TextureFlags.ComputeWrite;

			if( ( usage & Usages.BlitDestination ) != 0 )
				flags |= TextureFlags.BlitDestination;

			if( ( usage & Usages.ReadBack ) != 0 )
				flags |= TextureFlags.ReadBack;


			//MSAASample
			//RenderTargetWriteOnly

			if( fullSceneAntialiasing != 0 )
			{
				switch( fullSceneAntialiasing )
				{
				case 2: flags |= TextureFlags.RenderTargetMultisample2x; break;
				case 4: flags |= TextureFlags.RenderTargetMultisample4x; break;
				case 8: flags |= TextureFlags.RenderTargetMultisample8x; break;
				case 16: flags |= TextureFlags.RenderTargetMultisample16x; break;
				}

				//flags |= TextureFlags.MSAASample;
				//flags |= TextureFlags.RenderTargetWriteOnly;
			}
			//flags |= TextureFlags.RenderTargetWriteOnly;


			switch( textureType )
			{
			case ImageComponent.TypeEnum._2D:
				nativeObject = Texture.Create2D( resultSize.X, resultSize.Y, mipmaps, arrayLayers, realFormat, flags );
				nativeObjectLastUsedTime = EngineApp.EngineTime;
				break;

			case ImageComponent.TypeEnum._3D:
				nativeObject = Texture.Create3D( resultSize.X, resultSize.Y, sourceDepth, mipmaps, realFormat, flags );
				nativeObjectLastUsedTime = EngineApp.EngineTime;
				break;

			case ImageComponent.TypeEnum.Cube:
				nativeObject = Texture.CreateCube( resultSize.X, mipmaps, arrayLayers, realFormat, flags );
				nativeObjectLastUsedTime = EngineApp.EngineTime;
				break;
			}

			if( nativeObject != null )
				resultMipLevels = nativeObject.MipLevels;
		}

		GpuTexture( string[] loadFileNames, bool cube4x3, string componentTextureVirtualFileName )
		{
			mode = ModeEnum.Load;
			this.loadFileNames = loadFileNames;
			this.loadFileCube4x3 = cube4x3;
			this.componentTextureVirtualFileName = componentTextureVirtualFileName;

			lock( all )
				all.Add( this );
		}

		public static GpuTexture CreateFromFile( string virtualFileName, bool cube4x3, out string error )
		{
			var result = new GpuTexture( new string[] { virtualFileName }, cube4x3, virtualFileName );

			//detect type of DDS texture
			if( Path.GetExtension( virtualFileName ) == ".dds" )
			{
				//!!!!slowly?
				if( ImageUtility.GetImageFlags( virtualFileName, out ImageUtility.ImageFlags flags, out error ) )
				{
					if( flags.HasFlag( ImageUtility.ImageFlags.Cubemap ) )
						result.TextureType = ImageComponent.TypeEnum.Cube;
					else if( flags.HasFlag( ImageUtility.ImageFlags.Texture3D ) )
						result.TextureType = ImageComponent.TypeEnum._3D;
					else
						result.TextureType = ImageComponent.TypeEnum._2D;
				}
			}
			if( cube4x3 )
				result.TextureType = ImageComponent.TypeEnum.Cube;

			if( !result.Load( out error ) )
			{
				result.Dispose();
				result = null;
			}
			return result;
		}

		//!!!!
		//public static GpuTexture CreateVolume( string virtualFileName, out string error )
		//{
		//	throw new NotImplementedException();
		//}

		//// name + postfixes or DDS.
		//public static GpuTexture CreateCube( string virtualFileName, out string error )
		//{
		//	var result = new GpuTexture( virtualFileName );
		//	result.TextureType = Texture.TypeEnum.Cube;

		//	if( !result.Load( out error ) )
		//	{
		//		result.Dispose();
		//		result = null;
		//	}
		//	return result;
		//}

		// unique names
		public static GpuTexture CreateCube( string[] virtualFileNames, bool cube4x3, string componentTextureVirtualFileName, out string error )
		{
			var result = new GpuTexture( virtualFileNames, cube4x3, componentTextureVirtualFileName );
			result.TextureType = ImageComponent.TypeEnum.Cube;

			if( !result.Load( out error ) )
			{
				result.Dispose();
				result = null;
			}
			return result;
		}

		internal static TextureFormat ConvertFormat( PixelFormat format )
		{
			//!!!!все форматы

			switch( format )
			{
			case PixelFormat.Unknown: return TextureFormat.Unknown;
			case PixelFormat.L8: return TextureFormat.R8;
			case PixelFormat.L16: return TextureFormat.R16;
			case PixelFormat.A8: return TextureFormat.A8;
			case PixelFormat.R8G8_UInt: return TextureFormat.RG8;
			case PixelFormat.R32_UInt: return TextureFormat.R32U;
			case PixelFormat.R32G32_UInt: return TextureFormat.RG32U;
			case PixelFormat.R32G32B32A32_UInt: return TextureFormat.RGBA32U;

			//case PixelFormat.R5G6B5: return TextureFormat.R5G6B5;
			//case PixelFormat.R8G8B8: return TextureFormat.RGB8;

			case PixelFormat.A8R8G8B8: return TextureFormat.BGRA8;
			case PixelFormat.A8B8G8R8: return TextureFormat.RGBA8;

			case PixelFormat.DXT1: return TextureFormat.BC1;
			case PixelFormat.DXT2: return TextureFormat.BC1;
			case PixelFormat.DXT3: return TextureFormat.BC2;
			case PixelFormat.DXT4: return TextureFormat.BC2;
			case PixelFormat.DXT5: return TextureFormat.BC3;
			case PixelFormat.BC4_UNorm: return TextureFormat.BC4;
			case PixelFormat.BC5_UNorm: return TextureFormat.BC5;
			case PixelFormat.BC6H_SF16: return TextureFormat.BC6H;
			case PixelFormat.BC7_UNorm: return TextureFormat.BC7;//BC7_UNorm_SRGB

			case PixelFormat.Float16R: return TextureFormat.R16F;
			case PixelFormat.Float16GR: return TextureFormat.RG16F;
			case PixelFormat.Float16RGBA: return TextureFormat.RGBA16F;
			case PixelFormat.Float32R: return TextureFormat.R32F;
			case PixelFormat.Float32GR: return TextureFormat.RG32F;
			case PixelFormat.Float32RGBA: return TextureFormat.RGBA32F;
			case PixelFormat.R11G11B10_Float: return TextureFormat.RG11B10F;

			case PixelFormat.ShortRGBA: return TextureFormat.RGBA16U;

			case PixelFormat.Depth24S8: return TextureFormat.D24S8;
			case PixelFormat.Depth32F: return TextureFormat.D32F;
			}

			Log.Fatal( "GpuTexture: ConvertFormat (from PixelFormat to TextureFormat): " + format.ToString() + "." );
			return TextureFormat.Unknown;
		}

		internal static PixelFormat ConvertFormat( TextureFormat format )
		{
			switch( format )
			{
			case TextureFormat.Unknown: return PixelFormat.Unknown;

			case TextureFormat.R8: return PixelFormat.L8;
			case TextureFormat.R16: return PixelFormat.L16;
			case TextureFormat.A8: return PixelFormat.A8;
			case TextureFormat.RG8: return PixelFormat.R8G8_UInt;
			case TextureFormat.R32U: return PixelFormat.R32_UInt;
			case TextureFormat.RG32U: return PixelFormat.R32G32_UInt;
			case TextureFormat.RGBA32U: return PixelFormat.R32G32B32A32_UInt;

			//case PixelFormat.R5G6B5: return TextureFormat.R5G6B5;

			//!!!!new. так? или B8G8R8?
			case TextureFormat.RGB8: return PixelFormat.R8G8B8;

			case TextureFormat.BGRA8: return PixelFormat.A8R8G8B8;
			case TextureFormat.RGBA8: return PixelFormat.A8B8G8R8;

			case TextureFormat.BC1: return PixelFormat.DXT1;
			//case TextureFormat.BC1: return PixelFormat.DXT2;
			case TextureFormat.BC2: return PixelFormat.DXT3;
			//case TextureFormat.BC2: return PixelFormat.DXT4;
			case TextureFormat.BC3: return PixelFormat.DXT5;
			case TextureFormat.BC4: return PixelFormat.BC4_UNorm;
			case TextureFormat.BC5: return PixelFormat.BC5_UNorm;
			case TextureFormat.BC6H: return PixelFormat.BC6H_SF16;
			case TextureFormat.BC7: return PixelFormat.BC7_UNorm;//BC7_UNorm_SRGB

			case TextureFormat.R16F: return PixelFormat.Float16R;
			case TextureFormat.RGBA16F: return PixelFormat.Float16RGBA;
			case TextureFormat.R32F: return PixelFormat.Float32R;
			case TextureFormat.RGBA32F: return PixelFormat.Float32RGBA;
			case TextureFormat.RG16F: return PixelFormat.Float16GR;
			case TextureFormat.RG32F: return PixelFormat.Float32GR;
			case TextureFormat.RG11B10F: return PixelFormat.R11G11B10_Float;

			case TextureFormat.RGBA16U: return PixelFormat.ShortRGBA;

			case TextureFormat.D24S8: return PixelFormat.Depth24S8;
			case TextureFormat.D32F: return PixelFormat.Depth32F;
			}

			Log.Fatal( "GpuTexture: ConvertFormat (from TextureFormat to PixelFormat): " + format.ToString() + "." );
			return PixelFormat.Unknown;
		}

		string GetCompressedFileRealFileName()
		{
			return PathUtility.Combine( VirtualFileSystem.Directories.Project, @"Caches\Files", componentTextureVirtualFileName ) + ".dds";
		}

		void ReadCompressedFileInfo( out string hash, out Vector2I sourceFileSize, out PixelFormat sourceFileFormat, out bool fileNotExists )
		{
			hash = "";
			sourceFileSize = Vector2I.Zero;
			sourceFileFormat = PixelFormat.Unknown;
			fileNotExists = false;

			var compressedFileRealFileName = GetCompressedFileRealFileName();

			if( File.Exists( compressedFileRealFileName ) )
			{
				string cacheInfoFile = compressedFileRealFileName + ".info";
				var block = TextBlockUtility.LoadFromRealFile( cacheInfoFile, out _ );
				if( block != null )
				{
					try
					{
						hash = block.GetAttribute( "SourceFileHash" );
						sourceFileSize = Vector2I.Parse( block.GetAttribute( "SourceFileSize" ) );
						sourceFileFormat = (PixelFormat)Enum.Parse( typeof( PixelFormat ), block.GetAttribute( "SourceFileFormat" ) );
					}
					catch { }
				}
			}
			else
				fileNotExists = true;
		}

		bool WriteDDSCompressed2D( string sourceFileHash, DDSTextureTools.DDSImage.FormatEnum outputFormat, bool normalMap, out string error )
		{
			//!!!!все форматы

			string destRealFileName = GetCompressedFileRealFileName();

			//!!!!check error
			string directoryName = Path.GetDirectoryName( destRealFileName );
			if( directoryName != "" && !Directory.Exists( directoryName ) )
				Directory.CreateDirectory( directoryName );

#if !DEPLOY
			ScreenNotifications.IStickyNotificationItem notification = null;
			if( EngineApp.IsEditor )
			{
				var text = string.Format( EditorLocalization.Translate( "Texture", "Compressing \'{0}\'..." ), Path.GetFileName( loadFileNames[ 0 ] ) );
				notification = ScreenNotifications.ShowSticky( text );
			}
#endif

			try
			{
				if( !DDSTextureTools.Convert2DToDDS( loadFileNames[ 0 ], destRealFileName, outputFormat, normalMap, true, out var sourceFileSize, out var sourceFileFormat, out error ) )
					return false;

				//make .info file
				{
					string cacheInfoFile = destRealFileName + ".info";
					var block = new TextBlock();
					block.SetAttribute( "SourceFileHash", sourceFileHash );
					block.SetAttribute( "SourceFileSize", sourceFileSize.ToString() );
					block.SetAttribute( "SourceFileFormat", sourceFileFormat.ToString() );
					if( !TextBlockUtility.SaveToRealFile( block, cacheInfoFile, out error ) )
						return false;
				}
			}
			finally
			{
#if !DEPLOY
				notification?.Close();
#endif
			}

			error = "";
			return true;
		}

		void UpdateTextureTypeFromNativeObject()
		{
			if( nativeObject.IsCubeMap )
				textureType = ImageComponent.TypeEnum.Cube;
			else if( nativeObject.Depth != 1 )
				textureType = ImageComponent.TypeEnum._3D;
			else
				textureType = ImageComponent.TypeEnum._2D;
		}

		bool LoadResultData( string fileName, bool isVirtualFileName, out string error )
		{
			byte[] data;
			if( isVirtualFileName )
				data = VirtualFile.ReadAllBytes( fileName );
			else
				data = File.ReadAllBytes( fileName );

			var memory = MemoryBlock.FromArray( data );
			nativeObject = Texture.FromFile( memory, TextureFlags.None, 0 );
			//SharpBgfx bug fix
			if( nativeObject != null && TextureType == ImageComponent.TypeEnum.Cube )
				nativeObject.IsCubeMap = true;
			nativeObjectLastUsedTime = EngineApp.EngineTime;

			if( nativeObject == null )
			{
				error = "Unable to read texture data.";
				return false;
			}

			UpdateTextureTypeFromNativeObject();
			resultSize = new Vector2I( nativeObject.Width, nativeObject.Height );
			resultDepth = nativeObject.Depth;
			resultFormat = ConvertFormat( nativeObject.Format );
			resultMipLevels = nativeObject.MipLevels;
			mipmaps = nativeObject.MipLevels > 1;
			usage = Usages.Static;

			error = "";
			return true;
		}

		bool LoadResultDataCube4x3( string fileName, bool isVirtualFileName, out string error )
		{
			byte[] sourceData;
			Vector2I sourceSize;
			PixelFormat sourceFormat;
			if( isVirtualFileName )
			{
				if( !ImageUtility.LoadFromVirtualFile( fileName, out sourceData, out sourceSize, out _, out sourceFormat, out var faces, out var numMipmaps, out error ) )
					return false;
			}
			else
			{
				if( !ImageUtility.LoadFromRealFile( fileName, out sourceData, out sourceSize, out _, out sourceFormat, out var faces, out var numMipmaps, out error ) )
					return false;
			}

			var sourceSizeF = sourceSize.ToVector2F();
			if( Math.Abs( sourceSizeF.X / sourceSizeF.Y - 1.333333333333333f ) > 0.01f )
			{
				error = "The size must be 4x3.";
				return false;
			}
			int size = sourceSize.X / 4;
			if( size * 4 != sourceSize.X || size * 3 != sourceSize.Y )
			{
				error = "The size must be 4x3.";
				return false;
			}

			var sourceImage = new ImageUtility.Image2D( sourceFormat, sourceSize, sourceData );

			var format = sourceImage.Format;
			if( format == PixelFormat.R8G8B8 )
				format = PixelFormat.A8R8G8B8;

			var pixelSize = PixelFormatUtility.GetNumElemBytes( format );
			byte[] data = new byte[ size * size * pixelSize * 6 ];

			try
			{
				var faceImage = new ImageUtility.Image2D( format, new Vector2I( size, size ) );

				for( int face = 0; face < 6; face++ )
				{
					Vector2I index = Vector2I.Zero;
					switch( face )
					{
					case 1: index = new Vector2I( 2, 1 ); break;
					case 0: index = new Vector2I( 0, 1 ); break;
					case 2: index = new Vector2I( 1, 0 ); break;
					case 3: index = new Vector2I( 1, 2 ); break;
					case 4: index = new Vector2I( 1, 1 ); break;
					case 5: index = new Vector2I( 3, 1 ); break;
					}

					faceImage.Blit( Vector2I.Zero, sourceImage, new Vector2I( size, size ) * index );

					Array.Copy( faceImage.Data, 0, data, size * size * pixelSize * face, faceImage.Data.Length );
				}


				//faceImage.Blit( Vector2I.Zero, sourceImage, new Vector2I( size, size ) );

				//for( int n = 0; n < 6; n++ )
				//{
				//	Array.Copy( faceImage.Data, 0, data, size * size * pixelSize * n, faceImage.Data.Length );
				//}

			}
			catch( Exception ex )
			{
				error = ex.Message;
				return false;
			}

			var memory = MemoryBlock.FromArray( data );
			nativeObject = Texture.CreateCube( size, false, 1, ConvertFormat( format ), TextureFlags.None, memory );
			//SharpBgfx bug fix
			if( nativeObject != null && TextureType == ImageComponent.TypeEnum.Cube )
				nativeObject.IsCubeMap = true;
			nativeObjectLastUsedTime = EngineApp.EngineTime;

			if( nativeObject == null )
			{
				error = "Unable to read texture data.";
				return false;
			}

			UpdateTextureTypeFromNativeObject();
			resultSize = new Vector2I( nativeObject.Width, nativeObject.Height );
			resultDepth = nativeObject.Depth;
			resultFormat = ConvertFormat( nativeObject.Format );
			resultMipLevels = nativeObject.MipLevels;
			mipmaps = nativeObject.MipLevels > 1;
			usage = Usages.Static;

			error = "";
			return true;
		}

		string CalculateSourceFileHash()
		{
			string result = "";

			foreach( var fileName in loadFileNames )
			{
				string hash = "";

				try
				{
					if( VirtualFile.Exists( fileName ) )
					{
						var data = VirtualFile.ReadAllBytes( fileName );
						//!!!!optimization: faster method maybe
						//!!!!optimization: может один раз создавать для всего?
						using( var sha = new SHA256Managed() )
						{
							byte[] checksum = sha.ComputeHash( data );
							hash = BitConverter.ToString( checksum ).Replace( "-", String.Empty );
						}
					}
				}
				catch { }

				if( result != "" )
					result += "_";
				result += hash;
			}

			return result;
		}

		bool ReadSourceData( out Vector2I size, out PixelFormat format, out byte[][] data, out string error )
		{
			//out int depth,

			//int firstSize = 0;
			//PixelFormat firstFormat = PixelFormat.Unknown;

			if( loadFileNames.Length == 1 )// textureType == Texture.TypeEnum._2D )
			{
				if( !ImageUtility.LoadFromVirtualFile( loadFileNames[ 0 ], out var data2, out size, out var depth, out format, out var faces, out var mips, out error ) )
				{
					data = null;
					return false;
				}

				data = new byte[ 1 ][] { data2 };
				error = "";
				return true;
			}
			else if( loadFileNames.Length == 6 ) //if( textureType == Texture.TypeEnum.Cube )
			{
				size = Vector2I.Zero;
				format = PixelFormat.Unknown;
				data = new byte[ 6 ][];

				for( int n = 0; n < 6; n++ )
				{
					if( !ImageUtility.LoadFromVirtualFile( loadFileNames[ n ], out var data2, out var size2, out var depth, out var format2, out var faces, out var mips, out error ) )
					{
						size = Vector2I.Zero;
						format = PixelFormat.Unknown;
						data = null;
						return false;
					}

					data[ n ] = data2;

					if( n == 0 )
					{
						size = size2;
						format = format2;
					}
					else
					{
						//!!!!проверить чтобы все одного размера были
						//!!!!чтобы квадратные?
						//!!!!формат тот же
					}
				}

				error = "";
				return true;
			}
			else
			{
				size = Vector2I.Zero;
				format = PixelFormat.Unknown;
				data = null;
				error = "Internal error. ReadSourceData.";
				return false;
			}
		}

		//2D (1 source data item), Cube map support (6 source data items)
		static ICollection<byte[]> GenerateMipmaps( byte[][] sourceData, Vector2I size, PixelFormat format )
		{
			var resultData = new List<byte[]>();

			foreach( var sourceData2 in sourceData )
			{
				resultData.Add( sourceData2 );

				var currentData = sourceData2;
				Vector2I currentSize = size;
				var nextSize = currentSize / 2;

				//!!!!new 
				while( nextSize.X > 0 || nextSize.Y > 0 )
				//while( nextSize.X > 0 && nextSize.Y > 0 )
				{
					//!!!!new
					if( nextSize.X == 0 )
						nextSize.X = 1;
					if( nextSize.Y == 0 )
						nextSize.Y = 1;

					ImageUtility.Scale( currentData, currentSize, format, nextSize, ImageUtility.Filters.Bicubic, out var data );
					resultData.Add( data );

					currentData = data;
					currentSize = nextSize;
					nextSize = currentSize / 2;
				}
			}

			return resultData;

			//generate mips from original size
			//List<byte[]> newData = new List<byte[]>();
			//newData.Add( sourceData[ 0 ] );
			//Vec2I currentSize = resultSize / 2;
			//while( currentSize.X > 0 && currentSize.Y > 0 )
			//{
			//	ImageManager.Scale( sourceData[ 0 ], resultSize, resultFormat, currentSize, ImageManager.Filters.Bicubic, out var newDataItem );
			//	newData.Add( newDataItem );
			//	currentSize /= 2;
			//}
		}

		static byte[] MergeByteArrays( ICollection<byte[]> arrays )
		{
			int totalSize = 0;
			foreach( var data in arrays )
				totalSize += data.Length;

			var totalData = new byte[ totalSize ];
			int pos = 0;
			foreach( var data in arrays )
			{
				Buffer.BlockCopy( data, 0, totalData, pos, data.Length );
				pos += data.Length;
			}

			return totalData;
		}

		bool Load( out string error )
		{
			EngineThreading.CheckMainThread();

			if( nativeObject != null )
				Log.Fatal( "Texture: Load: nativeObject != null." );

			nativeObjectUnloadSupport = true;

			//!!!!все форматы

			try
			{
				//load without compression support (DDS)
				if( loadFileNames.Length == 1 && Path.GetExtension( loadFileNames[ 0 ] ).ToLower() == ".dds" )
				{
					if( !LoadResultData( loadFileNames[ 0 ], true, out error ) )
						return false;
					sourceSize = resultSize;
					sourceFormat = resultFormat;
					error = "";
					return true;
				}

				//!!!!compression support? mipmaps
				//load cube 4x3
				if( loadFileNames.Length == 1 && loadFileCube4x3 )
				{
					if( !LoadResultDataCube4x3( loadFileNames[ 0 ], true, out error ) )
						return false;
					sourceSize = resultSize;
					sourceFormat = resultFormat;
					error = "";
					return true;
				}

				string sourceFileHash = CalculateSourceFileHash();
				if( string.IsNullOrEmpty( sourceFileHash ) )
				{
					error = "File is not exists.";
					return false;
				}

				string compressedFileRealFileName = GetCompressedFileRealFileName();
				ReadCompressedFileInfo( out var currentCompressedHash, out var currentCompressedSourceFileSize, out var currentCompressedSourceFileFormat, out var fileNotExists );

				bool forceNoCompression = false;
				if( fileNotExists )
				{
					//!!!!по сути для .Windows тоже если задеплоен и read only. хотя может быть не read only.
					//!!!!!опцией продукта?

					//!!!!Android, iOS, Web readonly?
					bool readOnly = SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP || SystemSettings.CurrentPlatform == SystemSettings.Platform.Android || SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web;
					if( readOnly )
						forceNoCompression = true;
				}

				if( !forceNoCompression )
				{
					bool needUpdateCompression = string.IsNullOrEmpty( currentCompressedHash ) || sourceFileHash != currentCompressedHash;

					//load compressed file
					if( !needUpdateCompression )
					{
						if( !LoadResultData( compressedFileRealFileName, false, out error ) )
							return false;
						sourceSize = currentCompressedSourceFileSize;
						sourceFormat = currentCompressedSourceFileFormat;
						error = "";
						return true;
					}
				}

				//update compressed file or no compression mode

				//delete old compressed file
				if( !forceNoCompression )
				{
					if( File.Exists( compressedFileRealFileName ) )
						File.Delete( compressedFileRealFileName );
					var infoFile = compressedFileRealFileName + ".info";
					if( File.Exists( infoFile ) )
						File.Delete( infoFile );
				}

				if( !ReadSourceData( out sourceSize, out sourceFormat, out var sourceData, out error ) )
					return false;

				//auto rescale
				var allowedSize = RenderingSystem.LimitTextureSize;
				if( allowedSize > 0 && sourceSize.MaxComponent() > allowedSize )
				{
					var newSizeF = sourceSize.ToVector2();
					if( newSizeF.X > allowedSize )
						newSizeF *= ( (double)allowedSize ) / newSizeF.X;
					if( newSizeF.Y > allowedSize )
						newSizeF *= ( (double)allowedSize ) / newSizeF.Y;
					var newSize = newSizeF.ToVector2I();

					var newData = new byte[ sourceData.Length ][];

					for( int n = 0; n < sourceData.Length; n++ )
					{
						ImageUtility.Scale( sourceData[ n ], sourceSize, sourceFormat, newSize, ImageUtility.Filters.Bicubic, out var newData2 );
						newData[ n ] = newData2;
					}

					sourceData = newData;
					sourceSize = newSize;
				}

				string useCompression;
				{
					if( !forceNoCompression )
					{
						useCompression = ImageSettingsFile.GetParameter( componentTextureVirtualFileName, "Compression", out _ );
						if( useCompression == "Auto" || useCompression == "" )
						{
							//Auto

							//!!!!no cubemap support. loadFileNames.Length == 1
							if( loadFileNames.Length == 1 )
								useCompression = ImageUtility.ImageAutoCompressionDetectType( sourceData[ 0 ], sourceSize, sourceFormat, Path.GetFileName( componentTextureVirtualFileName ) );
							else
								useCompression = "NoCompression";
						}
					}
					else
						useCompression = "NoCompression";
				}

				//!!!!
				if( loadFileNames.Length == 6 && useCompression != "NoCompression" )
				{
					useCompression = "NoCompression";
					Log.Warning( "Texture: Compression for cubemaps is not supported. No implementation." );
				}

				if( useCompression == "NoCompression" )
				{
					//no compression

					resultSize = sourceSize;
					resultFormat = sourceFormat;

					//convert R8G8B8 to A8R8G8B8
					if( sourceFormat == PixelFormat.R8G8B8 )
					{
						for( int nData = 0; nData < sourceData.Length; nData++ )
						{
							var data = sourceData[ nData ];
							var newData = new byte[ sourceSize.X * sourceSize.Y * 4 ];
							int destPos = 0;
							for( int sourcePos = 0; sourcePos < data.Length; sourcePos += 3 )
							{
								newData[ destPos++ ] = data[ sourcePos + 0 ];
								newData[ destPos++ ] = data[ sourcePos + 1 ];
								newData[ destPos++ ] = data[ sourcePos + 2 ];
								newData[ destPos++ ] = 255;
							}
							sourceData[ nData ] = newData;
						}
						resultFormat = PixelFormat.A8R8G8B8;
					}

					//convert Float32RGB to Float32RGBA
					if( sourceFormat == PixelFormat.Float32RGB )
					{
						for( int nData = 0; nData < sourceData.Length; nData++ )
						{
							var data = sourceData[ nData ];

							unsafe
							{
								fixed( byte* pData2 = data )
								{
									float* pData = (float*)pData2;

									var newData = new byte[ sourceSize.X * sourceSize.Y * 4 * 4 ];
									fixed( byte* pNewData2 = newData )
									{
										float* pNewData = (float*)pNewData2;

										int destPos = 0;
										for( int sourcePos = 0; sourcePos < data.Length / 4; sourcePos += 3 )
										{
											pNewData[ destPos++ ] = pData[ sourcePos + 0 ];
											pNewData[ destPos++ ] = pData[ sourcePos + 1 ];
											pNewData[ destPos++ ] = pData[ sourcePos + 2 ];
											pNewData[ destPos++ ] = 1.0f;
										}
									}

									sourceData[ nData ] = newData;
								}
							}
						}
						resultFormat = PixelFormat.Float32RGBA;
					}

					//convert Short16RGB to Float16RGBA
					if( sourceFormat == PixelFormat.ShortRGB )
					{
						for( int nData = 0; nData < sourceData.Length; nData++ )
						{
							var data = sourceData[ nData ];

							unsafe
							{
								fixed( byte* pData2 = data )
								{
									ushort* pData = (ushort*)pData2;

									var newData = new byte[ sourceSize.X * sourceSize.Y * 4 * 2 ];
									fixed( byte* pNewData2 = newData )
									{
										HalfType* pNewData = (HalfType*)pNewData2;

										int destPos = 0;
										for( int sourcePos = 0; sourcePos < data.Length / 2; sourcePos += 3 )
										{
											pNewData[ destPos++ ] = new HalfType( (float)pData[ sourcePos + 0 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new HalfType( (float)pData[ sourcePos + 1 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new HalfType( (float)pData[ sourcePos + 2 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new HalfType( 1.0f );
										}
									}

									sourceData[ nData ] = newData;
								}
							}
						}
						resultFormat = PixelFormat.Float16RGBA;
					}

					if( loadFileNames.Length == 1 )// textureType == Texture.TypeEnum._2D )
					{
						//!!!!можно сразу в конечный массив записывать
						var newData = GenerateMipmaps( sourceData, resultSize, resultFormat );
						var totalData = MergeByteArrays( newData );

						var memory = MemoryBlock.FromArray( totalData );
						nativeObject = Texture.Create2D( resultSize.X, resultSize.Y, true, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );
						nativeObjectLastUsedTime = EngineApp.EngineTime;
					}
					else if( loadFileNames.Length == 6 )
					{
						//!!!!можно сразу в конечный массив записывать
						var newData = GenerateMipmaps( sourceData, resultSize, resultFormat );
						var totalData = MergeByteArrays( newData );
						//var totalData = MergeByteArrays( sourceData );

						var memory = MemoryBlock.FromArray( totalData );
						nativeObject = Texture.CreateCube( resultSize.X, true, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );
						nativeObjectLastUsedTime = EngineApp.EngineTime;
						//realObject = Texture.CreateCube( resultSize.X, false, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );

						//SharpBgfx bug fix
						if( nativeObject != null )
							nativeObject.IsCubeMap = true;
					}

					if( nativeObject == null )
						return false;

					//get properties
					UpdateTextureTypeFromNativeObject();
					//resultSize = sourceSize;
					sourceDepth = nativeObject.Depth;
					//resultFormat = sourceFormat;
					mipmaps = nativeObject.MipLevels > 1;
					usage = Usages.Static;
					return true;
				}
				else
				{
					//compression

					if( textureType == ImageComponent.TypeEnum._2D )
					{
						DDSTextureTools.DDSImage.FormatEnum ddsFormat = DDSTextureTools.DDSImage.FormatEnum.DXT1;
						bool normalMap = false;
						switch( useCompression )
						{
						case "DXT1": ddsFormat = DDSTextureTools.DDSImage.FormatEnum.DXT1; break;
						case "DXT5": ddsFormat = DDSTextureTools.DDSImage.FormatEnum.DXT5; break;
						case "NormalMap": ddsFormat = DDSTextureTools.DDSImage.FormatEnum.BC5; normalMap = true; break;
						}

						//!!!! need to fix for UWP ! write access only in app data/temp etc.
						if( !WriteDDSCompressed2D( sourceFileHash, ddsFormat, normalMap, out error ) )
							return false;
					}
					else
					{
						//!!!!
					}

					if( !LoadResultData( compressedFileRealFileName, false, out error ) )
						return false;
					return true;
				}

			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
		}

		protected unsafe override void OnDispose()
		{
			if( nativeObject != null )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "GpuTexture: Dispose after shutdown." );

				EngineThreading.ExecuteFromMainThreadLater( delegate ( Texture nativeObject2, RenderTargetItem[] renderTargets2 )
				{
					foreach( var target in renderTargets2 )
						target.RenderTarget.DisposeInternal();
					nativeObject2.Dispose();
				}, nativeObject, renderTargets.ToArray() );

				nativeObject = null;
				renderTargets.Clear();

				lock( all )
					all.Remove( this );
			}

			//base.OnDispose();
		}

		/// <summary>Gets the source size of the texture.</summary>
		[Description( "The source size of the texture." )]
		public Vector2I SourceSize
		{
			get { return sourceSize; }
		}

		/// <summary>Gets the result size of the texture (after processing).</summary>
		[Description( "The result size of the texture (after processing)." )]
		public Vector2I ResultSize
		{
			get { return resultSize; }
		}

		/// <summary>Gets the source depth of the texture.</summary>
		[Description( "The source depth of the texture." )]
		public int SourceDepth
		{
			get { return sourceDepth; }
		}

		/// <summary>Gets the result depth of the texture.</summary>
		[Description( "The result depth of the texture." )]
		public int ResultDepth
		{
			get { return resultDepth; }
		}

		/// <summary>Gets the source pixel format for the texture surface.</summary>
		[Description( "The source format of the texture." )]
		public PixelFormat SourceFormat
		{
			get { return sourceFormat; }
		}

		/// <summary>Gets the result format of the texture (after processing).</summary>
		[Description( "The result format of the texture (after processing)." )]
		public PixelFormat ResultFormat
		{
			get { return resultFormat; }
		}

		public int ResultMipLevels
		{
			get { return resultMipLevels; }
		}

		public bool Mipmaps
		{
			get { return mipmaps; }
		}

		///// <summary>Gets the number of mipmaps to be used for this texture.</summary>
		//[Description( "The number of mipmaps to be used for this texture." )]
		//public int MipmapCount
		//{
		//	get { return mipmapCount; }
		//}

		//public int GetSizeInBytes()
		//{
		//	unsafe
		//	{
		//		return OgreTexturePtr.getSize( realObjectPtr );
		//	}
		//}

		public Usages Usage
		{
			get { return usage; }
		}

		public int Depth
		{
			get { return sourceDepth; }
		}

		public int ArrayLayers
		{
			get { return arrayLayers; }
		}

		public ImageComponent.TypeEnum TextureType
		{
			get { return textureType; }
			set { textureType = value; }
		}

		////!!!!это в capatibilities. хотя их тоже может перенести куда-нибудь попроще
		///// <summary>
		///// Returns whether this render system can natively support the precise texture 
		///// format requested with the given usage options.
		///// </summary>
		///// <remarks>
		///// <para>
		///// You can still create textures with this format even if this method returns
		///// <b>false</b>; the texture format will just be altered to one which the device does
		///// support.
		///// </para>
		///// </remarks>
		///// <param name="textureType">The texture type.</param>
		///// <param name="format">The pixel format requested.</param>
		///// <param name="usage">
		///// The kind of usage this texture is intended for, a combination of the 
		///// <b>Texture.Usage</b> flags.
		///// </param>
		///// <returns>
		///// <b>true</b> if the format is natively supported, <b>false</b> if a fallback would be used.
		///// </returns>
		//public static bool IsFormatSupported( Texture.TypeEnum textureType, PixelFormat format, GpuTexture.Usages usage )
		//{
		//	unsafe
		//	{
		//		return OgreTextureManager.isFormatSupported( RendererWorld.realRoot, textureType, format, (int)usage );
		//	}
		//}

		////!!!!static юзать?
		///// <summary>
		///// Returns whether this render system can support the texture format requested
		///// with the given usage options, or another format with no quality reduction.
		///// </summary>
		///// <param name="textureType">The texture type.</param>
		///// <param name="format">The pixel format requested.</param>
		///// <param name="usage">
		///// The kind of usage this texture is intended for, a combination of the 
		///// <b>Texture.Usage</b> flags.
		///// </param>
		///// <returns>
		///// <b>true</b> if the format is natively supported, <b>false</b> if a fallback would be used.
		///// </returns>
		//public static bool IsEquivalentFormatSupported( Texture.TypeEnum textureType, PixelFormat format, GpuTexture.Usages usage )
		//{
		//	unsafe
		//	{
		//		return OgreTextureManager.isEquivalentFormatSupported( RendererWorld.realRoot, textureType, format, (int)usage );
		//	}
		//}

		//!!!!
		//public void _SaveToFile( string fileName )
		//{
		//	//!!!!!
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		OgreTexturePtr.saveToFile( realObjectPtr, fileName );
		//	}
		//}

		public RenderTexture GetRenderTarget( int mip = 0, int slice = 0 )
		{
			//commented to optimize
			//EngineThreading.CheckMainThread();

			if( nativeObject == null )
				return null;

			foreach( var item2 in renderTargets )
			{
				if( item2.Mip == mip && item2.Slice == slice )
					return item2.RenderTarget;
			}

			var sizeF = new Vector2F( ResultSize.X, ResultSize.Y );
			sizeF /= (float)Math.Pow( 2.0, mip );
			//sizeF.X = (float)Math.Round((double)sizeF.X);
			//sizeF.Y = (float)Math.Round((double)sizeF.Y);
			sizeF.X += 0.5f;
			sizeF.Y += 0.5f;
			var newSize = new Vector2I( (int)sizeF.X, (int)sizeF.Y );

			if( newSize.X < 1 || newSize.Y < 1 )
				return null;

			////!!!!
			//var buffer = new FrameBuffer( new Texture[] { realObject } );

			var attachment = new Attachment();
			attachment.Texture = nativeObject;
			attachment.Mip = mip;
			attachment.Layer = slice;
			//!!!!
			attachment.NumLayers = 1;
			attachment.Access = ComputeBufferAccess.Write;
			var attachments = new Attachment[] { attachment };
			var buffer = new FrameBuffer( attachments ); // create bgfx framebuffer with attachment

			//var sizeF = new Vector2F( (float)ResultSize.X, (float)ResultSize.Y );
			//sizeF /= (float)Math.Pow( 2.0, (double)mip );
			////sizeF.X = (float)Math.Round((double)sizeF.X);
			////sizeF.Y = (float)Math.Round((double)sizeF.Y);
			//sizeF.X += 0.5f;
			//sizeF.Y += 0.5f;
			//var newSize = new Vector2I( (int)sizeF.X, (int)sizeF.Y );

			var target = new RenderTexture( buffer, newSize, this );

			var item = new RenderTargetItem( mip, slice, target );
			renderTargets.Add( item );
			return target;
		}

		internal unsafe void PrepareNativeObject()
		{
			if( Disposed )
				return;

			if( mode == ModeEnum.Create )
			{
				if( nativeObject == null )
					CreateNativeObject();

				if( nativeObject != null && needUpdateNative )
				{
					var d = data;// newData.Set( null );
					if( d != null )
					{
						needUpdateNative = false;

						//write to native
						foreach( var item in d )
						{
							switch( TextureType )
							{
							case ImageComponent.TypeEnum._2D:
								{
									//!!!!
									int pitch = ushort.MaxValue;


									//!!!!var size =  как в _3D


									var memory = MemoryBlock.FromArray( item.Data );
									//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( item.Data );
									nativeObject.Update2D( item.ArrayLayer, item.MipLevel, 0, 0, ResultSize.X, ResultSize.Y, memory, pitch );
								}
								break;

							case ImageComponent.TypeEnum.Cube:
								{
									//!!!!check face order

									//!!!!
									int pitch = ushort.MaxValue;


									//!!!!var size =  как в _3D


									var memory = MemoryBlock.FromArray( item.Data );
									//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( item.Data );
									nativeObject.UpdateCube( (CubeMapFace)item.Face, item.ArrayLayer, item.MipLevel, 0, 0, ResultSize.X, ResultSize.Y, memory, pitch );
								}
								break;

							case ImageComponent.TypeEnum._3D:
								{
									var memory = MemoryBlock.FromArray( item.Data );

									var size = new Vector3I( ResultSize, ResultDepth );
									for( int n = 0; n < item.MipLevel; n++ )
										size /= 2;

									nativeObject.Update3D( item.MipLevel, 0, 0, 0, size.X, size.Y, size.Z, memory );
								}
								break;

							default:
								Log.Fatal( "GpuTexture: PrepareNativeObject: TextureType impl." );
								break;
							}
						}

						//foreach( var pair in d )
						//{
						//	var buffer = GetBuffer( pair.Key.face, pair.Key.mipmap );
						//	if( buffer != null )
						//		buffer.SetData( pair.Value );
						//}
					}
				}
			}
		}

		public void SetData( SurfaceData[] data, bool prepareNativeObject = true )
		{
			if( mode != ModeEnum.Create )
				Log.Fatal( "GpuTexture: SetData: Unable to set data for loaded or render target textures." );

			this.data = data;
			needUpdateNative = true;
			//newData.Set( data );

			if( prepareNativeObject )
				PrepareNativeObject();
		}

		/// <summary>
		/// Returns data that was initialized by the SetData method.
		/// </summary>
		/// <returns></returns>
		public SurfaceData[] GetData()
		{
			return data;
		}

		public ModeEnum Mode
		{
			get { return mode; }
		}

		//!!!!было
		//public IntPtr CallCustomMethod( string message, IntPtr param )
		//{
		//	//!!!!?
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		return OgreTexturePtr.callCustomMethod( realObjectPtr, message, param );
		//	}
		//}

		public Texture GetNativeObject( bool withUpdate )
		{
			if( nativeObject == null && withUpdate && nativeObjectUnloaded )
				RestoreUnloaded();

			if( withUpdate )
				nativeObjectLastUsedTime = EngineApp.EngineTime;
			return nativeObject;
		}

		public static GpuTexture[] GetAll()
		{
			lock( all )
				return all.ToArray();
		}

		void RestoreUnloaded()
		{
			if( !Load( out var error ) )
			{
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( $"Unable to reload texture. " + error );
					//Log.Warning( $"Unable to reload texture \'{loadFromOneFile}\'. " + error );
				}
			}

			nativeObjectUnloaded = false;
		}

		public void Unload()
		{
			if( nativeObject != null )
			{
				nativeObject.Dispose();
				//EngineThreading.ExecuteFromMainThreadLater( delegate ( Texture realObject2, List<RenderTargetItem> renderTargets2 )
				//{
				//	foreach( var target in renderTargets2 )
				//		target.RenderTarget.DisposeInternal();
				//	realObject2.Dispose();
				//}, realObject, renderTargets );

				nativeObject = null;
			}

			nativeObjectUnloaded = true;
		}

		//public static Random testRandom = new Random();

		/// <summary>
		/// A method to temporary unload textures which are not used long time.
		/// </summary>
		/// <param name="howLongHasNotBeenUsedInSeconds"></param>
		public static void UnloadNotUsedForLongTime( double howLongHasNotBeenUsedInSeconds )
		{
			foreach( var texture in GetAll() )
			{
				if( texture.nativeObjectUnloadSupport && !texture.nativeObjectUnloaded )
				{
					if( EngineApp.EngineTime - texture.nativeObjectLastUsedTime > howLongHasNotBeenUsedInSeconds )
					{
						//if( testRandom.NextDouble() > 0.5 )
						texture.Unload();
					}
				}
			}
		}

		public static void UnloadAllUnloadable()
		{
			UnloadNotUsedForLongTime( -1 );
		}
	}
}
