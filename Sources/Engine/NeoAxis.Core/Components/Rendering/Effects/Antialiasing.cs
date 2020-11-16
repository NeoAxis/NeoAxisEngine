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
	/// Anti-aliasing screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_Antialiasing : Component_RenderingEffect
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
		public event Action<Component_RenderingEffect_Antialiasing> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		public enum TechniqueEnum
		{
			None,
			FXAA,
			SSAAx2,
			SSAAx3,
			SSAAx4,
			//MSAAx2,
			//MSAAx4,
			//MSAAx8,
			//MSAAx16,
		}

		/// <summary>
		/// Used anti-aliasing technique.
		/// </summary>
		[DefaultValue( TechniqueEnum.SSAAx2 )]
		public Reference<TechniqueEnum> Technique
		{
			get { if( _technique.BeginGet() ) Technique = _technique.Get( this ); return _technique.value; }
			set { if( _technique.BeginSet( ref value ) ) { try { TechniqueChanged?.Invoke( this ); } finally { _technique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Technique"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Antialiasing> TechniqueChanged;
		ReferenceField<TechniqueEnum> _technique = TechniqueEnum.SSAAx2;

		[DefaultValue( 1.0 )]
		[Range( 0, 3 )]
		public Reference<double> DownscaleSamplerMultiplier
		{
			get { if( _downscaleSamplerMultiplier.BeginGet() ) DownscaleSamplerMultiplier = _downscaleSamplerMultiplier.Get( this ); return _downscaleSamplerMultiplier.value; }
			set { if( _downscaleSamplerMultiplier.BeginSet( ref value ) ) { try { DownscaleSamplerMultiplierChanged?.Invoke( this ); } finally { _downscaleSamplerMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscaleSamplerMultiplier"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Antialiasing> DownscaleSamplerMultiplierChanged;
		ReferenceField<double> _downscaleSamplerMultiplier = 1.0;

		[DefaultValue( true )]
		public Reference<bool> DownscaleBeforeSceneEffects
		{
			get { if( _downscaleBeforeSceneEffects.BeginGet() ) DownscaleBeforeSceneEffects = _downscaleBeforeSceneEffects.Get( this ); return _downscaleBeforeSceneEffects.value; }
			set { if( _downscaleBeforeSceneEffects.BeginSet( ref value ) ) { try { DownscaleBeforeSceneEffectsChanged?.Invoke( this ); } finally { _downscaleBeforeSceneEffects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscaleBeforeSceneEffects"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Antialiasing> DownscaleBeforeSceneEffectsChanged;
		ReferenceField<bool> _downscaleBeforeSceneEffects = true;

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
					if( Technique.Value == TechniqueEnum.None || Technique.Value == TechniqueEnum.FXAA /*|| ( Technique.Value >= TechniqueEnum.MSAAx2 && Technique.Value <= TechniqueEnum.MSAAx16 ) */)
						skip = true;
					break;
				}
			}
		}

		public static void RenderFXAA( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture, double intensity )
		{
			//render luma

			//!!!!!A8R8G8B8  где еще. когда tonemapping
			//!!!!Specifically do FXAA after tonemapping.

			//!!!!? PixelFormat.Float16RGBA
			var lumaTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );
			{
				context.SetViewport( lumaTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\FXAA\Luma_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//!!!!одно и тоже ставится

				//var size = context.owner.SizeInPixels;
				//shader.Parameters.Set( "viewportSize", new Vec4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVec4F() );

				//Mat4F identity = Mat4F.Identity;
				//shader.Parameters.Set( "worldViewProjMatrix", ParameterType.Matrix4x4, 1, &identity, sizeof( Mat4F ) );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old texture
			context.DynamicTexture_Free( actualTexture );

			//render final
			{
				actualTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );

				context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\FXAA\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, lumaTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				var size = actualTexture.Result.ResultSize;
				shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

				shader.Parameters.Set( "intensity", (float)intensity );// Intensity );

				//Mat4F identity = Mat4F.Identity;
				//shader.Parameters.Set( "worldViewProjMatrix", ParameterType.Matrix4x4, 1, &identity, sizeof( Mat4F ) );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free luma texture
			context.DynamicTexture_Free( lumaTexture );
		}

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			if( Technique.Value != TechniqueEnum.None )
			{
				if( Technique.Value == TechniqueEnum.FXAA )
					RenderFXAA( context, frameData, ref actualTexture, Intensity );
				else
					RenderDownscale( context, frameData, actualTexture.Result.ResultSize, ref actualTexture );
			}
		}

		public virtual Vector2 GetResolutionMultiplier()
		{
			switch( Technique.Value )
			{
			case TechniqueEnum.SSAAx2: return new Vector2( 1.4, 1.4 );
			case TechniqueEnum.SSAAx3: return new Vector2( 1.7, 1.7 );
			case TechniqueEnum.SSAAx4: return new Vector2( 2.0, 2.0 );
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

		public void RenderDownscale( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, Vector2I destinationSize, ref Component_Image actualTexture )
		{
			if( Technique.Value >= TechniqueEnum.SSAAx2 && Technique.Value <= TechniqueEnum.SSAAx4 && context.Owner.SizeInPixels != actualTexture.Result.ResultSize )
			{
				var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

				//copy to scene texture with downscale
				{
					context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\DownscaleAntialiasing_fs.sc";

					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( Technique.Value.ToString().ToUpper() ) );

					var multiplier = new Vector2F( 1, 1 ) / destinationSize.ToVector2F() * (float)DownscaleSamplerMultiplier.Value;
					shader.Parameters.Set( "antialiasing_multiplier", multiplier );

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
						actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

					context.RenderQuadToCurrentViewport( shader );
				}

				context.DynamicTexture_Free( actualTexture );

				actualTexture = rescaledTexture;
			}
		}
	}
}
