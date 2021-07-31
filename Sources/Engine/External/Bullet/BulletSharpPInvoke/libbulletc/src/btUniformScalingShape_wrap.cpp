#include <BulletCollision/CollisionShapes/btUniformScalingShape.h>

#include "btUniformScalingShape_wrap.h"

btUniformScalingShape* btUniformScalingShape_new(btConvexShape* convexChildShape,
	btScalar uniformScalingFactor)
{
	return new btUniformScalingShape(convexChildShape, uniformScalingFactor);
}

btConvexShape* btUniformScalingShape_getChildShape(btUniformScalingShape* obj)
{
	return obj->getChildShape();
}

btScalar btUniformScalingShape_getUniformScalingFactor(btUniformScalingShape* obj)
{
	return obj->getUniformScalingFactor();
}
