#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btTriangleMeshShape_getLocalAabbMax(btTriangleMeshShape* obj, btVector3* value);
	EXPORT void btTriangleMeshShape_getLocalAabbMin(btTriangleMeshShape* obj, btVector3* value);
	EXPORT btStridingMeshInterface* btTriangleMeshShape_getMeshInterface(btTriangleMeshShape* obj);
	EXPORT void btTriangleMeshShape_localGetSupportingVertex(btTriangleMeshShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btTriangleMeshShape_localGetSupportingVertexWithoutMargin(btTriangleMeshShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btTriangleMeshShape_recalcLocalAabb(btTriangleMeshShape* obj);
#ifdef __cplusplus
}
#endif
