using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class ConvexTriangleMeshShape : PolyhedralConvexAabbCachingShape
	{
		private StridingMeshInterface _meshInterface;

		internal ConvexTriangleMeshShape(IntPtr native)
			: base(native)
		{
		}

		public ConvexTriangleMeshShape(StridingMeshInterface meshInterface, bool calcAabb = true)
			: base(btConvexTriangleMeshShape_new(meshInterface.Native, calcAabb))
		{
			_meshInterface = meshInterface;
		}

		public void CalculatePrincipalAxisTransform(BMatrix principal, out BVector3 inertia,
			out double volume)
		{
			btConvexTriangleMeshShape_calculatePrincipalAxisTransform(Native, ref principal,
				out inertia, out volume);
		}

		public StridingMeshInterface MeshInterface => _meshInterface;
	}
}
