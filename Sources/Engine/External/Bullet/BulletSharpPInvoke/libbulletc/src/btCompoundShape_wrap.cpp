#include <BulletCollision/BroadphaseCollision/btDbvt.h>
#include <BulletCollision/CollisionShapes/btCompoundShape.h>

#include "conversion.h"
#include "btCompoundShape_wrap.h"

btScalar btCompoundShapeChild_getChildMargin(btCompoundShapeChild* obj)
{
	return obj->m_childMargin;
}

btCollisionShape* btCompoundShapeChild_getChildShape(btCompoundShapeChild* obj)
{
	return obj->m_childShape;
}

int btCompoundShapeChild_getChildShapeType(btCompoundShapeChild* obj)
{
	return obj->m_childShapeType;
}

btDbvtNode* btCompoundShapeChild_getNode(btCompoundShapeChild* obj)
{
	return obj->m_node;
}

void btCompoundShapeChild_getTransform(btCompoundShapeChild* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_transform);
}

void btCompoundShapeChild_setChildMargin(btCompoundShapeChild* obj, btScalar value)
{
	obj->m_childMargin = value;
}

void btCompoundShapeChild_setChildShape(btCompoundShapeChild* obj, btCollisionShape* value)
{
	obj->m_childShape = value;
}

void btCompoundShapeChild_setChildShapeType(btCompoundShapeChild* obj, int value)
{
	obj->m_childShapeType = value;
}

void btCompoundShapeChild_setNode(btCompoundShapeChild* obj, btDbvtNode* value)
{
	obj->m_node = value;
}

void btCompoundShapeChild_setTransform(btCompoundShapeChild* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_transform, value);
}

void btCompoundShapeChild_delete(btCompoundShapeChild* obj)
{
	delete obj;
}


btCompoundShape* btCompoundShape_new(bool enableDynamicAabbTree, int initialChildCapacity)
{
	return new btCompoundShape(enableDynamicAabbTree, initialChildCapacity);
}

void btCompoundShape_addChildShape(btCompoundShape* obj, const btTransform* localTransform,
	btCollisionShape* shape)
{
	BTTRANSFORM_IN(localTransform);
	obj->addChildShape(BTTRANSFORM_USE(localTransform), shape);
}

void btCompoundShape_calculatePrincipalAxisTransform(btCompoundShape* obj, btScalar* masses,
	btTransform* principal, btVector3* inertia)
{
	BTTRANSFORM_IN(principal);
	BTVECTOR3_DEF(inertia);
	obj->calculatePrincipalAxisTransform(masses, BTTRANSFORM_USE(principal), BTVECTOR3_USE(inertia));
	BTTRANSFORM_DEF_OUT(principal);
	BTVECTOR3_DEF_OUT(inertia);
}

void btCompoundShape_createAabbTreeFromChildren(btCompoundShape* obj)
{
	obj->createAabbTreeFromChildren();
}

btCompoundShapeChild* btCompoundShape_getChildList(btCompoundShape* obj)
{
	return obj->getChildList();
}

btCollisionShape* btCompoundShape_getChildShape(btCompoundShape* obj, int index)
{
	return obj->getChildShape(index);
}

void btCompoundShape_getChildTransform(btCompoundShape* obj, int index, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getChildTransform(index));
}

btDbvt* btCompoundShape_getDynamicAabbTree(btCompoundShape* obj)
{
	return obj->getDynamicAabbTree();
}

int btCompoundShape_getNumChildShapes(btCompoundShape* obj)
{
	return obj->getNumChildShapes();
}

int btCompoundShape_getUpdateRevision(btCompoundShape* obj)
{
	return obj->getUpdateRevision();
}

void btCompoundShape_recalculateLocalAabb(btCompoundShape* obj)
{
	obj->recalculateLocalAabb();
}

void btCompoundShape_removeChildShape(btCompoundShape* obj, btCollisionShape* shape)
{
	obj->removeChildShape(shape);
}

void btCompoundShape_removeChildShapeByIndex(btCompoundShape* obj, int childShapeindex)
{
	obj->removeChildShapeByIndex(childShapeindex);
}

void btCompoundShape_updateChildTransform(btCompoundShape* obj, int childIndex,
	const btTransform* newChildTransform, bool shouldRecalculateLocalAabb)
{
	BTTRANSFORM_IN(newChildTransform);
	obj->updateChildTransform(childIndex, BTTRANSFORM_USE(newChildTransform), shouldRecalculateLocalAabb);
}
