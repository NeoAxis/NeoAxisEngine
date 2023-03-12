// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect for converting scene to high dynamic range.
	/// </summary>
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ToHDR : RenderingEffect
	{
		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			var demandFormat = PixelFormat.Float16RGBA;

			if( actualTexture.Result.ResultFormat != demandFormat )
			{
				var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, demandFormat );

				context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				//shader.VertexProgramFunctionName = "main_vp";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\ToHDR_fs.sc";
				//shader.FragmentProgramFunctionName = "main_fp";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//var size = context.owner.DimensionsInPixels.Size;
				//shader.Parameters.Set( "viewportSize", new Vec4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVec4F() );

				//Mat4F identity = Mat4F.Identity;
				//shader.Parameters.Set( "worldViewProjMatrix", ParameterType.Matrix4x4, 1, &identity, sizeof( Mat4F ) );

				context.RenderQuadToCurrentViewport( shader );

				//free old texture
				context.DynamicTexture_Free( actualTexture );

				actualTexture = newTexture;
			}
		}
	}
}
