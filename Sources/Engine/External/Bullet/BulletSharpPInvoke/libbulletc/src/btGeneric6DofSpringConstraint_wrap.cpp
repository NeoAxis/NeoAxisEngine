#include <BulletDynamics/ConstraintSolver/btGeneric6DofSpringConstraint.h>

#include "conversion.h"
#include "btGeneric6DofSpringConstraint_wrap.h"

btGeneric6DofSpringConstraint* btGeneric6DofSpringConstraint_new(btRigidBody* rbA,
	btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB, bool useLinearReferenceFrameA)
{
	BTTRANSFORM_IN(frameInA);
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofSpringConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA),
		BTTRANSFORM_USE(frameInB), useLinearReferenceFrameA);
}

btGeneric6DofSpringConstraint* btGeneric6DofSpringConstraint_new2(btRigidBody* rbB,
	const btTransform* frameInB, bool useLinearReferenceFrameB)
{
	BTTRANSFORM_IN(frameInB);
	return new btGeneric6DofSpringConstraint(*rbB, BTTRANSFORM_USE(frameInB), useLinearReferenceFrameB);
}

void btGeneric6DofSpringConstraint_enableSpring(btGeneric6DofSpringConstraint* obj,
	int index, bool onOff)
{
	obj->enableSpring(index, onOff);
}

btScalar btGeneric6DofSpringConstraint_getDamping(btGeneric6DofSpringConstraint* obj,
	int index)
{
	return obj->getDamping(index);
}

btScalar btGeneric6DofSpringConstraint_getEquilibriumPoint(btGeneric6DofSpringConstraint* obj,
	int index)
{
	return obj->getEquilibriumPoint(index);
}

btScalar btGeneric6DofSpringConstraint_getStiffness(btGeneric6DofSpringConstraint* obj,
	int index)
{
	return obj->getStiffness(index);
}

bool btGeneric6DofSpringConstraint_isSpringEnabled(btGeneric6DofSpringConstraint* obj,
	int index)
{
	return obj->isSpringEnabled(index);
}

void btGeneric6DofSpringConstraint_setDamping(btGeneric6DofSpringConstraint* obj,
	int index, btScalar damping)
{
	obj->setDamping(index, damping);
}

void btGeneric6DofSpringConstraint_setEquilibriumPoint(btGeneric6DofSpringConstraint* obj)
{
	obj->setEquilibriumPoint();
}

void btGeneric6DofSpringConstraint_setEquilibriumPoint2(btGeneric6DofSpringConstraint* obj,
	int index)
{
	obj->setEquilibriumPoint(index);
}

void btGeneric6DofSpringConstraint_setEquilibriumPoint3(btGeneric6DofSpringConstraint* obj,
	int index, btScalar val)
{
	obj->setEquilibriumPoint(index, val);
}

void btGeneric6DofSpringConstraint_setStiffness(btGeneric6DofSpringConstraint* obj,
	int index, btScalar stiffness)
{
	obj->setStiffness(index, stiffness);
}
