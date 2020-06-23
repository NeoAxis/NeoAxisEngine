#include <BulletDynamics/Featherstone/btMultiBody.h>
#include <BulletDynamics/Featherstone/btMultiBodyConstraint.h>
#include <BulletDynamics/Featherstone/btMultiBodySolverConstraint.h>

#include "conversion.h"
#include "btMultiBodySolverConstraint_wrap.h"

btMultiBodySolverConstraint* btMultiBodySolverConstraint_new()
{
	return new btMultiBodySolverConstraint();
}

void btMultiBodySolverConstraint_getAngularComponentA(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_angularComponentA);
}

void btMultiBodySolverConstraint_getAngularComponentB(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_angularComponentB);
}

btScalar btMultiBodySolverConstraint_getAppliedImpulse(btMultiBodySolverConstraint* obj)
{
	return obj->m_appliedImpulse;
}

btScalar btMultiBodySolverConstraint_getAppliedPushImpulse(btMultiBodySolverConstraint* obj)
{
	return obj->m_appliedPushImpulse;
}

btScalar btMultiBodySolverConstraint_getCfm(btMultiBodySolverConstraint* obj)
{
	return obj->m_cfm;
}

void btMultiBodySolverConstraint_getContactNormal1(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_contactNormal1);
}

void btMultiBodySolverConstraint_getContactNormal2(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_contactNormal2);
}

int btMultiBodySolverConstraint_getDeltaVelAindex(btMultiBodySolverConstraint* obj)
{
	return obj->m_deltaVelAindex;
}

int btMultiBodySolverConstraint_getDeltaVelBindex(btMultiBodySolverConstraint* obj)
{
	return obj->m_deltaVelBindex;
}

btScalar btMultiBodySolverConstraint_getFriction(btMultiBodySolverConstraint* obj)
{
	return obj->m_friction;
}

int btMultiBodySolverConstraint_getFrictionIndex(btMultiBodySolverConstraint* obj)
{
	return obj->m_frictionIndex;
}

int btMultiBodySolverConstraint_getJacAindex(btMultiBodySolverConstraint* obj)
{
	return obj->m_jacAindex;
}

int btMultiBodySolverConstraint_getJacBindex(btMultiBodySolverConstraint* obj)
{
	return obj->m_jacBindex;
}

btScalar btMultiBodySolverConstraint_getJacDiagABInv(btMultiBodySolverConstraint* obj)
{
	return obj->m_jacDiagABInv;
}

int btMultiBodySolverConstraint_getLinkA(btMultiBodySolverConstraint* obj)
{
	return obj->m_linkA;
}

int btMultiBodySolverConstraint_getLinkB(btMultiBodySolverConstraint* obj)
{
	return obj->m_linkB;
}

btScalar btMultiBodySolverConstraint_getLowerLimit(btMultiBodySolverConstraint* obj)
{
	return obj->m_lowerLimit;
}

btMultiBody* btMultiBodySolverConstraint_getMultiBodyA(btMultiBodySolverConstraint* obj)
{
	return obj->m_multiBodyA;
}

btMultiBody* btMultiBodySolverConstraint_getMultiBodyB(btMultiBodySolverConstraint* obj)
{
	return obj->m_multiBodyB;
}

btMultiBodyConstraint* btMultiBodySolverConstraint_getOrgConstraint(btMultiBodySolverConstraint* obj)
{
	return obj->m_orgConstraint;
}

int btMultiBodySolverConstraint_getOrgDofIndex(btMultiBodySolverConstraint* obj)
{
	return obj->m_orgDofIndex;
}

void* btMultiBodySolverConstraint_getOriginalContactPoint(btMultiBodySolverConstraint* obj)
{
	return obj->m_originalContactPoint;
}

int btMultiBodySolverConstraint_getOverrideNumSolverIterations(btMultiBodySolverConstraint* obj)
{
	return obj->m_overrideNumSolverIterations;
}

void btMultiBodySolverConstraint_getRelpos1CrossNormal(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_relpos1CrossNormal);
}

void btMultiBodySolverConstraint_getRelpos2CrossNormal(btMultiBodySolverConstraint* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_relpos2CrossNormal);
}

btScalar btMultiBodySolverConstraint_getRhs(btMultiBodySolverConstraint* obj)
{
	return obj->m_rhs;
}

btScalar btMultiBodySolverConstraint_getRhsPenetration(btMultiBodySolverConstraint* obj)
{
	return obj->m_rhsPenetration;
}

int btMultiBodySolverConstraint_getSolverBodyIdA(btMultiBodySolverConstraint* obj)
{
	return obj->m_solverBodyIdA;
}

