#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConvexPlaneCollisionAlgorithm_CreateFunc* btConvexPlaneCollisionAlgorithm_CreateFunc_new();
	EXPORT int btConvexPlaneCollisionAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(btConvexPlaneCollisionAlgorithm_CreateFunc* obj);
	EXPORT int btConvexPlaneCollisionAlgorithm_CreateFunc_getNumPerturbationIterations(btConvexPlaneCollisionAlgorithm_CreateFunc* obj);
	EXPORT void btConvexPlaneCollisionAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(btConvexPlaneCollisionAlgorithm_CreateFunc* obj, int value);
	EXPORT void btConvexPlaneCollisionAlgorithm_CreateFunc_setNumPerturbationIterations(btConvexPlaneCollisionAlgorithm_CreateFunc* obj, int value);

	EXPORT btConvexPlaneCollisionAlgorithm* btConvexPlaneCollisionAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap, bool isSwapped, int numPerturbationIterations, int minimumPointsPerturbationThreshold);
	EXPORT void btConvexPlaneCollisionAlgorithm_collideSingleContact(btConvexPlaneCollisionAlgorithm* obj, const btQuaternion* perturbeRot, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap, const btDispatcherInfo* dispatchInfo, btManifoldResult* resultOut);
#ifdef __cplusplus
}
#endif
