#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btTriangle* btTriangle_new();
	EXPORT int btTriangle_getPartId(btTriangle* obj);
	EXPORT int btTriangle_getTriangleIndex(btTriangle* obj);
	EXPORT void btTriangle_getVertex0(btTriangle* obj, btVector3* value);
	EXPORT void btTriangle_getVertex1(btTriangle* obj, btVector3* value);
	EXPORT void btTriangle_getVertex2(btTriangle* obj, btVector3* value);
	EXPORT void btTriangle_setPartId(btTriangle* obj, int value);
	EXPORT void btTriangle_setTriangleIndex(btTriangle* obj, int value);
	EXPORT void btTriangle_setVertex0(btTriangle* obj, const btVector3* value);
	EXPORT void btTriangle_setVertex1(btTriangle* obj, const btVector3* value);
	EXPORT void btTriangle_setVertex2(btTriangle* obj, const btVector3* value);
	EXPORT void btTriangle_delete(btTriangle* obj);

	EXPORT btTriangleBuffer* btTriangleBuffer_new();
	EXPORT void btTriangleBuffer_clearBuffer(btTriangleBuffer* obj);
	EXPORT int btTriangleBuffer_getNumTriangles(btTriangleBuffer* obj);
	EXPORT const btTriangle* btTriangleBuffer_getTriangle(btTriangleBuffer* obj, int index);
#ifdef __cplusplus
}
#endif
