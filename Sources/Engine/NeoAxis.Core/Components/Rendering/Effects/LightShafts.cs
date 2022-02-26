// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect for adding light shafts (god rays).
	/// </summary>
	[DefaultOrderOfEffect( 2.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_LightShafts : RenderingEffect
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
		public event Action<RenderingEffect_LightShafts> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The color of the rays.
		/// </summary>
		[DefaultValue( "1 1 0.6" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 0.6 );

		/// <summary>
		/// The amount of falloff decay applied.
		/// </summary>
		[DefaultValue( 0.9 )]
		[Range( 0, 1 )]
		public Reference<double> Decay
		{
			get { if( _decay.BeginGet() ) Decay = _decay.Get( this ); return _decay.value; }
			set { if( _decay.BeginSet( ref value ) ) { try { DecayChanged?.Invoke( this ); } finally { _decay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Decay"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> DecayChanged;
		ReferenceField<double> _decay = 0.9;

		/// <summary>
		/// The scattering medium density.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> Density
		{
			get { if( _density.BeginGet() ) Density = _density.Get( this ); return _density.value; }
			set { if( _density.BeginSet( ref value ) ) { try { DensityChanged?.Invoke( this ); } finally { _density.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Density"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> DensityChanged;
		ReferenceField<double> _density = 1.0;

		/// <summary>
		/// The amount of the blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.2 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 0.2;

		/// <summary>
		/// Specifies an effect quality.
		/// </summary>
		[DefaultValue( 5 )]
		[Range( 1, 8 )]
		public Reference<int> Resolution
		{
			get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
			set { if( _resolution.BeginSet( ref value ) ) { try { ResolutionChanged?.Invoke( this ); } finally { _resolution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> ResolutionChanged;
		ReferenceField<int> _resolution = 5;

		/// <summary>
		/// Specifies a light source of the light shafts. When is null the first directional light of the scene is used.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Light> Light
		{
			get { if( _light.BeginGet() ) Light = _light.Get( this ); return _light.value; }
			set { if( _light.BeginSet( ref value ) ) { try { LightChanged?.Invoke( this ); } finally { _light.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Light"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> LightChanged;
		ReferenceField<Light> _light = null;

		/// <summary>
		/// Override rays direction.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> OverrideDirection
		{
			get { if( _overrideDirection.BeginGet() ) OverrideDirection = _overrideDirection.Get( this ); return _overrideDirection.value; }
			set { if( _overrideDirection.BeginSet( ref value ) ) { try { OverrideDirectionChanged?.Invoke( this ); } finally { _overrideDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OverrideDirection"/> property value changes.</summary>
		public event Action<RenderingEffect_LightShafts> OverrideDirectionChanged;
		ReferenceField<Vector3> _overrideDirection;

		//!!!!
		//!!!!need per camera context
		//double smoothIntensityFactorLastUpdate;
		//double? smoothIntensityFactor;

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var cameraSettings = context.Owner.CameraSettings;

			bool lightFound = false;
			Vector3 lightPosition = Vector3.Zero;

			if( OverrideDirection.Value != Vector3.Zero )
			{
				lightFound = true;
				lightPosition = cameraSettings.Position - OverrideDirection.Value.GetNormalize() * cameraSettings.FarClipDistance;
			}
			else
			{
				if( Light.ReferenceSpecified )
				{
					var light = Light.Value;
					if( light != null )
					{
						lightFound = true;
						if( light.Type.Value == NeoAxis.Light.TypeEnum.Directional )
							lightPosition = cameraSettings.Position - light.TransformV.Rotation.GetForward() * cameraSettings.FarClipDistance;
						else
							lightPosition = light.TransformV.Position;
					}
				}
				else
				{
					var frameData2 = frameData as RenderingPipeline_Basic.FrameData;
					if( frameData2 != null )
					{
						foreach( var lightIndex in frameData2.LightsInFrustumSorted )
						{
							var item = frameData2.Lights[ lightIndex ];
							if( item.data.Type == NeoAxis.Light.TypeEnum.Directional )
							{
								lightFound = true;
								lightPosition = cameraSettings.Position - item.data.Rotation.GetForward() * (float)cameraSettings.FarClipDistance;
								break;
							}
						}
					}
				}
			}

			if( !lightFound )
				return;
			if( !context.Owner.CameraSettings.ProjectToScreenCoordinates( lightPosition, out var screenLightPosition ) )
				return;

			//calculate intensity factor by the sun position on the screen.
			double demandIntensityFactor = 0;
			{
				Degree angle = 0;
				if( lightPosition != cameraSettings.Position )
				{
					angle = MathAlgorithms.GetVectorsAngle( cameraSettings.Rotation.GetForward(),
						( lightPosition - cameraSettings.Position ).GetNormalize() ).InDegrees();
				}
				else
					angle = 10000;

				var curve = new CurveLine();
				curve.AddPoint( 0, new Vector3( 1, 0, 0 ) );
				curve.AddPoint( 60, new Vector3( 1, 0, 0 ) );
				curve.AddPoint( 75, new Vector3( 0, 0, 0 ) );
				curve.AddPoint( 90, new Vector3( 0, 0, 0 ) );
				var angleFactor = curve.CalculateValueByTime( angle ).X;

				var curve2 = new CurveLine();
				curve2.AddPoint( -0.2, new Vector3( 0, 0, 0 ) );
				curve2.AddPoint( -0.1, new Vector3( 1, 0, 0 ) );
				curve2.AddPoint( 1.1, new Vector3( 1, 0, 0 ) );
				curve2.AddPoint( 1.2, new Vector3( 0, 0, 0 ) );
				var heightFactor = curve2.CalculateValueByTime( screenLightPosition.Y ).X;

				demandIntensityFactor = angleFactor * heightFactor;

				//const float screenFadingBorder = .1f;

				//double minDistance = 1;

				//for( int axis = 0; axis < 2; axis++ )
				//{
				//	if( screenLightPosition[ axis ] < screenFadingBorder )
				//	{
				//		var d = screenLightPosition[ axis ];
				//		if( d < minDistance )
				//			minDistance = d;
				//	}
				//	else if( screenLightPosition[ axis ] > 1.0f - screenFadingBorder )
				//	{
				//		var d = 1.0f - screenLightPosition[ axis ];
				//		if( d < minDistance )
				//			minDistance = d;
				//	}
				//}
				//needIntensityFactor = minDistance / screenFadingBorder;
				//MathEx.Saturate( ref needIntensityFactor );

				////clamp screenLightPosition
				//if( !new Rectangle( 0, 0, 1, 1 ).Contains( screenLightPosition ) )
				//{
				//	if( MathAlgorithms.IntersectRectangleLine( new Rectangle( .0001f, .0001f, .9999f, .9999f ),
				//		new Vector2( .5f, .5f ), screenLightPosition, out var intersectPoint1, out var intersectPoint2 ) != 0 )
				//	{
				//		screenLightPosition = intersectPoint1;
				//	}
				//}
			}

			var smoothIntensityFactor = demandIntensityFactor;
			////update smooth intensity factor
			//if( smoothIntensityFactor == null )
			//	smoothIntensityFactor = needIntensityFactor;
			//if( smoothIntensityFactorLastUpdate != context.Owner.LastUpdateTime )
			//{
			//	smoothIntensityFactorLastUpdate = context.Owner.LastUpdateTime;

			//	const double smoothSpeed = 1;
			//	var step = context.Owner.LastUpdateTimeStep * smoothSpeed;

			//	if( needIntensityFactor > smoothIntensityFactor )
			//	{
			//		smoothIntensityFactor += step;
			//		if( smoothIntensityFactor > needIntensityFactor )
			//			smoothIntensityFactor = needIntensityFactor;
			//	}
			//	else
			//	{
			//		smoothIntensityFactor -= step;
			//		if( smoothIntensityFactor < needIntensityFactor )
			//			smoothIntensityFactor = needIntensityFactor;
			//	}
			//}

			//get result intensity
			var resultIntensity = Intensity.Value * smoothIntensityFactor;
			if( resultIntensity <= 0 )
				return;

			double divisor = 9.0 - Resolution;
			if( divisor <= 1 )
				divisor = 1;
			var sizeFloat = actualTexture.Result.ResultSize.ToVector2() / divisor;
			var size = new Vector2I( (int)sizeFloat.X, (int)sizeFloat.Y );

			//scattering pass
			var scatteringTexture = context.RenderTarget2D_Alloc( size, PixelFormat.A8R8G8B8 );//!!!! PixelFormat.R8G8B8 );
			{
				context.SetViewport( scatteringTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\LightShafts\Scattering_fs.sc";

				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
				if( depthTexture == null )
				{
					//!!!!
				}

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"depthTexture"*/, depthTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( "screenLightPosition", screenLightPosition.ToVector2F() );
				shader.Parameters.Set( "decay", (float)Decay );
				shader.Parameters.Set( "density", (float)Density );

				context.RenderQuadToCurrentViewport( shader );
			}

			//blur pass
			var blurTexture = context.RenderTarget2D_Alloc( size, PixelFormat.A8R8G8B8 );//!!!! PixelFormat.R8G8B8 );
			{
				context.SetViewport( blurTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\LightShafts\Blur_fs.sc";

				//!!!!Linear?
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"scatteringTexture"*/, scatteringTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				shader.Parameters.Set( "color", Color.Value.ToVector3F() );
				shader.Parameters.Set( "screenLightPosition", screenLightPosition.ToVector2F() );
				shader.Parameters.Set( "blurFactor", (float)BlurFactor );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( scatteringTexture );

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\LightShafts\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"blurTexture"*/, blurTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				shader.Parameters.Set( "intensity", (float)resultIntensity );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );
			context.DynamicTexture_Free( blurTexture );

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
