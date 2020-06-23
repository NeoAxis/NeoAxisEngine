#include <BulletCollision/Gimpact/btGImpactShape.h>

#include "conversion.h"
#include "btGImpactShape_wrap.h"

btTetrahedronShapeEx* btTetrahedronShapeEx_new()
{
	return new btTetrahedronShapeEx();
}

void btTetrahedronShapeEx_setVertices(btTetrahedronShapeEx* obj, const btVector3* v0,
	const btVector3* v1, const btVector3* v2, const btVector3* v3)
{
	BTVECTOR3_IN(v0);
	BTVECTOR3_IN(v1);
	BTVECTOR3_IN(v2);
	BTVECTOR3_IN(v3);
	obj->setVertices(BTVECTOR3_USE(v0), BTVECTOR3_USE(v1), BTVECTOR3_USE(v2), BTVECTOR3_USE(v3));
}


bool btGImpactShapeInterface_childrenHasTransform(btGImpactShapeInterface* obj)
{
	return obj->childrenHasTransform();
}

const btGImpactBoxSet* btGImpactShapeInterface_getBoxSet(btGImpactShapeInterface* obj)
{
	return obj->getBoxSet();
}

void btGImpactShapeInterface_getBulletTetrahedron(btGImpactShapeInterface* obj, int prim_index,
	btTetrahedronShapeEx* tetrahedron)
{
	obj->getBulletTetrahedron(prim_index, *tetrahedron);
}

void btGImpactShapeInterface_getBulletTriangle(btGImpactShapeInterface* obj, int prim_index,
	btTriangleShapeEx* triangle)
{
	obj->getBulletTriangle(prim_index, *triangle);
}

void btGImpactShapeInterface_getChildAabb(btGImpactShapeInterface* obj, int child_index,
	const btTransform* t, btVector3* aabbMin, btVector3* aabbMax)
{
	BTTRANSFORM_IN(t);
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getChildAabb(child_index, BTTRANSFORM_USE(t), BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

btCollisionShape* btGImpactShapeInterface_getChildShape(btGImpactShapeInterface* obj,
	int index)
{
	return obj->getChildShape(index);
}

void btGImpactShapeInterface_getChildTransform(btGImpactShapeInterface* obj, int index,
	btTransform* value)
{
	ATTRIBUTE_ALIGNED16(btTransform) temp = obj->getChildTransform(index);
	BTTRANSFORM_SET(value, temp);
}

eGIMPACT_SHAPE_TYPE btGImpactShapeInterface_getGImpactShapeType(btGImpactShapeInterface* obj)
{
	return obj->getGImpactShapeType();
}

const btAABB* btGImpactShapeInterface_getLocalBox(btGImpactShapeInterface* obj)
{
	return &obj->getLocalBox();
}

int btGImpactShapeInterface_getNumChildShapes(btGImpactShapeInterface* obj)
{
	return obj->getNumChildShapes();
}

const btPrimitiveManagerBase* btGImpactShapeInterface_getPrimitiveManager(btGImpactShapeInterface* obj)
{
	return obj->getPrimitiveManager();
}

void btGImpactShapeInterface_getPrimitiveTriangle(btGImpactShapeInterface* obj, int index,
	btPrimitiveTriangle* triangle)
{
	obj->getPrimitiveTriangle(index, *triangle);
}

bool btGImpactShapeInterface_hasBoxSet(btGImpactShapeInterface* obj)
{
	return obj->hasBoxSet();
}

void btGImpactShapeInterface_lockChildShapes(btGImpactShapeInterface* obj)
{
	obj->lockChildShapes();
}

bool btGImpactShapeInterface_needsRetrieveTetrahedrons(btGImpactShapeInterface* obj)
{
	return obj->needsRetrieveTetrahedrons();
}

bool btGImpactShapeInterface_needsRetrieveTriangles(btGImpactShapeInterface* obj)
{
	return obj->needsRetrieveTriangles();
}

void btGImpactShapeInterface_postUpdate(btGImpactShapeInterface* obj)
{
	obj->postUpdate();
}

void btGImpactShapeInterface_processAllTrianglesRay(btGImpactShapeInterface* obj,
	btTriangleCallback* __unnamed0, const btVector3* __unnamed1, const btVector3* __unnamed2)
{
	BTVECTOR3_IN(__unnamed1);
	BTVECTOR3_IN(__unnamed2);
	obj->processAllTrianglesRay(__unnamed0, BTVECTOR3_USE(__unnamed1), BTVECTOR3_USE(__unnamed2));
}

void btGImpactShapeInterface_rayTest(btGImpactShapeInterface* obj, const btVector3* rayFrom,
	const btVector3* rayTo, btCollisionWorld_RayResultCallback* resultCallback)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *resultCallback);
}

