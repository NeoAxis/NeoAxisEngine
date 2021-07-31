#include <BulletCollision/BroadphaseCollision/btBroadphaseInterface.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/BroadphaseCollision/btOverlappingPairCache.h>

#include "conversion.h"
#include "btBroadphaseInterface_wrap.h"

btBroadphaseAabbCallbackWrapper::btBroadphaseAabbCallbackWrapper(p_btBroadphaseAabbCallback_process processCallback)
{
	_processCallback = processCallback;
}

bool btBroadphaseAabbCallbackWrapper::process(const btBroadphaseProxy* proxy)
{
	return _processCallback(proxy);
}


btBroadphaseRayCallbackWrapper::btBroadphaseRayCallbackWrapper(p_btBroadphaseAabbCallback_process processCallback)
{
	_processCallback = processCallback;
}

bool btBroadphaseRayCallbackWrapper::process(const btBroadphaseProxy* proxy)
{
	return _processCallback(proxy);
}


btBroadphaseAabbCallbackWrapper* btBroadphaseAabbCallbackWrapper_new(p_btBroadphaseAabbCallback_process processCallback)
{
	return new btBroadphaseAabbCallbackWrapper(processCallback);
}


bool btBroadphaseAabbCallback_process(btBroadphaseAabbCallback* obj, const btBroadphaseProxy* proxy)
{
	return obj->process(proxy);
}

void btBroadphaseAabbCallback_delete(btBroadphaseAabbCallback* obj)
{
	delete obj;
}


btBroadphaseRayCallbackWrapper* btBroadphaseRayCallbackWrapper_new(p_btBroadphaseAabbCallback_process processCallback)
{
	return new btBroadphaseRayCallbackWrapper(processCallback);
}


btScalar btBroadphaseRayCallback_getLambda_max(btBroadphaseRayCallback* obj)
{
	return obj->m_lambda_max;
}

void btBroadphaseRayCallback_getRayDirectionInverse(btBroadphaseRayCallback* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_rayDirectionInverse);
}

unsigned int* btBroadphaseRayCallback_getSigns(btBroadphaseRayCallback* obj)
{
	return obj->m_signs;
}

void btBroadphaseRayCallback_setLambda_max(btBroadphaseRayCallback* obj, btScalar value)
{
	obj->m_lambda_max = value;
}

void btBroadphaseRayCallback_setRayDirectionInverse(btBroadphaseRayCallback* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_rayDirectionInverse, value);
}


void btBroadphaseInterface_aabbTest(btBroadphaseInterface* obj, const btVector3* aabbMin,
	const btVector3* aabbMax, btBroadphaseAabbCallback* callback)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->aabbTest(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax), *callback);
}

void btBroadphaseInterface_calculateOverlappingPairs(btBroadphaseInterface* obj,
	btDispatcher* dispatcher)
{
	obj->calculateOverlappingPairs(dispatcher);
}

btBroadphaseProxy* btBroadphaseInterface_createProxy(btBroadphaseInterface* obj,
	const btVector3* aabbMin, const btVector3* aabbMax, int shapeType, void* userPtr,
	int collisionFilterGroup, int collisionFilterMask, btDispatcher* dispatcher)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	return obj->createProxy(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax), shapeType,
		userPtr, collisionFilterGroup, collisionFilterMask, dispatcher);
}

void btBroadphaseInterface_destroyProxy(btBroadphaseInterface* obj, btBroadphaseProxy* proxy,
	btDispatcher* dispatcher)
{
	obj->destroyProxy(proxy, dispatcher);
}

void btBroadphaseInterface_getAabb(btBroadphaseInterface* obj, btBroadphaseProxy* proxy,
	btVector3* aabbMin, btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getAabb(proxy, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

void btBroadphaseInterface_getBroadphaseAabb(btBroadphaseInterface* obj, btVector3* aabbMin,
	btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getBroadphaseAabb(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

btOverlappingPairCache* btBroadphaseInterface_getOverlappingPairCache(btBroadphaseInterface* obj)
{
	return obj->getOverlappingPairCache();
}

void btBroadphaseInterface_printStats(btBroadphaseInterface* obj)
{
	obj->printStats();
}

void btBroadphaseInterface_rayTest(btBroadphaseInterface* obj, const btVector3* rayFrom,
	const btVector3* rayTo, btBroadphaseRayCallback* rayCallback)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *rayCallback);
}

void btBroadphaseInterface_rayTest2(btBroadphaseInterface* obj, const btVector3* rayFrom,
	const btVector3* rayTo, btBroadphaseRayCallback* rayCallback, const btVector3* aabbMin)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	BTVECTOR3_IN(aabbMin);
	obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *rayCallback, BTVECTOR3_USE(aabbMin));
}

void btBroadphaseInterface_rayTest3(btBroadphaseInterface* obj, const btVector3* rayFrom,
	const btVector3* rayTo, btBroadphaseRayCallback* rayCallback, const btVector3* aabbMin,
	const btVector3* aabbMax)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *rayCallback, BTVECTOR3_USE(aabbMin),
		BTVECTOR3_USE(aabbMax));
}

void btBroadphaseInterface_resetPool(btBroadphaseInterface* obj, btDispatcher* dispatcher)
{
	obj->resetPool(dispatcher);
}

void btBroadphaseInterface_setAabb(btBroadphaseInterface* obj, btBroadphaseProxy* proxy,
	const btVector3* aabbMin, const btVector3* aabbMax, btDispatcher* dispatcher)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->setAabb(proxy, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax), dispatcher);
}

void btBroadphaseInterface_delete(btBroadphaseInterface* obj)
{
	delete obj;
}
