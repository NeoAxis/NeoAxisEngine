// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a 2D or cubemap image. The component is useful to make cubemap textures from 6 2D images.
	/// </summary>
	[ResourceFileExtension( "image" )]
#if !DEPLOY
	[EditorControl( typeof( ImageEditor ) )]
	[Preview( typeof( ImagePreview ) )]
	[PreviewImage( typeof( ImagePreviewImage ) )]
#endif
	public class ImageComponent : ResultCompile<GpuTexture>
	{
		/// <summary>
		/// File name to load a texture from.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		public Reference<ReferenceValueType_Resource> LoadFile
		{
			get { if( _loadFile.BeginGet() ) LoadFile = _loadFile.Get( this ); return _loadFile.value; }
			set
			{
				if( _loadFile.BeginSet( ref value ) )
				{
					try
					{
						LoadFileChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadFile.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadFile"/> property value changes.</summary>
		public event Action<ImageComponent> LoadFileChanged;
		ReferenceField<ReferenceValueType_Resource> _loadFile;

		/// <summary>
		/// File name to load a cube texture from, Negative X side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube -X" )]
		public Reference<ReferenceValueType_Resource> LoadCubeNegativeX
		{
			get { if( _loadCubeNegativeX.BeginGet() ) LoadCubeNegativeX = _loadCubeNegativeX.Get( this ); return _loadCubeNegativeX.value; }
			set
			{
				if( _loadCubeNegativeX.BeginSet( ref value ) )
				{
					try
					{
						LoadCubeNegativeXChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubeNegativeX.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubeNegativeX"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubeNegativeXChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubeNegativeX;

		/// <summary>
		/// File name to load a cube texture from, Negative Y side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube -Y" )]
		public Reference<ReferenceValueType_Resource> LoadCubeNegativeY
		{
			get { if( _loadCubeNegativeY.BeginGet() ) LoadCubeNegativeY = _loadCubeNegativeY.Get( this ); return _loadCubeNegativeY.value; }
			set
			{
				if( _loadCubeNegativeY.BeginSet( ref value ) )
				{
					try
					{
						LoadCubeNegativeYChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubeNegativeY.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubeNegativeY"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubeNegativeYChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubeNegativeY;

		/// <summary>
		/// File name to load a cube texture from, Negative Z side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube -Z" )]
		public Reference<ReferenceValueType_Resource> LoadCubeNegativeZ
		{
			get { if( _loadCubeNegativeZ.BeginGet() ) LoadCubeNegativeZ = _loadCubeNegativeZ.Get( this ); return _loadCubeNegativeZ.value; }
			set
			{
				if( _loadCubeNegativeZ.BeginSet( ref value ) )
				{
					try
					{
						LoadCubeNegativeZChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubeNegativeZ.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubeNegativeZ"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubeNegativeZChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubeNegativeZ;

		/// <summary>
		/// File name to load a cube texture from, Positive X side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube +X" )]
		public Reference<ReferenceValueType_Resource> LoadCubePositiveX
		{
			get { if( _loadCubePositiveX.BeginGet() ) LoadCubePositiveX = _loadCubePositiveX.Get( this ); return _loadCubePositiveX.value; }
			set
			{
				if( _loadCubePositiveX.BeginSet( ref value ) )
				{
					try
					{
						LoadCubePositiveXChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubePositiveX.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubePositiveX"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubePositiveXChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubePositiveX;

		/// <summary>
		/// File name to load a cube texture from, Positive Y side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube +Y" )]
		public Reference<ReferenceValueType_Resource> LoadCubePositiveY
		{
			get { if( _loadCubePositiveY.BeginGet() ) LoadCubePositiveY = _loadCubePositiveY.Get( this ); return _loadCubePositiveY.value; }
			set
			{
				if( _loadCubePositiveY.BeginSet( ref value ) )
				{
					try
					{
						LoadCubePositiveYChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubePositiveY.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubePositiveY"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubePositiveYChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubePositiveY;

		/// <summary>
		/// File name to load a cube texture from, Positive Z side.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Load" )]
		[DisplayName( "Load Cube +Z" )]
		public Reference<ReferenceValueType_Resource> LoadCubePositiveZ
		{
			get { if( _loadCubePositiveZ.BeginGet() ) LoadCubePositiveZ = _loadCubePositiveZ.Get( this ); return _loadCubePositiveZ.value; }
			set
			{
				if( _loadCubePositiveZ.BeginSet( ref value ) )
				{
					try
					{
						LoadCubePositiveZChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _loadCubePositiveZ.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadCubePositiveZ"/> property value changes.</summary>
		public event Action<ImageComponent> LoadCubePositiveZChanged;
		ReferenceField<ReferenceValueType_Resource> _loadCubePositiveZ;

		/////////////////////////////////////////

		/// <summary>
		/// The format of a texture being created.
		/// </summary>
		[Serialize]
		[DefaultValue( PixelFormat.Unknown )]
		[Category( "Create" )]
		public Reference<PixelFormat> CreateFormat
		{
			get { if( _createFormat.BeginGet() ) CreateFormat = _createFormat.Get( this ); return _createFormat.value; }
			set { if( _createFormat.BeginSet( ref value ) ) { try { CreateFormatChanged?.Invoke( this ); } finally { _createFormat.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateFormat"/> property value changes.</summary>
		public event Action<ImageComponent> CreateFormatChanged;
		ReferenceField<PixelFormat> _createFormat = PixelFormat.Unknown;

		/// <summary>
		/// The type of texture being created.
		/// </summary>
		[Serialize]
		[DefaultValue( TypeEnum._2D )]
		[Category( "Create" )]
		public Reference<TypeEnum> CreateType
		{
			get { if( _createType.BeginGet() ) CreateType = _createType.Get( this ); return _createType.value; }
			set { if( _createType.BeginSet( ref value ) ) { try { CreateTypeChanged?.Invoke( this ); } finally { _createType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateType"/> property value changes.</summary>
		public event Action<ImageComponent> CreateTypeChanged;
		ReferenceField<TypeEnum> _createType = TypeEnum._2D;

		/// <summary>
		/// The size of a texture being created.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0" )]
		[Category( "Create" )]
		public Reference<Vector2I> CreateSize
		{
			get { if( _createSize.BeginGet() ) CreateSize = _createSize.Get( this ); return _createSize.value; }
			set { if( _createSize.BeginSet( ref value ) ) { try { CreateSizeChanged?.Invoke( this ); } finally { _createSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateSize"/> property value changes.</summary>
		public event Action<ImageComponent> CreateSizeChanged;
		ReferenceField<Vector2I> _createSize = Vector2I.Zero;

		/// <summary>
		/// The number of depth levels of a texture being created.
		/// </summary>
		[Serialize]
		[DefaultValue( 0 )]
		[Category( "Create" )]
		public Reference<int> CreateDepth
		{
			get { if( _createDepth.BeginGet() ) CreateDepth = _createDepth.Get( this ); return _createDepth.value; }
			set
			{
				//!!!!так? везде проверки поставить
				//@@@ if( value < 1 )
				//	value = 1;

				if( _createDepth.BeginSet( ref value ) )
				{
					try { CreateDepthChanged?.Invoke( this ); }
					finally { _createDepth.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CreateDepth"/> property value changes.</summary>
		public event Action<ImageComponent> CreateDepthChanged;
		ReferenceField<int> _createDepth;

		/// <summary>
		/// The number of layers of a texture being created.
		/// </summary>
		[Serialize]
		[DefaultValue( 1 )]
		[Category( "Create" )]
		public Reference<int> CreateArrayLayers
		{
			get { if( _createArrayLayers.BeginGet() ) CreateArrayLayers = _createArrayLayers.Get( this ); return _createArrayLayers.value; }
			set { if( _createArrayLayers.BeginSet( ref value ) ) { try { CreateArrayLayersChanged?.Invoke( this ); } finally { _createArrayLayers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateArrayLayers"/> property value changes.</summary>
		public event Action<ImageComponent> CreateArrayLayersChanged;
		ReferenceField<int> _createArrayLayers = 1;

		/// <summary>
		/// Whether to create mip levels.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		[Category( "Create" )]
		public Reference<bool> CreateMipmaps
		{
			get { if( _createMipmaps.BeginGet() ) CreateMipmaps = _createMipmaps.Get( this ); return _createMipmaps.value; }
			set { if( _createMipmaps.BeginSet( ref value ) ) { try { CreateMipmapsChanged?.Invoke( this ); } finally { _createMipmaps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateMipmaps"/> property value changes.</summary>
		public event Action<ImageComponent> CreateMipmapsChanged;
		ReferenceField<bool> _createMipmaps;
		////CreateMipmaps
		//ReferenceField<int> _createMipmaps;
		//[Serialize]
		//[DefaultValue( 0 )]
		//public Reference<int> CreateMipmaps
		//{
		//	get
		//	{
		//		if( _createMipmaps.BeginGet() )
		//			CreateMipmaps = _createMipmaps.Get( this );
		//		return _createMipmaps.value;
		//	}
		//	set
		//	{
		//		if( _createMipmaps.BeginSet( ref value ) )
		//		{
		//			try { CreateMipmapsChanged?.Invoke( this ); }
		//			finally { _createMipmaps.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Texture> CreateMipmapsChanged;

		//!!!!нет способа указать данные в свойстве. byte[] CreateImageData

		/// <summary>
		/// Specifies texture creation flags.
		/// </summary>
		[Serialize]
		[DefaultValue( Usages.WriteOnly )]
		[Category( "Create" )]
		public Reference<Usages> CreateUsage
		{
			get { if( _createUsage.BeginGet() ) CreateUsage = _createUsage.Get( this ); return _createUsage.value; }
			set { if( _createUsage.BeginSet( ref value ) ) { try { CreateUsageChanged?.Invoke( this ); } finally { _createUsage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateUsage"/> property value changes.</summary>
		public event Action<ImageComponent> CreateUsageChanged;
		ReferenceField<Usages> _createUsage = Usages.WriteOnly;

		/// <summary>
		/// Enables Full Screen Anti-Aliasing.
		/// </summary>
		[Serialize]
		[DefaultValue( 0 )]
		[Category( "Create" )]
		public Reference<int> CreateFSAA
		{
			get { if( _createFSAA.BeginGet() ) CreateFSAA = _createFSAA.Get( this ); return _createFSAA.value; }
			set { if( _createFSAA.BeginSet( ref value ) ) { try { CreateFSAAChanged?.Invoke( this ); } finally { _createFSAA.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateFSAA"/> property value changes.</summary>
		public event Action<ImageComponent> CreateFSAAChanged;
		ReferenceField<int> _createFSAA;

		/////////////////////////////////////////

		/// <summary>Enum identifying the texture type.</summary>
		public enum TypeEnum
		{
			///// <summary>1D texture, used in combination with 1D texture coordinates.</summary>
			//_1D = 1,//Ogre::TEX_TYPE_1D,

			/// <summary>2D texture, used in combination with 2D texture coordinates (default).</summary>
			_2D = 2,//Ogre::TEX_TYPE_2D,

			/// <summary>3D volume texture, used in combination with 3D texture coordinates.</summary>
			_3D = 3,//Ogre::TEX_TYPE_2D,

			/// <summary>3D cube map, used in combination with 3D texture coordinates.</summary>
			Cube = 4,//Ogre::TEX_TYPE_CUBE_MAP,
		}

		/////////////////////////////////////////

		/// <summary>Enum describing buffer usage; not mutually exclusive.</summary>
		[Flags]
		public enum Usages
		{
			/// <summary>
			/// Indicates the application would like to modify this buffer with the CPU
			/// fairly often. 
			/// Buffers created with this flag will typically end up in AGP memory rather 
			/// than video memory.
			/// </summary>
			Dynamic = 1,

			/// <summary>
			/// Indicates the application will never read the contents of the buffer back, 
			/// it will only ever write data. Locking a buffer with this flag will ALWAYS 
			/// return a pointer to new, blank memory rather than the memory associated 
			/// with the contents of the buffer; this avoids DMA stalls because you can 
			/// write to a new memory area while the previous one is being used. 
			/// </summary>
			WriteOnly = 2,

			//!!!!
			/// <summary>
			/// Mipmaps will be automatically generated for this texture.
			/// </summary>
			AutoMipmaps = 4,

			/// <summary>
			/// This texture will be a render target, ie. used as a target for render to texture
			/// setting this flag will ignore all other texture usages except <b>AutoMipmap</b>.
			/// </summary>
			RenderTarget = 8,

			/// <summary>
			/// Texture can be used as the destination of a blit operation.
			/// </summary>
			BlitDestination = 16,

			/// <summary>
			/// Texture data can be read back.
			/// </summary>
			ReadBack = 32,

			ComputeWrite = 64,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Attribute for setting parameters of texture binding.
		/// </summary>
		public class BindSettingsAttribute : Attribute
		{
			TextureAddressingMode addressingMode;
			FilterOption filteringMin;
			FilterOption filteringMag;
			FilterOption filteringMip;
			//!!!!maybe temp
			int samplerIndex;

			//

			public BindSettingsAttribute( TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, int samplerIndex )
			{
				this.addressingMode = addressingMode;
				this.filteringMin = filteringMin;
				this.filteringMag = filteringMag;
				this.filteringMip = filteringMip;
				this.samplerIndex = samplerIndex;
			}

			public TextureAddressingMode AddressingMode
			{
				get { return addressingMode; }
			}

			public FilterOption FilteringMin
			{
				get { return filteringMin; }
			}

			public FilterOption FilteringMag
			{
				get { return filteringMag; }
			}

			public FilterOption FilteringMip
			{
				get { return filteringMip; }
			}

			public int SamplerIndex
			{
				get { return samplerIndex; }
			}
		}

		/////////////////////////////////////////

		public bool AnyCubemapSideIsSpecified()
		{
			for( int n = 0; n < 6; n++ )
				if( GetLoadCubeSide( n ) != null )
					return true;
			return false;
		}

		public bool AllCubemapSidesAreaSpecified()
		{
			for( int n = 0; n < 6; n++ )
				if( GetLoadCubeSide( n ) == null )
					return false;
			return true;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( CreateType ):
				case nameof( CreateSize ):
				case nameof( CreateDepth ):
				case nameof( CreateArrayLayers ):
				case nameof( CreateMipmaps ):
				case nameof( CreateUsage ):
				case nameof( CreateFSAA ):
					if( CreateFormat.Value == PixelFormat.Unknown )
						skip = true;
					if( p.Name == nameof( CreateDepth ) && CreateType.Value != TypeEnum._3D )
						skip = true;
					break;

				case nameof( LoadFile ):
					if( CreateFormat.Value != PixelFormat.Unknown )
						skip = true;
					if( AnyCubemapSideIsSpecified() )
						skip = true;
					break;

				case nameof( LoadCubePositiveX ):
				case nameof( LoadCubeNegativeX ):
				case nameof( LoadCubePositiveY ):
				case nameof( LoadCubeNegativeY ):
				case nameof( LoadCubePositiveZ ):
				case nameof( LoadCubeNegativeZ ):
					if( CreateFormat.Value != PixelFormat.Unknown )
						skip = true;
					if( LoadFile.Value != null )
						skip = true;
					break;
				}
			}
		}

		public ReferenceValueType_Resource GetLoadCubeSide( int face )
		{
			switch( face )
			{
			case 0: return LoadCubePositiveX;
			case 1: return LoadCubeNegativeX;
			case 2: return LoadCubePositiveY;
			case 3: return LoadCubeNegativeY;
			case 4: return LoadCubePositiveZ;
			case 5: return LoadCubeNegativeZ;
			}
			return null;
		}

		protected override void OnResultCompile()
		{
			if( Result == null )
			{
				GpuTexture result = null;

				PixelFormat createFormatV = CreateFormat;
				if( createFormatV != PixelFormat.Unknown )
				{
					//create

					Vector2I size = CreateSize;
					if( size.X > 0 && size.Y > 0 )
					{
						//!!!!!что еще проверить?

						//!!!!3d support

						TypeEnum type = CreateType;
						bool mipmaps = CreateMipmaps;
						int arrayLayers = CreateArrayLayers;
						Usages usage = CreateUsage;
						int fsaa = CreateFSAA;

						int depth;
						if( type == TypeEnum._2D )
							depth = 1;
						else if( type == TypeEnum.Cube )
							depth = 6;
						else
							depth = CreateDepth;

						GpuTexture.Usages gpuUsage = 0;
						if( ( usage & Usages.Dynamic ) != 0 )
							gpuUsage |= GpuTexture.Usages.Dynamic;
						else
							gpuUsage |= GpuTexture.Usages.Static;
						if( ( usage & Usages.WriteOnly ) != 0 )
							gpuUsage |= GpuTexture.Usages.WriteOnly;
						if( ( usage & Usages.AutoMipmaps ) != 0 )
							gpuUsage |= GpuTexture.Usages.AutoMipmap;
						if( ( usage & Usages.RenderTarget ) != 0 )
							gpuUsage |= GpuTexture.Usages.RenderTarget;
						if( ( usage & Usages.ComputeWrite ) != 0 )
							gpuUsage |= GpuTexture.Usages.ComputeWrite;
						if( ( usage & Usages.ReadBack ) != 0 )
							gpuUsage |= GpuTexture.Usages.ReadBack;
						if( ( usage & Usages.BlitDestination ) != 0 )
							gpuUsage |= GpuTexture.Usages.BlitDestination;

						//!!!!!почему для других не Discardable?
						if( ( gpuUsage & GpuTexture.Usages.Dynamic ) != 0 && ( gpuUsage & GpuTexture.Usages.WriteOnly ) != 0 )
							gpuUsage |= GpuTexture.Usages.DynamicWriteOnlyDiscardable;

						result = new GpuTexture( type, size, depth, mipmaps, arrayLayers, createFormatV, gpuUsage, fsaa, out var error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							result.Dispose();
							result = null;

							Log.Warning( "Unable to create texture. " + error2 );
						}

						if( result != null && gpuUsage.HasFlag( GpuTexture.Usages.BlitDestination ) )
							result.PrepareNativeObject();
					}
				}
				else
				{
					//load

					//!!!!3d support

					//!!!!!проверять наличие файла. если нет, то он по дефолту создает 512 512 Unknown формат. это какая-то хрень. надо разрулить

					//!!!!threading


					//!!!!!каждый раз проверять будет если нет файла. где еще так?

					string[] loadCubeMap6Files = null;
					string loadFromOneFile = null;

					if( AnyCubemapSideIsSpecified() )
					{
						if( AllCubemapSidesAreaSpecified() )
						{
							loadCubeMap6Files = new string[ 6 ];
							for( int n = 0; n < 6; n++ )
							{
								var v = GetLoadCubeSide( n );
								if( v != null )
									loadCubeMap6Files[ n ] = v.ResourceName;

								if( string.IsNullOrEmpty( loadCubeMap6Files[ n ] ) || !VirtualFile.Exists( loadCubeMap6Files[ n ] ) )
								{
									loadCubeMap6Files = null;
									break;
								}
							}
						}
					}
					else
					{
						var v = LoadFile.Value;
						loadFromOneFile = v != null ? v.ResourceName : "";
						if( string.IsNullOrEmpty( loadFromOneFile ) || !VirtualFile.Exists( loadFromOneFile ) )
							loadFromOneFile = null;
					}

					//!!!!
					//bool RenderToTexture = false;
					//if( RenderToTexture )
					//	result = RenderToTextureTest();
					//else
					if( loadFromOneFile != null )
						result = LoadOneTexture( loadFromOneFile );
					else if( loadCubeMap6Files != null )
						result = LoadCubeTexture( loadCubeMap6Files );
				}

				Result = result;
			}
		}

		//private GpuTexture RenderToTextureTest()
		//{
		//	GpuTexture result = null;
		//	//!!!!temp

		//	TypeEnum type = TypeEnum.Cube;
		//	Usages usage = Usages.RenderTarget;
		//	//Usages usage = Usages.WriteOnly;// | Usages.RenderTarget;
		//	int fsaa = 0;
		//	int mipmaps = 0;
		//	PixelFormat format = PixelFormat.A8R8G8B8;

		//	GpuTexture.Usages gpuUsage = 0;
		//	if( ( usage & Usages.Dynamic ) != 0 )
		//		gpuUsage |= GpuTexture.Usages.Dynamic;
		//	else
		//		gpuUsage |= GpuTexture.Usages.Static;
		//	if( ( usage & Usages.WriteOnly ) != 0 )
		//		gpuUsage |= GpuTexture.Usages.WriteOnly;
		//	if( ( usage & Usages.AutoMipmaps ) != 0 )
		//		gpuUsage |= GpuTexture.Usages.AutoMipmap;
		//	if( ( usage & Usages.RenderTarget ) != 0 )
		//		gpuUsage |= GpuTexture.Usages.RenderTarget;

		//	//!!!!!почему для других не Discardable?
		//	if( ( gpuUsage & GpuTexture.Usages.Dynamic ) != 0 && ( gpuUsage & GpuTexture.Usages.WriteOnly ) != 0 )
		//		gpuUsage |= GpuTexture.Usages.DynamicWriteOnlyDiscardable;

		//	string error2;

		//	result = new GpuTexture( type, new Vec2I( 16, 16 ), 1, mipmaps, format, gpuUsage, fsaa, out error2 );
		//	if( !string.IsNullOrEmpty( error2 ) )
		//	{
		//		result.Dispose();
		//		result = null;

		//		//!!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	//result = GpuTexture.CreateManual("test", type, new Vec2I(16, 16), 1, mipmaps, format, gpuUsage, fsaa, out error2);
		//	//if (!result.Load2(out error2))
		//	//{
		//	//	result.Dispose();
		//	//	result = null;

		//	//	//!!!!
		//	//	Log.Fatal($"Unable to load texture \'{loadFromOneFile}\'. " + error2);
		//	//}

		//	unsafe
		//	{
		//		ColorValue[] colors = new ColorValue[ 6 ];
		//		colors[ 0 ] = new ColorValue( 1, 0, 0 );
		//		colors[ 1 ] = new ColorValue( .3, 0, 0 );
		//		colors[ 2 ] = new ColorValue( 0, 1, 0 );
		//		colors[ 3 ] = new ColorValue( 0, .3, 0 );
		//		colors[ 4 ] = new ColorValue( 0, 0, 1 );
		//		colors[ 5 ] = new ColorValue( 0, 0, .3 );

		//		for( int nIteration = 0; nIteration < 6; nIteration++ )
		//		{
		//			var renderTarget = result.GetRenderTarget( nIteration );
		//			renderTarget.AddViewport( false, false );

		//			Viewport viewport = renderTarget.Viewports[ 0 ];
		//			RenderingSystem.CallCustomMethod( "_setViewport", (IntPtr)viewport.realObject );
		//			viewport._ViewportRendering_Clear( FrameBufferTypes.All, colors[ nIteration ], 1, 0 );
		//			RenderingSystem.CallCustomMethod( "_setViewport", IntPtr.Zero );
		//		}
		//	}

		//	return result;
		//}

		GpuTexture LoadOneTexture( string loadFromOneFile )
		{
			GpuTexture result = null;

			//!!!!slow
			EngineThreading.ExecuteFromMainThreadWait( delegate ( ImageComponent _this )
			{
				if( loadFromOneFile != null )
				{
					//2D, Cube, etc

					//!!!!возможно хорошая идея грузить данные тут, т.е. в C#. потом закидывать в созданную GpuTexture.
					//!!!!!!для DDS был бы особый случай, т.к. они умеют сразу в D3D создаваться.
					result = GpuTexture.CreateFromFile( loadFromOneFile, out var error2 );
					if( result == null )
						Log.Warning( $"Unable to load texture \'{loadFromOneFile}\'. " + error2 );
				}
			}, this );

			return result;
		}

		GpuTexture LoadCubeTexture( string[] loadCubeMap6Files )
		{
			GpuTexture result = null;

			EngineThreading.ExecuteFromMainThreadWait( delegate ( ImageComponent _this )
			{
				var componentTextureVirtualFileName = ComponentUtility.GetOwnedFileNameOfComponent( this );
				if( string.IsNullOrEmpty( componentTextureVirtualFileName ) )
					componentTextureVirtualFileName = loadCubeMap6Files[ 0 ];

				result = GpuTexture.CreateCube( loadCubeMap6Files, componentTextureVirtualFileName, out var error2 );
				if( result == null )
					Log.Warning( $"Unable to load texture \'{componentTextureVirtualFileName}\'. " + error2 );
			}, this );

			return result;
		}


		//					if( loadFromOneFile != null || loadCubeMap6Files != null )
		//					{

		//						//!!!!slow
		//						EngineThreading.ExecuteFromMainThreadWait( delegate ( Texture _this )
		//						{
		//							if( loadFromOneFile != null )
		//							{
		//								//2D, Cube, etc


		//								//!!!!

		//								////!!!!возможно хорошая идея грузить данные тут, т.е. в C#. потом закидывать в созданную GpuTexture.
		//								////!!!!!!для DDS был бы особый случай, т.к. они умеют сразу в D3D создаваться.
		//								result = new GpuTexture( loadFromOneFile );

		//								string error2;
		//								if( !result.Load( out error2 ) )
		//								{
		//									result.Dispose();
		//									result = null;

		//									//!!!!
		//									Log.Fatal( $"Unable to load texture \'{loadFromOneFile}\'. " + error2 );
		//								}
		//								if( !string.IsNullOrEmpty( error2 ) )
		//								{
		//									result.Dispose();
		//									result = null;

		//									//!!!!
		//									Log.Fatal( "impl" );
		//								}


		//								//if( !ImageManager.LoadFromFile( loadFromOneFile, out byte[] data, out Vec2I size, out int depth,
		//								//	out PixelFormat format, out int numFaces, out int numMipmaps, out string error ) )
		//								//{
		//								//	//!!!!

		//								//	Log.Fatal( "Texture: Loading image failed ({0}).", error );
		//								//}

		//								////!!!!что еще проверять

		//								////!!!!пока только 2D
		//								//if( numFaces == 1 )
		//								//{
		//								//	//TypeEnum type = CreateType;
		//								//	//int mipmaps = CreateMipmaps;
		//								//	//Usages usage = CreateUsage;
		//								//	//int fsaa = CreateFSAA;

		//								//	//int depth;
		//								//	//if( type == TypeEnum._2D )
		//								//	//	depth = 1;
		//								//	//else if( type == TypeEnum.Cube )
		//								//	//	depth = 6;
		//								//	//else
		//								//	//	depth = CreateDepth;

		//								//	//!!!!
		//								//	TypeEnum type = TypeEnum._2D;

		//								//	GpuTexture.Usages gpuUsage = GpuTexture.Usages.Static | GpuTexture.Usages.WriteOnly;

		//								//	result = new GpuTexture( type, size, depth, numMipmaps + 1, format, gpuUsage, 0, out string error2 );
		//								//	if( !string.IsNullOrEmpty( error2 ) )
		//								//	{
		//								//		result.Dispose();
		//								//		result = null;

		//								//		//!!!!!
		//								//		Log.Fatal( "impl" );
		//								//	}

		//								//	//set data
		//								//	{
		//								//		var dict = new Dictionary<GpuTexture.SurfaceIndex, byte[]>();

		//								//		Vec2I currentSize = size;
		//								//		int currentOffset = 0;
		//								//		for( int mipLevel = 0; mipLevel < numMipmaps + 1; mipLevel++ )
		//								//		{
		//								//			int memorySize = PixelFormatUtils.GetMemorySize( currentSize.X, currentSize.Y, depth, format );

		//								//			//!!!!может закидывать как есть массив со смещением

		//								//			byte[] d = new byte[ memorySize ];
		//								//			Buffer.BlockCopy( data, currentOffset, d, 0, memorySize );
		//								//			dict.Add( new GpuTexture.SurfaceIndex( 0, mipLevel ), d );

		//								//			currentSize /= 2;
		//								//			if( currentSize.X < 1 )
		//								//				currentSize.X = 1;
		//								//			if( currentSize.Y < 1 )
		//								//				currentSize.Y = 1;

		//								//			currentOffset += memorySize;
		//								//		}
		//								//		if( currentOffset != data.Length )
		//								//			Log.Fatal( "currentOffset != data.Length" );

		//								//		result.SetData( dict );
		//								//	}
		//								//}
		//								//else
		//								//{
		//								//}


		//							}
		//							else if( loadCubeMap6Files != null )
		//							{
		//								//!!!!temp

		//								TypeEnum type = TypeEnum.Cube;
		//								int depth = 6;
		//								int mipmaps = 0;
		//								Usages usage = Usages.RenderTarget;
		//								//Usages usage = Usages.WriteOnly;// | Usages.RenderTarget;
		//								int fsaa = 0;
		//								PixelFormat format = PixelFormat.A8R8G8B8;

		//								GpuTexture.Usages gpuUsage = 0;
		//								if( ( usage & Usages.Dynamic ) != 0 )
		//									gpuUsage |= GpuTexture.Usages.Dynamic;
		//								else
		//									gpuUsage |= GpuTexture.Usages.Static;
		//								if( ( usage & Usages.WriteOnly ) != 0 )
		//									gpuUsage |= GpuTexture.Usages.WriteOnly;
		//								if( ( usage & Usages.AutoMipmaps ) != 0 )
		//									gpuUsage |= GpuTexture.Usages.AutoMipmap;
		//								if( ( usage & Usages.RenderTarget ) != 0 )
		//									gpuUsage |= GpuTexture.Usages.RenderTarget;

		//								//!!!!!почему для других не Discardable?
		//								if( ( gpuUsage & GpuTexture.Usages.Dynamic ) != 0 && ( gpuUsage & GpuTexture.Usages.WriteOnly ) != 0 )
		//									gpuUsage |= GpuTexture.Usages.DynamicWriteOnlyDiscardable;

		//								string error2;
		//								result = new GpuTexture( type, new Vec2I( 16, 16 ), depth, mipmaps, format, gpuUsage, fsaa, out error2 );
		//								if( !string.IsNullOrEmpty( error2 ) )
		//								{
		//									result.Dispose();
		//									result = null;

		//									//!!!!!
		//									Log.Fatal( "impl" );
		//								}

		//								unsafe
		//								{
		//									ColorValue[] colors = new ColorValue[ 6 ];
		//									colors[ 0 ] = new ColorValue( 1, 0, 0 );
		//									colors[ 1 ] = new ColorValue( .3, 0, 0 );
		//									colors[ 2 ] = new ColorValue( 0, 1, 0 );
		//									colors[ 3 ] = new ColorValue( 0, .3, 0 );
		//									colors[ 4 ] = new ColorValue( 0, 0, 1 );
		//									colors[ 5 ] = new ColorValue( 0, 0, .3 );

		//									for( int nIteration = 0; nIteration < 6; nIteration++ )
		//									{
		//										var renderTarget = result.GetRenderTarget( nIteration );
		//										renderTarget.AddViewport( false, false );

		//										Viewport viewport = renderTarget.Viewports[ 0 ];
		//										RenderingSystem.CallCustomMethod( "_setViewport", (IntPtr)viewport.realObject );
		//										viewport._ViewportRendering_Clear( FrameBufferTypes.All, colors[ nIteration ], 1, 0 );
		//										RenderingSystem.CallCustomMethod( "_setViewport", IntPtr.Zero );
		//									}
		//								}

		//							}

		//						}, this );
		//					}
		//				}

		//				Result = result;
		//			}
		//		}


		public static bool DeleteCompressedFile( string virtualFileName, out string error )
		{
			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				var realFileName = PathUtility.Combine( VirtualFileSystem.Directories.Project, @"Caches\Files", virtualFileName ) + ".dds";

				try
				{
					if( File.Exists( realFileName ) )
						File.Delete( realFileName );
					var infoFile = realFileName + ".info";
					if( File.Exists( infoFile ) )
						File.Delete( infoFile );
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}
			}

			error = "";
			return true;
		}
	}
}
