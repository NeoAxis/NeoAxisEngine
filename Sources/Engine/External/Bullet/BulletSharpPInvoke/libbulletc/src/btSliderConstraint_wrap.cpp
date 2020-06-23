#include <BulletDynamics/ConstraintSolver/btSliderConstraint.h>

#include "conversion.h"
#include "btSliderConstraint_wrap.h"

btSliderConstraint* btSliderConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA,
	const btTransform* frameInB, bool useLinearReferenceFrameA)
{
	BTTRANSFORM_IN(frameInA);
	BTTRANSFORM_IN(frameInB);
	return new btSliderConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA), BTTRANSFORM_USE(frameInB),
		useLinearReferenceFrameA);
}

btSliderConstraint* btSliderConstraint_new2(btRigidBody* rbB, const btTransform* frameInB,
	bool useLinearReferenceFrameA)
{
	BTTRANSFORM_IN(frameInB);
	return new btSliderConstraint(*rbB, BTTRANSFORM_USE(frameInB), useLinearReferenceFrameA);
}

void btSliderConstraint_calculateTransforms(btSliderConstraint* obj, const btTransform* transA,
	const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->calculateTransforms(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btSliderConstraint_getAncorInA(btSliderConstraint* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAncorInA();
	BTVECTOR3_SET(value, temp);
}

void btSliderConstraint_getAncorInB(btSliderConstraint* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAncorInB();
	BTVECTOR3_SET(value, temp);
}

btScalar btSliderConstraint_getAngDepth(btSliderConstraint* obj)
{
	return obj->getAngDepth();
}

btScalar btSliderConstraint_getAngularPos(btSliderConstraint* obj)
{
	return obj->getAngularPos();
}

void btSliderConstraint_getCalculatedTransformA(btSliderConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformA());
}

void btSliderConstraint_getCalculatedTransformB(btSliderConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCalculatedTransformB());
}

btScalar btSliderConstraint_getDampingDirAng(btSliderConstraint* obj)
{
	return obj->getDampingDirAng();
}

btScalar btSliderConstraint_getDampingDirLin(btSliderConstraint* obj)
{
	return obj->getDampingDirLin();
}

btScalar btSliderConstraint_getDampingLimAng(btSliderConstraint* obj)
{
	return obj->getDampingLimAng();
}

btScalar btSliderConstraint_getDampingLimLin(btSliderConstraint* obj)
{
	return obj->getDampingLimLin();
}

btScalar btSliderConstraint_getDampingOrthoAng(btSliderConstraint* obj)
{
	return obj->getDampingOrthoAng();
}

btScalar btSliderConstraint_getDampingOrthoLin(btSliderConstraint* obj)
{
	return obj->getDampingOrthoLin();
}

int btSliderConstraint_getFlags(btSliderConstraint* obj)
{
	return obj->getFlags();
}

void btSliderConstraint_getFrameOffsetA(btSliderConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetA());
}

void btSliderConstraint_getFrameOffsetB(btSliderConstraint* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getFrameOffsetB());
}

void btSliderConstraint_getInfo1NonVirtual(btSliderConstraint* obj, btTypedConstraint_btConstraintInfo1* info)
{
	obj->getInfo1NonVirtual(info);
}

void btSliderConstraint_getInfo2NonVirtual(btSliderConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* transA, const btTransform* transB, const btVector3* linVelA,
	const btVector3* linVelB, btScalar rbAinvMass, btScalar rbBinvMass)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_IN(linVelA);
	BTVECTOR3_IN(linVelB);
	obj->getInfo2NonVirtual(info, BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB),
		BTVECTOR3_USE(linVelA), BTVECTOR3_USE(linVelB), rbAinvMass, rbBinvMass);
}

btScalar btSliderConstraint_getLinDepth(btSliderConstraint* obj)
{
	return obj->getLinDepth();
}

btScalar btSliderConstraint_getLinearPos(btSliderConstraint* obj)
{
	return obj->getLinearPos();
}

btScalar btSliderConstraint_getLowerAngLimit(btSliderConstraint* obj)
{
	return obj->getLowerAngLimit();
}

btScalar btSliderConstraint_getLowerLinLimit(btSliderConstraint* obj)
{
	return obj->getLowerLinLimit();
}

btScalar btSliderConstraint_getMaxAngMotorForce(btSliderConstraint* obj)
{
	return obj->getMaxAngMotorForce();
}

btScalar btSliderConstraint_getMaxLinMotorForce(btSliderConstraint* obj)
{
	return obj->getMaxLinMotorForce();
}

