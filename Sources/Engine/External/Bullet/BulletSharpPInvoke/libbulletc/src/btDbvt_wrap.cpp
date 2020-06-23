#include <BulletCollision/BroadphaseCollision/btDbvt.h>

#include "conversion.h"
#include "btDbvt_wrap.h"

btDbvtAabbMm* btDbvtAabbMm_new()
{
	return new btDbvtAabbMm();
}

void btDbvtAabbMm_Center(btDbvtAabbMm* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->Center();
	BTVECTOR3_SET(value, temp);
}

int btDbvtAabbMm_Classify(btDbvtAabbMm* obj, const btVector3* n, btScalar o, int s)
{
	BTVECTOR3_IN(n);
	return obj->Classify(BTVECTOR3_USE(n), o, s);
}

bool btDbvtAabbMm_Contain(btDbvtAabbMm* obj, const btDbvtAabbMm* a)
{
	return obj->Contain(*a);
}

void btDbvtAabbMm_Expand(btDbvtAabbMm* obj, const btVector3* e)
{
	BTVECTOR3_IN(e);
	obj->Expand(BTVECTOR3_USE(e));
}

void btDbvtAabbMm_Extents(btDbvtAabbMm* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->Extents();
	BTVECTOR3_SET(value, temp);
}

btDbvtAabbMm* btDbvtAabbMm_FromCE(const btVector3* c, const btVector3* e)
{
	btDbvtAabbMm* ret = new btDbvtAabbMm;
	BTVECTOR3_IN(c);
	BTVECTOR3_IN(e);
	*ret = btDbvtAabbMm::FromCE(BTVECTOR3_USE(c), BTVECTOR3_USE(e));
	return ret;
}

btDbvtAabbMm* btDbvtAabbMm_FromCR(const btVector3* c, btScalar r)
{
	btDbvtAabbMm* ret = new btDbvtAabbMm;
	BTVECTOR3_IN(c);
	*ret = btDbvtAabbMm::FromCR(BTVECTOR3_USE(c), r);
	return ret;
}

btDbvtAabbMm* btDbvtAabbMm_FromMM(const btVector3* mi, const btVector3* mx)
{
	btDbvtAabbMm* ret = new btDbvtAabbMm;
	BTVECTOR3_IN(mi);
	BTVECTOR3_IN(mx);
	*ret = btDbvtAabbMm::FromMM(BTVECTOR3_USE(mi), BTVECTOR3_USE(mx));
	return ret;
}

btDbvtAabbMm* btDbvtAabbMm_FromPoints(const btVector3** ppts, int n)
{
	btDbvtAabbMm* ret = new btDbvtAabbMm;
	*ret = btDbvtAabbMm::FromPoints(ppts, n);
	return ret;
}

btDbvtAabbMm* btDbvtAabbMm_FromPoints2(const btVector3* pts, int n)
{
	btDbvtAabbMm* ret = new btDbvtAabbMm;
	*ret = btDbvtAabbMm::FromPoints(pts, n);
	return ret;
}

void btDbvtAabbMm_Lengths(btDbvtAabbMm* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->Lengths();
	BTVECTOR3_SET(value, temp);
}

void btDbvtAabbMm_Maxs(btDbvtAabbMm* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->Maxs());
}

void btDbvtAabbMm_Mins(btDbvtAabbMm* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->Mins());
}

btScalar btDbvtAabbMm_ProjectMinimum(btDbvtAabbMm* obj, const btVector3* v, unsigned int signs)
{
	BTVECTOR3_IN(v);
	return obj->ProjectMinimum(BTVECTOR3_USE(v), signs);
}

void btDbvtAabbMm_SignedExpand(btDbvtAabbMm* obj, const btVector3* e)
{
	BTVECTOR3_IN(e);
	obj->SignedExpand(BTVECTOR3_USE(e));
}

void btDbvtAabbMm_tMaxs(btDbvtAabbMm* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->tMaxs());
}

