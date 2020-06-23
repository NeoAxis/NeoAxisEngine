#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btSpatialMotionVector* btMultibodyLink_getAbsFrameLocVelocity(btMultibodyLink* obj);
	EXPORT btSpatialMotionVector* btMultibodyLink_getAbsFrameTotVelocity(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_getAppliedConstraintForce(btMultibodyLink* obj, btVector3* value);
	EXPORT void btMultibodyLink_getAppliedConstraintTorque(btMultibodyLink* obj, btVector3* value);
	EXPORT void btMultibodyLink_getAppliedForce(btMultibodyLink* obj, btVector3* value);
	EXPORT void btMultibodyLink_getAppliedTorque(btMultibodyLink* obj, btVector3* value);
	EXPORT btSpatialMotionVector* btMultibodyLink_getAxes(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_getAxisBottom(btMultibodyLink* obj, int dof, btVector3* value);
	EXPORT void btMultibodyLink_getAxisTop(btMultibodyLink* obj, int dof, btVector3* value);
	EXPORT void btMultibodyLink_getCachedRotParentToThis(btMultibodyLink* obj, btQuaternion* value);
	EXPORT void btMultibodyLink_getCachedRVector(btMultibodyLink* obj, btVector3* value);
	EXPORT void btMultibodyLink_getCachedWorldTransform(btMultibodyLink* obj, btTransform* value);
	EXPORT int btMultibodyLink_getCfgOffset(btMultibodyLink* obj);
	EXPORT btMultiBodyLinkCollider* btMultibodyLink_getCollider(btMultibodyLink* obj);
	EXPORT int btMultibodyLink_getDofCount(btMultibodyLink* obj);
	EXPORT int btMultibodyLink_getDofOffset(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_getDVector(btMultibodyLink* obj, btVector3* value);
	EXPORT void btMultibodyLink_getEVector(btMultibodyLink* obj, btVector3* value);
	EXPORT int btMultibodyLink_getFlags(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_getInertiaLocal(btMultibodyLink* obj, btVector3* value);
	EXPORT btScalar btMultibodyLink_getJointDamping(btMultibodyLink* obj);
	EXPORT btMultiBodyJointFeedback* btMultibodyLink_getJointFeedback(btMultibodyLink* obj);
	EXPORT btScalar btMultibodyLink_getJointFriction(btMultibodyLink* obj);
	EXPORT const char* btMultibodyLink_getJointName(btMultibodyLink* obj);
	EXPORT btScalar* btMultibodyLink_getJointPos(btMultibodyLink* obj);
	EXPORT btScalar* btMultibodyLink_getJointTorque(btMultibodyLink* obj);
	EXPORT btMultibodyLink_eFeatherstoneJointType btMultibodyLink_getJointType(btMultibodyLink* obj);
	EXPORT const char* btMultibodyLink_getLinkName(btMultibodyLink* obj);
	EXPORT btScalar btMultibodyLink_getMass(btMultibodyLink* obj);
	EXPORT int btMultibodyLink_getParent(btMultibodyLink* obj);
	EXPORT int btMultibodyLink_getPosVarCount(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_getZeroRotParentToThis(btMultibodyLink* obj, btQuaternion* value);
	EXPORT const void* btMultibodyLink_getUserPtr(btMultibodyLink* obj);
	EXPORT void btMultibodyLink_setAppliedConstraintForce(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setAppliedConstraintTorque(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setAppliedForce(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setAppliedTorque(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setAxisBottom(btMultibodyLink* obj, int dof, btScalar x, btScalar y, btScalar z);
	EXPORT void btMultibodyLink_setAxisBottom2(btMultibodyLink* obj, int dof, const btVector3* axis);
	EXPORT void btMultibodyLink_setAxisTop(btMultibodyLink* obj, int dof, btScalar x, btScalar y, btScalar z);
	EXPORT void btMultibodyLink_setAxisTop2(btMultibodyLink* obj, int dof, const btVector3* axis);
	EXPORT void btMultibodyLink_setCachedRotParentToThis(btMultibodyLink* obj, const btQuaternion* value);
	EXPORT void btMultibodyLink_setCachedRVector(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setCachedWorldTransform(btMultibodyLink* obj, const btTransform* value);
	EXPORT void btMultibodyLink_setCfgOffset(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setCollider(btMultibodyLink* obj, btMultiBodyLinkCollider* value);
	EXPORT void btMultibodyLink_setDofCount(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setDofOffset(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setDVector(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setEVector(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setFlags(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setInertiaLocal(btMultibodyLink* obj, const btVector3* value);
	EXPORT void btMultibodyLink_setJointDamping(btMultibodyLink* obj, btScalar value);
	EXPORT void btMultibodyLink_setJointFeedback(btMultibodyLink* obj, btMultiBodyJointFeedback* value);
	EXPORT void btMultibodyLink_setJointFriction(btMultibodyLink* obj, btScalar value);
	EXPORT void btMultibodyLink_setJointName(btMultibodyLink* obj, const char* value);
	EXPORT void btMultibodyLink_setJointType(btMultibodyLink* obj, btMultibodyLink_eFeatherstoneJointType value);
	EXPORT void btMultibodyLink_setLinkName(btMultibodyLink* obj, const char* value);
	EXPORT void btMultibodyLink_setMass(btMultibodyLink* obj, btScalar value);
	EXPORT void btMultibodyLink_setParent(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setPosVarCount(btMultibodyLink* obj, int value);
	EXPORT void btMultibodyLink_setZeroRotParentToThis(btMultibodyLink* obj, const btQuaternion* value);
	EXPORT void btMultibodyLink_setUserPtr(btMultibodyLink* obj, const void* value);
	EXPORT void btMultibodyLink_updateCacheMultiDof(btMultibodyLink* obj, btScalar* pq);
#ifdef __cplusplus
}
#endif
