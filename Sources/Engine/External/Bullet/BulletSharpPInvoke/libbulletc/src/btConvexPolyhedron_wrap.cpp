#include <BulletCollision/CollisionShapes/btConvexPolyhedron.h>

#include "conversion.h"
#include "btConvexPolyhedron_wrap.h"

btFace* btFace_new()
{
	return new btFace();
}

btAlignedObjectArray_int* btFace_getIndices(btFace* obj)
{
	return &obj->m_indices;
}

btScalar* btFace_getPlane(btFace* obj)
{
	return obj->m_plane;
}

void btFace_delete(btFace* obj)
{
	delete obj;
}


btConvexPolyhedron* btConvexPolyhedron_new()
{
	return new btConvexPolyhedron();
}

void btConvexPolyhedron_getExtents(btConvexPolyhedron* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_extents);
}

btAlignedObjectArray_btFace* btConvexPolyhedron_getFaces(btConvexPolyhedron* obj)
{
	return &obj->m_faces;
}

void btConvexPolyhedron_getLocalCenter(btConvexPolyhedron* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_localCenter);
}

void btConvexPolyhedron_getMC(btConvexPolyhedron* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->mC);
}

void btConvexPolyhedron_getME(btConvexPolyhedron* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->mE);
}

btScalar btConvexPolyhedron_getRadius(btConvexPolyhedron* obj)
{
	return obj->m_radius;
}

btAlignedObjectArray_btVector3* btConvexPolyhedron_getUniqueEdges(btConvexPolyhedron* obj)
{
	return &obj->m_uniqueEdges;
}

btAlignedObjectArray_btVector3* btConvexPolyhedron_getVertices(btConvexPolyhedron* obj)
{
	return &obj->m_vertices;
}

void btConvexPolyhedron_initialize(btConvexPolyhedron* obj)
{
	obj->initialize();
}

void btConvexPolyhedron_project(btConvexPolyhedron* obj, const btTransform* trans,
	const btVector3* dir, btScalar* minProj, btScalar* maxProj, btVector3* witnesPtMin,
	btVector3* witnesPtMax)
{
	BTTRANSFORM_IN(trans);
	BTVECTOR3_IN(dir);
	BTVECTOR3_DEF(witnesPtMin);
	BTVECTOR3_DEF(witnesPtMax);
	obj->project(BTTRANSFORM_USE(trans), BTVECTOR3_USE(dir), *minProj, *maxProj,
		BTVECTOR3_USE(witnesPtMin), BTVECTOR3_USE(witnesPtMax));
	BTVECTOR3_DEF_OUT(witnesPtMin);
	BTVECTOR3_DEF_OUT(witnesPtMax);
}

void btConvexPolyhedron_setExtents(btConvexPolyhedron* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_extents, value);
}

void btConvexPolyhedron_setLocalCenter(btConvexPolyhedron* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_localCenter, value);
}

void btConvexPolyhedron_setMC(btConvexPolyhedron* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->mC, value);
}

void btConvexPolyhedron_setME(btConvexPolyhedron* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->mE, value);
}

void btConvexPolyhedron_setRadius(btConvexPolyhedron* obj, btScalar value)
{
	obj->m_radius = value;
}

bool btConvexPolyhedron_testContainment(btConvexPolyhedron* obj)
{
	return obj->testContainment();
}

void btConvexPolyhedron_delete(btConvexPolyhedron* obj)
{
	delete obj;
}