void btDbvtAabbMm_tMins(btDbvtAabbMm* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->tMins());
}

void btDbvtAabbMm_delete(btDbvtAabbMm* obj)
{
	delete obj;
}


btDbvtNode* btDbvtNode_new()
{
	return new btDbvtNode();
}

btDbvtNode** btDbvtNode_getChilds(btDbvtNode* obj)
{
	return obj->childs;
}

void* btDbvtNode_getData(btDbvtNode* obj)
{
	return obj->data;
}

int btDbvtNode_getDataAsInt(btDbvtNode* obj)
{
	return obj->dataAsInt;
}

btDbvtNode* btDbvtNode_getParent(btDbvtNode* obj)
{
	return obj->parent;
}

btDbvtVolume* btDbvtNode_getVolume(btDbvtNode* obj)
{
	return &obj->volume;
}

bool btDbvtNode_isinternal(btDbvtNode* obj)
{
	return obj->isinternal();
}

bool btDbvtNode_isleaf(btDbvtNode* obj)
{
	return obj->isleaf();
}

void btDbvtNode_setData(btDbvtNode* obj, void* value)
{
	obj->data = value;
}

void btDbvtNode_setDataAsInt(btDbvtNode* obj, int value)
{
	obj->dataAsInt = value;
}

void btDbvtNode_setParent(btDbvtNode* obj, btDbvtNode* value)
{
	obj->parent = value;
}

void btDbvtNode_delete(btDbvtNode* obj)
{
	delete obj;
}


btDbvt_IClone* btDbvt_IClone_new()
{
	return new btDbvt::IClone();
}

void btDbvt_IClone_CloneLeaf(btDbvt_IClone* obj, btDbvtNode* __unnamed0)
{
	obj->CloneLeaf(__unnamed0);
}

void btDbvt_IClone_delete(btDbvt_IClone* obj)
{
	delete obj;
}


btDbvt_ICollide* btDbvt_ICollide_new()
{
	return new btDbvt::ICollide();
}

bool btDbvt_ICollide_AllLeaves(btDbvt_ICollide* obj, const btDbvtNode* __unnamed0)
{
	return obj->AllLeaves(__unnamed0);
}

bool btDbvt_ICollide_Descent(btDbvt_ICollide* obj, const btDbvtNode* __unnamed0)
{
	return obj->Descent(__unnamed0);
}

void btDbvt_ICollide_Process(btDbvt_ICollide* obj, const btDbvtNode* __unnamed0,
	const btDbvtNode* __unnamed1)
{
	obj->Process(__unnamed0, __unnamed1);
}

void btDbvt_ICollide_Process2(btDbvt_ICollide* obj, const btDbvtNode* __unnamed0)
{
	obj->Process(__unnamed0);
}

void btDbvt_ICollide_Process3(btDbvt_ICollide* obj, const btDbvtNode* n, btScalar __unnamed1)
{
	obj->Process(n, __unnamed1);
}

void btDbvt_ICollide_delete(btDbvt_ICollide* obj)
{
	delete obj;
}


void btDbvt_IWriter_Prepare(btDbvt_IWriter* obj, const btDbvtNode* root, int numnodes)
{
	obj->Prepare(root, numnodes);
}

void btDbvt_IWriter_WriteLeaf(btDbvt_IWriter* obj, const btDbvtNode* __unnamed0,
	int index, int parent)
{
	obj->WriteLeaf(__unnamed0, index, parent);
}

void btDbvt_IWriter_WriteNode(btDbvt_IWriter* obj, const btDbvtNode* __unnamed0,
	int index, int parent, int child0, int child1)
{
	obj->WriteNode(__unnamed0, index, parent, child0, child1);
}

void btDbvt_IWriter_delete(btDbvt_IWriter* obj)
{
	delete obj;
}


