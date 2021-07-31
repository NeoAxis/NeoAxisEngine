#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConvexTriangleMeshShape* btConvexTriangleMeshShape_new(btStridingMeshInterface* meshInterface, bool calcAabb);
	EXPORT void btConvexTriangleMeshShape_calculatePrincipalAxisTransform(btConvexTriangleMeshShape* obj, btTransform* principal, btVector3* inertia, btScalar* volume);
	EXPORT btStridingMeshInterface* btConvexTriangleMeshShape_getMeshInterface(btConvexTriangleMeshShape* obj);
#ifdef __cplusplus
}
#endif
