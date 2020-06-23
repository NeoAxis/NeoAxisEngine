#include <BulletCollision/CollisionDispatch/btBox2dBox2dCollisionAlgorithm.h>
#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/NarrowPhaseCollision/btPersistentManifold.h>

#include "btBox2dBox2dCollisionAlgorithm_wrap.h"

btBox2dBox2dCollisionAlgorithm_CreateFunc* btBox2dBox2dCollisionAlgorithm_CreateFunc_new()
{
	return new btBox2dBox2dCollisionAlgorithm::CreateFunc();
}


btBox2dBox2dCollisionAlgorithm* btBox2dBox2dCollisionAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci)
{
	return new btBox2dBox2dCollisionAlgorithm(*ci);
}

btBox2dBox2dCollisionAlgorithm* btBox2dBox2dCollisionAlgorithm_new2(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap)
{
	return new btBox2dBox2dCollisionAlgorithm(mf, *ci, body0Wrap, body1Wrap);
}
