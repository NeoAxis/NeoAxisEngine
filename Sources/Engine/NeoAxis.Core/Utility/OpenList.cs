// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A list that has the ability to directly work with elements of an array. For example, you can access to item by reference.
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

		public OpenList()
			: this( 4 )
		{
		}

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

		public void Add( T item )
		{
			Add( ref item );
		}

		public T[] ToArray()
		{
			var result = new T[ Count ];
			Array.Copy( Data, result, Count );
			return result;
		}

		/// <summary>
		/// Clears the list. The method only sets the Count to 0. Values of items are not reset to default.
		/// </summary>
		public void Clear()
		{
			Count = 0;
		}

		//!!!!AddRange
	}
}
