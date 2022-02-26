using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MinkowskiPenetrationDepthSolver : ConvexPenetrationDepthSolver
	{
		public MinkowskiPenetrationDepthSolver()
			: base(btMinkowskiPenetrationDepthSolver_new())
		{
		}
	}
}
