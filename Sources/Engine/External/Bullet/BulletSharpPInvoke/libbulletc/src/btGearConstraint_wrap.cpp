#include <BulletDynamics/ConstraintSolver/btGearConstraint.h>

#include "conversion.h"
#include "btGearConstraint_wrap.h"

btGearConstraint* btGearConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* axisInA,
	const btVector3* axisInB, btScalar ratio)
{
	BTVECTOR3_IN(axisInA);
	BTVECTOR3_IN(axisInB);
	return new btGearConstraint(*rbA, *rbB, BTVECTOR3_USE(axisInA), BTVECTOR3_USE(axisInB),
		ratio);
}

void btGearConstraint_getAxisA(btGearConstraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxisA());
}

void btGearConstraint_getAxisB(btGearConstraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxisB());
}

btScalar btGearConstraint_getRatio(btGearConstraint* obj)
{
	return obj->getRatio();
}

void btGearConstraint_setAxisA(btGearConstraint* obj, btVector3* axisA)
{
	BTVECTOR3_IN(axisA);
	obj->setAxisA(BTVECTOR3_USE(axisA));
}

void btGearConstraint_setAxisB(btGearConstraint* obj, btVector3* axisB)
{
	BTVECTOR3_IN(axisB);
	obj->setAxisB(BTVECTOR3_USE(axisB));
}

void btGearConstraint_setRatio(btGearConstraint* obj, btScalar ratio)
{
	obj->setRatio(ratio);
}
