#include <BulletCollision/CollisionShapes/btCollisionShape.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btCollisionShape_wrap.h"

void btCollisionShape_calculateLocalInertia(btCollisionShape* obj, btScalar mass,
	btVector3* inertia)
{
	BTVECTOR3_DEF(inertia);
	obj->calculateLocalInertia(mass, BTVECTOR3_USE(inertia));
	BTVECTOR3_DEF_OUT(inertia);
}

int btCollisionShape_calculateSerializeBufferSize(btCollisionShape* obj)
{
	return obj->calculateSerializeBufferSize();
}

void btCollisionShape_calculateTemporalAabb(btCollisionShape* obj, const btTransform* curTrans,
	const btVector3* linvel, const btVector3* angvel, btScalar timeStep, btVector3* temporalAabbMin,
	btVector3* temporalAabbMax)
{
	BTTRANSFORM_IN(curTrans);
	BTVECTOR3_IN(linvel);
	BTVECTOR3_IN(angvel);
	BTVECTOR3_DEF(temporalAabbMin);
	BTVECTOR3_DEF(temporalAabbMax);
	obj->calculateTemporalAabb(BTTRANSFORM_USE(curTrans), BTVECTOR3_USE(linvel),
		BTVECTOR3_USE(angvel), timeStep, BTVECTOR3_USE(temporalAabbMin), BTVECTOR3_USE(temporalAabbMax));
	BTVECTOR3_DEF_OUT(temporalAabbMin);
	BTVECTOR3_DEF_OUT(temporalAabbMax);
}

void btCollisionShape_getAabb(btCollisionShape* obj, const btTransform* t, btVector3* aabbMin,
	btVector3* aabbMax)
{
	BTTRANSFORM_IN(t);
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getAabb(BTTRANSFORM_USE(t), BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

btScalar btCollisionShape_getAngularMotionDisc(btCollisionShape* obj)
{
	return obj->getAngularMotionDisc();
}

void btCollisionShape_getAnisotropicRollingFrictionDirection(btCollisionShape* obj,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAnisotropicRollingFrictionDirection();
	BTVECTOR3_SET(value, temp);
}

void btCollisionShape_getBoundingSphere(btCollisionShape* obj, btVector3* center,
	btScalar* radius)
{
	BTVECTOR3_DEF(center);
	obj->getBoundingSphere(BTVECTOR3_USE(center), *radius);
	BTVECTOR3_DEF_OUT(center);
}

btScalar btCollisionShape_getContactBreakingThreshold(btCollisionShape* obj, btScalar defaultContactThresholdFactor)
{
	return obj->getContactBreakingThreshold(defaultContactThresholdFactor);
}

void btCollisionShape_getLocalScaling(btCollisionShape* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLocalScaling());
}

btScalar btCollisionShape_getMargin(btCollisionShape* obj)
{
	return obj->getMargin();
}

const char* btCollisionShape_getName(btCollisionShape* obj)
{
	return obj->getName();
}

int btCollisionShape_getShapeType(btCollisionShape* obj)
{
	return obj->getShapeType();
}

int btCollisionShape_getUserIndex(btCollisionShape* obj)
{
	return obj->getUserIndex();
}

void* btCollisionShape_getUserPointer(btCollisionShape* obj)
{
	return obj->getUserPointer();
}

bool btCollisionShape_isCompound(btCollisionShape* obj)
{
	return obj->isCompound();
}

bool btCollisionShape_isConcave(btCollisionShape* obj)
{
	return obj->isConcave();
}

bool btCollisionShape_isConvex(btCollisionShape* obj)
{
	return obj->isConvex();
}

bool btCollisionShape_isConvex2d(btCollisionShape* obj)
{
	return obj->isConvex2d();
}

bool btCollisionShape_isInfinite(btCollisionShape* obj)
{
	return obj->isInfinite();
}

bool btCollisionShape_isNonMoving(btCollisionShape* obj)
{
	return obj->isNonMoving();
}

bool btCollisionShape_isPolyhedral(btCollisionShape* obj)
{
	return obj->isPolyhedral();
}

bool btCollisionShape_isSoftBody(btCollisionShape* obj)
{
	return obj->isSoftBody();
}

const char* btCollisionShape_serialize(btCollisionShape* obj, void* dataBuffer, btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btCollisionShape_serializeSingleShape(btCollisionShape* obj, btSerializer* serializer)
{
	obj->serializeSingleShape(serializer);
}

void btCollisionShape_setLocalScaling(btCollisionShape* obj, const btVector3* scaling)
{
	BTVECTOR3_IN(scaling);
	obj->setLocalScaling(BTVECTOR3_USE(scaling));
}

void btCollisionShape_setMargin(btCollisionShape* obj, btScalar margin)
{
	obj->setMargin(margin);
}

void btCollisionShape_setUserIndex(btCollisionShape* obj, int index)
{
	obj->setUserIndex(index);
}

void btCollisionShape_setUserPointer(btCollisionShape* obj, void* userPtr)
{
	obj->setUserPointer(userPtr);
}

void btCollisionShape_delete(btCollisionShape* obj)
{
	delete obj;
}
