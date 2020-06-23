#include <BulletCollision/CollisionShapes/btConeShape.h>

#include "btConeShape_wrap.h"

btConeShape* btConeShape_new(btScalar radius, btScalar height)
{
	return new btConeShape(radius, height);
}

int btConeShape_getConeUpIndex(btConeShape* obj)
{
	return obj->getConeUpIndex();
}

btScalar btConeShape_getHeight(btConeShape* obj)
{
	return obj->getHeight();
}

btScalar btConeShape_getRadius(btConeShape* obj)
{
	return obj->getRadius();
}

void btConeShape_setConeUpIndex(btConeShape* obj, int upIndex)
{
	obj->setConeUpIndex(upIndex);
}

void btConeShape_setHeight(btConeShape* obj, btScalar height)
{
	obj->setHeight(height);
}

void btConeShape_setRadius(btConeShape* obj, btScalar radius)
{
	obj->setRadius(radius);
}


btConeShapeX* btConeShapeX_new(btScalar radius, btScalar height)
{
	return new btConeShapeX(radius, height);
}


btConeShapeZ* btConeShapeZ_new(btScalar radius, btScalar height)
{
	return new btConeShapeZ(radius, height);
}
