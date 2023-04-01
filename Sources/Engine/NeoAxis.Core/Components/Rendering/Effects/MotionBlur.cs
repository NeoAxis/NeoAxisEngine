// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Per-object motion blur.
	/// </summary>
	[DefaultOrderOfEffect( 6.1 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_MotionBlur : RenderingEffect
	{
		public static double GlobalMultiplier = 1;

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
		public event Action<RenderingEffect_MotionBlur> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// Output velocity multiplier.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_MotionBlur> MultiplierChanged;
		ReferenceField<double> _multiplier = 1.0;

		/// <summary>
		/// Depth boundary value for detecting the edge of an object.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> DepthThreshold
		{
			get { if( _depthThreshold.BeginGet() ) DepthThreshold = _depthThreshold.Get( this ); return _depthThreshold.value; }
			set { if( _depthThreshold.BeginSet( ref value ) ) { try { DepthThresholdChanged?.Invoke( this ); } finally { _depthThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthThreshold"/> property value changes.</summary>
		public event Action<RenderingEffect_MotionBlur> DepthThresholdChanged;
		ReferenceField<double> _depthThreshold = 0.3;

		[DefaultValue( 0.001 )]
		public Reference<double> VelocityThreshold
		{
			get { if( _velocityThreshold.BeginGet() ) VelocityThreshold = _velocityThreshold.Get( this ); return _velocityThreshold.value; }
			set { if( _velocityThreshold.BeginSet( ref value ) ) { try { VelocityThresholdChanged?.Invoke( this ); } finally { _velocityThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VelocityThreshold"/> property value changes.</summary>
		public event Action<RenderingEffect_MotionBlur> VelocityThresholdChanged;
		ReferenceField<double> _velocityThreshold = 0.001;

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var multiplier = 0.0;
			if( context.Owner.LastUpdateTimeStepSmooth != 0 )
				multiplier = ( 1.0 / context.Owner.LastUpdateTimeStepSmooth ) / 70.0;
			multiplier *= Multiplier.Value * GlobalMultiplier;

			if( multiplier <= 0 )
				return;

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "motionAndObjectIdTexture", out var motionAndObjectIdTexture );
			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
			if( motionAndObjectIdTexture == null || depthTexture == null || Intensity <= 0 )
				return;

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\MotionBlur_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"motionTexture"*/, motionAndObjectIdTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/*"depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );

				//fix initial rattling
				if( context.Owner.LastUpdateTimeStepSmooth > 0.1 )
					multiplier = 0;

				var parameters = new Vector4F( (float)multiplier, (float)DepthThreshold, (float)VelocityThreshold, 0 );
				shader.Parameters.Set( "motionBlurParameters", parameters );

				var size = actualTexture.Result.ResultSize;
				shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = finalTexture;
		}
	}
}