bool btSliderConstraint_getPoweredAngMotor(btSliderConstraint* obj)
{
	return obj->getPoweredAngMotor();
}

bool btSliderConstraint_getPoweredLinMotor(btSliderConstraint* obj)
{
	return obj->getPoweredLinMotor();
}

btScalar btSliderConstraint_getRestitutionDirAng(btSliderConstraint* obj)
{
	return obj->getRestitutionDirAng();
}

btScalar btSliderConstraint_getRestitutionDirLin(btSliderConstraint* obj)
{
	return obj->getRestitutionDirLin();
}

btScalar btSliderConstraint_getRestitutionLimAng(btSliderConstraint* obj)
{
	return obj->getRestitutionLimAng();
}

btScalar btSliderConstraint_getRestitutionLimLin(btSliderConstraint* obj)
{
	return obj->getRestitutionLimLin();
}

btScalar btSliderConstraint_getRestitutionOrthoAng(btSliderConstraint* obj)
{
	return obj->getRestitutionOrthoAng();
}

btScalar btSliderConstraint_getRestitutionOrthoLin(btSliderConstraint* obj)
{
	return obj->getRestitutionOrthoLin();
}

btScalar btSliderConstraint_getSoftnessDirAng(btSliderConstraint* obj)
{
	return obj->getSoftnessDirAng();
}

btScalar btSliderConstraint_getSoftnessDirLin(btSliderConstraint* obj)
{
	return obj->getSoftnessDirLin();
}

btScalar btSliderConstraint_getSoftnessLimAng(btSliderConstraint* obj)
{
	return obj->getSoftnessLimAng();
}

btScalar btSliderConstraint_getSoftnessLimLin(btSliderConstraint* obj)
{
	return obj->getSoftnessLimLin();
}

btScalar btSliderConstraint_getSoftnessOrthoAng(btSliderConstraint* obj)
{
	return obj->getSoftnessOrthoAng();
}

btScalar btSliderConstraint_getSoftnessOrthoLin(btSliderConstraint* obj)
{
	return obj->getSoftnessOrthoLin();
}

bool btSliderConstraint_getSolveAngLimit(btSliderConstraint* obj)
{
	return obj->getSolveAngLimit();
}

bool btSliderConstraint_getSolveLinLimit(btSliderConstraint* obj)
{
	return obj->getSolveLinLimit();
}

btScalar btSliderConstraint_getTargetAngMotorVelocity(btSliderConstraint* obj)
{
	return obj->getTargetAngMotorVelocity();
}

btScalar btSliderConstraint_getTargetLinMotorVelocity(btSliderConstraint* obj)
{
	return obj->getTargetLinMotorVelocity();
}

btScalar btSliderConstraint_getUpperAngLimit(btSliderConstraint* obj)
{
	return obj->getUpperAngLimit();
}

btScalar btSliderConstraint_getUpperLinLimit(btSliderConstraint* obj)
{
	return obj->getUpperLinLimit();
}

bool btSliderConstraint_getUseFrameOffset(btSliderConstraint* obj)
{
	return obj->getUseFrameOffset();
}

bool btSliderConstraint_getUseLinearReferenceFrameA(btSliderConstraint* obj)
{
	return obj->getUseLinearReferenceFrameA();
}

void btSliderConstraint_setDampingDirAng(btSliderConstraint* obj, btScalar dampingDirAng)
{
	obj->setDampingDirAng(dampingDirAng);
}

void btSliderConstraint_setDampingDirLin(btSliderConstraint* obj, btScalar dampingDirLin)
{
	obj->setDampingDirLin(dampingDirLin);
}

void btSliderConstraint_setDampingLimAng(btSliderConstraint* obj, btScalar dampingLimAng)
{
	obj->setDampingLimAng(dampingLimAng);
}

void btSliderConstraint_setDampingLimLin(btSliderConstraint* obj, btScalar dampingLimLin)
{
	obj->setDampingLimLin(dampingLimLin);
}

void btSliderConstraint_setDampingOrthoAng(btSliderConstraint* obj, btScalar dampingOrthoAng)
{
	obj->setDampingOrthoAng(dampingOrthoAng);
}

void btSliderConstraint_setDampingOrthoLin(btSliderConstraint* obj, btScalar dampingOrthoLin)
{
	obj->setDampingOrthoLin(dampingOrthoLin);
}

void btSliderConstraint_setFrames(btSliderConstraint* obj, const btTransform* frameA,
	const btTransform* frameB)
{
	BTTRANSFORM_IN(frameA);
	BTTRANSFORM_IN(frameB);
	obj->setFrames(BTTRANSFORM_USE(frameA), BTTRANSFORM_USE(frameB));
}

