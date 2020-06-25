// <copyright file="StoParallelMergeSort.cs" company="Martin Stoeckli">
// Copyright (c) Martin Stoeckli 2011 www.martinstoeckli.ch
// This code may be freely used in all kind of software.
// Version 2.1.
// </copyright>
namespace Sto
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// Implements a parallel and stable mergesort algorithm.
	/// - The sorting will run parallel, to take advantage of several processors.
	/// - This algorithm is stable, equal elements keep their original order, so
	///   you can sort several times without mixing up previous sortings.
	/// - The class can sort any type of arrays and lists (IList interface).
	/// - When reaching small block sizes, it switches to InsertionSort for
	///   performance reasons.
	/// </summary>
	/// <example>
	/// There are extensions available, so you can simply use it this way:
	/// <code>
	/// // Using the default comparer
	/// int[] intList = new int[] { 8, 0, 2, 2, 3 };
	/// intList.ParallelMergeSort();
	/// 
	/// // Using your own comparer
	/// string[] stringList = new string[] { "The", "brown", "fox", "jumps", "Fox" };
	/// stringList.ParallelMergeSort((a, b) => string.Compare(a, b));
	/// </code>
	/// It is also possible to work with the class directly, just create an
	/// instance of the StoParallelMergeSort class and call its Sort method.
	/// </example>
	/// <remarks>
	/// The mergesort algorithm needs at least 30% less comparisons than a
	/// quicksort, even less by switching to InsertionSort. This can be crucial,
	/// because the comparison of two objects can be expensive.
	/// </remarks>
	/// <typeparam name="T">Type of the list elements.</typeparam>
	class StoParallelMergeSort<T>
	{
		private const int InsertionSortBlockSize = 64;
		private readonly IComparer<T> _comparer;
		private readonly int _maxParallelDepth;
		//private bool _ascending = true;

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// It uses the default comparer. Call an overloaded constructor, if you
		/// have a specific comparer.
		/// </summary>
		public StoParallelMergeSort()
		{
			_comparer = Comparer<T>.Default;
			_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// </summary>
		/// <param name="comparison">A delegate, which can compare two elements
		/// of the list.</param>
		public StoParallelMergeSort( Comparison<T> comparison )
		{
			if( comparison == null )
				throw new ArgumentNullException( "comparison" );
			_comparer = new ComparerFromComparison<T>( comparison );
			_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// </summary>
		/// <param name="comparer">A comparer which can compare two elements
		/// of the list.</param>
		public StoParallelMergeSort( IComparer<T> comparer )
		{
			if( comparer == null )
				throw new ArgumentNullException( "comparer" );
			_comparer = comparer;
			_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Sorts an array of elements.
		/// </summary>
		/// <param name="list">Array of elements to sort.</param>
		/// <param name="ascending">Determines whether the elements will be sorted
		/// in ascending order. A descending sorting will still be stable.</param>
		public void Sort( T[] list, bool ascending = true )
		{
			if( list == null )
				throw new ArgumentNullException( "list" );
			if( list.Length < 2 )
				return;
			if( !ascending )
				NeoAxis.Log.Fatal( "StoParallelMergeSort: Sort: !ascending." );
			//_ascending = ascending;

			T[] tempList = new T[ list.Length ];
			SortBlock( list, tempList, 0, list.Length - 1, 1 );
		}

		/// <summary>
		/// Sorts a list of elements.
		/// </summary>
		/// <param name="list">List of elements to sort.</param>
		/// <param name="ascending">Determines whether the elements will be sorted
		/// in ascending order. A descending sorting will still be stable.</param>
		public void Sort( IList<T> list, bool ascending = true )
		{
			if( list == null )
				throw new ArgumentNullException( "list" );
			if( list.Count < 2 )
				return;

			// Create array from list for fast access
			T[] arrayList = new T[ list.Count ];
			list.CopyTo( arrayList, 0 );

			Sort( arrayList, ascending );

			// Copy ordered elements back to the list
			for( int index = 0; index < arrayList.Length; index++ )
				list[ index ] = arrayList[ index ];
		}

		/// <summary>
		/// Recursively called method which sorts a given range of the list.
		/// It splits the sorting into two independend blocks and afterwards calls
		/// the merging procedure for the independend sorted blocks.
		/// </summary>
		/// <param name="list">Original list with elements to sort.</param>
		/// <param name="tempList">Reused temporary array used for the merging.</param>
		/// <param name="beginBlock">First index of block to sort.</param>
		/// <param name="endBlock">Last index of the block to sort.</param>
		/// <param name="recursionDepth">Level of recursion.</param>
		void SortBlock( T[] list, T[] tempList, int beginBlock, int endBlock, int recursionDepth )
		{
			// Odd levels should store the result in the list, even levels in the
			// in tempList. This swapping avoids array copying from a temp list.
			bool mergeToTempList = recursionDepth % 2 == 0;
			bool workParallel = recursionDepth <= _maxParallelDepth;
			int blockSize = endBlock - beginBlock + 1;
			bool isSmallEnoughForInsertionSort = blockSize <= InsertionSortBlockSize;

			if( isSmallEnoughForInsertionSort )
			{
				// Switch to InsertionSort
				InsertionSort( list, beginBlock, endBlock );
				if( mergeToTempList )
					Array.Copy( list, beginBlock, tempList, beginBlock, blockSize );
			}
			else
			{
				// Split sorting into halves
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows
				if( workParallel )
				{
					Parallel.Invoke(
						() => SortBlock( list, tempList, beginBlock, middle, recursionDepth + 1 ),
						() => SortBlock( list, tempList, middle + 1, endBlock, recursionDepth + 1 ) );
				}
				else
				{
					SortBlock( list, tempList, beginBlock, middle, recursionDepth + 1 );
					SortBlock( list, tempList, middle + 1, endBlock, recursionDepth + 1 );
				}

				// Merge sorted halves
				if( mergeToTempList )
					MergeTwoBlocks( list, tempList, beginBlock, middle, middle + 1, endBlock );
				else
					MergeTwoBlocks( tempList, list, beginBlock, middle, middle + 1, endBlock );
			}
		}

		/// <summary>
		/// Merges two consecutive and already sorted blocks from <paramref name="sourceList"/>
		/// to one sorted block in <paramref name="targetList"/>. The block in the target list
		/// will begin at the same index.
		/// </summary>
		/// <param name="sourceList">Contains the two blocks to merge.</param>
		/// <param name="targetList">Receives sorted elements.</param>
		/// <param name="beginBlock1">First index of block 1, this is also the index
		/// where the block in targetList will start.</param>
		/// <param name="endBlock1">Index of last element of block 1.</param>
		/// <param name="beginBlock2">First index of block 2 (always endBlock1 + 1).</param>
		/// <param name="endBlock2">Index of last element of block2.</param>
		void MergeTwoBlocks( T[] sourceList, T[] targetList, int beginBlock1, int endBlock1, int beginBlock2, int endBlock2 )
		{
			for( int targetIndex = beginBlock1; targetIndex <= endBlock2; targetIndex++ )
			{
				if( beginBlock1 > endBlock1 )
				{
					// Nothing is left from block1, take next element from block2
					targetList[ targetIndex ] = sourceList[ beginBlock2++ ];
				}
				else if( beginBlock2 > endBlock2 )
				{
					// Nothing is left from block2, take next element from block1
					targetList[ targetIndex ] = sourceList[ beginBlock1++ ];
				}
				else
				{
					// Compare the next elements from both blocks and take the smaller one
					if( Compare( sourceList[ beginBlock1 ], sourceList[ beginBlock2 ] ) <= 0 )
						targetList[ targetIndex ] = sourceList[ beginBlock1++ ];
					else
						targetList[ targetIndex ] = sourceList[ beginBlock2++ ];
				}
			}
		}

		/// <summary>
		/// Implementation of the insertionsort which is efficient for small lists.
		/// </summary>
		/// <param name="list">List with element to sort.</param>
		/// <param name="beginBlock">Index of the first element in the block to sort.</param>
		/// <param name="endBlock">Index of the last element in the block so sort.</param>
		internal void InsertionSort( T[] list, int beginBlock, int endBlock )
		{
			for( int endAlreadySorted = beginBlock; endAlreadySorted < endBlock; endAlreadySorted++ )
			{
				T elementToInsert = list[ endAlreadySorted + 1 ];

				int insertPos = InsertionSortBinarySearch( list, beginBlock, endAlreadySorted, elementToInsert );
				if( insertPos <= endAlreadySorted )
				{
					// Shift elements to the right to make place for the elementToInsert
					Array.Copy( list, insertPos, list, insertPos + 1, endAlreadySorted - insertPos + 1 );
					list[ insertPos ] = elementToInsert;
				}
			}
		}

		/// <summary>
		/// Searches for the index in <paramref name="list"/> where the <paramref name="elementToInsert"/>
		/// should be inserted. The given search range has to be sorted already.
		/// If the element has an equal value to other existing element in the list,
		/// it will be placed after the existing elements (keep it stable).
		/// <example>
		/// list: { 3, 6, 9 }
		/// insert 2 => {^, 3, 6, 9 }
		/// insert 3 => {3, ^, 6, 9 }
		/// insert 10 => {3, 6, 9, ^ }
		/// </example>
		/// </summary>
		/// <param name="list">Search for the position within this list.</param>
		/// <param name="beginBlock">First index of the already sorted block, where we
		/// want to insert the element.</param>
		/// <param name="endBlock">Last index of the already sorted block, where we
		/// want to insert the element.</param>
		/// <param name="elementToInsert">Element we are looking for a place.</param>
		/// <returns>The index in list, where the element should be inserted.</returns>
		internal int InsertionSortBinarySearch( T[] list, int beginBlock, int endBlock, T elementToInsert )
		{
			while( beginBlock <= endBlock )
			{
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows

				int comparisonRes = Compare( elementToInsert, list[ middle ] );
				if( comparisonRes < 0 )
				{
					// elementToInsert was smaller, go to the left half
					endBlock = middle - 1;
				}
				else if( comparisonRes > 0 )
				{
					// elementToInsert was bigger, go to the right half
					beginBlock = middle + 1;
				}
				else
				{
					// elementToInsert was equal, move to the right as long as elements
					// are equal, to get the sorting stable
					beginBlock = middle + 1;
					while( ( beginBlock < endBlock ) && ( Compare( elementToInsert, list[ beginBlock + 1 ] ) == 0 ) )
						beginBlock++;
				}
			}
			return beginBlock;
		}

		/// <summary>
		/// Determines the depth of splitting the sorting into 2 tasks.
		/// This results in 2^depth tasks.
		/// </summary>
		/// <returns>Depth of splitting.</returns>
		int DetermineMaxParallelDepth()
		{
			const int MaxTasksPerProcessor = 8;
			int maxTaskCount = Environment.ProcessorCount * MaxTasksPerProcessor;
			return (int)Math.Log( maxTaskCount, 2 );
		}

		/// <summary>
		/// Helper function to get a central point for comparing operations.
		/// </summary>
		/// <param name="x">First element to compare.</param>
		/// <param name="y">Second element to compare.</param>
		/// <returns>The result of the comparer.</returns>
		int Compare( T x, T y )
		{
			int result = _comparer.Compare( x, y );
			//if( _ascending )
			return result;
			//else
			//	return -result;
		}

		/// <summary>
		/// Private helper class, which creates a Comparer around a Comparison delegate.
		/// </summary>
		/// <typeparam name="TU">Type of the elements of the sortable list.</typeparam>
		private class ComparerFromComparison<TU> : IComparer<TU>
		{
			private readonly Comparison<TU> _comparison;

			public ComparerFromComparison( Comparison<TU> comparison )
			{
				_comparison = comparison;
			}

			public int Compare( TU x, TU y )
			{
				return _comparison( x, y );
			}
		}
	}

	/// <summary>
	/// Implements an extension to all objects with <see cref="IList{T}"/> interfaces.
	/// </summary>
	static class StoParallelMergeSortExtension
	{
		public static void ParallelMergeSort<T>( this IList<T> list, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>();
			sorter.Sort( list, ascending );
		}

		public static void ParallelMergeSort<T>( this T[] list, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>();
			sorter.Sort( list, ascending );
		}

		public static void ParallelMergeSort<T>( this IList<T> list, Comparison<T> comparison, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>( comparison );
			sorter.Sort( list, ascending );
		}

		public static void ParallelMergeSort<T>( this T[] list, Comparison<T> comparison, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>( comparison );
			sorter.Sort( list, ascending );
		}

		public static void ParallelMergeSort<T>( this IList<T> list, IComparer<T> comparer, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>( comparer );
			sorter.Sort( list, ascending );
		}

		public static void ParallelMergeSort<T>( this T[] list, IComparer<T> comparer, bool ascending = true )
		{
			StoParallelMergeSort<T> sorter = new StoParallelMergeSort<T>( comparer );
			sorter.Sort( list, ascending );
		}
	}
}
