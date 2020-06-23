#include <BulletDynamics/Featherstone/btMultiBodyJointFeedback.h>
#include <BulletDynamics/Featherstone/btMultiBodyLink.h>
#include <BulletDynamics/Featherstone/btMultiBodyLinkCollider.h>

#include "conversion.h"
#include "btMultiBodyLink_wrap.h"

btSpatialMotionVector* btMultibodyLink_getAbsFrameLocVelocity(btMultibodyLink* obj)
{
	return &obj->m_absFrameLocVelocity;
}

btSpatialMotionVector* btMultibodyLink_getAbsFrameTotVelocity(btMultibodyLink* obj)
{
	return &obj->m_absFrameTotVelocity;
}

void btMultibodyLink_getAppliedConstraintForce(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedConstraintForce);
}

void btMultibodyLink_getAppliedConstraintTorque(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedConstraintTorque);
}

void btMultibodyLink_getAppliedForce(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedForce);
}

void btMultibodyLink_getAppliedTorque(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedTorque);
}

btSpatialMotionVector* btMultibodyLink_getAxes(btMultibodyLink* obj)
{
	return obj->m_axes;
}

void btMultibodyLink_getAxisBottom(btMultibodyLink* obj, int dof, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxisBottom(dof));
}

void btMultibodyLink_getAxisTop(btMultibodyLink* obj, int dof, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxisTop(dof));
}

void btMultibodyLink_getCachedRotParentToThis(btMultibodyLink* obj, btQuaternion* value)
{
	BTQUATERNION_SET(value, obj->m_cachedRotParentToThis);
}

void btMultibodyLink_getCachedRVector(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_cachedRVector);
}

void btMultibodyLink_getCachedWorldTransform(btMultibodyLink* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_cachedWorldTransform);
}

int btMultibodyLink_getCfgOffset(btMultibodyLink* obj)
{
	return obj->m_cfgOffset;
}

btMultiBodyLinkCollider* btMultibodyLink_getCollider(btMultibodyLink* obj)
{
	return obj->m_collider;
}

int btMultibodyLink_getDofCount(btMultibodyLink* obj)
{
	return obj->m_dofCount;
}

int btMultibodyLink_getDofOffset(btMultibodyLink* obj)
{
	return obj->m_dofOffset;
}

void btMultibodyLink_getDVector(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_dVector);
}

void btMultibodyLink_getEVector(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_eVector);
}

int btMultibodyLink_getFlags(btMultibodyLink* obj)
{
	return obj->m_flags;
}

void btMultibodyLink_getInertiaLocal(btMultibodyLink* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_inertiaLocal);
}

btScalar btMultibodyLink_getJointDamping(btMultibodyLink* obj)
{
	return obj->m_jointDamping;
}

btMultiBodyJointFeedback* btMultibodyLink_getJointFeedback(btMultibodyLink* obj)
{
	return obj->m_jointFeedback;
}

btScalar btMultibodyLink_getJointFriction(btMultibodyLink* obj)
{
	return obj->m_jointFriction;
}

const char* btMultibodyLink_getJointName(btMultibodyLink* obj)
{
	return obj->m_jointName;
}

btScalar* btMultibodyLink_getJointPos(btMultibodyLink* obj)
{
	return obj->m_jointPos;
}

btScalar* btMultibodyLink_getJointTorque(btMultibodyLink* obj)
{
	return obj->m_jointTorque;
}

btMultibodyLink::eFeatherstoneJointType btMultibodyLink_getJointType(btMultibodyLink* obj)
{
	return obj->m_jointType;
}

const char* btMultibodyLink_getLinkName(btMultibodyLink* obj)
{
	return obj->m_linkName;
}

btScalar btMultibodyLink_getMass(btMultibodyLink* obj)
{
	return obj->m_mass;
}

int btMultibodyLink_getParent(btMultibodyLink* obj)
{
	return obj->m_parent;
}

int btMultibodyLink_getPosVarCount(btMultibodyLink* obj)
{
	return obj->m_posVarCount;
}

void btMultibodyLink_getZeroRotParentToThis(btMultibodyLink* obj, btQuaternion* value)
{
	BTQUATERNION_SET(value, obj->m_zeroRotParentToThis);
}

const void* btMultibodyLink_getUserPtr(btMultibodyLink* obj)
{
	return obj->m_userPtr;
}

