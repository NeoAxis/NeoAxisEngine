using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class UniversalConstraint : Generic6DofConstraint
	{
		public UniversalConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, BVector3 anchor,
			BVector3 axis1, BVector3 axis2)
			: base(btUniversalConstraint_new(rigidBodyA.Native, rigidBodyB.Native,
				ref anchor, ref axis1, ref axis2))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = rigidBodyB;
		}

		public void SetLowerLimit(double ang1min, double ang2min)
		{
			btUniversalConstraint_setLowerLimit(Native, ang1min, ang2min);
		}

		public void SetUpperLimit(double ang1max, double ang2max)
		{
			btUniversalConstraint_setUpperLimit(Native, ang1max, ang2max);
		}

		public BVector3 Anchor
		{
			get
			{
				BVector3 value;
				btUniversalConstraint_getAnchor(Native, out value);
				return value;
			}
		}

		public BVector3 Anchor2
		{
			get
			{
				BVector3 value;
				btUniversalConstraint_getAnchor2(Native, out value);
				return value;
			}
		}

		public double Angle1 => btUniversalConstraint_getAngle1(Native);

		public double Angle2 => btUniversalConstraint_getAngle2(Native);

		public BVector3 Axis1
		{
			get
			{
				BVector3 value;
				btUniversalConstraint_getAxis1(Native, out value);
				return value;
			}
		}

		public BVector3 Axis2
		{
			get
			{
				BVector3 value;
				btUniversalConstraint_getAxis2(Native, out value);
				return value;
			}
		}
	}
}
