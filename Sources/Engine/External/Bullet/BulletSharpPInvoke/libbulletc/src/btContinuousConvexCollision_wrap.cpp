#include <BulletCollision/CollisionShapes/btConvexShape.h>
#include <BulletCollision/CollisionShapes/btStaticPlaneShape.h>
#include <BulletCollision/NarrowPhaseCollision/btContinuousConvexCollision.h>
#include <BulletCollision/NarrowPhaseCollision/btConvexPenetrationDepthSolver.h>

#include "btContinuousConvexCollision_wrap.h"

btContinuousConvexCollision* btContinuousConvexCollision_new(const btConvexShape* shapeA,
	const btConvexShape* shapeB, btVoronoiSimplexSolver* simplexSolver, btConvexPenetrationDepthSolver* penetrationDepthSolver)
{
	return new btContinuousConvexCollision(shapeA, shapeB, simplexSolver, penetrationDepthSolver);
}

btContinuousConvexCollision* btContinuousConvexCollision_new2(const btConvexShape* shapeA,
	const btStaticPlaneShape* plane)
{
	return new btContinuousConvexCollision(shapeA, plane);
}
