#include <LinearMath/btGeometryUtil.h>

#include "conversion.h"
#include "btGeometryUtil_wrap.h"

#ifndef BULLETC_DISABLE_GEOMETRY_UTIL

bool btGeometryUtil_areVerticesBehindPlane(const btVector3* planeNormal, const btAlignedObjectArray_btVector3* vertices,
	btScalar margin)
{
	BTVECTOR3_IN(planeNormal);
	return btGeometryUtil::areVerticesBehindPlane(BTVECTOR3_USE(planeNormal), *vertices,
		margin);
}

void btGeometryUtil_getPlaneEquationsFromVertices(btAlignedObjectArray_btVector3* vertices,
	btAlignedObjectArray_btVector3* planeEquationsOut)
{
	btGeometryUtil::getPlaneEquationsFromVertices(*vertices, *planeEquationsOut);
}

void btGeometryUtil_getVerticesFromPlaneEquations(const btAlignedObjectArray_btVector3* planeEquations,
	btAlignedObjectArray_btVector3* verticesOut)
{
	btGeometryUtil::getVerticesFromPlaneEquations(*planeEquations, *verticesOut);
}
/*
bool btGeometryUtil_isInside(const btAlignedObjectArray_btVector3* vertices, const btVector3* planeNormal,
	btScalar margin)
{
	BTVECTOR3_IN(planeNormal);
	return btGeometryUtil::isInside(*vertices, BTVECTOR3_USE(planeNormal), margin);
}
*/
bool btGeometryUtil_isPointInsidePlanes(const btAlignedObjectArray_btVector3* planeEquations,
	const btVector3* point, btScalar margin)
{
	BTVECTOR3_IN(point);
	return btGeometryUtil::isPointInsidePlanes(*planeEquations, BTVECTOR3_USE(point),
		margin);
}

#endif
