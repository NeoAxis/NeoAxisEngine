using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class Hinge2Constraint : Generic6DofSpring2Constraint
	{
		public Hinge2Constraint(RigidBody rigidBodyA, RigidBody rigidBodyB, BVector3 anchor,
			BVector3 axis1, BVector3 axis2)
			: base(btHinge2Constraint_new(rigidBodyA.Native, rigidBodyB.Native,
				ref anchor, ref axis1, ref axis2))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = rigidBodyB;
		}

		public void SetLowerLimit(double ang1min)
		{
			btHinge2Constraint_setLowerLimit(Native, ang1min);
		}

		public void SetUpperLimit(double ang1max)
		{
			btHinge2Constraint_setUpperLimit(Native, ang1max);
		}

		public BVector3 Anchor
		{
			get
			{
				BVector3 value;
				btHinge2Constraint_getAnchor(Native, out value);
				return value;
			}
		}

		public BVector3 Anchor2
		{
			get
			{
				BVector3 value;
				btHinge2Constraint_getAnchor2(Native, out value);
				return value;
			}
		}

		public double Angle1 => btHinge2Constraint_getAngle1(Native);

		public double Angle2 => btHinge2Constraint_getAngle2(Native);

		public BVector3 Axis1
		{
			get
			{
				BVector3 value;
				btHinge2Constraint_getAxis1(Native, out value);
				return value;
			}
		}

		public BVector3 Axis2
		{
			get
			{
				BVector3 value;
				btHinge2Constraint_getAxis2(Native, out value);
				return value;
			}
		}
	}
}
