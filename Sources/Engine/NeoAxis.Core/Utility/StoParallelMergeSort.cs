// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// <copyright file="StoParallelMergeSort.cs" company="Martin Stoeckli">
// Copyright (c) Martin Stoeckli 2011 www.martinstoeckli.ch
// This code may be freely used in all kind of software.
// Version 2.1.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NeoAxis;

namespace Sto
{
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
		const int InsertionSortBlockSize = 64;
		readonly IComparer<T> _comparer;
		readonly int _maxParallelDepth;
		bool multithreaded;

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// </summary>
		/// <param name="comparer">A comparer which can compare two elements
		/// of the list.</param>
		public StoParallelMergeSort( IComparer<T> comparer, bool multithreaded )
		{
			if( comparer == null )
				throw new ArgumentNullException( "comparer" );
			_comparer = comparer;
			this.multithreaded = multithreaded;
			if( multithreaded )
				_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Sorts an array of elements.
		/// </summary>
		/// <param name="list">Array of elements to sort.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public void Sort( T[] list )
		{
			if( list == null )
				throw new ArgumentNullException( "list" );
			if( list.Length < 2 )
				return;

			var tempList = new T[ list.Length ];
			SortBlock( list, tempList, 0, list.Length - 1, 1 );
		}

		/// <summary>
		/// Sorts a list of elements.
		/// </summary>
		/// <param name="list">List of elements to sort.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public void Sort( IList<T> list )
		{
			if( list == null )
				throw new ArgumentNullException( "list" );
			if( list.Count < 2 )
				return;

			// Create array from list for fast access
			var arrayList = new T[ list.Count ];
			list.CopyTo( arrayList, 0 );

			Sort( arrayList );

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
		[MethodImpl( (MethodImplOptions)512 )]
		void SortBlock( T[] list, T[] tempList, int beginBlock, int endBlock, int recursionDepth )
		{
			// Odd levels should store the result in the list, even levels in the
			// in tempList. This swapping avoids array copying from a temp list.
			bool mergeToTempList = recursionDepth % 2 == 0;
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
				bool workParallel = multithreaded && recursionDepth <= _maxParallelDepth;
				if( workParallel )
				{
					//!!!!GC
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
		[MethodImpl( (MethodImplOptions)512 )]
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
					if( _comparer.Compare( sourceList[ beginBlock1 ], sourceList[ beginBlock2 ] ) <= 0 )
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
		[MethodImpl( (MethodImplOptions)512 )]
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
		[MethodImpl( (MethodImplOptions)512 )]
		internal int InsertionSortBinarySearch( T[] list, int beginBlock, int endBlock, T elementToInsert )
		{
			while( beginBlock <= endBlock )
			{
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows

				int comparisonRes = _comparer.Compare( elementToInsert, list[ middle ] );
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
					while( ( beginBlock < endBlock ) && ( _comparer.Compare( elementToInsert, list[ beginBlock + 1 ] ) == 0 ) )
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		int DetermineMaxParallelDepth()
		{
			const int MaxTasksPerProcessor = 8;
			int maxTaskCount = Environment.ProcessorCount * MaxTasksPerProcessor;
			return (int)Math.Log( maxTaskCount, 2 );
		}

		///// <summary>
		///// Helper function to get a central point for comparing operations.
		///// </summary>
		///// <param name="x">First element to compare.</param>
		///// <param name="y">Second element to compare.</param>
		///// <returns>The result of the comparer.</returns>
		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//int Compare( T x, T y )
		//{
		//	return _comparer.Compare( x, y );
		//}
	}

	/// <summary>
	/// Implements an extension to all objects with <see cref="IList{T}"/> interfaces.
	/// </summary>
	static class StoParallelMergeSortExtension
	{
		public static void ParallelMergeSort<T>( this IList<T> list, IComparer<T> comparer, bool multithreaded )
		{
			var sorter = new StoParallelMergeSort<T>( comparer, multithreaded );
			sorter.Sort( list );
		}

		public static void ParallelMergeSort<T>( this T[] list, IComparer<T> comparer, bool multithreaded )
		{
			var sorter = new StoParallelMergeSort<T>( comparer, multithreaded );
			sorter.Sort( list );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class StoParallelMergeSortUnmanaged<T> where T : unmanaged
	{
		const int InsertionSortBlockSize = 64;
		readonly IComparer<T> _comparer;
		readonly int _maxParallelDepth;
		bool multithreaded;

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// </summary>
		/// <param name="comparer">A comparer which can compare two elements
		/// of the list.</param>
		public StoParallelMergeSortUnmanaged( IComparer<T> comparer, bool multithreaded )
		{
			if( comparer == null )
				throw new ArgumentNullException( "comparer" );
			_comparer = comparer;
			this.multithreaded = multithreaded;
			if( multithreaded )
				_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Sorts an array of elements.
		/// </summary>
		/// <param name="list">Array of elements to sort.</param>
		/// <param name="ascending">Determines whether the elements will be sorted
		/// in ascending order. A descending sorting will still be stable.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void Sort( T* pointer, int count )
		{
			if( pointer == null )
				throw new ArgumentNullException( "pointer" );
			if( count < 2 )
				return;

			var size = sizeof( T ) * count;
			if( size < 4096 )
			{
				var tempList = stackalloc T[ count ];
				SortBlock( pointer, tempList, 0, count - 1, 1 );
			}
			else
			{
				var tempList = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, size );
				try
				{
					SortBlock( pointer, tempList, 0, count - 1, 1 );
				}
				finally
				{
					NativeUtility.Free( tempList );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe static void ArrayCopy( T* sourceArray, int sourceIndex, T* destinationArray, int destinationIndex, int length )
		{
			NativeUtility.CopyMemory( destinationArray + destinationIndex, sourceArray + sourceIndex, sizeof( T ) * length );
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void SortBlock( T* list, T* tempList, int beginBlock, int endBlock, int recursionDepth )
		{
			// Odd levels should store the result in the list, even levels in the
			// in tempList. This swapping avoids array copying from a temp list.
			bool mergeToTempList = recursionDepth % 2 == 0;
			int blockSize = endBlock - beginBlock + 1;
			bool isSmallEnoughForInsertionSort = blockSize <= InsertionSortBlockSize;

			if( isSmallEnoughForInsertionSort )
			{
				// Switch to InsertionSort
				InsertionSort( list, beginBlock, endBlock );
				if( mergeToTempList )
					ArrayCopy( list, beginBlock, tempList, beginBlock, blockSize );
			}
			else
			{
				// Split sorting into halves
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows
				bool workParallel = multithreaded && recursionDepth <= _maxParallelDepth;
				if( workParallel )
				{
					//!!!!GC
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void MergeTwoBlocks( T* sourceList, T* targetList, int beginBlock1, int endBlock1, int beginBlock2, int endBlock2 )
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
					if( _comparer.Compare( sourceList[ beginBlock1 ], sourceList[ beginBlock2 ] ) <= 0 )
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe internal void InsertionSort( T* list, int beginBlock, int endBlock )
		{
			for( int endAlreadySorted = beginBlock; endAlreadySorted < endBlock; endAlreadySorted++ )
			{
				T elementToInsert = list[ endAlreadySorted + 1 ];

				int insertPos = InsertionSortBinarySearch( list, beginBlock, endAlreadySorted, elementToInsert );
				if( insertPos <= endAlreadySorted )
				{
					// Shift elements to the right to make place for the elementToInsert
					ArrayCopy( list, insertPos, list, insertPos + 1, endAlreadySorted - insertPos + 1 );
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe internal int InsertionSortBinarySearch( T* list, int beginBlock, int endBlock, T elementToInsert )
		{
			while( beginBlock <= endBlock )
			{
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows

				int comparisonRes = _comparer.Compare( elementToInsert, list[ middle ] );
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
					while( ( beginBlock < endBlock ) && ( _comparer.Compare( elementToInsert, list[ beginBlock + 1 ] ) == 0 ) )
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		int DetermineMaxParallelDepth()
		{
			const int MaxTasksPerProcessor = 8;
			int maxTaskCount = Environment.ProcessorCount * MaxTasksPerProcessor;
			return (int)Math.Log( maxTaskCount, 2 );
		}

		///// <summary>
		///// Helper function to get a central point for comparing operations.
		///// </summary>
		///// <param name="x">First element to compare.</param>
		///// <param name="y">Second element to compare.</param>
		///// <returns>The result of the comparer.</returns>
		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//int Compare( T x, T y )
		//{
		//	return _comparer.Compare( x, y );
		//}
	}

	/// <summary>
	/// Implements an extension to all objects with <see cref="IList{T}"/> interfaces.
	/// </summary>
	static class StoParallelMergeSortExtensionUnmanaged
	{
		public unsafe static void ParallelMergeSort<T>( T* list, int count, IComparer<T> comparer, bool multithreaded ) where T : unmanaged
		{
			var sorter = new StoParallelMergeSortUnmanaged<T>( comparer, multithreaded );
			sorter.Sort( list, count );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class StoParallelMergeSortUnmanagedPointerComparison<T> where T : unmanaged
	{
		const int InsertionSortBlockSize = 64;
		readonly PointerComparison<T> _comparer;
		readonly int _maxParallelDepth;
		bool multithreaded;

		/// <summary>
		/// Initializes a new instance of the StoParallelMergeSort class.
		/// </summary>
		/// <param name="comparer">A comparer which can compare two elements
		/// of the list.</param>
		public StoParallelMergeSortUnmanagedPointerComparison( PointerComparison<T> comparer, bool multithreaded )
		{
			if( comparer == null )
				throw new ArgumentNullException( "comparer" );
			_comparer = comparer;
			this.multithreaded = multithreaded;
			if( multithreaded )
				_maxParallelDepth = DetermineMaxParallelDepth();
		}

		/// <summary>
		/// Sorts an array of elements.
		/// </summary>
		/// <param name="list">Array of elements to sort.</param>
		/// <param name="ascending">Determines whether the elements will be sorted
		/// in ascending order. A descending sorting will still be stable.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void Sort( T* pointer, int count )
		{
			if( pointer == null )
				throw new ArgumentNullException( "pointer" );
			if( count < 2 )
				return;

			var size = sizeof( T ) * count;
			if( size < 4096 )
			{
				var tempList = stackalloc T[ count ];
				SortBlock( pointer, tempList, 0, count - 1, 1 );
			}
			else
			{
				var tempList = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, size );
				try
				{
					SortBlock( pointer, tempList, 0, count - 1, 1 );
				}
				finally
				{
					NativeUtility.Free( tempList );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe static void ArrayCopy( T* sourceArray, int sourceIndex, T* destinationArray, int destinationIndex, int length )
		{
			NativeUtility.CopyMemory( destinationArray + destinationIndex, sourceArray + sourceIndex, sizeof( T ) * length );
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void SortBlock( T* list, T* tempList, int beginBlock, int endBlock, int recursionDepth )
		{
			// Odd levels should store the result in the list, even levels in the
			// in tempList. This swapping avoids array copying from a temp list.
			bool mergeToTempList = recursionDepth % 2 == 0;
			int blockSize = endBlock - beginBlock + 1;
			bool isSmallEnoughForInsertionSort = blockSize <= InsertionSortBlockSize;

			if( isSmallEnoughForInsertionSort )
			{
				// Switch to InsertionSort
				InsertionSort( list, beginBlock, endBlock );
				if( mergeToTempList )
					ArrayCopy( list, beginBlock, tempList, beginBlock, blockSize );
			}
			else
			{
				// Split sorting into halves
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows
				bool workParallel = multithreaded && recursionDepth <= _maxParallelDepth;
				if( workParallel )
				{
					//!!!!GC
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void MergeTwoBlocks( T* sourceList, T* targetList, int beginBlock1, int endBlock1, int beginBlock2, int endBlock2 )
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
					if( _comparer( &sourceList[ beginBlock1 ], &sourceList[ beginBlock2 ] ) <= 0 )
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe internal void InsertionSort( T* list, int beginBlock, int endBlock )
		{
			for( int endAlreadySorted = beginBlock; endAlreadySorted < endBlock; endAlreadySorted++ )
			{
				T elementToInsert = list[ endAlreadySorted + 1 ];

				int insertPos = InsertionSortBinarySearch( list, beginBlock, endAlreadySorted, &elementToInsert );
				if( insertPos <= endAlreadySorted )
				{
					// Shift elements to the right to make place for the elementToInsert
					ArrayCopy( list, insertPos, list, insertPos + 1, endAlreadySorted - insertPos + 1 );
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
		[MethodImpl( (MethodImplOptions)512 )]
		unsafe internal int InsertionSortBinarySearch( T* list, int beginBlock, int endBlock, T* elementToInsert )
		{
			while( beginBlock <= endBlock )
			{
				int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows

				int comparisonRes = _comparer( elementToInsert, &list[ middle ] );
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
					while( ( beginBlock < endBlock ) && ( _comparer( elementToInsert, &list[ beginBlock + 1 ] ) == 0 ) )
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		int DetermineMaxParallelDepth()
		{
			const int MaxTasksPerProcessor = 8;
			int maxTaskCount = Environment.ProcessorCount * MaxTasksPerProcessor;
			return (int)Math.Log( maxTaskCount, 2 );
		}
	}

	/// <summary>
	/// Implements an extension to all objects with <see cref="IList{T}"/> interfaces.
	/// </summary>
	static class StoParallelMergeSortExtensionUnmanagedPointerComparison
	{
		public unsafe static void ParallelMergeSort<T>( T* list, int count, PointerComparison<T> comparer, bool multithreaded ) where T : unmanaged
		{
			var sorter = new StoParallelMergeSortUnmanagedPointerComparison<T>( comparer, multithreaded );
			sorter.Sort( list, count );
		}
	}

}







//#if !UWP

//class StoParallelMergeSortUnmanaged<T> where T : unmanaged
//{
//	const int InsertionSortBlockSize = 64;
//	readonly IComparer<T> _comparer;
//	readonly int _maxParallelDepth;
//	bool multithreaded;

//	/// <summary>
//	/// Initializes a new instance of the StoParallelMergeSort class.
//	/// </summary>
//	/// <param name="comparer">A comparer which can compare two elements
//	/// of the list.</param>
//	public StoParallelMergeSortUnmanaged( IComparer<T> comparer, bool multithreaded )
//	{
//		if( comparer == null )
//			throw new ArgumentNullException( "comparer" );
//		_comparer = comparer;
//		this.multithreaded = multithreaded;
//		if( multithreaded )
//			_maxParallelDepth = DetermineMaxParallelDepth();
//	}

//	/// <summary>
//	/// Sorts an array of elements.
//	/// </summary>
//	/// <param name="list">Array of elements to sort.</param>
//	/// <param name="ascending">Determines whether the elements will be sorted
//	/// in ascending order. A descending sorting will still be stable.</param>
//	public unsafe void Sort( Memory<T> list )
//	{
//		//if( list == null )
//		//	throw new ArgumentNullException( "list" );
//		if( list.Length < 2 )
//			return;

//		var tempList = (T*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, list.Length * sizeof( T ) );
//		try
//		{
//			var tempList2 = new Memory<T>( tempList, list.Length );
//			//var tempList2 = new Span<T>( tempList, list.Length );
//			SortBlock( list, tempList2, 0, list.Length - 1, 1 );
//		}
//		finally
//		{
//			NativeUtility.Free( tempList );
//		}
//	}

//	static void ArrayCopy( Memory<T> sourceArray, int sourceIndex, Memory<T> destinationArray, int destinationIndex, int length )
//	{
//		var s = sourceArray.Slice( sourceIndex, length );
//		var d = destinationArray.Slice( destinationIndex, length );
//		s.CopyTo( d );
//	}

//	/// <summary>
//	/// Recursively called method which sorts a given range of the list.
//	/// It splits the sorting into two independend blocks and afterwards calls
//	/// the merging procedure for the independend sorted blocks.
//	/// </summary>
//	/// <param name="list">Original list with elements to sort.</param>
//	/// <param name="tempList">Reused temporary array used for the merging.</param>
//	/// <param name="beginBlock">First index of block to sort.</param>
//	/// <param name="endBlock">Last index of the block to sort.</param>
//	/// <param name="recursionDepth">Level of recursion.</param>
//	unsafe void SortBlock( Memory<T> list, Memory<T> tempList, int beginBlock, int endBlock, int recursionDepth )
//	{
//		// Odd levels should store the result in the list, even levels in the
//		// in tempList. This swapping avoids array copying from a temp list.
//		bool mergeToTempList = recursionDepth % 2 == 0;
//		int blockSize = endBlock - beginBlock + 1;
//		bool isSmallEnoughForInsertionSort = blockSize <= InsertionSortBlockSize;

//		if( isSmallEnoughForInsertionSort )
//		{
//			// Switch to InsertionSort
//			InsertionSort( list, beginBlock, endBlock );
//			if( mergeToTempList )
//				ArrayCopy( list, beginBlock, tempList, beginBlock, blockSize );
//		}
//		else
//		{
//			// Split sorting into halves
//			int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows
//			bool workParallel = multithreaded && recursionDepth <= _maxParallelDepth;
//			if( workParallel )
//			{
//				//var list2 = list;
//				//var tempList2 = tempList;

//				////ReadOnlySpan

//				//void Method( int beginBlock, int endBlock, int recursionDepth )
//				//{
//				//	SortBlock( list2, tempList2, beginBlock, middle, recursionDepth + 1 );
//				//}

//				//!!!!GC
//				Parallel.Invoke(
//					() => SortBlock( list, tempList, beginBlock, middle, recursionDepth + 1 ),
//					() => SortBlock( list, tempList, middle + 1, endBlock, recursionDepth + 1 ) );

//				////!!!!GC
//				//Parallel.Invoke(
//				//	() => SortBlock( list, tempList, beginBlock, middle, recursionDepth + 1 ),
//				//	() => SortBlock( list, tempList, middle + 1, endBlock, recursionDepth + 1 ) );
//			}
//			else
//			{
//				SortBlock( list, tempList, beginBlock, middle, recursionDepth + 1 );
//				SortBlock( list, tempList, middle + 1, endBlock, recursionDepth + 1 );
//			}

//			// Merge sorted halves
//			if( mergeToTempList )
//				MergeTwoBlocks( list, tempList, beginBlock, middle, middle + 1, endBlock );
//			else
//				MergeTwoBlocks( tempList, list, beginBlock, middle, middle + 1, endBlock );
//		}
//	}

//	/// <summary>
//	/// Merges two consecutive and already sorted blocks from <paramref name="sourceList"/>
//	/// to one sorted block in <paramref name="targetList"/>. The block in the target list
//	/// will begin at the same index.
//	/// </summary>
//	/// <param name="sourceList">Contains the two blocks to merge.</param>
//	/// <param name="targetList">Receives sorted elements.</param>
//	/// <param name="beginBlock1">First index of block 1, this is also the index
//	/// where the block in targetList will start.</param>
//	/// <param name="endBlock1">Index of last element of block 1.</param>
//	/// <param name="beginBlock2">First index of block 2 (always endBlock1 + 1).</param>
//	/// <param name="endBlock2">Index of last element of block2.</param>
//	unsafe void MergeTwoBlocks( Span<T> sourceList, Span<T> targetList, int beginBlock1, int endBlock1, int beginBlock2, int endBlock2 )
//	{
//		for( int targetIndex = beginBlock1; targetIndex <= endBlock2; targetIndex++ )
//		{
//			if( beginBlock1 > endBlock1 )
//			{
//				// Nothing is left from block1, take next element from block2
//				targetList[ targetIndex ] = sourceList[ beginBlock2++ ];
//			}
//			else if( beginBlock2 > endBlock2 )
//			{
//				// Nothing is left from block2, take next element from block1
//				targetList[ targetIndex ] = sourceList[ beginBlock1++ ];
//			}
//			else
//			{
//				// Compare the next elements from both blocks and take the smaller one
//				if( Compare( sourceList[ beginBlock1 ], sourceList[ beginBlock2 ] ) <= 0 )
//					targetList[ targetIndex ] = sourceList[ beginBlock1++ ];
//				else
//					targetList[ targetIndex ] = sourceList[ beginBlock2++ ];
//			}
//		}
//	}

//	/// <summary>
//	/// Implementation of the insertionsort which is efficient for small lists.
//	/// </summary>
//	/// <param name="list">List with element to sort.</param>
//	/// <param name="beginBlock">Index of the first element in the block to sort.</param>
//	/// <param name="endBlock">Index of the last element in the block so sort.</param>
//	unsafe internal void InsertionSort( Span<T> list, int beginBlock, int endBlock )
//	{
//		for( int endAlreadySorted = beginBlock; endAlreadySorted < endBlock; endAlreadySorted++ )
//		{
//			T elementToInsert = list[ endAlreadySorted + 1 ];

//			int insertPos = InsertionSortBinarySearch( list, beginBlock, endAlreadySorted, elementToInsert );
//			if( insertPos <= endAlreadySorted )
//			{
//				// Shift elements to the right to make place for the elementToInsert
//				ArrayCopy( list, insertPos, list, insertPos + 1, endAlreadySorted - insertPos + 1 );
//				list[ insertPos ] = elementToInsert;
//			}
//		}
//	}

//	/// <summary>
//	/// Searches for the index in <paramref name="list"/> where the <paramref name="elementToInsert"/>
//	/// should be inserted. The given search range has to be sorted already.
//	/// If the element has an equal value to other existing element in the list,
//	/// it will be placed after the existing elements (keep it stable).
//	/// <example>
//	/// list: { 3, 6, 9 }
//	/// insert 2 => {^, 3, 6, 9 }
//	/// insert 3 => {3, ^, 6, 9 }
//	/// insert 10 => {3, 6, 9, ^ }
//	/// </example>
//	/// </summary>
//	/// <param name="list">Search for the position within this list.</param>
//	/// <param name="beginBlock">First index of the already sorted block, where we
//	/// want to insert the element.</param>
//	/// <param name="endBlock">Last index of the already sorted block, where we
//	/// want to insert the element.</param>
//	/// <param name="elementToInsert">Element we are looking for a place.</param>
//	/// <returns>The index in list, where the element should be inserted.</returns>
//	internal int InsertionSortBinarySearch( Span<T> list, int beginBlock, int endBlock, T elementToInsert )
//	{
//		while( beginBlock <= endBlock )
//		{
//			int middle = beginBlock + ( ( endBlock - beginBlock ) / 2 ); // avoid overflows

//			int comparisonRes = Compare( elementToInsert, list[ middle ] );
//			if( comparisonRes < 0 )
//			{
//				// elementToInsert was smaller, go to the left half
//				endBlock = middle - 1;
//			}
//			else if( comparisonRes > 0 )
//			{
//				// elementToInsert was bigger, go to the right half
//				beginBlock = middle + 1;
//			}
//			else
//			{
//				// elementToInsert was equal, move to the right as long as elements
//				// are equal, to get the sorting stable
//				beginBlock = middle + 1;
//				while( ( beginBlock < endBlock ) && ( Compare( elementToInsert, list[ beginBlock + 1 ] ) == 0 ) )
//					beginBlock++;
//			}
//		}
//		return beginBlock;
//	}

//	/// <summary>
//	/// Determines the depth of splitting the sorting into 2 tasks.
//	/// This results in 2^depth tasks.
//	/// </summary>
//	/// <returns>Depth of splitting.</returns>
//	int DetermineMaxParallelDepth()
//	{
//		const int MaxTasksPerProcessor = 8;
//		int maxTaskCount = Environment.ProcessorCount * MaxTasksPerProcessor;
//		return (int)Math.Log( maxTaskCount, 2 );
//	}

//	/// <summary>
//	/// Helper function to get a central point for comparing operations.
//	/// </summary>
//	/// <param name="x">First element to compare.</param>
//	/// <param name="y">Second element to compare.</param>
//	/// <returns>The result of the comparer.</returns>
//	int Compare( T x, T y )
//	{
//		return _comparer.Compare( x, y );
//	}
//}

///// <summary>
///// Implements an extension to all objects with <see cref="IList{T}"/> interfaces.
///// </summary>
//static class StoParallelMergeSortExtensionUnmanaged
//{
//	public static void ParallelMergeSort<T>( this Span<T> list, IComparer<T> comparer, bool multithreaded ) where T : unmanaged
//	{
//		var sorter = new StoParallelMergeSortUnmanaged<T>( comparer, multithreaded );
//		sorter.Sort( list );
//	}
//}

//#endif
