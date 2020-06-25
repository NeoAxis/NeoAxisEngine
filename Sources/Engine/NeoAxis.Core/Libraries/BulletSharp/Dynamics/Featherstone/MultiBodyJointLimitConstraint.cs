using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public class MultiBodyJointLimitConstraint : MultiBodyConstraint
	{
		public MultiBodyJointLimitConstraint(MultiBody body, int link, double lower,
			double upper)
			: base(btMultiBodyJointLimitConstraint_new(body.Native, link, lower,
				upper), body, body)
		{
		}
	}
}
