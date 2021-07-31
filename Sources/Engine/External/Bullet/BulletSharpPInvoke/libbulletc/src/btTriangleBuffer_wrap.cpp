#include <BulletCollision/CollisionShapes/btTriangleBuffer.h>

#include "conversion.h"
#include "btTriangleBuffer_wrap.h"

btTriangle* btTriangle_new()
{
	return new btTriangle();
}

int btTriangle_getPartId(btTriangle* obj)
{
	return obj->m_partId;
}

int btTriangle_getTriangleIndex(btTriangle* obj)
{
	return obj->m_triangleIndex;
}

void btTriangle_getVertex0(btTriangle* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_vertex0);
}

void btTriangle_getVertex1(btTriangle* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_vertex1);
}

void btTriangle_getVertex2(btTriangle* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_vertex2);
}

void btTriangle_setPartId(btTriangle* obj, int value)
{
	obj->m_partId = value;
}

void btTriangle_setTriangleIndex(btTriangle* obj, int value)
{
	obj->m_triangleIndex = value;
}

void btTriangle_setVertex0(btTriangle* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_vertex0, value);
}

void btTriangle_setVertex1(btTriangle* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_vertex1, value);
}

void btTriangle_setVertex2(btTriangle* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_vertex2, value);
}

void btTriangle_delete(btTriangle* obj)
{
	delete obj;
}


btTriangleBuffer* btTriangleBuffer_new()
{
	return new btTriangleBuffer();
}

void btTriangleBuffer_clearBuffer(btTriangleBuffer* obj)
{
	obj->clearBuffer();
}

int btTriangleBuffer_getNumTriangles(btTriangleBuffer* obj)
{
	return obj->getNumTriangles();
}

const btTriangle* btTriangleBuffer_getTriangle(btTriangleBuffer* obj, int index)
{
	return &obj->getTriangle(index);
}
