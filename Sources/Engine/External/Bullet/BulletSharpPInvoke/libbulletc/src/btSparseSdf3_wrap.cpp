#include <BulletSoftBody/btSparseSDF.h>

#include "btSparseSdf3_wrap.h"

btSparseSdf_3* btSparseSdf_new()
{
	return new btSparseSdf_3();
}

void btSparseSdf3_GarbageCollect(btSparseSdf_3* obj, int lifetime)
{
	obj->GarbageCollect(lifetime);
}

void btSparseSdf3_Initialize(btSparseSdf_3* obj, int hashsize, int clampCells)
{
	obj->Initialize(hashsize, clampCells);
}

int btSparseSdf3_RemoveReferences(btSparseSdf_3* obj, btCollisionShape* pcs)
{
	return obj->RemoveReferences(pcs);
}

void btSparseSdf3_Reset(btSparseSdf_3* obj)
{
	obj->Reset();
}

void btSparseSdf_delete(btSparseSdf_3* obj)
{
	delete obj;
}
