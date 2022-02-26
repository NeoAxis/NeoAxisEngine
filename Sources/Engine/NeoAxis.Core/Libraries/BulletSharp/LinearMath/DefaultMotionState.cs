using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class DefaultMotionState : MotionState
	{
		public DefaultMotionState()
			: base(btDefaultMotionState_new())
		{
		}

		public DefaultMotionState(BMatrix startTrans)
			: base(btDefaultMotionState_new2(ref startTrans))
		{
		}

		public DefaultMotionState(BMatrix startTrans, BMatrix centerOfMassOffset)
			: base(btDefaultMotionState_new3(ref startTrans, ref centerOfMassOffset))
		{
		}

		public override void GetWorldTransform(out BMatrix worldTrans)
		{
			btMotionState_getWorldTransform(_native, out worldTrans);
		}

		public override void SetWorldTransform(ref BMatrix worldTrans)
		{
			btMotionState_setWorldTransform(_native, ref worldTrans);
		}

		public BMatrix CenterOfMassOffset
		{
			get
			{
				BMatrix value;
				btDefaultMotionState_getCenterOfMassOffset(_native, out value);
				return value;
			}
			set => btDefaultMotionState_setCenterOfMassOffset(_native, ref value);
		}

		public BMatrix GraphicsWorldTrans
		{
			get
			{
				BMatrix value;
				btDefaultMotionState_getGraphicsWorldTrans(_native, out value);
				return value;
			}
			set => btDefaultMotionState_setGraphicsWorldTrans(_native, ref value);
		}

		public BMatrix StartWorldTrans
		{
			get
			{
				BMatrix value;
				btDefaultMotionState_getStartWorldTrans(_native, out value);
				return value;
			}
			set => btDefaultMotionState_setStartWorldTrans(_native, ref value);
		}

		public IntPtr UserPointer
		{
			get => btDefaultMotionState_getUserPointer(_native);
			set => btDefaultMotionState_setUserPointer(_native, value);
		}
	}
}
