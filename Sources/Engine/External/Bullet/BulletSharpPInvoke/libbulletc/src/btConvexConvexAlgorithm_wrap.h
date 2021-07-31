#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConvexConvexAlgorithm_CreateFunc* btConvexConvexAlgorithm_CreateFunc_new(btConvexPenetrationDepthSolver* pdSolver);
	EXPORT int btConvexConvexAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(btConvexConvexAlgorithm_CreateFunc* obj);
	EXPORT int btConvexConvexAlgorithm_CreateFunc_getNumPerturbationIterations(btConvexConvexAlgorithm_CreateFunc* obj);
	EXPORT btConvexPenetrationDepthSolver* btConvexConvexAlgorithm_CreateFunc_getPdSolver(btConvexConvexAlgorithm_CreateFunc* obj);
	EXPORT void btConvexConvexAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(btConvexConvexAlgorithm_CreateFunc* obj, int value);
	EXPORT void btConvexConvexAlgorithm_CreateFunc_setNumPerturbationIterations(btConvexConvexAlgorithm_CreateFunc* obj, int value);
	EXPORT void btConvexConvexAlgorithm_CreateFunc_setPdSolver(btConvexConvexAlgorithm_CreateFunc* obj, btConvexPenetrationDepthSolver* value);

	EXPORT btConvexConvexAlgorithm* btConvexConvexAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap, btConvexPenetrationDepthSolver* pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold);
	EXPORT const btPersistentManifold* btConvexConvexAlgorithm_getManifold(btConvexConvexAlgorithm* obj);
	EXPORT void btConvexConvexAlgorithm_setLowLevelOfDetail(btConvexConvexAlgorithm* obj, bool useLowLevel);
#ifdef __cplusplus
}
#endif
