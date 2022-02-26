// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
//!!!!
using Internal.SharpBgfx;


//!!!!скрывать свойства


namespace NeoAxis
{
	/// <summary>
	/// Indirect lighting screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 0.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_IndirectLighting : RenderingEffect
	{
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
		public event Action<RenderingEffect_IndirectLighting> IntensityChanged;
		ReferenceField<double> _intensity = 1;


		public enum TechniqueEnum
		{
			//!!!!name
			Scene,
			ScreenSpace,
			Hybrid,
		}

		//!!!!
		public TechniqueEnum Technique
		{
			get { return TechniqueEnum.ScreenSpace; }
		}


		//!!!!


		//[DefaultValue( false )]
		//public Reference<bool> TestNew
		//{
		//	get { if( _testNew.BeginGet() ) TestNew = _testNew.Get( this ); return _testNew.value; }
		//	set { if( _testNew.BeginSet( ref value ) ) { try { TestNewChanged?.Invoke( this ); } finally { _testNew.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TestNew"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> TestNewChanged;
		//ReferenceField<bool> _testNew = false;


		//[DefaultValue( "0 0 0 0" )]
		//public Reference<Vector4F> TestParameter
		//{
		//	get { if( _testParameter.BeginGet() ) TestParameter = _testParameter.Get( this ); return _testParameter.value; }
		//	set { if( _testParameter.BeginSet( ref value ) ) { try { TestParameterChanged?.Invoke( this ); } finally { _testParameter.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TestParameter"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> TestParameterChanged;
		//ReferenceField<Vector4F> _testParameter;

		//[DefaultValue( 0.0f )]
		//public Reference<float> TestParameter
		//{
		//	get { if( _testParameter.BeginGet() ) TestParameter = _testParameter.Get( this ); return _testParameter.value; }
		//	set { if( _testParameter.BeginSet( ref value ) ) { try { TestParameterChanged?.Invoke( this ); } finally { _testParameter.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TestParameter"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> TestParameterChanged;
		//ReferenceField<float> _testParameter = 0.0f;





		public enum QualityEnum
		{
			Low,
			Medium,
			High,
			//Highest,
		}

		/// <summary>
		/// The quality of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( QualityEnum.Medium )]
		[Category( "Basic" )]
		public Reference<QualityEnum> Quality
		{
			get { if( _quality.BeginGet() ) Quality = _quality.Get( this ); return _quality.value; }
			set { if( _quality.BeginSet( ref value ) ) { try { QualityChanged?.Invoke( this ); } finally { _quality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Quality"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> QualityChanged;
		ReferenceField<QualityEnum> _quality = QualityEnum.Medium;

		public enum ResolutionEnum
		{
			Full = 1,
			Half = 2,
			Quarter = 4,
			//Eight = 8
		}

		/// <summary>
		/// Using lower resolution light buffer can improve performance, but can accentuate aliasing.
		/// </summary>
		[DefaultValue( ResolutionEnum.Half )]
		[Category( "Basic" )]
		public Reference<ResolutionEnum> Resolution
		{
			get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
			set { if( _resolution.BeginSet( ref value ) ) { try { ResolutionChanged?.Invoke( this ); } finally { _resolution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ResolutionChanged;
		ReferenceField<ResolutionEnum> _resolution = ResolutionEnum.Half;

		/// <summary>
		/// Linear multiplier of effect strength.
		/// </summary>
		[Serialize]
		[DefaultValue( 10.0 )]
		[Range( 0, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Basic" )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _multiplier.BeginSet( ref value ) )
				{
					try { MultiplierChanged?.Invoke( this ); }
					finally { _multiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> MultiplierChanged;
		ReferenceField<double> _multiplier = 10.0;

		/// <summary>
		/// The linear reduction of final lighting.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Basic" )]
		public Reference<double> Reduction
		{
			get { if( _reduction.BeginGet() ) Reduction = _reduction.Get( this ); return _reduction.value; }
			set { if( _reduction.BeginSet( ref value ) ) { try { ReductionChanged?.Invoke( this ); } finally { _reduction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Reduction"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ReductionChanged;
		ReferenceField<double> _reduction = 0.05;

		/// <summary>
		/// Effective sampling radius in world space. The effect can only have influence within that radius.
		/// </summary>
		[DefaultValue( 3 )]
		[Range( 0, 30, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Sampling" )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> RadiusChanged;
		ReferenceField<double> _radius = 3;

		/// <summary>
		/// Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Sampling" )]
		public Reference<double> ExpStart
		{
			get { if( _expStart.BeginGet() ) ExpStart = _expStart.Get( this ); return _expStart.value; }
			set { if( _expStart.BeginSet( ref value ) ) { try { ExpStartChanged?.Invoke( this ); } finally { _expStart.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExpStart"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ExpStartChanged;
		ReferenceField<double> _expStart = 1.0;

		/// <summary>
		/// Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 1, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Sampling" )]
		public Reference<double> ExpFactor
		{
			get { if( _expFactor.BeginGet() ) ExpFactor = _expFactor.Get( this ); return _expFactor.value; }
			set { if( _expFactor.BeginSet( ref value ) ) { try { ExpFactorChanged?.Invoke( this ); } finally { _expFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExpFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ExpFactorChanged;
		ReferenceField<double> _expFactor = 1.0;

		/// <summary>
		/// A factor of adding sky lighting.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Sampling" )]
		public Reference<double> SkyLighting
		{
			get { if( _skyLighting.BeginGet() ) SkyLighting = _skyLighting.Get( this ); return _skyLighting.value; }
			set { if( _skyLighting.BeginSet( ref value ) ) { try { SkyLightingChanged?.Invoke( this ); } finally { _skyLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkyLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> SkyLightingChanged;
		ReferenceField<double> _skyLighting = 0.0;

		///// <summary>
		///// Applies some noise on sample positions to hide the banding artifacts that can occur when there is undersampling.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Sampling" )]
		//public Reference<bool> JitterSamples
		//{
		//	get { if( _jitterSamples.BeginGet() ) JitterSamples = _jitterSamples.Get( this ); return _jitterSamples.value; }
		//	set { if( _jitterSamples.BeginSet( ref value ) ) { try { JitterSamplesChanged?.Invoke( this ); } finally { _jitterSamples.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="JitterSamples"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> JitterSamplesChanged;
		//ReferenceField<bool> _jitterSamples = true;

		///// <summary>
		///// Bypass the dot(lightNormal, lightDirection) weighting.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "GI" )]
		//public Reference<double> LnDlOffset
		//{
		//	get { if( _lnDlOffset.BeginGet() ) LnDlOffset = _lnDlOffset.Get( this ); return _lnDlOffset.value; }
		//	set { if( _lnDlOffset.BeginSet( ref value ) ) { try { LnDlOffsetChanged?.Invoke( this ); } finally { _lnDlOffset.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LnDlOffset"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> LnDlOffsetChanged;
		//ReferenceField<double> _lnDlOffset = 0.0;

		///// <summary>
		///// Bypass the dot(normal, lightDirection) weighting.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "GI" )]
		//public Reference<double> NDlOffset
		//{
		//	get { if( _nDlOffset.BeginGet() ) NDlOffset = _nDlOffset.Get( this ); return _nDlOffset.value; }
		//	set { if( _nDlOffset.BeginSet( ref value ) ) { try { NDlOffsetChanged?.Invoke( this ); } finally { _nDlOffset.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="NDlOffset"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> NDlOffsetChanged;
		//ReferenceField<double> _nDlOffset = 0.0;

		/// <summary>
		/// Constant thickness value of objects on the screen in world space. Is used to ignore occlusion past that thickness level, as if light can travel behind the object.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Range( 0.1, 10 )]
		[Category( "Occlusion" )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ThicknessChanged;
		ReferenceField<double> _thickness = 5.0;

		/// <summary>
		/// Occlusion falloff relative to distance.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Occlusion" )]
		public Reference<double> Falloff
		{
			get { if( _falloff.BeginGet() ) Falloff = _falloff.Get( this ); return _falloff.value; }
			set { if( _falloff.BeginSet( ref value ) ) { try { FalloffChanged?.Invoke( this ); } finally { _falloff.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Falloff"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> FalloffChanged;
		ReferenceField<double> _falloff = 1.0;

		///// <summary>
		///// The number of frames to accumulate indirect lighting.
		///// </summary>
		//[DefaultValue( 10 )]
		//[Range( 0, 60, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<int> AccumulateFrames
		//{
		//	get { if( _accumulateFrames.BeginGet() ) AccumulateFrames = _accumulateFrames.Get( this ); return _accumulateFrames.value; }
		//	set { if( _accumulateFrames.BeginSet( ref value ) ) { try { AccumulateFramesChanged?.Invoke( this ); } finally { _accumulateFrames.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AccumulateFrames"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> AccumulateFramesChanged;
		//ReferenceField<int> _accumulateFrames = 10;

		/// <summary>
		/// The amount of the blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 5.0 )]
		[Range( 0, 15, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Blur" )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 5;

		/// <summary>
		/// The blur downscaling mode used.
		/// </summary>
		[DefaultValue( RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		[Serialize]
		[Category( "Blur" )]
		public Reference<RenderingPipeline_Basic.DownscalingModeEnum> BlurDownscalingMode
		{
			get { if( _blurDownscalingMode.BeginGet() ) BlurDownscalingMode = _blurDownscalingMode.Get( this ); return _blurDownscalingMode.value; }
			set { if( _blurDownscalingMode.BeginSet( ref value ) ) { try { BlurDownscalingModeChanged?.Invoke( this ); } finally { _blurDownscalingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingMode"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurDownscalingModeChanged;
		ReferenceField<RenderingPipeline_Basic.DownscalingModeEnum> _blurDownscalingMode = RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level of blur texture downscaling.
		/// </summary>
		[DefaultValue( 0 )]
		[Serialize]
		[Range( 0, 6 )]
		[Category( "Blur" )]
		public Reference<int> BlurDownscalingValue
		{
			get { if( _blurDownscalingValue.BeginGet() ) BlurDownscalingValue = _blurDownscalingValue.Get( this ); return _blurDownscalingValue.value; }
			set { if( _blurDownscalingValue.BeginSet( ref value ) ) { try { BlurDownscalingValueChanged?.Invoke( this ); } finally { _blurDownscalingValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingValue"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurDownscalingValueChanged;
		ReferenceField<int> _blurDownscalingValue = 0;

		/// <summary>
		/// Enables the debug visualization of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		[Category( "Debug" )]
		public Reference<bool> ShowIndirectLighting
		{
			get { if( _showIndirectLighting.BeginGet() ) ShowIndirectLighting = _showIndirectLighting.Get( this ); return _showIndirectLighting.value; }
			set { if( _showIndirectLighting.BeginSet( ref value ) ) { try { ShowIndirectLightingChanged?.Invoke( this ); } finally { _showIndirectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowIndirectLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ShowIndirectLightingChanged;
		ReferenceField<bool> _showIndirectLighting;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( BlurDownscalingValue ):
					if( BlurDownscalingMode.Value == RenderingPipeline_Basic.DownscalingModeEnum.Auto )
						skip = true;
					break;
				}
			}
		}

		//static ImageComponent CreateAccumulationBuffer( Vector2I size, PixelFormat format )
		//{
		//	ImageComponent texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
		//	texture.CreateSize = size;
		//	texture.CreateFormat = format;
		//	texture.CreateUsage = ImageComponent.Usages.RenderTarget;
		//	texture.Enabled = true;

		//	RenderTexture renderTexture = texture.Result.GetRenderTarget();
		//	var viewport = renderTexture.AddViewport( false, false );

		//	viewport.RenderingPipelineCreate();
		//	viewport.RenderingPipelineCreated.UseRenderTargets = false;

		//	return texture;
		//}

		unsafe protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;

#if __TEST
			//!!!!
			if( TestNew.Value )
			{
				Test( context, frameData, ref actualTexture );
				return;
			}
#endif

			//is not supported
			if( !pipeline.GetUseMultiRenderTargets() )
				return;

			////get noise texture
			//const string noiseTextureDefault = @"Base\Images\Noise.png";
			//var noiseTexture = ResourceManager.LoadResource<Image>( noiseTextureDefault );
			//if( noiseTexture == null )
			//	return;

			//downscale
			//var downscaledTexture = actualTexture;
			//if( actualTexture.Result.ResultSize != context.Owner.SizeInPixels )
			//{
			//	actualTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTextureSource.Result.ResultFormat );

			//	//copy to scene texture with downscale
			//	context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

			//	shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / actualTextureSource.Result.ResultSize.ToVector2F() );

			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
			//		actualTextureSource, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			////!!!!сбрасывать при резком перемещении камеры. может счетчик в viewport добавить чтобы не через эвент
			////update accumulation buffer
			//Image accumulationBuffer = null;
			//{
			//	var anyDataKey = "IndirectLighting " + GetPathFromRoot();

			//	//!!!!downscaled?
			//	var demandedSize = actualTexture/*downscaledTexture*/.Result.ResultSize;

			//	//check to destroy
			//	{
			//		context.anyImageAutoDispose.TryGetValue( anyDataKey, out var current );

			//		if( AccumulateFrames.Value == 0 || ( current != null && current.Result.ResultSize != demandedSize ) )
			//		{
			//			//destroy
			//			context.anyImageAutoDispose.Remove( anyDataKey );
			//			current?.Dispose();
			//		}
			//	}

			//	//create and get
			//	if( AccumulateFrames.Value != 0 )
			//	{
			//		context.anyImageAutoDispose.TryGetValue( anyDataKey, out var current );

			//		if( current == null )
			//		{
			//			//create
			//			current = CreateAccumulationBuffer( demandedSize, actualTexture.Result.ResultFormat );
			//			context.anyImageAutoDispose[ anyDataKey ] = current;

			//			//clear
			//			context.SetViewport( current.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
			//		}

			//		accumulationBuffer = current;
			//	}
			//}


			//calculate indirect lighting

			//Image lightingTexture = null;

			//Image lightingNoAccumulationTexture = null;
			//if( accumulationBuffer != null )
			//	lightingTexture = accumulationBuffer;
			//else
			//{
			//	lightingNoAccumulationTexture = context.RenderTarget2D_Alloc( actualTexture/*downscaledTexture*/.Result.ResultSize, actualTexture/*downscaledTexture*/.Result.ResultFormat );
			//	lightingTexture = lightingNoAccumulationTexture;
			//}

			var lightingTextureSize = actualTexture/*downscaledTexture*/.Result.ResultSize / (int)Resolution.Value;

			var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, actualTexture/*downscaledTexture*/.Result.ResultFormat );
			{
				context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Lighting_fs.sc";

				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture/*downscaledTexture*/, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, noiseTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, accumulationBuffer ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );


				//float aspectRatio = (float)context.Owner.CameraSettings.AspectRatio;
				//float fov = (float)context.Owner.CameraSettings.FieldOfView;
				//float zNear = (float)context.Owner.CameraSettings.NearClipDistance;
				//float zFar = (float)context.Owner.CameraSettings.FarClipDistance;

				//shader.Parameters.Set( "colorTextureSize", new Vector4F( (float)lightingTexture.Result.ResultSize.X, (float)lightingTexture.Result.ResultSize.Y, 0.0f, 0.0f ) );
				//shader.Parameters.Set( "zNear", zNear );
				//shader.Parameters.Set( "zFar", zFar );
				//shader.Parameters.Set( "fov", fov );
				//shader.Parameters.Set( "aspectRatio", aspectRatio );

				//{
				//	var size = lightingTexture.Result.ResultSize;
				//	shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
				//}

				//{
				//	var size = noiseTexture.Result.ResultSize;
				//	shader.Parameters.Set( "noiseTextureSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
				//}

				int rotationCount = 3;
				int stepCount = 4;
				switch( Quality.Value )
				{
				case QualityEnum.Low: rotationCount = 2; stepCount = 3; break;
				case QualityEnum.Medium: rotationCount = 3; stepCount = 6; break;
				case QualityEnum.High: rotationCount = 4; stepCount = 9; break;
				}

				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "ROTATION_COUNT", rotationCount.ToString() ) );
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "STEP_COUNT", stepCount.ToString() ) );


				shader.Parameters.Set( "resolution", (float)Resolution.Value );
				shader.Parameters.Set( "skyLighting", (float)SkyLighting.Value );

				shader.Parameters.Set( "radius", (float)Radius.Value );
				shader.Parameters.Set( "expStart", (float)ExpStart.Value );
				shader.Parameters.Set( "expFactor", (float)ExpFactor.Value );
				//shader.Parameters.Set( "jitterSamples", (float)( JitterSamples.Value ? 1.0f : 0.0f ) );

				//shader.Parameters.Set( "lnDlOffset", (float)LnDlOffset.Value );
				//shader.Parameters.Set( "nDlOffset", (float)NDlOffset.Value );

				shader.Parameters.Set( "thickness", (float)Thickness.Value );
				shader.Parameters.Set( "falloff", (float)Falloff.Value );

				shader.Parameters.Set( "reduction", (float)Reduction.Value );

				//!!!!double
				Matrix4F itViewMatrix = ( context.Owner.CameraSettings.ViewMatrix.GetInverse().ToMatrix4F() ).GetTranspose();
				shader.Parameters.Set( "itViewMatrix", itViewMatrix );

				//Vector4F seeds = Vector4F.Zero;
				//if( AccumulateFrames.Value != 0 )
				//{
				//	var random = new Random();
				//	seeds = new Vector4F( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
				//}
				//shader.Parameters.Set( "randomSeeds", seeds );

				//shader.Parameters.Set( "accumulateFrames", (float)AccumulateFrames.Value );

				//!!!!double
				Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
				Matrix4F invProjectionMatrix = projectionMatrix.GetInverse();
				//Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();
				//Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
				//Matrix4F invViewProjMatrix = viewProjMatrix.GetInverse();

				//shader.Parameters.Set( "viewProj", viewProjMatrix );
				//shader.Parameters.Set( "invViewProj", invViewProjMatrix );
				shader.Parameters.Set( "invProj", invProjectionMatrix );

				//!!!!double
				Vector3F cameraPosition = context.Owner.CameraSettings.Position.ToVector3F();
				shader.Parameters.Set( "cameraPosition", cameraPosition );

				var fov = (double)context.Owner.CameraSettings.FieldOfView.InRadians();
				double halfProjScale = (double)context.Owner.SizeInPixels.Y / ( Math.Tan( fov * 0.5f ) * 2 ) * 0.5f;
				shader.Parameters.Set( "halfProjScale", (float)halfProjScale );


				//if( accumulationBuffer != null )
				//	context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
				//else
				context.RenderQuadToCurrentViewport( shader );
			}

			////copy to accumulation buffer
			//if( accumulationBuffer != null )
			//{
			//	context.SetViewport( accumulationBuffer.Result.GetRenderTarget().Viewports[ 0 ] );
			//	( (RenderingPipeline_Basic)context.renderingPipeline ).CopyToCurrentViewport( context, lightingTexture );
			//}

			//if( downscaledTexture != actualTexture )
			//	context.DynamicTexture_Free( downscaledTexture );

			//blur
			var blurTexture = pipeline.GaussianBlur( context, /* accumulationBuffer ?? */lightingTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );

			if( lightingTexture != null )
				context.DynamicTexture_Free( lightingTexture );
			//if( lightingNoAccumulationTexture != null )
			//	context.DynamicTexture_Free( lightingNoAccumulationTexture );

			//final pass
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "showIndirectLighting", ShowIndirectLighting.Value ? 1.0f : 0.0f );
				shader.Parameters.Set( "multiplier", (float)Multiplier * (float)Resolution.Value );


#if __TEST
				//!!!!
				shader.Parameters.Set( "testNew", (float)0.0f );
#endif


				context.RenderQuadToCurrentViewport( shader );
			}

			//free targets
			context.DynamicTexture_Free( blurTexture );
			context.DynamicTexture_Free( actualTexture );
			//!!!!какие еще?

			//update actual texture
			actualTexture = finalTexture;
		}


#if __TEST

		//!!!!


		static Program? compiledProgram;

		static ImageComponent texture;

		//!!!!
		static Uniform? u_testParameter;


		//static Dictionary<int, Program> bgfxComputePrograms = new Dictionary<int, Program>();
		//static Uniform? u_skinningParameters;
		//static Dictionary<int, Uniform> u_skinningBones = new Dictionary<int, Uniform>();


		//!!!!

		Program? GetProgram()
		//GpuProgram GetProgram()
		{
			if( compiledProgram == null )
			{

				var defines = new List<(string, string)>();
				//defines.Add( ("MAX_BONES", maxBones.ToString()) );

				var program = GpuProgramManager.GetProgram( "IndirectLighting", GpuProgramType.Compute, @"Base\Shaders\Effects\IndirectLighting\TestCompute.sc", defines, true, out var error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					//var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
					//!!!!
					//Log.Error( error );
					Log.Warning( error2 );


					//bgfxProgram = Program.Invalid;
					return null;
				}

				compiledProgram = new Program( program.RealObject );
			}

			return compiledProgram.Value;

			//return program;
		}

		//bool GetBgfxComputeProgram( int maxBones, out Program bgfxProgram )
		//{
		//	// Try to get program from the cache.
		//	if( bgfxComputePrograms.TryGetValue( maxBones, out bgfxProgram ) )
		//		return true;

		//	var defines = new List<(string, string)>();
		//	defines.Add( ("MAX_BONES", maxBones.ToString()) );

		//	var program = GpuProgramManager.GetProgram( "MeshSkeletonAnimation",
		//		GpuProgramType.Compute, @"Base\Shaders\MeshSkeletonAnimation.sc", defines, out var error2 );
		//	if( !string.IsNullOrEmpty( error2 ) )
		//	{
		//		var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
		//		Log.Error( error );

		//		bgfxProgram = Program.Invalid;
		//		return false;
		//	}

		//	bgfxProgram = new Program( program.RealObject );
		//	bgfxComputePrograms[ maxBones ] = bgfxProgram;

		//	return true;
		//}



		//!!!!
		void Test( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;


			var computeProgram = GetProgram();
			if( computeProgram == null )
				return;


			//!!!!

			//Log.Info( computeProgram.ToString() );


			//!!!!удалять

			if( texture == null )
			{
				texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

				bool mipmaps = false;

				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = new Vector2I( 16, 1 );
				texture.CreateMipmaps = mipmaps;
				texture.CreateFormat = PixelFormat.Float32RGBA;
				//texture.CreateFormat = PixelFormat.Float32R;

				//!!!!write only?
				var usage = ImageComponent.Usages.WriteOnly | ImageComponent.Usages.ComputeWrite;
				if( mipmaps )
					usage |= ImageComponent.Usages.AutoMipmaps;
				texture.CreateUsage = usage;

				texture.Enabled = true;

				//!!!!
				texture.Result.PrepareNativeObject();

			}
			//m_aoMap = bgfx::createTexture2D( uint16_t( m_size[ 0 ] ), uint16_t( m_size[ 1 ] ), false, 1, bgfx::TextureFormat::R8, BGFX_TEXTURE_COMPUTE_WRITE | SAMPLER_POINT_CLAMP );

			//var texture = context.RenderTarget2D_Alloc( new Vector2I( 16, 1 ), PixelFormat.Float32R );
			//var texture = context.RenderTarget2D_Alloc( new Vector2I( 16, 1 ), PixelFormat.Float16R );


			unsafe
			{

				//!!!!TEMP чтобы обновить view number counter
				context.SetComputeView();
				//context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );



				//var maxBones = Math.Max( MathEx.NextPowerOfTwo( bones.Length ), 64 );

				////set bone data
				//var boneData = new Matrix4F[ maxBones ];
				//{
				//	//!!!!
				//}

				////enumerate render operations of the mesh
				//for( int nOper = 0; nOper < modifiableMesh.Result.MeshData.RenderOperations.Count; nOper++ )
				//{

				//var sourceOper = originalMesh.Result.MeshData.RenderOperations[ nOper ];
				//var destOper = modifiableMesh.Result.MeshData.RenderOperations[ nOper ];

				//var sourceVertexBuffer = sourceOper.VertexBuffers[ 0 ];
				//var destVertexBuffer = destOper.VertexBuffers[ 0 ];

				//bind buffers
				//if( sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				//	Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );
				//else
				//	Bgfx.SetComputeBuffer( 0, (VertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );



				//!!!!
				//Bgfx.SetComputeBuffer(

				//!!!!
				Bgfx.SetComputeImage( 0, texture.Result.GetNativeObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.RGBA32F );
				//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );
				// R16F );
				//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( false ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );// R16F );

				//Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)destVertexBuffer.GetNativeObject(), ComputeBufferAccess.Write );


				//bind parameters
				{
					Vector4F testParameter = TestParameter.Value;
					//Vector4F testParameter = new Vector4F( TestParameter.Value, 0, 0, 0 );

					var arraySize = 1;// sizeof( SkinningParameters ) / sizeof( Vector4F );
					if( u_testParameter == null )
						u_testParameter = GpuProgramManager.RegisterUniform( "u_testParameter", UniformType.Vector4, arraySize );
					Bgfx.SetUniform( u_testParameter.Value, &testParameter, arraySize );

					//var arraySize = sizeof( SkinningParameters ) / sizeof( Vector4F );
					//if( u_skinningParameters == null )
					//	u_skinningParameters = GpuProgramManager.RegisterUniform( "u_skinningParameters", UniformType.Vector4, arraySize );
					//Bgfx.SetUniform( u_skinningParameters.Value, &parameters, arraySize );
				}

				//!!!!
				Bgfx.Dispatch( context.CurrentViewNumber, computeProgram.Value, 1, 1, 1, DiscardFlags.All );

				//!!!! ( sourceVertexBuffer.VertexCount + 1023 ) / 1024
				//Bgfx.Dispatch( context.CurrentViewNumber, bgfxProgram, 1 );
				//Bgfx.Dispatch( context.CurrentViewNumber, bgfxProgram, ( sourceVertexBuffer.VertexCount + 1023 ) / 1024 );
				//}

			}


			//final pass
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\FinalTest_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//!!!!
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//!!!!
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "showIndirectLighting", ShowIndirectLighting.Value ? 1.0f : 0.0f );
				shader.Parameters.Set( "multiplier", (float)Multiplier * (float)Resolution.Value );


				//!!!!
				//shader.Parameters.Set( "testNew", (float)1.0f );


				context.RenderQuadToCurrentViewport( shader );
			}

			//free targets
			//!!!!
			//context.DynamicTexture_Free( blurTexture );
			context.DynamicTexture_Free( actualTexture );
			//!!!!какие еще?

			//update actual texture
			actualTexture = finalTexture;

		}
#endif

	}
}
