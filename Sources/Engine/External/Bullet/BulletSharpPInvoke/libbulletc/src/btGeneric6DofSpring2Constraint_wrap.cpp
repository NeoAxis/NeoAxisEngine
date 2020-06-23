#include <BulletDynamics/ConstraintSolver/btGeneric6DofSpring2Constraint.h>

#include "conversion.h"
#include "btGeneric6DofSpring2Constraint_wrap.h"

btRotationalLimitMotor2* btRotationalLimitMotor2_new()
{
	return new btRotationalLimitMotor2();
}

btRotationalLimitMotor2* btRotationalLimitMotor2_new2(const btRotationalLimitMotor2* limot)
{
	return new btRotationalLimitMotor2(*limot);
}

btScalar btRotationalLimitMotor2_getBounce(btRotationalLimitMotor2* obj)
{
	return obj->m_bounce;
}

int btRotationalLimitMotor2_getCurrentLimit(btRotationalLimitMotor2* obj)
{
	return obj->m_currentLimit;
}

btScalar btRotationalLimitMotor2_getCurrentLimitError(btRotationalLimitMotor2* obj)
{
	return obj->m_currentLimitError;
}

btScalar btRotationalLimitMotor2_getCurrentLimitErrorHi(btRotationalLimitMotor2* obj)
{
	return obj->m_currentLimitErrorHi;
}

btScalar btRotationalLimitMotor2_getCurrentPosition(btRotationalLimitMotor2* obj)
{
	return obj->m_currentPosition;
}

bool btRotationalLimitMotor2_getEnableMotor(btRotationalLimitMotor2* obj)
{
	return obj->m_enableMotor;
}

bool btRotationalLimitMotor2_getEnableSpring(btRotationalLimitMotor2* obj)
{
	return obj->m_enableSpring;
}

btScalar btRotationalLimitMotor2_getEquilibriumPoint(btRotationalLimitMotor2* obj)
{
	return obj->m_equilibriumPoint;
}

btScalar btRotationalLimitMotor2_getHiLimit(btRotationalLimitMotor2* obj)
{
	return obj->m_hiLimit;
}

btScalar btRotationalLimitMotor2_getLoLimit(btRotationalLimitMotor2* obj)
{
	return obj->m_loLimit;
}

btScalar btRotationalLimitMotor2_getMaxMotorForce(btRotationalLimitMotor2* obj)
{
	return obj->m_maxMotorForce;
}

btScalar btRotationalLimitMotor2_getMotorCFM(btRotationalLimitMotor2* obj)
{
	return obj->m_motorCFM;
}

btScalar btRotationalLimitMotor2_getMotorERP(btRotationalLimitMotor2* obj)
{
	return obj->m_motorERP;
}

bool btRotationalLimitMotor2_getServoMotor(btRotationalLimitMotor2* obj)
{
	return obj->m_servoMotor;
}

btScalar btRotationalLimitMotor2_getServoTarget(btRotationalLimitMotor2* obj)
{
	return obj->m_servoTarget;
}

btScalar btRotationalLimitMotor2_getSpringDamping(btRotationalLimitMotor2* obj)
{
	return obj->m_springDamping;
}

bool btRotationalLimitMotor2_getSpringDampingLimited(btRotationalLimitMotor2* obj)
{
	return obj->m_springDampingLimited;
}

btScalar btRotationalLimitMotor2_getSpringStiffness(btRotationalLimitMotor2* obj)
{
	return obj->m_springStiffness;
}

bool btRotationalLimitMotor2_getSpringStiffnessLimited(btRotationalLimitMotor2* obj)
{
	return obj->m_springStiffnessLimited;
}

btScalar btRotationalLimitMotor2_getStopCFM(btRotationalLimitMotor2* obj)
{
	return obj->m_stopCFM;
}

btScalar btRotationalLimitMotor2_getStopERP(btRotationalLimitMotor2* obj)
{
	return obj->m_stopERP;
}

btScalar btRotationalLimitMotor2_getTargetVelocity(btRotationalLimitMotor2* obj)
{
	return obj->m_targetVelocity;
}

bool btRotationalLimitMotor2_isLimited(btRotationalLimitMotor2* obj)
{
	return obj->isLimited();
}

void btRotationalLimitMotor2_setBounce(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_bounce = value;
}

void btRotationalLimitMotor2_setCurrentLimit(btRotationalLimitMotor2* obj, int value)
{
	obj->m_currentLimit = value;
}

void btRotationalLimitMotor2_setCurrentLimitError(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_currentLimitError = value;
}

