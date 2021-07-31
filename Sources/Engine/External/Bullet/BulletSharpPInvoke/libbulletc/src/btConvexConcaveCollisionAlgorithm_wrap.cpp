#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletCollision/CollisionDispatch/btConvexConcaveCollisionAlgorithm.h>
#include <BulletCollision/CollisionDispatch/btManifoldResult.h>

#include "conversion.h"
#include "btConvexConcaveCollisionAlgorithm_wrap.h"

btConvexTriangleCallback* btConvexTriangleCallback_new(btDispatcher* dispatcher,
	const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	bool isSwapped)
{
	return new btConvexTriangleCallback(dispatcher, body0Wrap, body1Wrap, isSwapped);
}

void btConvexTriangleCallback_clearCache(btConvexTriangleCallback* obj)
{
	obj->clearCache();
}

void btConvexTriangleCallback_clearWrapperData(btConvexTriangleCallback* obj)
{
	obj->clearWrapperData();
}

void btConvexTriangleCallback_getAabbMax(btConvexTriangleCallback* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAabbMax());
}

void btConvexTriangleCallback_getAabbMin(btConvexTriangleCallback* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAabbMin());
}

btPersistentManifold* btConvexTriangleCallback_getManifoldPtr(btConvexTriangleCallback* obj)
{
	return obj->m_manifoldPtr;
}

int btConvexTriangleCallback_getTriangleCount(btConvexTriangleCallback* obj)
{
	return obj->m_triangleCount;
}

void btConvexTriangleCallback_setManifoldPtr(btConvexTriangleCallback* obj, btPersistentManifold* value)
{
	obj->m_manifoldPtr = value;
}

void btConvexTriangleCallback_setTimeStepAndCounters(btConvexTriangleCallback* obj,
	btScalar collisionMarginTriangle, const btDispatcherInfo* dispatchInfo, const btCollisionObjectWrapper* convexBodyWrap,
	const btCollisionObjectWrapper* triBodyWrap, btManifoldResult* resultOut)
{
	obj->setTimeStepAndCounters(collisionMarginTriangle, *dispatchInfo, convexBodyWrap,
		triBodyWrap, resultOut);
}

void btConvexTriangleCallback_setTriangleCount(btConvexTriangleCallback* obj, int value)
{
	obj->m_triangleCount = value;
}


btConvexConcaveCollisionAlgorithm_CreateFunc* btConvexConcaveCollisionAlgorithm_CreateFunc_new()
{
	return new btConvexConcaveCollisionAlgorithm::CreateFunc();
}


btConvexConcaveCollisionAlgorithm_SwappedCreateFunc* btConvexConcaveCollisionAlgorithm_SwappedCreateFunc_new()
{
	return new btConvexConcaveCollisionAlgorithm::SwappedCreateFunc();
}


btConvexConcaveCollisionAlgorithm* btConvexConcaveCollisionAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci,
	const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	bool isSwapped)
{
	return new btConvexConcaveCollisionAlgorithm(*ci, body0Wrap, body1Wrap, isSwapped);
}

void btConvexConcaveCollisionAlgorithm_clearCache(btConvexConcaveCollisionAlgorithm* obj)
{
	obj->clearCache();
}
