#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btScaledBvhTriangleMeshShape* btScaledBvhTriangleMeshShape_new(btBvhTriangleMeshShape* childShape, const btVector3* localScaling);
	EXPORT btBvhTriangleMeshShape* btScaledBvhTriangleMeshShape_getChildShape(btScaledBvhTriangleMeshShape* obj);
#ifdef __cplusplus
}
#endif
