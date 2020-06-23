#include <BulletCollision/Gimpact/btGImpactQuantizedBvh.h>

#include "conversion.h"
#include "btGImpactQuantizedBvh_wrap.h"

BT_QUANTIZED_BVH_NODE* BT_QUANTIZED_BVH_NODE_new()
{
	return new BT_QUANTIZED_BVH_NODE();
}

int BT_QUANTIZED_BVH_NODE_getDataIndex(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->getDataIndex();
}

int BT_QUANTIZED_BVH_NODE_getEscapeIndex(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->getEscapeIndex();
}

int BT_QUANTIZED_BVH_NODE_getEscapeIndexOrDataIndex(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->m_escapeIndexOrDataIndex;
}

unsigned short* BT_QUANTIZED_BVH_NODE_getQuantizedAabbMax(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->m_quantizedAabbMax;
}

unsigned short* BT_QUANTIZED_BVH_NODE_getQuantizedAabbMin(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->m_quantizedAabbMin;
}

bool BT_QUANTIZED_BVH_NODE_isLeafNode(BT_QUANTIZED_BVH_NODE* obj)
{
	return obj->isLeafNode();
}

void BT_QUANTIZED_BVH_NODE_setDataIndex(BT_QUANTIZED_BVH_NODE* obj, int index)
{
	obj->setDataIndex(index);
}

void BT_QUANTIZED_BVH_NODE_setEscapeIndex(BT_QUANTIZED_BVH_NODE* obj, int index)
{
	obj->setEscapeIndex(index);
}

void BT_QUANTIZED_BVH_NODE_setEscapeIndexOrDataIndex(BT_QUANTIZED_BVH_NODE* obj,
	int value)
{
	obj->m_escapeIndexOrDataIndex = value;
}

bool BT_QUANTIZED_BVH_NODE_testQuantizedBoxOverlapp(BT_QUANTIZED_BVH_NODE* obj, unsigned short* quantizedMin,
	unsigned short* quantizedMax)
{
	return obj->testQuantizedBoxOverlapp(quantizedMin, quantizedMax);
}

void BT_QUANTIZED_BVH_NODE_delete(BT_QUANTIZED_BVH_NODE* obj)
{
	delete obj;
}


GIM_QUANTIZED_BVH_NODE_ARRAY* GIM_QUANTIZED_BVH_NODE_ARRAY_new()
{
	return new GIM_QUANTIZED_BVH_NODE_ARRAY();
}

void GIM_QUANTIZED_BVH_NODE_ARRAY_delete(GIM_QUANTIZED_BVH_NODE_ARRAY* obj)
{
	delete obj;
}


btQuantizedBvhTree* btQuantizedBvhTree_new()
{
	return new btQuantizedBvhTree();
}

void btQuantizedBvhTree_build_tree(btQuantizedBvhTree* obj, GIM_BVH_DATA_ARRAY* primitive_boxes)
{
	obj->build_tree(*primitive_boxes);
}

void btQuantizedBvhTree_clearNodes(btQuantizedBvhTree* obj)
{
	obj->clearNodes();
}

const BT_QUANTIZED_BVH_NODE* btQuantizedBvhTree_get_node_pointer(btQuantizedBvhTree* obj,
	int index)
{
	return obj->get_node_pointer(index);
}

int btQuantizedBvhTree_getEscapeNodeIndex(btQuantizedBvhTree* obj, int nodeindex)
{
	return obj->getEscapeNodeIndex(nodeindex);
}

int btQuantizedBvhTree_getLeftNode(btQuantizedBvhTree* obj, int nodeindex)
{
	return obj->getLeftNode(nodeindex);
}

void btQuantizedBvhTree_getNodeBound(btQuantizedBvhTree* obj, int nodeindex, btAABB* bound)
{
	obj->getNodeBound(nodeindex, *bound);
}

int btQuantizedBvhTree_getNodeCount(btQuantizedBvhTree* obj)
{
	return obj->getNodeCount();
}

int btQuantizedBvhTree_getNodeData(btQuantizedBvhTree* obj, int nodeindex)
{
	return obj->getNodeData(nodeindex);
}

int btQuantizedBvhTree_getRightNode(btQuantizedBvhTree* obj, int nodeindex)
{
	return obj->getRightNode(nodeindex);
}

bool btQuantizedBvhTree_isLeafNode(btQuantizedBvhTree* obj, int nodeindex)
{
	return obj->isLeafNode(nodeindex);
}

void btQuantizedBvhTree_quantizePoint(btQuantizedBvhTree* obj, unsigned short* quantizedpoint,
	const btVector3* point)
{
	BTVECTOR3_IN(point);
	obj->quantizePoint(quantizedpoint, BTVECTOR3_USE(point));
}

void btQuantizedBvhTree_setNodeBound(btQuantizedBvhTree* obj, int nodeindex, const btAABB* bound)
{
	obj->setNodeBound(nodeindex, *bound);
}

bool btQuantizedBvhTree_testQuantizedBoxOverlapp(btQuantizedBvhTree* obj, int node_index,
	unsigned short* quantizedMin, unsigned short* quantizedMax)
{
	return obj->testQuantizedBoxOverlapp(node_index, quantizedMin, quantizedMax);
}

