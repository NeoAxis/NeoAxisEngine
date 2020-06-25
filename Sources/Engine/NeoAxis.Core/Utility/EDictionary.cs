using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The class is an associative container that stores elements formed by a combination of a key value and a mapped value. This implementation keeps insertion order.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[DebuggerDisplay( "Count = {Count}" )]
	public class EDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		Dictionary<TKey, int> baseDictionary;
		List<OrderedItem> ordered;
		Stack<int> freeOrderedIndexes;
		int orderedStartIndex = -1;
		int orderedLastIndex = -1;
		int version;

		/////////////////////////////////////////

		struct OrderedItem
		{
			public int previousItemIndex;
			public int nextItemIndex;
			public TKey key;
			public TValue value;
		}

		/////////////////////////////////////////

		class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			EDictionary<TKey, TValue> dictionary;
			int version;
			int orderedIndex;
			KeyValuePair<TKey, TValue> current;

			//

			public Enumerator( EDictionary<TKey, TValue> dictionary )
			{
				this.dictionary = dictionary;
				this.version = dictionary.version;

				this.orderedIndex = dictionary.orderedStartIndex;
				this.current = default( KeyValuePair<TKey, TValue> );
			}

			public KeyValuePair<TKey, TValue> Current
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
				if( this.version != this.dictionary.version )
					throw new InvalidOperationException( "Enumerate failed version" );

				if( orderedIndex != -1 )
				{
					OrderedItem i = dictionary.ordered[ orderedIndex ];
					orderedIndex = i.nextItemIndex;
					current = new KeyValuePair<TKey, TValue>( i.key, i.value );
					return true;
				}
				else
				{
					current = default( KeyValuePair<TKey, TValue> );
					return false;
				}
			}

			public void Reset()
			{
				if( this.version != this.dictionary.version )
				{
					throw new InvalidOperationException( "Enumerate failed version" );
				}
				this.orderedIndex = dictionary.orderedStartIndex;
				this.current = default( KeyValuePair<TKey, TValue> );
			}
		}

		/////////////////////////////////////////

		public EDictionary()
			: this( 0, null )
		{
		}

		public EDictionary( IEqualityComparer<TKey> comparer )
			: this( 0, comparer )
		{
		}

		public EDictionary( int capacity )
			: this( capacity, null )
		{
		}

		public EDictionary( IDictionary<TKey, TValue> dictionary )
			: this( dictionary, null )
		{
		}

		public EDictionary( int capacity, IEqualityComparer<TKey> comparer )
		{
			baseDictionary = new Dictionary<TKey, int>( capacity, comparer );
			ordered = new List<OrderedItem>( capacity );
		}

		public EDictionary( IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer )
		{
			baseDictionary = new Dictionary<TKey, int>( dictionary.Count, comparer );
			ordered = new List<OrderedItem>( dictionary.Count );
			foreach( KeyValuePair<TKey, TValue> p in dictionary )
				Add( p.Key, p.Value );
		}

		public TValue this[ TKey key ]
		{
			get { return ordered[ baseDictionary[ key ] ].value; }
			set
			{
				Remove( key );
				Add( key, value );

				unchecked
				{
					version++;
				}
			}
		}

		public int Count
		{
			get { return baseDictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public ICollection<TKey> Keys
		{
			get
			{
				//TO DO: good?
				TKey[] v = new TKey[ Count ];
				int n = 0;
				foreach( KeyValuePair<TKey, TValue> p in this )
				{
					v[ n ] = p.Key;
					n++;
				}
				return v;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				//TO DO: good?
				TValue[] v = new TValue[ Count ];
				int n = 0;
				foreach( KeyValuePair<TKey, TValue> p in this )
				{
					v[ n ] = p.Value;
					n++;
				}
				return v;
			}
		}

		public void Add( KeyValuePair<TKey, TValue> item )
		{
			Add( item.Key, item.Value );
		}

		public void Add( TKey key, TValue value )
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
			orderedItem.value = value;
			ordered[ orderedIndex ] = orderedItem;

			orderedLastIndex = orderedIndex;
			if( orderedStartIndex == -1 )
				orderedStartIndex = orderedIndex;

			unchecked
			{
				version++;
			}
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

		public bool ContainsKey( TKey key )
		{
			return baseDictionary.ContainsKey( key );
		}

		public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
		{
			int index = arrayIndex;
			foreach( KeyValuePair<TKey, TValue> v in this )
			{
				array[ index ] = v;
				index++;
			}
		}

		public bool Contains( KeyValuePair<TKey, TValue> item )
		{
			TValue v;
			if( TryGetValue( item.Key, out v ) )
			{
				if( v.Equals( item.Value ) )
					return true;
			}
			return false;
		}

		public bool Remove( KeyValuePair<TKey, TValue> item )
		{
			if( Contains( item ) )
			{
				Remove( item.Key );
				return true;
			}
			return false;
		}

		public bool Remove( TKey key )
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

		public bool TryGetValue( TKey key, out TValue value )
		{
			int orderedIndex;
			if( baseDictionary.TryGetValue( key, out orderedIndex ) )
			{
				value = ordered[ orderedIndex ].value;
				return true;
			}
			else
			{
				value = default( TValue );
				return false;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new Enumerator( this );
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator( this );
		}
	}
}
