#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBox2dBox2dCollisionAlgorithm_CreateFunc* btBox2dBox2dCollisionAlgorithm_CreateFunc_new();

	EXPORT btBox2dBox2dCollisionAlgorithm* btBox2dBox2dCollisionAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci);
	EXPORT btBox2dBox2dCollisionAlgorithm* btBox2dBox2dCollisionAlgorithm_new2(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap);
#ifdef __cplusplus
}
#endif