void btQuantizedBvhTree_delete(btQuantizedBvhTree* obj)
{
	delete obj;
}


btGImpactQuantizedBvh* btGImpactQuantizedBvh_new()
{
	return new btGImpactQuantizedBvh();
}

btGImpactQuantizedBvh* btGImpactQuantizedBvh_new2(btPrimitiveManagerBase* primitive_manager)
{
	return new btGImpactQuantizedBvh(primitive_manager);
}

bool btGImpactQuantizedBvh_boxQuery(btGImpactQuantizedBvh* obj, const btAABB* box,
	btAlignedObjectArray_int* collided_results)
{
	return obj->boxQuery(*box, *collided_results);
}

bool btGImpactQuantizedBvh_boxQueryTrans(btGImpactQuantizedBvh* obj, const btAABB* box,
	const btTransform* transform, btAlignedObjectArray_int* collided_results)
{
	BTTRANSFORM_IN(transform);
	return obj->boxQueryTrans(*box, BTTRANSFORM_USE(transform), *collided_results);
}

void btGImpactQuantizedBvh_buildSet(btGImpactQuantizedBvh* obj)
{
	obj->buildSet();
}

void btGImpactQuantizedBvh_find_collision(const btGImpactQuantizedBvh* boxset1, const btTransform* trans1,
	const btGImpactQuantizedBvh* boxset2, const btTransform* trans2, btPairSet* collision_pairs)
{
	BTTRANSFORM_IN(trans1);
	BTTRANSFORM_IN(trans2);
	btGImpactQuantizedBvh::find_collision(boxset1, BTTRANSFORM_USE(trans1), boxset2,
		BTTRANSFORM_USE(trans2), *collision_pairs);
}

const BT_QUANTIZED_BVH_NODE* btGImpactQuantizedBvh_get_node_pointer(btGImpactQuantizedBvh* obj,
	int index)
{
	return obj->get_node_pointer(index);
}

int btGImpactQuantizedBvh_getEscapeNodeIndex(btGImpactQuantizedBvh* obj, int nodeindex)
{
	return obj->getEscapeNodeIndex(nodeindex);
}

btAABB* btGImpactQuantizedBvh_getGlobalBox(btGImpactQuantizedBvh* obj)
{
	btAABB* box = new btAABB;
	*box = obj->getGlobalBox();
	return box;
}

int btGImpactQuantizedBvh_getLeftNode(btGImpactQuantizedBvh* obj, int nodeindex)
{
	return obj->getLeftNode(nodeindex);
}

void btGImpactQuantizedBvh_getNodeBound(btGImpactQuantizedBvh* obj, int nodeindex,
	btAABB* bound)
{
	obj->getNodeBound(nodeindex, *bound);
}

int btGImpactQuantizedBvh_getNodeCount(btGImpactQuantizedBvh* obj)
{
	return obj->getNodeCount();
}

int btGImpactQuantizedBvh_getNodeData(btGImpactQuantizedBvh* obj, int nodeindex)
{
	return obj->getNodeData(nodeindex);
}

void btGImpactQuantizedBvh_getNodeTriangle(btGImpactQuantizedBvh* obj, int nodeindex,
	btPrimitiveTriangle* triangle)
{
	obj->getNodeTriangle(nodeindex, *triangle);
}

btPrimitiveManagerBase* btGImpactQuantizedBvh_getPrimitiveManager(btGImpactQuantizedBvh* obj)
{
	return obj->getPrimitiveManager();
}

int btGImpactQuantizedBvh_getRightNode(btGImpactQuantizedBvh* obj, int nodeindex)
{
	return obj->getRightNode(nodeindex);
}

bool btGImpactQuantizedBvh_hasHierarchy(btGImpactQuantizedBvh* obj)
{
	return obj->hasHierarchy();
}

bool btGImpactQuantizedBvh_isLeafNode(btGImpactQuantizedBvh* obj, int nodeindex)
{
	return obj->isLeafNode(nodeindex);
}

bool btGImpactQuantizedBvh_isTrimesh(btGImpactQuantizedBvh* obj)
{
	return obj->isTrimesh();
}

bool btGImpactQuantizedBvh_rayQuery(btGImpactQuantizedBvh* obj, const btVector3* ray_dir,
	const btVector3* ray_origin, btAlignedObjectArray_int* collided_results)
{
	BTVECTOR3_IN(ray_dir);
	BTVECTOR3_IN(ray_origin);
	return obj->rayQuery(BTVECTOR3_USE(ray_dir), BTVECTOR3_USE(ray_origin), *collided_results);
}

void btGImpactQuantizedBvh_setNodeBound(btGImpactQuantizedBvh* obj, int nodeindex,
	const btAABB* bound)
{
	obj->setNodeBound(nodeindex, *bound);
}

void btGImpactQuantizedBvh_setPrimitiveManager(btGImpactQuantizedBvh* obj, btPrimitiveManagerBase* primitive_manager)
{
	obj->setPrimitiveManager(primitive_manager);
}

void btGImpactQuantizedBvh_update(btGImpactQuantizedBvh* obj)
{
	obj->update();
}

void btGImpactQuantizedBvh_delete(btGImpactQuantizedBvh* obj)
{
	delete obj;
}
