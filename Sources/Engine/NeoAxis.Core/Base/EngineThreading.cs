// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a structure for thread-safe storage of values of any type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ThreadSafeExchangeAny<T>
	{
		T value;
		//!!!!вроде как нельзя SpinLock, т.к. это структура и ThreadSafeExchangeAny структура
		//SpinLock spinLock;
		object lockObject;

		//

		public ThreadSafeExchangeAny( T value )
		{
			this.value = value;
			//spinLock = new SpinLock();
			lockObject = null;
			Interlocked.CompareExchange( ref this.lockObject, new object(), null );
		}

		public T Get()
		{
			//bool lockTaken = false;
			//try
			//{
			//	spinLock.Enter( ref lockTaken );
			//	return value;
			//}
			//finally
			//{
			//	if( lockTaken )
			//		spinLock.Exit();
			//}

			if( lockObject == null )
				Interlocked.CompareExchange( ref this.lockObject, new object(), null );
			lock( lockObject )
				return value;
		}

		public T Set( T value )
		{
			//bool lockTaken = false;
			//try
			//{
			//	spinLock.Enter( ref lockTaken );
			//	T oldValue = this.value;
			//	this.value = value;
			//	return oldValue;
			//}
			//finally
			//{
			//	if( lockTaken )
			//		spinLock.Exit();
			//}

			if( lockObject == null )
				Interlocked.CompareExchange( ref this.lockObject, new object(), null );
			lock( lockObject )
			{
				T oldValue = this.value;
				this.value = value;
				return oldValue;
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a structure for thread-safe storage of values of a class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ThreadSafeExchangeClass<T> where T : class
	{
		//!!!!good?
		volatile T value;

		//

		public ThreadSafeExchangeClass( T value )
		{
			this.value = value;
		}

		public T Get()
		{
			return value;
		}

		public T Set( T value )
		{
			return Interlocked.Exchange( ref this.value, value );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a structure for thread-safe storage of boolean value.
	/// </summary>
	public struct ThreadSafeExchangeBool
	{
		//!!!!good?
		volatile int value;

		//

		public ThreadSafeExchangeBool( bool value )
		{
			this.value = value ? 1 : 0;
		}

		public bool Get()
		{
			return value != 0;
		}

		public bool Set( bool value )
		{
			return Interlocked.Exchange( ref this.value, value ? 1 : 0 ) != 0;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a structure for class creation with thread-safe implemetntation of Dispose method.
	/// </summary>
	public abstract class ThreadSafeDisposable : IDisposable
	{
		ThreadSafeExchangeBool disposed;

		//!!!!DisposedEvent?

		//

		public bool Disposed
		{
			get { return disposed.Get(); }
		}

		public void Dispose()
		{
			if( !disposed.Set( true ) )
				OnDispose();
		}

		protected abstract void OnDispose();
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Class for multi-threading in the engine.
	/// </summary>
	public static class EngineThreading
	{
		class QueuedActionItem
		{
			//public Delegate actionGeneric;
			//public object actionGenericParam;
			public Delegate action;
			//public int paramCount;
			public object[] parameters;
			volatile public bool executed;
			public object result;
		}
		static ConcurrentQueue<QueuedActionItem> queuedActiosToExecuteFromMainThread = new ConcurrentQueue<QueuedActionItem>();

		//

		public static void CheckMainThread()
		{
			if( VirtualFileSystem.MainThread != Thread.CurrentThread )
				Log.Fatal( "Prohibited call from not app main thread." );
		}

		public static void ExecuteQueuedActionsFromMainThread()
		{
			CheckMainThread();

			QueuedActionItem item;
			while( queuedActiosToExecuteFromMainThread.TryDequeue( out item ) )
			{
				//!!!!try?
				try
				{
					//!!!!slow
					item.result = item.action.DynamicInvoke( item.parameters );

					////!!!!slow?
					//if( item.actionGeneric != null )
					//	item.actionGeneric.DynamicInvoke( item.actionGenericParam );
					//else if( item.action != null )
					//	item.action.DynamicInvoke();
					//if( item.actionGeneric != null )
					//	item.actionGeneric.Method.Invoke( null, new object[] { item.actionGenericParam } );
					//else if( item.action != null )
					//	item.action.Method.Invoke( null, new object[ 0 ] );

				}
				finally
				{
					item.executed = true;
				}
			}
		}





		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2, T3, T4>( Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = action;
			item.parameters = new object[] { param1, param2, param3, param4 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2, T3>( Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = action;
			item.parameters = new object[] { param1, param2, param3 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2>( Action<T1, T2> action, T1 param1, T2 param2 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = action;
			item.parameters = new object[] { param1, param2 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T>( Action<T> action, T param )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = action;
			item.parameters = new object[] { param };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal( Action action )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = action;
			item.parameters = new object[ 0 ];
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2, T3, T4, TResult>( Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = function;
			item.parameters = new object[] { param1, param2, param3, param4 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2, T3, TResult>( Func<T1, T2, T3, TResult> function, T1 param1, T2 param2, T3 param3 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = function;
			item.parameters = new object[] { param1, param2, param3 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T1, T2, TResult>( Func<T1, T2, TResult> function, T1 param1, T2 param2 )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = function;
			item.parameters = new object[] { param1, param2 };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<T, TResult>( Func<T, TResult> function, T param )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = function;
			item.parameters = new object[] { param };
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}

		static QueuedActionItem ExecuteFromMainThreadLater_Internal<TResult>( Func<TResult> function )
		{
			QueuedActionItem item = new QueuedActionItem();
			item.action = function;
			item.parameters = new object[ 0 ];
			queuedActiosToExecuteFromMainThread.Enqueue( item );
			return item;
		}





		public static void ExecuteFromMainThreadLater<T1, T2, T3, T4>( Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2, param3, param4 );
			else
				ExecuteFromMainThreadLater_Internal( action, param1, param2, param3, param4 );
		}

		public static void ExecuteFromMainThreadLater<T1, T2, T3>( Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2, param3 );
			else
				ExecuteFromMainThreadLater_Internal( action, param1, param2, param3 );
		}

		public static void ExecuteFromMainThreadLater<T1, T2>( Action<T1, T2> action, T1 param1, T2 param2 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2 );
			else
				ExecuteFromMainThreadLater_Internal( action, param1, param2 );
		}

		public static void ExecuteFromMainThreadLater<T>( Action<T> action, T param )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param );
			else
				ExecuteFromMainThreadLater_Internal( action, param );
		}

		public static void ExecuteFromMainThreadLater( Action action )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action();
			else
				ExecuteFromMainThreadLater_Internal( action );
		}





		static void WaitItemExecution( QueuedActionItem item )
		{
			//!!!!!так?
			while( !item.executed )
				Thread.Sleep( 0 );
		}

		public static void ExecuteFromMainThreadWait<T1, T2, T3, T4>( Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2, param3, param4 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( action, param1, param2, param3, param4 );
				WaitItemExecution( item );
			}
		}

		public static void ExecuteFromMainThreadWait<T1, T2, T3>( Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2, param3 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( action, param1, param2, param3 );
				WaitItemExecution( item );
			}
		}

		public static void ExecuteFromMainThreadWait<T1, T2>( Action<T1, T2> action, T1 param1, T2 param2 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param1, param2 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( action, param1, param2 );
				WaitItemExecution( item );
			}
		}

		public static void ExecuteFromMainThreadWait<T>( Action<T> action, T param )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action( param );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( action, param );
				WaitItemExecution( item );
			}
		}

		public static void ExecuteFromMainThreadWait( Action action )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				action();
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( action );
				WaitItemExecution( item );
			}
		}

		public static TResult ExecuteFromMainThreadWait<T1, T2, T3, T4, TResult>( Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				return function( param1, param2, param3, param4 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( function, param1, param2, param3, param4 );
				WaitItemExecution( item );
				return (TResult)item.result;
			}
		}

		public static TResult ExecuteFromMainThreadWait<T1, T2, T3, TResult>( Func<T1, T2, T3, TResult> function, T1 param1, T2 param2, T3 param3 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				return function( param1, param2, param3 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( function, param1, param2, param3 );
				WaitItemExecution( item );
				return (TResult)item.result;
			}
		}

		public static TResult ExecuteFromMainThreadWait<T1, T2, TResult>( Func<T1, T2, TResult> function, T1 param1, T2 param2 )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				return function( param1, param2 );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( function, param1, param2 );
				WaitItemExecution( item );
				return (TResult)item.result;
			}
		}

		public static TResult ExecuteFromMainThreadWait<T, TResult>( Func<T, TResult> function, T param )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				return function( param );
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( function, param );
				WaitItemExecution( item );
				return (TResult)item.result;
			}
		}

		public static TResult ExecuteFromMainThreadWait<TResult>( Func<TResult> function )
		{
			if( VirtualFileSystem.MainThread == Thread.CurrentThread )
				return function();
			else
			{
				var item = ExecuteFromMainThreadLater_Internal( function );
				WaitItemExecution( item );
				return (TResult)item.result;
			}
		}





		//!!!!тут? может лучше внутрь RootComponentData?

		public static void LockComponentHierarchy( Component rootComponent )
		{
			if( rootComponent.HierarchyController == null )
				Log.Fatal( "EngineThreading: LockComponentHierarchy: component.RootComponentData == null." );
			Monitor.Enter( rootComponent.HierarchyController.LockObjectHierarchy );
		}

		public static bool TryLockComponentHierarchy( Component rootComponent, int millisecondsTimeout = 0 )
		{
			if( rootComponent.HierarchyController == null )
				Log.Fatal( "EngineThreading: TryLockComponentHierarchy: component.RootComponentData == null." );

			if( millisecondsTimeout == 0 )
				return Monitor.TryEnter( rootComponent.HierarchyController.LockObjectHierarchy );
			else
				return Monitor.TryEnter( rootComponent.HierarchyController.LockObjectHierarchy, millisecondsTimeout );
		}

		public static void UnlockComponentHierarchy( Component rootComponent )
		{
			if( rootComponent.HierarchyController == null )
				Log.Fatal( "EngineThreading: UnlockComponentHierarchy: component.RootComponentData == null." );
			Monitor.Exit( rootComponent.HierarchyController.LockObjectHierarchy );
		}
	}
}
