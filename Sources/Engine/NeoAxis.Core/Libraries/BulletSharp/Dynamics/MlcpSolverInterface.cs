using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public abstract class MlcpSolverInterface : IDisposable
	{
		internal IntPtr Native;

		internal MlcpSolverInterface(IntPtr native)
		{
			Native = native;
		}
		/*
		public bool SolveMLCP(btMatrixX<double> a, btVectorX<double> b, btVectorX<double> x,
			btVectorX<double> lo, btVectorX<double> hi, AlignedObjectArray<int> limitDependency,
			int numIterations, bool useSparsity = true)
		{
			return btMLCPSolverInterface_solveMLCP(Native, a.Native, b.Native,
				x.Native, lo.Native, hi.Native, limitDependency.Native, numIterations,
				useSparsity);
		}
		*/
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				btMLCPSolverInterface_delete(Native);
				Native = IntPtr.Zero;
			}
		}

		~MlcpSolverInterface()
		{
			Dispose(false);
		}
	}
}
