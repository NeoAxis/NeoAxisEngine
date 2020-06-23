#include <BulletCollision/CollisionShapes/btOptimizedBvh.h>
#include <BulletCollision/CollisionShapes/btStridingMeshInterface.h>

#include "conversion.h"
#include "btOptimizedBvh_wrap.h"

btOptimizedBvh* btOptimizedBvh_new()
{
	return new btOptimizedBvh();
}

void btOptimizedBvh_build(btOptimizedBvh* obj, btStridingMeshInterface* triangles,
	bool useQuantizedAabbCompression, const btVector3* bvhAabbMin, const btVector3* bvhAabbMax)
{
	BTVECTOR3_IN(bvhAabbMin);
	BTVECTOR3_IN(bvhAabbMax);
	obj->build(triangles, useQuantizedAabbCompression, BTVECTOR3_USE(bvhAabbMin),
		BTVECTOR3_USE(bvhAabbMax));
}

btOptimizedBvh* btOptimizedBvh_deSerializeInPlace(void* i_alignedDataBuffer, unsigned int i_dataBufferSize,
	bool i_swapEndian)
{
	return btOptimizedBvh::deSerializeInPlace(i_alignedDataBuffer, i_dataBufferSize,
		i_swapEndian);
}

void btOptimizedBvh_refit(btOptimizedBvh* obj, btStridingMeshInterface* triangles,
	const btVector3* aabbMin, const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->refit(triangles, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

void btOptimizedBvh_refitPartial(btOptimizedBvh* obj, btStridingMeshInterface* triangles,
	const btVector3* aabbMin, const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->refitPartial(triangles, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

bool btOptimizedBvh_serializeInPlace(btOptimizedBvh* obj, void* o_alignedDataBuffer,
	unsigned int i_dataBufferSize, bool i_swapEndian)
{
	return obj->serializeInPlace(o_alignedDataBuffer, i_dataBufferSize, i_swapEndian);
}

void btOptimizedBvh_updateBvhNodes(btOptimizedBvh* obj, btStridingMeshInterface* meshInterface,
	int firstNode, int endNode, int index)
{
	obj->updateBvhNodes(meshInterface, firstNode, endNode, index);
}
