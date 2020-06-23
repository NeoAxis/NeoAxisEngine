#include <BulletCollision/Gimpact/btBoxCollision.h>

#include "conversion.h"
#include "btBoxCollision_wrap.h"

BT_BOX_BOX_TRANSFORM_CACHE* BT_BOX_BOX_TRANSFORM_CACHE_new()
{
	return new BT_BOX_BOX_TRANSFORM_CACHE();
}

void BT_BOX_BOX_TRANSFORM_CACHE_calc_absolute_matrix(BT_BOX_BOX_TRANSFORM_CACHE* obj)
{
	obj->calc_absolute_matrix();
}

void BT_BOX_BOX_TRANSFORM_CACHE_calc_from_full_invert(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btTransform* trans0, const btTransform* trans1)
{
	BTTRANSFORM_IN(trans0);
	BTTRANSFORM_IN(trans1);
	obj->calc_from_full_invert(BTTRANSFORM_USE(trans0), BTTRANSFORM_USE(trans1));
}

void BT_BOX_BOX_TRANSFORM_CACHE_calc_from_homogenic(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btTransform* trans0, const btTransform* trans1)
{
	BTTRANSFORM_IN(trans0);
	BTTRANSFORM_IN(trans1);
	obj->calc_from_homogenic(BTTRANSFORM_USE(trans0), BTTRANSFORM_USE(trans1));
}

void BT_BOX_BOX_TRANSFORM_CACHE_getAR(BT_BOX_BOX_TRANSFORM_CACHE* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_AR);
}

void BT_BOX_BOX_TRANSFORM_CACHE_getR1to0(BT_BOX_BOX_TRANSFORM_CACHE* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_R1to0);
}

void BT_BOX_BOX_TRANSFORM_CACHE_getT1to0(BT_BOX_BOX_TRANSFORM_CACHE* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_T1to0);
}

void BT_BOX_BOX_TRANSFORM_CACHE_setAR(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_AR, (btScalar*)value);
}

void BT_BOX_BOX_TRANSFORM_CACHE_setR1to0(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_R1to0, (btScalar*)value);
}

void BT_BOX_BOX_TRANSFORM_CACHE_setT1to0(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_T1to0, value);
}

void BT_BOX_BOX_TRANSFORM_CACHE_transform(BT_BOX_BOX_TRANSFORM_CACHE* obj, const btVector3* point, btVector3* value)
{
	BTVECTOR3_DEF(value);
	BTVECTOR3_IN(point);
	*value = obj->transform(BTVECTOR3_USE(point));
	BTVECTOR3_DEF_OUT(value);
}

void BT_BOX_BOX_TRANSFORM_CACHE_delete(BT_BOX_BOX_TRANSFORM_CACHE* obj)
{
	delete obj;
}


btAABB* btAABB_new()
{
	return new btAABB();
}

btAABB* btAABB_new2(const btVector3* V1, const btVector3* V2, const btVector3* V3)
{
	BTVECTOR3_IN(V1);
	BTVECTOR3_IN(V2);
	BTVECTOR3_IN(V3);
	return new btAABB(BTVECTOR3_USE(V1), BTVECTOR3_USE(V2), BTVECTOR3_USE(V3));
}

btAABB* btAABB_new3(const btVector3* V1, const btVector3* V2, const btVector3* V3,
	btScalar margin)
{
	BTVECTOR3_IN(V1);
	BTVECTOR3_IN(V2);
	BTVECTOR3_IN(V3);
	return new btAABB(BTVECTOR3_USE(V1), BTVECTOR3_USE(V2), BTVECTOR3_USE(V3), margin);
}

btAABB* btAABB_new4(const btAABB* other)
{
	return new btAABB(*other);
}

btAABB* btAABB_new5(const btAABB* other, btScalar margin)
{
	return new btAABB(*other, margin);
}

void btAABB_appy_transform(btAABB* obj, const btTransform* trans)
{
	BTTRANSFORM_IN(trans);
	obj->appy_transform(BTTRANSFORM_USE(trans));
}

