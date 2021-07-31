#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btConcaveShape_processAllTriangles(btConcaveShape* obj, btTriangleCallback* callback, const btVector3* aabbMin, const btVector3* aabbMax);
#ifdef __cplusplus
}
#endif
