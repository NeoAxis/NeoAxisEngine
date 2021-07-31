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
	/// Screen effect for adding a bloom of the image. 
	/// </summary>
	[DefaultOrderOfEffect( 2 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_Bloom : Component_RenderingEffect// Component_RenderingEffect_Simple
	{
		//!!!!property names
		//!!!!default values

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
		public event Action<Component_RenderingEffect_Bloom> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The brightness threshold.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.2 )]
		[Range( 0.1, 2.0 )]
		public Reference<double> BrightThreshold
		{
			get { if( _brightThreshold.BeginGet() ) BrightThreshold = _brightThreshold.Get( this ); return _brightThreshold.value; }
			set { if( _brightThreshold.BeginSet( ref value ) ) { try { BrightThresholdChanged?.Invoke( this ); } finally { _brightThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BrightThreshold"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Bloom> BrightThresholdChanged;
		ReferenceField<double> _brightThreshold = 1.2;

		/// <summary>
		/// The scale of the bloom effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.25 )]
		[Range( 0.0, 5.0 )]
		public Reference<double> Scale
		{
			get { if( _scale.BeginGet() ) Scale = _scale.Get( this ); return _scale.value; }
			set { if( _scale.BeginSet( ref value ) ) { try { ScaleChanged?.Invoke( this ); } finally { _scale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scale"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Bloom> ScaleChanged;
		ReferenceField<double> _scale = 1.25;

		/// <summary>
		/// The amount of the blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 5.0 )]
		[Range( 0, 15, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Bloom> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 5;

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
		public event Action<Component_RenderingEffect_Bloom> BlurDownscalingModeChanged;
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
		public event Action<Component_RenderingEffect_Bloom> BlurDownscalingValueChanged;
		ReferenceField<int> _blurDownscalingValue = 0;

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

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			//PixelFormat.A8R8G8B8 );

			//bright pass
			//!!!!или A8R8G8B8?
			//R8G8B8
			var brightTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );//!!!! PixelFormat.R8G8B8 );
			{
				context.SetViewport( brightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Bloom\Bright_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
				shader.Parameters.Set( "brightThreshold", (float)BrightThreshold );

				context.RenderQuadToCurrentViewport( shader );
			}

			//blur
			var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
			var blurTexture = pipeline.GaussianBlur( context, this, brightTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );
			context.DynamicTexture_Free( brightTexture );

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Bloom\Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"bloomTexture"*/, blurTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "scale", (float)Scale );

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

	}
}
