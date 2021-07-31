#include <BulletCollision/BroadphaseCollision/btBroadphaseProxy.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/BroadphaseCollision/btOverlappingPairCallback.h>

#include "btOverlappingPairCallback_wrap.h"

btBroadphasePair* btOverlappingPairCallback_addOverlappingPair(btOverlappingPairCallback* obj,
	btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1)
{
	return obj->addOverlappingPair(proxy0, proxy1);
}

void* btOverlappingPairCallback_removeOverlappingPair(btOverlappingPairCallback* obj,
	btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1, btDispatcher* dispatcher)
{
	return obj->removeOverlappingPair(proxy0, proxy1, dispatcher);
}

void btOverlappingPairCallback_removeOverlappingPairsContainingProxy(btOverlappingPairCallback* obj,
	btBroadphaseProxy* proxy0, btDispatcher* dispatcher)
{
	obj->removeOverlappingPairsContainingProxy(proxy0, dispatcher);
}

void btOverlappingPairCallback_delete(btOverlappingPairCallback* obj)
{
	delete obj;
}
