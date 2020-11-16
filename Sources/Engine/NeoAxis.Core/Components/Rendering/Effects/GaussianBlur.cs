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
	/// Gaussian blur screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 11 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_GaussianBlur : Component_RenderingEffect// Component_RenderingEffect_Simple
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
		public event Action<Component_RenderingEffect_GaussianBlur> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The amount of blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 15, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_GaussianBlur> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 1.0;

		/// <summary>
		/// The image downscaling mode used.
		/// </summary>
		[DefaultValue( Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		[Serialize]
		public Reference<Component_RenderingPipeline_Basic.DownscalingModeEnum> DownscalingMode
		{
			get { if( _downscalingMode.BeginGet() ) DownscalingMode = _downscalingMode.Get( this ); return _downscalingMode.value; }
			set { if( _downscalingMode.BeginSet( ref value ) ) { try { DownscalingModeChanged?.Invoke( this ); } finally { _downscalingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscalingMode"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_GaussianBlur> DownscalingModeChanged;
		ReferenceField<Component_RenderingPipeline_Basic.DownscalingModeEnum> _downscalingMode = Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level of downscaling applied.
		/// </summary>
		[DefaultValue( 0 )]
		[Serialize]
		[Range( 0, 6 )]
		public Reference<int> DownscalingValue
		{
			get { if( _downscalingValue.BeginGet() ) DownscalingValue = _downscalingValue.Get( this ); return _downscalingValue.value; }
			set { if( _downscalingValue.BeginSet( ref value ) ) { try { DownscalingValueChanged?.Invoke( this ); } finally { _downscalingValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DownscalingValue"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_GaussianBlur> DownscalingValueChanged;
		ReferenceField<int> _downscalingValue = 0;

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
		//public event Action<Component_RenderingEffect_GaussianBlur> BlurDimensionsChanged;

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
					if( DownscalingMode.Value == Component_RenderingPipeline_Basic.DownscalingModeEnum.Auto )
					{
						skip = true;
						return;
					}
					break;
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			//blur
			var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
			var blurTexture = pipeline.GaussianBlur( context, this, actualTexture, BlurFactor, DownscalingMode, DownscalingValue );

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Lerp_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"source1Texture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"source2Texture"*/, blurTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( "intensity", (float)Intensity );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );
			context.DynamicTexture_Free( blurTexture );

			//update actual texture
			actualTexture = finalTexture;



			//////////////

			//int downscaling;
			//if( DownscalingMode.Value == DownscalingModeEnum.Auto )
			//	downscaling = (int)( BlurFactor / 1.5 );
			//else
			//	downscaling = Downscaling;

			////var dimensions = BlurDimensions.Value;
			////var downscaling = Math.Max( Downscaling.Value, 1 );
			////var size = context.Owner.SizeInPixels / downscaling;

			//List<Component_Texture> textureToFree = new List<Component_Texture>();
			//Component_Texture currentTexture = actualTexture;

			////Component_Texture textureDownscalingd = null;
			////Component_Texture textureH = null;
			////Component_Texture textureV = null;

			////downscaling
			//for( int n = 0; n < downscaling; n++ )
			//{
			//	var texture = context.RenderTarget2D_Alloc( currentTexture.Result.Size / 2, currentTexture.Result.Format );
			//	{
			//		context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

			//		var shader = new CanvasRenderer.ShaderItem();
			//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

			//		shader.Parameters.Set( "sourceSizeInv", new Vec2F( 1, 1 ) / currentTexture.Result.Size.ToVec2F() );

			//		//shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			//		//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
			//		shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( currentTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

			//		context.RenderQuadToCurrentViewport( shader );
			//	}

			//	currentTexture = texture;
			//	textureToFree.Add( texture );
			//}

			////horizontal blur
			////if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Horizontal )
			//{
			//	var texture = context.RenderTarget2D_Alloc( actualTexture.Result.Size, currentTexture.Result.Format );
			//	//var texture = context.RenderTarget2D_Alloc( currentTexture.Result.Size, currentTexture.Result.Format );
			//	{
			//		context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

			//		var shader = new CanvasRenderer.ShaderItem();
			//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

			//		shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( currentTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
			//		SetShaderParameters( shader, texture.Result.Size, true );

			//		context.RenderQuadToCurrentViewport( shader );
			//	}

			//	currentTexture = texture;
			//	textureToFree.Add( texture );
			//}

			////vertical blur
			////if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Vertical )
			//{
			//	var texture = context.RenderTarget2D_Alloc( actualTexture.Result.Size, currentTexture.Result.Format );
			//	//var texture = context.RenderTarget2D_Alloc( currentTexture.Result.Size, currentTexture.Result.Format );
			//	{
			//		context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

			//		var shader = new CanvasRenderer.ShaderItem();
			//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

			//		shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( currentTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
			//		SetShaderParameters( shader, texture.Result.Size, false );

			//		context.RenderQuadToCurrentViewport( shader );
			//	}

			//	currentTexture = texture;
			//	textureToFree.Add( texture );
			//}

			//////horizontal blur
			////if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Horizontal )
			////{
			////	textureH = context.RenderTarget2D_Alloc( size, actualTexture.Result.Format );
			////	{
			////		context.SetViewport( textureH.Result.GetRenderTarget().Viewports[ 0 ] );

			////		var shader = new CanvasRenderer.ShaderItem();
			////		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			////		shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

			////		shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			////			TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
			////		SetShaderParameters( shader, size, true );

			////		context.RenderQuadToCurrentViewport( shader );
			////	}
			////}

			//////vertical blur
			////if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Vertical )
			////{
			////	textureV = context.RenderTarget2D_Alloc( size, actualTexture.Result.Format );
			////	{
			////		context.SetViewport( textureV.Result.GetRenderTarget().Viewports[ 0 ] );

			////		var shader = new CanvasRenderer.ShaderItem();
			////		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			////		shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

			////		shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( textureH != null ? textureH : actualTexture,
			////			TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );
			////		SetShaderParameters( shader, size, false );

			////		context.RenderQuadToCurrentViewport( shader );
			////	}
			////}

			//////free old texture
			////if( textureH != null && textureV != null )
			////{
			////	context.RenderTarget_Free( textureH );
			////	textureH = null;
			////}

			////create final
			//var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.Format );
			//{
			//	context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	var shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Lerp_fs.sc";

			//	shader.Parameters.Set( "0"/*"source1Texture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			//		TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
			//	shader.Parameters.Set( "1"/*"source2Texture"*/, new GpuMaterialPass.TextureParameterValue( currentTexture,
			//		TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
			//	shader.Parameters.Set( "intensity", (float)Intensity );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			////free old textures
			//context.RenderTarget_Free( actualTexture );
			//foreach( var texture in textureToFree )
			//	context.RenderTarget_Free( texture );
			////if( textureH != null )
			////	context.RenderTarget_Free( textureH );
			////if( textureV != null )
			////	context.RenderTarget_Free( textureV );

			////update actual texture
			//actualTexture = finalTexture;
		}

		//void SetShaderParameters( CanvasRenderer.ShaderItem shader, Vec2I size, bool horizontal )
		//{
		//	var values = GaussianBlurMath.Calculate15( size, horizontal, BlurFactor );
		//	shader.Parameters.Set( "sampleOffsets", values.SampleOffsetsAsVec4Array );
		//	shader.Parameters.Set( "sampleWeights", values.SampleWeights );
		//}

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
