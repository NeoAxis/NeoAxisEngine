#include <BulletCollision/NarrowPhaseCollision/btDiscreteCollisionDetectorInterface.h>
#include <LinearMath/btIDebugDraw.h>

#include "conversion.h"
#include "btDiscreteCollisionDetectorInterface_wrap.h"

btDiscreteCollisionDetectorInterface_ClosestPointInput* btDiscreteCollisionDetectorInterface_ClosestPointInput_new()
{
	return new btDiscreteCollisionDetectorInterface::ClosestPointInput();
}

btScalar btDiscreteCollisionDetectorInterface_ClosestPointInput_getMaximumDistanceSquared(
	btDiscreteCollisionDetectorInterface_ClosestPointInput* obj)
{
	return obj->m_maximumDistanceSquared;
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_getTransformA(btDiscreteCollisionDetectorInterface_ClosestPointInput* obj,
	btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_transformA);
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_getTransformB(btDiscreteCollisionDetectorInterface_ClosestPointInput* obj,
	btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_transformB);
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_setMaximumDistanceSquared(
	btDiscreteCollisionDetectorInterface_ClosestPointInput* obj, btScalar value)
{
	obj->m_maximumDistanceSquared = value;
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_setTransformA(btDiscreteCollisionDetectorInterface_ClosestPointInput* obj,
	const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_transformA, value);
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_setTransformB(btDiscreteCollisionDetectorInterface_ClosestPointInput* obj,
	const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_transformB, value);
}

void btDiscreteCollisionDetectorInterface_ClosestPointInput_delete(btDiscreteCollisionDetectorInterface_ClosestPointInput* obj)
{
	delete obj;
}


void btDiscreteCollisionDetectorInterface_Result_addContactPoint(btDiscreteCollisionDetectorInterface_Result* obj,
	const btVector3* normalOnBInWorld, const btVector3* pointInWorld, btScalar depth)
{
	BTVECTOR3_IN(normalOnBInWorld);
	BTVECTOR3_IN(pointInWorld);
	obj->addContactPoint(BTVECTOR3_USE(normalOnBInWorld), BTVECTOR3_USE(pointInWorld),
		depth);
}

void btDiscreteCollisionDetectorInterface_Result_setShapeIdentifiersA(btDiscreteCollisionDetectorInterface_Result* obj,
	int partId0, int index0)
{
	obj->setShapeIdentifiersA(partId0, index0);
}

void btDiscreteCollisionDetectorInterface_Result_setShapeIdentifiersB(btDiscreteCollisionDetectorInterface_Result* obj,
	int partId1, int index1)
{
	obj->setShapeIdentifiersB(partId1, index1);
}

void btDiscreteCollisionDetectorInterface_Result_delete(btDiscreteCollisionDetectorInterface_Result* obj)
{
	delete obj;
}


void btDiscreteCollisionDetectorInterface_getClosestPoints(btDiscreteCollisionDetectorInterface* obj,
	const btDiscreteCollisionDetectorInterface_ClosestPointInput* input, btDiscreteCollisionDetectorInterface_Result* output,
	btIDebugDraw* debugDraw, bool swapResults)
{
	obj->getClosestPoints(*input, *output, debugDraw, swapResults);
}

void btDiscreteCollisionDetectorInterface_delete(btDiscreteCollisionDetectorInterface* obj)
{
	delete obj;
}


void btStorageResult_getClosestPointInB(btStorageResult* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_closestPointInB);
}

btScalar btStorageResult_getDistance(btStorageResult* obj)
{
	return obj->m_distance;
}

void btStorageResult_getNormalOnSurfaceB(btStorageResult* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normalOnSurfaceB);
}

void btStorageResult_setClosestPointInB(btStorageResult* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_closestPointInB, value);
}

void btStorageResult_setDistance(btStorageResult* obj, btScalar value)
{
	obj->m_distance = value;
}

void btStorageResult_setNormalOnSurfaceB(btStorageResult* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normalOnSurfaceB, value);
}
