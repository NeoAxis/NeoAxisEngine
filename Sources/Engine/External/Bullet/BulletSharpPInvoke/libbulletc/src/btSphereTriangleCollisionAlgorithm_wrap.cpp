#include <BulletCollision/CollisionDispatch/btSphereTriangleCollisionAlgorithm.h>

#include "btSphereTriangleCollisionAlgorithm_wrap.h"

btSphereTriangleCollisionAlgorithm_CreateFunc* btSphereTriangleCollisionAlgorithm_CreateFunc_new()
{
	return new btSphereTriangleCollisionAlgorithm::CreateFunc();
}


btSphereTriangleCollisionAlgorithm* btSphereTriangleCollisionAlgorithm_new(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap, bool swapped)
{
	return new btSphereTriangleCollisionAlgorithm(mf, *ci, body0Wrap, body1Wrap,
		swapped);
}

btSphereTriangleCollisionAlgorithm* btSphereTriangleCollisionAlgorithm_new2(const btCollisionAlgorithmConstructionInfo* ci)
{
	return new btSphereTriangleCollisionAlgorithm(*ci);
}
