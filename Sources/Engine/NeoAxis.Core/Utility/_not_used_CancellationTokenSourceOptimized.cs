//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using NeoAxis.CloudServer;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace NeoAxis
//{
//	static class CancellationTokenSourceOptimizedManager
//	{
//		static readonly ConcurrentDictionary<CancellationTokenSourceOptimized, int> sources;
//		static int taskStarted;
//		static DateTime currentTime;

//		//

//		static CancellationTokenSourceOptimizedManager()
//		{
//			sources = new ConcurrentDictionary<CancellationTokenSourceOptimized, int>();
//			currentTime = DateTime.UtcNow;
//		}

//		public static void AddSource( CancellationTokenSourceOptimized source, TimeSpan delay, out DateTime endTime )
//		{
//			//this additional checking code to prevent double task running if exception will be thrown
//			if( Volatile.Read( ref taskStarted ) == 0 )
//			{
//				currentTime = DateTime.UtcNow;
//				Task.Run( UpdateAsync );
//			}

//			sources[ source ] = 1;
//			endTime = currentTime + delay;
//		}

//		public static void RemoveSource( CancellationTokenSourceOptimized source )
//		{
//			sources.TryRemove( source, out _ );
//		}

//		static async Task UpdateAsync()
//		{
//			if( Interlocked.Exchange( ref taskStarted, 1 ) != 0 )
//				return;

//			while( true )
//			{
//				//update current time
//				currentTime = DateTime.UtcNow;

//				//cancel ended sources
//				foreach( var source in sources.Keys )
//				{
//					if( currentTime > source.EndTime )
//					{
//						try
//						{
//							source.Cancel();
//						}
//						catch( ObjectDisposedException )
//						{
//						}

//						sources.TryRemove( source, out _ );
//					}
//				}

//				await Task.Delay( 100 ).ConfigureAwait( false );
//			}
//		}
//	}

//	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	public sealed class CancellationTokenSourceOptimized : IDisposable
//	{
//		readonly CancellationTokenSource cts;
//		readonly DateTime endTime;
//		int disposed;

//		//

//		public CancellationTokenSourceOptimized( TimeSpan delay )
//		{
//			cts = new CancellationTokenSource();

//			if( delay <= TimeSpan.Zero )
//				throw new ArgumentOutOfRangeException( nameof( delay ) );

//			CancellationTokenSourceOptimizedManager.AddSource( this, delay, out endTime );
//		}

//		public CancellationTokenSourceOptimized( int hours, int minutes, int seconds )
//			: this( new TimeSpan( hours, minutes, seconds ) )
//		{
//		}

//		public CancellationToken Token
//		{
//			get
//			{
//				ThrowIfDisposed();
//				return cts.Token;
//			}
//		}

//		public void Cancel()
//		{
//			if( Volatile.Read( ref disposed ) != 0 )
//				return;

//			try
//			{
//				cts.Cancel();
//			}
//			catch( ObjectDisposedException )
//			{
//			}
//		}

//		public void Dispose()
//		{
//			if( Interlocked.Exchange( ref disposed, 1 ) != 0 )
//				return;

//			CancellationTokenSourceOptimizedManager.RemoveSource( this );
//			cts.Dispose();
//		}

//		void ThrowIfDisposed()
//		{
//			if( Volatile.Read( ref disposed ) != 0 )
//				throw new ObjectDisposedException( GetType().Name );
//		}

//		public DateTime EndTime
//		{
//			get { return endTime; }
//		}
//	}

//	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	public class CancellationTokenSourceOptimizedTests
//	{
//		static async Task SimulatedWorkAsync( CancellationToken token )
//		{
//			//small CPU work
//			double x = 0;
//			for( int i = 0; i < 10; i++ )
//				x += Math.Sqrt( i + 1 );

//			await Task.Delay( 1, token ).ConfigureAwait( false );
//		}

//		static long MeasureMs( Action action )
//		{
//			var sw = System.Diagnostics.Stopwatch.StartNew();
//			action();
//			sw.Stop();
//			return sw.ElapsedMilliseconds;
//		}

//		public static async Task TestAsync()
//		{
//			const int iterations = 1000;
//			var perIterationTimeout = TimeSpan.FromSeconds( 1 );

//			//warmup
//			{
//				using var cts = new CancellationTokenSource( perIterationTimeout );
//				try { await SimulatedWorkAsync( cts.Token ).ConfigureAwait( false ); }
//				catch( OperationCanceledException ) { }
//			}
//			{
//				using var ctsOpt = new CancellationTokenSourceOptimized( perIterationTimeout );
//				try { await SimulatedWorkAsync( ctsOpt.Token ).ConfigureAwait( false ); }
//				catch( OperationCanceledException ) { }
//			}

//			//measure standard CTS creation/use
//			int canceled1 = 0;
//			int completed1 = 0;
//			long elapsedCts = MeasureMs( () =>
//			{
//				for( int i = 0; i < iterations; i++ )
//				{
//					using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 5 ) );
//					try
//					{
//						SimulatedWorkAsync( cts.Token ).GetAwaiter().GetResult();
//						completed1++;
//					}
//					catch( OperationCanceledException )
//					{
//						canceled1++;
//					}
//				}
//			} );

//			//measure optimized CTS creation/use
//			int canceled2 = 0;
//			int completed2 = 0;
//			long elapsedOpt = MeasureMs( () =>
//			{
//				for( int i = 0; i < iterations; i++ )
//				{
//					using var cts = new CancellationTokenSourceOptimized( TimeSpan.FromSeconds( 5 ) );
//					try
//					{
//						SimulatedWorkAsync( cts.Token ).GetAwaiter().GetResult(); 
//						completed2++;
//					}
//					catch( OperationCanceledException )
//					{
//						canceled2++;
//					}
//				}
//			} );

//			Log.Info( $"CancellationTokenSourceOptimizedTests: iterations={iterations}" );
//			Log.Info( $"CancellationTokenSource: elapsed={elapsedCts} ms, completed={completed1}, canceled={canceled1}" );
//			Log.Info( $"CancellationTokenSourceOptimized: elapsed={elapsedOpt} ms, completed={completed2}, canceled={canceled2}" );
//			Log.Info( "CancellationTokenSourceOptimizedTests: Test finished." );
//		}
//	}

//}
