#include <BulletCollision/CollisionShapes/btTriangleIndexVertexMaterialArray.h>

#include "btTriangleIndexVertexMaterialArray_wrap.h"

btMaterialProperties* btMaterialProperties_new()
{
	return new btMaterialProperties();
}

const unsigned char* btMaterialProperties_getMaterialBase(btMaterialProperties* obj)
{
	return obj->m_materialBase;
}

int btMaterialProperties_getMaterialStride(btMaterialProperties* obj)
{
	return obj->m_materialStride;
}

PHY_ScalarType btMaterialProperties_getMaterialType(btMaterialProperties* obj)
{
	return obj->m_materialType;
}

int btMaterialProperties_getNumMaterials(btMaterialProperties* obj)
{
	return obj->m_numMaterials;
}

int btMaterialProperties_getNumTriangles(btMaterialProperties* obj)
{
	return obj->m_numTriangles;
}

const unsigned char* btMaterialProperties_getTriangleMaterialsBase(btMaterialProperties* obj)
{
	return obj->m_triangleMaterialsBase;
}

int btMaterialProperties_getTriangleMaterialStride(btMaterialProperties* obj)
{
	return obj->m_triangleMaterialStride;
}

PHY_ScalarType btMaterialProperties_getTriangleType(btMaterialProperties* obj)
{
	return obj->m_triangleType;
}

void btMaterialProperties_setMaterialBase(btMaterialProperties* obj, const unsigned char* value)
{
	obj->m_materialBase = value;
}

void btMaterialProperties_setMaterialStride(btMaterialProperties* obj, int value)
{
	obj->m_materialStride = value;
}

void btMaterialProperties_setMaterialType(btMaterialProperties* obj, PHY_ScalarType value)
{
	obj->m_materialType = value;
}

void btMaterialProperties_setNumMaterials(btMaterialProperties* obj, int value)
{
	obj->m_numMaterials = value;
}

void btMaterialProperties_setNumTriangles(btMaterialProperties* obj, int value)
{
	obj->m_numTriangles = value;
}

void btMaterialProperties_setTriangleMaterialsBase(btMaterialProperties* obj, const unsigned char* value)
{
	obj->m_triangleMaterialsBase = value;
}

void btMaterialProperties_setTriangleMaterialStride(btMaterialProperties* obj, int value)
{
	obj->m_triangleMaterialStride = value;
}

void btMaterialProperties_setTriangleType(btMaterialProperties* obj, PHY_ScalarType value)
{
	obj->m_triangleType = value;
}

void btMaterialProperties_delete(btMaterialProperties* obj)
{
	delete obj;
}


btTriangleIndexVertexMaterialArray* btTriangleIndexVertexMaterialArray_new()
{
	return new btTriangleIndexVertexMaterialArray();
}

btTriangleIndexVertexMaterialArray* btTriangleIndexVertexMaterialArray_new2(int numTriangles,
	int* triangleIndexBase, int triangleIndexStride, int numVertices, btScalar* vertexBase,
	int vertexStride, int numMaterials, unsigned char* materialBase, int materialStride,
	int* triangleMaterialsBase, int materialIndexStride)
{
	return new btTriangleIndexVertexMaterialArray(numTriangles, triangleIndexBase,
		triangleIndexStride, numVertices, vertexBase, vertexStride, numMaterials, materialBase,
		materialStride, triangleMaterialsBase, materialIndexStride);
}

void btTriangleIndexVertexMaterialArray_addMaterialProperties(btTriangleIndexVertexMaterialArray* obj,
	const btMaterialProperties* mat, PHY_ScalarType triangleType)
{
	obj->addMaterialProperties(*mat, triangleType);
}

void btTriangleIndexVertexMaterialArray_getLockedMaterialBase(btTriangleIndexVertexMaterialArray* obj,
	unsigned char** materialBase, int* numMaterials, PHY_ScalarType* materialType,
	int* materialStride, unsigned char** triangleMaterialBase, int* numTriangles, int* triangleMaterialStride,
	PHY_ScalarType* triangleType, int subpart)
{
	obj->getLockedMaterialBase(materialBase, *numMaterials, *materialType, *materialStride,
		triangleMaterialBase, *numTriangles, *triangleMaterialStride, *triangleType,
		subpart);
}

void btTriangleIndexVertexMaterialArray_getLockedReadOnlyMaterialBase(btTriangleIndexVertexMaterialArray* obj,
	const unsigned char** materialBase, int* numMaterials, PHY_ScalarType* materialType,
	int* materialStride, const unsigned char** triangleMaterialBase, int* numTriangles,
	int* triangleMaterialStride, PHY_ScalarType* triangleType, int subpart)
{
	obj->getLockedReadOnlyMaterialBase(materialBase, *numMaterials, *materialType,
		*materialStride, triangleMaterialBase, *numTriangles, *triangleMaterialStride,
		*triangleType, subpart);
}
