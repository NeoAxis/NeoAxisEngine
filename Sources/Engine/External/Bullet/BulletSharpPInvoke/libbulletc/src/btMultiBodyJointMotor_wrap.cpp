#include <BulletDynamics/Featherstone/btMultiBodyJointMotor.h>

#include "btMultiBodyJointMotor_wrap.h"

btMultiBodyJointMotor* btMultiBodyJointMotor_new(btMultiBody* body, int link, btScalar desiredVelocity,
	btScalar maxMotorImpulse)
{
	return new btMultiBodyJointMotor(body, link, desiredVelocity, maxMotorImpulse);
}

btMultiBodyJointMotor* btMultiBodyJointMotor_new2(btMultiBody* body, int link, int linkDoF,
	btScalar desiredVelocity, btScalar maxMotorImpulse)
{
	return new btMultiBodyJointMotor(body, link, linkDoF, desiredVelocity, maxMotorImpulse);
}

void btMultiBodyJointMotor_setPositionTarget(btMultiBodyJointMotor* obj, btScalar posTarget)
{
	obj->setPositionTarget(posTarget);
}

void btMultiBodyJointMotor_setPositionTarget2(btMultiBodyJointMotor* obj, btScalar posTarget,
	btScalar kp)
{
	obj->setPositionTarget(posTarget, kp);
}

void btMultiBodyJointMotor_setVelocityTarget(btMultiBodyJointMotor* obj, btScalar velTarget)
{
	obj->setVelocityTarget(velTarget);
}

void btMultiBodyJointMotor_setVelocityTarget2(btMultiBodyJointMotor* obj, btScalar velTarget,
	btScalar kd)
{
	obj->setVelocityTarget(velTarget, kd);
}
