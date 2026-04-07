// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	public sealed class AsyncAutoResetEvent : IDisposable
	{
		readonly SemaphoreSlim semaphore = new SemaphoreSlim( 0, 1 );
		volatile bool disposed;

		//

		public Task WaitAsync( CancellationToken cancellationToken = default )
		{
			if( disposed )
				return Task.FromCanceled( cancellationToken.IsCancellationRequested ? cancellationToken : new CancellationToken( true ) );

			try
			{
				return semaphore.WaitAsync( cancellationToken );
			}
			catch( ObjectDisposedException )
			{
				// Dispose race: behave the same as the fast-path above (canceled).
				return Task.FromCanceled( cancellationToken.IsCancellationRequested ? cancellationToken : new CancellationToken( true ) );
			}
		}

		public void Set()
		{
			if( disposed )
				return;

			try
			{
				semaphore.Release();
			}
			catch( SemaphoreFullException )
			{
			}
			catch( ObjectDisposedException )
			{
			}
		}

		public void Dispose()
		{
			if( disposed )
				return;
			disposed = true;
			semaphore.Dispose();
		}
	}
}