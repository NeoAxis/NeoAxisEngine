// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Represents a vertex buffer.
	/// </summary>
	public class GpuVertexBuffer : ThreadSafeDisposable
	{
		byte[] vertices;
		SharpBgfx.VertexLayout vertexDeclaration;
		int vertexSize;
		int vertexCount;
		int vertexCountActual;
		GpuBufferFlags flags;

		IDisposable realObject;
		bool dynamic_needUpdateNative;

		//

		internal unsafe GpuVertexBuffer( byte[] vertices, int vertexCount, SharpBgfx.VertexLayout vertexDeclaration, GpuBufferFlags flags )
		{
			lock( GpuBufferManager.vertexBuffers )
				GpuBufferManager.vertexBuffers.Add( this );

			this.vertices = vertices;
			this.vertexDeclaration = vertexDeclaration;
			this.vertexCount = vertexCount;
			vertexCountActual = vertexCount;
			this.flags = flags;
			vertexSize = vertexDeclaration.Stride;

			if( Flags.HasFlag( GpuBufferFlags.Dynamic ) )
			{
				if( this.vertices == null )
					this.vertices = new byte[ vertexCount * vertexSize ];
				dynamic_needUpdateNative = true;
			}
		}

		protected override void OnDispose()
		{
			if( realObject != null )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
				{
					//waiting for .NET Standard 2.0
					Log.Fatal( "Renderer: Dispose after Shutdown." );
					//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
				}
				EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, realObject );
				realObject = null;
			}

			lock( GpuBufferManager.vertexBuffers )
				GpuBufferManager.vertexBuffers.Remove( this );

			//base.OnDispose();
		}

		public byte[] Vertices
		{
			get { return vertices; }
		}

		public SharpBgfx.VertexLayout VertexDeclaration
		{
			get { return vertexDeclaration; }
		}

		public int VertexSize
		{
			get { return vertexSize; }
		}

		public int VertexCount
		{
			get { return vertexCount; }
		}

		public GpuBufferFlags Flags
		{
			get { return flags; }
		}

		public void SetData( byte[] vertices, int vertexCountActual = -1 )
		{
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
				Log.Fatal( "GpuVertexBuffer: SetData: The buffer must be dynamic or must be still not be created on GPU." );
			if( this.vertices != null && vertices.Length != this.vertices.Length )
				Log.Fatal( "GpuVertexBuffer: SetData: vertices.Length != this.vertices.Length." );

			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			this.vertices = vertices;
			this.vertexCountActual = vertexCountActual != -1 ? vertexCountActual : vertexCount;
			dynamic_needUpdateNative = true;
		}

		public void Write( IntPtr vertices, int vertexCount )
		{
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
				Log.Fatal( "GpuVertexBuffer: Write: The buffer must be dynamic or must be still not be created on GPU." );
			if( vertexCount > this.vertexCount )
				Log.Fatal( "GpuIndexBuffer: Write: vertexCount > this.vertexCount." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			unsafe
			{
				fixed ( byte* pVertices = this.vertices )
					NativeUtility.CopyMemory( (IntPtr)pVertices, vertices, vertexCount * vertexSize );
			}
			vertexCountActual = vertexCount;
			dynamic_needUpdateNative = true;
		}

		public void Write( IntPtr vertices )
		{
			Write( vertices, vertexCount );
		}

		public byte[] WriteBegin()
		{
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
				Log.Fatal( "GpuVertexBuffer: WriteBegin: The buffer must be dynamic or must be still not be created on GPU." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			return vertices;
		}

		public void WriteEnd()
		{
			dynamic_needUpdateNative = true;
			vertexCountActual = vertexCount;
		}

		internal IDisposable GetNativeObject()
		{
			if( Disposed )
				Log.Fatal( "GpuVertexBuffer: GetNativeObject: Disposed." );

			EngineThreading.CheckMainThread();

			if( Flags.HasFlag( GpuBufferFlags.Dynamic ) || Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
			{
				////delete old static when changed to dynamic
				//if( realObject != null && realObject is SharpBgfx.VertexBuffer )
				//{
				//	realObject.Dispose();
				//	realObject = null;
				//}

				if( realObject == null )
				{
					var nativeFlags = SharpBgfx.BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
						nativeFlags |= SharpBgfx.BufferFlags.ComputeWrite;

					realObject = new SharpBgfx.DynamicVertexBuffer( vertexCount, vertexDeclaration, nativeFlags );
					dynamic_needUpdateNative = true;
				}

				if( !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				{
					if( dynamic_needUpdateNative )
					{
						dynamic_needUpdateNative = false;

						var buffer = (SharpBgfx.DynamicVertexBuffer)realObject;
						var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<byte>( vertices, 0, vertexCountActual * vertexSize ) );
						buffer.Update( 0, memory );
					}
				}
			}
			else
			{
				if( realObject == null )
				{
					var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( vertices );

					realObject = new SharpBgfx.VertexBuffer( memory, vertexDeclaration, SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
				}
			}

			return realObject;
		}

		public T[] ExtractChannel<T>( int startOffsetInBytes ) where T : unmanaged
		{
			T[] result = new T[ VertexCount ];
			unsafe
			{
				fixed ( byte* pVertices = vertices )
				{
					byte* src = pVertices + startOffsetInBytes;
					for( int n = 0; n < VertexCount; n++ )
					{
						result[ n ] = *(T*)src;
						src += VertexSize;
					}
				}
			}
			return result;
		}

		//public T[] ExtractData<T>( VertexElement element ) where T : struct
		//{
		//	unsafe
		//	{
		//		return null;
		//	}
		//}

		//public float[] ExtractChannelFloat1( int startOffsetInBytes )
		//{
		//	float[] result = new float[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(float*)src;
		//				src += VertexSize;
		//			}
		//		}

		//	}
		//	return result;
		//}

		//public Vec2F[] ExtractChannelFloat2( int startOffsetInBytes )
		//{
		//	Vec2F[] result = new Vec2F[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec2F*)src;
		//				src += VertexSize;
		//			}
		//		}

		//	}
		//	return result;
		//}

		//public Vec3F[] ExtractChannelFloat3( int startOffsetInBytes )
		//{
		//	Vec3F[] result = new Vec3F[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec3F*)src;
		//				src += VertexSize;
		//			}
		//		}

		//	}
		//	return result;
		//}

		//public Vec4F[] ExtractChannelFloat4( int startOffsetInBytes )
		//{
		//	Vec4F[] result = new Vec4F[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec4F*)src;
		//				src += VertexSize;
		//			}
		//		}

		//	}
		//	return result;
		//}

		//public uint[] ExtractChannelByte4( int startOffsetInBytes )
		//{
		//	uint[] result = new uint[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(uint*)src;
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	return result;
		//}

		//public int[] ExtractChannelInteger1( int startOffsetInBytes )
		//{
		//	int[] result = new int[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(int*)src;
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	return result;
		//}

		//public Vec2I[] ExtractChannelInteger2( int startOffsetInBytes )
		//{
		//	Vec2I[] result = new Vec2I[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec2I*)src;
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	return result;
		//}

		//public Vec3I[] ExtractChannelInteger3( int startOffsetInBytes )
		//{
		//	Vec3I[] result = new Vec3I[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec3I*)src;
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	return result;
		//}

		//public Vec4I[] ExtractChannelInteger4( int startOffsetInBytes )
		//{
		//	Vec4I[] result = new Vec4I[ VertexCount ];
		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				result[ n ] = *(Vec4I*)src;
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	return result;
		//}

		public void MakeCopyOfData()
		{
			if( vertices != null )
				vertices = (byte[])vertices.Clone();
		}

		public void WriteChannel<T>( int startOffsetInBytes, T[] data ) where T : unmanaged
		{
			WriteBegin();
			unsafe
			{
				fixed ( byte* pVertices = vertices )
				{
					byte* src = pVertices + startOffsetInBytes;
					for( int n = 0; n < VertexCount; n++ )
					{
						*(T*)src = data[ n ];
						src += VertexSize;
					}
				}
			}
			WriteEnd();
		}

		public unsafe void WriteChannel<T>( int startOffsetInBytes, T* data ) where T : unmanaged
		{
			WriteBegin();
			fixed ( byte* pVertices = vertices )
			{
				byte* src = pVertices + startOffsetInBytes;
				for( int n = 0; n < VertexCount; n++ )
				{
					*(T*)src = data[ n ];
					src += VertexSize;
				}
			}
			WriteEnd();
		}

		//public void WriteChannel( int startOffsetInBytes, float[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(float*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, float* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(float*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec2F[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec2F*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec2F* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec2F*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec3F[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec3F*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec3F* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec3F*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec4F[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec4F*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec4F* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec4F*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, uint[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(uint*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, uint* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(uint*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, int[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(int*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, int* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(int*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec2I[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec2I*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec2I* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec2I*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec3I[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec3I*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec3I* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec3I*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public void WriteChannel( int startOffsetInBytes, Vec4I[] data )
		//{
		//	//!!!!проверки

		//	unsafe
		//	{
		//		fixed ( byte* pVertices = vertices )
		//		{
		//			byte* src = pVertices + startOffsetInBytes;
		//			for( int n = 0; n < VertexCount; n++ )
		//			{
		//				*(Vec4I*)src = data[ n ];
		//				src += VertexSize;
		//			}
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}

		//public unsafe void WriteChannel( int startOffsetInBytes, Vec4I* data )
		//{
		//	fixed ( byte* pVertices = vertices )
		//	{
		//		byte* src = pVertices + startOffsetInBytes;
		//		for( int n = 0; n < VertexCount; n++ )
		//		{
		//			*(Vec4I*)src = data[ n ];
		//			src += VertexSize;
		//		}
		//	}
		//	dynamic_needUpdateNative = true;
		//}


		//Double1,2,3,4, Int1,2,3,4, UInt1,2,3,4, Color

		//public StandardVertexF[] ExtractDataStandardVertexF()
		//{
		//}

		//public void SetDataStandardVertexF( StandardVertexF[] vertices )
		//{
		//}
	}
}
