#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletCollision/CollisionDispatch/btCollisionCreateFunc.h>
#include <LinearMath/btPoolAllocator.h>

#include "btCollisionConfiguration_wrap.h"

btCollisionAlgorithmCreateFunc* btCollisionConfiguration_getCollisionAlgorithmCreateFunc(
	btCollisionConfiguration* obj, int proxyType0, int proxyType1)
{
	return obj->getCollisionAlgorithmCreateFunc(proxyType0, proxyType1);
}

btPoolAllocator* btCollisionConfiguration_getCollisionAlgorithmPool(btCollisionConfiguration* obj)
{
	return obj->getCollisionAlgorithmPool();
}

btPoolAllocator* btCollisionConfiguration_getPersistentManifoldPool(btCollisionConfiguration* obj)
{
	return obj->getPersistentManifoldPool();
}

void btCollisionConfiguration_delete(btCollisionConfiguration* obj)
{
	delete obj;
}
