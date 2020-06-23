#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btOptimizedBvh* btOptimizedBvh_new();
	EXPORT void btOptimizedBvh_build(btOptimizedBvh* obj, btStridingMeshInterface* triangles, bool useQuantizedAabbCompression, const btVector3* bvhAabbMin, const btVector3* bvhAabbMax);
	EXPORT btOptimizedBvh* btOptimizedBvh_deSerializeInPlace(void* i_alignedDataBuffer, unsigned int i_dataBufferSize, bool i_swapEndian);
	EXPORT void btOptimizedBvh_refit(btOptimizedBvh* obj, btStridingMeshInterface* triangles, const btVector3* aabbMin, const btVector3* aabbMax);
	EXPORT void btOptimizedBvh_refitPartial(btOptimizedBvh* obj, btStridingMeshInterface* triangles, const btVector3* aabbMin, const btVector3* aabbMax);
	EXPORT bool btOptimizedBvh_serializeInPlace(btOptimizedBvh* obj, void* o_alignedDataBuffer, unsigned int i_dataBufferSize, bool i_swapEndian);
	EXPORT void btOptimizedBvh_updateBvhNodes(btOptimizedBvh* obj, btStridingMeshInterface* meshInterface, int firstNode, int endNode, int index);
#ifdef __cplusplus
}
#endif
