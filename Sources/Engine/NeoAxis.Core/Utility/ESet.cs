using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// The class is a container that stores unique elements. This implementation keeps insertion order.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay( "Count = {Count}" )]
	public class ESet<T> : ICollection<T>, ICollection
	{
		object _syncRoot;
		Dictionary<T, int> baseDictionary;
		List<OrderedItem> ordered;
		Stack<int> freeOrderedIndexes;
		int orderedStartIndex = -1;
		int orderedLastIndex = -1;
		int version;
		ReadOnlyICollection<T> readOnlyCollection;

		/////////////////////////////////////////

		struct OrderedItem
		{
			public int previousItemIndex;
			public int nextItemIndex;
			public T key;
		}

		/////////////////////////////////////////

		class Enumerator : IEnumerator<T>
		{
			ESet<T> set;
			int version;
			int orderedIndex;
			T current;

			//

			public Enumerator( ESet<T> set )
			{
				this.set = set;
				this.version = set.version;
				this.orderedIndex = set.orderedStartIndex;
				this.current = default( T );
			}

			public T Current
			{
				get { return current; }
			}

			object IEnumerator.Current
			{
				get
				{
					if( orderedIndex == -1 )
						throw new InvalidOperationException( "Enumerate operation cannot happen" );
					return this.current;
				}
			}

			public void Dispose()
			{
				GC.SuppressFinalize( this );
			}

			public bool MoveNext()
			{
				if( this.version != this.set.version )
					throw new InvalidOperationException( "Enumerate failed version" );

				if( orderedIndex != -1 )
				{
					OrderedItem i = set.ordered[ orderedIndex ];
					orderedIndex = i.nextItemIndex;
					current = i.key;
					return true;
				}
				else
				{
					current = default( T );
					return false;
				}
			}

			public void Reset()
			{
				if( this.version != this.set.version )
				{
					throw new InvalidOperationException( "Enumerate failed version" );
				}
				this.orderedIndex = set.orderedStartIndex;
				this.current = default( T );
			}
		}

		/////////////////////////////////////////

		class ReverseEnumerator : IEnumerator<T>
		{
			ESet<T> set;
			int version;
			int orderedIndex;
			T current;

			//

			public ReverseEnumerator( ESet<T> dictionary )
			{
				this.set = dictionary;
				this.version = dictionary.version;
				this.orderedIndex = dictionary.orderedLastIndex;
				this.current = default( T );
			}

			public T Current
			{
				get { return current; }
			}

			object IEnumerator.Current
			{
				get
				{
					if( orderedIndex == -1 )
						throw new InvalidOperationException( "Enumerate operation cannot happen" );
					return this.current;
				}
			}

			public void Dispose()
			{
				GC.SuppressFinalize( this );
			}

			public bool MoveNext()
			{
				if( this.version != this.set.version )
					throw new InvalidOperationException( "Enumerate failed version" );

				if( orderedIndex != -1 )
				{
					OrderedItem i = set.ordered[ orderedIndex ];
					orderedIndex = i.previousItemIndex;
					current = i.key;
					return true;
				}
				else
				{
					current = default( T );
					return false;
				}
			}

			public void Reset()
			{
				if( this.version != this.set.version )
				{
					throw new InvalidOperationException( "Enumerate failed version" );
				}
				this.orderedIndex = set.orderedLastIndex;
				this.current = default( T );
			}
		}

		/////////////////////////////////////////

		public ESet()
			: this( 0, null )
		{
		}

		public ESet( IEqualityComparer<T> comparer )
			: this( 0, comparer )
		{
		}

		public ESet( int capacity )
			: this( capacity, null )
		{
		}

		public ESet( ICollection<T> collection )
			: this( collection, null )
		{
		}

		public ESet( int capacity, IEqualityComparer<T> comparer )
		{
			baseDictionary = new Dictionary<T, int>( capacity, comparer );
			ordered = new List<OrderedItem>( capacity );
		}

		public ESet( ICollection<T> set, IEqualityComparer<T> comparer )
		{
			baseDictionary = new Dictionary<T, int>( set.Count, comparer );
			ordered = new List<OrderedItem>( set.Count );
			foreach( T p in set )
				Add( p );
		}

		public int Count
		{
			get { return baseDictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get
			{
				if( this._syncRoot == null )
				{
					Interlocked.CompareExchange( ref this._syncRoot, new object(), null );
				}
				return this._syncRoot;
			}
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public void Add( T key )
		{
			int orderedIndex;
			if( freeOrderedIndexes != null && freeOrderedIndexes.Count != 0 )
			{
				orderedIndex = freeOrderedIndexes.Pop();
			}
			else
			{
				orderedIndex = ordered.Count;
				ordered.Add( new OrderedItem() );
			}

			if( orderedLastIndex != -1 )
			{
				OrderedItem i2 = ordered[ orderedLastIndex ];
				i2.nextItemIndex = orderedIndex;
				ordered[ orderedLastIndex ] = i2;
			}

			baseDictionary.Add( key, orderedIndex );

			OrderedItem orderedItem = new OrderedItem();
			orderedItem.previousItemIndex = orderedLastIndex;
			orderedItem.nextItemIndex = -1;
			orderedItem.key = key;
			ordered[ orderedIndex ] = orderedItem;

			orderedLastIndex = orderedIndex;
			if( orderedStartIndex == -1 )
				orderedStartIndex = orderedIndex;

			unchecked
			{
				version++;
			}
		}

		public bool AddWithCheckAlreadyContained( T key )
		{
			if( Contains( key ) )
				return false;
			Add( key );
			return true;
		}

		public void Clear()
		{
			baseDictionary.Clear();
			ordered.Clear();
			if( freeOrderedIndexes != null )
				freeOrderedIndexes.Clear();
			orderedStartIndex = -1;
			orderedLastIndex = -1;
			unchecked
			{
				version++;
			}
		}

		public bool Contains( T key )
		{
			if( Count == 0 )
				return false;
			return baseDictionary.ContainsKey( key );
		}

		public void CopyTo( T[] array, int arrayIndex = 0 )
		{
			int n = arrayIndex;
			foreach( T v in this )
			{
				array[ n ] = v;
				n++;
			}
		}

		public void CopyTo( Array array, int arrayIndex = 0 )
		{
			int n = arrayIndex;
			foreach( T v in this )
			{
				array.SetValue( v, n );
				n++;
			}
		}

		public T[] ToArray()
		{
			T[] array = new T[ Count ];
			CopyTo( array, 0 );
			return array;
		}

		public bool Remove( T key )
		{
			int orderedIndex;
			if( !baseDictionary.TryGetValue( key, out orderedIndex ) )
				return false;

			baseDictionary.Remove( key );

			OrderedItem i = ordered[ orderedIndex ];

			//this is last?
			if( orderedIndex == orderedLastIndex )
				orderedLastIndex = i.previousItemIndex;
			//this is first?
			if( orderedIndex == orderedStartIndex )
				orderedStartIndex = i.nextItemIndex;

			if( i.previousItemIndex != -1 )
			{
				OrderedItem i2 = ordered[ i.previousItemIndex ];
				i2.nextItemIndex = i.nextItemIndex;
				ordered[ i.previousItemIndex ] = i2;
			}
			if( i.nextItemIndex != -1 )
			{
				OrderedItem i2 = ordered[ i.nextItemIndex ];
				i2.previousItemIndex = i.previousItemIndex;
				ordered[ i.nextItemIndex ] = i2;
			}

			if( freeOrderedIndexes == null )
				freeOrderedIndexes = new Stack<int>();
			freeOrderedIndexes.Push( orderedIndex );

			unchecked
			{
				version++;
			}

			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator( this );
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator( this );
		}

		public IEnumerable<T> Reverse()
		{
			ReverseEnumerator e = new ReverseEnumerator( this );
			while( e.MoveNext() )
				yield return e.Current;
		}

		public ICollection<T> AsReadOnly()
		{
			if( readOnlyCollection == null )
				readOnlyCollection = new ReadOnlyICollection<T>( this );
			return readOnlyCollection;
		}

		public void AddRange( IEnumerable<T> collection )
		{
			foreach( T v in collection )
				Add( v );
		}

		public void AddRangeWithCheckAlreadyContained( IEnumerable<T> collection )
		{
			foreach( T v in collection )
				AddWithCheckAlreadyContained( v );
		}

		public static bool IsEqual( ESet<T> set1, ESet<T> set2 )
		{
			if( set1.Count != set2.Count )
				return false;

			var enum1 = set1.GetEnumerator();
			var enum2 = set2.GetEnumerator();
			while( enum1.MoveNext() )
			{
				enum2.MoveNext();
				if( !Equals( enum1.Current, enum2.Current ) )
					return false;
			}

			return true;
		}

		//public T Find( Predicate<T> match )
		//{
		//}
	}
}
