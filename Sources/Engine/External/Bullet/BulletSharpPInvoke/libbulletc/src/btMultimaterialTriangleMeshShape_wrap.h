#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultimaterialTriangleMeshShape* btMultimaterialTriangleMeshShape_new(btStridingMeshInterface* meshInterface, bool useQuantizedAabbCompression, bool buildBvh);
	EXPORT btMultimaterialTriangleMeshShape* btMultimaterialTriangleMeshShape_new2(btStridingMeshInterface* meshInterface, bool useQuantizedAabbCompression, const btVector3* bvhAabbMin, const btVector3* bvhAabbMax, bool buildBvh);
	EXPORT const btMaterial* btMultimaterialTriangleMeshShape_getMaterialProperties(btMultimaterialTriangleMeshShape* obj, int partID, int triIndex);
#ifdef __cplusplus
}
#endif
