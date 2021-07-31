#include <BulletCollision/CollisionDispatch/btGhostObject.h>
#include <BulletCollision/CollisionShapes/btConvexShape.h>

#include "conversion.h"
#include "btGhostObject_wrap.h"

btGhostObject* btGhostObject_new()
{
	return new btGhostObject();
}

void btGhostObject_addOverlappingObjectInternal(btGhostObject* obj, btBroadphaseProxy* otherProxy,
	btBroadphaseProxy* thisProxy)
{
	obj->addOverlappingObjectInternal(otherProxy, thisProxy);
}

void btGhostObject_convexSweepTest(btGhostObject* obj, const btConvexShape* castShape,
	const btTransform* convexFromWorld, const btTransform* convexToWorld, btCollisionWorld_ConvexResultCallback* resultCallback,
	btScalar allowedCcdPenetration)
{
	BTTRANSFORM_IN(convexFromWorld);
	BTTRANSFORM_IN(convexToWorld);
	obj->convexSweepTest(castShape, BTTRANSFORM_USE(convexFromWorld), BTTRANSFORM_USE(convexToWorld),
		*resultCallback, allowedCcdPenetration);
}

int btGhostObject_getNumOverlappingObjects(btGhostObject* obj)
{
	return obj->getNumOverlappingObjects();
}

btCollisionObject* btGhostObject_getOverlappingObject(btGhostObject* obj, int index)
{
	return obj->getOverlappingObject(index);
}

btAlignedObjectArray_btCollisionObjectPtr* btGhostObject_getOverlappingPairs(btGhostObject* obj)
{
	return &obj->getOverlappingPairs();
}

void btGhostObject_rayTest(btGhostObject* obj, const btVector3* rayFromWorld, const btVector3* rayToWorld,
	btCollisionWorld_RayResultCallback* resultCallback)
{
	BTVECTOR3_IN(rayFromWorld);
	BTVECTOR3_IN(rayToWorld);
	obj->rayTest(BTVECTOR3_USE(rayFromWorld), BTVECTOR3_USE(rayToWorld), *resultCallback);
}

void btGhostObject_removeOverlappingObjectInternal(btGhostObject* obj, btBroadphaseProxy* otherProxy,
	btDispatcher* dispatcher, btBroadphaseProxy* thisProxy)
{
	obj->removeOverlappingObjectInternal(otherProxy, dispatcher, thisProxy);
}

btGhostObject* btGhostObject_upcast(btCollisionObject* colObj)
{
	return btGhostObject::upcast(colObj);
}


btPairCachingGhostObject* btPairCachingGhostObject_new()
{
	return new btPairCachingGhostObject();
}

btHashedOverlappingPairCache* btPairCachingGhostObject_getOverlappingPairCache(btPairCachingGhostObject* obj)
{
	return obj->getOverlappingPairCache();
}


btGhostPairCallback* btGhostPairCallback_new()
{
	return new btGhostPairCallback();
}
