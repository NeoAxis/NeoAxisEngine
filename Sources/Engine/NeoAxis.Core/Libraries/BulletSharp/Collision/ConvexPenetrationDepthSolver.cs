using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class ConvexPenetrationDepthSolver : IDisposable
	{
		internal IntPtr Native;

		internal ConvexPenetrationDepthSolver(IntPtr native)
		{
			Native = native;
		}

		public bool CalcPenDepth(VoronoiSimplexSolver simplexSolver, ConvexShape convexA,
			ConvexShape convexB, BMatrix transA, BMatrix transB, out BVector3 v, out BVector3 pa,
			out BVector3 pb, IDebugDraw debugDraw)
		{
			return btConvexPenetrationDepthSolver_calcPenDepth(Native, simplexSolver.Native,
				convexA.Native, convexB.Native, ref transA, ref transB, out v, out pa,
				out pb, DebugDraw.GetUnmanaged(debugDraw));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				btConvexPenetrationDepthSolver_delete(Native);
				Native = IntPtr.Zero;
			}
		}

		~ConvexPenetrationDepthSolver()
		{
			Dispose(false);
		}
	}
}
