#include <BulletCollision/CollisionShapes/btCylinderShape.h>

#include "conversion.h"
#include "btCylinderShape_wrap.h"

btCylinderShape* btCylinderShape_new(const btVector3* halfExtents)
{
	BTVECTOR3_IN(halfExtents);
	return new btCylinderShape(BTVECTOR3_USE(halfExtents));
}

btCylinderShape* btCylinderShape_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ)
{
	return new btCylinderShape(btVector3(halfExtentX, halfExtentY, halfExtentZ));
}

void btCylinderShape_getHalfExtentsWithMargin(btCylinderShape* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getHalfExtentsWithMargin();
	BTVECTOR3_SET(value, temp);
}

void btCylinderShape_getHalfExtentsWithoutMargin(btCylinderShape* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getHalfExtentsWithoutMargin());
}

btScalar btCylinderShape_getRadius(btCylinderShape* obj)
{
	return obj->getRadius();
}

int btCylinderShape_getUpAxis(btCylinderShape* obj)
{
	return obj->getUpAxis();
}


btCylinderShapeX* btCylinderShapeX_new(const btVector3* halfExtents)
{
	BTVECTOR3_IN(halfExtents);
	return new btCylinderShapeX(BTVECTOR3_USE(halfExtents));
}

btCylinderShapeX* btCylinderShapeX_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ)
{
	return new btCylinderShapeX(btVector3(halfExtentX, halfExtentY, halfExtentZ));
}


btCylinderShapeZ* btCylinderShapeZ_new(const btVector3* halfExtents)
{
	BTVECTOR3_IN(halfExtents);
	return new btCylinderShapeZ(BTVECTOR3_USE(halfExtents));
}

btCylinderShapeZ* btCylinderShapeZ_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ)
{
	return new btCylinderShapeZ(btVector3(halfExtentX, halfExtentY, halfExtentZ));
}
