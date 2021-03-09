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
	public abstract class Component_Sky : Component
	{
		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//rendering pipeline optimization
			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Add( this );
				else
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Remove( this );
			}
		}

		public abstract void Render( Component_RenderingPipeline pipeline, ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData );//, Viewport renderToViewport );// ref Component_Texture actualTexture );

		public abstract bool GetEnvironmentTextureData( out Component_RenderingPipeline.EnvironmentTextureData environmentCubemap, out Component_RenderingPipeline.EnvironmentTextureData irradianceCubemap );

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Sky", true );
		}
	}
}
