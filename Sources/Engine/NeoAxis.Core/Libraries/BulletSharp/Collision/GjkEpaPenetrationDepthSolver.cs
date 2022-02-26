using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class GjkEpaPenetrationDepthSolver : ConvexPenetrationDepthSolver
	{
		public GjkEpaPenetrationDepthSolver()
			: base(btGjkEpaPenetrationDepthSolver_new())
		{
		}
	}
}
