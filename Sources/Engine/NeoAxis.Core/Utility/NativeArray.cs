//// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Threading;

//namespace NeoAxis
//{
//!!!! is not tested
//	public class NativeArray<T> : IEnumerable<T>, IEnumerable, IDisposable where T : unmanaged //ICollection<T>
//	{
//		IntPtr buffer;
//		int length;

//		/////////////////////////////////////////

//		public struct Enumerator : IEnumerator<T>
//		{
//			NativeArray<T> array;
//			int index;

//			/// <summary>
//			/// The current enumerated item.
//			/// </summary>
//			public T Current
//			{
//				get { return array[ index ]; }
//			}

//			object IEnumerator.Current
//			{
//				get { return Current; }
//			}

//			internal Enumerator( NativeArray<T> array )
//			{
//				this.array = array;
//				index = -1;
//			}

//			/// <summary>
//			/// Advances to the next item in the sequence.
//			/// </summary>
//			/// <returns><c>true</c> if there are more items in the collection; otherwise, <c>false</c>.</returns>
//			public bool MoveNext()
//			{
//				var newIndex = index + 1;
//				if( newIndex >= array.Length )
//					return false;

//				index = newIndex;
//				return true;
//			}

//			/// <summary>
//			/// Empty; does nothing.
//			/// </summary>
//			public void Dispose()
//			{
//			}

//			/// <summary>
//			/// Not implemented.
//			/// </summary>
//			public void Reset()
//			{
//			}
//		}

//		/////////////////////////////////////////

//		public unsafe NativeArray( IntPtr data, int length, NativeUtils.MemoryAllocationType allocationType = NativeUtils.MemoryAllocationType.Utils )
//		{
//			this.buffer = NativeUtils.Alloc( allocationType, length * sizeof( T ) );
//			this.length = length;
//			NativeUtils.CopyMemory( this.buffer, data, length * sizeof( T ) );
//		}

//		public unsafe NativeArray( T[] data, NativeUtils.MemoryAllocationType allocationType = NativeUtils.MemoryAllocationType.Utils )
//		{
//			buffer = NativeUtils.Alloc( allocationType, data.Length * sizeof( T ) );
//			length = data.Length;
//			fixed ( T* p = data )
//				NativeUtils.CopyMemory( buffer, (IntPtr)p, data.Length * sizeof( T ) );
//		}

//		public void Dispose()
//		{
//			var buffer2 = Interlocked.Exchange( ref buffer, IntPtr.Zero );
//			if( buffer2 != IntPtr.Zero )
//				NativeUtils.Free( buffer2 );
//			length = 0;
//		}

//		public IntPtr Buffer
//		{
//			get { return buffer; }
//		}

//		public int Length
//		{
//			get { return length; }
//		}

//		public unsafe int ItemSizeInBytes
//		{
//			get { return sizeof( T ); }
//		}

//		public unsafe int BufferSizeInBytes
//		{
//			get { return length * sizeof( T ); }
//		}

//		public unsafe void CopyTo( T[] array, int startIndex = 0 )
//		{
//			fixed ( T* p = array )
//				NativeUtils.CopyMemory( (IntPtr)( p + startIndex ), buffer, length * sizeof( T ) );
//		}

//		public unsafe void CopyFrom( T[] array, int startIndex = 0 )
//		{
//			fixed ( T* p = array )
//				NativeUtils.CopyMemory( buffer, (IntPtr)( p + startIndex ), length * sizeof( T ) );
//		}

//		public unsafe void CopyFrom( IntPtr data )
//		{
//			NativeUtils.CopyMemory( buffer, data, length * sizeof( T ) );
//		}

//		public unsafe T[] ToArray()
//		{
//			var result = new T[ length ];
//			fixed ( T* p = result )
//				NativeUtils.CopyMemory( (IntPtr)p, buffer, length * sizeof( T ) );
//			return result;
//		}

//		public unsafe T this[ int index ]
//		{
//			get { return *( (T*)buffer + index ); }
//			set { *( (T*)buffer + index ) = value; }
//		}

//		public IEnumerator<T> GetEnumerator()
//		{
//			return new Enumerator( this );
//		}

//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return GetEnumerator();
//		}
//	}
//}

//	// ICollection implementation in unmanaged heap, useful for buffers you have to pass the pointer to native 
//	// (e.g. gpu buffer objects for pinvoked libs). 
//	// NOTE: Requires C# >=7.3. 
//	// PERF: Insertion faster or equal List<T>, everything else faster
//	//
//	// https://github.com/Alan-FGR/UnmanagedCollection
//	// https://github.com/MikePopoloski/SharpBgfx/issues/22
//	//

//	// TODO: inherit from System.Buffers.MemoryManager<T> ?
//	// see https://github.com/dotnet/corefx/blob/master/src/Common/tests/System/Buffers/NativeMemoryManager.cs
//	// https://github.com/aalmada/SpanSample/blob/master/SpanSample/NativeMemoryManager.cs
//	// https://medium.com/@antao.almada/p-invoking-using-span-t-a398b86f95d3

