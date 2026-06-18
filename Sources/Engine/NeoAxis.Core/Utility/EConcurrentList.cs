// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis
{
	/// <summary>
	/// Represents a thread-safe, dynamically sized list of objects that supports indexed access and collection operations.
	/// </summary>
	/// <remarks>
	/// EConcurrentList<T> provides synchronized access to its elements, making it suitable for use in
	/// multi-threaded scenarios where multiple threads may add, remove, or access items concurrently. Enumeration is
	/// performed over a snapshot of the collection, so changes made to the list after the enumerator is created are not
	/// reflected during enumeration. This class is not a replacement for all concurrent collection scenarios; for
	/// high-contention or highly parallel workloads, consider using collections from
	/// System.Collections.Concurrent.
	/// </remarks>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	[DebuggerDisplay( "Count = {Count}" )]
	public class EConcurrentList<T> : IList<T>
	{
		readonly List<T> list = new List<T>();

		//

		public EConcurrentList()
		{
		}

		public EConcurrentList( IEnumerable<T> collection )
			: this()
		{
			foreach( var item in collection )
				Add( item );
		}

		public T this[ int index ]
		{
			get
			{
				lock( list )
					return list[ index ];
			}

			set
			{
				lock( list )
					list[ index ] = value;
			}
		}

		public int Count
		{
			get
			{
				lock( list )
					return list.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add( T item )
		{
			lock( list )
				list.Add( item );
		}

		public void Clear()
		{
			lock( list )
				list.Clear();
		}

		public bool Contains( T item )
		{
			lock( list )
				return list.Contains( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			lock( list )
				list.CopyTo( array, arrayIndex );
		}

		public IEnumerator<T> GetEnumerator()
		{
			// Snapshot enumerator to avoid holding the lock during enumeration
			T[] snapshot;
			lock( list )
				snapshot = list.ToArray();

			return ( (IEnumerable<T>)snapshot ).GetEnumerator();
		}

		public int IndexOf( T item )
		{
			lock( list )
				return list.IndexOf( item );
		}

		public void Insert( int index, T item )
		{
			lock( list )
				list.Insert( index, item );
		}

		public bool Remove( T item )
		{
			lock( list )
				return list.Remove( item );
		}

		public void RemoveAt( int index )
		{
			lock( list )
				list.RemoveAt( index );
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}