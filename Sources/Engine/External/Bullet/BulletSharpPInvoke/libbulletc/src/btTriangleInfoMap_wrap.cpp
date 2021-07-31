#include <BulletCollision/CollisionShapes/btTriangleInfoMap.h>

#include "btTriangleInfoMap_wrap.h"

btTriangleInfo* btTriangleInfo_new()
{
	return new btTriangleInfo();
}

btScalar btTriangleInfo_getEdgeV0V1Angle(btTriangleInfo* obj)
{
	return obj->m_edgeV0V1Angle;
}

btScalar btTriangleInfo_getEdgeV1V2Angle(btTriangleInfo* obj)
{
	return obj->m_edgeV1V2Angle;
}

btScalar btTriangleInfo_getEdgeV2V0Angle(btTriangleInfo* obj)
{
	return obj->m_edgeV2V0Angle;
}

int btTriangleInfo_getFlags(btTriangleInfo* obj)
{
	return obj->m_flags;
}

void btTriangleInfo_setEdgeV0V1Angle(btTriangleInfo* obj, btScalar value)
{
	obj->m_edgeV0V1Angle = value;
}

void btTriangleInfo_setEdgeV1V2Angle(btTriangleInfo* obj, btScalar value)
{
	obj->m_edgeV1V2Angle = value;
}

void btTriangleInfo_setEdgeV2V0Angle(btTriangleInfo* obj, btScalar value)
{
	obj->m_edgeV2V0Angle = value;
}

void btTriangleInfo_setFlags(btTriangleInfo* obj, int value)
{
	obj->m_flags = value;
}

void btTriangleInfo_delete(btTriangleInfo* obj)
{
	delete obj;
}


btTriangleInfoMap* btTriangleInfoMap_new()
{
	return new btTriangleInfoMap();
}

int btTriangleInfoMap_calculateSerializeBufferSize(btTriangleInfoMap* obj)
{
	return obj->calculateSerializeBufferSize();
}
/*
void btTriangleInfoMap_deSerialize(btTriangleInfoMap* obj, btTriangleInfoMapData* data)
{
	obj->deSerialize(*data);
}
*/
btScalar btTriangleInfoMap_getConvexEpsilon(btTriangleInfoMap* obj)
{
	return obj->m_convexEpsilon;
}

btScalar btTriangleInfoMap_getEdgeDistanceThreshold(btTriangleInfoMap* obj)
{
	return obj->m_edgeDistanceThreshold;
}

btScalar btTriangleInfoMap_getEqualVertexThreshold(btTriangleInfoMap* obj)
{
	return obj->m_equalVertexThreshold;
}

btScalar btTriangleInfoMap_getMaxEdgeAngleThreshold(btTriangleInfoMap* obj)
{
	return obj->m_maxEdgeAngleThreshold;
}

btScalar btTriangleInfoMap_getPlanarEpsilon(btTriangleInfoMap* obj)
{
	return obj->m_planarEpsilon;
}

btScalar btTriangleInfoMap_getZeroAreaThreshold(btTriangleInfoMap* obj)
{
	return obj->m_zeroAreaThreshold;
}

const char* btTriangleInfoMap_serialize(btTriangleInfoMap* obj, void* dataBuffer,
	btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btTriangleInfoMap_setConvexEpsilon(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_convexEpsilon = value;
}

void btTriangleInfoMap_setEdgeDistanceThreshold(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_edgeDistanceThreshold = value;
}

void btTriangleInfoMap_setEqualVertexThreshold(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_equalVertexThreshold = value;
}

void btTriangleInfoMap_setMaxEdgeAngleThreshold(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_maxEdgeAngleThreshold = value;
}

void btTriangleInfoMap_setPlanarEpsilon(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_planarEpsilon = value;
}

void btTriangleInfoMap_setZeroAreaThreshold(btTriangleInfoMap* obj, btScalar value)
{
	obj->m_zeroAreaThreshold = value;
}

void btTriangleInfoMap_delete(btTriangleInfoMap* obj)
{
	delete obj;
}
