// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBgfx;

namespace NeoAxis
{
	internal static class RendererMemoryUtility
	{
		public unsafe static MemoryBlock AllocateAutoReleaseMemoryBlock<T>( ArraySegment<T> data ) where T : unmanaged
		{
			var size = data.Count * sizeof( T );

			var buffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, size );
			fixed ( T* p = data.Array )
				NativeUtility.CopyMemory( buffer, (IntPtr)( p + data.Offset ), size );

			var memory = MemoryBlock.MakeRef( buffer, size, buffer, ReleaseHandleCallback );
			return memory;
		}

		public static MemoryBlock AllocateAutoReleaseMemoryBlock<T>( T[] data ) where T : unmanaged
		{
			return AllocateAutoReleaseMemoryBlock( new ArraySegment<T>( data ) );
		}

		public static MemoryBlock AllocateAutoReleaseMemoryBlock( IntPtr data, int size )
		{
			var buffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, size );
			NativeUtility.CopyMemory( buffer, data, size );

			var memory = MemoryBlock.MakeRef( buffer, size, buffer, ReleaseHandleCallback );
			return memory;
		}

		static ReleaseCallback ReleaseHandleCallback = ReleaseHandle;
		static void ReleaseHandle( IntPtr usedRef )
		{
			NativeUtility.Free( usedRef );
		}
	}

	//internal static class RefsManager
	//{
	//	static Dictionary<IntPtr, UnmanagedCollection<byte>> _usedRefs = new Dictionary<IntPtr, UnmanagedCollection<byte>>();
	//	static HashSet<IntPtr> _markedForDisposing = new HashSet<IntPtr>();

	//	public static MemoryBlock MakeRef( UnmanagedCollection<byte> data )
	//	{
	//		var memory = MemoryBlock.MakeRef( data.Data, data.DataSizeInBytes, data.Data, ReleaseHandleCallback );
	//		_usedRefs.Add( data.Data, data );
	//		return memory;
	//	}

	//	public static void DisposeData( UnmanagedCollection<byte> data )
	//	{
	//		if( !IsUsed( data ) )
	//		{
	//			data.Dispose();
	//			return;
	//		}

	//		_markedForDisposing.Add( data.Data );
	//	}

	//	public static bool IsUsed( UnmanagedCollection<byte> data )
	//	{
	//		return _usedRefs.ContainsKey( data.Data );
	//	}

	//	static ReleaseCallback ReleaseHandleCallback = ReleaseHandle;
	//	static void ReleaseHandle( IntPtr usedRef )
	//	{
	//		if( _markedForDisposing.Contains( usedRef ) )
	//		{
	//			_usedRefs[ usedRef ].Dispose();
	//			_markedForDisposing.Remove( usedRef );
	//		}

	//		_usedRefs.Remove( usedRef );
	//	}
	//}
}
