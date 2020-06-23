#include <BulletDynamics/ConstraintSolver/btHinge2Constraint.h>

#include "conversion.h"
#include "btHinge2Constraint_wrap.h"

btHinge2Constraint* btHinge2Constraint_new(btRigidBody* rbA, btRigidBody* rbB, btVector3* anchor,
	btVector3* axis1, btVector3* axis2)
{
	BTVECTOR3_IN(anchor);
	BTVECTOR3_IN(axis1);
	BTVECTOR3_IN(axis2);
	return new btHinge2Constraint(*rbA, *rbB, BTVECTOR3_USE(anchor), BTVECTOR3_USE(axis1),
		BTVECTOR3_USE(axis2));
}

void btHinge2Constraint_getAnchor(btHinge2Constraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAnchor());
}

void btHinge2Constraint_getAnchor2(btHinge2Constraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAnchor2());
}

btScalar btHinge2Constraint_getAngle1(btHinge2Constraint* obj)
{
	return obj->getAngle1();
}

btScalar btHinge2Constraint_getAngle2(btHinge2Constraint* obj)
{
	return obj->getAngle2();
}

void btHinge2Constraint_getAxis1(btHinge2Constraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxis1());
}

void btHinge2Constraint_getAxis2(btHinge2Constraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAxis2());
}

void btHinge2Constraint_setLowerLimit(btHinge2Constraint* obj, btScalar ang1min)
{
	obj->setLowerLimit(ang1min);
}

void btHinge2Constraint_setUpperLimit(btHinge2Constraint* obj, btScalar ang1max)
{
	obj->setUpperLimit(ang1max);
}
