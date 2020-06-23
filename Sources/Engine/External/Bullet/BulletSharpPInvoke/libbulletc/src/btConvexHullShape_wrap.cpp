#include <BulletCollision/CollisionShapes/btConvexHullShape.h>

#include "conversion.h"
#include "btConvexHullShape_wrap.h"

btConvexHullShape* btConvexHullShape_new()
{
	return new btConvexHullShape();
}

btConvexHullShape* btConvexHullShape_new2(const btScalar* points)
{
	return new btConvexHullShape(points);
}

btConvexHullShape* btConvexHullShape_new3(const btScalar* points, int numPoints)
{
	return new btConvexHullShape(points, numPoints);
}

btConvexHullShape* btConvexHullShape_new4(const btScalar* points, int numPoints,
	int stride)
{
	return new btConvexHullShape(points, numPoints, stride);
}

void btConvexHullShape_addPoint(btConvexHullShape* obj, const btVector3* point,
	bool recalculateLocalAabb)
{
	BTVECTOR3_IN(point);
	obj->addPoint(BTVECTOR3_USE(point), recalculateLocalAabb);
}

int btConvexHullShape_getNumPoints(btConvexHullShape* obj)
{
	return obj->getNumPoints();
}

void btConvexHullShape_getScaledPoint(btConvexHullShape* obj, int i, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getScaledPoint(i);
	BTVECTOR3_SET(value, temp);
}

btVector3* btConvexHullShape_getUnscaledPoints(btConvexHullShape* obj)
{
	return obj->getUnscaledPoints();
}

void btConvexHullShape_optimizeConvexHull(btConvexHullShape* obj)
{
	obj->optimizeConvexHull();
}
