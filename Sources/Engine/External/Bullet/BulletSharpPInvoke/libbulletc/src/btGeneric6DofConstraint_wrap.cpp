#include <BulletDynamics/ConstraintSolver/btGeneric6DofConstraint.h>

#include "conversion.h"
#include "btGeneric6DofConstraint_wrap.h"

btRotationalLimitMotor* btRotationalLimitMotor_new()
{
	return new btRotationalLimitMotor();
}

btRotationalLimitMotor* btRotationalLimitMotor_new2(const btRotationalLimitMotor* limot)
{
	return new btRotationalLimitMotor(*limot);
}

btScalar btRotationalLimitMotor_getAccumulatedImpulse(btRotationalLimitMotor* obj)
{
	return obj->m_accumulatedImpulse;
}

btScalar btRotationalLimitMotor_getBounce(btRotationalLimitMotor* obj)
{
	return obj->m_bounce;
}

int btRotationalLimitMotor_getCurrentLimit(btRotationalLimitMotor* obj)
{
	return obj->m_currentLimit;
}

btScalar btRotationalLimitMotor_getCurrentLimitError(btRotationalLimitMotor* obj)
{
	return obj->m_currentLimitError;
}

btScalar btRotationalLimitMotor_getCurrentPosition(btRotationalLimitMotor* obj)
{
	return obj->m_currentPosition;
}

btScalar btRotationalLimitMotor_getDamping(btRotationalLimitMotor* obj)
{
	return obj->m_damping;
}

bool btRotationalLimitMotor_getEnableMotor(btRotationalLimitMotor* obj)
{
	return obj->m_enableMotor;
}

btScalar btRotationalLimitMotor_getHiLimit(btRotationalLimitMotor* obj)
{
	return obj->m_hiLimit;
}

btScalar btRotationalLimitMotor_getLimitSoftness(btRotationalLimitMotor* obj)
{
	return obj->m_limitSoftness;
}

btScalar btRotationalLimitMotor_getLoLimit(btRotationalLimitMotor* obj)
{
	return obj->m_loLimit;
}

btScalar btRotationalLimitMotor_getMaxLimitForce(btRotationalLimitMotor* obj)
{
	return obj->m_maxLimitForce;
}

btScalar btRotationalLimitMotor_getMaxMotorForce(btRotationalLimitMotor* obj)
{
	return obj->m_maxMotorForce;
}

btScalar btRotationalLimitMotor_getNormalCFM(btRotationalLimitMotor* obj)
{
	return obj->m_normalCFM;
}

btScalar btRotationalLimitMotor_getStopCFM(btRotationalLimitMotor* obj)
{
	return obj->m_stopCFM;
}

btScalar btRotationalLimitMotor_getStopERP(btRotationalLimitMotor* obj)
{
	return obj->m_stopERP;
}

btScalar btRotationalLimitMotor_getTargetVelocity(btRotationalLimitMotor* obj)
{
	return obj->m_targetVelocity;
}

bool btRotationalLimitMotor_isLimited(btRotationalLimitMotor* obj)
{
	return obj->isLimited();
}

bool btRotationalLimitMotor_needApplyTorques(btRotationalLimitMotor* obj)
{
	return obj->needApplyTorques();
}

void btRotationalLimitMotor_setAccumulatedImpulse(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_accumulatedImpulse = value;
}

void btRotationalLimitMotor_setBounce(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_bounce = value;
}

void btRotationalLimitMotor_setCurrentLimit(btRotationalLimitMotor* obj, int value)
{
	obj->m_currentLimit = value;
}

void btRotationalLimitMotor_setCurrentLimitError(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_currentLimitError = value;
}

void btRotationalLimitMotor_setCurrentPosition(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_currentPosition = value;
}

void btRotationalLimitMotor_setDamping(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_damping = value;
}

void btRotationalLimitMotor_setEnableMotor(btRotationalLimitMotor* obj, bool value)
{
	obj->m_enableMotor = value;
}

