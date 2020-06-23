#include <BulletCollision/CollisionShapes/btConvexShape.h>
#include <BulletCollision/NarrowPhaseCollision/btGjkConvexCast.h>

#include "btGjkConvexCast_wrap.h"

btGjkConvexCast* btGjkConvexCast_new(const btConvexShape* convexA, const btConvexShape* convexB,
	btVoronoiSimplexSolver* simplexSolver)
{
	return new btGjkConvexCast(convexA, convexB, simplexSolver);
}
