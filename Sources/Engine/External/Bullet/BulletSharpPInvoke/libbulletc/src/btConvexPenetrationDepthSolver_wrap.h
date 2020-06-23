#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT bool btConvexPenetrationDepthSolver_calcPenDepth(btConvexPenetrationDepthSolver* obj, btVoronoiSimplexSolver* simplexSolver, const btConvexShape* convexA, const btConvexShape* convexB, const btTransform* transA, const btTransform* transB, btVector3* v, btVector3* pa, btVector3* pb, btIDebugDraw* debugDraw);
	EXPORT void btConvexPenetrationDepthSolver_delete(btConvexPenetrationDepthSolver* obj);
#ifdef __cplusplus
}
#endif