void btMultibodyLink_setAppliedConstraintForce(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedConstraintForce, value);
}

void btMultibodyLink_setAppliedConstraintTorque(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedConstraintTorque, value);
}

void btMultibodyLink_setAppliedForce(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedForce, value);
}

void btMultibodyLink_setAppliedTorque(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedTorque, value);
}

void btMultibodyLink_setAxisBottom(btMultibodyLink* obj, int dof, btScalar x, btScalar y,
	btScalar z)
{
	obj->setAxisBottom(dof, x, y, z);
}

void btMultibodyLink_setAxisBottom2(btMultibodyLink* obj, int dof, const btVector3* axis)
{
	BTVECTOR3_IN(axis);
	obj->setAxisBottom(dof, BTVECTOR3_USE(axis));
}

void btMultibodyLink_setAxisTop(btMultibodyLink* obj, int dof, btScalar x, btScalar y,
	btScalar z)
{
	obj->setAxisTop(dof, x, y, z);
}

void btMultibodyLink_setAxisTop2(btMultibodyLink* obj, int dof, const btVector3* axis)
{
	BTVECTOR3_IN(axis);
	obj->setAxisTop(dof, BTVECTOR3_USE(axis));
}

void btMultibodyLink_setCachedRotParentToThis(btMultibodyLink* obj, const btQuaternion* value)
{
	BTQUATERNION_COPY(&obj->m_cachedRotParentToThis, value);
}

void btMultibodyLink_setCachedRVector(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_cachedRVector, value);
}

void btMultibodyLink_setCachedWorldTransform(btMultibodyLink* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_cachedWorldTransform, value);
}

void btMultibodyLink_setCfgOffset(btMultibodyLink* obj, int value)
{
	obj->m_cfgOffset = value;
}

void btMultibodyLink_setCollider(btMultibodyLink* obj, btMultiBodyLinkCollider* value)
{
	obj->m_collider = value;
}

void btMultibodyLink_setDofCount(btMultibodyLink* obj, int value)
{
	obj->m_dofCount = value;
}

void btMultibodyLink_setDofOffset(btMultibodyLink* obj, int value)
{
	obj->m_dofOffset = value;
}

void btMultibodyLink_setDVector(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_dVector, value);
}

void btMultibodyLink_setEVector(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_eVector, value);
}

void btMultibodyLink_setFlags(btMultibodyLink* obj, int value)
{
	obj->m_flags = value;
}

void btMultibodyLink_setInertiaLocal(btMultibodyLink* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_inertiaLocal, value);
}

void btMultibodyLink_setJointDamping(btMultibodyLink* obj, btScalar value)
{
	obj->m_jointDamping = value;
}

void btMultibodyLink_setJointFeedback(btMultibodyLink* obj, btMultiBodyJointFeedback* value)
{
	obj->m_jointFeedback = value;
}

void btMultibodyLink_setJointFriction(btMultibodyLink* obj, btScalar value)
{
	obj->m_jointFriction = value;
}

void btMultibodyLink_setJointName(btMultibodyLink* obj, const char* value)
{
	obj->m_jointName = value;
}

void btMultibodyLink_setJointType(btMultibodyLink* obj, btMultibodyLink_eFeatherstoneJointType value)
{
	obj->m_jointType = value;
}

void btMultibodyLink_setLinkName(btMultibodyLink* obj, const char* value)
{
	obj->m_linkName = value;
}

void btMultibodyLink_setMass(btMultibodyLink* obj, btScalar value)
{
	obj->m_mass = value;
}

void btMultibodyLink_setParent(btMultibodyLink* obj, int value)
{
	obj->m_parent = value;
}

void btMultibodyLink_setPosVarCount(btMultibodyLink* obj, int value)
{
	obj->m_posVarCount = value;
}

void btMultibodyLink_setZeroRotParentToThis(btMultibodyLink* obj, const btQuaternion* value)
{
	BTQUATERNION_COPY(&obj->m_zeroRotParentToThis, value);
}

void btMultibodyLink_setUserPtr(btMultibodyLink* obj, const void* value)
{
	obj->m_userPtr = value;
}

void btMultibodyLink_updateCacheMultiDof(btMultibodyLink* obj, btScalar* pq)
{
	obj->updateCacheMultiDof(pq);
}
