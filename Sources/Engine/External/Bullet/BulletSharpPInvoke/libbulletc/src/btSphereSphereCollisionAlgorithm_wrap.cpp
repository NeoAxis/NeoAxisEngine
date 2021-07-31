#include <BulletCollision/CollisionDispatch/btSphereSphereCollisionAlgorithm.h>

#include "btSphereSphereCollisionAlgorithm_wrap.h"

btSphereSphereCollisionAlgorithm_CreateFunc* btSphereSphereCollisionAlgorithm_CreateFunc_new()
{
	return new btSphereSphereCollisionAlgorithm::CreateFunc();
}


btSphereSphereCollisionAlgorithm* btSphereSphereCollisionAlgorithm_new(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* col0Wrap,
	const btCollisionObjectWrapper* col1Wrap)
{
	return new btSphereSphereCollisionAlgorithm(mf, *ci, col0Wrap, col1Wrap);
}

btSphereSphereCollisionAlgorithm* btSphereSphereCollisionAlgorithm_new2(const btCollisionAlgorithmConstructionInfo* ci)
{
	return new btSphereSphereCollisionAlgorithm(*ci);
}
