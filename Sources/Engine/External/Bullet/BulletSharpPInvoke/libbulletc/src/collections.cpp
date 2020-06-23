#include <BulletCollision/CollisionShapes/btCompoundShape.h>
#include <BulletSoftBody/btSoftBody.h>

#include "conversion.h"
#include "collections.h"

btCompoundShapeChild* btCompoundShapeChild_array_at(btCompoundShapeChild* a, int n)
{
	return &a[n];
}

btSoftBody::Node* btSoftBodyNodePtrArray_at(btSoftBody::Node** obj, int n)
{
	return obj[n];
}

void btSoftBodyNodePtrArray_set(btSoftBodyNodePtrArray* obj, btSoftBody_Node* value, int index)
{
	obj[index] = value;
}

void btVector3_array_at(const btVector3* obj, int n, btVector3* value)
{
	BTVECTOR3_SET(value, obj[n]);
}

void btVector3_array_set(btVector3* obj, int n, const btVector3* value)
{
	BTVECTOR3_COPY(&obj[n], value);
}

btAlignedObjectArray_btVector3* btAlignedObjectArray_btVector3_new()
{
	return new btAlignedObjectArray_btVector3();
}

void btAlignedObjectArray_btVector3_at(btAlignedObjectArray_btVector3* obj, int n, btVector3* value)
{
	BTVECTOR3_SET(value, obj->at(n));
}

void btAlignedObjectArray_btVector3_push_back(btAlignedObjectArray_btVector3* obj, const btVector3* value)
{
	BTVECTOR3_IN(value);
	obj->push_back(BTVECTOR3_USE(value));
}

void btAlignedObjectArray_btVector3_push_back2(btAlignedObjectArray_btVector3* obj, const btVector4* value)
{
	BTVECTOR4_IN(value);
	obj->push_back(BTVECTOR4_USE(value));
}

void btAlignedObjectArray_btVector3_set(btAlignedObjectArray_btVector3* obj, int n, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->at(n), value);
}

int btAlignedObjectArray_btVector3_size(btAlignedObjectArray_btVector3* obj)
{
	return obj->size();
}

void btAlignedObjectArray_btVector3_delete(btAlignedObjectArray_btVector3* obj)
{
	delete obj;
}
