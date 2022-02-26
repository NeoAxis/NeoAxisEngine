// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with collections.
	/// </summary>
	public static class CollectionUtility
	{
		class ComparisonComparer<T> : IComparer<T>
		{
			readonly Comparison<T> comparison;

			public ComparisonComparer( Comparison<T> comparison )
			{
				this.comparison = comparison;
			}

			public int Compare( T x, T y )
			{
				return comparison( x, y );
			}
		}

		static void MergeSort<T>( T[] array, int low, int high, IComparer<T> comparer )
		{
			int n = high - low;
			if( n <= 1 )
				return;

			int mid = low + n / 2;

			MergeSort( array, low, mid, comparer );
			MergeSort( array, mid, high, comparer );

			T[] aux = new T[ n ];
			int i = low, j = mid;
			for( int k = 0; k < n; k++ )
			{
				if( i == mid ) aux[ k ] = array[ j++ ];
				else if( j == high ) aux[ k ] = array[ i++ ];
				else if( comparer.Compare( array[ j ], array[ i ] ) < 0 ) aux[ k ] = array[ j++ ];
				else aux[ k ] = array[ i++ ];
			}

			for( int k = 0; k < n; k++ )
				array[ low + k ] = aux[ k ];
		}

		unsafe static void MergeSort( int* array, int low, int high, IComparer<int> comparer )
		{
			int n = high - low;
			if( n <= 1 )
				return;

			int mid = low + n / 2;

			MergeSort( array, low, mid, comparer );
			MergeSort( array, mid, high, comparer );

			//int[] aux = new int[ n ];

			//!!!!тоже крешится
			//var aux = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, n * 4 );

			var useStack = n < 4096;
			int* aux = stackalloc int[ useStack ? n : 0 ];
			if( !useStack )
				aux = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, n * 4 );

			//int[] aux = new int[ n ];

			int i = low, j = mid;
			for( int k = 0; k < n; k++ )
			{
				if( i == mid ) aux[ k ] = array[ j++ ];
				else if( j == high ) aux[ k ] = array[ i++ ];
				else if( comparer.Compare( array[ j ], array[ i ] ) < 0 ) aux[ k ] = array[ j++ ];
				else aux[ k ] = array[ i++ ];
			}

			for( int k = 0; k < n; k++ )
				array[ low + k ] = aux[ k ];

			//NativeUtility.Free( aux );

			if( !useStack )
				NativeUtility.Free( aux );
		}

		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public static void MergeSort<T>( T[] array, IComparer<T> comparer, bool multithreaded = false )
		{
			if( multithreaded )
				Sto.StoParallelMergeSortExtension.ParallelMergeSort( array, comparer );
			else
				MergeSort( array, 0, array.Length, comparer );
		}

		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public static void MergeSort<T>( T[] array, Comparison<T> comparer, bool multithreaded = false )
		{
			if( multithreaded )
				Sto.StoParallelMergeSortExtension.ParallelMergeSort( array, comparer );
			else
				MergeSort( array, new ComparisonComparer<T>( comparer ), multithreaded );
		}

		//!!!!threading
		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public unsafe static void MergeSort( int* array, int count, IComparer<int> comparer )//, bool multithreaded = false )
		{
			//!!!!impl
			//if( multithreaded )
			//	Sto.StoParallelMergeSortExtension.ParallelMergeSort( array, comparer );
			//else
			MergeSort( array, 0, count, comparer );
		}

		//!!!!threading
		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public unsafe static void MergeSort( int* array, int count, Comparison<int> comparer )//, bool multithreaded = false )
		{
			//if( multithreaded )
			//	Sto.StoParallelMergeSortExtension.ParallelMergeSort( array, comparer );
			//else
			MergeSort( array, count, new ComparisonComparer<int>( comparer ) );//, multithreaded );
		}

		/// <summary>
		/// Insertion sort is a simple sorting algorithm that works the way we sort playing cards in our hands. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void InsertionSort<T>( T[] array, IComparer<T> comparer )
		{
			int first = 0;
			int last = array.Length - 1;

			for( var i = first + 1; i <= last; i++ )
			{
				var entry = array[ i ];
				var j = i;

				while( j > first && comparer.Compare( array[ j - 1 ], entry ) > 0 )
					array[ j ] = array[ --j ];

				array[ j ] = entry;
			}
		}

		/// <summary>
		/// Insertion sort is a simple sorting algorithm that works the way we sort playing cards in our hands. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void InsertionSort<T>( T[] array, Comparison<T> comparer )
		{
			InsertionSort( array, new ComparisonComparer<T>( comparer ) );
		}

		/// <summary>
		/// The selection sort algorithm sorts an array by repeatedly finding the minimum element. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void SelectionSort<T>( T[] array, IComparer<T> comparer )
		{
			int length = array.Length;

			for( int n = 0; n < length - 1; n++ )
			{
				T v1 = array[ n ];

				T smallestValue = v1;
				int smallestIndex = n;

				for( int z = n + 1; z < length; z++ )
				{
					T v2 = array[ z ];

					if( comparer.Compare( smallestValue, v2 ) > 0 )
					{
						smallestValue = v2;
						smallestIndex = z;
					}
				}

				if( smallestIndex != n )
				{
					array[ n ] = smallestValue;
					array[ smallestIndex ] = v1;
				}
			}
		}

		/// <summary>
		/// The selection sort algorithm sorts an array by repeatedly finding the minimum element. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="comparer"></param>
		public static void SelectionSort<T>( T[] array, Comparison<T> comparer )
		{
			SelectionSort( array, new ComparisonComparer<T>( comparer ) );
		}

		/// <summary>
		/// Merges two arrays into one.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array1"></param>
		/// <param name="array2"></param>
		/// <returns></returns>
		public static T[] Merge<T>( T[] array1, T[] array2 )
		{
			T[] newArray = new T[ array1.Length + array2.Length ];
			Array.Copy( array1, newArray, array1.Length );
			Array.Copy( array2, 0, newArray, array1.Length, array2.Length );
			return newArray;
		}

		/// <summary>
		/// Merges a collection of arrays into one.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arrays"></param>
		/// <returns></returns>
		public static T[] Merge<T>( ICollection<T[]> arrays )
		{
			int total = 0;
			foreach( T[] a in arrays )
				total += a.Length;

			T[] result = new T[ total ];

			int pos = 0;
			foreach( T[] a in arrays )
			{
				Array.Copy( a, 0, result, pos, a.Length );
				pos += a.Length;
			}

			return result;
		}

		static void MergeSort<T>( IList<T> list, int low, int high, IComparer<T> comparer )
		{
			int n = high - low;
			if( n <= 1 )
				return;

			int mid = low + n / 2;

			MergeSort( list, low, mid, comparer );
			MergeSort( list, mid, high, comparer );

			T[] aux = new T[ n ];
			int i = low, j = mid;
			for( int k = 0; k < n; k++ )
			{
				if( i == mid ) aux[ k ] = list[ j++ ];
				else if( j == high ) aux[ k ] = list[ i++ ];
				else if( comparer.Compare( list[ j ], list[ i ] ) < 0 ) aux[ k ] = list[ j++ ];
				else aux[ k ] = list[ i++ ];
			}

			for( int k = 0; k < n; k++ )
				list[ low + k ] = aux[ k ];
		}

		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public static void MergeSort<T>( IList<T> list, IComparer<T> comparer, bool multithreaded = false )
		{
			if( multithreaded )
				Sto.StoParallelMergeSortExtension.ParallelMergeSort( list, comparer );
			else
				MergeSort( list, 0, list.Count, comparer );
		}

		/// <summary>
		/// Merge sort is a efficient sorting method. It divides input array in two halves, calls itself for the two halves and then merges the two sorted halves. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		/// <param name="multithreaded"></param>
		public static void MergeSort<T>( IList<T> list, Comparison<T> comparer, bool multithreaded = false )
		{
			MergeSort( list, new ComparisonComparer<T>( comparer ), multithreaded );
		}

		/// <summary>
		/// Insertion sort is a simple sorting algorithm that works the way we sort playing cards in our hands. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void InsertionSort<T>( IList<T> list, IComparer<T> comparer )
		{
			int first = 0;
			int last = list.Count - 1;

			for( var i = first + 1; i <= last; i++ )
			{
				var entry = list[ i ];
				var j = i;

				while( j > first && comparer.Compare( list[ j - 1 ], entry ) > 0 )
					list[ j ] = list[ --j ];

				list[ j ] = entry;
			}
		}

		/// <summary>
		/// Insertion sort is a simple sorting algorithm that works the way we sort playing cards in our hands. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void InsertionSort<T>( IList<T> list, Comparison<T> comparer )
		{
			InsertionSort<T>( list, new ComparisonComparer<T>( comparer ) );
		}

		/// <summary>
		/// The selection sort algorithm sorts an array by repeatedly finding the minimum element. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void SelectionSort<T>( IList<T> list, IComparer<T> comparer )
		{
			int count = list.Count;

			for( int n = 0; n < count - 1; n++ )
			{
				T v1 = list[ n ];

				T smallestValue = v1;
				int smallestIndex = n;

				for( int z = n + 1; z < count; z++ )
				{
					T v2 = list[ z ];

					if( comparer.Compare( smallestValue, v2 ) > 0 )
					{
						smallestValue = v2;
						smallestIndex = z;
					}
				}

				if( smallestIndex != n )
				{
					list[ n ] = smallestValue;
					list[ smallestIndex ] = v1;
				}
			}
		}

		/// <summary>
		/// The selection sort algorithm sorts an array by repeatedly finding the minimum element. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void SelectionSort<T>( IList<T> list, Comparison<T> comparer )
		{
			SelectionSort<T>( list, new ComparisonComparer<T>( comparer ) );
		}

		/// <summary>
		/// The selection sort algorithm sorts an array by repeatedly finding the minimum element. No memory allocation. Stable sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void SelectionSort<T>( IList<T> list )
		{
			if( list.Count == 0 )
				return;

			IComparable<T> comparerTest = list[ 0 ] as IComparable<T>;
			if( comparerTest == null )
				throw new Exception( "Item type must have a IComparable<T> interface." );

			int count = list.Count;

			for( int n = 0; n < count - 1; n++ )
			{
				T v1 = list[ n ];

				T smallestValue = v1;
				int smallestIndex = n;

				for( int z = n + 1; z < count; z++ )
				{
					T v2 = list[ z ];

					if( ( (IComparable<T>)smallestValue ).CompareTo( v2 ) > 0 )
					{
						smallestValue = v2;
						smallestIndex = z;
					}
				}

				if( smallestIndex != n )
				{
					list[ n ] = smallestValue;
					list[ smallestIndex ] = v1;
				}
			}
		}

		/// <summary>
		/// Returns reversed list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetReverse<T>( this IList<T> list )
		{
			for( int n = list.Count - 1; n >= 0; n-- )
				yield return list[ n ];
		}

		/// <summary>
		/// Returns reversed linked list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetReverse<T>( this LinkedList<T> list )
		{
			var i = list.Last;
			while( i != null )
			{
				yield return i.Value;
				i = i.Previous;
			}
		}

		public static T[] FromByteArray<T>( byte[] array ) where T : unmanaged
		{
			unsafe
			{
				int count = array.Length / sizeof( T );
				var result = new T[ count ];
				fixed( byte* pArray = array )
				fixed( T* pResult = result )
					NativeUtility.CopyMemory( pResult, pArray, count * sizeof( T ) );
				return result;
			}
		}

		public static byte[] ToByteArray<T>( T[] array ) where T : unmanaged
		{
			unsafe
			{
				var result = new byte[ array.Length * sizeof( T ) ];
				fixed( T* pArray = array )
				fixed( byte* pResult = result )
					NativeUtility.CopyMemory( pResult, pArray, result.Length );
				return result;
			}
		}
	}
}