//	public unsafe class UnmanagedCollection<T> : ICollection<T>,/* IMemoryOwner<T>,*/ IDisposable where T : unmanaged
//	{
//		public bool IsReadOnly => false;
//		public IntPtr Data => (IntPtr)data;
//		public int DataSizeInBytes => elementsCount * elementSize;
//		public int Count { get; private set; } = 0;

//		// for Span<T> and Memory<T> support add nuget "System.Memory"
//		/*
//		public Span<T> GetSpan() => new Span<T>( data, elementsCount );
//		public Span<T> GetSpan( int start ) => GetSpan().Slice( start );
//		public Span<T> GetSpan( int start, int length ) => GetSpan().Slice( start, length );

//		// [MethodImpl( MethodImplOptions.AggressiveInlining )]
//		public Memory<T> Memory => new Memory<T>( this, 0, GetSpan().Length );
//		*/

//		public bool IsDisposed
//		{
//			get { lock( this ) return disposed; }
//		}

//		private int elementsCount;
//		private T* data;
//		private bool disposed;

//		private readonly float overflowMult;
//		private readonly int elementSize;


//		public UnmanagedCollection( int count )
//		{
//			float overflowMult = 1.5f;

//			if( (int)( count * overflowMult ) < count + 1 )
//				throw new ArgumentOutOfRangeException( "Overflow multiplier doesn't increase size" );

//			this.overflowMult = overflowMult;
//			elementSize = Marshal.SizeOf<T>();
//			elementsCount = count;
//			data = (T*)Marshal.AllocHGlobal( DataSizeInBytes );
//			GC.AddMemoryPressure( DataSizeInBytes );
//		}

//		~UnmanagedCollection()
//		{
//			Dispose( false );
//		}

//		public void Add( T item )
//		{
//			if( Count + 1 > elementsCount )
//				GrowMemoryBlock( GetNextBlockSize() );

//			data[ Count ] = item;
//			Count++;
//		}

//		public void AddRange( UnmanagedCollection<T> collection )
//		{
//			AssureSize( Count + collection.Count );

//			for( int i = 0; i < collection.Count; i++ )
//				data[ Count + i ] = collection.data[ i ];

//			Count += collection.Count;
//		}

//		public void AddRange( IList<T> collection )
//		{
//			AssureSize( Count + collection.Count );

//			for( int i = 0; i < collection.Count; i++ )
//				data[ Count + i ] = collection[ i ];

//			Count += collection.Count;
//		}

//		private void AssureSize( int size )
//		{
//			if( size > elementsCount )
//			{
//				var nextAccomodatingSize = GetNextBlockSize();
//				while( nextAccomodatingSize < size )
//					nextAccomodatingSize = GetNextBlockSize();
//				GrowMemoryBlock( nextAccomodatingSize );
//			}
//		}

//		private int GetNextBlockSize()
//		{
//			return (int)( elementsCount * overflowMult );
//		}

//		private void GrowMemoryBlock( int newElementCount )
//		{
//			var newDataSize = elementSize * newElementCount;
//			var newData = (T*)Marshal.AllocHGlobal( newDataSize );
//			Buffer.MemoryCopy( data, newData, DataSizeInBytes, DataSizeInBytes );
//			Marshal.FreeHGlobal( (IntPtr)data );
//			elementsCount = newElementCount;
//			data = newData;
//		}

//		public void Clear()
//		{
//			Count = 0;
//		}

//		public T GetUnsafe( int index )
//		{
//			return data[ index ];
//		}

//		public IEnumerator<T> GetEnumerator()
//		{
//			for( int i = 0; i < Count; i++ )
//				yield return GetUnsafe( i );
//		}

//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return GetEnumerator();
//		}

//		public bool Contains( T item )
//		{
//			for( int i = 0; i < Count; i++ )
//				if( data[ i ].Equals( item ) )
//					return true;
//			return false;
//		}

//		public void CopyTo( T[] array, int arrayIndex )
//		{
//			if( arrayIndex + Count > array.Length )
//				throw new IndexOutOfRangeException( "Array to copy to doesn't have enough space" );

//			for( int i = 0; i < Count; i++ )
//				array[ arrayIndex + i ] = data[ i ];
//		}

//		public void CopyTo( IntPtr memAddr )
//		{
//			// I also like to live dangerously *trollface*
//			Buffer.MemoryCopy( data, (void*)memAddr, DataSizeInBytes, DataSizeInBytes );
//		}

//		public bool Remove( T item )
//		{
//			throw new NotImplementedException( "The memory block doesn't currently shrink" );
//		}

//		public void Dispose()
//		{
//			Dispose( true );
//			GC.SuppressFinalize( this );
//		}

//		protected virtual void Dispose( bool disposing )
//		{
//			lock( this )
//			{
//				if( !disposed )
//				{
//					disposed = true;
//					Marshal.FreeHGlobal( (IntPtr)data );
//					GC.RemoveMemoryPressure( DataSizeInBytes );
//				}
//			}
//		}
//	}
//}