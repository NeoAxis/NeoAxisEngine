#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btConvexInternalShape_getImplicitShapeDimensions(btConvexInternalShape* obj, btVector3* value);
	EXPORT void btConvexInternalShape_getLocalScalingNV(btConvexInternalShape* obj, btVector3* value);
	EXPORT btScalar btConvexInternalShape_getMarginNV(btConvexInternalShape* obj);
	EXPORT void btConvexInternalShape_setImplicitShapeDimensions(btConvexInternalShape* obj, const btVector3* dimensions);
	EXPORT void btConvexInternalShape_setSafeMargin(btConvexInternalShape* obj, btScalar minDimension, btScalar defaultMarginMultiplier);
	EXPORT void btConvexInternalShape_setSafeMargin2(btConvexInternalShape* obj, const btVector3* halfExtents, btScalar defaultMarginMultiplier);

	EXPORT void btConvexInternalAabbCachingShape_recalcLocalAabb(btConvexInternalAabbCachingShape* obj);
#ifdef __cplusplus
}
#endif
