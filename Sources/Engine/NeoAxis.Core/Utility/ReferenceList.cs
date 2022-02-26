// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// An interface to get general data of <see cref="ReferenceList{T}"/>.
	/// </summary>
	public interface IReferenceList
	{
		Component Owner { get; }
		Type GetItemType();
		Type GetItemValueType();

		//object CreateItemValue();
	}

	//!!!!name ListWithReferenceSupport
	/// <summary>
	/// List which supports NeoAxis references.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay( "Count = {Count}" )]
	public class ReferenceList<T> : /*IEnumerable<Reference<T>>, IEnumerable,*/ IList<Reference<T>>, IList, IReferenceList
	// : IList<T>, System.Collections.IList, IReadOnlyList<T>
	//: IEnumerable<T>, IEnumerable
	//!!!!, IList<T>, ICollection<T>, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
	{
		Component owner;
		Action changedEvent;
		ReferenceField<T>[] _items = new ReferenceField<T>[ 4 ];
		int _size;
		//int _version;
		object _syncRoot;

		//!!!!changed events. внутри класса?

		/////////////////////////////////////////

		/// <summary>
		/// Represents an enumerator for <see cref="ReferenceList{T}"/>.
		/// </summary>
		public class Enumerator : IEnumerator<Reference<T>>, IEnumerator
		{
			ReferenceList<T> list;
			int index;
			//int version;
			Reference<T> current;

			//

			public Enumerator( ReferenceList<T> list )
			{
				this.list = list;
				index = 0;
				//version = list.version;
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
					if( index == 0 || index == list.Count + 1 )
						throw new InvalidOperationException( "Enumerate operation cannot happen" );
					return this.current;
				}
			}

			Reference<T> IEnumerator<Reference<T>>.Current
			{
				get
				{
					if( index == 0 || index == list.Count + 1 )
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
				var localList = list;

				if(/* version == localList._version && */( (uint)index < (uint)localList.Count ) )
				{
					current = localList[ index ];
					index++;
					return true;
				}
				else
				{
					//if( version != list._version )
					//{
					//	ThrowHelper.ThrowInvalidOperationException( ExceptionResource.InvalidOperation_EnumFailedVersion );
					//}

					index = list.Count + 1;
					current = default( T );
					return false;
				}
			}

			public void Reset()
			{
				this.index = 0;
				this.current = default( T );
			}
		}

		/////////////////////////////////////////

		public ReferenceList( Component owner, Action changedEvent )
		{
			this.owner = owner;
			this.changedEvent = changedEvent;
		}

		public Component Owner
		{
			get { return owner; }
		}

		//!!!!получать что изменилось?
		public Action ChangedEvent
		{
			get { return changedEvent; }
		}

		public Reference<T> this[ int index ]
		{
			get
			{
				ref var itemByRef = ref _items[ index ];
				if( itemByRef.BeginGet() )
					this[ index ] = itemByRef.Get( owner );
				return _items[ index ].value;
			}
			set
			{
				ref var itemByRef = ref _items[ index ];
				if( itemByRef.BeginSet( ref value ) )
				{
					try { PerformChangedEvent(); }
					finally { itemByRef.EndSet(); }
				}
			}
		}

		//object IList.this[ int index ]
		//{
		//	get
		//	{
		//		throw new NotImplementedException();
		//	}

		//	set
		//	{
		//		throw new NotImplementedException();
		//	}
		//}

		public int Count
		{
			get { return _size; }
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		//public bool IsFixedSize
		//{
		//	get
		//	{
		//		throw new NotImplementedException();
		//	}
		//}

		public object SyncRoot
		{
			get
			{
				if( _syncRoot == null )
					Interlocked.CompareExchange( ref _syncRoot, new object(), null );
				return _syncRoot;
			}
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public int Capacity
		{
			get { return _items.Length; }
			set
			{
				if( value < _size )
					throw new OutOfMemoryException( "Small capacity." );

				if( value != _items.Length )
				{
					if( value > 0 )
					{
						ReferenceField<T>[] newItems = new ReferenceField<T>[ value ];
						if( _size > 0 )
							Array.Copy( _items, 0, newItems, 0, _size );
						_items = newItems;
					}
					else
						_items = Array.Empty<ReferenceField<T>>();
				}
			}
		}

		public bool IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object IList.this[ int index ]
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		void EnsureCapacity( int min )
		{
			if( _items.Length < min )
			{
				int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
				if( newCapacity < min ) newCapacity = min;
				Capacity = newCapacity;
			}
		}

		public void Add( Reference<T> item )
		{
			if( _size == _items.Length ) EnsureCapacity( _size + 1 );
			_items[ _size++ ] = item;
			//_version++;
			PerformChangedEvent();
		}

		public void Clear()
		{
			if( _size > 0 )
			{
				Array.Clear( _items, 0, _size );
				_size = 0;
				PerformChangedEvent();
			}
			//_version++;
		}

		//AddRange, InsertRange и другие


		//public void CopyTo( T[] array, int arrayIndex )
		//{
		//	throw new NotImplementedException();
		//}

		//public void CopyTo( Array array, int index )
		//{
		//	throw new NotImplementedException();
		//}

		//!!!!check
		//public int IndexOf( Reference<T> item )
		//{
		//	for( int n = 0; n < Count; n++ )
		//		if( Equals( this[ n ], item ) )
		//			return n;
		//	return -1;
		//}

		public int IndexOf( T itemValue )
		{
			for( int n = 0; n < Count; n++ )
				if( Equals( this[ n ].Value, itemValue ) )
					return n;
			return -1;
		}

		//!!!!check
		//public bool Contains( Reference<T> item )
		//{
		//	for( int n = 0; n < Count; n++ )
		//		if( Equals( this[ n ], item ) )
		//			return true;
		//	return false;
		//}

		public bool Contains( T itemValue )
		{
			for( int n = 0; n < Count; n++ )
				if( Equals( this[ n ].Value, itemValue ) )
					return true;
			return false;
		}

		//public int IndexOf( object value )
		//{
		//	throw new NotImplementedException();
		//}

		//public bool Contains( object value )
		//{
		//	throw new NotImplementedException();
		//}

		public void Insert( int index, Reference<T> item )
		{
			// Note that insertions at the end are legal.
			if( (uint)index > (uint)_size )
				throw new OutOfMemoryException( "Out of range list insert." );
			if( _size == _items.Length ) EnsureCapacity( _size + 1 );
			if( index < _size )
				Array.Copy( _items, index, _items, index + 1, _size - index );
			_items[ index ] = item;
			_size++;
			//_version++;
			PerformChangedEvent();
		}

		public void RemoveAt( int index )
		{
			if( (uint)index >= (uint)_size )
				throw new OutOfMemoryException( "Out of range." );
			_size--;
			if( index < _size )
				Array.Copy( _items, index + 1, _items, index, _size - index );
			_items[ _size ] = default( ReferenceField<T> );
			//_version++;
			PerformChangedEvent();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator( this );
		}

		IEnumerator<Reference<T>> IEnumerable<Reference<T>>.GetEnumerator()
		{
			return new Enumerator( this );
		}

		public void PerformChangedEvent()
		{
			changedEvent?.Invoke();
		}

		public int IndexOf( Reference<T> item )
		{
			//!!!!check

			for( int n = 0; n < Count; n++ )
			{
				ref var v = ref _items[ n ].value;
				if( ( v.ReferenceSpecified || item.ReferenceSpecified ) )
				{
					if( v.GetByReference == item.GetByReference )
						return n;
				}
				else if( v.Value.Equals( item.Value ) )
					return n;
			}
			return -1;
		}

		public bool Contains( Reference<T> item )
		{
			return IndexOf( item ) != -1;
		}

		public void CopyTo( Reference<T>[] array, int arrayIndex )
		{
			for( int n = 0; n < array.Length; n++ )
				array[ n + arrayIndex ] = _items[ n ].value;
		}

		public bool Remove( Reference<T> item )
		{
			var index = IndexOf( item );
			if( index != -1 )
			{
				RemoveAt( index );
				return true;
			}
			else
				return false;
		}

		public int Add( object value )
		{
			if( value != null )
			{
				if( value is Reference<T> )
					Add( (Reference<T>)value );
				else if( value is T )
					Add( new Reference<T>( (T)value ) );
			}
			else
				Add( new Reference<T>() );

			//Add( (Reference<T>)value );
			return Count - 1;
		}

		public bool Contains( object value )
		{
			return Contains( (Reference<T>)value );
		}

		public int IndexOf( object value )
		{
			return IndexOf( (Reference<T>)value );
		}

		public void Insert( int index, object value )
		{
			Insert( index, (Reference<T>)value );
		}

		public void Remove( object value )
		{
			Remove( (Reference<T>)value );
		}

		public void CopyTo( Array array, int index )
		{
			CopyTo( (Reference<T>[])array, index );
		}

		//public object CreateItemValue()
		//{
		//	return new Reference<T>();
		//}

		public Type GetItemType()
		{
			return typeof( Reference<T> );
		}

		public Type GetItemValueType()
		{
			return typeof( T );
		}

		//!!!!другие методы полезные из ESet
	}
}
