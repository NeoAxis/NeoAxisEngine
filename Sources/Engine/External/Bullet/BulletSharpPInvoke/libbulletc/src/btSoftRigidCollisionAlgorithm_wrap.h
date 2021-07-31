#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btSoftRigidCollisionAlgorithm_CreateFunc* btSoftRigidCollisionAlgorithm_CreateFunc_new();

	EXPORT btSoftRigidCollisionAlgorithm* btSoftRigidCollisionAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* col0, const btCollisionObjectWrapper* col1Wrap, bool isSwapped);
#ifdef __cplusplus
}
#endif
