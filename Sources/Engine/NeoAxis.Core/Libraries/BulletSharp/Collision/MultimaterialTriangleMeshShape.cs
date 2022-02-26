using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MultimaterialTriangleMeshShape : BvhTriangleMeshShape
	{
		public MultimaterialTriangleMeshShape(StridingMeshInterface meshInterface,
			bool useQuantizedAabbCompression, bool buildBvh = true)
			: base(btMultimaterialTriangleMeshShape_new(meshInterface.Native, useQuantizedAabbCompression,
				buildBvh))
		{
			_meshInterface = meshInterface;
		}

		public MultimaterialTriangleMeshShape(StridingMeshInterface meshInterface,
			bool useQuantizedAabbCompression, BVector3 bvhAabbMin, BVector3 bvhAabbMax,
			bool buildBvh = true)
			: base(btMultimaterialTriangleMeshShape_new2(meshInterface.Native, useQuantizedAabbCompression,
				ref bvhAabbMin, ref bvhAabbMax, buildBvh))
		{
			_meshInterface = meshInterface;
		}
		/*
		public BulletMaterial GetMaterialProperties(int partID, int triIndex)
		{
			return btMultimaterialTriangleMeshShape_getMaterialProperties(Native,
				partID, triIndex);
		}
		*/
	}
}
