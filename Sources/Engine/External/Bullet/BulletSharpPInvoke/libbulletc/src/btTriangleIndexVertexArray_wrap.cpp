#include <BulletCollision/CollisionShapes/btTriangleIndexVertexArray.h>

#include "btTriangleIndexVertexArray_wrap.h"

btIndexedMesh* btIndexedMesh_new()
{
	return new btIndexedMesh();
}

PHY_ScalarType btIndexedMesh_getIndexType(btIndexedMesh* obj)
{
	return obj->m_indexType;
}

int btIndexedMesh_getNumTriangles(btIndexedMesh* obj)
{
	return obj->m_numTriangles;
}

int btIndexedMesh_getNumVertices(btIndexedMesh* obj)
{
	return obj->m_numVertices;
}

const unsigned char* btIndexedMesh_getTriangleIndexBase(btIndexedMesh* obj)
{
	return obj->m_triangleIndexBase;
}

int btIndexedMesh_getTriangleIndexStride(btIndexedMesh* obj)
{
	return obj->m_triangleIndexStride;
}

const unsigned char* btIndexedMesh_getVertexBase(btIndexedMesh* obj)
{
	return obj->m_vertexBase;
}

int btIndexedMesh_getVertexStride(btIndexedMesh* obj)
{
	return obj->m_vertexStride;
}

PHY_ScalarType btIndexedMesh_getVertexType(btIndexedMesh* obj)
{
	return obj->m_vertexType;
}

void btIndexedMesh_setIndexType(btIndexedMesh* obj, PHY_ScalarType value)
{
	obj->m_indexType = value;
}

void btIndexedMesh_setNumTriangles(btIndexedMesh* obj, int value)
{
	obj->m_numTriangles = value;
}

void btIndexedMesh_setNumVertices(btIndexedMesh* obj, int value)
{
	obj->m_numVertices = value;
}

void btIndexedMesh_setTriangleIndexBase(btIndexedMesh* obj, const unsigned char* value)
{
	obj->m_triangleIndexBase = value;
}

void btIndexedMesh_setTriangleIndexStride(btIndexedMesh* obj, int value)
{
	obj->m_triangleIndexStride = value;
}

void btIndexedMesh_setVertexBase(btIndexedMesh* obj, const unsigned char* value)
{
	obj->m_vertexBase = value;
}

void btIndexedMesh_setVertexStride(btIndexedMesh* obj, int value)
{
	obj->m_vertexStride = value;
}

void btIndexedMesh_setVertexType(btIndexedMesh* obj, PHY_ScalarType value)
{
	obj->m_vertexType = value;
}

void btIndexedMesh_delete(btIndexedMesh* obj)
{
	delete obj;
}


btTriangleIndexVertexArray* btTriangleIndexVertexArray_new()
{
	return new btTriangleIndexVertexArray();
}

btTriangleIndexVertexArray* btTriangleIndexVertexArray_new2(int numTriangles, int* triangleIndexBase,
	int triangleIndexStride, int numVertices, btScalar* vertexBase, int vertexStride)
{
	return new btTriangleIndexVertexArray(numTriangles, triangleIndexBase, triangleIndexStride,
		numVertices, vertexBase, vertexStride);
}

void btTriangleIndexVertexArray_addIndexedMesh(btTriangleIndexVertexArray* obj,
	const btIndexedMesh* mesh, PHY_ScalarType indexType)
{
	obj->addIndexedMesh(*mesh, indexType);
}

btAlignedObjectArray_btIndexedMesh* btTriangleIndexVertexArray_getIndexedMeshArray(
	btTriangleIndexVertexArray* obj)
{
	return &obj->getIndexedMeshArray();
}