void btRotationalLimitMotor2_setCurrentLimitErrorHi(btRotationalLimitMotor2* obj,
	btScalar value)
{
	obj->m_currentLimitErrorHi = value;
}

void btRotationalLimitMotor2_setCurrentPosition(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_currentPosition = value;
}

void btRotationalLimitMotor2_setEnableMotor(btRotationalLimitMotor2* obj, bool value)
{
	obj->m_enableMotor = value;
}

void btRotationalLimitMotor2_setEnableSpring(btRotationalLimitMotor2* obj, bool value)
{
	obj->m_enableSpring = value;
}

void btRotationalLimitMotor2_setEquilibriumPoint(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_equilibriumPoint = value;
}

void btRotationalLimitMotor2_setHiLimit(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_hiLimit = value;
}

void btRotationalLimitMotor2_setLoLimit(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_loLimit = value;
}

void btRotationalLimitMotor2_setMaxMotorForce(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_maxMotorForce = value;
}

void btRotationalLimitMotor2_setMotorCFM(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_motorCFM = value;
}

void btRotationalLimitMotor2_setMotorERP(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_motorERP = value;
}

void btRotationalLimitMotor2_setServoMotor(btRotationalLimitMotor2* obj, bool value)
{
	obj->m_servoMotor = value;
}

void btRotationalLimitMotor2_setServoTarget(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_servoTarget = value;
}

void btRotationalLimitMotor2_setSpringDamping(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_springDamping = value;
}

void btRotationalLimitMotor2_setSpringDampingLimited(btRotationalLimitMotor2* obj,
	bool value)
{
	obj->m_springDampingLimited = value;
}

void btRotationalLimitMotor2_setSpringStiffness(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_springStiffness = value;
}

void btRotationalLimitMotor2_setSpringStiffnessLimited(btRotationalLimitMotor2* obj,
	bool value)
{
	obj->m_springStiffnessLimited = value;
}

void btRotationalLimitMotor2_setStopCFM(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_stopCFM = value;
}

void btRotationalLimitMotor2_setStopERP(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_stopERP = value;
}

void btRotationalLimitMotor2_setTargetVelocity(btRotationalLimitMotor2* obj, btScalar value)
{
	obj->m_targetVelocity = value;
}

void btRotationalLimitMotor2_testLimitValue(btRotationalLimitMotor2* obj, btScalar test_value)
{
	obj->testLimitValue(test_value);
}

void btRotationalLimitMotor2_delete(btRotationalLimitMotor2* obj)
{
	delete obj;
}


btTranslationalLimitMotor2* btTranslationalLimitMotor2_new()
{
	return new btTranslationalLimitMotor2();
}

btTranslationalLimitMotor2* btTranslationalLimitMotor2_new2(const btTranslationalLimitMotor2* other)
{
	return new btTranslationalLimitMotor2(*other);
}

void btTranslationalLimitMotor2_getBounce(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_bounce);
}

int* btTranslationalLimitMotor2_getCurrentLimit(btTranslationalLimitMotor2* obj)
{
	return obj->m_currentLimit;
}

void btTranslationalLimitMotor2_getCurrentLimitError(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_currentLimitError);
}

void btTranslationalLimitMotor2_getCurrentLimitErrorHi(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_currentLimitErrorHi);
}

void btTranslationalLimitMotor2_getCurrentLinearDiff(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_currentLinearDiff);
}

bool* btTranslationalLimitMotor2_getEnableMotor(btTranslationalLimitMotor2* obj)
{
	return obj->m_enableMotor;
}

bool* btTranslationalLimitMotor2_getEnableSpring(btTranslationalLimitMotor2* obj)
{
	return obj->m_enableSpring;
}

void btTranslationalLimitMotor2_getEquilibriumPoint(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_equilibriumPoint);
}

void btTranslationalLimitMotor2_getLowerLimit(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_lowerLimit);
}

void btTranslationalLimitMotor2_getMaxMotorForce(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_maxMotorForce);
}

void btTranslationalLimitMotor2_getMotorCFM(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_motorCFM);
}

void btTranslationalLimitMotor2_getMotorERP(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_motorERP);
}

bool* btTranslationalLimitMotor2_getServoMotor(btTranslationalLimitMotor2* obj)
{
	return obj->m_servoMotor;
}

void btTranslationalLimitMotor2_getServoTarget(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_servoTarget);
}

void btTranslationalLimitMotor2_getSpringDamping(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_springDamping);
}

bool* btTranslationalLimitMotor2_getSpringDampingLimited(btTranslationalLimitMotor2* obj)
{
	return obj->m_springDampingLimited;
}

void btTranslationalLimitMotor2_getSpringStiffness(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_springStiffness);
}

