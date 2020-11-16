// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Depth of field screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 7 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_DepthOfField : Component_RenderingEffect
	{
		//!!!!downscale
		//!!!!auto focus

		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The distance at which to focus to achieve the maximum effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 50.0 )]
		[Range( 0, 300 )]
		public Reference<double> FocalDistance
		{
			get { if( _focalDistance.BeginGet() ) FocalDistance = _focalDistance.Get( this ); return _focalDistance.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _focalDistance.BeginSet( ref value ) ) { try { FocalDistanceChanged?.Invoke( this ); } finally { _focalDistance.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="FocalDistance"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> FocalDistanceChanged;
		ReferenceField<double> _focalDistance = 50;


		//bool autoFocus = true;
		//[DefaultValue( true )]
		//public bool AutoFocus
		//{
		//	get { return autoFocus; }
		//	set { autoFocus = value; }
		//}


		/// <summary>
		/// The focal size.
		/// </summary>
		[Serialize]
		[DefaultValue( 20.0 )]
		[Range( 0, 100 )]
		public Reference<double> FocalSize
		{
			get { if( _focalSize.BeginGet() ) FocalSize = _focalSize.Get( this ); return _focalSize.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _focalSize.BeginSet( ref value ) ) { try { FocalSizeChanged?.Invoke( this ); } finally { _focalSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="FocalSize"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> FocalSizeChanged;
		ReferenceField<double> _focalSize = 20;


		//!!!!
		////BlurTextureResolution
		//ReferenceField<double> blurTextureResolution = 6;
		//[Serialize]
		//[DefaultValue( 6.0 )]
		////[Editor( typeof( SingleValueEditor ), typeof( UITypeEditor ) )]
		////[EditorLimitsRange( 1, 7 )]
		//public virtual Reference<double> BlurTextureResolution
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( blurTextureResolution.GetByReference ) )
		//			BlurTextureResolution = blurTextureResolution.GetValue( this );
		//		return blurTextureResolution;
		//	}
		//	set
		//	{
		//		if( value < 1 )
		//			value = 1;
		//		if( value > 7 )
		//			value = 7;
		//		if( blurTextureResolution == value ) return;
		//		blurTextureResolution = value;
		//		BlurTextureResolutionChanged?.Invoke( this );
		//	}
		//}
		//public event Action<Component_RenderingEffect_DepthOfField> BlurTextureResolutionChanged;

		/// <summary>
		/// Distance beyound 1.5 focal lengths over which objects will become completely blured.
		/// </summary>
		[Serialize]
		[DefaultValue( 100.0 )]
		[Range( 0, 200 )]
		public Reference<double> BackgroundTransitionLength
		{
			get { if( _backgroundTransitionLength.BeginGet() ) BackgroundTransitionLength = _backgroundTransitionLength.Get( this ); return _backgroundTransitionLength.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _backgroundTransitionLength.BeginSet( ref value ) ) { try { BackgroundTransitionLengthChanged?.Invoke( this ); } finally { _backgroundTransitionLength.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="BackgroundTransitionLength"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> BackgroundTransitionLengthChanged;
		ReferenceField<double> _backgroundTransitionLength = 100;

		/// <summary>
		/// Whether the foreground is blured.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> BlurForeground
		{
			get { if( _blurForeground.BeginGet() ) BlurForeground = _blurForeground.Get( this ); return _blurForeground.value; }
			set { if( _blurForeground.BeginSet( ref value ) ) { try { BlurForegroundChanged?.Invoke( this ); } finally { _blurForeground.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurForeground"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> BlurForegroundChanged;
		ReferenceField<bool> _blurForeground = false;

		/// <summary>
		/// Distance before 0.5 focal lengths over which objects will become completely sharp.
		/// </summary>
		[Serialize]
		[DefaultValue( 40.0 )]
		[Range( 0, 100 )]
		public Reference<double> ForegroundTransitionLength
		{
			get { if( _foregroundTransitionLength.BeginGet() ) ForegroundTransitionLength = _foregroundTransitionLength.Get( this ); return _foregroundTransitionLength.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _foregroundTransitionLength.BeginSet( ref value ) ) { try { ForegroundTransitionLengthChanged?.Invoke( this ); } finally { _foregroundTransitionLength.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="ForegroundTransitionLength"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> ForegroundTransitionLengthChanged;
		ReferenceField<double> _foregroundTransitionLength = 40;

		/// <summary>
		/// The amount of blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 1;

		/// <summary>
		/// The blur downscaling mode used.
		/// </summary>
		[DefaultValue( Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		[Serialize]
		public Reference<Component_RenderingPipeline_Basic.DownscalingModeEnum> BlurDownscalingMode
		{
			get { if( _blurDownscalingMode.BeginGet() ) BlurDownscalingMode = _blurDownscalingMode.Get( this ); return _blurDownscalingMode.value; }
			set { if( _blurDownscalingMode.BeginSet( ref value ) ) { try { BlurDownscalingModeChanged?.Invoke( this ); } finally { _blurDownscalingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingMode"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> BlurDownscalingModeChanged;
		ReferenceField<Component_RenderingPipeline_Basic.DownscalingModeEnum> _blurDownscalingMode = Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level blur downscaling.
		/// </summary>
		[DefaultValue( 0 )]
		[Serialize]
		[Range( 0, 6 )]
		public Reference<int> BlurDownscalingValue
		{
			get { if( _blurDownscalingValue.BeginGet() ) BlurDownscalingValue = _blurDownscalingValue.Get( this ); return _blurDownscalingValue.value; }
			set { if( _blurDownscalingValue.BeginSet( ref value ) ) { try { BlurDownscalingValueChanged?.Invoke( this ); } finally { _blurDownscalingValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingValue"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_DepthOfField> BlurDownscalingValueChanged;
		ReferenceField<int> _blurDownscalingValue = 0;


		//Range autoFocusRange = new Range( 1, 70 );
		//[DefaultValue( typeof( Range ), "1 70" )]
		//[Editor( typeof( RangeValueEditor ), typeof( UITypeEditor ) )]
		//[EditorLimitsRange( 1, 200 )]
		//public Range AutoFocusRange
		//{
		//	get { return autoFocusRange; }
		//	set { autoFocusRange = value; }
		//}


		//float autoFocusTransitionSpeed = 100;
		//[DefaultValue( 100.0f )]
		//[Editor( typeof( SingleValueEditor ), typeof( UITypeEditor ) )]
		//[EditorLimitsRange( 1, 500 )]
		//public float AutoFocusTransitionSpeed
		//{
		//	get { return autoFocusTransitionSpeed; }
		//	set
		//	{
		//		if( value < 0 )
		//			value = 0;
		//		autoFocusTransitionSpeed = value;
		//	}
		//}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( ForegroundTransitionLength ):
					if( !BlurForeground.Value )
						skip = true;
					break;

				case nameof( BlurDownscalingValue ):
					if( BlurDownscalingMode.Value == Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			//blur
			var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
			var blurTexture = pipeline.GaussianBlur( context, this, actualTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );
			//context.RenderTarget_Free( brightTexture );

			////horizontal blur
			//var texture1 = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.Format );
			//{
			//	context.SetViewport( texture1.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";
			//	//shader.FragmentProgramFileName = @"Base\Shaders\Effects\DepthOfField\Blur_fs.sc";

			//	shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			//		TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
			//	BlurSetShaderParameters( context, shader, true );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			////vertical blur
			//var blurTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, texture1.Result.Format );
			//{
			//	context.SetViewport( blurTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";
			//	//shader.FragmentProgramFileName = @"Base\Shaders\Effects\DepthOfField\Blur_fs.sc";

			//	shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( texture1,
			//		TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
			//	BlurSetShaderParameters( context, shader, false );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			////free old texture
			//context.RenderTarget_Free( texture1 );

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, blurTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\DepthOfField\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"blurTexture"*/, blurTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out Component_Image depthTexture );
				if( depthTexture == null )
				{
					//!!!!
				}
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/*"depthTexture"*/, depthTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				Vector4 properties = new Vector4( FocalDistance, FocalSize, BackgroundTransitionLength, BlurForeground ? ForegroundTransitionLength : -1 );
				shader.Parameters.Set( "u_depthOfFieldProperties", properties.ToVector4F() );
				shader.Parameters.Set( "intensity", (float)Intensity );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );
			context.DynamicTexture_Free( blurTexture );

			//update actual texture
			actualTexture = finalTexture;
		}




		//Vec2I downscaleTextureSize;


		//protected override void OnCreateTexture( string definitionName, ref Vec2I size, ref PixelFormat format )
		//{
		//	base.OnCreateTexture( definitionName, ref size, ref format );

		//	if( definitionName == "rt_downscale" || definitionName == "rt_blurHorizontal" ||
		//		definitionName == "rt_blurVertical" )
		//	{
		//		float divisor = 8.0f - blurTextureResolution;
		//		if( divisor <= 1 )
		//			divisor = 1;
		//		Vec2 sizeFloat = Owner.DimensionsInPixels.Size.ToVec2() / divisor;
		//		size = new Vec2I( (int)sizeFloat.X, (int)sizeFloat.Y );
		//		if( size.X < 1 )
		//			size.X = 1;
		//		if( size.Y < 1 )
		//			size.Y = 1;

		//		downscaleTextureSize = size;
		//	}
		//}


		//static void CalculateDownScale4x4SampleOffsets( Vec2I sourceTextureSize, Vec2F[] sampleOffsets )
		//{
		//	// Sample from the 16 surrounding points. Since the center point will be in
		//	// the exact center of 16 texels, a 0.5f offset is needed to specify a texel
		//	// center.
		//	Vec2F invSize = 1.0f / sourceTextureSize.ToVec2F();
		//	int index = 0;
		//	for( int y = 0; y < 4; y++ )
		//	{
		//		for( int x = 0; x < 4; x++ )
		//		{
		//			sampleOffsets[ index ] = new Vec2F( ( (float)x - 1.5f ), ( (float)y - 1.5f ) ) * invSize;
		//			index++;
		//		}
		//	}
		//}


		//static void CalculateGaussianBlur5x5SampleOffsets( Vec2I textureSize, Vec2F[] sampleOffsets, Vec4F[] sampleWeights, float multiplier )
		//{
		//	float tu = 1.0f / (float)textureSize.X;
		//	float tv = 1.0f / (float)textureSize.Y;

		//	Vec4F white = new Vec4F( 1, 1, 1, 1 );

		//	float totalWeight = 0.0f;
		//	int index = 0;
		//	for( int x = -2; x <= 2; x++ )
		//	{
		//		for( int y = -2; y <= 2; y++ )
		//		{
		//			// Exclude pixels with a block distance greater than 2. This will
		//			// create a kernel which approximates a 5x5 kernel using only 13
		//			// sample points instead of 25; this is necessary since 2.0 shaders
		//			// only support 16 texture grabs.
		//			if( Math.Abs( x ) + Math.Abs( y ) > 2 )
		//				continue;

		//			// Get the unscaled Gaussian intensity for this offset
		//			sampleOffsets[ index ] = new Vec2F( x * tu, y * tv );
		//			sampleWeights[ index ] = white * (float)GaussianDistribution( x, y, 1 );
		//			totalWeight += sampleWeights[ index ].X;

		//			index++;
		//		}
		//	}

		//	// Divide the current weight by the total weight of all the samples; Gaussian
		//	// blur kernels add to 1.0f to ensure that the intensity of the image isn't
		//	// changed when the blur occurs. An optional multiplier variable is used to
		//	// add or remove image intensity during the blur.
		//	for( int i = 0; i < index; i++ )
		//	{
		//		sampleWeights[ i ] /= totalWeight;
		//		sampleWeights[ i ] *= multiplier;
		//	}
		//}

		//static void CalculateBlurSampleOffsets( int textureSize, float[] sampleOffsets, Vec4F[] sampleWeights, float deviation, float multiplier )
		//{
		//	float tu = 1.0f / (float)textureSize;

		//	// Fill the center texel
		//	{
		//		float weight = multiplier * (float)GaussianDistribution( 0, 0, deviation );
		//		sampleOffsets[ 0 ] = 0.0f;
		//		sampleWeights[ 0 ] = new Vec4F( weight, weight, weight, 1.0f );
		//	}

		//	// Fill the first half
		//	for( int n = 1; n < 8; n++ )
		//	{
		//		// Get the Gaussian intensity for this offset
		//		float weight = multiplier * (float)GaussianDistribution( n, 0, deviation );
		//		sampleOffsets[ n ] = n * tu;
		//		sampleWeights[ n ] = new Vec4F( weight, weight, weight, 1.0f );
		//	}

		//	// Mirror to the second half
		//	for( int n = 8; n < 15; n++ )
		//	{
		//		sampleOffsets[ n ] = -sampleOffsets[ n - 7 ];
		//		sampleWeights[ n ] = sampleWeights[ n - 7 ];
		//	}

		//	//normalize weights (fix epsilon errors)
		//	{
		//		Vec4F total = Vec4F.Zero;
		//		for( int n = 0; n < sampleWeights.Length; n++ )
		//			total += sampleWeights[ n ];
		//		for( int n = 0; n < sampleWeights.Length; n++ )
		//			sampleWeights[ n ] = ( sampleWeights[ n ] / total ) * multiplier;
		//	}
		//}


		//protected override void OnMaterialRender( uint passId, Material material, ref bool skipPass )
		//{
		//	base.OnMaterialRender( passId, material, ref skipPass );

		//	const int rt_downscale = 100;
		//	const int rt_blurHorizontal = 200;
		//	const int rt_blurVertical = 300;
		//	const int rt_autoFocus1 = 400;
		//	const int rt_autoFocus2 = 401;
		//	const int rt_autoFocus3 = 402;
		//	const int rt_autoFocusFinal = 403;
		//	const int rt_autoFocusCurrent = 404;
		//	//const int rt_blurFactors = 500;
		//	const int rt_targetOutput = 600;

		//	//Skip auto focus passes if no auto focus is enabled
		//	if( !autoFocus )
		//	{
		//		if( passId == rt_autoFocus1 || passId == rt_autoFocus2 || passId == rt_autoFocus3 ||
		//			passId == rt_autoFocusFinal || passId == rt_autoFocusCurrent )
		//		{
		//			skipPass = true;
		//			return;
		//		}
		//	}

		//	// Prepare the fragment params offsets
		//	switch( passId )
		//	{
		//	case rt_downscale:
		//		{
		//			Vec2[] sampleOffsets = new Vec2[ 16 ];

		//			CalculateDownScale4x4SampleOffsets( Owner.DimensionsInPixels.Size, sampleOffsets );

		//			//convert to Vec4 array
		//			Vec4[] vec4Offsets = new Vec4[ 16 ];
		//			for( int n = 0; n < 16; n++ )
		//			{
		//				Vec2 offset = sampleOffsets[ n ];
		//				vec4Offsets[ n ] = new Vec4( offset[ 0 ], offset[ 1 ], 0, 0 );
		//			}

		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;
		//			parameters.SetNamedConstant( "sampleOffsets", vec4Offsets );
		//		}
		//		break;

		//	case rt_blurHorizontal:
		//	case rt_blurVertical:
		//		{
		//			// horizontal and vertical blur
		//			bool horizontal = passId == rt_blurHorizontal;

		//			float[] sampleOffsets = new float[ 15 ];
		//			Vec4[] sampleWeights = new Vec4[ 15 ];

		//			CalculateBlurSampleOffsets( horizontal ? downscaleTextureSize.X : downscaleTextureSize.Y,
		//				sampleOffsets, sampleWeights, 3, 1 );

		//			//convert to Vec4 array
		//			Vec4[] vec4Offsets = new Vec4[ 15 ];
		//			for( int n = 0; n < 15; n++ )
		//			{
		//				float offset = sampleOffsets[ n ] * blurFactor;

		//				if( horizontal )
		//					vec4Offsets[ n ] = new Vec4( offset, 0, 0, 0 );
		//				else
		//					vec4Offsets[ n ] = new Vec4( 0, offset, 0, 0 );
		//			}

		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;
		//			parameters.SetNamedConstant( "sampleOffsets", vec4Offsets );
		//			parameters.SetNamedConstant( "sampleWeights", sampleWeights );
		//		}
		//		break;

		//	case rt_autoFocus1:
		//		{
		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;
		//			parameters.SetNamedAutoConstant( "farClipDistance",
		//				GpuProgramParameters.AutoConstantType.FarClipDistance );
		//		}
		//		break;

		//	case rt_autoFocus2:
		//	case rt_autoFocus3:
		//		{
		//			Vec2[] sampleOffsets = new Vec2[ 16 ];

		//			string textureSizeFrom = null;
		//			switch( passId )
		//			{
		//			case rt_autoFocus2: textureSizeFrom = "rt_autoFocus1"; break;
		//			case rt_autoFocus3: textureSizeFrom = "rt_autoFocus2"; break;
		//			default: Trace.Assert( false ); break;
		//			}
		//			Vec2I sourceTextureSize = Technique.GetTextureDefinition( textureSizeFrom ).Size;
		//			CalculateDownScale4x4SampleOffsets( sourceTextureSize, sampleOffsets );

		//			//convert to Vec4 array
		//			Vec4[] vec4Offsets = new Vec4[ 16 ];
		//			for( int n = 0; n < 16; n++ )
		//			{
		//				Vec2 offset = sampleOffsets[ n ];
		//				vec4Offsets[ n ] = new Vec4( offset[ 0 ], offset[ 1 ], 0, 0 );
		//			}

		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;
		//			parameters.SetNamedConstant( "sampleOffsets", vec4Offsets );
		//		}
		//		break;

		//	case rt_autoFocusFinal:
		//		{
		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;

		//			Vec4 properties = Vec4.Zero;
		//			properties.X = autoFocusRange.Minimum;
		//			properties.Y = autoFocusRange.Maximum;
		//			properties.Z = RendererWorld.FrameRenderTimeStep * autoFocusTransitionSpeed;
		//			parameters.SetNamedConstant( "properties", properties );
		//		}
		//		break;

		//	case rt_autoFocusCurrent:
		//		break;

		//	//case rt_blurFactors:
		//	//   {
		//	//      GpuProgramParameters parameters = material.GetBestTechnique().
		//	//         Passes[ 0 ].FragmentProgramParameters;
		//	//      parameters.SetNamedAutoConstant( "farClipDistance",
		//	//         GpuProgramParameters.AutoConstantType.FarClipDistance );

		//	//      Vec4 properties = Vec4.Zero;
		//	//      properties.X = autoFocus ? -1.0f : focalDistance;
		//	//      properties.Y = focalSize;
		//	//      properties.Z = backgroundTransitionLength;
		//	//      properties.W = blurForeground ? foregroundTransitionLength : -1;
		//	//      parameters.SetNamedConstant( "properties", properties );
		//	//   }
		//	//   break;

		//	//Final pass
		//	case rt_targetOutput:
		//		{
		//			GpuProgramParameters parameters = material.GetBestTechnique().
		//				Passes[ 0 ].FragmentProgramParameters;

		//			parameters.SetNamedAutoConstant( "farClipDistance",
		//				GpuProgramParameters.AutoConstantType.FarClipDistance );

		//			Vec4 properties = Vec4.Zero;
		//			properties.X = autoFocus ? -1.0f : focalDistance;
		//			properties.Y = focalSize;
		//			properties.Z = backgroundTransitionLength;
		//			properties.W = blurForeground ? foregroundTransitionLength : -1;
		//			parameters.SetNamedConstant( "properties", properties );

		//			//Vec2[] sampleOffsets = new Vec2[ 49 ];
		//			//Vec2 textureSize = Owner.DimensionsInPixels.Size.ToVec2();
		//			//for( int y = -3; y <= 3; y++ )
		//			//   for( int x = -3; x <= 3; x++ )
		//			//      sampleOffsets[ ( y + 3 ) * 7 + ( x + 3 ) ] = new Vec2( x, y ) / textureSize;

		//			////convert to Vec4 array
		//			//Vec4[] vec4Offsets = new Vec4[ 49 ];
		//			//for( int n = 0; n < 49; n++ )
		//			//{
		//			//   Vec2 offset = sampleOffsets[ n ];
		//			//   vec4Offsets[ n ] = new Vec4( offset[ 0 ], offset[ 1 ], 0, 0 );
		//			//}
		//			//parameters.SetNamedConstant( "sampleOffsets", vec4Offsets );
		//		}
		//		break;
		//	}
		//}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ScreenEffect", true );
		}
	}
}
