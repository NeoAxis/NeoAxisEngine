#include <BulletCollision/Gimpact/btTriangleShapeEx.h>

#include "conversion.h"
#include "btTriangleShapeEx_wrap.h"

GIM_TRIANGLE_CONTACT* GIM_TRIANGLE_CONTACT_new()
{
	return new GIM_TRIANGLE_CONTACT();
}

GIM_TRIANGLE_CONTACT* GIM_TRIANGLE_CONTACT_new2(const GIM_TRIANGLE_CONTACT* other)
{
	return new GIM_TRIANGLE_CONTACT(*other);
}

void GIM_TRIANGLE_CONTACT_copy_from(GIM_TRIANGLE_CONTACT* obj, const GIM_TRIANGLE_CONTACT* other)
{
	obj->copy_from(*other);
}

btScalar GIM_TRIANGLE_CONTACT_getPenetration_depth(GIM_TRIANGLE_CONTACT* obj)
{
	return obj->m_penetration_depth;
}

int GIM_TRIANGLE_CONTACT_getPoint_count(GIM_TRIANGLE_CONTACT* obj)
{
	return obj->m_point_count;
}

btVector3* GIM_TRIANGLE_CONTACT_getPoints(GIM_TRIANGLE_CONTACT* obj)
{
	return obj->m_points;
}

void GIM_TRIANGLE_CONTACT_getSeparating_normal(GIM_TRIANGLE_CONTACT* obj, btVector4* value)
{
	BTVECTOR4_SET(value, obj->m_separating_normal);
}

void GIM_TRIANGLE_CONTACT_merge_points(GIM_TRIANGLE_CONTACT* obj, const btVector4* plane,
	btScalar margin, const btVector3* points, int point_count)
{
	BTVECTOR4_IN(plane);
	obj->merge_points(BTVECTOR4_USE(plane), margin, points, point_count);
}

void GIM_TRIANGLE_CONTACT_setPenetration_depth(GIM_TRIANGLE_CONTACT* obj, btScalar value)
{
	obj->m_penetration_depth = value;
}

void GIM_TRIANGLE_CONTACT_setPoint_count(GIM_TRIANGLE_CONTACT* obj, int value)
{
	obj->m_point_count = value;
}

void GIM_TRIANGLE_CONTACT_setSeparating_normal(GIM_TRIANGLE_CONTACT* obj, const btVector4* value)
{
	BTVECTOR4_COPY(&obj->m_separating_normal, value);
}

void GIM_TRIANGLE_CONTACT_delete(GIM_TRIANGLE_CONTACT* obj)
{
	delete obj;
}


btPrimitiveTriangle* btPrimitiveTriangle_new()
{
	return new btPrimitiveTriangle();
}

void btPrimitiveTriangle_applyTransform(btPrimitiveTriangle* obj, const btTransform* t)
{
	BTTRANSFORM_IN(t);
	obj->applyTransform(BTTRANSFORM_USE(t));
}

void btPrimitiveTriangle_buildTriPlane(btPrimitiveTriangle* obj)
{
	obj->buildTriPlane();
}

int btPrimitiveTriangle_clip_triangle(btPrimitiveTriangle* obj, btPrimitiveTriangle* other,
	btVector3* clipped_points)
{
	return obj->clip_triangle(*other, clipped_points);
}

bool btPrimitiveTriangle_find_triangle_collision_clip_method(btPrimitiveTriangle* obj,
	btPrimitiveTriangle* other, GIM_TRIANGLE_CONTACT* contacts)
{
	return obj->find_triangle_collision_clip_method(*other, *contacts);
}

void btPrimitiveTriangle_get_edge_plane(btPrimitiveTriangle* obj, int edge_index,
	btVector4* plane)
{
	BTVECTOR4_DEF(plane);
	obj->get_edge_plane(edge_index, BTVECTOR4_USE(plane));
	BTVECTOR4_DEF_OUT(plane);
}

btScalar btPrimitiveTriangle_getDummy(btPrimitiveTriangle* obj)
{
	return obj->m_dummy;
}

btScalar btPrimitiveTriangle_getMargin(btPrimitiveTriangle* obj)
{
	return obj->m_margin;
}

void btPrimitiveTriangle_getPlane(btPrimitiveTriangle* obj, btVector4* value)
{
	BTVECTOR4_SET(value, obj->m_plane);
}

btVector3* btPrimitiveTriangle_getVertices(btPrimitiveTriangle* obj)
{
	return obj->m_vertices;
}

bool btPrimitiveTriangle_overlap_test_conservative(btPrimitiveTriangle* obj, const btPrimitiveTriangle* other)
{
	return obj->overlap_test_conservative(*other);
}

void btPrimitiveTriangle_setDummy(btPrimitiveTriangle* obj, btScalar value)
{
	obj->m_dummy = value;
}

void btPrimitiveTriangle_setMargin(btPrimitiveTriangle* obj, btScalar value)
{
	obj->m_margin = value;
}

void btPrimitiveTriangle_setPlane(btPrimitiveTriangle* obj, const btVector4* value)
{
	BTVECTOR4_COPY(&obj->m_plane, value);
}

void btPrimitiveTriangle_delete(btPrimitiveTriangle* obj)
{
	delete obj;
}


btTriangleShapeEx* btTriangleShapeEx_new()
{
	return new btTriangleShapeEx();
}

btTriangleShapeEx* btTriangleShapeEx_new2(const btVector3* p0, const btVector3* p1,
	const btVector3* p2)
{
	BTVECTOR3_IN(p0);
	BTVECTOR3_IN(p1);
	BTVECTOR3_IN(p2);
	return new btTriangleShapeEx(BTVECTOR3_USE(p0), BTVECTOR3_USE(p1), BTVECTOR3_USE(p2));
}

btTriangleShapeEx* btTriangleShapeEx_new3(const btTriangleShapeEx* other)
{
	return new btTriangleShapeEx(*other);
}

void btTriangleShapeEx_applyTransform(btTriangleShapeEx* obj, const btTransform* t)
{
	BTTRANSFORM_IN(t);
	obj->applyTransform(BTTRANSFORM_USE(t));
}

void btTriangleShapeEx_buildTriPlane(btTriangleShapeEx* obj, btVector4* plane)
{
	BTVECTOR4_DEF(plane);
	obj->buildTriPlane(BTVECTOR4_USE(plane));
	BTVECTOR4_DEF_OUT(plane);
}

bool btTriangleShapeEx_overlap_test_conservative(btTriangleShapeEx* obj, const btTriangleShapeEx* other)
{
	return obj->overlap_test_conservative(*other);
}
