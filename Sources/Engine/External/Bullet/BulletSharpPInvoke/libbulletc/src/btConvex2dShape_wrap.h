#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConvex2dShape* btConvex2dShape_new(btConvexShape* convexChildShape);
	EXPORT btConvexShape* btConvex2dShape_getChildShape(btConvex2dShape* obj);
#ifdef __cplusplus
}
#endif
