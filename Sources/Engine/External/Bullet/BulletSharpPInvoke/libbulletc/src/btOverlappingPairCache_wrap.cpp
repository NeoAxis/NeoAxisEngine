#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/BroadphaseCollision/btOverlappingPairCache.h>

#include "btOverlappingPairCache_wrap.h"

bool btOverlapCallback_processOverlap(btOverlapCallback* obj, btBroadphasePair* pair)
{
	return obj->processOverlap(*pair);
}

void btOverlapCallback_delete(btOverlapCallback* obj)
{
	delete obj;
}


bool btOverlapFilterCallback_needBroadphaseCollision(btOverlapFilterCallback* obj,
	btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1)
{
	return obj->needBroadphaseCollision(proxy0, proxy1);
}

void btOverlapFilterCallback_delete(btOverlapFilterCallback* obj)
{
	delete obj;
}


void btOverlappingPairCache_cleanOverlappingPair(btOverlappingPairCache* obj, btBroadphasePair* pair,
	btDispatcher* dispatcher)
{
	obj->cleanOverlappingPair(*pair, dispatcher);
}

void btOverlappingPairCache_cleanProxyFromPairs(btOverlappingPairCache* obj, btBroadphaseProxy* proxy,
	btDispatcher* dispatcher)
{
	obj->cleanProxyFromPairs(proxy, dispatcher);
}

btBroadphasePair* btOverlappingPairCache_findPair(btOverlappingPairCache* obj, btBroadphaseProxy* proxy0,
	btBroadphaseProxy* proxy1)
{
	return obj->findPair(proxy0, proxy1);
}

int btOverlappingPairCache_getNumOverlappingPairs(btOverlappingPairCache* obj)
{
	return obj->getNumOverlappingPairs();
}

btAlignedObjectArray_btBroadphasePair* btOverlappingPairCache_getOverlappingPairArray(
	btOverlappingPairCache* obj)
{
	return &obj->getOverlappingPairArray();
}

btBroadphasePair* btOverlappingPairCache_getOverlappingPairArrayPtr(btOverlappingPairCache* obj)
{
	return obj->getOverlappingPairArrayPtr();
}

bool btOverlappingPairCache_hasDeferredRemoval(btOverlappingPairCache* obj)
{
	return obj->hasDeferredRemoval();
}

void btOverlappingPairCache_processAllOverlappingPairs(btOverlappingPairCache* obj,
	btOverlapCallback* __unnamed0, btDispatcher* dispatcher)
{
	obj->processAllOverlappingPairs(__unnamed0, dispatcher);
}

void btOverlappingPairCache_setInternalGhostPairCallback(btOverlappingPairCache* obj,
	btOverlappingPairCallback* ghostPairCallback)
{
	obj->setInternalGhostPairCallback(ghostPairCallback);
}

void btOverlappingPairCache_setOverlapFilterCallback(btOverlappingPairCache* obj,
	btOverlapFilterCallback* callback)
{
	obj->setOverlapFilterCallback(callback);
}

void btOverlappingPairCache_sortOverlappingPairs(btOverlappingPairCache* obj, btDispatcher* dispatcher)
{
	obj->sortOverlappingPairs(dispatcher);
}


btOverlapFilterCallbackWrapper * btOverlapFilterCallbackWrapper_new(p_btOverlapFilterCallback_needBroadphaseCollision needBroadphaseCollision)
{
	return new btOverlapFilterCallbackWrapper(needBroadphaseCollision);
}


btHashedOverlappingPairCache* btHashedOverlappingPairCache_new()
{
	return new btHashedOverlappingPairCache();
}

int btHashedOverlappingPairCache_GetCount(btHashedOverlappingPairCache* obj)
{
	return obj->GetCount();
}

btOverlapFilterCallback* btHashedOverlappingPairCache_getOverlapFilterCallback(btHashedOverlappingPairCache* obj)
{
	return obj->getOverlapFilterCallback();
}

bool btHashedOverlappingPairCache_needsBroadphaseCollision(btHashedOverlappingPairCache* obj,
	btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1)
{
	return obj->needsBroadphaseCollision(proxy0, proxy1);
}


btSortedOverlappingPairCache* btSortedOverlappingPairCache_new()
{
	return new btSortedOverlappingPairCache();
}

btOverlapFilterCallback* btSortedOverlappingPairCache_getOverlapFilterCallback(btSortedOverlappingPairCache* obj)
{
	return obj->getOverlapFilterCallback();
}

bool btSortedOverlappingPairCache_needsBroadphaseCollision(btSortedOverlappingPairCache* obj,
	btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1)
{
	return obj->needsBroadphaseCollision(proxy0, proxy1);
}


btNullPairCache* btNullPairCache_new()
{
	return new btNullPairCache();
}


btOverlapFilterCallbackWrapper::btOverlapFilterCallbackWrapper(p_btOverlapFilterCallback_needBroadphaseCollision needBroadphaseCollision)
{
	_needBroadphaseCollision = needBroadphaseCollision;
}

bool btOverlapFilterCallbackWrapper::needBroadphaseCollision(btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1) const
{
	return _needBroadphaseCollision(proxy0, proxy1);
}
