// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Edge detection screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 7.9 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_EdgeDetection : RenderingEffect
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
		public event Action<RenderingEffect_EdgeDetection> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The threshold value when determining edges by distance.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10000.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 10 )]
		public Reference<double> DepthThreshold
		{
			get { if( _depthThreshold.BeginGet() ) DepthThreshold = _depthThreshold.Get( this ); return _depthThreshold.value; }
			set { if( _depthThreshold.BeginSet( ref value ) ) { try { DepthThresholdChanged?.Invoke( this ); } finally { _depthThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthThreshold"/> property value changes.</summary>
		public event Action<RenderingEffect_EdgeDetection> DepthThresholdChanged;
		ReferenceField<double> _depthThreshold = 1.0;

		/// <summary>
		/// The threshold value when determining edges by normals.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.01, 1 )]
		public Reference<double> NormalsThreshold
		{
			get { if( _normalsThreshold.BeginGet() ) NormalsThreshold = _normalsThreshold.Get( this ); return _normalsThreshold.value; }
			set { if( _normalsThreshold.BeginSet( ref value ) ) { try { NormalsThresholdChanged?.Invoke( this ); } finally { _normalsThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NormalsThreshold"/> property value changes.</summary>
		public event Action<RenderingEffect_EdgeDetection> NormalsThresholdChanged;
		ReferenceField<double> _normalsThreshold = 1.0;

		/// <summary>
		/// The color of edges.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<ColorValue> EdgeColor
		{
			get { if( _edgeColor.BeginGet() ) EdgeColor = _edgeColor.Get( this ); return _edgeColor.value; }
			set { if( _edgeColor.BeginSet( ref value ) ) { try { EdgeColorChanged?.Invoke( this ); } finally { _edgeColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EdgeColor"/> property value changes.</summary>
		public event Action<RenderingEffect_EdgeDetection> EdgeColorChanged;
		ReferenceField<ColorValue> _edgeColor = new ColorValue( 0, 0, 0 );

		/// <summary>
		/// The thickness of edges.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Range( 1.0, 8.0 )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<RenderingEffect_EdgeDetection> ThicknessChanged;
		ReferenceField<double> _thickness = 2.0;

		/// <summary>
		/// The maximum visible distance.
		/// </summary>
		[DefaultValue( 30.0 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> MaxDistance
		{
			get { if( _maxDistance.BeginGet() ) MaxDistance = _maxDistance.Get( this ); return _maxDistance.value; }
			set { if( _maxDistance.BeginSet( ref value ) ) { try { MaxDistanceChanged?.Invoke( this ); } finally { _maxDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxDistance"/> property value changes.</summary>
		public event Action<RenderingEffect_EdgeDetection> MaxDistanceChanged;
		ReferenceField<double> _maxDistance = 30.0;

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\EdgeDetection_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"depthTexture"*/, depthTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );
				if( normalTexture == null )
					normalTexture = ResourceUtility.WhiteTexture2D;
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2/*"normalTexture"*/, normalTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "depthThreshold", (float)DepthThreshold );
				shader.Parameters.Set( "normalsThreshold", (float)NormalsThreshold );
				shader.Parameters.Set( "edgeColor", EdgeColor.Value.ToVector4F() );
				shader.Parameters.Set( "thickness", (float)Thickness );
				shader.Parameters.Set( "maxDistance", (float)MaxDistance );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = finalTexture;
		}

		/////////////////////////////////////////

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
