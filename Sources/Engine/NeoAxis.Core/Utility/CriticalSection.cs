// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using System.Threading;

namespace NeoAxis
{
	//!!!!old

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class CriticalSection : IDisposable
	{
		public static CriticalSection Create()
		{

			return new DefaultCriticalSection();


			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
			//	return new MacOSXCriticalSection();
			//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			//	return new Win32CriticalSection();
			//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			//	return new Win32CriticalSection();
			////else if( PlatformInfo.Platform == PlatformInfo.Platforms.Android )
			////   return new AndroidCriticalSection();
			//else
			//{
			//	Log.Fatal( "CriticalSection: Unknown platform." );
			//	return null;
			//}
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize( this );
		}
		public abstract void Enter();
		public abstract void Leave();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class DefaultCriticalSection : CriticalSection
	{
		Mutex mutex;

		//

		public DefaultCriticalSection()
		{
			mutex = new Mutex();
		}

		public override void Dispose()
		{
			mutex.Close();
			base.Dispose();
		}

		public override void Enter()
		{
			mutex.WaitOne();
		}

		public override void Leave()
		{
			mutex.ReleaseMutex();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//class Win32CriticalSection : CriticalSection
	//{
	//	unsafe CRITICAL_SECTION* criticalSection;

	//	///////////////////////////////////////////

	//	[StructLayout( LayoutKind.Sequential )]
	//	struct CRITICAL_SECTION
	//	{
	//		IntPtr/*PRTL_CRITICAL_SECTION_DEBUG*/ DebugInfo;
	//		int LockCount;
	//		int RecursionCount;
	//		IntPtr/*HANDLE*/ OwningThread;
	//		IntPtr/*HANDLE*/ LockSemaphore;
	//		IntPtr/*ULONG_PTR*/ SpinCount;
	//	}

	//	///////////////////////////////////////////

	//	[DllImport( "kernel32.dll" )]
	//	unsafe static extern void InitializeCriticalSection( void*/*CRITICAL_SECTION*/ section );
	//	[DllImport( "kernel32.dll" )]
	//	unsafe static extern void DeleteCriticalSection( void*/*CRITICAL_SECTION*/ section );
	//	[DllImport( "kernel32.dll" )]
	//	unsafe static extern void EnterCriticalSection( void*/*CRITICAL_SECTION*/ section );
	//	[DllImport( "kernel32.dll" )]
	//	unsafe static extern void LeaveCriticalSection( void*/*CRITICAL_SECTION*/ section );

	//	//

	//	public Win32CriticalSection()
	//	{
	//		unsafe
	//		{
	//			criticalSection = (CRITICAL_SECTION*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utils, sizeof( CRITICAL_SECTION ) );
	//			InitializeCriticalSection( criticalSection );
	//		}
	//	}

	//	public override void Dispose()
	//	{
	//		unsafe
	//		{
	//			if( criticalSection != null )
	//			{
	//				DeleteCriticalSection( criticalSection );
	//				NativeUtility.Free( (IntPtr)criticalSection );
	//				criticalSection = null;
	//			}
	//		}
	//		base.Dispose();
	//	}

	//	public override void Enter()
	//	{
	//		unsafe
	//		{
	//			EnterCriticalSection( criticalSection );
	//		}
	//	}

	//	public override void Leave()
	//	{
	//		unsafe
	//		{
	//			LeaveCriticalSection( criticalSection );
	//		}
	//	}
	//}

	//////////////////////////////////////////////////////////////////////////////////////////////////

	//class MacOSXCriticalSection : CriticalSection
	//{
	//	IntPtr mutex;

	//	//

	//	//!!!!name. не было .dll
	//	const string library = "NeoAxisCoreNative";

	//	[DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_init" )]
	//	unsafe static extern IntPtr pthread_mutex_init();

	//	[DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_destroy" )]
	//	unsafe static extern void pthread_mutex_destroy( IntPtr mutex );

	//	[DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_lock" )]
	//	[return: MarshalAs( UnmanagedType.U1 )]
	//	unsafe static extern bool pthread_mutex_lock( IntPtr mutex );

	//	[DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_unlock" )]
	//	[return: MarshalAs( UnmanagedType.U1 )]
	//	unsafe static extern bool pthread_mutex_unlock( IntPtr mutex );

	//	//

	//	public MacOSXCriticalSection()
	//	{
	//		mutex = pthread_mutex_init();
	//		if( mutex == IntPtr.Zero )
	//			Log.Warning( "MacOSXCriticalSection: Initialization failed." );
	//	}

	//	public override void Dispose()
	//	{
	//		if( mutex != IntPtr.Zero )
	//		{
	//			pthread_mutex_destroy( mutex );
	//			mutex = IntPtr.Zero;
	//		}

	//		base.Dispose();
	//	}

	//	public override void Enter()
	//	{
	//		if( !pthread_mutex_lock( mutex ) )
	//			Log.Warning( "MacOSXCriticalSection: Locking failed." );
	//	}

	//	public override void Leave()
	//	{
	//		if( !pthread_mutex_unlock( mutex ) )
	//			Log.Warning( "MacOSXCriticalSection: Unlocking failed." );
	//	}
	//}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//class AndroidCriticalSection : CriticalSection
	//{
	//   IntPtr mutex;

	//   //

	//   const string library = "NeoAxisCoreNative";

	//   [DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_init" )]
	//   unsafe static extern IntPtr pthread_mutex_init();

	//   [DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_destroy" )]
	//   unsafe static extern void pthread_mutex_destroy( IntPtr mutex );

	//   [DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_lock" )]
	//   [return: MarshalAs( UnmanagedType.U1 )]
	//   unsafe static extern bool pthread_mutex_lock( IntPtr mutex );

	//   [DllImport( library, EntryPoint = "UtilsNativeWrapper_pthread_mutex_unlock" )]
	//   [return: MarshalAs( UnmanagedType.U1 )]
	//   unsafe static extern bool pthread_mutex_unlock( IntPtr mutex );

	//   //

	//   public AndroidCriticalSection()
	//   {
	//      mutex = pthread_mutex_init();
	//      if( mutex == IntPtr.Zero )
	//         Log.Warning( "AndroidCriticalSection: Initialization failed." );
	//   }

	//   public override void Dispose()
	//   {
	//      if( mutex != IntPtr.Zero )
	//      {
	//         pthread_mutex_destroy( mutex );
	//         mutex = IntPtr.Zero;
	//      }

	//      base.Dispose();
	//   }

	//   public override void Enter()
	//   {
	//      if( !pthread_mutex_lock( mutex ) )
	//         Log.Warning( "AndroidCriticalSection: Locking failed." );
	//   }

	//   public override void Leave()
	//   {
	//      if( !pthread_mutex_unlock( mutex ) )
	//         Log.Warning( "AndroidCriticalSection: Unlocking failed." );
	//   }
	//}

	////////////////////////////////////////////////////////////////////////////////////////////////

}
