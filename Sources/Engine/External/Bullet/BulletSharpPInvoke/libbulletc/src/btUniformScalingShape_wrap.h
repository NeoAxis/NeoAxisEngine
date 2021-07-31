#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btUniformScalingShape* btUniformScalingShape_new(btConvexShape* convexChildShape, btScalar uniformScalingFactor);
	EXPORT btConvexShape* btUniformScalingShape_getChildShape(btUniformScalingShape* obj);
	EXPORT btScalar btUniformScalingShape_getUniformScalingFactor(btUniformScalingShape* obj);
#ifdef __cplusplus
}
#endif
