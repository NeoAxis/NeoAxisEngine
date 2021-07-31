#include <BulletCollision/CollisionShapes/btSphereShape.h>

#include "btSphereShape_wrap.h"

btSphereShape* btSphereShape_new(btScalar radius)
{
	return new btSphereShape(radius);
}

btScalar btSphereShape_getRadius(btSphereShape* obj)
{
	return obj->getRadius();
}

void btSphereShape_setUnscaledRadius(btSphereShape* obj, btScalar radius)
{
	obj->setUnscaledRadius(radius);
}
