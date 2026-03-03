//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;

//namespace NeoAxis
//{
//	/// <summary>
//	/// A list that internally creates a native array.
//	/// </summary>
//	/// <typeparam name="T"></typeparam>
//	public struct OpenListNativeStruct<T> : IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged
//	{
//		public unsafe T* Data;
//		int Capacity;
//		public int Count;

//		/////////////////////////////////////////

//		/// <summary>
//		/// Represents an enumerator for <see cref="OpenListNativeStruct{T}"/>.
//		/// </summary>
//		public class Enumerator : IEnumerator<T>, IEnumerator
//		{
//			OpenListNativeStruct<T> list;
//			int index;
//			//int version;
//			T current;

//			//

//			public Enumerator( OpenListNativeStruct<T> list )
//			{
//				this.list = list;
//				index = 0;
//				//version = list.version;
//				this.current = default( T );
//			}

//			public T Current
//			{
//				get { return current; }
//			}

//			object IEnumerator.Current
//			{
//				get
//				{
//					if( index == 0 || index == list.Count + 1 )
//						throw new InvalidOperationException( "Enumerate operation cannot happen" );
//					return this.current;
//				}
//			}

//			T IEnumerator<T>.Current
//			{
//				get
//				{
//					if( index == 0 || index == list.Count + 1 )
//						throw new InvalidOperationException( "Enumerate operation cannot happen" );
//					return this.current;
//				}
//			}

//			public void Dispose()
//			{
//				GC.SuppressFinalize( this );
//			}

//			public bool MoveNext()
//			{
//				var localList = list;

//				if(/* version == localList._version && */( (uint)index < (uint)localList.Count ) )
//				{
//					current = localList[ index ];
//					index++;
//					return true;
//				}
//				else
//				{
//					//if( version != list._version )
//					//{
//					//	ThrowHelper.ThrowInvalidOperationException( ExceptionResource.InvalidOperation_EnumFailedVersion );
//					//}

//					index = list.Count + 1;
//					current = default( T );
//					return false;
//				}
//			}

//			public void Reset()
//			{
//				this.index = 0;
//				this.current = default( T );
//			}
//		}

//		/////////////////////////////////////////

//		public OpenListNativeStruct( int capacity )
//		{
//			unsafe
//			{
//				Capacity = capacity;
//				Data = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, Capacity * sizeof( T ) );
//				Count = 0;
//			}
//		}

//		//public OpenListNativeStruct()
//		//	: this( 4 )
//		//{
//		//}

//		public void Dispose()
//		{
//			unsafe
//			{
//				Clear();
//				if( Data != null )
//				{
//					NativeUtility.Free( Data );
//					Data = null;
//					Capacity = 0;
//				}
//			}
//		}

//		public void Add( ref T item )
//		{
//			unsafe
//			{
//				if( Count == Capacity )
//				{
//					T* oldData = Data;
//					int oldLength = Capacity;

//					Capacity = oldLength != 0 ? oldLength * 2 : 4;
//					Data = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, Capacity * sizeof( T ) );
//					NativeUtility.CopyMemory( Data, oldData, Count * sizeof( T ) );

//					NativeUtility.Free( oldData );
//				}
//				Data[ Count++ ] = item;
//			}
//		}

//		public void Add( T item )
//		{
//			Add( ref item );
//		}

//		public T[] ToArray()
//		{
//			unsafe
//			{
//				var result = new T[ Count ];
//				fixed( T* pResult = result )
//					NativeUtility.CopyMemory( pResult, Data, Count * sizeof( T ) );
//				return result;
//			}
//		}

//		public void Clear()
//		{
//			Count = 0;
//		}

//		public T this[ int index ]
//		{
//			get
//			{
//				unsafe
//				{
//					return Data[ index ];
//				}
//			}
//			set
//			{
//				unsafe
//				{
//					Data[ index ] = value;
//				}
//			}
//		}

//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return new Enumerator( this );
//		}

//		IEnumerator<T> IEnumerable<T>.GetEnumerator()
//		{
//			return new Enumerator( this );
//		}

//		public OpenListNativeStruct<T> Clone()
//		{
//			unsafe
//			{
//				var result = new OpenListNativeStruct<T>( Count );
//				for( int n = 0; n < Count; n++ )
//					result.Add( ref Data[ n ] );
//				return result;
//			}
//		}

//		//!!!!AddRange
//	}
//}
