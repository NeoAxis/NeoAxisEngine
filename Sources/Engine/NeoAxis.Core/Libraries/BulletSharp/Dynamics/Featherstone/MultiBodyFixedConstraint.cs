using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MultiBodyFixedConstraint : MultiBodyConstraint
	{
		public MultiBodyFixedConstraint(MultiBody body, int link, RigidBody bodyB,
			BVector3 pivotInA, BVector3 pivotInB, BMatrix frameInA, BMatrix frameInB)
			: base(btMultiBodyFixedConstraint_new(body.Native, link, bodyB.Native,
				ref pivotInA, ref pivotInB, ref frameInA, ref frameInB), body, null)
		{
		}

		public MultiBodyFixedConstraint(MultiBody bodyA, int linkA, MultiBody bodyB,
			int linkB, BVector3 pivotInA, BVector3 pivotInB, BMatrix frameInA, BMatrix frameInB)
			: base(btMultiBodyFixedConstraint_new2(bodyA.Native, linkA, bodyB.Native,
				linkB, ref pivotInA, ref pivotInB, ref frameInA, ref frameInB), bodyA, bodyB)
		{
		}

		public BMatrix FrameInA
		{
			get
			{
				BMatrix value;
				btMultiBodyFixedConstraint_getFrameInA(Native, out value);
				return value;
			}
			set => btMultiBodyFixedConstraint_setFrameInA(Native, ref value);
		}

		public BMatrix FrameInB
		{
			get
			{
				BMatrix value;
				btMultiBodyFixedConstraint_getFrameInB(Native, out value);
				return value;
			}
			set => btMultiBodyFixedConstraint_setFrameInB(Native, ref value);
		}

		public BVector3 PivotInA
		{
			get
			{
				BVector3 value;
				btMultiBodyFixedConstraint_getPivotInA(Native, out value);
				return value;
			}
			set => btMultiBodyFixedConstraint_setPivotInA(Native, ref value);
		}

		public BVector3 PivotInB
		{
			get
			{
				BVector3 value;
				btMultiBodyFixedConstraint_getPivotInB(Native, out value);
				return value;
			}
			set => btMultiBodyFixedConstraint_setPivotInB(Native, ref value);
		}
	}
}
