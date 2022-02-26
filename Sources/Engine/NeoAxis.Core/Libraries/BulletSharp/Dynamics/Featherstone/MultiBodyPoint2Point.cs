using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MultiBodyPoint2Point : MultiBodyConstraint
	{
		public MultiBodyPoint2Point(MultiBody body, int link, RigidBody bodyB, BVector3 pivotInA,
			BVector3 pivotInB)
			: base(btMultiBodyPoint2Point_new(body.Native, link, bodyB != null ? bodyB.Native : IntPtr.Zero,
				ref pivotInA, ref pivotInB), body, null)
		{
		}

		public MultiBodyPoint2Point(MultiBody bodyA, int linkA, MultiBody bodyB,
			int linkB, BVector3 pivotInA, BVector3 pivotInB)
			: base(btMultiBodyPoint2Point_new2(bodyA.Native, linkA, bodyB.Native,
				linkB, ref pivotInA, ref pivotInB), bodyA, bodyB)
		{
		}

		public BVector3 PivotInB
		{
			get
			{
				BVector3 value;
				btMultiBodyPoint2Point_getPivotInB(Native, out value);
				return value;
			}
			set => btMultiBodyPoint2Point_setPivotInB(Native, ref value);
		}
	}
}