btDbvt_sStkCLN* btDbvt_sStkCLN_new(const btDbvtNode* n, btDbvtNode* p)
{
	return new btDbvt::sStkCLN(n, p);
}

const btDbvtNode* btDbvt_sStkCLN_getNode(btDbvt_sStkCLN* obj)
{
	return obj->node;
}

btDbvtNode* btDbvt_sStkCLN_getParent(btDbvt_sStkCLN* obj)
{
	return obj->parent;
}

void btDbvt_sStkCLN_setNode(btDbvt_sStkCLN* obj, const btDbvtNode* value)
{
	obj->node = value;
}

void btDbvt_sStkCLN_setParent(btDbvt_sStkCLN* obj, btDbvtNode* value)
{
	obj->parent = value;
}

void btDbvt_sStkCLN_delete(btDbvt_sStkCLN* obj)
{
	delete obj;
}


btDbvt_sStkNN* btDbvt_sStkNN_new()
{
	return new btDbvt::sStkNN();
}

btDbvt_sStkNN* btDbvt_sStkNN_new2(const btDbvtNode* na, const btDbvtNode* nb)
{
	return new btDbvt::sStkNN(na, nb);
}

const btDbvtNode* btDbvt_sStkNN_getA(btDbvt_sStkNN* obj)
{
	return obj->a;
}

const btDbvtNode* btDbvt_sStkNN_getB(btDbvt_sStkNN* obj)
{
	return obj->b;
}

void btDbvt_sStkNN_setA(btDbvt_sStkNN* obj, const btDbvtNode* value)
{
	obj->a = value;
}

void btDbvt_sStkNN_setB(btDbvt_sStkNN* obj, const btDbvtNode* value)
{
	obj->b = value;
}

void btDbvt_sStkNN_delete(btDbvt_sStkNN* obj)
{
	delete obj;
}


btDbvt_sStkNP* btDbvt_sStkNP_new(const btDbvtNode* n, unsigned int m)
{
	return new btDbvt::sStkNP(n, m);
}

int btDbvt_sStkNP_getMask(btDbvt_sStkNP* obj)
{
	return obj->mask;
}

const btDbvtNode* btDbvt_sStkNP_getNode(btDbvt_sStkNP* obj)
{
	return obj->node;
}

void btDbvt_sStkNP_setMask(btDbvt_sStkNP* obj, int value)
{
	obj->mask = value;
}

void btDbvt_sStkNP_setNode(btDbvt_sStkNP* obj, const btDbvtNode* value)
{
	obj->node = value;
}

void btDbvt_sStkNP_delete(btDbvt_sStkNP* obj)
{
	delete obj;
}


btDbvt_sStkNPS* btDbvt_sStkNPS_new()
{
	return new btDbvt::sStkNPS();
}

btDbvt_sStkNPS* btDbvt_sStkNPS_new2(const btDbvtNode* n, unsigned int m, btScalar v)
{
	return new btDbvt::sStkNPS(n, m, v);
}

int btDbvt_sStkNPS_getMask(btDbvt_sStkNPS* obj)
{
	return obj->mask;
}

const btDbvtNode* btDbvt_sStkNPS_getNode(btDbvt_sStkNPS* obj)
{
	return obj->node;
}

btScalar btDbvt_sStkNPS_getValue(btDbvt_sStkNPS* obj)
{
	return obj->value;
}

void btDbvt_sStkNPS_setMask(btDbvt_sStkNPS* obj, int value)
{
	obj->mask = value;
}

void btDbvt_sStkNPS_setNode(btDbvt_sStkNPS* obj, const btDbvtNode* value)
{
	obj->node = value;
}

void btDbvt_sStkNPS_setValue(btDbvt_sStkNPS* obj, btScalar value)
{
	obj->value = value;
}

void btDbvt_sStkNPS_delete(btDbvt_sStkNPS* obj)
{
	delete obj;
}


btDbvt* btDbvt_new()
{
	return new btDbvt();
}

