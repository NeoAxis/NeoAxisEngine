#include <BulletCollision/BroadphaseCollision/btCollisionAlgorithm.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/BroadphaseCollision/btOverlappingPairCache.h>
#include <BulletCollision/CollisionDispatch/btCollisionObject.h>
#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/NarrowPhaseCollision/btPersistentManifold.h>
#include <LinearMath/btIDebugDraw.h>
#include <LinearMath/btPoolAllocator.h>

#include "btDispatcher_wrap.h"

btScalar btDispatcherInfo_getAllowedCcdPenetration(btDispatcherInfo* obj)
{
	return obj->m_allowedCcdPenetration;
}

btScalar btDispatcherInfo_getConvexConservativeDistanceThreshold(btDispatcherInfo* obj)
{
	return obj->m_convexConservativeDistanceThreshold;
}

btIDebugDraw* btDispatcherInfo_getDebugDraw(btDispatcherInfo* obj)
{
	return obj->m_debugDraw;
}

int btDispatcherInfo_getDispatchFunc(btDispatcherInfo* obj)
{
	return obj->m_dispatchFunc;
}

bool btDispatcherInfo_getEnableSatConvex(btDispatcherInfo* obj)
{
	return obj->m_enableSatConvex;
}

bool btDispatcherInfo_getEnableSPU(btDispatcherInfo* obj)
{
	return obj->m_enableSPU;
}

int btDispatcherInfo_getStepCount(btDispatcherInfo* obj)
{
	return obj->m_stepCount;
}

btScalar btDispatcherInfo_getTimeOfImpact(btDispatcherInfo* obj)
{
	return obj->m_timeOfImpact;
}

btScalar btDispatcherInfo_getTimeStep(btDispatcherInfo* obj)
{
	return obj->m_timeStep;
}

bool btDispatcherInfo_getUseContinuous(btDispatcherInfo* obj)
{
	return obj->m_useContinuous;
}

bool btDispatcherInfo_getUseConvexConservativeDistanceUtil(btDispatcherInfo* obj)
{
	return obj->m_useConvexConservativeDistanceUtil;
}

bool btDispatcherInfo_getUseEpa(btDispatcherInfo* obj)
{
	return obj->m_useEpa;
}

void btDispatcherInfo_setAllowedCcdPenetration(btDispatcherInfo* obj, btScalar value)
{
	obj->m_allowedCcdPenetration = value;
}

void btDispatcherInfo_setConvexConservativeDistanceThreshold(btDispatcherInfo* obj,
	btScalar value)
{
	obj->m_convexConservativeDistanceThreshold = value;
}

void btDispatcherInfo_setDebugDraw(btDispatcherInfo* obj, btIDebugDraw* value)
{
	obj->m_debugDraw = value;
}

void btDispatcherInfo_setDispatchFunc(btDispatcherInfo* obj, int value)
{
	obj->m_dispatchFunc = value;
}

void btDispatcherInfo_setEnableSatConvex(btDispatcherInfo* obj, bool value)
{
	obj->m_enableSatConvex = value;
}

void btDispatcherInfo_setEnableSPU(btDispatcherInfo* obj, bool value)
{
	obj->m_enableSPU = value;
}

void btDispatcherInfo_setStepCount(btDispatcherInfo* obj, int value)
{
	obj->m_stepCount = value;
}

void btDispatcherInfo_setTimeOfImpact(btDispatcherInfo* obj, btScalar value)
{
	obj->m_timeOfImpact = value;
}

void btDispatcherInfo_setTimeStep(btDispatcherInfo* obj, btScalar value)
{
	obj->m_timeStep = value;
}

void btDispatcherInfo_setUseContinuous(btDispatcherInfo* obj, bool value)
{
	obj->m_useContinuous = value;
}

void btDispatcherInfo_setUseConvexConservativeDistanceUtil(btDispatcherInfo* obj,
	bool value)
{
	obj->m_useConvexConservativeDistanceUtil = value;
}

void btDispatcherInfo_setUseEpa(btDispatcherInfo* obj, bool value)
{
	obj->m_useEpa = value;
}


void* btDispatcher_allocateCollisionAlgorithm(btDispatcher* obj, int size)
{
	return obj->allocateCollisionAlgorithm(size);
}

void btDispatcher_clearManifold(btDispatcher* obj, btPersistentManifold* manifold)
{
	obj->clearManifold(manifold);
}

void btDispatcher_dispatchAllCollisionPairs(btDispatcher* obj, btOverlappingPairCache* pairCache,
	const btDispatcherInfo* dispatchInfo, btDispatcher* dispatcher)
{
	obj->dispatchAllCollisionPairs(pairCache, *dispatchInfo, dispatcher);
}

btCollisionAlgorithm* btDispatcher_findAlgorithm(btDispatcher* obj, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap, btPersistentManifold* sharedManifold,
	ebtDispatcherQueryType queryType)
{
	return obj->findAlgorithm(body0Wrap, body1Wrap, sharedManifold, queryType);
}

void btDispatcher_freeCollisionAlgorithm(btDispatcher* obj, void* ptr)
{
	obj->freeCollisionAlgorithm(ptr);
}

btPersistentManifold** btDispatcher_getInternalManifoldPointer(btDispatcher* obj)
{
	return obj->getInternalManifoldPointer();
}

btPoolAllocator* btDispatcher_getInternalManifoldPool(btDispatcher* obj)
{
	return obj->getInternalManifoldPool();
}

btPersistentManifold* btDispatcher_getManifoldByIndexInternal(btDispatcher* obj,
	int index)
{
	return obj->getManifoldByIndexInternal(index);
}

btPersistentManifold* btDispatcher_getNewManifold(btDispatcher* obj, const btCollisionObject* b0,
	const btCollisionObject* b1)
{
	return obj->getNewManifold(b0, b1);
}

int btDispatcher_getNumManifolds(btDispatcher* obj)
{
	return obj->getNumManifolds();
}

bool btDispatcher_needsCollision(btDispatcher* obj, const btCollisionObject* body0,
	const btCollisionObject* body1)
{
	return obj->needsCollision(body0, body1);
}

bool btDispatcher_needsResponse(btDispatcher* obj, const btCollisionObject* body0,
	const btCollisionObject* body1)
{
	return obj->needsResponse(body0, body1);
}

void btDispatcher_releaseManifold(btDispatcher* obj, btPersistentManifold* manifold)
{
	obj->releaseManifold(manifold);
}

void btDispatcher_delete(btDispatcher* obj)
{
	delete obj;
}
