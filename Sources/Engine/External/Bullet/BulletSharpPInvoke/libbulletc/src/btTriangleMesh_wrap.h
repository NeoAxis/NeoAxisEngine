#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btTriangleMesh* btTriangleMesh_new(bool use32bitIndices, bool use4componentVertices);
	EXPORT void btTriangleMesh_addIndex(btTriangleMesh* obj, int index);
	EXPORT void btTriangleMesh_addTriangle(btTriangleMesh* obj, const btVector3* vertex0, const btVector3* vertex1, const btVector3* vertex2, bool removeDuplicateVertices);
	EXPORT void btTriangleMesh_addTriangleIndices(btTriangleMesh* obj, int index1, int index2, int index3);
	EXPORT int btTriangleMesh_findOrAddVertex(btTriangleMesh* obj, const btVector3* vertex, bool removeDuplicateVertices);
	EXPORT int btTriangleMesh_getNumTriangles(btTriangleMesh* obj);
	EXPORT bool btTriangleMesh_getUse32bitIndices(btTriangleMesh* obj);
	EXPORT bool btTriangleMesh_getUse4componentVertices(btTriangleMesh* obj);
	EXPORT btScalar btTriangleMesh_getWeldingThreshold(btTriangleMesh* obj);
	EXPORT void btTriangleMesh_setWeldingThreshold(btTriangleMesh* obj, btScalar value);
#ifdef __cplusplus
}
#endif
