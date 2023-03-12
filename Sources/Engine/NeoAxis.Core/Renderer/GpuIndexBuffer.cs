// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Represents an index buffer.
	/// </summary>
	public class GpuIndexBuffer : ThreadSafeDisposable
	{
		int[] indices;
		int indexCount;
		int indexCountActual;
		GpuBufferFlags flags;

		ushort nativeObjectHandle = ushort.MaxValue;
		bool nativeObjectIsDynamicBuffer;
		//IDisposable nativeObject;

		bool dynamic_needUpdateNative;
		bool nativeObject16Bit;
		double nativeObjectLastUsedTime;

		//

		internal GpuIndexBuffer( int[] indices, int indexCount, GpuBufferFlags flags )
		{
			lock( GpuBufferManager.indexBuffers )
				GpuBufferManager.indexBuffers.Add( this );

			this.indices = indices;
			this.flags = flags;
			this.indexCount = indexCount;
			indexCountActual = indexCount;

			if( ( Flags & GpuBufferFlags.Dynamic ) != 0 )
			{
				if( this.indices == null )
					this.indices = new int[ indexCount ];
				dynamic_needUpdateNative = true;
			}
		}

		protected override void OnDispose()
		{
			if( nativeObjectHandle != ushort.MaxValue )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "GpuIndexBuffer: Dispose after shutdown." );

				DestroyNativeObject();
				//EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
				//nativeObject = null;
			}

			lock( GpuBufferManager.indexBuffers )
				GpuBufferManager.indexBuffers.Remove( this );

			//base.OnDispose();
		}

		public int[] Indices
		{
			get { return indices; }
		}

		public int IndexCount
		{
			get { return indexCount; }
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

		public bool NativeObject16Bit
		{
			get { return nativeObject16Bit; }
		}

		public void SetData( int[] indices, int indexCountActual = -1 )
		{
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
				Log.Fatal( "GpuIndexBuffer: SetData: The buffer must be dynamic or must be still not be created on GPU." );
			if( this.indices != null && indices.Length != this.indices.Length )
				Log.Fatal( "GpuIndexBuffer: SetData: indices.Length != this.indices.Length." );

			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			this.indices = indices;
			this.indexCountActual = indexCountActual != -1 ? indexCountActual : indexCount;
			dynamic_needUpdateNative = true;
		}

		public void Write( IntPtr indices, int indexCount )
		{
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
				Log.Fatal( "GpuIndexBuffer: Write: The buffer must be dynamic or must be still not be created on GPU." );
			if( indexCount > this.indexCount )
				Log.Fatal( "GpuIndexBuffer: Write: indexCount > this.indexCount." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			unsafe
			{
				fixed( int* pIndices = this.indices )
					NativeUtility.CopyMemory( (IntPtr)pIndices, indices, indexCount * sizeof( int ) );
			}
			indexCountActual = indexCount;
			dynamic_needUpdateNative = true;
		}

		public void Write( IntPtr indices )
		{
			Write( indices, this.indices.Length );
		}

		public int[] WriteBegin()
		{
			var isDynamic = ( Flags & GpuBufferFlags.Dynamic ) != 0;
			var isComputeWrite = ( Flags & GpuBufferFlags.ComputeWrite ) != 0;
			if( !( isDynamic || ( !NativeObjectCreated && !isDynamic && !isComputeWrite ) ) )
				Log.Fatal( "GpuIndexBuffer: WriteBegin: The buffer must be dynamic or must be still not be created on GPU." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			return indices;
		}

		public void WriteEnd()
		{
			dynamic_needUpdateNative = true;
			indexCountActual = indexCount;
		}

		unsafe internal void UpdateNativeObject()
		//unsafe internal IDisposable GetNativeObject()
		{
			if( Disposed )
				Log.Fatal( "GpuIndexBuffer: GetNativeObject: Disposed." );

			EngineThreading.CheckMainThread();

			if( ( Flags & ( GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeWrite ) ) != 0 )
			//if( ( ( Flags & GpuBufferFlags.Dynamic ) != 0 ) || ( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 ) )
			{
				if( nativeObjectHandle == ushort.MaxValue )
				{
					BufferFlags nativeFlags = 0;
					if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
						nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 )
						nativeFlags |= BufferFlags.ComputeWrite;
					//dynamic buffers are always 32-bit
					nativeFlags |= BufferFlags.Index32;

					nativeObjectHandle = NativeMethods.bgfx_create_dynamic_index_buffer( indices.Length, nativeFlags );
					nativeObjectIsDynamicBuffer = true;

					//nativeObject = new DynamicIndexBuffer( indices.Length, nativeFlags );
					dynamic_needUpdateNative = true;
					nativeObject16Bit = false;
				}

				if( ( Flags & GpuBufferFlags.ComputeWrite ) == 0 )
				{
					if( dynamic_needUpdateNative )
					{
						dynamic_needUpdateNative = false;

						//var buffer = (DynamicIndexBuffer)nativeObject;

						fixed( int* pIndices = indices )
						{
							var memory = new MemoryBlock( pIndices, indexCountActual * 4 );
							NativeMethods.bgfx_update_dynamic_index_buffer( nativeObjectHandle, 0, memory.ptr );
							//buffer.Update( 0, memory );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<int>( indices, 0, indexCountActual ) );
						//buffer.Update( 0, memory );
					}
				}

				//if( nativeObject == null )
				//{
				//	BufferFlags nativeFlags = 0;
				//	if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
				//		nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
				//	if( ( Flags & GpuBufferFlags.ComputeWrite ) != 0 )
				//		nativeFlags |= BufferFlags.ComputeWrite;
				//	//dynamic buffers are always 32-bit
				//	nativeFlags |= BufferFlags.Index32;

				//	nativeObject = new DynamicIndexBuffer( indices.Length, nativeFlags );
				//	dynamic_needUpdateNative = true;
				//	nativeObject16Bit = false;
				//}

				//if( ( Flags & GpuBufferFlags.ComputeWrite ) == 0 )
				//{
				//	if( dynamic_needUpdateNative )
				//	{
				//		dynamic_needUpdateNative = false;

				//		var buffer = (DynamicIndexBuffer)nativeObject;

				//		fixed( int* pIndices = indices )
				//		{
				//			var ptr = new MemoryBlock( pIndices, indexCountActual * 4 );
				//			buffer.Update( 0, ptr );
				//		}

				//		//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<int>( indices, 0, indexCountActual ) );
				//		//buffer.Update( 0, memory );
				//	}
				//}
			}
			else
			{
				if( nativeObjectHandle == ushort.MaxValue )// if( nativeObject == null )
				{
					bool use16Bit = true;
					if( indices.Length > 65000 )//fast skip
						use16Bit = false;
					else
					{
						foreach( var index in indices )
						{
							if( index > 65535 )
							{
								use16Bit = false;
								break;
							}
						}
					}

					BufferFlags nativeFlags = 0;
					if( ( Flags & GpuBufferFlags.ComputeRead ) != 0 )
						nativeFlags |= BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( !use16Bit )
						nativeFlags |= BufferFlags.Index32;

					if( use16Bit )
					{
						var indices16 = new ushort[ indices.Length ];
						for( int n = 0; n < indices16.Length; n++ )
							indices16[ n ] = (ushort)indices[ n ];

						fixed( ushort* pIndices = indices16 )
						{
							//!!!!можно заливать сразу в memory. public MemoryBlock (int size). где еще так

							var memory = new MemoryBlock( pIndices, indices.Length * 2 );

							nativeObjectHandle = NativeMethods.bgfx_create_index_buffer( memory.ptr, nativeFlags );
							nativeObjectIsDynamicBuffer = false;
							//nativeObject = new IndexBuffer( memory, nativeFlags );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices16 );
						//realObject = new IndexBuffer( memory, BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
					else
					{
						fixed( int* pIndices = indices )
						{
							var memory = new MemoryBlock( pIndices, indices.Length * 4 );

							nativeObjectHandle = NativeMethods.bgfx_create_index_buffer( memory.ptr, nativeFlags );
							nativeObjectIsDynamicBuffer = false;
							//nativeObject = new IndexBuffer( memory, nativeFlags );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices );
						//realObject = new IndexBuffer( memory, BufferFlags.Index32 | BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
					nativeObject16Bit = use16Bit;
				}
			}

			nativeObjectLastUsedTime = EngineApp.EngineTime;
			//return nativeObject;
		}

		public void MakeCopyOfData()
		{
			if( indices != null )
				indices = (int[])indices.Clone();
		}

		public void DestroyNativeObject()
		{
			if( nativeObjectHandle != ushort.MaxValue )
			{
				if( nativeObjectIsDynamicBuffer )
				{
					EngineThreading.ExecuteFromMainThreadLater( delegate ( ushort handle )
					{
						NativeMethods.bgfx_destroy_dynamic_index_buffer( handle );
					}, nativeObjectHandle );
				}
				else
				{
					EngineThreading.ExecuteFromMainThreadLater( delegate ( ushort handle )
					{
						NativeMethods.bgfx_destroy_index_buffer( handle );
					}, nativeObjectHandle );
				}

				nativeObjectHandle = ushort.MaxValue;
				nativeObjectIsDynamicBuffer = false;
				nativeObject16Bit = false;
			}

			//if( nativeObject != null )
			//{
			//	EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
			//	nativeObject = null;
			//}
		}

		public void DestroyNativeObjectNotUsedForLongTime( double howLongHasNotBeenUsedInSeconds )
		{
			if( nativeObjectHandle != ushort.MaxValue ) //if( nativeObject != null )
			{
				if( EngineApp.EngineTime - nativeObjectLastUsedTime > howLongHasNotBeenUsedInSeconds )
					DestroyNativeObject();
			}
		}
	}
}