void btRotationalLimitMotor_setHiLimit(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_hiLimit = value;
}

void btRotationalLimitMotor_setLimitSoftness(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_limitSoftness = value;
}

void btRotationalLimitMotor_setLoLimit(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_loLimit = value;
}

void btRotationalLimitMotor_setMaxLimitForce(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_maxLimitForce = value;
}

void btRotationalLimitMotor_setMaxMotorForce(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_maxMotorForce = value;
}

void btRotationalLimitMotor_setNormalCFM(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_normalCFM = value;
}

void btRotationalLimitMotor_setStopCFM(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_stopCFM = value;
}

void btRotationalLimitMotor_setStopERP(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_stopERP = value;
}

void btRotationalLimitMotor_setTargetVelocity(btRotationalLimitMotor* obj, btScalar value)
{
	obj->m_targetVelocity = value;
}

btScalar btRotationalLimitMotor_solveAngularLimits(btRotationalLimitMotor* obj, btScalar timeStep,
	btVector3* axis, btScalar jacDiagABInv, btRigidBody* body0, btRigidBody* body1)
{
	BTVECTOR3_IN(axis);
	return obj->solveAngularLimits(timeStep, BTVECTOR3_USE(axis), jacDiagABInv, body0,
		body1);
}

int btRotationalLimitMotor_testLimitValue(btRotationalLimitMotor* obj, btScalar test_value)
{
	return obj->testLimitValue(test_value);
}

void btRotationalLimitMotor_delete(btRotationalLimitMotor* obj)
{
	delete obj;
}


btTranslationalLimitMotor* btTranslationalLimitMotor_new()
{
	return new btTranslationalLimitMotor();
}

btTranslationalLimitMotor* btTranslationalLimitMotor_new2(const btTranslationalLimitMotor* other)
{
	return new btTranslationalLimitMotor(*other);
}

void btTranslationalLimitMotor_getAccumulatedImpulse(btTranslationalLimitMotor* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_accumulatedImpulse);
}

int* btTranslationalLimitMotor_getCurrentLimit(btTranslationalLimitMotor* obj)
{
	return obj->m_currentLimit;
}

void btTranslationalLimitMotor_getCurrentLimitError(btTranslationalLimitMotor* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_currentLimitError);
}

void btTranslationalLimitMotor_getCurrentLinearDiff(btTranslationalLimitMotor* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_currentLinearDiff);
}

btScalar btTranslationalLimitMotor_getDamping(btTranslationalLimitMotor* obj)
{
	return obj->m_damping;
}

bool* btTranslationalLimitMotor_getEnableMotor(btTranslationalLimitMotor* obj)
{
	return obj->m_enableMotor;
}

btScalar btTranslationalLimitMotor_getLimitSoftness(btTranslationalLimitMotor* obj)
{
	return obj->m_limitSoftness;
}

void btTranslationalLimitMotor_getLowerLimit(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_lowerLimit);
}

void btTranslationalLimitMotor_getMaxMotorForce(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_maxMotorForce);
}

void btTranslationalLimitMotor_getNormalCFM(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normalCFM);
}

btScalar btTranslationalLimitMotor_getRestitution(btTranslationalLimitMotor* obj)
{
	return obj->m_restitution;
}

void btTranslationalLimitMotor_getStopCFM(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_stopCFM);
}

void btTranslationalLimitMotor_getStopERP(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_stopERP);
}

void btTranslationalLimitMotor_getTargetVelocity(btTranslationalLimitMotor* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_targetVelocity);
}

void btTranslationalLimitMotor_getUpperLimit(btTranslationalLimitMotor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_upperLimit);
}

bool btTranslationalLimitMotor_isLimited(btTranslationalLimitMotor* obj, int limitIndex)
{
	return obj->isLimited(limitIndex);
}

bool btTranslationalLimitMotor_needApplyForce(btTranslationalLimitMotor* obj, int limitIndex)
{
	return obj->needApplyForce(limitIndex);
}

