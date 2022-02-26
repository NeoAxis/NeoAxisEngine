using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class ScaledBvhTriangleMeshShape : ConcaveShape
	{
		public ScaledBvhTriangleMeshShape(BvhTriangleMeshShape childShape, BVector3 localScaling)
			: base(btScaledBvhTriangleMeshShape_new(childShape.Native, ref localScaling))
		{
			ChildShape = childShape;
		}

		public BvhTriangleMeshShape ChildShape { get; }
	}
}
