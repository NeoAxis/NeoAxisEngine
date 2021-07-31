#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btConvexShape_batchedUnitVectorGetSupportingVertexWithoutMargin(btConvexShape* obj, const btVector3* vectors, btVector3* supportVerticesOut, int numVectors);
	EXPORT void btConvexShape_getAabbNonVirtual(btConvexShape* obj, const btTransform* t, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT void btConvexShape_getAabbSlow(btConvexShape* obj, const btTransform* t, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT btScalar btConvexShape_getMarginNonVirtual(btConvexShape* obj);
	EXPORT int btConvexShape_getNumPreferredPenetrationDirections(btConvexShape* obj);
	EXPORT void btConvexShape_getPreferredPenetrationDirection(btConvexShape* obj, int index, btVector3* penetrationVector);
	EXPORT void btConvexShape_localGetSupportingVertex(btConvexShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btConvexShape_localGetSupportingVertexWithoutMargin(btConvexShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btConvexShape_localGetSupportVertexNonVirtual(btConvexShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btConvexShape_localGetSupportVertexWithoutMarginNonVirtual(btConvexShape* obj, const btVector3* vec, btVector3* value);
	EXPORT void btConvexShape_project(btConvexShape* obj, const btTransform* trans, const btVector3* dir, btScalar* minProj, btScalar* maxProj, btVector3* witnesPtMin, btVector3* witnesPtMax);
#ifdef __cplusplus
}
#endif
