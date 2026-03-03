// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	public class ConcurrentLockManager<T>
	{
		readonly ConcurrentDictionary<T, object> locks = new ConcurrentDictionary<T, object>();

		/////////////////////

		class Releaser : IDisposable
		{
			ConcurrentDictionary<T, object> locks;
			T key;
			readonly object lockObject;

			//

			public Releaser( ConcurrentDictionary<T, object> locks, T key, object lockObject )
			{
				this.locks = locks;
				this.key = key;
				this.lockObject = lockObject;
			}

			public void Dispose()
			{
				//another threads still wait in Monitor.Enter, so we cannot remove unconditionally
				locks.TryRemove( key, out _ );

				Monitor.Exit( lockObject );
			}
		}

		/////////////////////

		public IDisposable LockDisposable( T key )
		{
			var lockObject = locks.GetOrAdd( key, _ => new object() );
			Monitor.Enter( lockObject );
			return new Releaser( locks, key, lockObject );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ConcurrentLockManagerAsync<T>
	{
		readonly ConcurrentDictionary<T, SemaphoreSlim> locks = new ConcurrentDictionary<T, SemaphoreSlim>();

		/////////////////////

		class Releaser : IDisposable
		{
			ConcurrentDictionary<T, SemaphoreSlim> locks;
			T key;
			readonly SemaphoreSlim lockObject;

			//

			public Releaser( ConcurrentDictionary<T, SemaphoreSlim> locks, T key, SemaphoreSlim lockObject )
			{
				this.locks = locks;
				this.key = key;
				this.lockObject = lockObject;
			}

			public void Dispose()
			{
				locks.TryRemove( key, out _ );

				lockObject.Release();
			}
		}

		/////////////////////

		public async Task<IDisposable> LockDisposableAsync( T key )
		{
			var lockObject = locks.GetOrAdd( key, _ => new SemaphoreSlim( 1, 1 ) );
			await lockObject.WaitAsync().ConfigureAwait( false );
			return new Releaser( locks, key, lockObject );
		}
	}
}
