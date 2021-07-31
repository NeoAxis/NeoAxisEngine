using BulletSharp.Math;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public class UniversalConstraint : Generic6DofConstraint
	{
		public UniversalConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, Vector3 anchor,
			Vector3 axis1, Vector3 axis2)
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

		public Vector3 Anchor
		{
			get
			{
				Vector3 value;
				btUniversalConstraint_getAnchor(Native, out value);
				return value;
			}
		}

		public Vector3 Anchor2
		{
			get
			{
				Vector3 value;
				btUniversalConstraint_getAnchor2(Native, out value);
				return value;
			}
		}

		public double Angle1 => btUniversalConstraint_getAngle1(Native);

		public double Angle2 => btUniversalConstraint_getAngle2(Native);

		public Vector3 Axis1
		{
			get
			{
				Vector3 value;
				btUniversalConstraint_getAxis1(Native, out value);
				return value;
			}
		}

		public Vector3 Axis2
		{
			get
			{
				Vector3 value;
				btUniversalConstraint_getAxis2(Native, out value);
				return value;
			}
		}
	}
}
