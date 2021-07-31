#include <BulletCollision/CollisionShapes/btMultimaterialTriangleMeshShape.h>

#include "conversion.h"
#include "btMultimaterialTriangleMeshShape_wrap.h"

btMultimaterialTriangleMeshShape* btMultimaterialTriangleMeshShape_new(btStridingMeshInterface* meshInterface,
	bool useQuantizedAabbCompression, bool buildBvh)
{
	return new btMultimaterialTriangleMeshShape(meshInterface, useQuantizedAabbCompression,
		buildBvh);
}

btMultimaterialTriangleMeshShape* btMultimaterialTriangleMeshShape_new2(btStridingMeshInterface* meshInterface,
	bool useQuantizedAabbCompression, const btVector3* bvhAabbMin, const btVector3* bvhAabbMax,
	bool buildBvh)
{
	BTVECTOR3_IN(bvhAabbMin);
	BTVECTOR3_IN(bvhAabbMax);
	return new btMultimaterialTriangleMeshShape(meshInterface, useQuantizedAabbCompression,
		BTVECTOR3_USE(bvhAabbMin), BTVECTOR3_USE(bvhAabbMax), buildBvh);
}

const btMaterial* btMultimaterialTriangleMeshShape_getMaterialProperties(btMultimaterialTriangleMeshShape* obj,
	int partID, int triIndex)
{
	return obj->getMaterialProperties(partID, triIndex);
}
