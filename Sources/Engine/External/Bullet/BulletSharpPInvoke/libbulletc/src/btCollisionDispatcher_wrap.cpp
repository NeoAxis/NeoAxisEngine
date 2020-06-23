#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletCollision/CollisionDispatch/btCollisionDispatcher.h>

#include "btCollisionDispatcher_wrap.h"

btCollisionDispatcher* btCollisionDispatcher_new(btCollisionConfiguration* collisionConfiguration)
{
	return new btCollisionDispatcher(collisionConfiguration);
}

void btCollisionDispatcher_defaultNearCallback(btBroadphasePair* collisionPair, btCollisionDispatcher* dispatcher,
	const btDispatcherInfo* dispatchInfo)
{
	btCollisionDispatcher::defaultNearCallback(*collisionPair, *dispatcher, *dispatchInfo);
}

btCollisionConfiguration* btCollisionDispatcher_getCollisionConfiguration(btCollisionDispatcher* obj)
{
	return obj->getCollisionConfiguration();
}

int btCollisionDispatcher_getDispatcherFlags(btCollisionDispatcher* obj)
{
	return obj->getDispatcherFlags();
}

btNearCallback btCollisionDispatcher_getNearCallback(btCollisionDispatcher* obj)
{
	return obj->getNearCallback();
}

void btCollisionDispatcher_registerCollisionCreateFunc(btCollisionDispatcher* obj,
	int proxyType0, int proxyType1, btCollisionAlgorithmCreateFunc* createFunc)
{
	obj->registerCollisionCreateFunc(proxyType0, proxyType1, createFunc);
}

void btCollisionDispatcher_registerClosestPointsCreateFunc(btCollisionDispatcher * obj,
	int proxyType0, int proxyType1, btCollisionAlgorithmCreateFunc * createFunc)
{
	obj->registerCollisionCreateFunc(proxyType0, proxyType1, createFunc);
}

void btCollisionDispatcher_setCollisionConfiguration(btCollisionDispatcher* obj,
	btCollisionConfiguration* config)
{
	obj->setCollisionConfiguration(config);
}

void btCollisionDispatcher_setDispatcherFlags(btCollisionDispatcher* obj, int flags)
{
	obj->setDispatcherFlags(flags);
}

void btCollisionDispatcher_setNearCallback(btCollisionDispatcher* obj, btNearCallback nearCallback)
{
	if (nearCallback == 0)
	{
		obj->setNearCallback(btCollisionDispatcher::defaultNearCallback);
		return;
	}
	obj->setNearCallback(nearCallback);
}
