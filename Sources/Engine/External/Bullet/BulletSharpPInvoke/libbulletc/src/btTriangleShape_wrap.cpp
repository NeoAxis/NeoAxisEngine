#include <BulletCollision/CollisionShapes/btTriangleShape.h>

#include "conversion.h"
#include "btTriangleShape_wrap.h"

btTriangleShape* btTriangleShape_new()
{
	return new btTriangleShape();
}

btTriangleShape* btTriangleShape_new2(const btVector3* p0, const btVector3* p1, const btVector3* p2)
{
	BTVECTOR3_IN(p0);
	BTVECTOR3_IN(p1);
	BTVECTOR3_IN(p2);
	return new btTriangleShape(BTVECTOR3_USE(p0), BTVECTOR3_USE(p1), BTVECTOR3_USE(p2));
}

void btTriangleShape_calcNormal(btTriangleShape* obj, btVector3* normal)
{
	BTVECTOR3_DEF(normal);
	obj->calcNormal(BTVECTOR3_USE(normal));
	BTVECTOR3_DEF_OUT(normal);
}

void btTriangleShape_getPlaneEquation(btTriangleShape* obj, int i, btVector3* planeNormal,
	btVector3* planeSupport)
{
	BTVECTOR3_DEF(planeNormal);
	BTVECTOR3_DEF(planeSupport);
	obj->getPlaneEquation(i, BTVECTOR3_USE(planeNormal), BTVECTOR3_USE(planeSupport));
	BTVECTOR3_DEF_OUT(planeNormal);
	BTVECTOR3_DEF_OUT(planeSupport);
}

const btScalar* btTriangleShape_getVertexPtr(btTriangleShape* obj, int index)
{
	return obj->getVertexPtr(index);
}

btVector3* btTriangleShape_getVertices1(btTriangleShape* obj)
{
	return obj->m_vertices1;
}
