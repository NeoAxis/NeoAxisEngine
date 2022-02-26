using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class BroadphaseAabbCallback : IDisposable
	{
		internal IntPtr Native;

		[UnmanagedFunctionPointer(Internal.BulletSharp.Native.Conv), SuppressUnmanagedCodeSecurity]
		internal delegate bool ProcessUnmanagedDelegate(IntPtr proxy);

		internal ProcessUnmanagedDelegate _process;

		internal BroadphaseAabbCallback(IntPtr native)
		{
			Native = native;
			_process = ProcessUnmanaged;
		}

		protected BroadphaseAabbCallback()
		{
			_process = ProcessUnmanaged;
			Native = btBroadphaseAabbCallbackWrapper_new(
				Marshal.GetFunctionPointerForDelegate(_process));
		}

		private bool ProcessUnmanaged(IntPtr proxy)
		{
			return Process(BroadphaseProxy.GetManaged(proxy));
		}

		public abstract bool Process(BroadphaseProxy proxy);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				btBroadphaseAabbCallback_delete(Native);
				Native = IntPtr.Zero;
			}
		}

		~BroadphaseAabbCallback()
		{
			Dispose(false);
		}
	}

	public abstract class BroadphaseRayCallback : BroadphaseAabbCallback
	{
		private UIntArray _signs;

		protected BroadphaseRayCallback()
			: base(IntPtr.Zero)
		{
			Native = btBroadphaseRayCallbackWrapper_new(
				Marshal.GetFunctionPointerForDelegate(_process));
		}

		public double LambdaMax
		{
			get => btBroadphaseRayCallback_getLambda_max(Native);
			set => btBroadphaseRayCallback_setLambda_max(Native, value);
		}

		public BVector3 RayDirectionInverse
		{
			get
			{
				BVector3 value;
				btBroadphaseRayCallback_getRayDirectionInverse(Native, out value);
				return value;
			}
			set => btBroadphaseRayCallback_setRayDirectionInverse(Native, ref value);
		}

		public UIntArray Signs
		{
			get
			{
				if (_signs == null)
				{
					_signs = new UIntArray(btBroadphaseRayCallback_getSigns(Native), 3);
				}
				return _signs;
			}
		}
	}

	public abstract class BroadphaseInterface : IDisposable
	{
		internal IntPtr Native;

		protected OverlappingPairCache _overlappingPairCache;
		internal List<CollisionWorld> _worldRefs = new List<CollisionWorld>(1);
		internal bool _worldDeferredCleanup;

		internal BroadphaseInterface(IntPtr native)
		{
			Native = native;
		}

		public void AabbTestRef(ref BVector3 aabbMin, ref BVector3 aabbMax, BroadphaseAabbCallback callback)
		{
			btBroadphaseInterface_aabbTest(Native, ref aabbMin, ref aabbMax, callback.Native);
		}

		public void AabbTest(BVector3 aabbMin, BVector3 aabbMax, BroadphaseAabbCallback callback)
		{
			btBroadphaseInterface_aabbTest(Native, ref aabbMin, ref aabbMax, callback.Native);
		}

		public void CalculateOverlappingPairs(Dispatcher dispatcher)
		{
			btBroadphaseInterface_calculateOverlappingPairs(Native, dispatcher.Native);
		}

		public abstract BroadphaseProxy CreateProxy(ref BVector3 aabbMin, ref BVector3 aabbMax,
			int shapeType, IntPtr userPtr, int collisionFilterGroup, int collisionFilterMask,
			Dispatcher dispatcher);

		public void DestroyProxy(BroadphaseProxy proxy, Dispatcher dispatcher)
		{
			btBroadphaseInterface_destroyProxy(Native, proxy.Native, dispatcher.Native);
		}

		public void GetAabb(BroadphaseProxy proxy, out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btBroadphaseInterface_getAabb(Native, proxy.Native, out aabbMin, out aabbMax);
		}

		public void GetBroadphaseAabb(out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btBroadphaseInterface_getBroadphaseAabb(Native, out aabbMin, out aabbMax);
		}

		public void PrintStats()
		{
			btBroadphaseInterface_printStats(Native);
		}

		public void RayTestRef(ref BVector3 rayFrom, ref BVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			btBroadphaseInterface_rayTest(Native, ref rayFrom, ref rayTo, rayCallback.Native);
		}

		public void RayTest(BVector3 rayFrom, BVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			btBroadphaseInterface_rayTest(Native, ref rayFrom, ref rayTo, rayCallback.Native);
		}

		public void RayTestRef(ref BVector3 rayFrom, ref BVector3 rayTo, BroadphaseRayCallback rayCallback, ref BVector3 aabbMin, ref BVector3 aabbMax)
		{
			btBroadphaseInterface_rayTest3(Native, ref rayFrom, ref rayTo, rayCallback.Native, ref aabbMin, ref aabbMax);
		}

		public void RayTest(BVector3 rayFrom, BVector3 rayTo, BroadphaseRayCallback rayCallback,
			BVector3 aabbMin, BVector3 aabbMax)
		{
			btBroadphaseInterface_rayTest3(Native, ref rayFrom, ref rayTo, rayCallback.Native,
				ref aabbMin, ref aabbMax);
		}

		public void ResetPool(Dispatcher dispatcher)
		{
			btBroadphaseInterface_resetPool(Native, dispatcher.Native);
		}

		public void SetAabbRef(BroadphaseProxy proxy, ref BVector3 aabbMin, ref BVector3 aabbMax, Dispatcher dispatcher)
		{
			btBroadphaseInterface_setAabb(Native, proxy.Native, ref aabbMin, ref aabbMax, dispatcher.Native);
		}

		public void SetAabb(BroadphaseProxy proxy, BVector3 aabbMin, BVector3 aabbMax,
			Dispatcher dispatcher)
		{
			btBroadphaseInterface_setAabb(Native, proxy.Native, ref aabbMin, ref aabbMax,
				dispatcher.Native);
		}

		public OverlappingPairCache OverlappingPairCache => _overlappingPairCache;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				if (_worldRefs.Count == 0)
				{
					btBroadphaseInterface_delete(Native);
					Native = IntPtr.Zero;
				}
				else
				{
					// Can't delete broadphase, because it is referenced by a world,
					// tell the world to clean up the broadphase later.
					_worldDeferredCleanup = true;
				}
			}
		}

		~BroadphaseInterface()
		{
			Dispose(false);
		}
	}
}
