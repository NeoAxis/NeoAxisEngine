#include <BulletCollision/CollisionShapes/btConvex2dShape.h>

#include "btConvex2dShape_wrap.h"

btConvex2dShape* btConvex2dShape_new(btConvexShape* convexChildShape)
{
	return new btConvex2dShape(convexChildShape);
}

btConvexShape* btConvex2dShape_getChildShape(btConvex2dShape* obj)
{
	return obj->getChildShape();
}