int btMultiBodySolverConstraint_getSolverBodyIdB(btMultiBodySolverConstraint* obj)
{
	return obj->m_solverBodyIdB;
}

btScalar btMultiBodySolverConstraint_getUnusedPadding4(btMultiBodySolverConstraint* obj)
{
	return obj->m_unusedPadding4;
}

btScalar btMultiBodySolverConstraint_getUpperLimit(btMultiBodySolverConstraint* obj)
{
	return obj->m_upperLimit;
}

void btMultiBodySolverConstraint_setAngularComponentA(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_angularComponentA, value);
}

void btMultiBodySolverConstraint_setAngularComponentB(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_angularComponentB, value);
}

void btMultiBodySolverConstraint_setAppliedImpulse(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_appliedImpulse = value;
}

void btMultiBodySolverConstraint_setAppliedPushImpulse(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_appliedPushImpulse = value;
}

void btMultiBodySolverConstraint_setCfm(btMultiBodySolverConstraint* obj, btScalar value)
{
	obj->m_cfm = value;
}

void btMultiBodySolverConstraint_setContactNormal1(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_contactNormal1, value);
}

void btMultiBodySolverConstraint_setContactNormal2(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_contactNormal2, value);
}

void btMultiBodySolverConstraint_setDeltaVelAindex(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_deltaVelAindex = value;
}

void btMultiBodySolverConstraint_setDeltaVelBindex(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_deltaVelBindex = value;
}

void btMultiBodySolverConstraint_setFriction(btMultiBodySolverConstraint* obj, btScalar value)
{
	obj->m_friction = value;
}

void btMultiBodySolverConstraint_setFrictionIndex(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_frictionIndex = value;
}

void btMultiBodySolverConstraint_setJacAindex(btMultiBodySolverConstraint* obj, int value)
{
	obj->m_jacAindex = value;
}

void btMultiBodySolverConstraint_setJacBindex(btMultiBodySolverConstraint* obj, int value)
{
	obj->m_jacBindex = value;
}

void btMultiBodySolverConstraint_setJacDiagABInv(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_jacDiagABInv = value;
}

void btMultiBodySolverConstraint_setLinkA(btMultiBodySolverConstraint* obj, int value)
{
	obj->m_linkA = value;
}

void btMultiBodySolverConstraint_setLinkB(btMultiBodySolverConstraint* obj, int value)
{
	obj->m_linkB = value;
}

void btMultiBodySolverConstraint_setLowerLimit(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_lowerLimit = value;
}

void btMultiBodySolverConstraint_setMultiBodyA(btMultiBodySolverConstraint* obj,
	btMultiBody* value)
{
	obj->m_multiBodyA = value;
}

void btMultiBodySolverConstraint_setMultiBodyB(btMultiBodySolverConstraint* obj,
	btMultiBody* value)
{
	obj->m_multiBodyB = value;
}

void btMultiBodySolverConstraint_setOrgConstraint(btMultiBodySolverConstraint* obj,
	btMultiBodyConstraint* value)
{
	obj->m_orgConstraint = value;
}

void btMultiBodySolverConstraint_setOrgDofIndex(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_orgDofIndex = value;
}

void btMultiBodySolverConstraint_setOriginalContactPoint(btMultiBodySolverConstraint* obj,
	void* value)
{
	obj->m_originalContactPoint = value;
}

void btMultiBodySolverConstraint_setOverrideNumSolverIterations(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_overrideNumSolverIterations = value;
}

void btMultiBodySolverConstraint_setRelpos1CrossNormal(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_relpos1CrossNormal, value);
}

void btMultiBodySolverConstraint_setRelpos2CrossNormal(btMultiBodySolverConstraint* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_relpos2CrossNormal, value);
}

void btMultiBodySolverConstraint_setRhs(btMultiBodySolverConstraint* obj, btScalar value)
{
	obj->m_rhs = value;
}

void btMultiBodySolverConstraint_setRhsPenetration(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_rhsPenetration = value;
}

void btMultiBodySolverConstraint_setSolverBodyIdA(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_solverBodyIdA = value;
}

void btMultiBodySolverConstraint_setSolverBodyIdB(btMultiBodySolverConstraint* obj,
	int value)
{
	obj->m_solverBodyIdB = value;
}

void btMultiBodySolverConstraint_setUnusedPadding4(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_unusedPadding4 = value;
}

void btMultiBodySolverConstraint_setUpperLimit(btMultiBodySolverConstraint* obj,
	btScalar value)
{
	obj->m_upperLimit = value;
}

void btMultiBodySolverConstraint_delete(btMultiBodySolverConstraint* obj)
{
	delete obj;
}
