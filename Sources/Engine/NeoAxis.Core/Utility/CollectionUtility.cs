// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		//!!!!было
		//!!!!что с поточностью тут?

		//public static T[] AddElement<T>( T[] array, T element )
		//{
		//	//!!!!!? в InsertElement тоже?. или Fatal?
		//	if( array == null )
		//		return new T[] { element };

		//	T[] newArray = new T[ array.Length + 1 ];
		//	for( int n = 0; n < array.Length; n++ )
		//		newArray[ n ] = array[ n ];
		//	newArray[ newArray.Length - 1 ] = element;
		//	return newArray;
		//}

		//public static T[] InsertElement<T>( T[] array, T element, int index )
		//{
		//	if( index < 0 || index > array.Length )
		//		Log.Fatal( "ArrayUtils: InsertElement: index < 0 || index > array.Length." );

		//	T[] newArray = new T[ array.Length + 1 ];
		//	for( int n = 0; n < index; n++ )
		//		newArray[ n ] = array[ n ];
		//	newArray[ index ] = element;
		//	for( int n = index + 1; n < newArray.Length; n++ )
		//		newArray[ n ] = array[ n - 1 ];
		//	return newArray;
		//}

		//public static T[] RemoveElement<T>( T[] array, int index )
		//{
		//	if( index < 0 || index >= array.Length )
		//		Log.Fatal( "ArrayUtils: RemoveElement: index < 0 || index >= array.Length." );

		//	T[] newArray = new T[ array.Length - 1 ];
		//	int newIndex = 0;
		//	for( int n = 0; n < array.Length; n++ )
		//	{
		//		if( n != index )
		//		{
		//			newArray[ newIndex ] = array[ n ];
		//			newIndex++;
		//		}
		//	}
		//	return newArray;
		//}

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
				Log.Fatal( "CollectionUtility: SelectionSort: Item not have IComparable<T> interface." );

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

	}
}