int btDbvt_allocate(btAlignedObjectArray_int* ifree, btAlignedObjectArray_btDbvt_sStkNPS* stock,
	const btDbvt_sStkNPS* value)
{
	return btDbvt::allocate(*ifree, *stock, *value);
}

void btDbvt_benchmark()
{
	btDbvt::benchmark();
}

void btDbvt_clear(btDbvt* obj)
{
	obj->clear();
}

void btDbvt_clone(btDbvt* obj, btDbvt* dest)
{
	obj->clone(*dest);
}

void btDbvt_clone2(btDbvt* obj, btDbvt* dest, btDbvt_IClone* iclone)
{
	obj->clone(*dest, iclone);
}
/*
void btDbvt_collideKDOP(const btDbvtNode* root, const btVector3* normals, const btScalar* offsets,
	int count, const btDbvt_ICollide* policy)
{
	btDbvt::collideKDOP(root, normals, offsets, count, *policy);
}

void btDbvt_collideOCL(const btDbvtNode* root, const btVector3* normals, const btScalar* offsets,
	const btVector3* sortaxis, int count, const btDbvt_ICollide* policy)
{
	BTVECTOR3_IN(sortaxis);
	btDbvt::collideOCL(root, normals, offsets, BTVECTOR3_USE(sortaxis), count, *policy);
}

void btDbvt_collideOCL2(const btDbvtNode* root, const btVector3* normals, const btScalar* offsets,
	const btVector3* sortaxis, int count, const btDbvt_ICollide* policy, bool fullsort)
{
	BTVECTOR3_IN(sortaxis);
	btDbvt::collideOCL(root, normals, offsets, BTVECTOR3_USE(sortaxis), count, *policy,
		fullsort);
}

void btDbvt_collideTT(btDbvt* obj, const btDbvtNode* root0, const btDbvtNode* root1,
	const btDbvt_ICollide* policy)
{
	obj->collideTT(root0, root1, *policy);
}

void btDbvt_collideTTpersistentStack(btDbvt* obj, const btDbvtNode* root0, const btDbvtNode* root1,
	const btDbvt_ICollide* policy)
{
	obj->collideTTpersistentStack(root0, root1, *policy);
}

void btDbvt_collideTU(const btDbvtNode* root, const btDbvt::ICollide* policy)
{
	btDbvt::collideTU(root, *policy);
}

void btDbvt_collideTV(btDbvt* obj, const btDbvtNode* root, const const btDbvtVolume** volume,
	const btDbvt_ICollide* policy)
{
	obj->collideTV(root, *volume, *policy);
}
*/
int btDbvt_countLeaves(const btDbvtNode* node)
{
	return btDbvt::countLeaves(node);
}

bool btDbvt_empty(btDbvt* obj)
{
	return obj->empty();
}
/*
void btDbvt_enumLeaves(const btDbvtNode* root, const btDbvt_ICollide* policy)
{
	btDbvt::enumLeaves(root, *policy);
}

void btDbvt_enumNodes(const btDbvtNode* root, const btDbvt_ICollide* policy)
{
	btDbvt::enumNodes(root, *policy);
}
*/
void btDbvt_extractLeaves(const btDbvtNode* node, btAlignedObjectArray_const_btDbvtNodePtr* leaves)
{
	btDbvt::extractLeaves(node, *leaves);
}

btDbvtNode* btDbvt_getFree(btDbvt* obj)
{
	return obj->m_free;
}

int btDbvt_getLeaves(btDbvt* obj)
{
	return obj->m_leaves;
}

int btDbvt_getLkhd(btDbvt* obj)
{
	return obj->m_lkhd;
}

unsigned int btDbvt_getOpath(btDbvt* obj)
{
	return obj->m_opath;
}

btDbvtNode* btDbvt_getRoot(btDbvt* obj)
{
	return obj->m_root;
}

btAlignedObjectArray_btDbvt_sStkNN* btDbvt_getStkStack(btDbvt* obj)
{
	return &obj->m_stkStack;
}

