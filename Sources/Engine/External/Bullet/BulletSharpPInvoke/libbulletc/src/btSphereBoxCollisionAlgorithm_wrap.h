#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btSphereBoxCollisionAlgorithm_CreateFunc* btSphereBoxCollisionAlgorithm_CreateFunc_new();

	EXPORT btSphereBoxCollisionAlgorithm* btSphereBoxCollisionAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap, bool isSwapped);
	EXPORT bool btSphereBoxCollisionAlgorithm_getSphereDistance(btSphereBoxCollisionAlgorithm* obj, const btCollisionObjectWrapper* boxObjWrap, btVector3* v3PointOnBox, btVector3* normal, btScalar* penetrationDepth, const btVector3* v3SphereCenter, btScalar fRadius, btScalar maxContactDistance);
	EXPORT btScalar btSphereBoxCollisionAlgorithm_getSpherePenetration(btSphereBoxCollisionAlgorithm* obj, const btVector3* boxHalfExtent, const btVector3* sphereRelPos, btVector3* closestPoint, btVector3* normal);
#ifdef __cplusplus
}
#endif
