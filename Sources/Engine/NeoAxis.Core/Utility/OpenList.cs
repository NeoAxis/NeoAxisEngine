// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A list that has the ability to directly work with elements of an array. For example you can access to item by reference.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OpenList<T>
	{
		public T[] Data;
		public int Count;

		public OpenList( int capacity )
		{
			Data = new T[ capacity ];
		}

		public OpenList( T[] array )
		{
			Data = array;
			Count = array.Length;
		}

		public OpenList()
			: this( 4 )
		{
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( ref T item )
		{
			if( Count == Data.Length )
			{
				var old = Data;
				Data = new T[ old.Length != 0 ? old.Length * 2 : 4 ];
				Array.Copy( old, Data, old.Length );
			}
			Data[ Count++ ] = item;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( T item )
		{
			Add( ref item );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void AddRange( IEnumerable<T> collection )
		{
			//!!!!slowly
			foreach( var item in collection )
				Add( item );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ref T AddNotInitialized()
		{
			if( Count == Data.Length )
			{
				var old = Data;
				Data = new T[ old.Length != 0 ? old.Length * 2 : 4 ];
				Array.Copy( old, Data, old.Length );
			}
			return ref Data[ Count++ ];
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public void AddDefault()
		//{
		//	if( Count == Data.Length )
		//	{
		//		var old = Data;
		//		Data = new T[ old.Length != 0 ? old.Length * 2 : 4 ];
		//		Array.Copy( old, Data, old.Length );
		//	}
		//	Data[ Count++ ] = default( T );
		//}

		public T[] ToArray()
		{
			var result = new T[ Count ];
			Array.Copy( Data, result, Count );
			return result;
		}

		public ArraySegment<T> ArraySegment
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return new ArraySegment<T>( Data, 0, Count ); }
		}

		/// <summary>
		/// Clears the list. The method only sets the Count to 0. Values of items are not reset to default.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clear()
		{
			Count = 0;
		}

		public T this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				unsafe
				{
					return Data[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				unsafe
				{
					Data[ index ] = value;
				}
			}
		}
	}
}
