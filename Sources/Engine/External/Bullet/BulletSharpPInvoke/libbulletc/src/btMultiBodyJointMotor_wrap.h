#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodyJointMotor* btMultiBodyJointMotor_new(btMultiBody* body, int link, btScalar desiredVelocity, btScalar maxMotorImpulse);
	EXPORT btMultiBodyJointMotor* btMultiBodyJointMotor_new2(btMultiBody* body, int link, int linkDoF, btScalar desiredVelocity, btScalar maxMotorImpulse);
	EXPORT void btMultiBodyJointMotor_setPositionTarget(btMultiBodyJointMotor* obj, btScalar posTarget);
	EXPORT void btMultiBodyJointMotor_setPositionTarget2(btMultiBodyJointMotor* obj, btScalar posTarget, btScalar kp);
	EXPORT void btMultiBodyJointMotor_setVelocityTarget(btMultiBodyJointMotor* obj, btScalar velTarget);
	EXPORT void btMultiBodyJointMotor_setVelocityTarget2(btMultiBodyJointMotor* obj, btScalar velTarget, btScalar kd);
#ifdef __cplusplus
}
#endif
