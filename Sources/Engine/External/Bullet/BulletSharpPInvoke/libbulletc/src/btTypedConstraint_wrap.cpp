#include <BulletDynamics/ConstraintSolver/btTypedConstraint.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btTypedConstraint_wrap.h"

btJointFeedback* btJointFeedback_new()
{
	return new btJointFeedback();
}

void btJointFeedback_getAppliedForceBodyA(btJointFeedback* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedForceBodyA);
}

void btJointFeedback_getAppliedForceBodyB(btJointFeedback* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedForceBodyB);
}

void btJointFeedback_getAppliedTorqueBodyA(btJointFeedback* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedTorqueBodyA);
}

void btJointFeedback_getAppliedTorqueBodyB(btJointFeedback* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_appliedTorqueBodyB);
}

void btJointFeedback_setAppliedForceBodyA(btJointFeedback* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedForceBodyA, value);
}

void btJointFeedback_setAppliedForceBodyB(btJointFeedback* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedForceBodyB, value);
}

void btJointFeedback_setAppliedTorqueBodyA(btJointFeedback* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedTorqueBodyA, value);
}

void btJointFeedback_setAppliedTorqueBodyB(btJointFeedback* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_appliedTorqueBodyB, value);
}

void btJointFeedback_delete(btJointFeedback* obj)
{
	delete obj;
}


btTypedConstraint_btConstraintInfo1* btTypedConstraint_btConstraintInfo1_new()
{
	return new btTypedConstraint::btConstraintInfo1();
}

int btTypedConstraint_btConstraintInfo1_getNub(btTypedConstraint_btConstraintInfo1* obj)
{
	return obj->nub;
}

int btTypedConstraint_btConstraintInfo1_getNumConstraintRows(btTypedConstraint_btConstraintInfo1* obj)
{
	return obj->m_numConstraintRows;
}

void btTypedConstraint_btConstraintInfo1_setNub(btTypedConstraint_btConstraintInfo1* obj,
	int value)
{
	obj->nub = value;
}

void btTypedConstraint_btConstraintInfo1_setNumConstraintRows(btTypedConstraint_btConstraintInfo1* obj,
	int value)
{
	obj->m_numConstraintRows = value;
}

void btTypedConstraint_btConstraintInfo1_delete(btTypedConstraint_btConstraintInfo1* obj)
{
	delete obj;
}


btTypedConstraint_btConstraintInfo2* btTypedConstraint_btConstraintInfo2_new()
{
	return new btTypedConstraint::btConstraintInfo2();
}

btScalar* btTypedConstraint_btConstraintInfo2_getCfm(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->cfm;
}

btScalar* btTypedConstraint_btConstraintInfo2_getConstraintError(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_constraintError;
}

btScalar btTypedConstraint_btConstraintInfo2_getDamping(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_damping;
}

btScalar btTypedConstraint_btConstraintInfo2_getErp(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->erp;
}

btScalar btTypedConstraint_btConstraintInfo2_getFps(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->fps;
}

btScalar* btTypedConstraint_btConstraintInfo2_getJ1angularAxis(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_J1angularAxis;
}

btScalar* btTypedConstraint_btConstraintInfo2_getJ1linearAxis(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_J1linearAxis;
}

btScalar* btTypedConstraint_btConstraintInfo2_getJ2angularAxis(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_J2angularAxis;
}

btScalar* btTypedConstraint_btConstraintInfo2_getJ2linearAxis(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_J2linearAxis;
}

btScalar* btTypedConstraint_btConstraintInfo2_getLowerLimit(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_lowerLimit;
}

int btTypedConstraint_btConstraintInfo2_getNumIterations(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_numIterations;
}

int btTypedConstraint_btConstraintInfo2_getRowskip(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->rowskip;
}

btScalar* btTypedConstraint_btConstraintInfo2_getUpperLimit(btTypedConstraint_btConstraintInfo2* obj)
{
	return obj->m_upperLimit;
}

void btTypedConstraint_btConstraintInfo2_setCfm(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->cfm = value;
}

void btTypedConstraint_btConstraintInfo2_setConstraintError(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_constraintError = value;
}

void btTypedConstraint_btConstraintInfo2_setDamping(btTypedConstraint_btConstraintInfo2* obj,
	btScalar value)
{
	obj->m_damping = value;
}

void btTypedConstraint_btConstraintInfo2_setErp(btTypedConstraint_btConstraintInfo2* obj,
	btScalar value)
{
	obj->erp = value;
}

