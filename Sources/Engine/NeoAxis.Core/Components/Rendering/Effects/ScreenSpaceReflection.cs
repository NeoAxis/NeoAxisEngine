// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Screen space reflection screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 0 )]
	public class Component_RenderingEffect_ScreenSpaceReflection : Component_RenderingEffect
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
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> IntensityChanged;
		ReferenceField<double> _intensity = 1.0;

		public enum QualityEnum
		{
			Lowest,
			Low,
			Medium,
			High,
			Highest,
		}

		/// <summary>
		/// The quality of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( QualityEnum.Medium )]
		public Reference<QualityEnum> Quality
		{
			get { if( _quality.BeginGet() ) Quality = _quality.Get( this ); return _quality.value; }
			set { if( _quality.BeginSet( ref value ) ) { try { QualityChanged?.Invoke( this ); } finally { _quality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Quality"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> QualityChanged;
		ReferenceField<QualityEnum> _quality = QualityEnum.Medium;

		/// <summary>
		/// Minimal power of blur.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0, 10 )]
		public Reference<double> BlurRoughnessMin
		{
			get { if( _blurRoughnessMin.BeginGet() ) BlurRoughnessMin = _blurRoughnessMin.Get( this ); return _blurRoughnessMin.value; }
			set { if( _blurRoughnessMin.BeginSet( ref value ) ) { try { BlurRoughnessMinChanged?.Invoke( this ); } finally { _blurRoughnessMin.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurRoughnessMin"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> BlurRoughnessMinChanged;
		ReferenceField<double> _blurRoughnessMin = 1.0;

		/// <summary>
		/// Maximal power of blur.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Serialize]
		[Range( 0, 10 )]
		public Reference<double> BlurRoughnessMax
		{
			get { if( _blurRoughnessMax.BeginGet() ) BlurRoughnessMax = _blurRoughnessMax.Get( this ); return _blurRoughnessMax.value; }
			set { if( _blurRoughnessMax.BeginSet( ref value ) ) { try { BlurRoughnessMaxChanged?.Invoke( this ); } finally { _blurRoughnessMax.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurRoughnessMax"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> BlurRoughnessMaxChanged;
		ReferenceField<double> _blurRoughnessMax = 5.0;

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
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> BlurDownscalingModeChanged;
		ReferenceField<Component_RenderingPipeline_Basic.DownscalingModeEnum> _blurDownscalingMode = Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level of blur texture downscaling.
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
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> BlurDownscalingValueChanged;
		ReferenceField<int> _blurDownscalingValue = 0;

		//!!!!name
		[Serialize]
		[DefaultValue( 8.0 )]
		[Range( 0, 40 )]
		public Reference<double> EdgeFactorPower
		{
			get { if( _edgeFactorPower.BeginGet() ) EdgeFactorPower = _edgeFactorPower.Get( this ); return _edgeFactorPower.value; }
			set { if( _edgeFactorPower.BeginSet( ref value ) ) { try { EdgeFactorPowerChanged?.Invoke( this ); } finally { _edgeFactorPower.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EdgeFactorPower"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ScreenSpaceReflection> EdgeFactorPowerChanged;
		ReferenceField<double> _edgeFactorPower = 8.0;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( BlurDownscalingValue ):
					if( BlurDownscalingMode.Value == Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			var pipeline = FindParent<Component_RenderingPipeline_Basic>();
			if( pipeline != null )
			{
				//pipeline.RenderBegin += Pipeline_RenderBegin;
				pipeline.RenderDeferredShadingEnd += Pipeline_RenderDeferredShadingEnd;
			}
		}

		protected override void OnDisabled()
		{
			var pipeline = FindParent<Component_RenderingPipeline_Basic>();
			if( pipeline != null )
			{
				//pipeline.RenderBegin -= Pipeline_RenderBegin;
				pipeline.RenderDeferredShadingEnd -= Pipeline_RenderDeferredShadingEnd;
			}

			base.OnDisabled();
		}

		//private void Pipeline_RenderBegin( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, Component_RenderingPipeline_Basic.FrameData frameData )
		//{
		//	if( EnabledInHierarchy && sender.UseRenderTargets )
		//	{
		//		//frameData.GenerateIBLSpecularTexture = true;
		//		//frameData.DeferredSpecularIBLItensity = 1.0 - Intensity;
		//	}
		//}

		private void Pipeline_RenderDeferredShadingEnd( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, Component_RenderingPipeline_Basic.FrameData frameData, ref Component_Image sceneTexture )
		{
			if( EnabledInHierarchy && sender.GetUseMultiRenderTargets() )
			{
				var actualTexture = sceneTexture;

				var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;

				Vector3 cameraPos = context.Owner.CameraSettings.Position;
				//!!!!double
				Vector3F cameraPosition = cameraPos.ToVector3F();

				//!!!!double
				Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
				Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

				Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
				Matrix4F invViewProjMatrix = viewProjMatrix.GetInverse();

				float aspectRatio = (float)context.Owner.CameraSettings.AspectRatio;
				float fov = (float)context.Owner.CameraSettings.FieldOfView;
				float zNear = (float)context.Owner.CameraSettings.NearClipDistance;
				float zFar = (float)context.Owner.CameraSettings.FarClipDistance;

				var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];
				var ambientLightPower = ambientLight.data.Power;

				var reflectionTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
				{
					context.SetViewport( reflectionTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\ScreenSpaceReflection\SSR_fs.sc";

					int maxSteps = 50;
					switch( Quality.Value )
					{
					case QualityEnum.Lowest: maxSteps = 20; break;
					case QualityEnum.Low: maxSteps = 50; break;
					case QualityEnum.Medium: maxSteps = 80; break;
					case QualityEnum.High: maxSteps = 120; break;
					case QualityEnum.Highest: maxSteps = 160; break;
					}

					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "MAX_STEPS", maxSteps.ToString() ) );

					context.objectsDuringUpdate.namedTextures.TryGetValue( "gBuffer0Texture", out var gBuffer0Texture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "gBuffer2Texture", out var gBuffer2Texture );

					//!!!!reflection probes?
					pipeline.GetBackgroundEnvironmentTextures( context, frameData, /*true, */out var environmentTexture, out var environmentTextureIBL );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, depthTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, actualTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture,
					   TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					//!!!!rotation, multiplier

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, environmentTexture.Value.texture,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, environmentTextureIBL.Value.texture,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, Component_RenderingPipeline_Basic.BrdfLUT,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 6, gBuffer2Texture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 7, gBuffer0Texture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					//!!!часть параметров есть UniformsGeneral.sh

					shader.Parameters.Set( "viewProj", viewProjMatrix );
					shader.Parameters.Set( "invViewProj", invViewProjMatrix );
					shader.Parameters.Set( "cameraPosition", cameraPosition );
					shader.Parameters.Set( "edgeFactorPower", (float)EdgeFactorPower );

					shader.Parameters.Set( "colorTextureSize", new Vector4F( (float)actualTexture.Result.ResultSize.X, (float)actualTexture.Result.ResultSize.Y, 0.0f, 0.0f ) );
					shader.Parameters.Set( "zNear", zNear );
					shader.Parameters.Set( "zFar", zFar );
					shader.Parameters.Set( "fov", fov );
					shader.Parameters.Set( "aspectRatio", aspectRatio );

					context.RenderQuadToCurrentViewport( shader );
				}

				var min = BlurRoughnessMin.Value;
				var max = BlurRoughnessMax.Value;
				if( min > max )
					min = max;
				var blurRoughnessMin = pipeline.GaussianBlur( context, this, reflectionTexture, min, BlurDownscalingMode, BlurDownscalingValue );
				var blurRoughnessMax = pipeline.GaussianBlur( context, this, reflectionTexture, max, BlurDownscalingMode, BlurDownscalingValue );
				//// Blur Pass:
				//var bluredReflection = pipeline.GaussianBlur( context, this, reflectionTexture, BlurFactorOnMaxRoughness, BlurDownscalingMode, BlurDownscalingValue );

				// Final Pass:
				var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
				{
					context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\ScreenSpaceReflection\SSR_Apply_fs.sc";

					context.objectsDuringUpdate.namedTextures.TryGetValue( "gBuffer0Texture", out var gBuffer0Texture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );
					context.objectsDuringUpdate.namedTextures.TryGetValue( "gBuffer2Texture", out var gBuffer2Texture );

					//!!!!reflection probes?
					pipeline.GetBackgroundEnvironmentTextures( context, frameData, /*true, */out var environmentTexture, out var environmentTextureIBL );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurRoughnessMin,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, blurRoughnessMax,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					//shader.Parameters.Set( "1", new GpuMaterialPass.TextureParameterValue( reflectionTexture,
					//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					//shader.Parameters.Set( "2", new GpuMaterialPass.TextureParameterValue( bluredReflection,
					//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, depthTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, normalTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

					//!!!!rotation, multiplier

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, environmentTexture.Value.texture,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 6, environmentTextureIBL.Value.texture,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 7, Component_RenderingPipeline_Basic.BrdfLUT,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 8, gBuffer2Texture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 9, gBuffer0Texture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					shader.Parameters.Set( "invViewProj", invViewProjMatrix );
					shader.Parameters.Set( "cameraPosition", cameraPosition );
					shader.Parameters.Set( "intensity", (float)Intensity );
					shader.Parameters.Set( "ambientLightPower", ambientLightPower );

					context.RenderQuadToCurrentViewport( shader );
				}

				// Free Targets:
				context.DynamicTexture_Free( reflectionTexture );
				context.DynamicTexture_Free( blurRoughnessMin );
				context.DynamicTexture_Free( blurRoughnessMax );
				//context.RenderTarget_Free( bluredReflection );
				context.DynamicTexture_Free( actualTexture );

				// Update actual Texture:
				actualTexture = finalTexture;

				//result
				sceneTexture = actualTexture;
			}
		}
	}
}
