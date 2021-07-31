#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBoxShape* btBoxShape_new(const btVector3* boxHalfExtents);
	EXPORT btBoxShape* btBoxShape_new2(btScalar boxHalfExtent);
	EXPORT btBoxShape* btBoxShape_new3(btScalar boxHalfExtentX, btScalar boxHalfExtentY, btScalar boxHalfExtentZ);
	EXPORT void btBoxShape_getHalfExtentsWithMargin(btBoxShape* obj, btVector3* value);
	EXPORT void btBoxShape_getHalfExtentsWithoutMargin(btBoxShape* obj, btVector3* value);
	EXPORT void btBoxShape_getPlaneEquation(btBoxShape* obj, btVector4* plane, int i);
#ifdef __cplusplus
}
#endif
