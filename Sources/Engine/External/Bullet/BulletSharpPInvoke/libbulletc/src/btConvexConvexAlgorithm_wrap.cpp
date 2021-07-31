#include <BulletCollision/CollisionDispatch/btConvexConvexAlgorithm.h>
#include <BulletCollision/NarrowPhaseCollision/btConvexPenetrationDepthSolver.h>

#include "btConvexConvexAlgorithm_wrap.h"

btConvexConvexAlgorithm_CreateFunc* btConvexConvexAlgorithm_CreateFunc_new(btConvexPenetrationDepthSolver* pdSolver)
{
	return new btConvexConvexAlgorithm::CreateFunc(pdSolver);
}

int btConvexConvexAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(btConvexConvexAlgorithm_CreateFunc* obj)
{
	return obj->m_minimumPointsPerturbationThreshold;
}

int btConvexConvexAlgorithm_CreateFunc_getNumPerturbationIterations(btConvexConvexAlgorithm_CreateFunc* obj)
{
	return obj->m_numPerturbationIterations;
}

btConvexPenetrationDepthSolver* btConvexConvexAlgorithm_CreateFunc_getPdSolver(btConvexConvexAlgorithm_CreateFunc* obj)
{
	return obj->m_pdSolver;
}

void btConvexConvexAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(btConvexConvexAlgorithm_CreateFunc* obj,
	int value)
{
	obj->m_minimumPointsPerturbationThreshold = value;
}

void btConvexConvexAlgorithm_CreateFunc_setNumPerturbationIterations(btConvexConvexAlgorithm_CreateFunc* obj,
	int value)
{
	obj->m_numPerturbationIterations = value;
}

void btConvexConvexAlgorithm_CreateFunc_setPdSolver(btConvexConvexAlgorithm_CreateFunc* obj,
	btConvexPenetrationDepthSolver* value)
{
	obj->m_pdSolver = value;
}


btConvexConvexAlgorithm* btConvexConvexAlgorithm_new(btPersistentManifold* mf, const btCollisionAlgorithmConstructionInfo* ci,
	const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	btConvexPenetrationDepthSolver* pdSolver, int numPerturbationIterations,
	int minimumPointsPerturbationThreshold)
{
	return new btConvexConvexAlgorithm(mf, *ci, body0Wrap, body1Wrap, pdSolver,
		numPerturbationIterations, minimumPointsPerturbationThreshold);
}

const btPersistentManifold* btConvexConvexAlgorithm_getManifold(btConvexConvexAlgorithm* obj)
{
	return obj->getManifold();
}

void btConvexConvexAlgorithm_setLowLevelOfDetail(btConvexConvexAlgorithm* obj, bool useLowLevel)
{
	obj->setLowLevelOfDetail(useLowLevel);
}
