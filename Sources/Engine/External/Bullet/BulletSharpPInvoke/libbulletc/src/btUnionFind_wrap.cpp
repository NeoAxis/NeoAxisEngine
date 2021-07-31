#include <BulletCollision/CollisionDispatch/btUnionFind.h>

#include "btUnionFind_wrap.h"

btElement* btElement_new()
{
	return new btElement();
}

int btElement_getId(btElement* obj)
{
	return obj->m_id;
}

int btElement_getSz(btElement* obj)
{
	return obj->m_sz;
}

void btElement_setId(btElement* obj, int value)
{
	obj->m_id = value;
}

void btElement_setSz(btElement* obj, int value)
{
	obj->m_sz = value;
}

void btElement_delete(btElement* obj)
{
	delete obj;
}


btUnionFind* btUnionFind_new()
{
	return new btUnionFind();
}

void btUnionFind_allocate(btUnionFind* obj, int N)
{
	obj->allocate(N);
}

int btUnionFind_find(btUnionFind* obj, int p, int q)
{
	return obj->find(p, q);
}

int btUnionFind_find2(btUnionFind* obj, int x)
{
	return obj->find(x);
}

void btUnionFind_Free(btUnionFind* obj)
{
	obj->Free();
}

btElement* btUnionFind_getElement(btUnionFind* obj, int index)
{
	return &obj->getElement(index);
}

int btUnionFind_getNumElements(btUnionFind* obj)
{
	return obj->getNumElements();
}

bool btUnionFind_isRoot(btUnionFind* obj, int x)
{
	return obj->isRoot(x);
}

void btUnionFind_reset(btUnionFind* obj, int N)
{
	obj->reset(N);
}

void btUnionFind_sortIslands(btUnionFind* obj)
{
	obj->sortIslands();
}

void btUnionFind_unite(btUnionFind* obj, int p, int q)
{
	obj->unite(p, q);
}

void btUnionFind_delete(btUnionFind* obj)
{
	delete obj;
}
