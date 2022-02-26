using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MultiBodySliderConstraint : MultiBodyConstraint
	{
		public MultiBodySliderConstraint(MultiBody body, int link, RigidBody bodyB,
			BVector3 pivotInA, BVector3 pivotInB, BMatrix frameInA, BMatrix frameInB, BVector3 jointAxis)
			: base(btMultiBodySliderConstraint_new(body.Native, link, bodyB.Native,
				ref pivotInA, ref pivotInB, ref frameInA, ref frameInB, ref jointAxis), body, null)
		{
		}

		public MultiBodySliderConstraint(MultiBody bodyA, int linkA, MultiBody bodyB,
			int linkB, BVector3 pivotInA, BVector3 pivotInB, BMatrix frameInA, BMatrix frameInB,
			BVector3 jointAxis)
			: base(btMultiBodySliderConstraint_new2(bodyA.Native, linkA, bodyB.Native,
				linkB, ref pivotInA, ref pivotInB, ref frameInA, ref frameInB, ref jointAxis), bodyA, bodyB)
		{
		}

		public BMatrix FrameInA
		{
			get
			{
				BMatrix value;
				btMultiBodySliderConstraint_getFrameInA(Native, out value);
				return value;
			}
			set => btMultiBodySliderConstraint_setFrameInA(Native, ref value);
		}

		public BMatrix FrameInB
		{
			get
			{
				BMatrix value;
				btMultiBodySliderConstraint_getFrameInB(Native, out value);
				return value;
			}
			set => btMultiBodySliderConstraint_setFrameInB(Native, ref value);
		}

		public BVector3 JointAxis
		{
			get
			{
				BVector3 value;
				btMultiBodySliderConstraint_getJointAxis(Native, out value);
				return value;
			}
			set => btMultiBodySliderConstraint_setJointAxis(Native, ref value);
		}

		public BVector3 PivotInA
		{
			get
			{
				BVector3 value;
				btMultiBodySliderConstraint_getPivotInA(Native, out value);
				return value;
			}
			set => btMultiBodySliderConstraint_setPivotInA(Native, ref value);
		}

		public BVector3 PivotInB
		{
			get
			{
				BVector3 value;
				btMultiBodySliderConstraint_getPivotInB(Native, out value);
				return value;
			}
			set => btMultiBodySliderConstraint_setPivotInB(Native, ref value);
		}
	}
}
