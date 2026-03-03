// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis
{
	[DebuggerDisplay( "Count = {Count}" )]
	public class EConcurrentHashSet<T> : ICollection<T>
	{
		readonly ConcurrentDictionary<T, int> dictionary;

		//

		public EConcurrentHashSet()
		{
			dictionary = new ConcurrentDictionary<T, int>();
		}

		public EConcurrentHashSet( IEnumerable<T> collection )
			: this()
		{
			foreach( var item in collection )
				Add( item );
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Add( T item )
		{
			if( !dictionary.TryAdd( item, 0 ) )
				throw new ArgumentException( "An item with the same value already exists in the set." );
		}

		public bool AddWithCheckAlreadyContained( T item )
		{
			return dictionary.TryAdd( item, 0 );

			//if( Contains( item ) )
			//	return false;
			//return dictionary.TryAdd( item, 0 );
		}

		public bool Remove( T item )
		{
			return dictionary.TryRemove( item, out _ );
		}

		public bool Contains( T item )
		{
			return dictionary.ContainsKey( item );
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			if( array == null ) throw new ArgumentNullException( nameof( array ) );
			if( arrayIndex < 0 || arrayIndex >= array.Length ) throw new ArgumentOutOfRangeException( nameof( arrayIndex ) );
			if( array.Length - arrayIndex < Count ) throw new ArgumentException( "Not enough space in the target array." );

			foreach( var key in dictionary.Keys )
				array[ arrayIndex++ ] = key;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return dictionary.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		//public void UnionWith( IEnumerable<T> other )
		//{
		//	foreach( var item in other )
		//		Add( item );
		//}

		//public void IntersectWith( IEnumerable<T> other )
		//{
		//	var itemsToRemove = new List<T>();

		//	foreach( var item in dictionary.Keys )
		//	{
		//		if( !Contains( item ) )
		//			itemsToRemove.Add( item );
		//	}

		//	foreach( var item in itemsToRemove )
		//		Remove( item );
		//}

		//public void ExceptWith( IEnumerable<T> other )
		//{
		//	foreach( var item in other )
		//		Remove( item );
		//}

		public void AddRange( IEnumerable<T> collection )
		{
			foreach( T v in collection )
				Add( v );
		}
	}
}