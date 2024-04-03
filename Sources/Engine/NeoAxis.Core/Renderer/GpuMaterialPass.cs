// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Specifies the material settings when rendering.
	/// </summary>
	public class GpuMaterialPass
	{
		//public static bool GlobalComposeOIT;

		Material.CompiledMaterialData owner;
		GpuLinkedProgram linkedProgram;
		//GpuProgram vertexProgram;
		//GpuProgram fragmentProgram;

		//bool lighting = false;

		//blending
		SceneBlendFactor sourceBlendFactor = SceneBlendFactor.One;
		SceneBlendFactor destinationBlendFactor = SceneBlendFactor.Zero;
		//!!!!separate alpha blending
		//!!!!_D3D11_RENDER_TARGET_BLEND_DESC1

		bool depthCheck = true;
		bool depthWrite = true;
		CompareFunction depthFunction = CompareFunction.LessEqual;
		CullingMode cullingMode = CullingMode.Clockwise;
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

		public GpuMaterialPass( Material.CompiledMaterialData owner, GpuProgram vertexProgram, GpuProgram fragmentProgram )
		{
			this.owner = owner;

			//!!!!может не сразу получать
			linkedProgram = GpuProgramManager.GetLinkedProgram( vertexProgram, fragmentProgram );
		}

		//internal unsafe GpuMaterialPass( GpuLinkedProgram linkedProgram )
		//{
		//	this.linkedProgram = linkedProgram;
		//}

		public Material.CompiledMaterialData Owner
		{
			get { return owner; }
		}

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

		//!!!!not used
		//public ParameterContainer ConstantParameterValues
		//{
		//	get { return constantParameterValues; }
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static RenderState ConvertBlendFactor( SceneBlendFactor factor )
		{
			//!!!!use array by index

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static RenderState ConvertRenderOperation( RenderOperationType renderOperation )
		{
			//!!!!use array by index

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
				if( RenderingSystem.ReversedZ )
				{
					switch( depthFunction )
					{
					case CompareFunction.AlwaysFail: state |= RenderState.DepthTestNever; break;
					case CompareFunction.AlwaysPass: state |= RenderState.DepthTestAlways; break;
					case CompareFunction.Less: state |= RenderState.DepthTestGreater; break;
					case CompareFunction.LessEqual: state |= RenderState.DepthTestGreaterEqual; break;
					case CompareFunction.Equal: state |= RenderState.DepthTestEqual; break;
					case CompareFunction.NotEqual: state |= RenderState.DepthTestNotEqual; break;
					case CompareFunction.GreaterEqual: state |= RenderState.DepthTestLessEqual; break;
					case CompareFunction.Greater: state |= RenderState.DepthTestLess; break;
					}
				}
				else
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

			needUpdate = false;
			cachedState = state;






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

			//blendSource = RenderState.BlendOne;
			//blendDestination = RenderState.BlendZero;

			//state |= RenderState.Multisampling;

			//if( _tempAlphaToCoverage )
			//{
			//	state |= RenderState.BlendAlphaToCoverage;
			//}

			//state |= RenderState.LineAA;

			//!!!!видать в шейдерах. в текстурах что-то еще есть про блендинг
			////CompareFunction alphaRejectFunction = CompareFunction.AlwaysPass;
			////int alphaRejectValue = 0;

			////!!!!было
			////AvailableParameterItem[] availableParameters;
			////Dictionary<string, AvailableParameterItem> availableParameterByName;

			////!!!!
			////!!!!threading
			//ParameterContainer constantParameterValues = new ParameterContainer();

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void RenderingProcess_SetRenderState( RenderOperationType renderOperation, bool canWriteRGBA, bool voxelRendering )
		{
			if( needUpdate )
				Update();

			//render operation
			var renderOperationState = ConvertRenderOperation( renderOperation );

			//blending
			RenderState blending;
			//if( GlobalComposeOIT )
			//	blending = RenderState.BlendFunction( RenderState.BlendOne, RenderState.BlendOne );
			//else
			{
				var blendSource = ConvertBlendFactor( sourceBlendFactor );
				var blendDestination = ConvertBlendFactor( destinationBlendFactor );
				blending = RenderState.BlendFunction( blendSource, blendDestination );
			}

			var state = cachedState | renderOperationState | blending;
			int colorRGBA = 0;

			if( advancedBlendingWriteMask != uint.MaxValue )
			{
				state |= RenderState.BlendIndependent;
				colorRGBA = (int)advancedBlendingWriteMask;
			}

			//if( GlobalComposeOIT )
			//{
			//	state |= RenderState.BlendIndependent;
			//	colorRGBA = 49;//BGFX_STATE_BLEND_FUNC_RT_1( BGFX_STATE_BLEND_ZERO, BGFX_STATE_BLEND_SRC_COLOR )

			//	//mrt
			//	//// Set render states.
			//	//bgfx::setState( stateNoDepth
			//	//	| BGFX_STATE_BLEND_FUNC( BGFX_STATE_BLEND_ONE, BGFX_STATE_BLEND_ONE ) | BGFX_STATE_BLEND_INDEPENDENT
			//	//	, BGFX_STATE_BLEND_FUNC_RT_1( BGFX_STATE_BLEND_ZERO, BGFX_STATE_BLEND_SRC_COLOR )
			//	//);

			//	////separate
			//	////bgfx::setState( stateNoDepth
			//	////| BGFX_STATE_BLEND_FUNC_SEPARATE( BGFX_STATE_BLEND_ONE, BGFX_STATE_BLEND_ONE, BGFX_STATE_BLEND_ZERO, BGFX_STATE_BLEND_INV_SRC_ALPHA )
			//	////);
			//}


			if( !canWriteRGBA )
				state &= ~( RenderState.WriteRGB | RenderState.WriteA );

			if( voxelRendering )
			{
				state &= ~( RenderState.NoCulling | RenderState.CullCounterclockwise );
				state |= RenderState.CullClockwise;
			}

			Bgfx.SetRenderState( state, colorRGBA );

			//Bgfx.SetRenderState( cachedState | renderOperationState | RenderState.Multisampling, colorRGBA );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public struct GpuMaterialPassGroup
	{
		public GpuMaterialPass Usual;
		public GpuMaterialPass Voxel;
		//public GpuMaterialPass Virtualized;
		public GpuMaterialPass Billboard;

		//

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public GpuMaterialPass Get( bool voxel/*, bool virtualized*/, bool billboard )
		{
			if( voxel )
				return Voxel;
			//else if( virtualized )
			//	return Virtualized;
			else if( billboard )
				return Billboard;
			else
				return Usual;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Set( GpuMaterialPass pass, bool voxel, /*bool virtualized, */bool billboard )
		{
			if( voxel )
				Voxel = pass;
			//else if( virtualized )
			//	Virtualized = pass;
			else if( billboard )
				Billboard = pass;
			else
				Usual = pass;
		}
	}
}
