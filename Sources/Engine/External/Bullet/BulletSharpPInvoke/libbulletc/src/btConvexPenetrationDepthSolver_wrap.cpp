#include <BulletCollision/CollisionShapes/btConvexShape.h>
#include <BulletCollision/NarrowPhaseCollision/btConvexPenetrationDepthSolver.h>
#include <LinearMath/btIDebugDraw.h>

#include "conversion.h"
#include "btConvexPenetrationDepthSolver_wrap.h"

bool btConvexPenetrationDepthSolver_calcPenDepth(btConvexPenetrationDepthSolver* obj,
	btVoronoiSimplexSolver* simplexSolver, const btConvexShape* convexA, const btConvexShape* convexB,
	const btTransform* transA, const btTransform* transB, btVector3* v, btVector3* pa,
	btVector3* pb, btIDebugDraw* debugDraw)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	BTVECTOR3_DEF(v);
	BTVECTOR3_DEF(pa);
	BTVECTOR3_DEF(pb);
	bool ret = obj->calcPenDepth(*simplexSolver, convexA, convexB, BTTRANSFORM_USE(transA),
		BTTRANSFORM_USE(transB), BTVECTOR3_USE(v), BTVECTOR3_USE(pa), BTVECTOR3_USE(pb),
		debugDraw);
	BTVECTOR3_DEF_OUT(v);
	BTVECTOR3_DEF_OUT(pa);
	BTVECTOR3_DEF_OUT(pb);
	return ret;
}

void btConvexPenetrationDepthSolver_delete(btConvexPenetrationDepthSolver* obj)
{
	delete obj;
}