bool* btTranslationalLimitMotor2_getSpringStiffnessLimited(btTranslationalLimitMotor2* obj)
{
	return obj->m_springStiffnessLimited;
}

void btTranslationalLimitMotor2_getStopCFM(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_stopCFM);
}

void btTranslationalLimitMotor2_getStopERP(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_stopERP);
}

void btTranslationalLimitMotor2_getTargetVelocity(btTranslationalLimitMotor2* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_targetVelocity);
}

void btTranslationalLimitMotor2_getUpperLimit(btTranslationalLimitMotor2* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_upperLimit);
}

bool btTranslationalLimitMotor2_isLimited(btTranslationalLimitMotor2* obj, int limitIndex)
{
	return obj->isLimited(limitIndex);
}

void btTranslationalLimitMotor2_setBounce(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_bounce, value);
}

void btTranslationalLimitMotor2_setCurrentLimitError(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_currentLimitError, value);
}

void btTranslationalLimitMotor2_setCurrentLimitErrorHi(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_currentLimitErrorHi, value);
}

void btTranslationalLimitMotor2_setCurrentLinearDiff(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_currentLinearDiff, value);
}

void btTranslationalLimitMotor2_setEquilibriumPoint(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_equilibriumPoint, value);
}

void btTranslationalLimitMotor2_setLowerLimit(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_lowerLimit, value);
}

void btTranslationalLimitMotor2_setMaxMotorForce(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_maxMotorForce, value);
}

void btTranslationalLimitMotor2_setMotorCFM(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_motorCFM, value);
}

void btTranslationalLimitMotor2_setMotorERP(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_motorERP, value);
}

void btTranslationalLimitMotor2_setServoTarget(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_servoTarget, value);
}

void btTranslationalLimitMotor2_setSpringDamping(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_springDamping, value);
}

void btTranslationalLimitMotor2_setSpringStiffness(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_springStiffness, value);
}

void btTranslationalLimitMotor2_setStopCFM(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_stopCFM, value);
}

void btTranslationalLimitMotor2_setStopERP(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_stopERP, value);
}

void btTranslationalLimitMotor2_setTargetVelocity(btTranslationalLimitMotor2* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_targetVelocity, value);
}

void btTranslationalLimitMotor2_setUpperLimit(btTranslationalLimitMotor2* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_upperLimit, value);
}

void btTranslationalLimitMotor2_testLimitValue(btTranslationalLimitMotor2* obj, int limitIndex,
	btScalar test_value)
{
	obj->testLimitValue(limitIndex, test_value);
}

void btTranslationalLimitMotor2_delete(btTranslationalLimitMotor2* obj)
{
	delete obj;
}


btGeneric6DofSpring2Constraint* btGeneric6DofSpring2Constraint_new(btRigidBody* rbA,
	btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB, RotateOrder rotOrder)
{
	BTTRANSFORM_IN(frameInA);
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofSpring2Constraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA),
		BTTRANSFORM_USE(frameInB), rotOrder);
}

btGeneric6DofSpring2Constraint* btGeneric6DofSpring2Constraint_new2(btRigidBody* rbB,
	const btTransform* frameInB, RotateOrder rotOrder)
{
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofSpring2Constraint(*rbB, BTTRANSFORM_USE(frameInB), rotOrder);
}

btScalar btGeneric6DofSpring2Constraint_btGetMatrixElem(const btMatrix3x3* mat, int index)
{
	BTMATRIX3X3_IN(mat);
	return btGeneric6DofSpring2Constraint::btGetMatrixElem(BTMATRIX3X3_USE(mat),
		index);
}

