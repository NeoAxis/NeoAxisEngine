#include <BulletDynamics/Dynamics/btRigidBody.h>
#include <BulletDynamics/Featherstone/btMultiBodyPoint2Point.h>

#include "conversion.h"
#include "btMultiBodyPoint2Point_wrap.h"

btMultiBodyPoint2Point* btMultiBodyPoint2Point_new(btMultiBody* body, int link, btRigidBody* bodyB,
	const btVector3* pivotInA, const btVector3* pivotInB)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(pivotInB);
	return new btMultiBodyPoint2Point(body, link, bodyB, BTVECTOR3_USE(pivotInA),
		BTVECTOR3_USE(pivotInB));
}

btMultiBodyPoint2Point* btMultiBodyPoint2Point_new2(btMultiBody* bodyA, int linkA,
	btMultiBody* bodyB, int linkB, const btVector3* pivotInA, const btVector3* pivotInB)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(pivotInB);
	return new btMultiBodyPoint2Point(bodyA, linkA, bodyB, linkB, BTVECTOR3_USE(pivotInA),
		BTVECTOR3_USE(pivotInB));
}

void btMultiBodyPoint2Point_getPivotInB(btMultiBodyPoint2Point* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getPivotInB());
}

void btMultiBodyPoint2Point_setPivotInB(btMultiBodyPoint2Point* obj, const btVector3* pivotInB)
{
	BTVECTOR3_IN(pivotInB);
	obj->setPivotInB(BTVECTOR3_USE(pivotInB));
}
