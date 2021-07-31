#include <BulletDynamics/Featherstone/btMultiBodyJointLimitConstraint.h>

#include "btMultiBodyJointLimitConstraint_wrap.h"

btMultiBodyJointLimitConstraint* btMultiBodyJointLimitConstraint_new(btMultiBody* body,
	int link, btScalar lower, btScalar upper)
{
	return new btMultiBodyJointLimitConstraint(body, link, lower, upper);
}
