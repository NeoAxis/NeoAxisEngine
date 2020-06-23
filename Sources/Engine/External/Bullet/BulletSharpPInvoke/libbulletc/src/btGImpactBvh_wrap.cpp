#include <BulletCollision/Gimpact/btGImpactBvh.h>

#include "conversion.h"
#include "btGImpactBvh_wrap.h"

GIM_PAIR* GIM_PAIR_new()
{
	return new GIM_PAIR();
}

GIM_PAIR* GIM_PAIR_new2(const GIM_PAIR* p)
{
	return new GIM_PAIR(*p);
}

GIM_PAIR* GIM_PAIR_new3(int index1, int index2)
{
	return new GIM_PAIR(index1, index2);
}

int GIM_PAIR_getIndex1(GIM_PAIR* obj)
{
	return obj->m_index1;
}

int GIM_PAIR_getIndex2(GIM_PAIR* obj)
{
	return obj->m_index2;
}

void GIM_PAIR_setIndex1(GIM_PAIR* obj, int value)
{
	obj->m_index1 = value;
}

void GIM_PAIR_setIndex2(GIM_PAIR* obj, int value)
{
	obj->m_index2 = value;
}

void GIM_PAIR_delete(GIM_PAIR* obj)
{
	delete obj;
}


btPairSet* btPairSet_new()
{
	return new btPairSet();
}

void btPairSet_push_pair(btPairSet* obj, int index1, int index2)
{
	obj->push_pair(index1, index2);
}

void btPairSet_push_pair_inv(btPairSet* obj, int index1, int index2)
{
	obj->push_pair_inv(index1, index2);
}

void btPairSet_delete(btPairSet* obj)
{
	delete obj;
}


GIM_BVH_DATA* GIM_BVH_DATA_new()
{
	return new GIM_BVH_DATA();
}

btAABB* GIM_BVH_DATA_getBound(GIM_BVH_DATA* obj)
{
	return &obj->m_bound;
}

int GIM_BVH_DATA_getData(GIM_BVH_DATA* obj)
{
	return obj->m_data;
}

void GIM_BVH_DATA_setBound(GIM_BVH_DATA* obj, const btAABB* value)
{
	obj->m_bound = *value;
}

void GIM_BVH_DATA_setData(GIM_BVH_DATA* obj, int value)
{
	obj->m_data = value;
}

void GIM_BVH_DATA_delete(GIM_BVH_DATA* obj)
{
	delete obj;
}


GIM_BVH_TREE_NODE* GIM_BVH_TREE_NODE_new()
{
	return new GIM_BVH_TREE_NODE();
}

btAABB* GIM_BVH_TREE_NODE_getBound(GIM_BVH_TREE_NODE* obj)
{
	return &obj->m_bound;
}

int GIM_BVH_TREE_NODE_getDataIndex(GIM_BVH_TREE_NODE* obj)
{
	return obj->getDataIndex();
}

int GIM_BVH_TREE_NODE_getEscapeIndex(GIM_BVH_TREE_NODE* obj)
{
	return obj->getEscapeIndex();
}

bool GIM_BVH_TREE_NODE_isLeafNode(GIM_BVH_TREE_NODE* obj)
{
	return obj->isLeafNode();
}

void GIM_BVH_TREE_NODE_setBound(GIM_BVH_TREE_NODE* obj, const btAABB* value)
{
	obj->m_bound = *value;
}

void GIM_BVH_TREE_NODE_setDataIndex(GIM_BVH_TREE_NODE* obj, int index)
{
	obj->setDataIndex(index);
}

void GIM_BVH_TREE_NODE_setEscapeIndex(GIM_BVH_TREE_NODE* obj, int index)
{
	obj->setEscapeIndex(index);
}

void GIM_BVH_TREE_NODE_delete(GIM_BVH_TREE_NODE* obj)
{
	delete obj;
}


