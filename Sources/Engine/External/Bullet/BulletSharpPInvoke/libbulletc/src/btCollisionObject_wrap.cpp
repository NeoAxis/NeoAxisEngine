#include <BulletCollision/BroadphaseCollision/btBroadphaseProxy.h>
#include <BulletCollision/CollisionDispatch/btCollisionObject.h>
#include <BulletCollision/CollisionShapes/btCollisionShape.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btCollisionObject_wrap.h"

btCollisionObject* btCollisionObject_new()
{
	return new btCollisionObject();
}

void btCollisionObject_activate(btCollisionObject* obj, bool forceActivation)
{
	obj->activate(forceActivation);
}

int btCollisionObject_calculateSerializeBufferSize(btCollisionObject* obj)
{
	return obj->calculateSerializeBufferSize();
}

bool btCollisionObject_checkCollideWith(btCollisionObject* obj, const btCollisionObject* co)
{
	return obj->checkCollideWith(co);
}

bool btCollisionObject_checkCollideWithOverride(btCollisionObject* obj, const btCollisionObject* co)
{
	return obj->checkCollideWithOverride(co);
}

void btCollisionObject_forceActivationState(btCollisionObject* obj, int newState)
{
	obj->forceActivationState(newState);
}

int btCollisionObject_getActivationState(btCollisionObject* obj)
{
	return obj->getActivationState();
}

void btCollisionObject_getAnisotropicFriction(btCollisionObject* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAnisotropicFriction());
}

btBroadphaseProxy* btCollisionObject_getBroadphaseHandle(btCollisionObject* obj)
{
	return obj->getBroadphaseHandle();
}

btScalar btCollisionObject_getCcdMotionThreshold(btCollisionObject* obj)
{
	return obj->getCcdMotionThreshold();
}

btScalar btCollisionObject_getCcdSquareMotionThreshold(btCollisionObject* obj)
{
	return obj->getCcdSquareMotionThreshold();
}

btScalar btCollisionObject_getCcdSweptSphereRadius(btCollisionObject* obj)
{
	return obj->getCcdSweptSphereRadius();
}

int btCollisionObject_getCollisionFlags(btCollisionObject* obj)
{
	return obj->getCollisionFlags();
}

btCollisionShape* btCollisionObject_getCollisionShape(btCollisionObject* obj)
{
	return obj->getCollisionShape();
}

int btCollisionObject_getCompanionId(btCollisionObject* obj)
{
	return obj->getCompanionId();
}

btScalar btCollisionObject_getContactDamping(btCollisionObject* obj)
{
	return obj->getContactDamping();
}

btScalar btCollisionObject_getContactProcessingThreshold(btCollisionObject* obj)
{
	return obj->getContactProcessingThreshold();
}

btScalar btCollisionObject_getContactStiffness(btCollisionObject* obj)
{
	return obj->getContactStiffness();
}

bool btCollisionObject_getCustomDebugColor(btCollisionObject* obj, btVector3* colorRGB)
{
	return obj->getCustomDebugColor(*colorRGB);
}

btScalar btCollisionObject_getDeactivationTime(btCollisionObject* obj)
{
	return obj->getDeactivationTime();
}

btScalar btCollisionObject_getFriction(btCollisionObject* obj)
{
	return obj->getFriction();
}

btScalar btCollisionObject_getHitFraction(btCollisionObject* obj)
{
	return obj->getHitFraction();
}

int btCollisionObject_getInternalType(btCollisionObject* obj)
{
	return obj->getInternalType();
}

void btCollisionObject_getInterpolationAngularVelocity(btCollisionObject* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getInterpolationAngularVelocity());
}

void btCollisionObject_getInterpolationLinearVelocity(btCollisionObject* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getInterpolationLinearVelocity());
}

void btCollisionObject_getInterpolationWorldTransform(btCollisionObject* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getInterpolationWorldTransform());
}

int btCollisionObject_getIslandTag(btCollisionObject* obj)
{
	return obj->getIslandTag();
}

btScalar btCollisionObject_getRestitution(btCollisionObject* obj)
{
	return obj->getRestitution();
}

btScalar btCollisionObject_getRollingFriction(btCollisionObject* obj)
{
	return obj->getRollingFriction();
}

btScalar btCollisionObject_getSpinningFriction(btCollisionObject* obj)
{
	return obj->getSpinningFriction();
}

int btCollisionObject_getWorldArrayIndex(btCollisionObject * obj)
{
	return obj->getWorldArrayIndex();
}

int btCollisionObject_getUserIndex(btCollisionObject* obj)
{
	return obj->getUserIndex();
}

int btCollisionObject_getUserIndex2(btCollisionObject* obj)
{
	return obj->getUserIndex2();
}

void* btCollisionObject_getUserPointer(btCollisionObject* obj)
{
	return obj->getUserPointer();
}

void btCollisionObject_getWorldTransform(btCollisionObject* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getWorldTransform());
}

bool btCollisionObject_hasAnisotropicFriction(btCollisionObject* obj, int frictionMode)
{
	return obj->hasAnisotropicFriction(frictionMode);
}

bool btCollisionObject_hasContactResponse(btCollisionObject* obj)
{
	return obj->hasContactResponse();
}

void* btCollisionObject_internalGetExtensionPointer(btCollisionObject* obj)
{
	return obj->internalGetExtensionPointer();
}

void btCollisionObject_internalSetExtensionPointer(btCollisionObject* obj, void* pointer)
{
	obj->internalSetExtensionPointer(pointer);
}

bool btCollisionObject_isActive(btCollisionObject* obj)
{
	return obj->isActive();
}

bool btCollisionObject_isKinematicObject(btCollisionObject* obj)
{
	return obj->isKinematicObject();
}

