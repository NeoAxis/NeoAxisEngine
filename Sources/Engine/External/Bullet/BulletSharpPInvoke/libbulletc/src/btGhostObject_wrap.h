#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btGhostObject* btGhostObject_new();
	EXPORT void btGhostObject_addOverlappingObjectInternal(btGhostObject* obj, btBroadphaseProxy* otherProxy, btBroadphaseProxy* thisProxy);
	EXPORT void btGhostObject_convexSweepTest(btGhostObject* obj, const btConvexShape* castShape, const btTransform* convexFromWorld, const btTransform* convexToWorld, btCollisionWorld_ConvexResultCallback* resultCallback, btScalar allowedCcdPenetration);
	EXPORT int btGhostObject_getNumOverlappingObjects(btGhostObject* obj);
	EXPORT btCollisionObject* btGhostObject_getOverlappingObject(btGhostObject* obj, int index);
	EXPORT btAlignedObjectArray_btCollisionObjectPtr* btGhostObject_getOverlappingPairs(btGhostObject* obj);
	EXPORT void btGhostObject_rayTest(btGhostObject* obj, const btVector3* rayFromWorld, const btVector3* rayToWorld, btCollisionWorld_RayResultCallback* resultCallback);
	EXPORT void btGhostObject_removeOverlappingObjectInternal(btGhostObject* obj, btBroadphaseProxy* otherProxy, btDispatcher* dispatcher, btBroadphaseProxy* thisProxy);
	EXPORT btGhostObject* btGhostObject_upcast(btCollisionObject* colObj);

	EXPORT btPairCachingGhostObject* btPairCachingGhostObject_new();
	EXPORT btHashedOverlappingPairCache* btPairCachingGhostObject_getOverlappingPairCache(btPairCachingGhostObject* obj);

	EXPORT btGhostPairCallback* btGhostPairCallback_new();
#ifdef __cplusplus
}
#endif
