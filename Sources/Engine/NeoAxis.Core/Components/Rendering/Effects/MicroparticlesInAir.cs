// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.SharpBgfx;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// An screen effect for visualization microparticles in air.
	/// </summary>
	[DefaultOrderOfEffect( 0.25 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_MicroparticlesInAir : RenderingEffect
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
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		//!!!!use ColorValuePowered?

		[DefaultValue( "1 0.8 0.5" )]
		[Category( "Effect" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 0.8, 0.5 );

		[DefaultValue( 0.0007 )]
		[Range( 0, 0.01, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Effect" )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( this, ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> MultiplierChanged;
		ReferenceField<double> _multiplier = 0.0007;

		/// <summary>
		/// Whether to apply shadowing to the calculation of the effect.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Effect" )]
		public Reference<bool> Shadows
		{
			get { if( _shadows.BeginGet() ) Shadows = _shadows.Get( this ); return _shadows.value; }
			set { if( _shadows.BeginSet( this, ref value ) ) { try { ShadowsChanged?.Invoke( this ); } finally { _shadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shadows"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> ShadowsChanged;
		ReferenceField<bool> _shadows = true;

		public enum QualityEnum
		{
			Fastest, // 1/8 size
			Fast, // 1/4 size
			Optimized, // 1/4 or 1/1 size
			Full // 1/1 size
		}

		/// <summary>
		/// The quality of the effect is used to calibrate rendering performance.
		/// </summary>
		[DefaultValue( QualityEnum.Optimized )]
		[Category( "Effect" )]
		public Reference<QualityEnum> Quality
		{
			get { if( _quality.BeginGet() ) Quality = _quality.Get( this ); return _quality.value; }
			set { if( _quality.BeginSet( this, ref value ) ) { try { QualityChanged?.Invoke( this ); } finally { _quality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Quality"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> QualityChanged;
		ReferenceField<QualityEnum> _quality = QualityEnum.Optimized;

		////default

		//public enum ResolutionEnum
		//{
		//	Full = 1,
		//	Half = 2,
		//	Quarter = 4,
		//	Eighth = 8,
		//}

		///// <summary>
		///// Using lower resolution light buffer can improve performance, but can accentuate aliasing.
		///// </summary>
		//[DefaultValue( ResolutionEnum.Full )]//Quarter )]
		//[Category( "Effect" )]
		//public Reference<ResolutionEnum> Resolution
		//{
		//	get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
		//	set { if( _resolution.BeginSet( this, ref value ) ) { try { ResolutionChanged?.Invoke( this ); } finally { _resolution.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		//public event Action<RenderingEffect_MicroparticlesInAir> ResolutionChanged;
		//ReferenceField<ResolutionEnum> _resolution = ResolutionEnum.Full;// Quarter;

		public enum RayStepsEnum
		{
			_16,
			_32,
			_64,
			_128,
			_256,
		}

		[DefaultValue( RayStepsEnum._64 )]
		[Category( "Effect" )]
		public Reference<RayStepsEnum> RaySteps
		{
			get { if( _raySteps.BeginGet() ) RaySteps = _raySteps.Get( this ); return _raySteps.value; }
			set { if( _raySteps.BeginSet( this, ref value ) ) { try { RayStepsChanged?.Invoke( this ); } finally { _raySteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RaySteps"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> RayStepsChanged;
		ReferenceField<RayStepsEnum> _raySteps = RayStepsEnum._64;

		[DefaultValue( 0.2 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Effect" )]
		public Reference<double> RayStepAdd
		{
			get { if( _rayStepAdd.BeginGet() ) RayStepAdd = _rayStepAdd.Get( this ); return _rayStepAdd.value; }
			set { if( _rayStepAdd.BeginSet( this, ref value ) ) { try { RayStepAddChanged?.Invoke( this ); } finally { _rayStepAdd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RayStepAdd"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> RayStepAddChanged;
		ReferenceField<double> _rayStepAdd = 0.2;

		[DefaultValue( 1.05 )]
		[Range( 1, 2, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Effect" )]
		public Reference<double> RayStepMultiply
		{
			get { if( _rayStepMultiply.BeginGet() ) RayStepMultiply = _rayStepMultiply.Get( this ); return _rayStepMultiply.value; }
			set { if( _rayStepMultiply.BeginSet( this, ref value ) ) { try { RayStepMultiplyChanged?.Invoke( this ); } finally { _rayStepMultiply.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RayStepMultiply"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> RayStepMultiplyChanged;
		ReferenceField<double> _rayStepMultiply = 1.05;

		[DefaultValue( 1.1 )]
		[Range( 1.0, 1.2 )]
		[Category( "Effect" )]
		public Reference<double> DepthThreshold
		{
			get { if( _depthThreshold.BeginGet() ) DepthThreshold = _depthThreshold.Get( this ); return _depthThreshold.value; }
			set { if( _depthThreshold.BeginSet( this, ref value ) ) { try { DepthThresholdChanged?.Invoke( this ); } finally { _depthThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthThreshold"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> DepthThresholdChanged;
		ReferenceField<double> _depthThreshold = 1.1;

		/// <summary>
		/// The strength of blur post effect.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Range( 0, 8 )]
		[Category( "Effect" )]
		public Reference<double> Blur
		{
			get { if( _blur.BeginGet() ) Blur = _blur.Get( this ); return _blur.value; }
			set { if( _blur.BeginSet( this, ref value ) ) { try { BlurChanged?.Invoke( this ); } finally { _blur.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Blur"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> BlurChanged;
		ReferenceField<double> _blur = 2.0;

		/// <summary>
		/// Whether to visualize edge detection.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Debug" )]
		public Reference<bool> DebugEdges
		{
			get { if( _debugEdges.BeginGet() ) DebugEdges = _debugEdges.Get( this ); return _debugEdges.value; }
			set { if( _debugEdges.BeginSet( this, ref value ) ) { try { DebugEdgesChanged?.Invoke( this ); } finally { _debugEdges.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugEdges"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> DebugEdgesChanged;
		ReferenceField<bool> _debugEdges = false;

		[DefaultValue( false )]
		[Category( "Debug" )]
		public Reference<bool> DebugEffect
		{
			get { if( _debugEffect.BeginGet() ) DebugEffect = _debugEffect.Get( this ); return _debugEffect.value; }
			set { if( _debugEffect.BeginSet( this, ref value ) ) { try { DebugEffectChanged?.Invoke( this ); } finally { _debugEffect.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugEffect"/> property value changes.</summary>
		public event Action<RenderingEffect_MicroparticlesInAir> DebugEffectChanged;
		ReferenceField<bool> _debugEffect = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//case nameof( DepthThreshold ):
				case nameof( DebugEdges ):
					if( Quality.Value != QualityEnum.Optimized )
						skip = true;
					break;

					//case nameof( Blur ):
					//	if( Quality.Value == QualityEnum.Full )
					//		skip = true;
					//	break;
				}
			}
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			var intensity = Intensity.Value * GlobalMultiplier;
			if( intensity <= 0 )
				return;

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
			if( depthTexture == null )
				return;

			var pipeline = context.RenderingPipeline;

			//check exists lights with effect
			//{
			//return;
			//}

			var microparticlesInAirColor = Color.Value.ToVector4() * Multiplier.Value * intensity;
			if( microparticlesInAirColor.MaxComponent() <= 0.00001 )
				return;

			{
				//!!!!без копирования сложнее потому что RenderTarget не создается с флагом ComputeWrite

				var downscaleFactor = 1.0;
				switch( Quality.Value )
				{
				case QualityEnum.Fastest: downscaleFactor = 1.0 / 8.0; break;
				case QualityEnum.Fast: downscaleFactor = 1.0 / 4.0; break;
				}
				var addTextureSize = ( actualTexture.Result.ResultSize.ToVector2() * downscaleFactor ).ToVector2I();

				var debugEdges = DebugEdges && Quality.Value == QualityEnum.Optimized;

				var addTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, addTextureSize, actualTexture.Result.ResultFormat, 0, false );


				{
					context.SetComputePass();

					var defines = new List<(string, string)>();
					defines.Add( ("STEPS", RaySteps.Value.ToString().Replace( "_", "" )) );

					var shadows = false;
					if( Shadows )
					{
						foreach( var lightIndex in frameData.LightsInFrustumSorted )
						{
							var lightItem = frameData.Lights[ lightIndex ];
							if( lightItem.prepareShadows )
							{
								shadows = true;
								break;
							}
						}
					}
					if( shadows )
						defines.Add( ("SHADOW_MAP", "1") );

					defines.Add( ("QUALITY_" + Quality.Value.ToString().ToUpper(), "1") );


					var program = ComputeProgramManager.GetProgram( "MicroparticlesInAir", @"Base\Shaders\Effects\MicroparticlesInAir.sc", defines, true );

					context.BindComputeImage( 0, addTexture, 0, ComputeBufferAccessEnum.ReadWrite );


					//!!!!can be removed
					var generalContainer = new ParameterContainer();


					//bind textures

					//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					//generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 15/* "s_lightGrid"*/, frameData.LightGrid ?? ResourceUtility.DummyTexture3DFloat32RGBA, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					//shadows
					{
						var isByte4Format = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4;
						var filtering = isByte4Format ? FilterOption.Point : FilterOption.Linear;
						var textureFlags = isByte4Format ? 0 : TextureFlags.CompareLessEqual;

						//shadow map directional
						{
							var shadowMap = frameData.ShadowTextureArrayDirectional;
							if( shadowMap == null )
								shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

							var wrap = isByte4Format;

							var textureValue = new ViewportRenderingContext.BindTextureData( 5/*s_shadowMapShadowDirectional*/, shadowMap, wrap ? TextureAddressingMode.Wrap : TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
							generalContainer.Set( ref textureValue );
						}

						//shadow map array spot
						{
							var shadowMap = frameData.ShadowTextureArraySpot;
							if( shadowMap == null )
								shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

							var textureValue = new ViewportRenderingContext.BindTextureData( 6/*s_shadowMapShadowSpot*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
							generalContainer.Set( ref textureValue );
						}

						//shadow map array point
						{
							var shadowMap = frameData.ShadowTextureArrayPoint;
							if( shadowMap == null )
								shadowMap = isByte4Format ? ResourceUtility.DummyTextureCubeArrayARGB8/*WhiteTextureCube*/ : ResourceUtility.DummyShadowMapCubeArrayFloat32R;

							var textureValue = new ViewportRenderingContext.BindTextureData( 7/*s_shadowMapShadowPoint*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
							generalContainer.Set( ref textureValue );
						}
					}

					//light masks
					if( RenderingSystem.LightMask )
					{
						//mask array directional
						{
							var texture = frameData.MaskTextureArrayDirectional ?? ResourceUtility.WhiteTexture2D;
							generalContainer.Set( new ViewportRenderingContext.BindTextureData( 12/*s_lightMaskDirectional*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
						}

						//mask array spot
						{
							var texture = frameData.MaskTextureArraySpot ?? ResourceUtility.WhiteTexture2D;
							generalContainer.Set( new ViewportRenderingContext.BindTextureData( 13/*s_lightMaskSpot*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
						}

						//mask array point
						{
							var texture = frameData.MaskTextureArrayPoint ?? ResourceUtility.WhiteTextureCube;
							generalContainer.Set( new ViewportRenderingContext.BindTextureData( 14/*s_lightMaskPoint*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
						}
					}

					unsafe
					{
						var v = microparticlesInAirColor.ToVector4F();
						context.SetUniform( "u_microparticlesInAirColor", ParameterType.Vector4, 1, &v );
					}
					unsafe
					{
						var v = new Vector4F( (float)RayStepAdd.Value, (float)RayStepMultiply.Value, debugEdges ? 1 : 0, (float)DepthThreshold.Value );
						context.SetUniform( "u_microparticlesInAirParams", ParameterType.Vector4, 1, &v );
					}

					context.BindParameterContainer( generalContainer, false );

					Vector3I jobSize;
					if( Quality.Value == QualityEnum.Optimized )
						jobSize = new Vector3I( (int)Math.Ceiling( addTextureSize.X / 16.0 / 4.0 ), (int)Math.Ceiling( addTextureSize.Y / 16.0 / 4.0 ), 1 );
					else
						jobSize = new Vector3I( (int)Math.Ceiling( addTextureSize.X / 16.0 ), (int)Math.Ceiling( addTextureSize.Y / 16.0 ), 1 );

					context.Dispatch( program, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
					//Bgfx.Dispatch( (ushort)context.CurrentViewNumber, program, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
					//context.UpdateStatisticsCurrent.ComputeDispatches++;
				}

				ImageComponent blurred;

				//blur
				if( Blur > 0 && !debugEdges )
				{

					//!!!!блюр можно сразу врендерить в actualTexture

					var blur = Blur.Value * downscaleFactor;

					var settings = new RenderingPipeline_Basic.GaussianBlurSettings();
					settings.SourceTexture = addTexture;
					settings.BlurFactor = blur;
					settings.DownscalingMode = RenderingPipeline_Basic.DownscalingModeEnum.Manual;
					settings.DownscalingValue = 0;

					settings.DepthCheck = true;
					settings.DepthCheckThreshold = DepthThreshold;

					blurred = pipeline.GaussianBlur( context, settings );

					context.DynamicTexture_Free( addTexture );
				}
				else
					blurred = addTexture;

				//add to scene texture
				{
					context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					var filtering = actualTexture.Result.ResultSize != blurred.Result.ResultSize ? FilterOption.Linear : FilterOption.Point;
					var blending = DebugEffect || debugEdges ? CanvasRenderer.BlendingType.Opaque : CanvasRenderer.BlendingType.Add;
					pipeline.CopyToCurrentViewport( context, blurred, blending, filtering );
				}

				context.DynamicTexture_Free( blurred );
			}

		}

		//!!!!
		public override bool LimitedDevicesSupport
		{
			get { return false; }
		}

		//public override ScreenLabelInfo GetScreenLabelInfo()
		//{
		//	return new ScreenLabelInfo( "ScreenEffect", true );
		//}
	}
}