void btTypedConstraint_btConstraintInfo2_setFps(btTypedConstraint_btConstraintInfo2* obj,
	btScalar value)
{
	obj->fps = value;
}

void btTypedConstraint_btConstraintInfo2_setJ1angularAxis(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_J1angularAxis = value;
}

void btTypedConstraint_btConstraintInfo2_setJ1linearAxis(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_J1linearAxis = value;
}

void btTypedConstraint_btConstraintInfo2_setJ2angularAxis(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_J2angularAxis = value;
}

void btTypedConstraint_btConstraintInfo2_setJ2linearAxis(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_J2linearAxis = value;
}

void btTypedConstraint_btConstraintInfo2_setLowerLimit(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_lowerLimit = value;
}

void btTypedConstraint_btConstraintInfo2_setNumIterations(btTypedConstraint_btConstraintInfo2* obj,
	int value)
{
	obj->m_numIterations = value;
}

void btTypedConstraint_btConstraintInfo2_setRowskip(btTypedConstraint_btConstraintInfo2* obj,
	int value)
{
	obj->rowskip = value;
}

void btTypedConstraint_btConstraintInfo2_setUpperLimit(btTypedConstraint_btConstraintInfo2* obj,
	btScalar* value)
{
	obj->m_upperLimit = value;
}

void btTypedConstraint_btConstraintInfo2_delete(btTypedConstraint_btConstraintInfo2* obj)
{
	delete obj;
}


void btTypedConstraint_buildJacobian(btTypedConstraint* obj)
{
	obj->buildJacobian();
}

int btTypedConstraint_calculateSerializeBufferSize(btTypedConstraint* obj)
{
	return obj->calculateSerializeBufferSize();
}

void btTypedConstraint_enableFeedback(btTypedConstraint* obj, bool needsFeedback)
{
	obj->enableFeedback(needsFeedback);
}

btScalar btTypedConstraint_getAppliedImpulse(btTypedConstraint* obj)
{
	return obj->getAppliedImpulse();
}

btScalar btTypedConstraint_getBreakingImpulseThreshold(btTypedConstraint* obj)
{
	return obj->getBreakingImpulseThreshold();
}

btTypedConstraintType btTypedConstraint_getConstraintType(btTypedConstraint* obj)
{
	return obj->getConstraintType();
}

btScalar btTypedConstraint_getDbgDrawSize(btTypedConstraint* obj)
{
	return obj->getDbgDrawSize();
}

btRigidBody* btTypedConstraint_getFixedBody()
{
	return &btTypedConstraint::getFixedBody();
}

void btTypedConstraint_getInfo1(btTypedConstraint* obj, btTypedConstraint_btConstraintInfo1* info)
{
	obj->getInfo1(info);
}

void btTypedConstraint_getInfo2(btTypedConstraint* obj, btTypedConstraint_btConstraintInfo2* info)
{
	obj->getInfo2(info);
}

btJointFeedback* btTypedConstraint_getJointFeedback(btTypedConstraint* obj)
{
	return obj->getJointFeedback();
}

int btTypedConstraint_getOverrideNumSolverIterations(btTypedConstraint* obj)
{
	return obj->getOverrideNumSolverIterations();
}

btScalar btTypedConstraint_getParam(btTypedConstraint* obj, int num)
{
	return obj->getParam(num);
}

btScalar btTypedConstraint_getParam2(btTypedConstraint* obj, int num, int axis)
{
	return obj->getParam(num, axis);
}

btRigidBody* btTypedConstraint_getRigidBodyA(btTypedConstraint* obj)
{
	return &obj->getRigidBodyA();
}

btRigidBody* btTypedConstraint_getRigidBodyB(btTypedConstraint* obj)
{
	return &obj->getRigidBodyB();
}

int btTypedConstraint_getUid(btTypedConstraint* obj)
{
	return obj->getUid();
}

int btTypedConstraint_getUserConstraintId(btTypedConstraint* obj)
{
	return obj->getUserConstraintId();
}

void* btTypedConstraint_getUserConstraintPtr(btTypedConstraint* obj)
{
	return obj->getUserConstraintPtr();
}

int btTypedConstraint_getUserConstraintType(btTypedConstraint* obj)
{
	return obj->getUserConstraintType();
}

btScalar btTypedConstraint_internalGetAppliedImpulse(btTypedConstraint* obj)
{
	return obj->internalGetAppliedImpulse();
}

void btTypedConstraint_internalSetAppliedImpulse(btTypedConstraint* obj, btScalar appliedImpulse)
{
	obj->internalSetAppliedImpulse(appliedImpulse);
}