void btTranslationalLimitMotor_setAccumulatedImpulse(btTranslationalLimitMotor* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_accumulatedImpulse, value);
}

void btTranslationalLimitMotor_setCurrentLimitError(btTranslationalLimitMotor* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_currentLimitError, value);
}

void btTranslationalLimitMotor_setCurrentLinearDiff(btTranslationalLimitMotor* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_currentLinearDiff, value);
}

void btTranslationalLimitMotor_setDamping(btTranslationalLimitMotor* obj, btScalar value)
{
	obj->m_damping = value;
}

void btTranslationalLimitMotor_setLimitSoftness(btTranslationalLimitMotor* obj, btScalar value)
{
	obj->m_limitSoftness = value;
}

void btTranslationalLimitMotor_setLowerLimit(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_lowerLimit, value);
}

void btTranslationalLimitMotor_setMaxMotorForce(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_maxMotorForce, value);
}

void btTranslationalLimitMotor_setNormalCFM(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normalCFM, value);
}

void btTranslationalLimitMotor_setRestitution(btTranslationalLimitMotor* obj, btScalar value)
{
	obj->m_restitution = value;
}

void btTranslationalLimitMotor_setStopCFM(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_stopCFM, value);
}

void btTranslationalLimitMotor_setStopERP(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_stopERP, value);
}

void btTranslationalLimitMotor_setTargetVelocity(btTranslationalLimitMotor* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_targetVelocity, value);
}

void btTranslationalLimitMotor_setUpperLimit(btTranslationalLimitMotor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_upperLimit, value);
}

btScalar btTranslationalLimitMotor_solveLinearAxis(btTranslationalLimitMotor* obj,
	btScalar timeStep, btScalar jacDiagABInv, btRigidBody* body1, const btVector3* pointInA,
	btRigidBody* body2, const btVector3* pointInB, int limit_index, const btVector3* axis_normal_on_a,
	const btVector3* anchorPos)
{
	BTVECTOR3_IN(pointInA);
	BTVECTOR3_IN(pointInB);
	BTVECTOR3_IN(axis_normal_on_a);
	BTVECTOR3_IN(anchorPos);
	return obj->solveLinearAxis(timeStep, jacDiagABInv, *body1, BTVECTOR3_USE(pointInA),
		*body2, BTVECTOR3_USE(pointInB), limit_index, BTVECTOR3_USE(axis_normal_on_a),
		BTVECTOR3_USE(anchorPos));
}

int btTranslationalLimitMotor_testLimitValue(btTranslationalLimitMotor* obj, int limitIndex,
	btScalar test_value)
{
	return obj->testLimitValue(limitIndex, test_value);
}

void btTranslationalLimitMotor_delete(btTranslationalLimitMotor* obj)
{
	delete obj;
}


btGeneric6DofConstraint* btGeneric6DofConstraint_new(btRigidBody* rbA, btRigidBody* rbB,
	const btTransform* frameInA, const btTransform* frameInB, bool useLinearReferenceFrameA)
{
	BTTRANSFORM_IN(frameInA);
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA), BTTRANSFORM_USE(frameInB),
		useLinearReferenceFrameA);
}

btGeneric6DofConstraint* btGeneric6DofConstraint_new2(btRigidBody* rbB, const btTransform* frameInB,
	bool useLinearReferenceFrameB)
{
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofConstraint(*rbB, BTTRANSFORM_USE(frameInB), useLinearReferenceFrameB);
}

void btGeneric6DofConstraint_calcAnchorPos(btGeneric6DofConstraint* obj)
{
	obj->calcAnchorPos();
}

