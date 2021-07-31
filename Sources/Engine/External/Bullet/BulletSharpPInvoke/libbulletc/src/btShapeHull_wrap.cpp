#include <BulletCollision/CollisionShapes/btShapeHull.h>

#include "btShapeHull_wrap.h"

btShapeHull* btShapeHull_new(const btConvexShape* shape)
{
	return ALIGNED_NEW(btShapeHull)(shape);
}

bool btShapeHull_buildHull(btShapeHull* obj, btScalar margin)
{
	return obj->buildHull(margin);
}

const unsigned int* btShapeHull_getIndexPointer(btShapeHull* obj)
{
	return obj->getIndexPointer();
}

const btVector3* btShapeHull_getVertexPointer(btShapeHull* obj)
{
	return obj->getVertexPointer();
}

int btShapeHull_numIndices(btShapeHull* obj)
{
	return obj->numIndices();
}

int btShapeHull_numTriangles(btShapeHull* obj)
{
	return obj->numTriangles();
}

int btShapeHull_numVertices(btShapeHull* obj)
{
	return obj->numVertices();
}

void btShapeHull_delete(btShapeHull* obj)
{
	ALIGNED_FREE(obj);
}