void btAABB_appy_transform_trans_cache(btAABB* obj, const BT_BOX_BOX_TRANSFORM_CACHE* trans)
{
	obj->appy_transform_trans_cache(*trans);
}

bool btAABB_collide_plane(btAABB* obj, const btVector4* plane)
{
	BTVECTOR4_IN(plane);
	return obj->collide_plane(BTVECTOR4_USE(plane));
}

bool btAABB_collide_ray(btAABB* obj, const btVector3* vorigin, const btVector3* vdir)
{
	BTVECTOR3_IN(vorigin);
	BTVECTOR3_IN(vdir);
	return obj->collide_ray(BTVECTOR3_USE(vorigin), BTVECTOR3_USE(vdir));
}

bool btAABB_collide_triangle_exact(btAABB* obj, const btVector3* p1, const btVector3* p2,
	const btVector3* p3, const btVector4* triangle_plane)
{
	BTVECTOR3_IN(p1);
	BTVECTOR3_IN(p2);
	BTVECTOR3_IN(p3);
	BTVECTOR4_IN(triangle_plane);
	return obj->collide_triangle_exact(BTVECTOR3_USE(p1), BTVECTOR3_USE(p2), BTVECTOR3_USE(p3),
		BTVECTOR4_USE(triangle_plane));
}

void btAABB_copy_with_margin(btAABB* obj, const btAABB* other, btScalar margin)
{
	obj->copy_with_margin(*other, margin);
}

void btAABB_find_intersection(btAABB* obj, const btAABB* other, btAABB* intersection)
{
	obj->find_intersection(*other, *intersection);
}

void btAABB_get_center_extend(btAABB* obj, btVector3* center, btVector3* extend)
{
	BTVECTOR3_DEF(center);
	BTVECTOR3_DEF(extend);
	obj->get_center_extend(BTVECTOR3_USE(center), BTVECTOR3_USE(extend));
	BTVECTOR3_DEF_OUT(center);
	BTVECTOR3_DEF_OUT(extend);
}

void btAABB_getMax(btAABB* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_max);
}

void btAABB_getMin(btAABB* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_min);
}

bool btAABB_has_collision(btAABB* obj, const btAABB* other)
{
	return obj->has_collision(*other);
}

void btAABB_increment_margin(btAABB* obj, btScalar margin)
{
	obj->increment_margin(margin);
}

void btAABB_invalidate(btAABB* obj)
{
	obj->invalidate();
}

void btAABB_merge(btAABB* obj, const btAABB* box)
{
	obj->merge(*box);
}

bool btAABB_overlapping_trans_cache(btAABB* obj, const btAABB* box, const BT_BOX_BOX_TRANSFORM_CACHE* transcache,
	bool fulltest)
{
	return obj->overlapping_trans_cache(*box, *transcache, fulltest);
}

bool btAABB_overlapping_trans_conservative(btAABB* obj, const btAABB* box, btTransform* trans1_to_0)
{
	BTTRANSFORM_IN(trans1_to_0);
	return obj->overlapping_trans_conservative(*box, BTTRANSFORM_USE(trans1_to_0));
}

bool btAABB_overlapping_trans_conservative2(btAABB* obj, const btAABB* box, const BT_BOX_BOX_TRANSFORM_CACHE* trans1_to_0)
{
	return obj->overlapping_trans_conservative2(*box, *trans1_to_0);
}

eBT_PLANE_INTERSECTION_TYPE btAABB_plane_classify(btAABB* obj, const btVector4* plane)
{
	BTVECTOR4_IN(plane);
	return obj->plane_classify(BTVECTOR4_USE(plane));
}

void btAABB_projection_interval(btAABB* obj, const btVector3* direction, btScalar* vmin,
	btScalar* vmax)
{
	BTVECTOR3_IN(direction);
	obj->projection_interval(BTVECTOR3_USE(direction), *vmin, *vmax);
}

void btAABB_setMax(btAABB* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_max, value);
}

void btAABB_setMin(btAABB* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_min, value);
}

void btAABB_delete(btAABB* obj)
{
	delete obj;
}
