#include <BulletDynamics/ConstraintSolver/btHingeConstraint.h>

#include "conversion.h"
#include "btHingeConstraint_wrap.h"

btHingeConstraint* btHingeConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* pivotInA,
	const btVector3* pivotInB, const btVector3* axisInA, const btVector3* axisInB,
	bool useReferenceFrameA)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(pivotInB);
	BTVECTOR3_IN(axisInA);
	BTVECTOR3_IN(axisInB);
	return new btHingeConstraint(*rbA, *rbB, BTVECTOR3_USE(pivotInA), BTVECTOR3_USE(pivotInB),
		BTVECTOR3_USE(axisInA), BTVECTOR3_USE(axisInB), useReferenceFrameA);
}

btHingeConstraint* btHingeConstraint_new2(btRigidBody* rbA, const btVector3* pivotInA,
	const btVector3* axisInA, bool useReferenceFrameA)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(axisInA);
	return new btHingeConstraint(*rbA, BTVECTOR3_USE(pivotInA), BTVECTOR3_USE(axisInA),
		useReferenceFrameA);
}

btHingeConstraint* btHingeConstraint_new3(btRigidBody* rbA, btRigidBody* rbB, const btTransform* rbAFrame,
	const btTransform* rbBFrame, bool useReferenceFrameA)
{
	BTTRANSFORM_IN(rbAFrame);
	BTTRANSFORM_IN(rbBFrame);
	return new btHingeConstraint(*rbA, *rbB, BTTRANSFORM_USE(rbAFrame), BTTRANSFORM_USE(rbBFrame),
		useReferenceFrameA);
}

btHingeConstraint* btHingeConstraint_new4(btRigidBody* rbA, const btTransform* rbAFrame,
	bool useReferenceFrameA)
{
	BTTRANSFORM_IN(rbAFrame);
	return new btHingeConstraint(*rbA, BTTRANSFORM_USE(rbAFrame), useReferenceFrameA);
}

void btHingeConstraint_enableAngularMotor(btHingeConstraint* obj, bool enableMotor,
	btScalar targetVelocity, btScalar maxMotorImpulse)
{
	obj->enableAngularMotor(enableMotor, targetVelocity, maxMotorImpulse);
}

void btHingeConstraint_enableMotor(btHingeConstraint* obj, bool enableMotor)
{
	obj->enableMotor(enableMotor);
}

void btHingeConstraint_getAFrame(btHingeConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getAFrame());
}

bool btHingeConstraint_getAngularOnly(btHingeConstraint* obj)
{
	return obj->getAngularOnly();
}

void btHingeConstraint_getBFrame(btHingeConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getBFrame());
}

bool btHingeConstraint_getEnableAngularMotor(btHingeConstraint* obj)
{
	return obj->getEnableAngularMotor();
}

int btHingeConstraint_getFlags(btHingeConstraint* obj)
{
	return obj->getFlags();
}

void btHingeConstraint_getFrameOffsetA(btHingeConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetA());
}

void btHingeConstraint_getFrameOffsetB(btHingeConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetB());
}

