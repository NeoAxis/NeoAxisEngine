#include <BulletSoftBody/btSoftRigidCollisionAlgorithm.h>

#include "btSoftRigidCollisionAlgorithm_wrap.h"

btSoftRigidCollisionAlgorithm_CreateFunc* btSoftRigidCollisionAlgorithm_CreateFunc_new()
{
	return new btSoftRigidCollisionAlgorithm::CreateFunc();
}


btSoftRigidCollisionAlgorithm* btSoftRigidCollisionAlgorithm_new(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* col0,
	const btCollisionObjectWrapper* col1Wrap, bool isSwapped)
{
	return new btSoftRigidCollisionAlgorithm(mf, *ci, col0, col1Wrap, isSwapped);
}
