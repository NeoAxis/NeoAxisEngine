#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConeTwistConstraint* btConeTwistConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame);
	EXPORT btConeTwistConstraint* btConeTwistConstraint_new2(btRigidBody* rbA, const btTransform* rbAFrame);
	EXPORT void btConeTwistConstraint_calcAngleInfo(btConeTwistConstraint* obj);
	EXPORT void btConeTwistConstraint_calcAngleInfo2(btConeTwistConstraint* obj, const btTransform* transA, const btTransform* transB, const btMatrix3x3* invInertiaWorldA, const btMatrix3x3* invInertiaWorldB);
	EXPORT void btConeTwistConstraint_enableMotor(btConeTwistConstraint* obj, bool b);
	EXPORT void btConeTwistConstraint_getAFrame(btConeTwistConstraint* obj, btTransform* value);
	EXPORT bool btConeTwistConstraint_getAngularOnly(btConeTwistConstraint* obj);
	EXPORT void btConeTwistConstraint_getBFrame(btConeTwistConstraint* obj, btTransform* value);
	EXPORT btScalar btConeTwistConstraint_getBiasFactor(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getDamping(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getFixThresh(btConeTwistConstraint* obj);
	EXPORT int btConeTwistConstraint_getFlags(btConeTwistConstraint* obj);
	EXPORT void btConeTwistConstraint_getFrameOffsetA(btConeTwistConstraint* obj, btTransform* value);
	EXPORT void btConeTwistConstraint_getFrameOffsetB(btConeTwistConstraint* obj, btTransform* value);
	EXPORT void btConeTwistConstraint_getInfo1NonVirtual(btConeTwistConstraint* obj, btTypedConstraint_btConstraintInfo1* info);
	EXPORT void btConeTwistConstraint_getInfo2NonVirtual(btConeTwistConstraint* obj, btTypedConstraint_btConstraintInfo2* info, const btTransform* transA, const btTransform* transB, const btMatrix3x3* invInertiaWorldA, const btMatrix3x3* invInertiaWorldB);
	EXPORT btScalar btConeTwistConstraint_getLimit(btConeTwistConstraint* obj, int limitIndex);
	EXPORT btScalar btConeTwistConstraint_getLimitSoftness(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getMaxMotorImpulse(btConeTwistConstraint* obj);
	EXPORT void btConeTwistConstraint_getMotorTarget(btConeTwistConstraint* obj, btQuaternion* value);
	EXPORT void btConeTwistConstraint_GetPointForAngle(btConeTwistConstraint* obj, btScalar fAngleInRadians, btScalar fLength, btVector3* value);
	EXPORT btScalar btConeTwistConstraint_getRelaxationFactor(btConeTwistConstraint* obj);
	EXPORT int btConeTwistConstraint_getSolveSwingLimit(btConeTwistConstraint* obj);
	EXPORT int btConeTwistConstraint_getSolveTwistLimit(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getSwingSpan1(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getSwingSpan2(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getTwistAngle(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getTwistLimitSign(btConeTwistConstraint* obj);
	EXPORT btScalar btConeTwistConstraint_getTwistSpan(btConeTwistConstraint* obj);
	EXPORT bool btConeTwistConstraint_isMaxMotorImpulseNormalized(btConeTwistConstraint* obj);
	EXPORT bool btConeTwistConstraint_isMotorEnabled(btConeTwistConstraint* obj);
	EXPORT bool btConeTwistConstraint_isPastSwingLimit(btConeTwistConstraint* obj);
	EXPORT void btConeTwistConstraint_setAngularOnly(btConeTwistConstraint* obj, bool angularOnly);
	EXPORT void btConeTwistConstraint_setDamping(btConeTwistConstraint* obj, btScalar damping);
	EXPORT void btConeTwistConstraint_setFixThresh(btConeTwistConstraint* obj, btScalar fixThresh);
	EXPORT void btConeTwistConstraint_setFrames(btConeTwistConstraint* obj, const btTransform* frameA, const btTransform* frameB);
	EXPORT void btConeTwistConstraint_setLimit(btConeTwistConstraint* obj, int limitIndex, btScalar limitValue);
	EXPORT void btConeTwistConstraint_setLimit2(btConeTwistConstraint* obj, btScalar _swingSpan1, btScalar _swingSpan2, btScalar _twistSpan, btScalar _softness, btScalar _biasFactor, btScalar _relaxationFactor);
	EXPORT void btConeTwistConstraint_setMaxMotorImpulse(btConeTwistConstraint* obj, btScalar maxMotorImpulse);
	EXPORT void btConeTwistConstraint_setMaxMotorImpulseNormalized(btConeTwistConstraint* obj, btScalar maxMotorImpulse);
	EXPORT void btConeTwistConstraint_setMotorTarget(btConeTwistConstraint* obj, const btQuaternion* q);
	EXPORT void btConeTwistConstraint_setMotorTargetInConstraintSpace(btConeTwistConstraint* obj, const btQuaternion* q);
	EXPORT void btConeTwistConstraint_updateRHS(btConeTwistConstraint* obj, btScalar timeStep);
#ifdef __cplusplus
}
#endif
