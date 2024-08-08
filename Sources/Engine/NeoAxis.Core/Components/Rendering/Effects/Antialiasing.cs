// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Anti-aliasing screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Antialiasing : RenderingEffect
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
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		public enum BasicTechniqueEnum
		{
			Auto,
			None,
			FXAA,
			//[DisplayNameEnum( "SSAA Quarter" )]
			//SSAAQuarter,
			//[DisplayNameEnum( "SSAA Half" )]
			//SSAAHalf,
			[DisplayNameEnum( "SSAA x2" )]
			SSAAx2,
			[DisplayNameEnum( "SSAA x3" )]
			SSAAx3,
			[DisplayNameEnum( "SSAA x4" )]
			SSAAx4,
			//[DisplayNameEnum( "SSAA x6" )]
			//SSAAx6,
			[DisplayNameEnum( "SSAA x8" )]
			SSAAx8,
			//MSAAx2,
			//MSAAx4,
			//MSAAx8,
			//MSAAx16,
		}

		public enum AdditionalTechniqueEnum
		{
			Auto,
			None,
			FXAA,
		}

		public enum MotionTechniqueEnum
		{
			Auto,
			None,
			TAA,
		}

		/// <summary>
		/// The basic technique of the antialiasing solution.
		/// </summary>
		[DefaultValue( BasicTechniqueEnum.Auto )]
		[Category( "Basic" )]
		public Reference<BasicTechniqueEnum> BasicTechnique
		{
			get { if( _basicTechnique.BeginGet() ) BasicTechnique = _basicTechnique.Get( this ); return _basicTechnique.value; }
			set { if( _basicTechnique.BeginSet( this, ref value ) ) { try { BasicTechniqueChanged?.Invoke( this ); } finally { _basicTechnique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BasicTechnique"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> BasicTechniqueChanged;
		ReferenceField<BasicTechniqueEnum> _basicTechnique = BasicTechniqueEnum.Auto;

		[DefaultValue( 1.0 )]
		[Range( 0, 3 )]
		[Category( "Basic" )]
		public Reference<double> DownscaleSamplerMultiplier
		{
			get { if( _downscaleSamplerMultiplier.BeginGet() ) DownscaleSamplerMultiplier = _downscaleSamplerMultiplier.Get( this ); return _downscaleSamplerMultiplier.value; }
			set { if( _downscaleSamplerMultiplier.BeginSet( this, ref value ) ) { try { DownscaleSamplerMultiplierChanged?.Invoke( this ); } finally { _downscaleSamplerMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscaleSamplerMultiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> DownscaleSamplerMultiplierChanged;
		ReferenceField<double> _downscaleSamplerMultiplier = 1.0;

		[DefaultValue( true )]
		[Category( "Basic" )]
		public Reference<bool> DownscaleBeforeSceneEffects
		{
			get { if( _downscaleBeforeSceneEffects.BeginGet() ) DownscaleBeforeSceneEffects = _downscaleBeforeSceneEffects.Get( this ); return _downscaleBeforeSceneEffects.value; }
			set { if( _downscaleBeforeSceneEffects.BeginSet( this, ref value ) ) { try { DownscaleBeforeSceneEffectsChanged?.Invoke( this ); } finally { _downscaleBeforeSceneEffects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscaleBeforeSceneEffects"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> DownscaleBeforeSceneEffectsChanged;
		ReferenceField<bool> _downscaleBeforeSceneEffects = true;

		/// <summary>
		/// The additional technique of the antialiasing solution.
		/// </summary>
		[DefaultValue( AdditionalTechniqueEnum.Auto )]
		[Category( "Additional" )]
		public Reference<AdditionalTechniqueEnum> AdditionalTechnique
		{
			get { if( _additionalTechnique.BeginGet() ) AdditionalTechnique = _additionalTechnique.Get( this ); return _additionalTechnique.value; }
			set { if( _additionalTechnique.BeginSet( this, ref value ) ) { try { AdditionalTechniqueChanged?.Invoke( this ); } finally { _additionalTechnique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AdditionalTechnique"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> AdditionalTechniqueChanged;
		ReferenceField<AdditionalTechniqueEnum> _additionalTechnique = AdditionalTechniqueEnum.Auto;

		/// <summary>
		/// The technique to work with moving objects and with moving camera.
		/// </summary>
		[DefaultValue( MotionTechniqueEnum.Auto )]
		[Category( "Motion" )]
		public Reference<MotionTechniqueEnum> MotionTechnique
		{
			get { if( _motionTechnique.BeginGet() ) MotionTechnique = _motionTechnique.Get( this ); return _motionTechnique.value; }
			set { if( _motionTechnique.BeginSet( this, ref value ) ) { try { MotionTechniqueChanged?.Invoke( this ); } finally { _motionTechnique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotionTechnique"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> MotionTechniqueChanged;
		ReferenceField<MotionTechniqueEnum> _motionTechnique = MotionTechniqueEnum.Auto;

		[DefaultValue( 1.3 )]//1.0 )]//1.7 )]//1.3 )]//2.0 )]
		[Range( 0, 4 )]
		[Category( "Motion" )]
		public Reference<double> Alpha
		{
			get { if( _alpha.BeginGet() ) Alpha = _alpha.Get( this ); return _alpha.value; }
			set { if( _alpha.BeginSet( this, ref value ) ) { try { AlphaChanged?.Invoke( this ); } finally { _alpha.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Alpha"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> AlphaChanged;
		ReferenceField<double> _alpha = 1.3;//1.0;//1.7;//1.3;//2.0;

		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Motion" )]
		public Reference<double> ColorBoxSigma
		{
			get { if( _colorBoxSigma.BeginGet() ) ColorBoxSigma = _colorBoxSigma.Get( this ); return _colorBoxSigma.value; }
			set { if( _colorBoxSigma.BeginSet( this, ref value ) ) { try { ColorBoxSigmaChanged?.Invoke( this ); } finally { _colorBoxSigma.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ColorBoxSigma"/> property value changes.</summary>
		public event Action<RenderingEffect_Antialiasing> ColorBoxSigmaChanged;
		ReferenceField<double> _colorBoxSigma = 1.0;

		/////////////////////////////////////////

		[Browsable( false )]
		public BasicTechniqueEnum BasicTechniqueAfterLoading = BasicTechniqueEnum.Auto;
		[Browsable( false )]
		public AdditionalTechniqueEnum AdditionalTechniqueAfterLoading = AdditionalTechniqueEnum.Auto;
		[Browsable( false )]
		public MotionTechniqueEnum MotionTechniqueAfterLoading = MotionTechniqueEnum.Auto;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( DownscaleSamplerMultiplier ):
				case nameof( DownscaleBeforeSceneEffects ):
					if( !BasicTechnique.Value.ToString().Contains( "SSAA" ) )
						//if( BasicTechnique.Value != BasicTechniqueEnum.SSAAx2 && BasicTechnique.Value != BasicTechniqueEnum.SSAAx3 && BasicTechnique.Value != BasicTechniqueEnum.SSAAx4 )
						skip = true;
					break;

				case nameof( Alpha ):
				case nameof( ColorBoxSigma ):
					if( MotionTechnique.Value != MotionTechniqueEnum.TAA )
						skip = true;
					break;
				}
			}
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			BasicTechniqueAfterLoading = BasicTechnique;
			AdditionalTechniqueAfterLoading = AdditionalTechnique;
			MotionTechniqueAfterLoading = MotionTechnique;

			return true;
		}

		public BasicTechniqueEnum GetBasicTechnique()
		{
			var result = BasicTechnique.Value;
			if( result == BasicTechniqueEnum.Auto )
			{
				if( SystemSettings.LimitedDevice )
					result = BasicTechniqueEnum.FXAA;
				else
					result = BasicTechniqueEnum.SSAAx2;
			}
			return result;
		}

		public AdditionalTechniqueEnum GetAdditionalTechnique()
		{
			var result = AdditionalTechnique.Value;
			if( result == AdditionalTechniqueEnum.Auto )
			{
				var basicTechnique = GetBasicTechnique();
				if( SystemSettings.LimitedDevice || basicTechnique == BasicTechniqueEnum.FXAA || basicTechnique == BasicTechniqueEnum.None )
					result = AdditionalTechniqueEnum.None;
				else
					result = AdditionalTechniqueEnum.FXAA;
			}
			return result;
		}

		public MotionTechniqueEnum GetMotionTechnique()
		{
			var result = MotionTechnique.Value;
			if( result == MotionTechniqueEnum.Auto )
			{
				if( SystemSettings.LimitedDevice )
					result = MotionTechniqueEnum.None;//no motion vector on mobile, because no MRT
				else
					result = MotionTechniqueEnum.TAA;
			}
			return result;
		}

		public static void RenderFXAA( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture, double intensity )
		{
			//render luma

			//!!!!!A8R8G8B8  где еще. когда tonemapping
			//!!!!Optionally do AA after tonemapping?

			var resultSize = actualTexture.Result.ResultSize;

			var lumaTexture = context.RenderTarget2D_Alloc( resultSize, PixelFormat.A8R8G8B8 );
			{
				context.SetViewport( lumaTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\FXAA\Luma_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old texture
			context.DynamicTexture_Free( actualTexture );

			//render final
			{
				actualTexture = context.RenderTarget2D_Alloc( resultSize, PixelFormat.A8R8G8B8 );

				context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\FXAA\Final_fs.sc";

				////no difference
				//if( EngineApp._DebugCapsLock )
				//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "FXAA_QUALITY__PRESET", "29" ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, lumaTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				var size = actualTexture.Result.ResultSize;
				shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

				shader.Parameters.Set( "intensity", (float)intensity );// Intensity );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free luma texture
			context.DynamicTexture_Free( lumaTexture );
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			if( Intensity > 0 )
			{
				if( GetBasicTechnique() != BasicTechniqueEnum.None )
				{
					if( GetBasicTechnique() == BasicTechniqueEnum.FXAA )
						RenderFXAA( context, frameData, ref actualTexture, Intensity );
					else
						RenderDownscale( context, frameData, ref actualTexture );
				}

				if( GetAdditionalTechnique() == AdditionalTechniqueEnum.FXAA )
					RenderFXAA( context, frameData, ref actualTexture, Intensity );

				if( GetMotionTechnique() != MotionTechniqueEnum.None )
				{
					if( GetMotionTechnique() == MotionTechniqueEnum.TAA )
						RenderTAA( context, frameData, ref actualTexture );
				}
			}
		}

		public virtual Vector2 GetResolutionMultiplier()
		{
			switch( GetBasicTechnique() )
			{
			//case BasicTechniqueEnum.SSAAQuarter: return new Vector2( 0.5, 0.5 );
			//case BasicTechniqueEnum.SSAAHalf: return new Vector2( 0.7, 0.7 );
			case BasicTechniqueEnum.SSAAx2: return new Vector2( 1.4, 1.4 );
			case BasicTechniqueEnum.SSAAx3: return new Vector2( 1.7, 1.7 );
			case BasicTechniqueEnum.SSAAx4: return new Vector2( 2.0, 2.0 );
			//case BasicTechniqueEnum.SSAAx6: return new Vector2( 2.45, 2.45 );
			case BasicTechniqueEnum.SSAAx8: return new Vector2( 2.82, 2.82 );
			}
			return Vector2.One;
		}

		public virtual int GetMSAALevel()
		{
			//switch( Technique.Value )
			//{
			//case TechniqueEnum.MSAAx2: return 2;
			//case TechniqueEnum.MSAAx4: return 4;
			//case TechniqueEnum.MSAAx8: return 8;
			//case TechniqueEnum.MSAAx16: return 16;
			//}
			return 0;
		}

		public void RenderDownscale( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			//if( BasicTechnique.Value >= BasicTechniqueEnum.SSAAQuarter && BasicTechnique.Value <= BasicTechniqueEnum.SSAAHalf && context.Owner.SizeInPixels != actualTexture.Result.ResultSize )
			//{
			//	var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			//	//copy to scene texture
			//	{
			//		context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//		CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//		shader.FragmentProgramFileName = @"Base\Shaders\Copy_fs.sc";

			//		//shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( Technique.Value.ToString().ToUpper() ) );

			//		//var multiplier = new Vector2F( 1, 1 ) / destinationSize.ToVector2F() * (float)DownscaleSamplerMultiplier.Value;
			//		//shader.Parameters.Set( "antialiasing_multiplier", multiplier );

			//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
			//			actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

			//		context.RenderQuadToCurrentViewport( shader );
			//	}

			//	context.DynamicTexture_Free( actualTexture );

			//	actualTexture = rescaledTexture;
			//}

			var destinationSize = context.SizeInPixelsLowResolutionBeforeUpscale;//context.Owner.SizeInPixels

			if( GetBasicTechnique().ToString().Contains( "SSAA" ) && destinationSize != actualTexture.Result.ResultSize )
			//if( GetBasicTechnique() >= BasicTechniqueEnum.SSAAx2 && GetBasicTechnique() <= BasicTechniqueEnum.SSAAx4 && destinationSize != actualTexture.Result.ResultSize )
			{
				var rescaledTexture = context.RenderTarget2D_Alloc( destinationSize, actualTexture.Result.ResultFormat );

				//copy to scene texture with downscale
				{
					context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\DownscaleAntialiasing_fs.sc";

					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( GetBasicTechnique().ToString().ToUpper() ) );

					var multiplier = new Vector2F( 1, 1 ) / actualTexture.Result.ResultSize/*destinationSize*/.ToVector2F() * (float)DownscaleSamplerMultiplier.Value;
					shader.Parameters.Set( "antialiasing_multiplier", multiplier );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

					context.RenderQuadToCurrentViewport( shader );
				}

				context.DynamicTexture_Free( actualTexture );

				actualTexture = rescaledTexture;
			}
		}

		string GetTAADataKey()
		{
			return "TAAPreviousColor " + GetPathFromRoot();
		}

		static ImageComponent TAACreatePreviousColorTexture( Vector2I size, PixelFormat format )
		{
			ImageComponent texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			texture.CreateSize = size;
			texture.CreateFormat = format;
			texture.CreateUsage = ImageComponent.Usages.RenderTarget;
			texture.Enabled = true;

			RenderTexture renderTexture = texture.Result.GetRenderTarget();
			var viewport = renderTexture.AddViewport( false, false );

			//!!!!?
			viewport.RenderingPipelineCreate();
			viewport.RenderingPipelineCreated.UseRenderTargets = false;

			return texture;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//!!!!может удалять если долго не юзается
			//if( !EnabledInHierarchy )
			//	TAADestroyPreviousColorTexture();
		}

		void TAADestroyPreviousColorTexture( ViewportRenderingContext context )
		{
			var anyDataKey = GetTAADataKey();

			context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
			if( current != null )
			{
				context.AnyDataAutoDispose.Remove( anyDataKey );
				current?.Dispose();
			}
		}

		void RenderTAA( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			var pipeline = context.RenderingPipeline as RenderingPipeline_Basic;
			if( pipeline == null )
				return;

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "motionAndObjectIdTexture", out var motionAndObjectIdTexture );
			if( motionAndObjectIdTexture == null )
			{
				TAADestroyPreviousColorTexture( context );
				return;
			}

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
			if( depthTexture == null )
			{
				TAADestroyPreviousColorTexture( context );
				return;
			}

			ImageComponent previousColorTexture = null;
			var previousColorTextureJustCreated = false;
			{
				var anyDataKey = GetTAADataKey();
				var demandedSize = actualTexture.Result.ResultSize;

				//check to destroy
				{
					context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
					var current2 = current as ImageComponent;

					if( current2 != null && current2.Result.ResultSize != demandedSize )
						TAADestroyPreviousColorTexture( context );
				}

				//create and get
				{
					context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
					var current2 = current as ImageComponent;

					if( current2 == null )
					{
						//create
						current2 = TAACreatePreviousColorTexture( demandedSize, actualTexture.Result.ResultFormat );
						context.AnyDataAutoDispose[ anyDataKey ] = current2;
						previousColorTextureJustCreated = true;
					}

					previousColorTexture = current2;
				}
			}

			//render final
			if( previousColorTexture != null && !previousColorTextureJustCreated )
			{
				ImageComponent newTexture;
				{
					newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );

					context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\TAA_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, motionAndObjectIdTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, previousColorTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					var size = newTexture.Result.ResultSize;
					shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

					shader.Parameters.Set( "intensity", (float)Intensity );

					var parameters = new Vector4F( (float)Alpha, (float)ColorBoxSigma, 0, 0 );
					shader.Parameters.Set( "taaParameters", parameters );

					context.RenderQuadToCurrentViewport( shader );
				}

				var oldTexture = actualTexture;
				actualTexture = newTexture;

				//free old texture
				context.DynamicTexture_Free( oldTexture );
			}

			//save actual scene image as a previous color image
			if( previousColorTexture != null )
			{
				context.SetViewport( previousColorTexture.Result.GetRenderTarget().Viewports[ 0 ] );
				pipeline.CopyToCurrentViewport( context, actualTexture );
			}

		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}
	}
}
