#include <BulletCollision/BroadphaseCollision/btQuantizedBvh.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btQuantizedBvh_wrap.h"

btQuantizedBvhNode* btQuantizedBvhNode_new()
{
	return new btQuantizedBvhNode();
}

int btQuantizedBvhNode_getEscapeIndex(btQuantizedBvhNode* obj)
{
	return obj->getEscapeIndex();
}

int btQuantizedBvhNode_getEscapeIndexOrTriangleIndex(btQuantizedBvhNode* obj)
{
	return obj->m_escapeIndexOrTriangleIndex;
}

int btQuantizedBvhNode_getPartId(btQuantizedBvhNode* obj)
{
	return obj->getPartId();
}

unsigned short* btQuantizedBvhNode_getQuantizedAabbMax(btQuantizedBvhNode* obj)
{
	return obj->m_quantizedAabbMax;
}

unsigned short* btQuantizedBvhNode_getQuantizedAabbMin(btQuantizedBvhNode* obj)
{
	return obj->m_quantizedAabbMin;
}

int btQuantizedBvhNode_getTriangleIndex(btQuantizedBvhNode* obj)
{
	return obj->getTriangleIndex();
}

bool btQuantizedBvhNode_isLeafNode(btQuantizedBvhNode* obj)
{
	return obj->isLeafNode();
}

void btQuantizedBvhNode_setEscapeIndexOrTriangleIndex(btQuantizedBvhNode* obj, int value)
{
	obj->m_escapeIndexOrTriangleIndex = value;
}

void btQuantizedBvhNode_delete(btQuantizedBvhNode* obj)
{
	delete obj;
}


btOptimizedBvhNode* btOptimizedBvhNode_new()
{
	return new btOptimizedBvhNode();
}

void btOptimizedBvhNode_getAabbMaxOrg(btOptimizedBvhNode* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_aabbMaxOrg);
}

void btOptimizedBvhNode_getAabbMinOrg(btOptimizedBvhNode* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_aabbMinOrg);
}

int btOptimizedBvhNode_getEscapeIndex(btOptimizedBvhNode* obj)
{
	return obj->m_escapeIndex;
}

int btOptimizedBvhNode_getSubPart(btOptimizedBvhNode* obj)
{
	return obj->m_subPart;
}

int btOptimizedBvhNode_getTriangleIndex(btOptimizedBvhNode* obj)
{
	return obj->m_triangleIndex;
}

void btOptimizedBvhNode_setAabbMaxOrg(btOptimizedBvhNode* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_aabbMaxOrg, value);
}

void btOptimizedBvhNode_setAabbMinOrg(btOptimizedBvhNode* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_aabbMinOrg, value);
}

void btOptimizedBvhNode_setEscapeIndex(btOptimizedBvhNode* obj, int value)
{
	obj->m_escapeIndex = value;
}

void btOptimizedBvhNode_setSubPart(btOptimizedBvhNode* obj, int value)
{
	obj->m_subPart = value;
}

void btOptimizedBvhNode_setTriangleIndex(btOptimizedBvhNode* obj, int value)
{
	obj->m_triangleIndex = value;
}

void btOptimizedBvhNode_delete(btOptimizedBvhNode* obj)
{
	delete obj;
}


void btNodeOverlapCallback_processNode(btNodeOverlapCallback* obj, int subPart, int triangleIndex)
{
	obj->processNode(subPart, triangleIndex);
}

void btNodeOverlapCallback_delete(btNodeOverlapCallback* obj)
{
	delete obj;
}


btQuantizedBvh* btQuantizedBvh_new()
{
	return new btQuantizedBvh();
}

void btQuantizedBvh_buildInternal(btQuantizedBvh* obj)
{
	obj->buildInternal();
}

unsigned int btQuantizedBvh_calculateSerializeBufferSize(btQuantizedBvh* obj)
{
	return obj->calculateSerializeBufferSize();
}

int btQuantizedBvh_calculateSerializeBufferSizeNew(btQuantizedBvh* obj)
{
	return obj->calculateSerializeBufferSizeNew();
}

void btQuantizedBvh_deSerializeDouble(btQuantizedBvh* obj, btQuantizedBvhDoubleData* quantizedBvhDoubleData)
{
	obj->deSerializeDouble(*quantizedBvhDoubleData);
}

void btQuantizedBvh_deSerializeFloat(btQuantizedBvh* obj, btQuantizedBvhFloatData* quantizedBvhFloatData)
{
	obj->deSerializeFloat(*quantizedBvhFloatData);
}

