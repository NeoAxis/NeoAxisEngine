// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// A simple thread-safe queue built on top of Queue{T}. Bug in .NET ConcurrentQueue; leaks when call TryPeek.
	/// </summary>
	public class EConcurrentQueue<T> : IEnumerable<T>
	{
		readonly Queue<T> queue;

		//

		public EConcurrentQueue()
		{
			queue = new Queue<T>();
		}

		public EConcurrentQueue( int capacity )
		{
			queue = new Queue<T>( capacity );
		}

		public EConcurrentQueue( IEnumerable<T> collection )
		{
			if( collection == null )
				throw new ArgumentNullException( nameof( collection ) );

			queue = new Queue<T>( collection );
		}

		/// <summary>
		/// Gets the number of elements contained in the queue.
		/// </summary>
		public int Count
		{
			get
			{
				lock( queue )
					return queue.Count;
			}
		}

		/// <summary>
		/// Adds an element at the end of the queue.
		/// </summary>
		public void Enqueue( T item )
		{
			lock( queue )
				queue.Enqueue( item );
		}

		/// <summary>
		/// Attempts to dequeue an element. Returns true if successful.
		/// </summary>
		public bool TryDequeue( out T result )
		{
			lock( queue )
			{
				if( queue.Count == 0 )
				{
					result = default!;
					return false;
				}

				result = queue.Dequeue();
				return true;
			}
		}

		/// <summary>
		/// Attempts to peek the first element without removing it.
		/// </summary>
		public bool TryPeek( out T result )
		{
			lock( queue )
			{
				if( queue.Count == 0 )
				{
					result = default!;
					return false;
				}

				result = queue.Peek();
				return true;
			}
		}

		/// <summary>
		/// Removes all objects from the queue.
		/// </summary>
		public void Clear()
		{
			lock( queue )
				queue.Clear();
		}

		/// <summary>
		/// Copies the elements to an array snapshot.
		/// </summary>
		public T[] ToArray()
		{
			lock( queue )
				return queue.ToArray();
		}

		/// <summary>
		/// Returns an enumerator for a snapshot of the queue. The snapshot is a copy of the current collection; it is safe to enumerate.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			T[] snapshot;
			lock( queue )
				snapshot = queue.ToArray();

			foreach( var item in snapshot )
				yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