void btGImpactShapeInterface_setChildTransform(btGImpactShapeInterface* obj, int index,
	const btTransform* transform)
{
	BTTRANSFORM_IN(transform);
	obj->setChildTransform(index, BTTRANSFORM_USE(transform));
}

void btGImpactShapeInterface_unlockChildShapes(btGImpactShapeInterface* obj)
{
	obj->unlockChildShapes();
}

void btGImpactShapeInterface_updateBound(btGImpactShapeInterface* obj)
{
	obj->updateBound();
}


btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new(
	const btGImpactCompoundShape_CompoundPrimitiveManager* compound)
{
	return new btGImpactCompoundShape::CompoundPrimitiveManager(*compound);
}

btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new2(
	btGImpactCompoundShape* compoundShape)
{
	return new btGImpactCompoundShape::CompoundPrimitiveManager(compoundShape);
}

btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_CompoundPrimitiveManager_new3()
{
	return new btGImpactCompoundShape::CompoundPrimitiveManager();
}

btGImpactCompoundShape* btGImpactCompoundShape_CompoundPrimitiveManager_getCompoundShape(
	btGImpactCompoundShape_CompoundPrimitiveManager* obj)
{
	return obj->m_compoundShape;
}

void btGImpactCompoundShape_CompoundPrimitiveManager_setCompoundShape(btGImpactCompoundShape_CompoundPrimitiveManager* obj,
	btGImpactCompoundShape* value)
{
	obj->m_compoundShape = value;
}


btGImpactCompoundShape* btGImpactCompoundShape_new(bool children_has_transform)
{
	return new btGImpactCompoundShape(children_has_transform);
}

void btGImpactCompoundShape_addChildShape(btGImpactCompoundShape* obj, const btTransform* localTransform,
	btCollisionShape* shape)
{
	BTTRANSFORM_IN(localTransform);
	obj->addChildShape(BTTRANSFORM_USE(localTransform), shape);
}

void btGImpactCompoundShape_addChildShape2(btGImpactCompoundShape* obj, btCollisionShape* shape)
{
	obj->addChildShape(shape);
}

btGImpactCompoundShape_CompoundPrimitiveManager* btGImpactCompoundShape_getCompoundPrimitiveManager(
	btGImpactCompoundShape* obj)
{
	return obj->getCompoundPrimitiveManager();
}


btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new(
	btStridingMeshInterface* meshInterface, int part)
{
	return new btGImpactMeshShapePart::TrimeshPrimitiveManager(meshInterface, part);
}

btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new2(
	const btGImpactMeshShapePart_TrimeshPrimitiveManager* manager)
{
	return new btGImpactMeshShapePart::TrimeshPrimitiveManager(*manager);
}

btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_TrimeshPrimitiveManager_new3()
{
	return new btGImpactMeshShapePart::TrimeshPrimitiveManager();
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_bullet_triangle(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int prim_index, btTriangleShapeEx* triangle)
{
	obj->get_bullet_triangle(prim_index, *triangle);
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_indices(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int face_index, unsigned int* i0, unsigned int* i1, unsigned int* i2b)
{
	obj->get_indices(face_index, *i0, *i1, *i2b);
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_get_vertex(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	unsigned int vertex_index, btVector3* vertex)
{
	BTVECTOR3_DEF(vertex);
	obj->get_vertex(vertex_index, BTVECTOR3_USE(vertex));
	BTVECTOR3_DEF_OUT(vertex);
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_get_vertex_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->get_vertex_count();
}

const unsigned char* btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndexbase(
	btGImpactMeshShapePart::TrimeshPrimitiveManager* obj)
{
	return obj->indexbase;
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndexstride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->indexstride;
}

PHY_ScalarType btGImpactMeshShapePart_TrimeshPrimitiveManager_getIndicestype(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->indicestype;
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getLock_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->m_lock_count;
}

btScalar btGImpactMeshShapePart_TrimeshPrimitiveManager_getMargin(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->m_margin;
}

btStridingMeshInterface* btGImpactMeshShapePart_TrimeshPrimitiveManager_getMeshInterface(
	btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->m_meshInterface;
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getNumfaces(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->numfaces;
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getNumverts(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->numverts;
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getPart(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->m_part;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_getScale(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_scale);
}

int btGImpactMeshShapePart_TrimeshPrimitiveManager_getStride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->stride;
}

PHY_ScalarType btGImpactMeshShapePart_TrimeshPrimitiveManager_getType(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	return obj->type;
}

const unsigned char* btGImpactMeshShapePart_TrimeshPrimitiveManager_getVertexbase(
	btGImpactMeshShapePart::TrimeshPrimitiveManager* obj)
{
	return obj->vertexbase;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_lock(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	obj->lock();
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	const unsigned char* value)
{
	obj->indexbase = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndexstride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->indexstride = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setIndicestype(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	PHY_ScalarType value)
{
	obj->indicestype = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setLock_count(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->m_lock_count = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setMargin(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	btScalar value)
{
	obj->m_margin = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setMeshInterface(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	btStridingMeshInterface* value)
{
	obj->m_meshInterface = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setNumfaces(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->numfaces = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setNumverts(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->numverts = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setPart(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->m_part = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setScale(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_scale, value);
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setStride(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	int value)
{
	obj->stride = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setType(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	PHY_ScalarType value)
{
	obj->type = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_setVertexbase(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj,
	const unsigned char* value)
{
	obj->vertexbase = value;
}

void btGImpactMeshShapePart_TrimeshPrimitiveManager_unlock(btGImpactMeshShapePart_TrimeshPrimitiveManager* obj)
{
	obj->unlock();
}


btGImpactMeshShapePart* btGImpactMeshShapePart_new()
{
	return new btGImpactMeshShapePart();
}

btGImpactMeshShapePart* btGImpactMeshShapePart_new2(btStridingMeshInterface* meshInterface,
	int part)
{
	return new btGImpactMeshShapePart(meshInterface, part);
}

int btGImpactMeshShapePart_getPart(btGImpactMeshShapePart* obj)
{
	return obj->getPart();
}

btGImpactMeshShapePart_TrimeshPrimitiveManager* btGImpactMeshShapePart_getTrimeshPrimitiveManager(
	btGImpactMeshShapePart* obj)
{
	return obj->getTrimeshPrimitiveManager();
}

void btGImpactMeshShapePart_getVertex(btGImpactMeshShapePart* obj, int vertex_index,
	btVector3* vertex)
{
	BTVECTOR3_DEF(vertex);
	obj->getVertex(vertex_index, BTVECTOR3_USE(vertex));
	BTVECTOR3_DEF_OUT(vertex);
}

int btGImpactMeshShapePart_getVertexCount(btGImpactMeshShapePart* obj)
{
	return obj->getVertexCount();
}


btGImpactMeshShape* btGImpactMeshShape_new(btStridingMeshInterface* meshInterface)
{
	return new btGImpactMeshShape(meshInterface);
}

btStridingMeshInterface* btGImpactMeshShape_getMeshInterface(btGImpactMeshShape* obj)
{
	return obj->getMeshInterface();
}

btGImpactMeshShapePart* btGImpactMeshShape_getMeshPart(btGImpactMeshShape* obj, int index)
{
	return obj->getMeshPart(index);
}

int btGImpactMeshShape_getMeshPartCount(btGImpactMeshShape* obj)
{
	return obj->getMeshPartCount();
}