btQuantizedBvh* btQuantizedBvh_deSerializeInPlace(void* i_alignedDataBuffer, unsigned int i_dataBufferSize,
	bool i_swapEndian)
{
	return btQuantizedBvh::deSerializeInPlace(i_alignedDataBuffer, i_dataBufferSize,
		i_swapEndian);
}

unsigned int btQuantizedBvh_getAlignmentSerializationPadding()
{
	return btQuantizedBvh::getAlignmentSerializationPadding();
}

btAlignedObjectArray_btQuantizedBvhNode* btQuantizedBvh_getLeafNodeArray(btQuantizedBvh* obj)
{
	return &obj->getLeafNodeArray();
}

btAlignedObjectArray_btQuantizedBvhNode* btQuantizedBvh_getQuantizedNodeArray(btQuantizedBvh* obj)
{
	return &obj->getQuantizedNodeArray();
}

btAlignedObjectArray_btBvhSubtreeInfo* btQuantizedBvh_getSubtreeInfoArray(btQuantizedBvh* obj)
{
	return &obj->getSubtreeInfoArray();
}

bool btQuantizedBvh_isQuantized(btQuantizedBvh* obj)
{
	return obj->isQuantized();
}

void btQuantizedBvh_quantize(btQuantizedBvh* obj, unsigned short* out, const btVector3* point,
	int isMax)
{
	BTVECTOR3_IN(point);
	obj->quantize(out, BTVECTOR3_USE(point), isMax);
}

void btQuantizedBvh_quantizeWithClamp(btQuantizedBvh* obj, unsigned short* out, const btVector3* point2,
	int isMax)
{
	BTVECTOR3_IN(point2);
	obj->quantizeWithClamp(out, BTVECTOR3_USE(point2), isMax);
}

void btQuantizedBvh_reportAabbOverlappingNodex(btQuantizedBvh* obj, btNodeOverlapCallback* nodeCallback,
	const btVector3* aabbMin, const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->reportAabbOverlappingNodex(nodeCallback, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

void btQuantizedBvh_reportBoxCastOverlappingNodex(btQuantizedBvh* obj, btNodeOverlapCallback* nodeCallback,
	const btVector3* raySource, const btVector3* rayTarget, const btVector3* aabbMin,
	const btVector3* aabbMax)
{
	BTVECTOR3_IN(raySource);
	BTVECTOR3_IN(rayTarget);
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->reportBoxCastOverlappingNodex(nodeCallback, BTVECTOR3_USE(raySource), BTVECTOR3_USE(rayTarget),
		BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}

void btQuantizedBvh_reportRayOverlappingNodex(btQuantizedBvh* obj, btNodeOverlapCallback* nodeCallback,
	const btVector3* raySource, const btVector3* rayTarget)
{
	BTVECTOR3_IN(raySource);
	BTVECTOR3_IN(rayTarget);
	obj->reportRayOverlappingNodex(nodeCallback, BTVECTOR3_USE(raySource), BTVECTOR3_USE(rayTarget));
}

bool btQuantizedBvh_serialize(btQuantizedBvh* obj, void* o_alignedDataBuffer, unsigned int i_dataBufferSize,
	bool i_swapEndian)
{
	return obj->serialize(o_alignedDataBuffer, i_dataBufferSize, i_swapEndian);
}

const char* btQuantizedBvh_serialize2(btQuantizedBvh* obj, void* dataBuffer, btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btQuantizedBvh_setQuantizationValues(btQuantizedBvh* obj, const btVector3* bvhAabbMin,
	const btVector3* bvhAabbMax, btScalar quantizationMargin)
{
	BTVECTOR3_IN(bvhAabbMin);
	BTVECTOR3_IN(bvhAabbMax);
	obj->setQuantizationValues(BTVECTOR3_USE(bvhAabbMin), BTVECTOR3_USE(bvhAabbMax),
		quantizationMargin);
}

void btQuantizedBvh_setTraversalMode(btQuantizedBvh* obj, btQuantizedBvh::btTraversalMode traversalMode)
{
	obj->setTraversalMode(traversalMode);
}

void btQuantizedBvh_unQuantize(btQuantizedBvh* obj, const unsigned short* vecIn,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->unQuantize(vecIn);
	BTVECTOR3_SET(value, temp);
}

void btQuantizedBvh_delete(btQuantizedBvh* obj)
{
	delete obj;
}
