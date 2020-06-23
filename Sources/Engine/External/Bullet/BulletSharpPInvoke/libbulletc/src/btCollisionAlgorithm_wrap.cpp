#include <BulletCollision/BroadphaseCollision/btCollisionAlgorithm.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/CollisionDispatch/btCollisionObject.h>
#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/CollisionDispatch/btManifoldResult.h>
#include <BulletCollision/NarrowPhaseCollision/btPersistentManifold.h>

#include "btCollisionAlgorithm_wrap.h"

btCollisionAlgorithmConstructionInfo* btCollisionAlgorithmConstructionInfo_new()
{
	return new btCollisionAlgorithmConstructionInfo();
}

btCollisionAlgorithmConstructionInfo* btCollisionAlgorithmConstructionInfo_new2(btDispatcher* dispatcher,
	int temp)
{
	return new btCollisionAlgorithmConstructionInfo(dispatcher, temp);
}

btDispatcher* btCollisionAlgorithmConstructionInfo_getDispatcher1(btCollisionAlgorithmConstructionInfo* obj)
{
	return obj->m_dispatcher1;
}

btPersistentManifold* btCollisionAlgorithmConstructionInfo_getManifold(btCollisionAlgorithmConstructionInfo* obj)
{
	return obj->m_manifold;
}

void btCollisionAlgorithmConstructionInfo_setDispatcher1(btCollisionAlgorithmConstructionInfo* obj,
	btDispatcher* value)
{
	obj->m_dispatcher1 = value;
}

void btCollisionAlgorithmConstructionInfo_setManifold(btCollisionAlgorithmConstructionInfo* obj,
	btPersistentManifold* value)
{
	obj->m_manifold = value;
}

void btCollisionAlgorithmConstructionInfo_delete(btCollisionAlgorithmConstructionInfo* obj)
{
	delete obj;
}


btScalar btCollisionAlgorithm_calculateTimeOfImpact(btCollisionAlgorithm* obj, btCollisionObject* body0,
	btCollisionObject* body1, const btDispatcherInfo* dispatchInfo, btManifoldResult* resultOut)
{
	return obj->calculateTimeOfImpact(body0, body1, *dispatchInfo, resultOut);
}

void btCollisionAlgorithm_getAllContactManifolds(btCollisionAlgorithm* obj, btAlignedObjectArray_btPersistentManifoldPtr* manifoldArray)
{
	obj->getAllContactManifolds(*manifoldArray);
}

void btCollisionAlgorithm_processCollision(btCollisionAlgorithm* obj, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap, const btDispatcherInfo* dispatchInfo,
	btManifoldResult* resultOut)
{
	obj->processCollision(body0Wrap, body1Wrap, *dispatchInfo, resultOut);
}

void btCollisionAlgorithm_delete(btCollisionAlgorithm* obj)
{
	delete obj;
}