bool btTypedConstraint_isEnabled(btTypedConstraint* obj)
{
	return obj->isEnabled();
}

bool btTypedConstraint_needsFeedback(btTypedConstraint* obj)
{
	return obj->needsFeedback();
}

const char* btTypedConstraint_serialize(btTypedConstraint* obj, void* dataBuffer,
	btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btTypedConstraint_setBreakingImpulseThreshold(btTypedConstraint* obj, btScalar threshold)
{
	obj->setBreakingImpulseThreshold(threshold);
}

void btTypedConstraint_setDbgDrawSize(btTypedConstraint* obj, btScalar dbgDrawSize)
{
	obj->setDbgDrawSize(dbgDrawSize);
}

void btTypedConstraint_setEnabled(btTypedConstraint* obj, bool enabled)
{
	obj->setEnabled(enabled);
}

void btTypedConstraint_setJointFeedback(btTypedConstraint* obj, btJointFeedback* jointFeedback)
{
	obj->setJointFeedback(jointFeedback);
}

void btTypedConstraint_setOverrideNumSolverIterations(btTypedConstraint* obj, int overideNumIterations)
{
	obj->setOverrideNumSolverIterations(overideNumIterations);
}

void btTypedConstraint_setParam(btTypedConstraint* obj, int num, btScalar value)
{
	obj->setParam(num, value);
}

void btTypedConstraint_setParam2(btTypedConstraint* obj, int num, btScalar value,
	int axis)
{
	obj->setParam(num, value, axis);
}

void btTypedConstraint_setupSolverConstraint(btTypedConstraint* obj, btAlignedObjectArray_btSolverConstraint* ca,
	int solverBodyA, int solverBodyB, btScalar timeStep)
{
	obj->setupSolverConstraint(*ca, solverBodyA, solverBodyB, timeStep);
}

void btTypedConstraint_setUserConstraintId(btTypedConstraint* obj, int uid)
{
	obj->setUserConstraintId(uid);
}

void btTypedConstraint_setUserConstraintPtr(btTypedConstraint* obj, void* ptr)
{
	obj->setUserConstraintPtr(ptr);
}

void btTypedConstraint_setUserConstraintType(btTypedConstraint* obj, int userConstraintType)
{
	obj->setUserConstraintType(userConstraintType);
}

void btTypedConstraint_solveConstraintObsolete(btTypedConstraint* obj, btSolverBody* __unnamed0,
	btSolverBody* __unnamed1, btScalar __unnamed2)
{
	obj->solveConstraintObsolete(*__unnamed0, *__unnamed1, __unnamed2);
}

void btTypedConstraint_delete(btTypedConstraint* obj)
{
	delete obj;
}


btAngularLimit* btAngularLimit_new()
{
	return new btAngularLimit();
}

void btAngularLimit_fit(btAngularLimit* obj, btScalar* angle)
{
	obj->fit(*angle);
}

btScalar btAngularLimit_getBiasFactor(btAngularLimit* obj)
{
	return obj->getBiasFactor();
}

btScalar btAngularLimit_getCorrection(btAngularLimit* obj)
{
	return obj->getCorrection();
}

btScalar btAngularLimit_getError(btAngularLimit* obj)
{
	return obj->getError();
}

btScalar btAngularLimit_getHalfRange(btAngularLimit* obj)
{
	return obj->getHalfRange();
}

btScalar btAngularLimit_getHigh(btAngularLimit* obj)
{
	return obj->getHigh();
}

btScalar btAngularLimit_getLow(btAngularLimit* obj)
{
	return obj->getLow();
}

btScalar btAngularLimit_getRelaxationFactor(btAngularLimit* obj)
{
	return obj->getRelaxationFactor();
}

btScalar btAngularLimit_getSign(btAngularLimit* obj)
{
	return obj->getSign();
}

btScalar btAngularLimit_getSoftness(btAngularLimit* obj)
{
	return obj->getSoftness();
}

bool btAngularLimit_isLimit(btAngularLimit* obj)
{
	return obj->isLimit();
}

void btAngularLimit_set(btAngularLimit* obj, btScalar low, btScalar high, btScalar _softness,
	btScalar _biasFactor, btScalar _relaxationFactor)
{
	obj->set(low, high, _softness, _biasFactor, _relaxationFactor);
}

void btAngularLimit_test(btAngularLimit* obj, btScalar angle)
{
	obj->test(angle);
}

void btAngularLimit_delete(btAngularLimit* obj)
{
	delete obj;
}
