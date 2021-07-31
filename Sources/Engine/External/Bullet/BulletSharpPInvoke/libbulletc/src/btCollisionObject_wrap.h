#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btCollisionObject* btCollisionObject_new();
	EXPORT void btCollisionObject_activate(btCollisionObject* obj, bool forceActivation);
	EXPORT int btCollisionObject_calculateSerializeBufferSize(btCollisionObject* obj);
	EXPORT bool btCollisionObject_checkCollideWith(btCollisionObject* obj, const btCollisionObject* co);
	EXPORT bool btCollisionObject_checkCollideWithOverride(btCollisionObject* obj, const btCollisionObject* co);
	EXPORT void btCollisionObject_forceActivationState(btCollisionObject* obj, int newState);
	EXPORT int btCollisionObject_getActivationState(btCollisionObject* obj);
	EXPORT void btCollisionObject_getAnisotropicFriction(btCollisionObject* obj, btVector3* value);
	EXPORT btBroadphaseProxy* btCollisionObject_getBroadphaseHandle(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getCcdMotionThreshold(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getCcdSquareMotionThreshold(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getCcdSweptSphereRadius(btCollisionObject* obj);
	EXPORT int btCollisionObject_getCollisionFlags(btCollisionObject* obj);
	EXPORT btCollisionShape* btCollisionObject_getCollisionShape(btCollisionObject* obj);
	EXPORT int btCollisionObject_getCompanionId(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getContactDamping(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getContactProcessingThreshold(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getContactStiffness(btCollisionObject* obj);
	EXPORT bool btCollisionObject_getCustomDebugColor(btCollisionObject* obj, btVector3* colorRGB);
	EXPORT btScalar btCollisionObject_getDeactivationTime(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getFriction(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getHitFraction(btCollisionObject* obj);
	EXPORT int btCollisionObject_getInternalType(btCollisionObject* obj);
	EXPORT void btCollisionObject_getInterpolationAngularVelocity(btCollisionObject* obj, btVector3* value);
	EXPORT void btCollisionObject_getInterpolationLinearVelocity(btCollisionObject* obj, btVector3* value);
	EXPORT void btCollisionObject_getInterpolationWorldTransform(btCollisionObject* obj, btTransform* value);
	EXPORT int btCollisionObject_getIslandTag(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getRestitution(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getRollingFriction(btCollisionObject* obj);
	EXPORT btScalar btCollisionObject_getSpinningFriction(btCollisionObject* obj);
	EXPORT int btCollisionObject_getWorldArrayIndex(btCollisionObject* obj);
	EXPORT int btCollisionObject_getUserIndex(btCollisionObject* obj);
	EXPORT int btCollisionObject_getUserIndex2(btCollisionObject* obj);
	EXPORT void* btCollisionObject_getUserPointer(btCollisionObject* obj);
	EXPORT void btCollisionObject_getWorldTransform(btCollisionObject* obj, btTransform* value);
	EXPORT bool btCollisionObject_hasAnisotropicFriction(btCollisionObject* obj, int frictionMode);
	EXPORT bool btCollisionObject_hasContactResponse(btCollisionObject* obj);
	EXPORT void* btCollisionObject_internalGetExtensionPointer(btCollisionObject* obj);
	EXPORT void btCollisionObject_internalSetExtensionPointer(btCollisionObject* obj, void* pointer);
	EXPORT bool btCollisionObject_isActive(btCollisionObject* obj);
	EXPORT bool btCollisionObject_isKinematicObject(btCollisionObject* obj);
	EXPORT bool btCollisionObject_isStaticObject(btCollisionObject* obj);
	EXPORT bool btCollisionObject_isStaticOrKinematicObject(btCollisionObject* obj);
	EXPORT bool btCollisionObject_mergesSimulationIslands(btCollisionObject* obj);
	EXPORT void btCollisionObject_removeCustomDebugColor(btCollisionObject* obj);
	EXPORT const char* btCollisionObject_serialize(btCollisionObject* obj, void* dataBuffer, btSerializer* serializer);
	EXPORT void btCollisionObject_serializeSingleObject(btCollisionObject* obj, btSerializer* serializer);
	EXPORT void btCollisionObject_setActivationState(btCollisionObject* obj, int newState);
	EXPORT void btCollisionObject_setAnisotropicFriction(btCollisionObject* obj, const btVector3* anisotropicFriction, int frictionMode);
	EXPORT void btCollisionObject_setBroadphaseHandle(btCollisionObject* obj, btBroadphaseProxy* handle);
	EXPORT void btCollisionObject_setCcdMotionThreshold(btCollisionObject* obj, btScalar ccdMotionThreshold);
	EXPORT void btCollisionObject_setCcdSweptSphereRadius(btCollisionObject* obj, btScalar radius);
	EXPORT void btCollisionObject_setCollisionFlags(btCollisionObject* obj, int flags);
	EXPORT void btCollisionObject_setCollisionShape(btCollisionObject* obj, btCollisionShape* collisionShape);
	EXPORT void btCollisionObject_setCompanionId(btCollisionObject* obj, int id);
	EXPORT void btCollisionObject_setContactProcessingThreshold(btCollisionObject* obj, btScalar contactProcessingThreshold);
	EXPORT void btCollisionObject_setContactStiffnessAndDamping(btCollisionObject* obj, btScalar stiffness, btScalar damping);
	EXPORT void btCollisionObject_setCustomDebugColor(btCollisionObject* obj, const btVector3* colorRGB);
	EXPORT void btCollisionObject_setDeactivationTime(btCollisionObject* obj, btScalar time);
	EXPORT void btCollisionObject_setFriction(btCollisionObject* obj, btScalar frict);
	EXPORT void btCollisionObject_setHitFraction(btCollisionObject* obj, btScalar hitFraction);
	EXPORT void btCollisionObject_setIgnoreCollisionCheck(btCollisionObject* obj, const btCollisionObject* co, bool ignoreCollisionCheck);
	EXPORT void btCollisionObject_setInterpolationAngularVelocity(btCollisionObject* obj, const btVector3* angvel);
	EXPORT void btCollisionObject_setInterpolationLinearVelocity(btCollisionObject* obj, const btVector3* linvel);
	EXPORT void btCollisionObject_setInterpolationWorldTransform(btCollisionObject* obj, const btTransform* trans);
	EXPORT void btCollisionObject_setIslandTag(btCollisionObject* obj, int tag);
	EXPORT void btCollisionObject_setRestitution(btCollisionObject* obj, btScalar rest);
	EXPORT void btCollisionObject_setRollingFriction(btCollisionObject* obj, btScalar frict);
	EXPORT void btCollisionObject_setSpinningFriction(btCollisionObject* obj, btScalar frict);
	EXPORT void btCollisionObject_setWorldArrayIndex(btCollisionObject* obj, int ix);
	EXPORT void btCollisionObject_setUserIndex(btCollisionObject* obj, int index);
	EXPORT void btCollisionObject_setUserIndex2(btCollisionObject* obj, int index);
	EXPORT void btCollisionObject_setUserPointer(btCollisionObject* obj, void* userPointer);
	EXPORT void btCollisionObject_setWorldTransform(btCollisionObject* obj, const btTransform* worldTrans);
	EXPORT void btCollisionObject_delete(btCollisionObject* obj);
#ifdef __cplusplus
}
#endif
