#include <BulletCollision/CollisionDispatch/btEmptyCollisionAlgorithm.h>

#include "btEmptyCollisionAlgorithm_wrap.h"

btEmptyAlgorithm_CreateFunc* btEmptyAlgorithm_CreateFunc_new()
{
	return new btEmptyAlgorithm::CreateFunc();
}


btEmptyAlgorithm* btEmptyAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci)
{
	return new btEmptyAlgorithm(*ci);
}
