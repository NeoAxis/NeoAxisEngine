#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/CollisionDispatch/btManifoldResult.h>
#include <BulletSoftBody/btSoftBodyConcaveCollisionAlgorithm.h>

#include "btSoftBodyConcaveCollisionAlgorithm_wrap.h"

btSoftBodyConcaveCollisionAlgorithm::CreateFunc* btSoftBodyConcaveCollisionAlgorithm_CreateFunc_new()
{
	return new btSoftBodyConcaveCollisionAlgorithm::CreateFunc();
}


btSoftBodyConcaveCollisionAlgorithm_SwappedCreateFunc* btSoftBodyConcaveCollisionAlgorithm_SwappedCreateFunc_new()
{
	return new btSoftBodyConcaveCollisionAlgorithm::SwappedCreateFunc();
}


btSoftBodyConcaveCollisionAlgorithm* btSoftBodyConcaveCollisionAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci,
	const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	bool isSwapped)
{
	return new btSoftBodyConcaveCollisionAlgorithm(*ci, body0Wrap, body1Wrap, isSwapped);
}

void btSoftBodyConcaveCollisionAlgorithm_clearCache(btSoftBodyConcaveCollisionAlgorithm* obj)
{
	obj->clearCache();
}
