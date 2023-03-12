// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// Base class of skies.
	/// </summary>
	public abstract class Sky : Component
	{
		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//rendering pipeline optimization
			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Add( this );
				else
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Remove( this );
			}
		}

		public abstract void Render( RenderingPipeline pipeline, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData );//, Viewport renderToViewport );// ref Texture actualTexture );

		public abstract bool GetEnvironmentTextureData( out RenderingPipeline.EnvironmentTextureData environmentCubemap, out RenderingPipeline.EnvironmentIrradianceData irradianceHarmonics );

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Sky", true );
		}
	}
}
