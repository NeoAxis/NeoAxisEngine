using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class GjkConvexCast : ConvexCast
	{
		public GjkConvexCast(ConvexShape convexA, ConvexShape convexB, VoronoiSimplexSolver simplexSolver)
			: base(btGjkConvexCast_new(convexA.Native, convexB.Native, simplexSolver.Native))
		{
		}
	}
}