void btGeneric6DofSpring2Constraint_calculateTransforms(btGeneric6DofSpring2Constraint* obj,
	const btTransform* transA, const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->calculateTransforms(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btGeneric6DofSpring2Constraint_calculateTransforms2(btGeneric6DofSpring2Constraint* obj)
{
	obj->calculateTransforms();
}

void btGeneric6DofSpring2Constraint_enableMotor(btGeneric6DofSpring2Constraint* obj,
	int index, bool onOff)
{
	obj->enableMotor(index, onOff);
}

void btGeneric6DofSpring2Constraint_enableSpring(btGeneric6DofSpring2Constraint* obj,
	int index, bool onOff)
{
	obj->enableSpring(index, onOff);
}

btScalar btGeneric6DofSpring2Constraint_getAngle(btGeneric6DofSpring2Constraint* obj,
	int axis_index)
{
	return obj->getAngle(axis_index);
}

void btGeneric6DofSpring2Constraint_getAngularLowerLimit(btGeneric6DofSpring2Constraint* obj,
	btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->getAngularLowerLimit(BTVECTOR3_USE(angularLower));
	BTVECTOR3_DEF_OUT(angularLower);
}

void btGeneric6DofSpring2Constraint_getAngularLowerLimitReversed(btGeneric6DofSpring2Constraint* obj,
	btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->getAngularLowerLimitReversed(BTVECTOR3_USE(angularLower));
	BTVECTOR3_DEF_OUT(angularLower);
}

void btGeneric6DofSpring2Constraint_getAngularUpperLimit(btGeneric6DofSpring2Constraint* obj,
	btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->getAngularUpperLimit(BTVECTOR3_USE(angularUpper));
	BTVECTOR3_DEF_OUT(angularUpper);
}

void btGeneric6DofSpring2Constraint_getAngularUpperLimitReversed(btGeneric6DofSpring2Constraint* obj,
	btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->getAngularUpperLimitReversed(BTVECTOR3_USE(angularUpper));
	BTVECTOR3_DEF_OUT(angularUpper);
}

void btGeneric6DofSpring2Constraint_getAxis(btGeneric6DofSpring2Constraint* obj,
	int axis_index, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAxis(axis_index);
	BTVECTOR3_SET(value, temp);
}

void btGeneric6DofSpring2Constraint_getCalculatedTransformA(btGeneric6DofSpring2Constraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformA());
}

void btGeneric6DofSpring2Constraint_getCalculatedTransformB(btGeneric6DofSpring2Constraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformB());
}

void btGeneric6DofSpring2Constraint_getFrameOffsetA(btGeneric6DofSpring2Constraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetA());
}

void btGeneric6DofSpring2Constraint_getFrameOffsetB(btGeneric6DofSpring2Constraint* obj,
	btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetB());
}

void btGeneric6DofSpring2Constraint_getLinearLowerLimit(btGeneric6DofSpring2Constraint* obj,
	btVector3* linearLower)
{
	BTVECTOR3_IN(linearLower);
	obj->getLinearLowerLimit(BTVECTOR3_USE(linearLower));
	BTVECTOR3_DEF_OUT(linearLower);
}

void btGeneric6DofSpring2Constraint_getLinearUpperLimit(btGeneric6DofSpring2Constraint* obj,
	btVector3* linearUpper)
{
	BTVECTOR3_IN(linearUpper);
	obj->getLinearUpperLimit(BTVECTOR3_USE(linearUpper));
	BTVECTOR3_DEF_OUT(linearUpper);
}

btScalar btGeneric6DofSpring2Constraint_getRelativePivotPosition(btGeneric6DofSpring2Constraint* obj,
	int axis_index)
{
	return obj->getRelativePivotPosition(axis_index);
}

btRotationalLimitMotor2* btGeneric6DofSpring2Constraint_getRotationalLimitMotor(btGeneric6DofSpring2Constraint* obj,
	int index)
{
	return obj->getRotationalLimitMotor(index);
}

RotateOrder btGeneric6DofSpring2Constraint_getRotationOrder(btGeneric6DofSpring2Constraint* obj)
{
	return obj->getRotationOrder();
}

btTranslationalLimitMotor2* btGeneric6DofSpring2Constraint_getTranslationalLimitMotor(
	btGeneric6DofSpring2Constraint* obj)
{
	return obj->getTranslationalLimitMotor();
}

bool btGeneric6DofSpring2Constraint_isLimited(btGeneric6DofSpring2Constraint* obj,
	int limitIndex)
{
	return obj->isLimited(limitIndex);
}

