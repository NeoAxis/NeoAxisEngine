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
    /// Screen effect for converting scene to low dynamic range.
    /// </summary>
	[DefaultOrderOfEffect( 4 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_ToLDR : Component_RenderingEffect
	{
		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
			pipeline.ConvertToLDR( context, ref actualTexture );

			//var demandFormat = PixelFormat.A8R8G8B8;

			//if( actualTexture.Result.Format != demandFormat )
			//{
			//	var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, demandFormat );

			//	context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	//shader.VertexProgramFunctionName = "main_vp";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\ToLDR_fs.sc";
			//	//shader.FragmentProgramFunctionName = "main_fp";

			//	shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			//		TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			//	//var size = context.owner.DimensionsInPixels.Size;
			//	//shader.Parameters.Set( "viewportSize", new Vec4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVec4F() );

			//	//Mat4F identity = Mat4F.Identity;
			//	//shader.Parameters.Set( "worldViewProjMatrix", ParameterType.Matrix4x4, 1, &identity, sizeof( Mat4F ) );

			//	context.RenderQuadToCurrentViewport( shader );

			//	//free old texture
			//	context.RenderTarget_Free( actualTexture );

			//	actualTexture = newTexture;
			//}
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}
	}
}