btScalar btHingeConstraint_getHingeAngle(btHingeConstraint* obj, const btTransform* transA,
	const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	return obj->getHingeAngle(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

btScalar btHingeConstraint_getHingeAngle2(btHingeConstraint* obj)
{
	return obj->getHingeAngle();
}

void btHingeConstraint_getInfo1NonVirtual(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo1* info)
{
	obj->getInfo1NonVirtual(info);
}

void btHingeConstraint_getInfo2Internal(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* transA, const btTransform* transB, const btVector3* angVelA,
	const btVector3* angVelB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(angVelA);
	BTVECTOR3_IN(angVelB);
	obj->getInfo2Internal(info, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(angVelA), BTVECTOR3_USE(angVelB));
}

void btHingeConstraint_getInfo2InternalUsingFrameOffset(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* transA, const btTransform* transB, const btVector3* angVelA,
	const btVector3* angVelB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(angVelA);
	BTVECTOR3_IN(angVelB);
	obj->getInfo2InternalUsingFrameOffset(info, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(angVelA), BTVECTOR3_USE(angVelB));
}

void btHingeConstraint_getInfo2NonVirtual(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* transA, const btTransform* transB, const btVector3* angVelA,
	const btVector3* angVelB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(angVelA);
	BTVECTOR3_IN(angVelB);
	obj->getInfo2NonVirtual(info, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(angVelA), BTVECTOR3_USE(angVelB));
}

btScalar btHingeConstraint_getLimitBiasFactor(btHingeConstraint* obj)
{
	return obj->getLimitBiasFactor();
}

btScalar btHingeConstraint_getLimitRelaxationFactor(btHingeConstraint* obj)
{
	return obj->getLimitRelaxationFactor();
}

btScalar btHingeConstraint_getLimitSign(btHingeConstraint* obj)
{
	return obj->getLimitSign();
}

btScalar btHingeConstraint_getLimitSoftness(btHingeConstraint* obj)
{
	return obj->getLimitSoftness();
}

btScalar btHingeConstraint_getLowerLimit(btHingeConstraint* obj)
{
	return obj->getLowerLimit();
}

btScalar btHingeConstraint_getMaxMotorImpulse(btHingeConstraint* obj)
{
	return obj->getMaxMotorImpulse();
}

btScalar btHingeConstraint_getMotorTargetVelocity(btHingeConstraint* obj)
{
	return obj->getMotorTargetVelocity();
}

int btHingeConstraint_getSolveLimit(btHingeConstraint* obj)
{
	return obj->getSolveLimit();
}

btScalar btHingeConstraint_getUpperLimit(btHingeConstraint* obj)
{
	return obj->getUpperLimit();
}

bool btHingeConstraint_getUseFrameOffset(btHingeConstraint* obj)
{
	return obj->getUseFrameOffset();
}

bool btHingeConstraint_getUseReferenceFrameA(btHingeConstraint* obj)
{
	return obj->getUseReferenceFrameA();
}

bool btHingeConstraint_hasLimit(btHingeConstraint* obj)
{
	return obj->hasLimit();
}

void btHingeConstraint_setAngularOnly(btHingeConstraint* obj, bool angularOnly)
{
	obj->setAngularOnly(angularOnly);
}

void btHingeConstraint_setAxis(btHingeConstraint* obj, btVector3* axisInA)
{
	BTVECTOR3_IN(axisInA);
	obj->setAxis(BTVECTOR3_USE(axisInA));
}

void btHingeConstraint_setFrames(btHingeConstraint* obj, const btTransform* frameA,
	const btTransform* frameB)
{
	BTTRANSFORM_IN(frameA);
	BTTRANSFORM_IN(frameB);
	obj->setFrames(BTTRANSFORM_USE(frameA), BTTRANSFORM_USE(frameB));
}

void btHingeConstraint_setLimit(btHingeConstraint* obj, btScalar low, btScalar high)
{
	obj->setLimit(low, high);
}

void btHingeConstraint_setLimit2(btHingeConstraint* obj, btScalar low, btScalar high,
	btScalar _softness)
{
	obj->setLimit(low, high, _softness);
}

void btHingeConstraint_setLimit3(btHingeConstraint* obj, btScalar low, btScalar high,
	btScalar _softness, btScalar _biasFactor)
{
	obj->setLimit(low, high, _softness, _biasFactor);
}

void btHingeConstraint_setLimit4(btHingeConstraint* obj, btScalar low, btScalar high,
	btScalar _softness, btScalar _biasFactor, btScalar _relaxationFactor)
{
	obj->setLimit(low, high, _softness, _biasFactor, _relaxationFactor);
}

void btHingeConstraint_setMaxMotorImpulse(btHingeConstraint* obj, btScalar maxMotorImpulse)
{
	obj->setMaxMotorImpulse(maxMotorImpulse);
}

void btHingeConstraint_setMotorTarget(btHingeConstraint* obj, btScalar targetAngle,
	btScalar dt)
{
	obj->setMotorTarget(targetAngle, dt);
}

void btHingeConstraint_setMotorTarget2(btHingeConstraint* obj, const btQuaternion* qAinB,
	btScalar dt)
{
	BTQUATERNION_IN(qAinB);
	obj->setMotorTarget(BTQUATERNION_USE(qAinB), dt);
}

void btHingeConstraint_setMotorTargetVelocity(btHingeConstraint* obj, btScalar motorTargetVelocity)
{
	obj->setMotorTargetVelocity(motorTargetVelocity);
}

void btHingeConstraint_setUseFrameOffset(btHingeConstraint* obj, bool frameOffsetOnOff)
{
	obj->setUseFrameOffset(frameOffsetOnOff);
}

void btHingeConstraint_setUseReferenceFrameA(btHingeConstraint* obj, bool useReferenceFrameA)
{
	obj->setUseReferenceFrameA(useReferenceFrameA);
}

void btHingeConstraint_testLimit(btHingeConstraint* obj, const btTransform* transA,
	const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->testLimit(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btHingeConstraint_updateRHS(btHingeConstraint* obj, btScalar timeStep)
{
	obj->updateRHS(timeStep);
}


btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new(btRigidBody* rbA,
	btRigidBody* rbB, const btVector3* pivotInA, const btVector3* pivotInB, const btVector3* axisInA,
	const btVector3* axisInB, bool useReferenceFrameA)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(pivotInB);
	BTVECTOR3_IN(axisInA);
	BTVECTOR3_IN(axisInB);
	return new btHingeAccumulatedAngleConstraint(*rbA, *rbB, BTVECTOR3_USE(pivotInA),
		BTVECTOR3_USE(pivotInB), BTVECTOR3_USE(axisInA), BTVECTOR3_USE(axisInB), useReferenceFrameA);
}

btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new2(btRigidBody* rbA,
	const btVector3* pivotInA, const btVector3* axisInA, bool useReferenceFrameA)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(axisInA);
	return new btHingeAccumulatedAngleConstraint(*rbA, BTVECTOR3_USE(pivotInA), BTVECTOR3_USE(axisInA),
		useReferenceFrameA);
}

btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new3(btRigidBody* rbA,
	btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame, bool useReferenceFrameA)
{
	BTTRANSFORM_IN(rbAFrame);
	BTTRANSFORM_IN(rbBFrame);
	return new btHingeAccumulatedAngleConstraint(*rbA, *rbB, BTTRANSFORM_USE(rbAFrame),
		BTTRANSFORM_USE(rbBFrame), useReferenceFrameA);
}

btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new4(btRigidBody* rbA,
	const btTransform* rbAFrame, bool useReferenceFrameA)
{
	BTTRANSFORM_IN(rbAFrame);
	return new btHingeAccumulatedAngleConstraint(*rbA, BTTRANSFORM_USE(rbAFrame),
		useReferenceFrameA);
}

btScalar btHingeAccumulatedAngleConstraint_getAccumulatedHingeAngle(btHingeAccumulatedAngleConstraint* obj)
{
	return obj->getAccumulatedHingeAngle();
}

void btHingeAccumulatedAngleConstraint_setAccumulatedHingeAngle(btHingeAccumulatedAngleConstraint* obj,
	btScalar accAngle)
{
	obj->setAccumulatedHingeAngle(accAngle);
}
