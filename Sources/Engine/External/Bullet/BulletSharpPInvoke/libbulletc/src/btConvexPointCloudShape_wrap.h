#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConvexPointCloudShape* btConvexPointCloudShape_new();
	EXPORT btConvexPointCloudShape* btConvexPointCloudShape_new2(btVector3* points, int numPoints, const btVector3* localScaling, bool computeAabb);
	EXPORT int btConvexPointCloudShape_getNumPoints(btConvexPointCloudShape* obj);
	EXPORT void btConvexPointCloudShape_getScaledPoint(btConvexPointCloudShape* obj, int index, btVector3* value);
	EXPORT btVector3* btConvexPointCloudShape_getUnscaledPoints(btConvexPointCloudShape* obj);
	EXPORT void btConvexPointCloudShape_setPoints(btConvexPointCloudShape* obj, btVector3* points, int numPoints, bool computeAabb);
	EXPORT void btConvexPointCloudShape_setPoints2(btConvexPointCloudShape* obj, btVector3* points, int numPoints, bool computeAabb, const btVector3* localScaling);
#ifdef __cplusplus
}
#endif
