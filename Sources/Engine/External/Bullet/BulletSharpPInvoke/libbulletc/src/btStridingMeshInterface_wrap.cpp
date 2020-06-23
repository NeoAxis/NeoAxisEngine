#include <BulletCollision/CollisionShapes/btStridingMeshInterface.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btStridingMeshInterface_wrap.h"

void btStridingMeshInterface_calculateAabbBruteForce(btStridingMeshInterface* obj,
	btVector3* aabbMin, btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->calculateAabbBruteForce(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

int btStridingMeshInterface_calculateSerializeBufferSize(btStridingMeshInterface* obj)
{
	return obj->calculateSerializeBufferSize();
}

void btStridingMeshInterface_getLockedReadOnlyVertexIndexBase(btStridingMeshInterface* obj,
	const unsigned char** vertexbase, int* numverts, PHY_ScalarType* type, int* vertexStride,
	const unsigned char** indexbase, int* indexstride, int* numfaces, PHY_ScalarType* indicestype,
	int subpart)
{
	obj->getLockedReadOnlyVertexIndexBase(vertexbase, *numverts, *type, *vertexStride,
		indexbase, *indexstride, *numfaces, *indicestype, subpart);
}

void btStridingMeshInterface_getLockedVertexIndexBase(btStridingMeshInterface* obj,
	unsigned char** vertexbase, int* numverts, PHY_ScalarType* type, int* vertexStride,
	unsigned char** indexbase, int* indexstride, int* numfaces, PHY_ScalarType* indicestype,
	int subpart)
{
	obj->getLockedVertexIndexBase(vertexbase, *numverts, *type, *vertexStride, indexbase,
		*indexstride, *numfaces, *indicestype, subpart);
}

int btStridingMeshInterface_getNumSubParts(btStridingMeshInterface* obj)
{
	return obj->getNumSubParts();
}

void btStridingMeshInterface_getPremadeAabb(btStridingMeshInterface* obj, btVector3* aabbMin,
	btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getPremadeAabb(&BTVECTOR3_USE(aabbMin), &BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

void btStridingMeshInterface_getScaling(btStridingMeshInterface* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getScaling());
}

bool btStridingMeshInterface_hasPremadeAabb(btStridingMeshInterface* obj)
{
	return obj->hasPremadeAabb();
}

void btStridingMeshInterface_InternalProcessAllTriangles(btStridingMeshInterface* obj,
	btInternalTriangleIndexCallback* callback, const btVector3* aabbMin, const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->InternalProcessAllTriangles(callback, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

void btStridingMeshInterface_preallocateIndices(btStridingMeshInterface* obj, int numindices)
{
	obj->preallocateIndices(numindices);
}

void btStridingMeshInterface_preallocateVertices(btStridingMeshInterface* obj, int numverts)
{
	obj->preallocateVertices(numverts);
}

const char* btStridingMeshInterface_serialize(btStridingMeshInterface* obj, void* dataBuffer,
	btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btStridingMeshInterface_setPremadeAabb(btStridingMeshInterface* obj, const btVector3* aabbMin,
	const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->setPremadeAabb(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

void btStridingMeshInterface_setScaling(btStridingMeshInterface* obj, const btVector3* scaling)
{
	BTVECTOR3_IN(scaling);
	obj->setScaling(BTVECTOR3_USE(scaling));
}

void btStridingMeshInterface_unLockReadOnlyVertexBase(btStridingMeshInterface* obj,
	int subpart)
{
	obj->unLockReadOnlyVertexBase(subpart);
}

void btStridingMeshInterface_unLockVertexBase(btStridingMeshInterface* obj, int subpart)
{
	obj->unLockVertexBase(subpart);
}

void btStridingMeshInterface_delete(btStridingMeshInterface* obj)
{
	delete obj;
}
