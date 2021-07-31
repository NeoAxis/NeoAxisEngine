#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btContinuousConvexCollision* btContinuousConvexCollision_new(const btConvexShape* shapeA, const btConvexShape* shapeB, btVoronoiSimplexSolver* simplexSolver, btConvexPenetrationDepthSolver* penetrationDepthSolver);
	EXPORT btContinuousConvexCollision* btContinuousConvexCollision_new2(const btConvexShape* shapeA, const btStaticPlaneShape* plane);
#ifdef __cplusplus
}
#endif
