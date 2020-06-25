// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Class for managing hardware buffers.
	/// </summary>
	public static class GpuBufferManager
	{
		internal static ESet<GpuVertexBuffer> vertexBuffers = new ESet<GpuVertexBuffer>();
		internal static ESet<GpuIndexBuffer> indexBuffers = new ESet<GpuIndexBuffer>();
		//internal static ESet<GpuBuffer> buffers = new ESet<GpuBuffer>();

		//

		internal static void Init()
		{
		}

		unsafe internal static void Shutdown()
		{
			//!!!!dispose buffers?

			//foreach( var d in vertexDeclarations.Values )
			//{
			//	IntPtr native = d.GetNativeObject( false );
			//	if( native != IntPtr.Zero )
			//		OgreHardwareBufferManager.destroyVertexDeclaration( RendererWorld.realRoot, (void*)native );
			//}
			//vertexDeclarations.Clear();
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public static GpuBuffer[] GetBuffers()
		//{
		//	lock( buffers )
		//		return buffers.ToArray();
		//}

		public static GpuVertexBuffer CreateVertexBuffer( byte[] vertices, SharpBgfx.VertexLayout vertexDeclaration, GpuBufferFlags flags = 0 )
		{
			if( vertices.Length == 0 )
				Log.Fatal( "GpuBufferManager: CreateVertexBuffer: vertices.Length == 0." );
			if( flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				Log.Fatal( "GpuBufferManager: CreateVertexBuffer: ComputeWrite buffer can't write data from CPU." );

			var vertexCount = vertices.Length / vertexDeclaration.Stride;
			return new GpuVertexBuffer( vertices, vertexCount, vertexDeclaration, flags );
		}

		public static GpuVertexBuffer CreateVertexBuffer( int vertexCount, SharpBgfx.VertexLayout vertexDeclaration, GpuBufferFlags flags )
		{
			if( vertexCount == 0 )
				Log.Fatal( "GpuBufferManager: CreateVertexBuffer: vertexCount == 0." );

			return new GpuVertexBuffer( null, vertexCount, vertexDeclaration, flags );
		}

		public static GpuIndexBuffer CreateIndexBuffer( int[] indices, GpuBufferFlags flags = 0 )
		{
			if( indices.Length == 0 )
				Log.Fatal( "GpuBufferManager: CreateIndexBuffer: indices.Length == 0." );
			if( flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				Log.Fatal( "GpuBufferManager: CreateIndexBuffer: ComputeWrite buffer can't write data from CPU." );

			return new GpuIndexBuffer( indices, indices.Length, flags );
		}

		public static GpuIndexBuffer CreateIndexBuffer( int indexCount, GpuBufferFlags flags )
		{
			if( indexCount == 0 )
				Log.Fatal( "GpuBufferManager: CreateIndexBuffer: indexCount == 0." );

			return new GpuIndexBuffer( null, indexCount, flags );
		}

		public static ICollection<GpuVertexBuffer> VertexBuffers
		{
			get { return vertexBuffers; }
		}

		public static ICollection<GpuIndexBuffer> IndexBuffers
		{
			get { return indexBuffers; }
		}
	}
}
