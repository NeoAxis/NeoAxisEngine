// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Specifies the material settings when rendering.
	/// </summary>
	public class GpuMaterialPass// : ThreadSafeDisposable
	{
		//GpuProgram vertexProgram;
		//GpuProgram fragmentProgram;

		GpuLinkedProgram linkedProgram;

		//bool lighting = false;
		SceneBlendFactor sourceBlendFactor = SceneBlendFactor.One;
		SceneBlendFactor destinationBlendFactor = SceneBlendFactor.Zero;
		//!!!!separate alpha blending
		//!!!!_D3D11_RENDER_TARGET_BLEND_DESC1

		bool depthCheck = true;
		bool depthWrite = true;
		CompareFunction depthFunction = CompareFunction.LessEqual;
		CullingMode cullingMode = CullingMode.Clockwise;
		//!!!!в D3D11 нет. еще там вроде баг "!!!!bug?"
		//CompareFunction alphaRejectFunction = CompareFunction.AlwaysPass;
		//int alphaRejectValue = 0;
		//!!!!в bgfx нет
		//PolygonMode polygonMode = PolygonMode.Solid;
		//bool normalizeNormals;
		bool colorWriteRed = true;
		bool colorWriteGreen = true;
		bool colorWriteBlue = true;
		bool colorWriteAlpha = true;

		//!!!!было
		//AvailableParameterItem[] availableParameters;
		//Dictionary<string, AvailableParameterItem> availableParameterByName;

		//!!!!not used
		//!!!!threading
		//ParameterContainer constantParameterValues = new ParameterContainer();

		uint advancedBlendingWriteMask = uint.MaxValue;

		bool needUpdate = true;
		RenderState cachedState;

		////!!!!temp
		//public bool _tempAlphaToCoverage;
		//public void SettempAlphaToCoverage( bool value ) { _tempAlphaToCoverage = value; needUpdate = true; }

		////////////

		////!!!!struct?
		//public struct AvailableParameterItem
		//{
		//	internal string name;//!!!!!!так?
		//	internal ParameterType type;
		//	internal int elementCount;

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public ParameterType Type
		//	{
		//		get { return type; }
		//	}

		//	public int ElementCount
		//	{
		//		get { return elementCount; }
		//	}
		//}

		////////////

		//public struct TextureParameterValue//public class TextureParameterValue
		//{
		//	xx xx;//int Index;

		//	//!!!!какие-то параметры могут быть тоже составными

		//	public Component_Image Texture;
		//	public TextureAddressingMode AddressingMode;
		//	public FilterOption FilteringMin;
		//	public FilterOption FilteringMag;
		//	public FilterOption FilteringMip;
		//	public ColorValue BorderColor;
		//	public TextureFlags AdditionFlags;

		//	//!!!!
		//	//int numMipmaps;
		//	//bool isAlpha;
		//	//virtual void _setTextureLayerAnisotropy( size_t unit, unsigned int maxAnisotropy ) = 0;
		//	//virtual void _setTextureMipmapBias( size_t unit, float bias ) = 0;
		//	//public ContentTypes contentType;
		//	//bool fetch4;

		//	//public TextureParameterValue()
		//	//{
		//	//}

		//	//public TextureParameterValue( Component_Image texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, ColorValue borderColor )
		//	//{
		//	//	this.texture = texture;
		//	//	this.addressingMode = addressingMode;
		//	//	this.filteringMin = filteringMin;
		//	//	this.filteringMag = filteringMag;
		//	//	this.filteringMip = filteringMip;
		//	//	this.borderColor = borderColor;
		//	//}

		//	public TextureParameterValue( Component_Image texture, TextureAddressingMode addressingMode, FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip )
		//	{
		//		this.Texture = texture;
		//		this.AddressingMode = addressingMode;
		//		this.FilteringMin = filteringMin;
		//		this.FilteringMag = filteringMag;
		//		this.FilteringMip = filteringMip;

		//		this.BorderColor = new ColorValue( 0, 0, 0, 0 );
		//		this.AdditionFlags = 0;
		//	}

		//	//public Texture Texture
		//	//{
		//	//	get { return texture; }
		//	//}
		//}

		////////////

		internal unsafe GpuMaterialPass( /*OgrePass* realObject, */GpuLinkedProgram linkedProgram )
		{
			this.linkedProgram = linkedProgram;

			//this.realObject = realObject;

			////init programs
			//this.programs = programs;
			//for( int n = 0; n < programs.Length; n++ )
			//{
			//	_old_GpuProgramType type = (_old_GpuProgramType)n;
			//	if( programs[ n ] != null )
			//		OgrePass.setProgram( realObject, type, programs[ n ].Name );
			//}

			////compile, prepare parameters
			//OgrePass.compile( realObject );
			////!!!!!check error

			////get available parameters
			//{
			//	int count = OgrePass.getAvailableParameterCount( realObject );
			//	availableParameters = new AvailableParameterItem[ count ];
			//	for( int n = 0; n < count; n++ )
			//	{
			//		IntPtr namePointer;
			//		ParameterType type;
			//		int elementCount;
			//		OgrePass.getAvailableParameterInfo( realObject, n, out namePointer, out type, out elementCount );

			//		AvailableParameterItem item = new AvailableParameterItem();
			//		item.name = OgreNativeWrapper.GetOutString( namePointer );
			//		item.type = type;
			//		item.elementCount = elementCount;
			//		availableParameters[ n ] = item;
			//	}
			//}
		}

		///// <summary>Releases the resources that are used by the object.</summary>
		//protected override void OnDispose()
		//{
		//	//unsafe
		//	//{
		//	//	if( realObject != null )
		//	//	{
		//	//		//after shutdown check
		//	//		if( RendererWorld.Disposed )
		//	//		{
		//	//			//waiting for .NET Standard 2.0
		//	//			Log.Fatal( "Renderer: Dispose after Shutdown." );
		//	//			//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
		//	//		}

		//	//		EngineThreading.ExecuteFromMainThreadLater( delegate ( IntPtr ptr )
		//	//		{
		//	//			OgrePass.Delete( (void*)ptr );
		//	//		}, (IntPtr)realObject );

		//	//		realObject = null;
		//	//	}
		//	//}

		//	//base.OnDispose();
		//}

		/// <summary>
		/// Gets or sets the source blend factor.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Allows very fine control of blending this Pass with the existing contents of the scene.
		/// Wheras the texture blending operations seen in the TextureUnitState class are 
		/// concerned with blending between texture layers, this blending is about combining 
		/// the output of the material as a whole with the existing contents of the rendering 
		/// target. This blending therefore allows object transparency and other special effects.
		/// </para>
		/// <para>
		/// This version of the method allows complete control over the blending operation, 
		/// by specifying the source and destination blending factors. The result of the 
		/// blending operation is: final = (texture * sourceFactor) + (pixel * destFactor)
		/// </para>
		/// <para>
		/// Each of the factors is specified as one of a number of options, as specified 
		/// in the SceneBlendFactor enumerated type.
		/// </para>
		/// </remarks>
		public SceneBlendFactor SourceBlendFactor
		{
			get { return sourceBlendFactor; }
			set
			{
				if( sourceBlendFactor == value )
					return;
				sourceBlendFactor = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets the destination blend factor.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Allows very fine control of blending this Pass with the existing contents of the scene.
		/// Wheras the texture blending operations seen in the TextureUnitState class are 
		/// concerned with blending between texture layers, this blending is about combining 
		/// the output of the material as a whole with the existing contents of the rendering 
		/// target. This blending therefore allows object transparency and other special effects.
		/// </para>
		/// <para>
		/// This version of the method allows complete control over the blending operation, 
		/// by specifying the source and destination blending factors. The result of the 
		/// blending operation is: final = (texture * sourceFactor) + (pixel * destFactor)
		/// </para>
		/// <para>
		/// Each of the factors is specified as one of a number of options, as specified 
		/// in the SceneBlendFactor enumerated type.
		/// </para>
		/// </remarks>
		public SceneBlendFactor DestinationBlendFactor
		{
			get { return destinationBlendFactor; }
			set
			{
				if( destinationBlendFactor == value )
					return;
				destinationBlendFactor = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets whether or not this pass renders with depth-buffer checking on or not.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If depth-buffer checking is on, whenever a pixel is about to be written to the 
		/// frame buffer the depth buffer is checked to see if the pixel is in front 
		/// of all other pixels written at that point. If not, the pixel is not written.
		/// </para>
		/// <para>
		/// If depth checking is off, pixels are written no matter what has been rendered before.
		/// Also see setDepthFunction for more advanced depth check configuration.
		/// </para>
		/// </remarks>
		public bool DepthCheck
		{
			get { return depthCheck; }
			set
			{
				if( depthCheck == value )
					return;
				depthCheck = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets whether or not this pass renders with depth-buffer writing on or not.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If depth-buffer writing is on, whenever a pixel is written to the frame buffer
		/// the depth buffer is updated with the depth value of that new pixel, thus affecting future
		/// rendering operations if future pixels are behind this one.
		/// </para>
		/// <para>
		/// If depth writing is off, pixels are written without updating the depth buffer 
		/// Depth writing should normally be on but can be turned off when rendering 
		/// static backgrounds or when rendering a collection of transparent objects 
		/// at the end of a scene so that they overlap each other correctly.
		/// </para>
		/// </remarks>
		public bool DepthWrite
		{
			get { return depthWrite; }
			set
			{
				if( depthWrite == value )
					return;
				depthWrite = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets the function used to compare depth values when depth checking is on.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If depth checking is enabled (see DepthCheckEnabled) a comparison occurs between the depth
		/// value of the pixel to be written and the current contents of the buffer. This comparison is
		/// normally <b>CompareFunction.LessEqual</b>, i.e. the pixel is written if it is closer (or at the same distance)
		/// than the current contents. If you wish you can change this comparison using this method.
		/// </para>
		/// </remarks>
		public CompareFunction DepthFunction
		{
			get { return depthFunction; }
			set
			{
				if( depthFunction == value )
					return;
				depthFunction = value;
				needUpdate = true;
			}
		}

		///// <summary>
		///// Gets or sets whether or not dynamic lighting is enabled.
		///// </summary>
		///// <remarks>
		///// <para>
		///// If <b>true</b>, dynamic lighting is performed on geometry with normals supplied, 
		///// geometry without normals will not be displayed.
		///// </para>
		///// <para>
		///// If <b>false</b>, no lighting is applied and all geometry will be full brightness.
		///// </para>
		///// </remarks>
		//public bool Lighting
		//{
		//	get { return lighting; }
		//	set { lighting = value; }
		//}

		/// <summary>
		/// Gets or sets the culling mode for this pass based on the 'vertex winding'.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A typical way for the rendering engine to cull triangles is based on the 
		/// 'vertex winding' of triangles. Vertex winding refers to the direction in 
		/// which the vertices are passed or indexed to in the rendering operation as 
		/// viewed from the camera, and will wither be clockwise or anticlockwise.
		/// The default is <b>CullingMode.Clockwise</b> i.e. that only triangles whose 
		/// vertices are passed/indexed in anticlockwise order are rendered - this is a common 
		/// approach and is used in 3D studio models for example. You can alter this 
		/// culling mode if you wish but it is not advised unless you know what you are doing.
		/// </para>
		/// <para>
		/// You may wish to use the <b>CullingMode.None</b> option for mesh data that you cull 
		/// yourself where the vertex winding is uncertain.
		/// </para>
		/// </remarks>
		public CullingMode CullingMode
		{
			get { return cullingMode; }
			set
			{
				if( cullingMode == value )
					return;
				cullingMode = value;
				needUpdate = true;
			}
		}

		///// <summary>
		///// Gets or sets the way the pass will have use alpha to totally reject pixels 
		///// from the pipeline.
		///// </summary>
		///// <remarks>
		///// <para>
		///// The default is <b>CompareFunction.AlwaysPass</b> i.e. alpha is not used to reject pixels.
		///// </para>
		///// <para>
		///// This option applies in both the fixed function and the programmable pipeline.
		///// </para>
		///// </remarks>
		//public CompareFunction AlphaRejectFunction
		//{
		//	get { return alphaRejectFunction; }
		//	set
		//	{
		//		if( alphaRejectFunction == value )
		//			return;
		//		alphaRejectFunction = value;
		//		unsafe
		//		{
		//			OgrePass.setAlphaRejectFunction( realObject, value );
		//		}
		//	}
		//}

		///// <summary>
		///// Gets or sets the alpha reject value.
		///// </summary>
		//public int AlphaRejectValue
		//{
		//	get { return alphaRejectValue; }
		//	set
		//	{
		//		if( alphaRejectValue == value )
		//			return;
		//		alphaRejectValue = value;
		//		unsafe
		//		{
		//			OgrePass.setAlphaRejectValue( realObject, (byte)value );
		//		}
		//	}
		//}

		///// <summary>
		///// Gets or sets whether to use alpha to coverage (A2C) when blending alpha rejected values.
		///// </summary>
		///// <remarks>
		///// Alpha to coverage performs multisampling on the edges of alpha-rejected
		///// textures to produce a smoother result. It is only supported when multisampling
		///// is already enabled on the render target, and when the hardware supports
		///// alpha to coverage (see RenderSystemCapabilities). 
		///// </remarks>
		//public bool AlphaToCoverage
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			return OgrePass.isAlphaToCoverageEnabled( realObject );
		//		}
		//	}
		//	set
		//	{
		//		unsafe
		//		{
		//			OgrePass.setAlphaToCoverageEnabled( realObject, value );
		//		}
		//	}
		//}

		///// <summary>
		///// Gets or sets the type of polygon rendering required
		///// </summary>
		//public PolygonMode PolygonMode
		//{
		//	get { return polygonMode; }
		//	set
		//	{
		//		if( polygonMode == value )
		//			return;
		//		polygonMode = value;
		//		needUpdate = true;
		//	}
		//}

		//!!!!!!
		//public float DepthBiasConstant
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			return OgrePass.getDepthBiasConstant( realObject );
		//		}
		//	}
		//	set
		//	{
		//		unsafe
		//		{
		//			OgrePass.setDepthBias( realObject, value, DepthBiasSlopeScale );
		//		}
		//	}
		//}

		//!!!!!!
		//public float DepthBiasSlopeScale
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			return OgrePass.getDepthBiasSlopeScale( realObject );
		//		}
		//	}
		//	set
		//	{
		//		unsafe
		//		{
		//			OgrePass.setDepthBias( realObject, DepthBiasConstant, value );
		//		}
		//	}
		//}

		//[DefaultValue( false )]
		//public bool NormalizeNormals
		//{
		//	get { return normalizeNormals; }
		//	set
		//	{
		//		if( normalizeNormals == value )
		//			return;
		//		normalizeNormals = value;
		//		//!!!!было. везде так
		//		//unsafe
		//		//{
		//		//	//!!!!!slowly всё такое?
		//		//	OgrePass.setNormaliseNormals( realObject, value );
		//		//}
		//	}
		//}

		//public bool SupportHardwareInstancing
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			return OgrePass.getSupportHardwareInstancing( realObject );
		//		}
		//	}
		//	set
		//	{
		//		unsafe
		//		{
		//			OgrePass.setSupportHardwareInstancing( realObject, value );
		//		}
		//	}
		//}

		[DefaultValue( true )]
		public bool ColorWriteRed
		{
			get { return colorWriteRed; }
			set
			{
				if( colorWriteRed == value )
					return;
				colorWriteRed = value;
				needUpdate = true;
			}
		}

		[DefaultValue( true )]
		public bool ColorWriteGreen
		{
			get { return colorWriteGreen; }
			set
			{
				if( colorWriteGreen == value )
					return;
				colorWriteGreen = value;
				needUpdate = true;
			}
		}

		[DefaultValue( true )]
		public bool ColorWriteBlue
		{
			get { return colorWriteBlue; }
			set
			{
				if( colorWriteBlue == value )
					return;
				colorWriteBlue = value;
				needUpdate = true;
			}
		}

		[DefaultValue( true )]
		public bool ColorWriteAlpha
		{
			get { return colorWriteAlpha; }
			set
			{
				if( colorWriteAlpha == value )
					return;
				colorWriteAlpha = value;
				needUpdate = true;
			}
		}

		[DefaultValue( uint.MaxValue )]
		public uint AdvancedBlendingWriteMask
		{
			get { return advancedBlendingWriteMask; }
			set
			{
				if( advancedBlendingWriteMask == value )
					return;
				advancedBlendingWriteMask = value;
				needUpdate = true;
			}
		}

		public GpuLinkedProgram LinkedProgram
		{
			get { return linkedProgram; }
		}

		//!!!!
		//public AvailableParameterItem[] AvailableParameters
		//{
		//	get { return availableParameters; }
		//}

		//!!!!
		//public bool GetAvailableParameterInfo( string name, out AvailableParameterItem item )
		//{
		//	if( availableParameterByName == null )
		//	{
		//		availableParameterByName = new Dictionary<string, AvailableParameterItem>( availableParameters.Length );
		//		foreach( AvailableParameterItem item2 in availableParameters )
		//			availableParameterByName[ item2.name ] = item2;
		//	}

		//	if( availableParameterByName.TryGetValue( name, out item ) )
		//		return true;
		//	item = new AvailableParameterItem();
		//	return false;
		//}

		//!!!!
		//public unsafe void TEMP_setParameterValue( string name, void* data, int dataSizeInBytes )
		//{
		//	//!!!!!пока так
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		OgrePass.TEMP_setParameterValue( realObject, name, data, dataSizeInBytes );
		//	}
		//}

		//!!!!
		//public unsafe void TEMP_setParameterValueTexture( string name, GpuTexture texture, TextureAddressingMode addressingMode,
		//	FilterOption filteringMin, FilterOption filteringMag, FilterOption filteringMip, ColorValue borderColor )
		//{
		//	//!!!!!пока так
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		OgrePass.TEMP_setParameterValueTexture( realObject, name, texture.realObjectPtr, addressingMode, filteringMin, filteringMag, filteringMip, ref borderColor );
		//	}
		//}

		//!!!!
		//public unsafe void TEMP_SetParameterValues( IList<ParameterContainer> additionalContainers )
		//{
		//	//!!!!!пока так
		//	EngineThreading.CheckMainThread();

		//	List<ParameterContainer> allContainers = new List<ParameterContainer>();
		//	if( constantParameterValues != null )
		//		allContainers.Add( constantParameterValues );
		//	allContainers.AddRange( additionalContainers );

		//	foreach( AvailableParameterItem item in AvailableParameters )
		//	{
		//		ParameterContainer.ParameterItem containerItem = GetValueFromContainers( allContainers, item.name );
		//		if( containerItem == null )
		//		{
		//			//!!!!!
		//			Log.Fatal( "Parameter value with name \'{0}\' is not exists in containers.", item.name );
		//		}

		//		if( ParameterTypeUtils.IsTexture( item.type ) )
		//		{
		//			//!!!!arrays

		//			//GpuMaterialPass.TextureParameterValue textureValue = null;
		//			//if( value.GetType().IsArray )
		//			//{
		//			//	Array array = (Array)value;
		//			//	if( array.GetLength( 0 ) > 0 )
		//			//		textureValue = array.GetValue( 0 ) as GpuMaterialPass.TextureParameterValue;
		//			//}
		//			//else
		//			//	textureValue = value as GpuMaterialPass.TextureParameterValue;


		//			TextureParameterValue v = (TextureParameterValue)containerItem.Value;

		//			//!!!! var d = v.texture != null ? v.texture.Result : null;
		//			var d = ResourceUtils.GetTextureCompiledData( v.texture );
		//			if( d == null )
		//			{
		//				//!!!!!

		//				d = ResourceUtils.WhiteTexture2D.Result;

		//				//Log.Fatal( "impl" );
		//			}

		//			TEMP_setParameterValueTexture( item.name, d, v.addressingMode, v.filteringMin, v.filteringMag, v.filteringMip, v.borderColor );
		//		}
		//		else //!!!!проверить, если что-то совсем не то в контейнере лежит. не поддерживаемое что-то
		//		{
		//			byte[] a = new byte[ containerItem.GetTotalSizeInBytes() ];
		//			containerItem.GetValue( a, 0 );
		//			fixed ( byte* pA = a )
		//			{
		//				TEMP_setParameterValue( item.name, pA, a.Length );
		//			}
		//		}
		//	}

		//	//!!!!!!
		//	OgrePass.TEMP_applyParameterValues( realObject );
		//}

		//!!!!not used
		//public ParameterContainer ConstantParameterValues
		//{
		//	get { return constantParameterValues; }
		//}

		static RenderState ConvertBlendFactor( SceneBlendFactor factor )
		{
			//!!!!еще параметры?

			switch( factor )
			{
			case SceneBlendFactor.One: return RenderState.BlendOne;
			case SceneBlendFactor.Zero: return RenderState.BlendZero;
			case SceneBlendFactor.DestinationColor: return RenderState.BlendDestinationColor;
			case SceneBlendFactor.SourceColor: return RenderState.BlendSourceColor;
			case SceneBlendFactor.DestinationAlpha: return RenderState.BlendDestinationAlpha;
			case SceneBlendFactor.SourceAlpha: return RenderState.BlendSourceAlpha;
			case SceneBlendFactor.OneMinusDestinationAlpha: return RenderState.BlendInverseDestinationAlpha;
			case SceneBlendFactor.OneMinusSourceAlpha: return RenderState.BlendInverseSourceAlpha;
			}

			Log.Fatal( "GpuMaterialPass: ConvertBlendFactor: internal error." );
			return 0;
		}

		static RenderState ConvertRenderOperation( RenderOperationType renderOperation )
		{
			switch( renderOperation )
			{
			case RenderOperationType.TriangleList: return RenderState.PrimitiveTriangles;
			case RenderOperationType.PointList: return RenderState.PrimitivePoints;
			case RenderOperationType.LineList: return RenderState.PrimitiveLines;
			case RenderOperationType.LineStrip: return RenderState.PrimitiveLineStrip;
			case RenderOperationType.TriangleStrip: return RenderState.PrimitiveTriangleStrip;
			}

			Log.Fatal( "GpuMaterialPass: ConvertRenderOperation: internal error." );
			return RenderState.PrimitiveTriangles;
		}

		void Update()
		{
			RenderState state = RenderState.None;

			//switch( PolygonMode )
			//{
			//case PolygonMode.Points:
			//	break;
			//case PolygonMode.Wireframe:
			//	break;
			//case PolygonMode.Solid:
			//	break;

			//	//Points = 1,//Ogre::PM_POINTS,
			//	//Wireframe = 2,//Ogre::PM_WIREFRAME,
			//	//Solid = 3,//Ogre::PM_SOLID,
			//}

			//culling
			switch( CullingMode )
			{
			case CullingMode.None: state |= RenderState.NoCulling; break;
			case CullingMode.Clockwise: state |= RenderState.CullClockwise; break;
			case CullingMode.Anticlockwise: state |= RenderState.CullCounterclockwise; break;
			}

			//depth check
			if( depthCheck )
			{
				switch( depthFunction )
				{
				case CompareFunction.AlwaysFail: state |= RenderState.DepthTestNever; break;
				case CompareFunction.AlwaysPass: state |= RenderState.DepthTestAlways; break;
				case CompareFunction.Less: state |= RenderState.DepthTestLess; break;
				case CompareFunction.LessEqual: state |= RenderState.DepthTestLessEqual; break;
				case CompareFunction.Equal: state |= RenderState.DepthTestEqual; break;
				case CompareFunction.NotEqual: state |= RenderState.DepthTestNotEqual; break;
				case CompareFunction.GreaterEqual: state |= RenderState.DepthTestGreaterEqual; break;
				case CompareFunction.Greater: state |= RenderState.DepthTestGreater; break;
				}
			}
			else
				state |= RenderState.DepthTestAlways;

			//depth write
			if( DepthWrite )
				state |= RenderState.WriteZ;

			//color write
			if( ColorWriteRed )
				state |= RenderState.WriteR;
			if( ColorWriteGreen )
				state |= RenderState.WriteG;
			if( ColorWriteBlue )
				state |= RenderState.WriteB;
			if( ColorWriteAlpha )
				state |= RenderState.WriteA;

			//!!!!более продвинутый блендинг
			//blending
			var blendSource = ConvertBlendFactor( sourceBlendFactor );
			var blendDestination = ConvertBlendFactor( destinationBlendFactor );

			//!!!!
			//blendSource = RenderState.BlendOne;
			//blendDestination = RenderState.BlendZero;

			////!!!!
			//state |= RenderState.Multisampling;
			////!!!!
			//if( _tempAlphaToCoverage )
			//{
			//	state |= RenderState.BlendAlphaToCoverage;
			//}
			//////!!!!
			//state |= RenderState.LineAA;

			state |= RenderState.BlendFunction( blendSource, blendDestination );

			//!!!!видать в шейдерах. в текстурах что-то еще есть про блендинг
			////CompareFunction alphaRejectFunction = CompareFunction.AlwaysPass;
			////int alphaRejectValue = 0;

			////!!!!было
			////AvailableParameterItem[] availableParameters;
			////Dictionary<string, AvailableParameterItem> availableParameterByName;

			////!!!!
			////!!!!threading
			//ParameterContainer constantParameterValues = new ParameterContainer();

			needUpdate = false;
			cachedState = state;

			//!!!!
			///// <summary>
			///// Use one minus the destination pixel color as an input to a blend equation.
			///// </summary>
			//public static readonly RenderState BlendInverseDestinationColor = 0x000000000000a000;

			///// <summary>
			///// Use the source pixel alpha (saturated) as an input to a blend equation.
			///// </summary>
			//public static readonly RenderState BlendSourceAlphaSaturate = 0x000000000000b000;

			///// <summary>
			///// Use an application supplied blending factor as an input to a blend equation.
			///// </summary>
			//public static readonly RenderState BlendFactor = 0x000000000000c000;

			///// <summary>
			///// Use one minus an application supplied blending factor as an input to a blend equation.
			///// </summary>
			//public static readonly RenderState BlendInverseFactor = 0x000000000000d000;

			///// <summary>
			///// Blend equation: A + B
			///// </summary>
			//public static readonly RenderState BlendEquationAdd = 0x0000000000000000;

			///// <summary>
			///// Blend equation: B - A
			///// </summary>
			//public static readonly RenderState BlendEquationSub = 0x0000000010000000;

			///// <summary>
			///// Blend equation: A - B
			///// </summary>
			//public static readonly RenderState BlendEquationReverseSub = 0x0000000020000000;

			///// <summary>
			///// Blend equation: min(a, b)
			///// </summary>
			//public static readonly RenderState BlendEquationMin = 0x0000000030000000;

			///// <summary>
			///// Blend equation: max(a, b)
			///// </summary>
			//public static readonly RenderState BlendEquationMax = 0x0000000040000000;

			///// <summary>
			///// Enable independent blending of simultaenous render targets.
			///// </summary>
			//public static readonly RenderState BlendIndependent = 0x0000000400000000;

			///// <summary>
			///// Enable alpha to coverage blending.
			///// </summary>
			//public static readonly RenderState BlendAlphaToCoverage = 0x0000000800000000;


			///// <summary>
			///// Enable multisampling.
			///// </summary>
			//public static readonly RenderState Multisampling = 0x1000000000000000;

			///// <summary>
			///// Enable line antialiasing.
			///// </summary>
			//public static readonly RenderState LineAA = 0x2000000000000000;

			///// <summary>
			///// Enable conservative rasterization.
			///// </summary>
			//public static readonly RenderState ConservativeRasterization = 0x4000000000000000;



			///// <summary>
			///// Predefined blend effect: additive blending.
			///// </summary>
			//public static readonly RenderState BlendAdd = BlendFunction( BlendOne, BlendOne );

			///// <summary>
			///// Predefined blend effect: alpha blending.
			///// </summary>
			//public static readonly RenderState BlendAlpha = BlendFunction( BlendSourceAlpha, BlendInverseSourceAlpha );

			///// <summary>
			///// Predefined blend effect: "darken" blending.
			///// </summary>
			//public static readonly RenderState BlendDarken = BlendFunction( BlendOne, BlendOne ) | BlendEquation( BlendEquationMin );

			///// <summary>
			///// Predefined blend effect: "lighten" blending.
			///// </summary>
			//public static readonly RenderState BlendLighten = BlendFunction( BlendOne, BlendOne ) | BlendEquation( BlendEquationMax );

			///// <summary>
			///// Predefined blend effect: multiplicative blending.
			///// </summary>
			//public static readonly RenderState BlendMultiply = BlendFunction( BlendDestinationColor, BlendZero );

			///// <summary>
			///// Predefined blend effect: normal blending based on alpha.
			///// </summary>
			//public static readonly RenderState BlendNormal = BlendFunction( BlendOne, BlendInverseSourceAlpha );

			///// <summary>
			///// Predefined blend effect: "screen" blending.
			///// </summary>
			//public static readonly RenderState BlendScreen = BlendFunction( BlendOne, BlendInverseSourceColor );

		}

		internal void RenderingProcess_SetRenderState( RenderOperationType renderOperation, bool canWriteRGBA )
		{
			if( needUpdate )
				Update();

			//!!!!что-то еще выставлять?

			var renderOperationState = ConvertRenderOperation( renderOperation );

			RenderState state = cachedState | renderOperationState;
			int colorRGBA = 0;
			// <param name="colorRgba">The color used for "factor" blending modes.</param>

			if( advancedBlendingWriteMask != uint.MaxValue )
			{
				state |= RenderState.BlendIndependent;
				colorRGBA = (int)advancedBlendingWriteMask;
			}

			if( !canWriteRGBA )
				state &= ~( RenderState.WriteRGB | RenderState.WriteA );

			Bgfx.SetRenderState( state, colorRGBA );

			//Bgfx.SetRenderState( cachedState | renderOperationState | RenderState.Multisampling, colorRGBA );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public GpuMaterialPass( GpuProgram vertexProgram, GpuProgram fragmentProgram )
		{
			//!!!!может не сразу получать
			linkedProgram = GpuProgramManager.GetLinkedProgram( vertexProgram, fragmentProgram );
		}

		////!!!!!возможность свободные пассы создавать?
		////!!!!!more program types
		//public static GpuMaterialPass CreatePass( GpuLinkedProgram linkedProgram )
		//{
		//	var pass = new GpuMaterialPass( linkedProgram );
		//	return pass;

		//	////!!!!!!пока так
		//	//EngineThreading.CheckMainThread();

		//	//unsafe
		//	//{
		//	//	OgrePass* realPass = (OgrePass*)OgrePass.New();
		//	//	_old_GpuProgram[] programs = new _old_GpuProgram[] { vertexProgram, fragmentProgram, geometryProgram };
		//	//	GpuMaterialPass pass = new GpuMaterialPass( realPass, programs );
		//	//	//passes.Add( pass );
		//	//	return pass;
		//	//}
		//}

		//public static GpuMaterialPass CreatePass( GpuProgram vertexProgram, GpuProgram fragmentProgram )//, GpuProgram geometryProgram = null )
		//{
		//	var linkedProgram = GpuProgramManager.GetLinkedProgram( vertexProgram, fragmentProgram );
		//	return CreatePass( linkedProgram );
		//}
	}
}
