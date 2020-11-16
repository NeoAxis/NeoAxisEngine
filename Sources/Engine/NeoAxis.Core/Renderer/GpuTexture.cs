// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using SharpBgfx;
using System.Security.Cryptography;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a texture.
	/// </summary>
	public class GpuTexture : ThreadSafeDisposable
	{
		static ESet<GpuTexture> instances = new ESet<GpuTexture>();

		//constants
		public enum ModeEnum
		{
			Load,
			Create,
			RenderTarget,
		}
		ModeEnum mode;
		string[] loadFileNames;
		string componentTextureVirtualFileName;

		Component_Image.TypeEnum textureType = Component_Image.TypeEnum._2D;
		Vector2I sourceSize;
		Vector2I resultSize;
		int depth;
		PixelFormat sourceFormat = PixelFormat.Unknown;
		PixelFormat resultFormat = PixelFormat.Unknown;
		bool mipmaps = true;
		int arrayLayers;
		Usages usage;
		int fullSceneAntialiasing;

		Texture realObject;
		bool realObjectUnloadSupport;
		bool realObjectUnloaded;
		double realObjectLastUsedTime;

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
			public int face;
			public int mip;
			public byte[] data;

			public SurfaceData( int face, int mip, byte[] data )
			{
				this.face = face;
				this.mip = mip;
				this.data = data;
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

		unsafe internal GpuTexture( Component_Image.TypeEnum type, Vector2I size, int depth, bool mipmaps, int arrayLayers, PixelFormat format,
			Usages usage, int fullSceneAntialiasing, out string error )
		{
			error = "";

			this.textureType = type;
			sourceSize = size;
			resultSize = size;
			this.depth = depth;
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

				if( realObject == null )
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

			lock( instances )
				instances.Add( this );
		}

		void CreateNativeObject()
		{
			EngineThreading.CheckMainThread();

			if( realObject != null )
				Log.Fatal( "Texture: Create: The texture data is already initialized. realObject != null." );

			//!!!!
			//fullSceneAntialiasing
			//что еще?

			TextureFormat realFormat = ConvertFormat( ResultFormat );

			TextureFlags flags = TextureFlags.None;
			if( ( usage & Usages.RenderTarget ) != 0 )
			{
				//!!!!
				//if( ( usage & Usages.WriteOnly ) != 0 )
				//flags |= TextureFlags.RenderTargetWriteOnly;
				//else
				flags |= TextureFlags.RenderTarget;
			}

			if( ( usage & Usages.BlitDestination ) != 0 )
				flags |= TextureFlags.BlitDestination;

			if( ( usage & Usages.ReadBack ) != 0 )
				flags |= TextureFlags.ReadBack;

			//!!!!
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
			case Component_Image.TypeEnum._2D:
				realObject = Texture.Create2D( resultSize.X, resultSize.Y, mipmaps, arrayLayers, realFormat, flags );
				realObjectLastUsedTime = EngineApp.EngineTime;
				break;

			case Component_Image.TypeEnum._3D:
				realObject = Texture.Create3D( resultSize.X, resultSize.Y, depth, mipmaps, realFormat, flags );
				realObjectLastUsedTime = EngineApp.EngineTime;
				break;

			case Component_Image.TypeEnum.Cube:
				realObject = Texture.CreateCube( resultSize.X, mipmaps, arrayLayers, realFormat, flags );
				realObjectLastUsedTime = EngineApp.EngineTime;
				break;
			}
		}

		GpuTexture( string[] loadFileNames, string componentTextureVirtualFileName )
		{
			mode = ModeEnum.Load;
			this.loadFileNames = loadFileNames;
			this.componentTextureVirtualFileName = componentTextureVirtualFileName;

			lock( instances )
				instances.Add( this );
		}

		public static GpuTexture CreateFromFile( string virtualFileName, out string error )
		{
			var result = new GpuTexture( new string[] { virtualFileName }, virtualFileName );

			//detect type of DDS texture
			if( Path.GetExtension( virtualFileName ) == ".dds" )
			{
				//!!!!slowly?
				if( ImageUtility.GetImageFlags( virtualFileName, out ImageUtility.ImageFlags flags, out error ) )
				{
					if( flags.HasFlag( ImageUtility.ImageFlags.Cubemap ) )
						result.TextureType = Component_Image.TypeEnum.Cube;
					else if( flags.HasFlag( ImageUtility.ImageFlags.Texture3D ) )
						result.TextureType = Component_Image.TypeEnum._3D;
					else
						result.TextureType = Component_Image.TypeEnum._2D;
				}
			}

			if( !result.Load( out error ) )
			{
				result.Dispose();
				result = null;
			}
			return result;
		}

		public static GpuTexture CreateVolume( string virtualFileName, out string error )
		{
			//!!!!
			throw new NotImplementedException();
		}

		//// name + postfixes or DDS.
		//public static GpuTexture CreateCube( string virtualFileName, out string error )
		//{
		//	var result = new GpuTexture( virtualFileName );
		//	result.TextureType = Component_Texture.TypeEnum.Cube;

		//	if( !result.Load( out error ) )
		//	{
		//		result.Dispose();
		//		result = null;
		//	}
		//	return result;
		//}

		// unique names
		public static GpuTexture CreateCube( string[] virtualFileNames, string componentTextureVirtualFileName, out string error )
		{
			var result = new GpuTexture( virtualFileNames, componentTextureVirtualFileName );
			result.TextureType = Component_Image.TypeEnum.Cube;

			if( !result.Load( out error ) )
			{
				result.Dispose();
				result = null;
			}
			return result;
		}

		static TextureFormat ConvertFormat( PixelFormat format )
		{
			//!!!!все форматы

			switch( format )
			{
			case PixelFormat.Unknown: return TextureFormat.Unknown;
			case PixelFormat.L8: return TextureFormat.R8;
			case PixelFormat.L16: return TextureFormat.R16;
			case PixelFormat.A8: return TextureFormat.A8;
			case PixelFormat.R8G8_UInt: return TextureFormat.RG8;

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

			//!!!!new
			case PixelFormat.ShortRGBA: return TextureFormat.RGBA16U;

			case PixelFormat.Depth24S8: return TextureFormat.D24S8;
			}

			Log.Fatal( "GpuTexture: ConvertFormat (from PixelFormat to TextureFormat): " + format.ToString() + "." );
			return TextureFormat.Unknown;
		}

		static PixelFormat ConvertFormat( TextureFormat format )
		{
			switch( format )
			{
			case TextureFormat.Unknown: return PixelFormat.Unknown;

			case TextureFormat.R8: return PixelFormat.L8;
			case TextureFormat.R16: return PixelFormat.L16;
			case TextureFormat.A8: return PixelFormat.A8;
			case TextureFormat.RG8: return PixelFormat.R8G8_UInt;

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

			case TextureFormat.D24S8: return PixelFormat.Depth24S8;

			//!!!!new
			case TextureFormat.RGBA16U: return PixelFormat.ShortRGBA;

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

			ScreenNotifications.StickyNotificationItem notification = null;
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				var text = string.Format( EditorLocalization.Translate( "Texture", "Compressing \'{0}\'..." ), Path.GetFileName( loadFileNames[ 0 ] ) );
				notification = Editor.ScreenNotifications.ShowSticky( text );
			}

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
				notification?.Close();
			}

			error = "";
			return true;
		}

		void UpdateTextureTypeFromRealObject()
		{
			if( realObject.IsCubeMap )
				textureType = Component_Image.TypeEnum.Cube;
			else if( realObject.Depth != 1 )
				textureType = Component_Image.TypeEnum._3D;
			else
				textureType = Component_Image.TypeEnum._2D;
		}

		bool LoadResultData( string fileName, bool isVirtualFileName, out string error )
		{
			byte[] data;
			if( isVirtualFileName )
				data = VirtualFile.ReadAllBytes( fileName );
			else
				data = File.ReadAllBytes( fileName );

			//!!!!надо ли чистить? везде так
			var memory = MemoryBlock.FromArray( data );
			realObject = Texture.FromFile( memory, TextureFlags.None, 0 );
			//SharpBgfx bug fix
			if( realObject != null && TextureType == Component_Image.TypeEnum.Cube )
				realObject.IsCubeMap = true;
			realObjectLastUsedTime = EngineApp.EngineTime;

			if( realObject == null )
			{
				error = "Unable to read texture data.";
				return false;
			}

			UpdateTextureTypeFromRealObject();
			resultSize = new Vector2I( realObject.Width, realObject.Height );
			depth = realObject.Depth;
			resultFormat = ConvertFormat( realObject.Format );
			mipmaps = realObject.MipLevels > 1;
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

			if( loadFileNames.Length == 1 )// textureType == Component_Texture.TypeEnum._2D )
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
			else if( loadFileNames.Length == 6 ) //if( textureType == Component_Texture.TypeEnum.Cube )
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

			if( realObject != null )
				Log.Fatal( "Texture: Load: realObject != null." );

			realObjectUnloadSupport = true;

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

					//!!!!Android readonly?
					bool readOnly = SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP || SystemSettings.CurrentPlatform == SystemSettings.Platform.Android;
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

				string useCompression;
				{
					if( !forceNoCompression )
					{
						useCompression = Component_Image_Settings.GetParameter( componentTextureVirtualFileName, "Compression", out _ );
						if( useCompression == "Auto" || useCompression == "" )
						{
							//Auto

							//!!!!no cubemap support. loadFileNames.Length == 1
							if( loadFileNames.Length == 1 )
								useCompression = ImageUtility.ImageAutoCompressionDetectType( sourceData[ 0 ], sourceSize, sourceFormat );
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
										Half* pNewData = (Half*)pNewData2;

										int destPos = 0;
										for( int sourcePos = 0; sourcePos < data.Length / 2; sourcePos += 3 )
										{
											pNewData[ destPos++ ] = new Half( (float)pData[ sourcePos + 0 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new Half( (float)pData[ sourcePos + 1 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new Half( (float)pData[ sourcePos + 2 ] / ushort.MaxValue );
											pNewData[ destPos++ ] = new Half( 1.0f );
										}
									}

									sourceData[ nData ] = newData;
								}
							}
						}
						resultFormat = PixelFormat.Float16RGBA;
					}

					if( loadFileNames.Length == 1 )// textureType == Component_Texture.TypeEnum._2D )
					{
						//!!!!можно сразу в конечный массив записывать
						var newData = GenerateMipmaps( sourceData, resultSize, resultFormat );
						var totalData = MergeByteArrays( newData );

						var memory = MemoryBlock.FromArray( totalData );
						realObject = Texture.Create2D( resultSize.X, resultSize.Y, true, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );
						realObjectLastUsedTime = EngineApp.EngineTime;
					}
					else if( loadFileNames.Length == 6 )
					{
						//!!!!можно сразу в конечный массив записывать
						var newData = GenerateMipmaps( sourceData, resultSize, resultFormat );
						var totalData = MergeByteArrays( newData );
						//var totalData = MergeByteArrays( sourceData );

						var memory = MemoryBlock.FromArray( totalData );
						realObject = Texture.CreateCube( resultSize.X, true, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );
						realObjectLastUsedTime = EngineApp.EngineTime;
						//realObject = Texture.CreateCube( resultSize.X, false, 1, ConvertFormat( resultFormat ), TextureFlags.None, memory );

						//SharpBgfx bug fix
						if( realObject != null )
							realObject.IsCubeMap = true;
					}

					if( realObject == null )
						return false;

					//get properties
					UpdateTextureTypeFromRealObject();
					//resultSize = sourceSize;
					depth = realObject.Depth;
					//resultFormat = sourceFormat;
					mipmaps = realObject.MipLevels > 1;
					usage = Usages.Static;
					return true;
				}
				else
				{
					//compression

					if( textureType == Component_Image.TypeEnum._2D )
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
			if( realObject != null )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
				{
					//waiting for .NET Standard 2.0
					Log.Fatal( "Renderer: Dispose after Shutdown." );
					//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
				}

				//!!!!везде такое
				EngineThreading.ExecuteFromMainThreadLater( delegate ( Texture realObject2, List<RenderTargetItem> renderTargets2 )
				{
					foreach( var target in renderTargets2 )
						target.RenderTarget.DisposeInternal();
					realObject2.Dispose();
				}, realObject, renderTargets );

				realObject = null;
				renderTargets.Clear();

				lock( instances )
					instances.Remove( this );
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
			get { return depth; }
		}

		public Component_Image.TypeEnum TextureType
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
		//public static bool IsFormatSupported( Component_Texture.TypeEnum textureType, PixelFormat format, GpuTexture.Usages usage )
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
		//public static bool IsEquivalentFormatSupported( Component_Texture.TypeEnum textureType, PixelFormat format, GpuTexture.Usages usage )
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
			//!!!!
			EngineThreading.CheckMainThread();

			if( realObject == null )
				return null;

			foreach( var item2 in renderTargets )
			{
				if( item2.Mip == mip && item2.Slice == slice )
					return item2.RenderTarget;
			}

			////!!!!
			//var buffer = new FrameBuffer( new Texture[] { realObject } );

			var attachment = new Attachment();
			attachment.Texture = realObject;
			attachment.Mip = mip;
			attachment.Layer = slice;
			attachment.Access = ComputeBufferAccess.Write;
			var attachments = new Attachment[] { attachment };
			var buffer = new FrameBuffer( attachments ); // create bgfx framebuffer with attachment

			Vector2F sizeF = new Vector2F( (float)ResultSize.X, (float)ResultSize.Y );
			sizeF /= (float)Math.Pow( 2.0, (double)mip );
			//sizeF.X = (float)Math.Round((double)sizeF.X);
			//sizeF.Y = (float)Math.Round((double)sizeF.Y);
			sizeF.X += 0.5f;
			sizeF.Y += 0.5f;

			Vector2I newSize = new Vector2I( (int)sizeF.X, (int)sizeF.Y );

			var target = new RenderTexture( buffer, newSize, this );

			var item = new RenderTargetItem( mip, slice, target );
			renderTargets.Add( item );
			return target;
		}

		public unsafe void PrepareNativeObject()
		{
			//!!!!!вызывать. пока в SetData

			if( Disposed )
				return;

			//!!!!
			EngineThreading.CheckMainThread();

			if( mode == ModeEnum.Create )
			{
				if( realObject == null )
					CreateNativeObject();

				if( realObject != null && needUpdateNative )
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
							case Component_Image.TypeEnum._2D:
								{
									//!!!!
									int pitch = ushort.MaxValue;

									var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( item.data );
									//var memory = MemoryBlock.FromArray( item.data );
									realObject.Update2D( 0, item.mip, 0, 0, ResultSize.X, ResultSize.Y, memory, pitch );
								}
								break;

							case Component_Image.TypeEnum.Cube:
								{
									//!!!!check face order

									//!!!!
									int pitch = ushort.MaxValue;

									var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( item.data );
									realObject.UpdateCube( (CubeMapFace)item.face, 0, item.mip, 0, 0, ResultSize.X, ResultSize.Y, memory, pitch );
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

		public void SetData( SurfaceData[] data )
		{
			if( mode != ModeEnum.Create )
				Log.Fatal( "GpuTexture: SetData: Unable to set data for loaded or render target textures." );

			this.data = data;
			needUpdateNative = true;
			//newData.Set( data );

			//!!!!temp?
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

		public Texture GetRealObject( bool withUpdate )
		{
			if( realObject == null && withUpdate && realObjectUnloaded )
				RestoreUnloaded();

			if( withUpdate )
				realObjectLastUsedTime = EngineApp.EngineTime;
			return realObject;
		}

		public static GpuTexture[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
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

			realObjectUnloaded = false;
		}

		public void Unload()
		{
			if( realObject != null )
			{
				realObject.Dispose();
				//EngineThreading.ExecuteFromMainThreadLater( delegate ( Texture realObject2, List<RenderTargetItem> renderTargets2 )
				//{
				//	foreach( var target in renderTargets2 )
				//		target.RenderTarget.DisposeInternal();
				//	realObject2.Dispose();
				//}, realObject, renderTargets );

				realObject = null;
			}

			realObjectUnloaded = true;
		}

		/// <summary>
		/// A method to temporary unload textures which are not used long time.
		/// </summary>
		/// <param name="howLongHasNotBeenUsedInSeconds"></param>
		public static void UnloadNotUsedForLongTime( double howLongHasNotBeenUsedInSeconds )
		{
			foreach( var texture in GetInstances() )
			{
				if( texture.realObjectUnloadSupport && !texture.realObjectUnloaded )
				{
					if( EngineApp.EngineTime - texture.realObjectLastUsedTime > howLongHasNotBeenUsedInSeconds )
						texture.Unload();
				}
			}
		}

	}
}