btDbvtNode* btDbvt_insert(btDbvt* obj, const btDbvtVolume* box, void* data)
{
	return obj->insert(*box, data);
}

int btDbvt_maxdepth(const btDbvtNode* node)
{
	return btDbvt::maxdepth(node);
}

int btDbvt_nearest(const int* i, const btDbvt_sStkNPS* a, btScalar v, int l, int h)
{
	return btDbvt::nearest(i, a, v, l, h);
}

void btDbvt_optimizeBottomUp(btDbvt* obj)
{
	obj->optimizeBottomUp();
}

void btDbvt_optimizeIncremental(btDbvt* obj, int passes)
{
	obj->optimizeIncremental(passes);
}

void btDbvt_optimizeTopDown(btDbvt* obj)
{
	obj->optimizeTopDown();
}

void btDbvt_optimizeTopDown2(btDbvt* obj, int bu_treshold)
{
	obj->optimizeTopDown(bu_treshold);
}
/*
void btDbvt_rayTest(const btDbvtNode* root, const btVector3* rayFrom, const btVector3* rayTo,
	const btDbvt_ICollide* policy)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	btDbvt::rayTest(root, BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *policy);
}

void btDbvt_rayTestInternal2(btDbvt* obj, const btDbvtNode* root, const btVector3* rayFrom,
	const btVector3* rayTo, const btVector3* rayDirectionInverse, unsigned int* signs,
	btScalar lambda_max, const btVector3* aabbMin, const btVector3* aabbMax, const btDbvt_ICollide* policy)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	BTVECTOR3_IN(rayDirectionInverse);
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->rayTestInternal(root, BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), BTVECTOR3_USE(rayDirectionInverse),
		signs, lambda_max, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax), *policy);
}
*/
void btDbvt_remove(btDbvt* obj, btDbvtNode* leaf)
{
	obj->remove(leaf);
}

void btDbvt_setFree(btDbvt* obj, btDbvtNode* value)
{
	obj->m_free = value;
}

void btDbvt_setLeaves(btDbvt* obj, int value)
{
	obj->m_leaves = value;
}

void btDbvt_setLkhd(btDbvt* obj, int value)
{
	obj->m_lkhd = value;
}

void btDbvt_setOpath(btDbvt* obj, unsigned int value)
{
	obj->m_opath = value;
}

void btDbvt_setRoot(btDbvt* obj, btDbvtNode* value)
{
	obj->m_root = value;
}

void btDbvt_update(btDbvt* obj, btDbvtNode* leaf, btDbvtVolume* volume)
{
	obj->update(leaf, *volume);
}

void btDbvt_update2(btDbvt* obj, btDbvtNode* leaf)
{
	obj->update(leaf);
}

void btDbvt_update3(btDbvt* obj, btDbvtNode* leaf, int lookahead)
{
	obj->update(leaf, lookahead);
}

bool btDbvt_update4(btDbvt* obj, btDbvtNode* leaf, btDbvtVolume* volume, btScalar margin)
{
	return obj->update(leaf, *volume, margin);
}

bool btDbvt_update5(btDbvt* obj, btDbvtNode* leaf, btDbvtVolume* volume, const btVector3* velocity)
{
	BTVECTOR3_IN(velocity);
	return obj->update(leaf, *volume, BTVECTOR3_USE(velocity));
}

bool btDbvt_update6(btDbvt* obj, btDbvtNode* leaf, btDbvtVolume* volume, const btVector3* velocity,
	btScalar margin)
{
	BTVECTOR3_IN(velocity);
	return obj->update(leaf, *volume, BTVECTOR3_USE(velocity), margin);
}

void btDbvt_write(btDbvt* obj, btDbvt_IWriter* iwriter)
{
	obj->write(iwriter);
}

void btDbvt_delete(btDbvt* obj)
{
	delete obj;
}
