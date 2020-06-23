#include <BulletCollision/CollisionShapes/btConvexPointCloudShape.h>

#include "conversion.h"
#include "btConvexPointCloudShape_wrap.h"

btConvexPointCloudShape* btConvexPointCloudShape_new()
{
	return new btConvexPointCloudShape();
}

btConvexPointCloudShape* btConvexPointCloudShape_new2(btVector3* points, int numPoints,
	const btVector3* localScaling, bool computeAabb)
{
	BTVECTOR3_IN(localScaling);
	return new btConvexPointCloudShape(points, numPoints, BTVECTOR3_USE(localScaling),
		computeAabb);
}

int btConvexPointCloudShape_getNumPoints(btConvexPointCloudShape* obj)
{
	return obj->getNumPoints();
}

void btConvexPointCloudShape_getScaledPoint(btConvexPointCloudShape* obj, int index,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getScaledPoint(index);
	BTVECTOR3_SET(value, temp);
}

btVector3* btConvexPointCloudShape_getUnscaledPoints(btConvexPointCloudShape* obj)
{
	return obj->getUnscaledPoints();
}

void btConvexPointCloudShape_setPoints(btConvexPointCloudShape* obj, btVector3* points,
	int numPoints)
{
	obj->setPoints(points, numPoints);
}

void btConvexPointCloudShape_setPoints(btConvexPointCloudShape* obj, btVector3* points,
	int numPoints, bool computeAabb)
{
	obj->setPoints(points, numPoints, computeAabb);
}

void btConvexPointCloudShape_setPoints2(btConvexPointCloudShape* obj, btVector3* points,
	int numPoints, bool computeAabb, const btVector3* localScaling)
{
	BTVECTOR3_IN(localScaling);
	obj->setPoints(points, numPoints, computeAabb, BTVECTOR3_USE(localScaling));
}
