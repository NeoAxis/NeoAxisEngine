#include <BulletCollision/CollisionShapes/btBoxShape.h>

#include "conversion.h"
#include "btBoxShape_wrap.h"

btBoxShape* btBoxShape_new(const btVector3* boxHalfExtents)
{
	BTVECTOR3_IN(boxHalfExtents);
	return new btBoxShape(BTVECTOR3_USE(boxHalfExtents));
}

btBoxShape* btBoxShape_new2(btScalar boxHalfExtent)
{
	return new btBoxShape(btVector3(boxHalfExtent, boxHalfExtent, boxHalfExtent));
}

btBoxShape* btBoxShape_new3(btScalar boxHalfExtentX, btScalar boxHalfExtentY, btScalar boxHalfExtentZ)
{
	return new btBoxShape(btVector3(boxHalfExtentX, boxHalfExtentY, boxHalfExtentZ));
}

void btBoxShape_getHalfExtentsWithMargin(btBoxShape* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getHalfExtentsWithMargin();
	BTVECTOR3_SET(value, temp);
}

void btBoxShape_getHalfExtentsWithoutMargin(btBoxShape* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getHalfExtentsWithoutMargin());
}

void btBoxShape_getPlaneEquation(btBoxShape* obj, btVector4* plane, int i)
{
	BTVECTOR4_DEF(plane);
	obj->getPlaneEquation(BTVECTOR4_USE(plane), i);
	BTVECTOR4_DEF_OUT(plane);
}
