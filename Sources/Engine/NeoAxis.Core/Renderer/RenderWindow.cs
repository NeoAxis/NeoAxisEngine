// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Manages the target rendering window.
	/// </summary>
	public class RenderWindow : RenderTarget
	{
		IntPtr windowHandle;
		bool thisIsApplicationWindow;

		//

		internal RenderWindow( FrameBuffer frameBuffer, Vector2I size, IntPtr windowHandle, bool thisIsApplicationWindow )
			: base( frameBuffer, !thisIsApplicationWindow, size )
		{
			this.windowHandle = windowHandle;
			this.thisIsApplicationWindow = thisIsApplicationWindow;
		}

		public IntPtr WindowHandle
		{
			get { return windowHandle; }
		}

		public bool ThisIsApplicationWindow
		{
			get { return thisIsApplicationWindow; }
		}

		/// <summary>Releases the resources that are used by the object.</summary>
		unsafe internal override void DisposeInternal()
		{
			//!!!!new
			//if( realObjectRenderWindow != null )
			//{
			//	//after shutdown check
			//	if( RendererWorld.Disposed )
			//	{
			//		//waiting for .NET Standard 2.0
			//		Log.Fatal( "Renderer: Dispose after Shutdown." );
			//		//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
			//	}

			//	realObjectRenderWindow = null;
			//}

			base.DisposeInternal();

			////!!!!new
			//if( realObjectRenderWindow != null )
			//{
			//	OgreRenderSystem.destroyRenderWindow( RendererWorld.realRoot, realObjectRenderWindow );
			//	realObjectRenderWindow = null;
			//}
		}

		/// <summary>Releases the resources that are used by the object.</summary>
		public void Dispose()
		{
			EngineThreading.CheckMainThread();

			DisposeInternal();
		}

		public void WindowMovedOrResized( Vector2I size )// bool fullScreen )//, Vec2I windowSize )
		{
			if( Disposed )
				return;

			EngineThreading.CheckMainThread();

			//unsafe
			//{
			//!!!!было
			//MyOgreSceneManager.destroyShadowTextures( SceneManager.realObject );

			//!!!!как теперь? текстуры композитинга удалять? где еще?
			//MyOgreSceneManager.destroyAdditionalMRTs( SceneManager.realObject );
			//}

			//foreach( Viewport viewport in viewports )
			//{
			//   foreach( CompositorInstance instance in viewport.CompositorInstances )
			//      instance.DoRealEnabledUpdate( false );
			//}

			this.size = size;

			//!!!!по идее не во всех графических API надо пересоздавать
			if( thisIsApplicationWindow )
			{
				Bgfx.Reset( size.X, size.Y, RenderingSystem.GetApplicationWindowResetFlags() );
			}
			else
			{
				frameBuffer.Dispose();
				frameBuffer = new FrameBuffer( windowHandle, size.X, size.Y );
			}

			//unsafe
			//{
			//	OgreRenderWindow.windowMovedOrResized( realObjectRenderWindow, fullScreen );//, windowSize.X, windowSize.Y );
			//}
			//GetSizeFromNative();

			//update aspect ratio
			foreach( Viewport viewport in viewports )
				viewport.UpdateAspectRatio();
		}
	}
}
