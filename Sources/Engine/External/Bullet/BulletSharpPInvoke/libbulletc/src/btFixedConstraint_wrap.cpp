#include <BulletDynamics/ConstraintSolver/btFixedConstraint.h>

#include "conversion.h"
#include "btFixedConstraint_wrap.h"

btFixedConstraint* btFixedConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA,
	const btTransform* frameInB)
{
	BTTRANSFORM_IN(frameInA);
	BTTRANSFORM_IN(frameInB);
	return new btFixedConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA), BTTRANSFORM_USE(frameInB));
}
