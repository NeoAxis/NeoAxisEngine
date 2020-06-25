// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		IDisposable realObject;
		bool dynamic_needUpdateNative;

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

		public void SetData( int[] indices, int indexCountActual = -1 )
		{
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
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
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
				Log.Fatal( "GpuIndexBuffer: Write: The buffer must be dynamic or must be still not be created on GPU." );
			if( indexCount > this.indexCount )
				Log.Fatal( "GpuIndexBuffer: Write: indexCount > this.indexCount." );
			////change type to dynamic
			//if( !dynamic && realObject != null )
			//	dynamic = true;

			unsafe
			{
				fixed ( int* pIndices = this.indices )
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
			if( !( Flags.HasFlag( GpuBufferFlags.Dynamic ) || ( realObject == null && !Flags.HasFlag( GpuBufferFlags.Dynamic ) && !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) ) ) )
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

		internal IDisposable GetNativeObject()
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

				if( realObject == null )
				{
					var nativeFlags = SharpBgfx.BufferFlags.Index32 | SharpBgfx.BufferFlags.ComputeRead;// | SharpBgfx.BufferFlags.ComputeTypeFloat;
					if( Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
						nativeFlags |= SharpBgfx.BufferFlags.ComputeWrite;

					//dynamic buffers are always 32-bit
					realObject = new SharpBgfx.DynamicIndexBuffer( indices.Length, nativeFlags );
					dynamic_needUpdateNative = true;
				}

				if( !Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
				{
					if( dynamic_needUpdateNative )
					{
						dynamic_needUpdateNative = false;

						var buffer = (SharpBgfx.DynamicIndexBuffer)realObject;
						var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( new ArraySegment<int>( indices, 0, indexCountActual ) );
						buffer.Update( 0, memory );
					}
				}
			}
			else
			{
				if( realObject == null )
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

						var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices16 );
						realObject = new SharpBgfx.IndexBuffer( memory, SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
					else
					{
						var memory = RendererMemoryUtility.AllocateAutoReleaseMemoryBlock( indices );
						realObject = new SharpBgfx.IndexBuffer( memory, SharpBgfx.BufferFlags.Index32 | SharpBgfx.BufferFlags.ComputeRead/* | SharpBgfx.BufferFlags.ComputeTypeFloat*/ );
					}
				}
			}

			return realObject;
		}

		public void MakeCopyOfData()
		{
			if( indices != null )
				indices = (int[])indices.Clone();
		}
	}
}
