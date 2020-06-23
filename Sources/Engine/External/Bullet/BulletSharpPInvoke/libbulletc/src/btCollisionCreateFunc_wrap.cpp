#include <BulletCollision/BroadphaseCollision/btCollisionAlgorithm.h>
#include <BulletCollision/CollisionDispatch/btCollisionCreateFunc.h>
#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>

#include "btCollisionCreateFunc_wrap.h"

btCollisionAlgorithmCreateFunc* btCollisionAlgorithmCreateFunc_new()
{
	return new btCollisionAlgorithmCreateFunc();
}

btCollisionAlgorithm* btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(btCollisionAlgorithmCreateFunc* obj,
	btCollisionAlgorithmConstructionInfo* __unnamed0, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap)
{
	return obj->CreateCollisionAlgorithm(*__unnamed0, body0Wrap, body1Wrap);
}

bool btCollisionAlgorithmCreateFunc_getSwapped(btCollisionAlgorithmCreateFunc* obj)
{
	return obj->m_swapped;
}

void btCollisionAlgorithmCreateFunc_setSwapped(btCollisionAlgorithmCreateFunc* obj,
	bool value)
{
	obj->m_swapped = value;
}

void btCollisionAlgorithmCreateFunc_delete(btCollisionAlgorithmCreateFunc* obj)
{
	delete obj;
}