GIM_BVH_DATA_ARRAY* GIM_BVH_DATA_ARRAY_new()
{
	return new GIM_BVH_DATA_ARRAY();
}

void GIM_BVH_DATA_ARRAY_delete(GIM_BVH_DATA_ARRAY* obj)
{
	delete obj;
}


GIM_BVH_TREE_NODE_ARRAY* GIM_BVH_TREE_NODE_ARRAY_new()
{
	return new GIM_BVH_TREE_NODE_ARRAY();
}

void GIM_BVH_TREE_NODE_ARRAY_delete(GIM_BVH_TREE_NODE_ARRAY* obj)
{
	delete obj;
}


btBvhTree* btBvhTree_new()
{
	return new btBvhTree();
}

void btBvhTree_build_tree(btBvhTree* obj, GIM_BVH_DATA_ARRAY* primitive_boxes)
{
	obj->build_tree(*primitive_boxes);
}

void btBvhTree_clearNodes(btBvhTree* obj)
{
	obj->clearNodes();
}

const GIM_BVH_TREE_NODE* btBvhTree_get_node_pointer(btBvhTree* obj)
{
	return obj->get_node_pointer();
}

const GIM_BVH_TREE_NODE* btBvhTree_get_node_pointer2(btBvhTree* obj, int index)
{
	return obj->get_node_pointer(index);
}

int btBvhTree_getEscapeNodeIndex(btBvhTree* obj, int nodeindex)
{
	return obj->getEscapeNodeIndex(nodeindex);
}

int btBvhTree_getLeftNode(btBvhTree* obj, int nodeindex)
{
	return obj->getLeftNode(nodeindex);
}

void btBvhTree_getNodeBound(btBvhTree* obj, int nodeindex, btAABB* bound)
{
	obj->getNodeBound(nodeindex, *bound);
}

int btBvhTree_getNodeCount(btBvhTree* obj)
{
	return obj->getNodeCount();
}

int btBvhTree_getNodeData(btBvhTree* obj, int nodeindex)
{
	return obj->getNodeData(nodeindex);
}

int btBvhTree_getRightNode(btBvhTree* obj, int nodeindex)
{
	return obj->getRightNode(nodeindex);
}

bool btBvhTree_isLeafNode(btBvhTree* obj, int nodeindex)
{
	return obj->isLeafNode(nodeindex);
}

void btBvhTree_setNodeBound(btBvhTree* obj, int nodeindex, const btAABB* bound)
{
	obj->setNodeBound(nodeindex, *bound);
}

void btBvhTree_delete(btBvhTree* obj)
{
	delete obj;
}


void btPrimitiveManagerBase_get_primitive_box(btPrimitiveManagerBase* obj, int prim_index,
	btAABB* primbox)
{
	obj->get_primitive_box(prim_index, *primbox);
}

int btPrimitiveManagerBase_get_primitive_count(btPrimitiveManagerBase* obj)
{
	return obj->get_primitive_count();
}

void btPrimitiveManagerBase_get_primitive_triangle(btPrimitiveManagerBase* obj, int prim_index,
	btPrimitiveTriangle* triangle)
{
	obj->get_primitive_triangle(prim_index, *triangle);
}

bool btPrimitiveManagerBase_is_trimesh(btPrimitiveManagerBase* obj)
{
	return obj->is_trimesh();
}

void btPrimitiveManagerBase_delete(btPrimitiveManagerBase* obj)
{
	delete obj;
}


btGImpactBvh* btGImpactBvh_new()
{
	return new btGImpactBvh();
}

btGImpactBvh* btGImpactBvh_new2(btPrimitiveManagerBase* primitive_manager)
{
	return new btGImpactBvh(primitive_manager);
}

bool btGImpactBvh_boxQuery(btGImpactBvh* obj, const btAABB* box, btAlignedObjectArray_int* collided_results)
{
	return obj->boxQuery(*box, *collided_results);
}

