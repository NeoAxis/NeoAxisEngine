// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Gaussian blur screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 11 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_GaussianBlur : RenderingEffect
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
		public event Action<RenderingEffect_GaussianBlur> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The amount of blur applied.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 15, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_GaussianBlur> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 1.0;

		/// <summary>
		/// The image downscaling mode used.
		/// </summary>
		[DefaultValue( RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		public Reference<RenderingPipeline_Basic.DownscalingModeEnum> DownscalingMode
		{
			get { if( _downscalingMode.BeginGet() ) DownscalingMode = _downscalingMode.Get( this ); return _downscalingMode.value; }
			set { if( _downscalingMode.BeginSet( ref value ) ) { try { DownscalingModeChanged?.Invoke( this ); } finally { _downscalingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscalingMode"/> property value changes.</summary>
		public event Action<RenderingEffect_GaussianBlur> DownscalingModeChanged;
		ReferenceField<RenderingPipeline_Basic.DownscalingModeEnum> _downscalingMode = RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level of downscaling applied.
		/// </summary>
		[DefaultValue( 0 )]
		[Range( 0, 6 )]
		public Reference<int> DownscalingValue
		{
			get { if( _downscalingValue.BeginGet() ) DownscalingValue = _downscalingValue.Get( this ); return _downscalingValue.value; }
			set { if( _downscalingValue.BeginSet( ref value ) ) { try { DownscalingValueChanged?.Invoke( this ); } finally { _downscalingValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscalingValue"/> property value changes.</summary>
		public event Action<RenderingEffect_GaussianBlur> DownscalingValueChanged;
		ReferenceField<int> _downscalingValue = 0;

		/// <summary>
		/// The standard deviation parameter of the gaussian equation.
		/// </summary>
		[DefaultValue( 3.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> StandardDeviation
		{
			get { if( _standardDeviation.BeginGet() ) StandardDeviation = _standardDeviation.Get( this ); return _standardDeviation.value; }
			set { if( _standardDeviation.BeginSet( ref value ) ) { try { StandardDeviationChanged?.Invoke( this ); } finally { _standardDeviation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StandardDeviation"/> property value changes.</summary>
		public event Action<RenderingEffect_GaussianBlur> StandardDeviationChanged;
		ReferenceField<double> _standardDeviation = 3.0;

		//public enum BlurDimensionsEnum
		//{
		//	HorizontalAndVertical,
		//	Horizontal,
		//	Vertical,
		//}

		////BlurDimensions
		//ReferenceField<BlurDimensionsEnum> _blurDimensions = BlurDimensionsEnum.HorizontalAndVertical;
		//[DefaultValue( BlurDimensionsEnum.HorizontalAndVertical )]
		//[Serialize]
		//public Reference<BlurDimensionsEnum> BlurDimensions
		//{
		//	get { if( _blurDimensions.BeginGet() ) BlurDimensions = _blurDimensions.Get( this ); return _blurDimensions.value; }
		//	set { if( _blurDimensions.BeginSet( ref value ) ) { try { BlurDimensionsChanged?.Invoke( this ); } finally { _blurDimensions.EndSet(); } } }
		//}
		//public event Action<RenderingEffect_GaussianBlur> BlurDimensionsChanged;

		//!!!!Quality

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( DownscalingValue ):
					if( DownscalingMode.Value == RenderingPipeline_Basic.DownscalingModeEnum.Auto )
					{
						skip = true;
						return;
					}
					break;
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var blurFactor = BlurFactor.Value;
			var dof = Parent as RenderingEffect_DepthOfField;
			if( dof != null )
				blurFactor *= RenderingEffect_DepthOfField.GlobalBlurFactor;

			if( blurFactor <= 0 )
				return;


			//blur
			ImageComponent blurTexture;
			{
				var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;

				var blurSettings = new RenderingPipeline_Basic.GaussianBlurSettings();
				blurSettings.SourceTexture = actualTexture;
				blurSettings.BlurFactor = blurFactor;
				blurSettings.DownscalingMode = DownscalingMode;
				blurSettings.DownscalingValue = DownscalingValue;
				blurSettings.StandardDeviation = StandardDeviation;

				blurSettings.BlendResultWithTexture = actualTexture;
				blurSettings.BlendResultWithTextureIntensity = Intensity;

				blurTexture = pipeline.GaussianBlur( context, blurSettings );
			}
			//var blurTexture = pipeline.GaussianBlur( context, actualTexture, BlurFactor, DownscalingMode, DownscalingValue, GaussianScale );


			//blend in the GaussianBlur

			////create final
			//var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			//{
			//	context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	var shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Lerp_fs.sc";

			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"source1Texture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"source2Texture"*/, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
			//	shader.Parameters.Set( "intensity", (float)Intensity );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			//free old textures
			context.DynamicTexture_Free( actualTexture );
			//context.DynamicTexture_Free( blurTexture );

			//update actual texture
			actualTexture = blurTexture;
			//actualTexture = finalTexture;
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
