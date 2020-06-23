#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btHingeConstraint* btHingeConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* pivotInA, const btVector3* pivotInB, const btVector3* axisInA, const btVector3* axisInB, bool useReferenceFrameA);
	EXPORT btHingeConstraint* btHingeConstraint_new2(btRigidBody* rbA, const btVector3* pivotInA, const btVector3* axisInA, bool useReferenceFrameA);
	EXPORT btHingeConstraint* btHingeConstraint_new3(btRigidBody* rbA, btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame, bool useReferenceFrameA);
	EXPORT btHingeConstraint* btHingeConstraint_new4(btRigidBody* rbA, const btTransform* rbAFrame, bool useReferenceFrameA);
	EXPORT void btHingeConstraint_enableAngularMotor(btHingeConstraint* obj, bool enableMotor, btScalar targetVelocity, btScalar maxMotorImpulse);
	EXPORT void btHingeConstraint_enableMotor(btHingeConstraint* obj, bool enableMotor);
	EXPORT void btHingeConstraint_getAFrame(btHingeConstraint* obj, btTransform* value);
	EXPORT bool btHingeConstraint_getAngularOnly(btHingeConstraint* obj);
	EXPORT void btHingeConstraint_getBFrame(btHingeConstraint* obj, btTransform* value);
	EXPORT bool btHingeConstraint_getEnableAngularMotor(btHingeConstraint* obj);
	EXPORT int btHingeConstraint_getFlags(btHingeConstraint* obj);
	EXPORT void btHingeConstraint_getFrameOffsetA(btHingeConstraint* obj, btTransform* value);
	EXPORT void btHingeConstraint_getFrameOffsetB(btHingeConstraint* obj, btTransform* value);
	EXPORT btScalar btHingeConstraint_getHingeAngle(btHingeConstraint* obj, const btTransform* transA, const btTransform* transB);
	EXPORT btScalar btHingeConstraint_getHingeAngle2(btHingeConstraint* obj);
	EXPORT void btHingeConstraint_getInfo1NonVirtual(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo1* info);
	EXPORT void btHingeConstraint_getInfo2Internal(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info, const btTransform* transA, const btTransform* transB, const btVector3* angVelA, const btVector3* angVelB);
	EXPORT void btHingeConstraint_getInfo2InternalUsingFrameOffset(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info, const btTransform* transA, const btTransform* transB, const btVector3* angVelA, const btVector3* angVelB);
	EXPORT void btHingeConstraint_getInfo2NonVirtual(btHingeConstraint* obj, btTypedConstraint_btConstraintInfo2* info, const btTransform* transA, const btTransform* transB, const btVector3* angVelA, const btVector3* angVelB);
	EXPORT btScalar btHingeConstraint_getLimitBiasFactor(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getLimitRelaxationFactor(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getLimitSign(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getLimitSoftness(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getLowerLimit(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getMaxMotorImpulse(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getMotorTargetVelocity(btHingeConstraint* obj);
	EXPORT int btHingeConstraint_getSolveLimit(btHingeConstraint* obj);
	EXPORT btScalar btHingeConstraint_getUpperLimit(btHingeConstraint* obj);
	EXPORT bool btHingeConstraint_getUseFrameOffset(btHingeConstraint* obj);
	EXPORT bool btHingeConstraint_getUseReferenceFrameA(btHingeConstraint* obj);
	EXPORT bool btHingeConstraint_hasLimit(btHingeConstraint* obj);
	EXPORT void btHingeConstraint_setAngularOnly(btHingeConstraint* obj, bool angularOnly);
	EXPORT void btHingeConstraint_setAxis(btHingeConstraint* obj, btVector3* axisInA);
	EXPORT void btHingeConstraint_setFrames(btHingeConstraint* obj, const btTransform* frameA, const btTransform* frameB);
	EXPORT void btHingeConstraint_setLimit(btHingeConstraint* obj, btScalar low, btScalar high);
	EXPORT void btHingeConstraint_setLimit2(btHingeConstraint* obj, btScalar low, btScalar high, btScalar _softness);
	EXPORT void btHingeConstraint_setLimit3(btHingeConstraint* obj, btScalar low, btScalar high, btScalar _softness, btScalar _biasFactor);
	EXPORT void btHingeConstraint_setLimit4(btHingeConstraint* obj, btScalar low, btScalar high, btScalar _softness, btScalar _biasFactor, btScalar _relaxationFactor);
	EXPORT void btHingeConstraint_setMaxMotorImpulse(btHingeConstraint* obj, btScalar maxMotorImpulse);
	EXPORT void btHingeConstraint_setMotorTarget(btHingeConstraint* obj, btScalar targetAngle, btScalar dt);
	EXPORT void btHingeConstraint_setMotorTarget2(btHingeConstraint* obj, const btQuaternion* qAinB, btScalar dt);
	EXPORT void btHingeConstraint_setMotorTargetVelocity(btHingeConstraint* obj, btScalar motorTargetVelocity);
	EXPORT void btHingeConstraint_setUseFrameOffset(btHingeConstraint* obj, bool frameOffsetOnOff);
	EXPORT void btHingeConstraint_setUseReferenceFrameA(btHingeConstraint* obj, bool useReferenceFrameA);
	EXPORT void btHingeConstraint_testLimit(btHingeConstraint* obj, const btTransform* transA, const btTransform* transB);
	EXPORT void btHingeConstraint_updateRHS(btHingeConstraint* obj, btScalar timeStep);

	EXPORT btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* pivotInA, const btVector3* pivotInB, const btVector3* axisInA, const btVector3* axisInB, bool useReferenceFrameA);
	EXPORT btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new2(btRigidBody* rbA, const btVector3* pivotInA, const btVector3* axisInA, bool useReferenceFrameA);
	EXPORT btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new3(btRigidBody* rbA, btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame, bool useReferenceFrameA);
	EXPORT btHingeAccumulatedAngleConstraint* btHingeAccumulatedAngleConstraint_new4(btRigidBody* rbA, const btTransform* rbAFrame, bool useReferenceFrameA);
	EXPORT btScalar btHingeAccumulatedAngleConstraint_getAccumulatedHingeAngle(btHingeAccumulatedAngleConstraint* obj);
	EXPORT void btHingeAccumulatedAngleConstraint_setAccumulatedHingeAngle(btHingeAccumulatedAngleConstraint* obj, btScalar accAngle);
#ifdef __cplusplus
}
#endif