void btGeneric6DofConstraint_calculateTransforms(btGeneric6DofConstraint* obj, const btTransform* transA,
	const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->calculateTransforms(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btGeneric6DofConstraint_calculateTransforms2(btGeneric6DofConstraint* obj)
{
	obj->calculateTransforms();
}

int btGeneric6DofConstraint_get_limit_motor_info2(btGeneric6DofConstraint* obj,
	btRotationalLimitMotor* limot, const btTransform* transA, const btTransform* transB,
	const btVector3* linVelA, const btVector3* linVelB, const btVector3* angVelA, const btVector3* angVelB,
	btTypedConstraint_btConstraintInfo2* info, int row, btVector3* ax1, int rotational,
	int rotAllowed = 0)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(linVelA);
	BTVECTOR3_IN(linVelB);
	BTVECTOR3_IN(angVelA);
	BTVECTOR3_IN(angVelB);
	BTVECTOR3_IN(ax1);
	int ret = obj->get_limit_motor_info2(limot, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(linVelA), BTVECTOR3_USE(linVelB), BTVECTOR3_USE(angVelA), BTVECTOR3_USE(angVelB),
		info, row, BTVECTOR3_USE(ax1), rotational, rotAllowed);
	BTVECTOR3_DEF_OUT(ax1);
	return ret;
}

btScalar btGeneric6DofConstraint_getAngle(btGeneric6DofConstraint* obj, int axis_index)
{
	return obj->getAngle(axis_index);
}

void btGeneric6DofConstraint_getAngularLowerLimit(btGeneric6DofConstraint* obj, btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->getAngularLowerLimit(BTVECTOR3_USE(angularLower));
	BTVECTOR3_DEF_OUT(angularLower);
}

void btGeneric6DofConstraint_getAngularUpperLimit(btGeneric6DofConstraint* obj, btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->getAngularUpperLimit(BTVECTOR3_USE(angularUpper));
	BTVECTOR3_DEF_OUT(angularUpper);
}

void btGeneric6DofConstraint_getAxis(btGeneric6DofConstraint* obj, int axis_index,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAxis(axis_index);
	BTVECTOR3_SET(value, temp);
}

void btGeneric6DofConstraint_getCalculatedTransformA(btGeneric6DofConstraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformA());
}

void btGeneric6DofConstraint_getCalculatedTransformB(btGeneric6DofConstraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformB());
}

int btGeneric6DofConstraint_getFlags(btGeneric6DofConstraint* obj)
{
	return obj->getFlags();
}

void btGeneric6DofConstraint_getFrameOffsetA(btGeneric6DofConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetA());
}

void btGeneric6DofConstraint_getFrameOffsetB(btGeneric6DofConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetB());
}

void btGeneric6DofConstraint_getInfo1NonVirtual(btGeneric6DofConstraint* obj, btTypedConstraint_btConstraintInfo1* info)
{
	obj->getInfo1NonVirtual(info);
}

void btGeneric6DofConstraint_getInfo2NonVirtual(btGeneric6DofConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* transA, const btTransform* transB, const btVector3* linVelA,
	const btVector3* linVelB, const btVector3* angVelA, const btVector3* angVelB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(linVelA);
	BTVECTOR3_IN(linVelB);
	BTVECTOR3_IN(angVelA);
	BTVECTOR3_IN(angVelB);
	obj->getInfo2NonVirtual(info, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(linVelA), BTVECTOR3_USE(linVelB), BTVECTOR3_USE(angVelA), BTVECTOR3_USE(angVelB));
}

void btGeneric6DofConstraint_getLinearLowerLimit(btGeneric6DofConstraint* obj, btVector3* linearLower)
{
	BTVECTOR3_IN(linearLower);
	obj->getLinearLowerLimit(BTVECTOR3_USE(linearLower));
	BTVECTOR3_DEF_OUT(linearLower);
}

void btGeneric6DofConstraint_getLinearUpperLimit(btGeneric6DofConstraint* obj, btVector3* linearUpper)
{
	BTVECTOR3_IN(linearUpper);
	obj->getLinearUpperLimit(BTVECTOR3_USE(linearUpper));
	BTVECTOR3_DEF_OUT(linearUpper);
}

btScalar btGeneric6DofConstraint_getRelativePivotPosition(btGeneric6DofConstraint* obj,
	int axis_index)
{
	return obj->getRelativePivotPosition(axis_index);
}

