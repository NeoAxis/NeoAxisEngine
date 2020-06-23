#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btManifoldResult* btManifoldResult_new();
	EXPORT btManifoldResult* btManifoldResult_new2(const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap);
	EXPORT btScalar btManifoldResult_calculateCombinedContactDamping(const btCollisionObject* body0, const btCollisionObject* body1);
	EXPORT btScalar btManifoldResult_calculateCombinedContactStiffness(const btCollisionObject* body0, const btCollisionObject* body1);
	EXPORT btScalar btManifoldResult_calculateCombinedFriction(const btCollisionObject* body0, const btCollisionObject* body1);
	EXPORT btScalar btManifoldResult_calculateCombinedRestitution(const btCollisionObject* body0, const btCollisionObject* body1);
	EXPORT btScalar btManifoldResult_calculateCombinedRollingFriction(const btCollisionObject* body0, const btCollisionObject* body1);
	EXPORT btScalar btManifoldResult_getClosestPointDistanceThreshold(btManifoldResult* obj);
	EXPORT const btCollisionObject* btManifoldResult_getBody0Internal(btManifoldResult* obj);
	EXPORT const btCollisionObjectWrapper* btManifoldResult_getBody0Wrap(btManifoldResult* obj);
	EXPORT const btCollisionObject* btManifoldResult_getBody1Internal(btManifoldResult* obj);
	EXPORT const btCollisionObjectWrapper* btManifoldResult_getBody1Wrap(btManifoldResult* obj);
	EXPORT btPersistentManifold* btManifoldResult_getPersistentManifold(btManifoldResult* obj);
	EXPORT void btManifoldResult_refreshContactPoints(btManifoldResult* obj);
	EXPORT void btManifoldResult_setBody0Wrap(btManifoldResult* obj, const btCollisionObjectWrapper* obj0Wrap);
	EXPORT void btManifoldResult_setBody1Wrap(btManifoldResult* obj, const btCollisionObjectWrapper* obj1Wrap);
	EXPORT void btManifoldResult_setClosestPointDistanceThreshold(btManifoldResult* obj, btScalar value);
	EXPORT void btManifoldResult_setPersistentManifold(btManifoldResult* obj, btPersistentManifold* manifoldPtr);
#ifdef __cplusplus
}
#endif