bool btGeneric6DofSpring2Constraint_matrixToEulerZXY(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerZXY(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

bool btGeneric6DofSpring2Constraint_matrixToEulerZYX(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerZYX(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

bool btGeneric6DofSpring2Constraint_matrixToEulerXZY(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerXZY(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

bool btGeneric6DofSpring2Constraint_matrixToEulerXYZ(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerXYZ(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

bool btGeneric6DofSpring2Constraint_matrixToEulerYZX(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerYZX(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

bool btGeneric6DofSpring2Constraint_matrixToEulerYXZ(const btMatrix3x3* mat, btVector3* xyz)
{
	BTMATRIX3X3_IN(mat);
	BTVECTOR3_IN(xyz);
	bool ret = btGeneric6DofSpring2Constraint::matrixToEulerYXZ(BTMATRIX3X3_USE(mat),
		BTVECTOR3_USE(xyz));
	BTVECTOR3_DEF_OUT(xyz);
	return ret;
}

void btGeneric6DofSpring2Constraint_setAngularLowerLimit(btGeneric6DofSpring2Constraint* obj,
	const btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->setAngularLowerLimit(BTVECTOR3_USE(angularLower));
}

void btGeneric6DofSpring2Constraint_setAngularLowerLimitReversed(btGeneric6DofSpring2Constraint* obj,
	const btVector3* angularLower)
{
	BTVECTOR3_IN(angularLower);
	obj->setAngularLowerLimitReversed(BTVECTOR3_USE(angularLower));
}

void btGeneric6DofSpring2Constraint_setAngularUpperLimit(btGeneric6DofSpring2Constraint* obj,
	const btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->setAngularUpperLimit(BTVECTOR3_USE(angularUpper));
}

void btGeneric6DofSpring2Constraint_setAngularUpperLimitReversed(btGeneric6DofSpring2Constraint* obj,
	const btVector3* angularUpper)
{
	BTVECTOR3_IN(angularUpper);
	obj->setAngularUpperLimitReversed(BTVECTOR3_USE(angularUpper));
}

void btGeneric6DofSpring2Constraint_setAxis(btGeneric6DofSpring2Constraint* obj,
	const btVector3* axis1, const btVector3* axis2)
{
	BTVECTOR3_IN(axis1);
	BTVECTOR3_IN(axis2);
	obj->setAxis(BTVECTOR3_USE(axis1), BTVECTOR3_USE(axis2));
}

void btGeneric6DofSpring2Constraint_setBounce(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar bounce)
{
	obj->setBounce(index, bounce);
}

void btGeneric6DofSpring2Constraint_setDamping(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar damping, bool limitIfNeeded)
{
	obj->setDamping(index, damping, limitIfNeeded);
}

void btGeneric6DofSpring2Constraint_setEquilibriumPoint(btGeneric6DofSpring2Constraint* obj)
{
	obj->setEquilibriumPoint();
}

void btGeneric6DofSpring2Constraint_setEquilibriumPoint2(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar val)
{
	obj->setEquilibriumPoint(index, val);
}

void btGeneric6DofSpring2Constraint_setEquilibriumPoint3(btGeneric6DofSpring2Constraint* obj,
	int index)
{
	obj->setEquilibriumPoint(index);
}

void btGeneric6DofSpring2Constraint_setFrames(btGeneric6DofSpring2Constraint* obj,
	const btTransform* frameA, const btTransform* frameB)
{
	BTTRANSFORM_IN(frameA);
	BTTRANSFORM_IN(frameB);
	obj->setFrames(BTTRANSFORM_USE(frameA), BTTRANSFORM_USE(frameB));
}

void btGeneric6DofSpring2Constraint_setLimit(btGeneric6DofSpring2Constraint* obj,
	int axis, btScalar lo, btScalar hi)
{
	obj->setLimit(axis, lo, hi);
}

void btGeneric6DofSpring2Constraint_setLimitReversed(btGeneric6DofSpring2Constraint* obj,
	int axis, btScalar lo, btScalar hi)
{
	obj->setLimitReversed(axis, lo, hi);
}

void btGeneric6DofSpring2Constraint_setLinearLowerLimit(btGeneric6DofSpring2Constraint* obj,
	const btVector3* linearLower)
{
	BTVECTOR3_IN(linearLower);
	obj->setLinearLowerLimit(BTVECTOR3_USE(linearLower));
}

void btGeneric6DofSpring2Constraint_setLinearUpperLimit(btGeneric6DofSpring2Constraint* obj,
	const btVector3* linearUpper)
{
	BTVECTOR3_IN(linearUpper);
	obj->setLinearUpperLimit(BTVECTOR3_USE(linearUpper));
}

void btGeneric6DofSpring2Constraint_setMaxMotorForce(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar force)
{
	obj->setMaxMotorForce(index, force);
}

void btGeneric6DofSpring2Constraint_setRotationOrder(btGeneric6DofSpring2Constraint* obj,
	RotateOrder order)
{
	obj->setRotationOrder(order);
}

void btGeneric6DofSpring2Constraint_setServo(btGeneric6DofSpring2Constraint* obj,
	int index, bool onOff)
{
	obj->setServo(index, onOff);
}

void btGeneric6DofSpring2Constraint_setServoTarget(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar target)
{
	obj->setServoTarget(index, target);
}

void btGeneric6DofSpring2Constraint_setStiffness(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar stiffness, bool limitIfNeeded)
{
	obj->setStiffness(index, stiffness, limitIfNeeded);
}

void btGeneric6DofSpring2Constraint_setTargetVelocity(btGeneric6DofSpring2Constraint* obj,
	int index, btScalar velocity)
{
	obj->setTargetVelocity(index, velocity);
}
