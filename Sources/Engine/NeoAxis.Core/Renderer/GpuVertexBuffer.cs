// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.CompilerServices;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Represents a vertex buffer.
	/// </summary>
	public class GpuVertexBuffer : ThreadSafeDisposable
	{
		byte[] vertices;
		VertexLayout vertexDeclaration;
		int vertexSize;
		int vertexCount;
		int vertexCountActual;
		GpuBufferFlags flags;

		ushort nativeObjectHandle = ushort.MaxValue;
		bool nativeObjectIsDynamicBuffer;
		//VertexBuffer vertexBuffer;
		//DynamicVertexBuffer dynamicVertexBuffer;
		//IDisposable nativeObject;

		bool dynamic_needUpdateNative;
		double nativeObjectLastUsedTime;

		//

		internal unsafe GpuVertexBuffer( byte[] vertices, int vertexCount, VertexLayout vertexDeclaration, GpuBufferFlags flags )
		{
			lock( GpuBufferManager.vertexBuffers )
				GpuBufferManager.vertexBuffers.Add( this );

			this.vertices = vertices;
			this.vertexDeclaration = vertexDeclaration;
			this.vertexCount = vertexCount;
			vertexCountActual = vertexCount;
			this.flags = flags;
			vertexSize = vertexDeclaration.Stride;

			if( ( Flags & GpuBufferFlags.Dynamic ) != 0 )
			{
				if( this.vertices == null )
					this.vertices = new byte[ vertexCount * vertexSize ];
				dynamic_needUpdateNative = true;
			}
		}

		protected override void OnDispose()
		{
			if( nativeObjectHandle != ushort.MaxValue )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "GpuVertexBuffer: Dispose after shutdown." );

				DestroyNativeObject();
				//EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
				//nativeHandle = ushort.MaxValue;
			}

			//if( nativeObject != null )
			//{
			//	//after shutdown check
			//	if( RenderingSystem.Disposed )
			//		Log.Fatal( "GpuVertexBuffer: Dispose after shutdown." );
			//	EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
			//	nativeObject = null;
			//	nativeHandle = 0;
			//}

			lock( GpuBufferManager.vertexBuffers )
				GpuBufferManager.vertexBuffers.Remove( this );

			//base.OnDispose();
		}

		public byte[] Vertices
		{
			get { return vertices; }
		}

		public VertexLayout VertexDeclaration
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

		public ushort NativeObjectHandle
		{
			get { return nativeObjectHandle; }
		}

		public bool NativeObjectCreated
		{
			get { return nativeObjectHandle != ushort.MaxValue; }
		}

		public bool NativeObjectIsDynamicBuffer
		{
			get { return nativeObjectIsDynamicBuffer; }
		}

		//public IDisposable NativeObject
		//{
		//	get { return nativeObject; }
		//}

		public void SetData( byte[] vertices, int vertexCountActual = -1 )
		{
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
				Log.Fatal( "GpuVertexBuffer: SetData: The buffer must be dynamic or must be still not be created on GPU." );
			//if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( nativeObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
			//	Log.Fatal( "GpuVertexBuffer: SetData: The buffer must be dynamic or must be still not be created on GPU." );

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
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
				Log.Fatal( "GpuVertexBuffer: Write: The buffer must be dynamic or must be still not be created on GPU." );

			if( vertexCount > this.vertexCount )
				Log.Fatal( "GpuIndexBuffer: Write: vertexCount > this.vertexCount." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			unsafe
			{
				fixed( byte* pVertices = this.vertices )
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
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
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

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe internal void UpdateNativeObject()
		{
			if( Disposed )
				Log.Fatal( "GpuVertexBuffer: GetNativeObject: Disposed." );

			if( ( Flags & ( GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeWrite ) ) != 0 )
			//if( ( ( Flags & GpuBufferFlags.Dynamic ) != 0 ) || ( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 ) )
			{
				if( nativeObjectHandle == ushort.MaxValue )
				{
					EngineThreading.CheckMainThread();

					BufferFlags nativeFlags = 0;
					if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
						nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 )
						nativeFlags |= BufferFlags.ComputeWrite;

					nativeObjectHandle = NativeMethods.bgfx_create_dynamic_vertex_buffer( vertexCount, ref vertexDeclaration.data, nativeFlags );
					nativeObjectIsDynamicBuffer = true;

					//var buffer = new DynamicVertexBuffer( vertexCount, vertexDeclaration, nativeFlags );
					//nativeObject = buffer;
					//nativeObjectHandle = buffer.handle;
					dynamic_needUpdateNative = true;
				}

				if( ( Flags & GpuBufferFlags.ComputeWrite ) == 0 )
				{
					if( dynamic_needUpdateNative )
					{
						dynamic_needUpdateNative = false;

						fixed( byte* pVertices = vertices )
						{
							var memory = new MemoryBlock( pVertices, vertexCountActual * vertexSize );
							NativeMethods.bgfx_update_dynamic_vertex_buffer( nativeObjectHandle, 0, memory.ptr );
						}

						//var buffer = (DynamicVertexBuffer)nativeObject;
						//fixed( byte* pVertices = vertices )
						//{
						//	var memory = new MemoryBlock( pVertices, vertexCountActual * vertexSize );
						//	buffer.Update( 0, memory );
						//}

						////var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<byte>( vertices, 0, vertexCountActual * vertexSize ) );
						////buffer.Update( 0, memory );
					}
				}

				//if( nativeObject == null )
				//{
				//	BufferFlags nativeFlags = 0;
				//	if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
				//		nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
				//	if( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 )
				//		nativeFlags |= BufferFlags.ComputeWrite;

				//	zzzzz;
				//	nativeObjectIsDynamicBuffer = true;

				//	var buffer = new DynamicVertexBuffer( vertexCount, vertexDeclaration, nativeFlags );
				//	nativeObject = buffer;
				//	nativeObjectHandle = buffer.handle;
				//	dynamic_needUpdateNative = true;
				//}

				//if( ( Flags & GpuBufferFlags.ComputeWrite ) == 0 )
				//{
				//	if( dynamic_needUpdateNative )
				//	{
				//		dynamic_needUpdateNative = false;

				//		var buffer = (DynamicVertexBuffer)nativeObject;

				//		fixed( byte* pVertices = vertices )
				//		{
				//			var memory = new MemoryBlock( pVertices, vertexCountActual * vertexSize );
				//			buffer.Update( 0, memory );
				//		}

				//		//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<byte>( vertices, 0, vertexCountActual * vertexSize ) );
				//		//buffer.Update( 0, memory );
				//	}
				//}
			}
			else
			{
				if( nativeObjectHandle == ushort.MaxValue )// if( nativeObject == null )
				{
					EngineThreading.CheckMainThread();

					var vertexDeclaration2 = vertexDeclaration;
					var vertices2 = vertices;

					//!!!!can convert normals, tangents to QTangent. but need to inform shader about format of the draw call
					////compress vertex data (static data only)
					//CompressVertices( ref vertexDeclaration2, ref vertices2, vertexCount );

					fixed( byte* pVertices = vertices2 )
					{
						var memory = new MemoryBlock( pVertices, vertices2.Length );

						BufferFlags nativeFlags = 0;
						if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
							nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;

						nativeObjectHandle = NativeMethods.bgfx_create_vertex_buffer( memory.ptr, ref vertexDeclaration2.data, nativeFlags );
						nativeObjectIsDynamicBuffer = false;

						//var buffer = new VertexBuffer( memory, vertexDeclaration2, nativeFlags );
						//nativeHandle = buffer.handle;
						//nativeObject = buffer;
					}

					//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( vertices );
					//realObject = new VertexBuffer( memory, vertexDeclaration, BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
				}
			}

			nativeObjectLastUsedTime = EngineApp.EngineTime;
			//return nativeObject;
		}

		public T[] ExtractChannel<T>( int startOffsetInBytes ) where T : unmanaged
		{
			T[] result = new T[ VertexCount ];
			unsafe
			{
				fixed( byte* pVertices = vertices )
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

		public float[] ExtractChannelSingle( int startOffsetInBytes ) { return ExtractChannel<float>( startOffsetInBytes ); }
		public Vector2F[] ExtractChannelVector2F( int startOffsetInBytes ) { return ExtractChannel<Vector2F>( startOffsetInBytes ); }
		public Vector3F[] ExtractChannelVector3F( int startOffsetInBytes ) { return ExtractChannel<Vector3F>( startOffsetInBytes ); }
		public Vector4F[] ExtractChannelVector4F( int startOffsetInBytes ) { return ExtractChannel<Vector4F>( startOffsetInBytes ); }
		public int[] ExtractChannelInteger( int startOffsetInBytes ) { return ExtractChannel<int>( startOffsetInBytes ); }
		public Vector2I[] ExtractChannelVector2I( int startOffsetInBytes ) { return ExtractChannel<Vector2I>( startOffsetInBytes ); }
		public Vector3I[] ExtractChannelVector3I( int startOffsetInBytes ) { return ExtractChannel<Vector3I>( startOffsetInBytes ); }
		public Vector4I[] ExtractChannelVector4I( int startOffsetInBytes ) { return ExtractChannel<Vector4I>( startOffsetInBytes ); }
		public ColorValue[] ExtractChannelColorValue( int startOffsetInBytes ) { return ExtractChannel<ColorValue>( startOffsetInBytes ); }
		public ColorByte[] ExtractChannelColorByte( int startOffsetInBytes ) { return ExtractChannel<ColorByte>( startOffsetInBytes ); }


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
				fixed( byte* pVertices = vertices )
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
			fixed( byte* pVertices = vertices )
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

		public unsafe void WriteChannelSingle( int startOffsetInBytes, float[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector2F( int startOffsetInBytes, Vector2F[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector3F( int startOffsetInBytes, Vector3F[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector4F( int startOffsetInBytes, Vector4F[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelInteger( int startOffsetInBytes, int[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector2I( int startOffsetInBytes, Vector2I[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector3I( int startOffsetInBytes, Vector3I[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelVector4I( int startOffsetInBytes, Vector4I[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelColorValue( int startOffsetInBytes, ColorValue[] data ) { WriteChannel( startOffsetInBytes, data ); }
		public unsafe void WriteChannelColorByte( int startOffsetInBytes, ColorByte[] data ) { WriteChannel( startOffsetInBytes, data ); }

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

		public void DestroyNativeObject()
		{
			if( nativeObjectHandle != ushort.MaxValue )
			{
				if( nativeObjectIsDynamicBuffer )
				{
					EngineThreading.ExecuteFromMainThreadLater( delegate ( ushort handle )
					{
						NativeMethods.bgfx_destroy_dynamic_vertex_buffer( handle );
					}, nativeObjectHandle );
				}
				else
				{
					EngineThreading.ExecuteFromMainThreadLater( delegate ( ushort handle )
					{
						NativeMethods.bgfx_destroy_vertex_buffer( handle );
					}, nativeObjectHandle );
				}

				nativeObjectHandle = ushort.MaxValue;
				nativeObjectIsDynamicBuffer = false;
			}

			//if( nativeObject != null )
			//{
			//	EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
			//	nativeObject = null;
			//	nativeHandle = 0;
			//}
		}

		public void DestroyNativeObjectNotUsedForLongTime( double howLongHasNotBeenUsedInSeconds )
		{
			if( nativeObjectHandle != ushort.MaxValue ) //if( nativeObject != null )
			{
				if( EngineApp.EngineTime - nativeObjectLastUsedTime > howLongHasNotBeenUsedInSeconds )
				{
					//if( GpuTexture.testRandom.NextDouble() > 0.5 )
					DestroyNativeObject();
				}
			}
		}

		//static void CompressVertices( ref VertexLayout layout, ref byte[] vertices, int vertexCount )
		//{
		//	var mode = RenderingSystem.CompressVertices;
		//	if( mode != ProjectSettingsPage_Rendering.CompressVerticesEnum.Original )
		//	{
		//		//!!!!impl

		//		////check need update
		//		//var needUpdate = false;
		//		//{
		//		//}

		//		//if( needUpdate )
		//		//{
		//		//	var newLayout = new VertexLayout();
		//		//	newLayout.Begin();

		//		//	//.Add( VertexAttributeUsage.Position, 3, VertexAttributeType.Float )
		//		//	//.Add( VertexAttributeUsage.Color0, 4, VertexAttributeType.Float )
		//		//	//.Add( VertexAttributeUsage.TexCoord0, 2, VertexAttributeType.Float )


		//		//	newLayout.Add( zzzz );

		//		//	newLayout.End();


		//		//	var newVertexSize = newLayout.Stride;
		//		//	var newVertices = new byte[ vertexCount * newVertexSize ];


		//		//	//!!!!

		//		//	//vertexDeclaration.HasAttribute

		//		//	//for( int nAttrib = 0; nAttrib < zzzzzz; nAttrib++ )
		//		//	//{

		//		//	//	//vertexDeclaration

		//		//	//}


		//		//	layout = newLayout;
		//		//	vertices = newVertices;
		//		//}
		//	}
		//}

	}
}
