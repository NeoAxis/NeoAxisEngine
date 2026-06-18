// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Internal.SharpBgfx;

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

			this.size = size;

			//maybe not is all API need to recreate frame buffer when window size changed
			if( thisIsApplicationWindow )
			{
				Bgfx.Reset( size.X, size.Y, RenderingSystem.GetApplicationWindowResetFlags() );
			}
			else
			{
				frameBuffer.Dispose();
				frameBuffer = new FrameBuffer( windowHandle, size.X, size.Y );
			}

			//update aspect ratio
			foreach( Viewport viewport in viewports )
				viewport.UpdateAspectRatio();
		}
	}
}
