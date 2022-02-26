// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Read only interface for ICollection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ReadOnlyICollection<T> : ICollection<T>
	{
		ICollection<T> collection;

		//

		public ReadOnlyICollection( ICollection<T> collection )
		{
			this.collection = collection;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)collection ).GetEnumerator();
		}

		public bool Contains( T item )
		{
			return collection.Contains( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			collection.CopyTo( array, arrayIndex );
		}

		public int Count
		{
			get { return collection.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add( T item )
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Remove( T item )
		{
			throw new NotSupportedException();
		}
	}
}
