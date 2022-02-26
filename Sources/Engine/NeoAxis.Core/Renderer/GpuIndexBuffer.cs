// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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

		IDisposable nativeObject;
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

			if( Flags.HasFlag( GpuBufferFlags.Dynamic ) )
			{
				if( this.indices == null )
					this.indices = new int[ indexCount ];
				dynamic_needUpdateNative = true;
			}
		}

		protected override void OnDispose()
		{
			if( nativeObject != null )
			{
				//after shutdown check
				if( RenderingSystem.Disposed )
					Log.Fatal( "GpuIndexBuffer: Dispose after shutdown." );
				EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
				nativeObject = null;
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

		public IDisposable NativeObject
		{
			get { return nativeObject; }
		}

		public bool NativeObject16Bit
		{
			get { return nativeObject16Bit; }
		}

		public void SetData( int[] indices, int indexCountActual = -1 )
		{
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( nativeObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
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
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( nativeObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
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
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( nativeObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
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

		unsafe internal IDisposable GetNativeObject()
		{
			if( Disposed )
				Log.Fatal( "GpuIndexBuffer: GetNativeObject: Disposed." );

			EngineThreading.CheckMainThread();

			if( Flags.HasFlag( GpuBufferFlags.Dynamic ) || Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
			{
				////delete old static when changed to dynamic
				//if( realObject != null && realObject is SharpBgfx.IndexBuffer )
				//{
				//	realObject.Dispose();
				//	realObject = null;
				//}

				if( nativeObject == null )
				{
					var nativeFlags = Internal.SharpBgfx.BufferFlags.Index32 | Internal.SharpBgfx.BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
						nativeFlags |= Internal.SharpBgfx.BufferFlags.ComputeWrite;

					//dynamic buffers are always 32-bit
					nativeObject = new Internal.SharpBgfx.DynamicIndexBuffer( indices.Length, nativeFlags );
					dynamic_needUpdateNative = true;
					nativeObject16Bit = false;
				}

				if( !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				{
					if( dynamic_needUpdateNative )
					{
						dynamic_needUpdateNative = false;

						var buffer = (Internal.SharpBgfx.DynamicIndexBuffer)nativeObject;

						fixed( int* pIndices = indices )
						{
							var ptr = new Internal.SharpBgfx.MemoryBlock( pIndices, indexCountActual * 4 );
							buffer.Update( 0, ptr );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<int>( indices, 0, indexCountActual ) );
						//buffer.Update( 0, memory );
					}
				}
			}
			else
			{
				if( nativeObject == null )
				{
					bool use16Bit = true;
					foreach( var index in indices )
					{
						if( index > 65535 )
						{
							use16Bit = false;
							break;
						}
					}

					if( use16Bit )
					{
						var indices16 = new ushort[ indices.Length ];
						for( int n = 0; n < indices16.Length; n++ )
							indices16[ n ] = (ushort)indices[ n ];

						fixed( ushort* pIndices = indices16 )
						{
							var memory = new Internal.SharpBgfx.MemoryBlock( pIndices, indices.Length * 2 );
							nativeObject = new Internal.SharpBgfx.IndexBuffer( memory, Internal.SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices16 );
						//realObject = new Internal.SharpBgfx.IndexBuffer( memory, Internal.SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
					else
					{
						fixed( int* pIndices = indices )
						{
							var memory = new Internal.SharpBgfx.MemoryBlock( pIndices, indices.Length * 4 );
							nativeObject = new Internal.SharpBgfx.IndexBuffer( memory, Internal.SharpBgfx.BufferFlags.Index32 | Internal.SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
						}

						//var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices );
						//realObject = new Internal.SharpBgfx.IndexBuffer( memory, Internal.SharpBgfx.BufferFlags.Index32 | Internal.SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
					nativeObject16Bit = use16Bit;
				}
			}

			nativeObjectLastUsedTime = EngineApp.EngineTime;
			return nativeObject;
		}

		public void MakeCopyOfData()
		{
			if( indices != null )
				indices = (int[])indices.Clone();
		}

		public void DestroyNativeObject()
		{
			if( nativeObject != null )
			{
				EngineThreading.ExecuteFromMainThreadLater( delegate ( IDisposable obj ) { obj.Dispose(); }, nativeObject );
				nativeObject = null;
			}
		}

		public void DestroyNativeObjectNotUsedForLongTime( double howLongHasNotBeenUsedInSeconds )
		{
			if( nativeObject != null )
			{
				if( EngineApp.EngineTime - nativeObjectLastUsedTime > howLongHasNotBeenUsedInSeconds )
					DestroyNativeObject();
			}
		}
	}
}
