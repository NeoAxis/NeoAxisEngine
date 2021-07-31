#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/CollisionDispatch/btCompoundCompoundCollisionAlgorithm.h>

#include "btCompoundCompoundCollisionAlgorithm_wrap.h"

btCompoundCompoundCollisionAlgorithm_CreateFunc* btCompoundCompoundCollisionAlgorithm_CreateFunc_new()
{
	return new btCompoundCompoundCollisionAlgorithm::CreateFunc();
}


btCompoundCompoundCollisionAlgorithm_SwappedCreateFunc* btCompoundCompoundCollisionAlgorithm_SwappedCreateFunc_new()
{
	return new btCompoundCompoundCollisionAlgorithm::SwappedCreateFunc();
}


btCompoundCompoundCollisionAlgorithm* btCompoundCompoundCollisionAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci,
	const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	bool isSwapped)
{
	return new btCompoundCompoundCollisionAlgorithm(*ci, body0Wrap, body1Wrap, isSwapped);
}
