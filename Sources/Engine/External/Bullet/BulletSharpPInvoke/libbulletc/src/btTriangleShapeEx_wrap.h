#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT GIM_TRIANGLE_CONTACT* GIM_TRIANGLE_CONTACT_new();
	EXPORT GIM_TRIANGLE_CONTACT* GIM_TRIANGLE_CONTACT_new2(const GIM_TRIANGLE_CONTACT* other);
	EXPORT void GIM_TRIANGLE_CONTACT_copy_from(GIM_TRIANGLE_CONTACT* obj, const GIM_TRIANGLE_CONTACT* other);
	EXPORT btScalar GIM_TRIANGLE_CONTACT_getPenetration_depth(GIM_TRIANGLE_CONTACT* obj);
	EXPORT int GIM_TRIANGLE_CONTACT_getPoint_count(GIM_TRIANGLE_CONTACT* obj);
	EXPORT btVector3* GIM_TRIANGLE_CONTACT_getPoints(GIM_TRIANGLE_CONTACT* obj);
	EXPORT void GIM_TRIANGLE_CONTACT_getSeparating_normal(GIM_TRIANGLE_CONTACT* obj, btVector4* value);
	EXPORT void GIM_TRIANGLE_CONTACT_merge_points(GIM_TRIANGLE_CONTACT* obj, const btVector4* plane, btScalar margin, const btVector3* points, int point_count);
	EXPORT void GIM_TRIANGLE_CONTACT_setPenetration_depth(GIM_TRIANGLE_CONTACT* obj, btScalar value);
	EXPORT void GIM_TRIANGLE_CONTACT_setPoint_count(GIM_TRIANGLE_CONTACT* obj, int value);
	EXPORT void GIM_TRIANGLE_CONTACT_setSeparating_normal(GIM_TRIANGLE_CONTACT* obj, const btVector4* value);
	EXPORT void GIM_TRIANGLE_CONTACT_delete(GIM_TRIANGLE_CONTACT* obj);

	EXPORT btPrimitiveTriangle* btPrimitiveTriangle_new();
	EXPORT void btPrimitiveTriangle_applyTransform(btPrimitiveTriangle* obj, const btTransform* t);
	EXPORT void btPrimitiveTriangle_buildTriPlane(btPrimitiveTriangle* obj);
	EXPORT int btPrimitiveTriangle_clip_triangle(btPrimitiveTriangle* obj, btPrimitiveTriangle* other, btVector3* clipped_points);
	EXPORT bool btPrimitiveTriangle_find_triangle_collision_clip_method(btPrimitiveTriangle* obj, btPrimitiveTriangle* other, GIM_TRIANGLE_CONTACT* contacts);
	EXPORT void btPrimitiveTriangle_get_edge_plane(btPrimitiveTriangle* obj, int edge_index, btVector4* plane);
	EXPORT btScalar btPrimitiveTriangle_getDummy(btPrimitiveTriangle* obj);
	EXPORT btScalar btPrimitiveTriangle_getMargin(btPrimitiveTriangle* obj);
	EXPORT void btPrimitiveTriangle_getPlane(btPrimitiveTriangle* obj, btVector4* value);
	EXPORT btVector3* btPrimitiveTriangle_getVertices(btPrimitiveTriangle* obj);
	EXPORT bool btPrimitiveTriangle_overlap_test_conservative(btPrimitiveTriangle* obj, const btPrimitiveTriangle* other);
	EXPORT void btPrimitiveTriangle_setDummy(btPrimitiveTriangle* obj, btScalar value);
	EXPORT void btPrimitiveTriangle_setMargin(btPrimitiveTriangle* obj, btScalar value);
	EXPORT void btPrimitiveTriangle_setPlane(btPrimitiveTriangle* obj, const btVector4* value);
	EXPORT void btPrimitiveTriangle_delete(btPrimitiveTriangle* obj);

	EXPORT btTriangleShapeEx* btTriangleShapeEx_new();
	EXPORT btTriangleShapeEx* btTriangleShapeEx_new2(const btVector3* p0, const btVector3* p1, const btVector3* p2);
	EXPORT btTriangleShapeEx* btTriangleShapeEx_new3(const btTriangleShapeEx* other);
	EXPORT void btTriangleShapeEx_applyTransform(btTriangleShapeEx* obj, const btTransform* t);
	EXPORT void btTriangleShapeEx_buildTriPlane(btTriangleShapeEx* obj, btVector4* plane);
	EXPORT bool btTriangleShapeEx_overlap_test_conservative(btTriangleShapeEx* obj, const btTriangleShapeEx* other);
#ifdef __cplusplus
}
#endif
