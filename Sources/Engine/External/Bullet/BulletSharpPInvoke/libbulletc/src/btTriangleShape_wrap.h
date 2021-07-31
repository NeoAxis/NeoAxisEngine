#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btTriangleShape* btTriangleShape_new();
	EXPORT btTriangleShape* btTriangleShape_new2(const btVector3* p0, const btVector3* p1, const btVector3* p2);
	EXPORT void btTriangleShape_calcNormal(btTriangleShape* obj, btVector3* normal);
	EXPORT void btTriangleShape_getPlaneEquation(btTriangleShape* obj, int i, btVector3* planeNormal, btVector3* planeSupport);
	EXPORT const btScalar* btTriangleShape_getVertexPtr(btTriangleShape* obj, int index);
	EXPORT btVector3* btTriangleShape_getVertices1(btTriangleShape* obj);
#ifdef __cplusplus
}
#endif
