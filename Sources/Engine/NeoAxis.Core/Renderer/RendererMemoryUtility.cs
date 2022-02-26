//// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using Internal.SharpBgfx;

//namespace NeoAxis
//{
//	internal static class RendererMemoryUtility
//	{
//		public unsafe static MemoryBlock AllocateAutoReleaseMemoryBlock<T>( ArraySegment<T> data ) where T : unmanaged
//		{
//			var size = data.Count * sizeof( T );

//			var buffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, size );
//			fixed ( T* p = data.Array )
//				NativeUtility.CopyMemory( buffer, (IntPtr)( p + data.Offset ), size );

//			var memory = MemoryBlock.MakeRef( buffer, size, buffer, ReleaseHandleCallback );
//			return memory;
//		}

//		public static MemoryBlock AllocateAutoReleaseMemoryBlock<T>( T[] data ) where T : unmanaged
//		{
//			return AllocateAutoReleaseMemoryBlock( new ArraySegment<T>( data ) );
//		}

//		public static MemoryBlock AllocateAutoReleaseMemoryBlock( IntPtr data, int size )
//		{
//			var buffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, size );
//			NativeUtility.CopyMemory( buffer, data, size );

//			var memory = MemoryBlock.MakeRef( buffer, size, buffer, ReleaseHandleCallback );
//			return memory;
//		}

//		static ReleaseCallback ReleaseHandleCallback = ReleaseHandle;
//		static void ReleaseHandle( IntPtr usedRef )
//		{
//			NativeUtility.Free( usedRef );
//		}
//	}
//}
