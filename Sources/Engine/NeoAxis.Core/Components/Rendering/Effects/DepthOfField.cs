// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Depth of field screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 7 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_DepthOfField : RenderingEffect
	{
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_DepthOfField> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The distance at which to focus to achieve the maximum effect.
		/// </summary>
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
		public event Action<RenderingEffect_DepthOfField> FocalDistanceChanged;
		ReferenceField<double> _focalDistance = 50;


		//!!!!
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
		public event Action<RenderingEffect_DepthOfField> FocalSizeChanged;
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
		//public event Action<RenderingEffect_DepthOfField> BlurTextureResolutionChanged;

		/// <summary>
		/// Distance beyound 1.5 focal lengths over which objects will become completely blured.
		/// </summary>
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
		public event Action<RenderingEffect_DepthOfField> BackgroundTransitionLengthChanged;
		ReferenceField<double> _backgroundTransitionLength = 100;

		/// <summary>
		/// Whether the foreground is blured.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> BlurForeground
		{
			get { if( _blurForeground.BeginGet() ) BlurForeground = _blurForeground.Get( this ); return _blurForeground.value; }
			set { if( _blurForeground.BeginSet( ref value ) ) { try { BlurForegroundChanged?.Invoke( this ); } finally { _blurForeground.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurForeground"/> property value changes.</summary>
		public event Action<RenderingEffect_DepthOfField> BlurForegroundChanged;
		ReferenceField<bool> _blurForeground = false;

		/// <summary>
		/// Distance before 0.5 focal lengths over which objects will become completely sharp.
		/// </summary>
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
		public event Action<RenderingEffect_DepthOfField> ForegroundTransitionLengthChanged;
		ReferenceField<double> _foregroundTransitionLength = 40;

		///// <summary>
		///// The amount of blur applied.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		//public Reference<double> BlurFactor
		//{
		//	get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
		//	set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		//public event Action<RenderingEffect_DepthOfField> BlurFactorChanged;
		//ReferenceField<double> _blurFactor = 1;

		///// <summary>
		///// The blur downscaling mode used.
		///// </summary>
		//[DefaultValue( RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		//public Reference<RenderingPipeline_Basic.DownscalingModeEnum> BlurDownscalingMode
		//{
		//	get { if( _blurDownscalingMode.BeginGet() ) BlurDownscalingMode = _blurDownscalingMode.Get( this ); return _blurDownscalingMode.value; }
		//	set { if( _blurDownscalingMode.BeginSet( ref value ) ) { try { BlurDownscalingModeChanged?.Invoke( this ); } finally { _blurDownscalingMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BlurDownscalingMode"/> property value changes.</summary>
		//public event Action<RenderingEffect_DepthOfField> BlurDownscalingModeChanged;
		//ReferenceField<RenderingPipeline_Basic.DownscalingModeEnum> _blurDownscalingMode = RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		///// <summary>
		///// The level blur downscaling.
		///// </summary>
		//[DefaultValue( 0 )]
		//[Range( 0, 6 )]
		//public Reference<int> BlurDownscalingValue
		//{
		//	get { if( _blurDownscalingValue.BeginGet() ) BlurDownscalingValue = _blurDownscalingValue.Get( this ); return _blurDownscalingValue.value; }
		//	set { if( _blurDownscalingValue.BeginSet( ref value ) ) { try { BlurDownscalingValueChanged?.Invoke( this ); } finally { _blurDownscalingValue.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BlurDownscalingValue"/> property value changes.</summary>
		//public event Action<RenderingEffect_DepthOfField> BlurDownscalingValueChanged;
		//ReferenceField<int> _blurDownscalingValue = 0;

		////!!!!в Blur эффект тоже те же параметры

		///// <summary>
		///// The intensity of the gaussian filter. When value is zero is equal to box filter.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//public Reference<double> GaussianIntensity
		//{
		//	get { if( _gaussianIntensity.BeginGet() ) GaussianIntensity = _gaussianIntensity.Get( this ); return _gaussianIntensity.value; }
		//	set { if( _gaussianIntensity.BeginSet( ref value ) ) { try { GaussianIntensityChanged?.Invoke( this ); } finally { _gaussianIntensity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="GaussianIntensity"/> property value changes.</summary>
		//public event Action<RenderingEffect_DepthOfField> GaussianIntensityChanged;
		//ReferenceField<double> _gaussianIntensity = 1.0;

		///// <summary>
		///// The distribution scale of the gaussian blur.
		///// </summary>
		//[DefaultValue( 3.0 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> GaussianScale
		//{
		//	get { if( _gaussianScale.BeginGet() ) GaussianScale = _gaussianScale.Get( this ); return _gaussianScale.value; }
		//	set { if( _gaussianScale.BeginSet( ref value ) ) { try { GaussianScaleChanged?.Invoke( this ); } finally { _gaussianScale.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="GaussianScale"/> property value changes.</summary>
		//public event Action<RenderingEffect_DepthOfField> GaussianScaleChanged;
		//ReferenceField<double> _gaussianScale = 3.0;



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

					//case nameof( BlurDownscalingValue ):
					//	if( BlurDownscalingMode.Value == RenderingPipeline_Basic.DownscalingModeEnum.Auto )
					//		skip = true;
					//	break;
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			var blur = CreateComponent<RenderingEffect_GaussianBlur>();
			blur.Name = "Gaussian Blur";
		}

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var pipeline = context.RenderingPipeline as RenderingPipeline_Basic;
			if( pipeline == null )
				return;

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out ImageComponent depthTexture );
			if( depthTexture == null )
				return;


			var effects = GetComponents<RenderingEffect>( false, false, true );
			if( effects.Length == 0 )
				return;

			//need to make a copy of scene texture because child effects will dispose it
			var currentTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			context.SetViewport( currentTexture.Result.GetRenderTarget().Viewports[ 0 ] );
			pipeline.CopyToCurrentViewport( context, actualTexture );

			//render effects
			foreach( var effect in effects )
				effect.Render( context, frameData, ref currentTexture );


			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, currentTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\DepthOfField\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"blurTexture"*/, currentTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/*"depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				Vector4 properties = new Vector4( FocalDistance, FocalSize, BackgroundTransitionLength, BlurForeground ? ForegroundTransitionLength : -1 );
				shader.Parameters.Set( "depthOfFieldProperties", properties.ToVector4F() );
				shader.Parameters.Set( "intensity", (float)Intensity );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );
			context.DynamicTexture_Free( currentTexture );

			//update actual texture
			actualTexture = finalTexture;
		}

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
