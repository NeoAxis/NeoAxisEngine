using System;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class DantzigSolver : MlcpSolverInterface
	{
		internal DantzigSolver(IntPtr native)
			: base(native)
		{
		}

		public DantzigSolver()
			: base(btDantzigSolver_new())
		{
		}
	}
}