void btSliderConstraint_setLowerAngLimit(btSliderConstraint* obj, btScalar lowerLimit)
{
	obj->setLowerAngLimit(lowerLimit);
}

void btSliderConstraint_setLowerLinLimit(btSliderConstraint* obj, btScalar lowerLimit)
{
	obj->setLowerLinLimit(lowerLimit);
}

void btSliderConstraint_setMaxAngMotorForce(btSliderConstraint* obj, btScalar maxAngMotorForce)
{
	obj->setMaxAngMotorForce(maxAngMotorForce);
}

void btSliderConstraint_setMaxLinMotorForce(btSliderConstraint* obj, btScalar maxLinMotorForce)
{
	obj->setMaxLinMotorForce(maxLinMotorForce);
}

void btSliderConstraint_setPoweredAngMotor(btSliderConstraint* obj, bool onOff)
{
	obj->setPoweredAngMotor(onOff);
}

void btSliderConstraint_setPoweredLinMotor(btSliderConstraint* obj, bool onOff)
{
	obj->setPoweredLinMotor(onOff);
}

void btSliderConstraint_setRestitutionDirAng(btSliderConstraint* obj, btScalar restitutionDirAng)
{
	obj->setRestitutionDirAng(restitutionDirAng);
}

void btSliderConstraint_setRestitutionDirLin(btSliderConstraint* obj, btScalar restitutionDirLin)
{
	obj->setRestitutionDirLin(restitutionDirLin);
}

void btSliderConstraint_setRestitutionLimAng(btSliderConstraint* obj, btScalar restitutionLimAng)
{
	obj->setRestitutionLimAng(restitutionLimAng);
}

void btSliderConstraint_setRestitutionLimLin(btSliderConstraint* obj, btScalar restitutionLimLin)
{
	obj->setRestitutionLimLin(restitutionLimLin);
}

void btSliderConstraint_setRestitutionOrthoAng(btSliderConstraint* obj, btScalar restitutionOrthoAng)
{
	obj->setRestitutionOrthoAng(restitutionOrthoAng);
}

void btSliderConstraint_setRestitutionOrthoLin(btSliderConstraint* obj, btScalar restitutionOrthoLin)
{
	obj->setRestitutionOrthoLin(restitutionOrthoLin);
}

void btSliderConstraint_setSoftnessDirAng(btSliderConstraint* obj, btScalar softnessDirAng)
{
	obj->setSoftnessDirAng(softnessDirAng);
}

void btSliderConstraint_setSoftnessDirLin(btSliderConstraint* obj, btScalar softnessDirLin)
{
	obj->setSoftnessDirLin(softnessDirLin);
}

void btSliderConstraint_setSoftnessLimAng(btSliderConstraint* obj, btScalar softnessLimAng)
{
	obj->setSoftnessLimAng(softnessLimAng);
}

void btSliderConstraint_setSoftnessLimLin(btSliderConstraint* obj, btScalar softnessLimLin)
{
	obj->setSoftnessLimLin(softnessLimLin);
}

void btSliderConstraint_setSoftnessOrthoAng(btSliderConstraint* obj, btScalar softnessOrthoAng)
{
	obj->setSoftnessOrthoAng(softnessOrthoAng);
}

void btSliderConstraint_setSoftnessOrthoLin(btSliderConstraint* obj, btScalar softnessOrthoLin)
{
	obj->setSoftnessOrthoLin(softnessOrthoLin);
}

void btSliderConstraint_setTargetAngMotorVelocity(btSliderConstraint* obj, btScalar targetAngMotorVelocity)
{
	obj->setTargetAngMotorVelocity(targetAngMotorVelocity);
}

void btSliderConstraint_setTargetLinMotorVelocity(btSliderConstraint* obj, btScalar targetLinMotorVelocity)
{
	obj->setTargetLinMotorVelocity(targetLinMotorVelocity);
}

void btSliderConstraint_setUpperAngLimit(btSliderConstraint* obj, btScalar upperLimit)
{
	obj->setUpperAngLimit(upperLimit);
}

void btSliderConstraint_setUpperLinLimit(btSliderConstraint* obj, btScalar upperLimit)
{
	obj->setUpperLinLimit(upperLimit);
}

void btSliderConstraint_setUseFrameOffset(btSliderConstraint* obj, bool frameOffsetOnOff)
{
	obj->setUseFrameOffset(frameOffsetOnOff);
}

void btSliderConstraint_testAngLimits(btSliderConstraint* obj)
{
	obj->testAngLimits();
}

void btSliderConstraint_testLinLimits(btSliderConstraint* obj)
{
	obj->testLinLimits();
}
