// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// Per-object motion blur.
	/// </summary>
	[DefaultOrderOfEffect( 1.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_MotionBlur : Component_RenderingEffect
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
		public event Action<Component_RenderingEffect_MotionBlur> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// Output velocity multiplier.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_MotionBlur> MultiplierChanged;
		ReferenceField<double> _multiplier = 1.0;

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			context.objectsDuringUpdate.namedTextures.TryGetValue( "motionTexture", out var motionTexture );

			if( motionTexture != null )
			{
				//create final
				var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
				{
					context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					var shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\MotionBlur_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"motionTexture"*/, motionTexture,
						TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( "intensity", (float)Intensity );

					var multiplier = 0.0;
					if( context.Owner.LastUpdateTimeStep != 0 )
						multiplier = ( 1.0 / context.Owner.LastUpdateTimeStep ) / 45.0;
					multiplier *= Multiplier.Value;

					//fix initial rattling
					if( context.Owner.LastUpdateTimeStep > 0.1 )
						multiplier = 0;

					shader.Parameters.Set( "motionBlurMultiplier", (float)multiplier );

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
}