btRotationalLimitMotor* btGeneric6DofConstraint_getRotationalLimitMotor(btGeneric6DofConstraint* obj,
	int index)
{
	return obj->getRotationalLimitMotor(index);
}

btTranslationalLimitMotor* btGeneric6DofConstraint_getTranslationalLimitMotor(btGeneric6DofConstraint* obj)
{
	return obj->getTranslationalLimitMotor();
}

bool btGeneric6DofConstraint_getUseFrameOffset(btGeneric6DofConstraint* obj)
{
	return obj->getUseFrameOffset();
}

bool btGeneric6DofConstraint_getUseLinearReferenceFrameA(btGeneric6DofConstraint* obj)
{
	return obj->getUseLinearReferenceFrameA();
}

bool btGeneric6DofConstraint_getUseSolveConstraintObsolete(btGeneric6DofConstraint* obj)
{
	return obj->m_useSolveConstraintObsolete;
}

bool btGeneric6DofConstraint_isLimited(btGeneric6DofConstraint* obj, int limitIndex)
{
	return obj->isLimited(limitIndex);
}

void btGeneric6DofConstraint_setAngularLowerLimit(btGeneric6DofConstraint* obj, const btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->setAngularLowerLimit(BTVECTOR3_USE(angularLower));
}

void btGeneric6DofConstraint_setAngularUpperLimit(btGeneric6DofConstraint* obj, const btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->setAngularUpperLimit(BTVECTOR3_USE(angularUpper));
}

void btGeneric6DofConstraint_setAxis(btGeneric6DofConstraint* obj, const btVector3* axis1,
	const btVector3* axis2)
{
	BTVECTOR3_IN(axis1);
	BTVECTOR3_IN(axis2);
	obj->setAxis(BTVECTOR3_USE(axis1), BTVECTOR3_USE(axis2));
}

void btGeneric6DofConstraint_setFrames(btGeneric6DofConstraint* obj, const btTransform* frameA,
	const btTransform* frameB)
{
	BTTRANSFORM_IN(frameA);
	BTTRANSFORM_IN(frameB);
	obj->setFrames(BTTRANSFORM_USE(frameA), BTTRANSFORM_USE(frameB));
}

void btGeneric6DofConstraint_setLimit(btGeneric6DofConstraint* obj, int axis, btScalar lo,
	btScalar hi)
{
	obj->setLimit(axis, lo, hi);
}

void btGeneric6DofConstraint_setLinearLowerLimit(btGeneric6DofConstraint* obj, const btVector3* linearLower)
{
	BTVECTOR3_IN(linearLower);
	obj->setLinearLowerLimit(BTVECTOR3_USE(linearLower));
}

void btGeneric6DofConstraint_setLinearUpperLimit(btGeneric6DofConstraint* obj, const btVector3* linearUpper)
{
	BTVECTOR3_IN(linearUpper);
	obj->setLinearUpperLimit(BTVECTOR3_USE(linearUpper));
}

void btGeneric6DofConstraint_setUseFrameOffset(btGeneric6DofConstraint* obj, bool frameOffsetOnOff)
{
	obj->setUseFrameOffset(frameOffsetOnOff);
}

void btGeneric6DofConstraint_setUseLinearReferenceFrameA(btGeneric6DofConstraint* obj,
	bool linearReferenceFrameA)
{
	obj->setUseLinearReferenceFrameA(linearReferenceFrameA);
}

void btGeneric6DofConstraint_setUseSolveConstraintObsolete(btGeneric6DofConstraint* obj,
	bool value)
{
	obj->m_useSolveConstraintObsolete = value;
}

bool btGeneric6DofConstraint_testAngularLimitMotor(btGeneric6DofConstraint* obj,
	int axis_index)
{
	return obj->testAngularLimitMotor(axis_index);
}

void btGeneric6DofConstraint_updateRHS(btGeneric6DofConstraint* obj, btScalar timeStep)
{
	obj->updateRHS(timeStep);
}
