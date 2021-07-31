#include "main.h"

#ifndef BULLETC_DISABLE_GEOMETRY_UTIL
#ifdef __cplusplus
extern "C" {
#endif
	EXPORT bool btGeometryUtil_areVerticesBehindPlane(const btVector3* planeNormal, const btAlignedObjectArray_btVector3* vertices, btScalar margin);
	EXPORT void btGeometryUtil_getPlaneEquationsFromVertices(btAlignedObjectArray_btVector3* vertices, btAlignedObjectArray_btVector3* planeEquationsOut);
	EXPORT void btGeometryUtil_getVerticesFromPlaneEquations(const btAlignedObjectArray_btVector3* planeEquations, btAlignedObjectArray_btVector3* verticesOut);
	//EXPORT bool btGeometryUtil_isInside(const btAlignedObjectArray_btVector3* vertices, const btVector3* planeNormal, btScalar margin);
	EXPORT bool btGeometryUtil_isPointInsidePlanes(const btAlignedObjectArray_btVector3* planeEquations, const btVector3* point, btScalar margin);
}
#ifdef __cplusplus
#endif
#endif