#include <BulletCollision/NarrowPhaseCollision/btMinkowskiPenetrationDepthSolver.h>

#include "btMinkowskiPenetrationDepthSolver_wrap.h"

btMinkowskiPenetrationDepthSolver* btMinkowskiPenetrationDepthSolver_new()
{
	return new btMinkowskiPenetrationDepthSolver();
}
