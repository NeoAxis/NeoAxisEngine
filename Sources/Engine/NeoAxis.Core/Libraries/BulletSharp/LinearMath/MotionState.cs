using System;
using System.Runtime.InteropServices;
using System.Security;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class MotionState : IDisposable
	{
		internal IntPtr _native;

		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate void GetWorldTransformUnmanagedDelegate(out BMatrix worldTrans);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate void SetWorldTransformUnmanagedDelegate(ref BMatrix worldTrans);

		private GetWorldTransformUnmanagedDelegate _getWorldTransform;
		private SetWorldTransformUnmanagedDelegate _setWorldTransform;

		internal MotionState(IntPtr native)
		{
			_native = native;
		}

		protected MotionState()
		{
			_getWorldTransform = new GetWorldTransformUnmanagedDelegate(GetWorldTransformUnmanaged);
			_setWorldTransform = new SetWorldTransformUnmanagedDelegate(SetWorldTransformUnmanaged);

			_native = btMotionStateWrapper_new(
				Marshal.GetFunctionPointerForDelegate(_getWorldTransform),
				Marshal.GetFunctionPointerForDelegate(_setWorldTransform));
		}

		void GetWorldTransformUnmanaged(out BMatrix worldTrans)
		{
			GetWorldTransform(out worldTrans);
		}

		void SetWorldTransformUnmanaged(ref BMatrix worldTrans)
		{
			SetWorldTransform(ref worldTrans);
		}

		public abstract void GetWorldTransform(out BMatrix worldTrans);
		public abstract void SetWorldTransform(ref BMatrix worldTrans);

		public BMatrix WorldTransform
		{
			get
			{
				BMatrix transform;
				GetWorldTransform(out transform);
				return transform;
			}
			set => SetWorldTransform(ref value);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				btMotionState_delete(_native);
				_native = IntPtr.Zero;
			}
		}

		~MotionState()
		{
			Dispose(false);
		}
	}
}