bool btCollisionObject_isStaticObject(btCollisionObject* obj)
{
	return obj->isStaticObject();
}

bool btCollisionObject_isStaticOrKinematicObject(btCollisionObject* obj)
{
	return obj->isStaticOrKinematicObject();
}

bool btCollisionObject_mergesSimulationIslands(btCollisionObject* obj)
{
	return obj->mergesSimulationIslands();
}

void btCollisionObject_removeCustomDebugColor(btCollisionObject * obj)
{
	obj->removeCustomDebugColor();
}

const char* btCollisionObject_serialize(btCollisionObject* obj, void* dataBuffer,
	btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btCollisionObject_serializeSingleObject(btCollisionObject* obj, btSerializer* serializer)
{
	obj->serializeSingleObject(serializer);
}

void btCollisionObject_setActivationState(btCollisionObject* obj, int newState)
{
	obj->setActivationState(newState);
}

void btCollisionObject_setAnisotropicFriction(btCollisionObject* obj, const btVector3* anisotropicFriction,
	int frictionMode)
{
	BTVECTOR3_IN(anisotropicFriction);
	obj->setAnisotropicFriction(BTVECTOR3_USE(anisotropicFriction), frictionMode);
}

void btCollisionObject_setBroadphaseHandle(btCollisionObject* obj, btBroadphaseProxy* handle)
{
	obj->setBroadphaseHandle(handle);
}

void btCollisionObject_setCcdMotionThreshold(btCollisionObject* obj, btScalar ccdMotionThreshold)
{
	obj->setCcdMotionThreshold(ccdMotionThreshold);
}

void btCollisionObject_setCcdSweptSphereRadius(btCollisionObject* obj, btScalar radius)
{
	obj->setCcdSweptSphereRadius(radius);
}

void btCollisionObject_setCollisionFlags(btCollisionObject* obj, int flags)
{
	obj->setCollisionFlags(flags);
}

void btCollisionObject_setCollisionShape(btCollisionObject* obj, btCollisionShape* collisionShape)
{
	obj->setCollisionShape(collisionShape);
}

void btCollisionObject_setCompanionId(btCollisionObject* obj, int id)
{
	obj->setCompanionId(id);
}

void btCollisionObject_setContactProcessingThreshold(btCollisionObject* obj, btScalar contactProcessingThreshold)
{
	obj->setContactProcessingThreshold(contactProcessingThreshold);
}

void btCollisionObject_setContactStiffnessAndDamping(btCollisionObject* obj, btScalar stiffness,
	btScalar damping)
{
	obj->setContactStiffnessAndDamping(stiffness, damping);
}

void btCollisionObject_setCustomDebugColor(btCollisionObject* obj, const btVector3* colorRGB)
{
	obj->setCustomDebugColor(*colorRGB);
}

void btCollisionObject_setDeactivationTime(btCollisionObject* obj, btScalar time)
{
	obj->setDeactivationTime(time);
}

void btCollisionObject_setFriction(btCollisionObject* obj, btScalar frict)
{
	obj->setFriction(frict);
}

void btCollisionObject_setHitFraction(btCollisionObject* obj, btScalar hitFraction)
{
	obj->setHitFraction(hitFraction);
}

void btCollisionObject_setIgnoreCollisionCheck(btCollisionObject* obj, const btCollisionObject* co,
	bool ignoreCollisionCheck)
{
	obj->setIgnoreCollisionCheck(co, ignoreCollisionCheck);
}

void btCollisionObject_setInterpolationAngularVelocity(btCollisionObject* obj, const btVector3* angvel)
{
	BTVECTOR3_IN(angvel);
	obj->setInterpolationAngularVelocity(BTVECTOR3_USE(angvel));
}

void btCollisionObject_setInterpolationLinearVelocity(btCollisionObject* obj, const btVector3* linvel)
{
	BTVECTOR3_IN(linvel);
	obj->setInterpolationLinearVelocity(BTVECTOR3_USE(linvel));
}

void btCollisionObject_setInterpolationWorldTransform(btCollisionObject* obj, const btTransform* trans)
{
	BTTRANSFORM_IN(trans);
	obj->setInterpolationWorldTransform(BTTRANSFORM_USE(trans));
}

void btCollisionObject_setIslandTag(btCollisionObject* obj, int tag)
{
	obj->setIslandTag(tag);
}

void btCollisionObject_setRestitution(btCollisionObject* obj, btScalar rest)
{
	obj->setRestitution(rest);
}

void btCollisionObject_setRollingFriction(btCollisionObject* obj, btScalar frict)
{
	obj->setRollingFriction(frict);
}

void btCollisionObject_setSpinningFriction(btCollisionObject* obj, btScalar frict)
{
	obj->setSpinningFriction(frict);
}

void btCollisionObject_setWorldArrayIndex(btCollisionObject * obj, int ix)
{
	obj->setWorldArrayIndex(ix);
}

void btCollisionObject_setUserIndex(btCollisionObject* obj, int index)
{
	obj->setUserIndex(index);
}

void btCollisionObject_setUserIndex2(btCollisionObject* obj, int index)
{
	obj->setUserIndex2(index);
}

void btCollisionObject_setUserPointer(btCollisionObject* obj, void* userPointer)
{
	obj->setUserPointer(userPointer);
}

void btCollisionObject_setWorldTransform(btCollisionObject* obj, const btTransform* worldTrans)
{
	BTTRANSFORM_IN(worldTrans);
	obj->setWorldTransform(BTTRANSFORM_USE(worldTrans));
}

void btCollisionObject_delete(btCollisionObject* obj)
{
	delete obj;
}
