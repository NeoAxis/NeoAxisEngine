#include <BulletCollision/CollisionShapes/btCapsuleShape.h>

#include "btCapsuleShape_wrap.h"

btCapsuleShape* btCapsuleShape_new(btScalar radius, btScalar height)
{
	return new btCapsuleShape(radius, height);
}

void btCapsuleShape_deSerializeFloat(btCapsuleShape* obj, btCapsuleShapeData* dataBuffer)
{
	obj->deSerializeFloat(dataBuffer);
}

btScalar btCapsuleShape_getHalfHeight(btCapsuleShape* obj)
{
	return obj->getHalfHeight();
}

btScalar btCapsuleShape_getRadius(btCapsuleShape* obj)
{
	return obj->getRadius();
}

int btCapsuleShape_getUpAxis(btCapsuleShape* obj)
{
	return obj->getUpAxis();
}


btCapsuleShapeX* btCapsuleShapeX_new(btScalar radius, btScalar height)
{
	return new btCapsuleShapeX(radius, height);
}


btCapsuleShapeZ* btCapsuleShapeZ_new(btScalar radius, btScalar height)
{
	return new btCapsuleShapeZ(radius, height);
}