bool btGImpactBvh_boxQueryTrans(btGImpactBvh* obj, const btAABB* box, const btTransform* transform,
	btAlignedObjectArray_int* collided_results)
{
	BTTRANSFORM_IN(transform);
	return obj->boxQueryTrans(*box, BTTRANSFORM_USE(transform), *collided_results);
}

void btGImpactBvh_buildSet(btGImpactBvh* obj)
{
	obj->buildSet();
}

void btGImpactBvh_find_collision(btGImpactBvh* boxset1, const btTransform* trans1,
	btGImpactBvh* boxset2, const btTransform* trans2, btPairSet* collision_pairs)
{
	BTTRANSFORM_IN(trans1);
	BTTRANSFORM_IN(trans2);
	btGImpactBvh::find_collision(boxset1, BTTRANSFORM_USE(trans1), boxset2, BTTRANSFORM_USE(trans2),
		*collision_pairs);
}

const GIM_BVH_TREE_NODE* btGImpactBvh_get_node_pointer(btGImpactBvh* obj, int index)
{
	return obj->get_node_pointer(index);
}

int btGImpactBvh_getEscapeNodeIndex(btGImpactBvh* obj, int nodeindex)
{
	return obj->getEscapeNodeIndex(nodeindex);
}

btAABB* btGImpactBvh_getGlobalBox(btGImpactBvh* obj)
{
	btAABB* box = new btAABB;
	*box = obj->getGlobalBox();
	return box;
}

int btGImpactBvh_getLeftNode(btGImpactBvh* obj, int nodeindex)
{
	return obj->getLeftNode(nodeindex);
}

void btGImpactBvh_getNodeBound(btGImpactBvh* obj, int nodeindex, btAABB* bound)
{
	obj->getNodeBound(nodeindex, *bound);
}

int btGImpactBvh_getNodeCount(btGImpactBvh* obj)
{
	return obj->getNodeCount();
}

int btGImpactBvh_getNodeData(btGImpactBvh* obj, int nodeindex)
{
	return obj->getNodeData(nodeindex);
}

void btGImpactBvh_getNodeTriangle(btGImpactBvh* obj, int nodeindex, btPrimitiveTriangle* triangle)
{
	obj->getNodeTriangle(nodeindex, *triangle);
}

btPrimitiveManagerBase* btGImpactBvh_getPrimitiveManager(btGImpactBvh* obj)
{
	return obj->getPrimitiveManager();
}

int btGImpactBvh_getRightNode(btGImpactBvh* obj, int nodeindex)
{
	return obj->getRightNode(nodeindex);
}

bool btGImpactBvh_hasHierarchy(btGImpactBvh* obj)
{
	return obj->hasHierarchy();
}

bool btGImpactBvh_isLeafNode(btGImpactBvh* obj, int nodeindex)
{
	return obj->isLeafNode(nodeindex);
}

bool btGImpactBvh_isTrimesh(btGImpactBvh* obj)
{
	return obj->isTrimesh();
}

bool btGImpactBvh_rayQuery(btGImpactBvh* obj, const btVector3* ray_dir, const btVector3* ray_origin,
	btAlignedObjectArray_int* collided_results)
{
	BTVECTOR3_IN(ray_dir);
	BTVECTOR3_IN(ray_origin);
	return obj->rayQuery(BTVECTOR3_USE(ray_dir), BTVECTOR3_USE(ray_origin), *collided_results);
}

void btGImpactBvh_setNodeBound(btGImpactBvh* obj, int nodeindex, const btAABB* bound)
{
	obj->setNodeBound(nodeindex, *bound);
}

void btGImpactBvh_setPrimitiveManager(btGImpactBvh* obj, btPrimitiveManagerBase* primitive_manager)
{
	obj->setPrimitiveManager(primitive_manager);
}

void btGImpactBvh_update(btGImpactBvh* obj)
{
	obj->update();
}

void btGImpactBvh_delete(btGImpactBvh* obj)
{
	delete obj;
}
