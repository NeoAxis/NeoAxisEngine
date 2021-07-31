// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A list that internally creates a native array.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OpenListNative<T> where T : unmanaged
	{
		public unsafe T* Data;
		int DataLength;
		public int Count;

		public unsafe OpenListNative( int capacity )
		{
			DataLength = capacity;
			Data = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, DataLength * sizeof( T ) );
		}

		public OpenListNative()
			: this( 4 )
		{
		}

		public unsafe void Dispose()
		{
			Clear();
			if( Data != null )
			{
				NativeUtility.Free( Data );
				Data = null;
				DataLength = 0;
			}
		}

		public unsafe void Add( ref T item )
		{
			if( Count == DataLength )
			{
				T* oldData = Data;
				int oldLength = DataLength;

				DataLength = oldLength != 0 ? oldLength * 2 : 4;
				Data = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, DataLength * sizeof( T ) );
				NativeUtility.CopyMemory( Data, oldData, Count * sizeof( T ) );

				NativeUtility.Free( oldData );
			}
			Data[ Count++ ] = item;
		}

		public void Add( T item )
		{
			Add( ref item );
		}

		public unsafe T[] ToArray()
		{
			var result = new T[ Count ];
			fixed ( T* pResult = result )
				NativeUtility.CopyMemory( pResult, Data, Count * sizeof( T ) );
			return result;
		}

		public void Clear()
		{
			Count = 0;
		}

		//!!!!AddRange
	}
}
