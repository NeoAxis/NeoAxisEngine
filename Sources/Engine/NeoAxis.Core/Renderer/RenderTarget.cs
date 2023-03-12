// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// A canvas which can receive the results of a rendering operation.
	/// </summary>
	/// <remarks>
	/// This class defines a common root to all targets of rendering operations. A
	/// render target could be a window on a screen, or another
	/// offscreen surface like a texture or bump map etc.
	/// </remarks>
	public abstract class RenderTarget
	{
		internal FrameBuffer frameBuffer;
		bool frameBufferNeedDispose;
		internal Vector2I size;
		bool disposed;

		internal List<Viewport> viewports = new List<Viewport>();
		ReadOnlyCollection<Viewport> viewportsReadOnly;

		//!!!!было. это в viewport скорее
		//internal bool scissorEnabled;
		//internal RectI scissorRectangle;

		//

		internal RenderTarget( FrameBuffer frameBuffer, bool frameBufferNeedDispose, Vector2I size )
		{
			this.frameBuffer = frameBuffer;
			this.frameBufferNeedDispose = frameBufferNeedDispose;
			this.size = size;

			viewportsReadOnly = new ReadOnlyCollection<Viewport>( viewports );

			lock( RenderingSystem.renderTargets )
				RenderingSystem.renderTargets.Add( this );
		}

		/// <summary>Releases the resources that are used by the object.</summary>
		internal virtual void DisposeInternal()
		{
			if( !disposed )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "RenderTarget: Dispose after shutdown." );

				lock( RenderingSystem.renderTargets )
					RenderingSystem.renderTargets.Remove( this );
				//lock( RendererWorld.renderTargets )
				//	RendererWorld.renderTargets.Remove( (IntPtr)realObject );

				while( viewports.Count != 0 )
				{
					Viewport viewport = viewports[ viewports.Count - 1 ];
					viewport.Dispose();
				}

				if( frameBufferNeedDispose )
					frameBuffer.Dispose();
				disposed = true;
			}

			GC.SuppressFinalize( this );
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		/// <summary>
		/// Adds a viewport to the rendering target.
		/// </summary>
		/// <param name="insertIndex">
		/// The relative order of the viewport with others on the target (allows overlapping
		/// viewports i.e. picture-in-picture).
		/// </param>
		/// <returns>The viewport.</returns>
		/// <remarks>
		/// A viewport is the rectangle into which redering output is sent. This method adds
		/// a viewport to the render target, rendering from the supplied camera. The
		/// rest of the parameters are only required if you wish to add more than one viewport
		/// to a single rendering target. Note that size information passed to this method is
		/// passed as a parametric, i.e. it is relative rather than absolute. This is to allow
		/// viewports to automatically resize along with the target.
		/// </remarks>
		public Viewport AddViewport( bool createSimple3DRenderer, bool createCanvasRenderer, int insertIndex = 100000 )
		{
			EngineThreading.CheckMainThread();

			//!!!!!!как указывать rendering pipeline?

			unsafe
			{
				//OgreViewport* realViewport = (OgreViewport*)OgreRenderTarget.addViewport( realObject, insertIndex );
				Viewport viewport = new Viewport();// realViewport );
				viewport.parent = this;
				//viewport.UpdateNativeBackgroundColor();

				if( insertIndex >= viewports.Count )
					viewports.Add( viewport );
				else
					viewports.Insert( insertIndex, viewport );

				viewport.OnAdd( createSimple3DRenderer, createCanvasRenderer );

				return viewport;
			}
		}

		public FrameBuffer FrameBuffer
		{
			get { return frameBuffer; }
		}

		/// <summary>
		/// Gets the render target size.
		/// </summary>
		public Vector2I Size
		{
			get { return size; }
		}

		//!!!!
		///// <summary>
		///// Writes the current contents of the render target to the named file.
		///// </summary>
		///// <param name="fileName">The file name.</param>
		//public void WriteContentsToFile( string fileName )
		//{
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		OgreRenderTarget.writeContentsToFile( realObject, fileName );
		//	}
		//}

		//!!!!
		//public void WriteContentsToMemory( IntPtr buffer, int bufferSize, PixelFormat format )
		//{
		//	EngineThreading.CheckMainThread();

		//	if( bufferSize != Size.X * Size.Y * PixelFormatUtils.GetNumElemBytes( format ) )
		//	{
		//		Log.Fatal( "RenderTarget: WriteContentsToMemory: bufferSize != Size.X * Size.Y * PixelFormatUtils.GetNumElemBytes( format )." );
		//		return;
		//	}

		//	unsafe
		//	{
		//		OgreRenderTarget.writeContentsToMemory( realObject, buffer, format );
		//	}
		//}

		//!!!!
		//public void WriteContentsToMemory( byte[] buffer, PixelFormat format )
		//{
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		fixed ( byte* pBuffer = buffer )
		//		{
		//			WriteContentsToMemory( (IntPtr)pBuffer, buffer.Length, format );
		//		}
		//	}
		//}

		//!!!!!было
		//public void SetScissorTest( RectI rectangle )
		//{
		//	scissorEnabled = true;
		//	scissorRectangle = rectangle;
		//}

		//!!!!!было
		//public void ResetScissorTest()
		//{
		//	scissorEnabled = false;
		//}

		public IList<Viewport> Viewports
		{
			get { return viewportsReadOnly; }
		}
	}
}
