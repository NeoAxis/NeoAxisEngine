#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT const btConvexPolyhedron* btPolyhedralConvexShape_getConvexPolyhedron(btPolyhedralConvexShape* obj);
	EXPORT void btPolyhedralConvexShape_getEdge(btPolyhedralConvexShape* obj, int i, btVector3* pa, btVector3* pb);
	EXPORT int btPolyhedralConvexShape_getNumEdges(btPolyhedralConvexShape* obj);
	EXPORT int btPolyhedralConvexShape_getNumPlanes(btPolyhedralConvexShape* obj);
	EXPORT int btPolyhedralConvexShape_getNumVertices(btPolyhedralConvexShape* obj);
	EXPORT void btPolyhedralConvexShape_getPlane(btPolyhedralConvexShape* obj, btVector3* planeNormal, btVector3* planeSupport, int i);
	EXPORT void btPolyhedralConvexShape_getVertex(btPolyhedralConvexShape* obj, int i, btVector3* vtx);
	EXPORT bool btPolyhedralConvexShape_initializePolyhedralFeatures(btPolyhedralConvexShape* obj, int shiftVerticesByMargin);
	EXPORT bool btPolyhedralConvexShape_isInside(btPolyhedralConvexShape* obj, const btVector3* pt, btScalar tolerance);

	EXPORT void btPolyhedralConvexAabbCachingShape_getNonvirtualAabb(btPolyhedralConvexAabbCachingShape* obj, const btTransform* trans, btVector3* aabbMin, btVector3* aabbMax, btScalar margin);
	EXPORT void btPolyhedralConvexAabbCachingShape_recalcLocalAabb(btPolyhedralConvexAabbCachingShape* obj);
#ifdef __cplusplus
}
#endif
