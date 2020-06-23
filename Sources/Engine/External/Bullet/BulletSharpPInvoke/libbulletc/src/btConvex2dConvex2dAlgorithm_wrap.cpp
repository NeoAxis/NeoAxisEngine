#include <BulletCollision/CollisionDispatch/btConvex2dConvex2dAlgorithm.h>
#include <BulletCollision/NarrowPhaseCollision/btConvexPenetrationDepthSolver.h>

#include "btConvex2dConvex2dAlgorithm_wrap.h"

btConvex2dConvex2dAlgorithm_CreateFunc* btConvex2dConvex2dAlgorithm_CreateFunc_new(
	btVoronoiSimplexSolver* simplexSolver, btConvexPenetrationDepthSolver* pdSolver)
{
	return new btConvex2dConvex2dAlgorithm::CreateFunc(simplexSolver, pdSolver);
}

int btConvex2dConvex2dAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(
	btConvex2dConvex2dAlgorithm_CreateFunc* obj)
{
	return obj->m_minimumPointsPerturbationThreshold;
}

int btConvex2dConvex2dAlgorithm_CreateFunc_getNumPerturbationIterations(btConvex2dConvex2dAlgorithm_CreateFunc* obj)
{
	return obj->m_numPerturbationIterations;
}

btConvexPenetrationDepthSolver* btConvex2dConvex2dAlgorithm_CreateFunc_getPdSolver(
	btConvex2dConvex2dAlgorithm_CreateFunc* obj)
{
	return obj->m_pdSolver;
}

btVoronoiSimplexSolver* btConvex2dConvex2dAlgorithm_CreateFunc_getSimplexSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj)
{
	return obj->m_simplexSolver;
}

void btConvex2dConvex2dAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(
	btConvex2dConvex2dAlgorithm_CreateFunc* obj, int value)
{
	obj->m_minimumPointsPerturbationThreshold = value;
}

void btConvex2dConvex2dAlgorithm_CreateFunc_setNumPerturbationIterations(btConvex2dConvex2dAlgorithm_CreateFunc* obj,
	int value)
{
	obj->m_numPerturbationIterations = value;
}

void btConvex2dConvex2dAlgorithm_CreateFunc_setPdSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj,
	btConvexPenetrationDepthSolver* value)
{
	obj->m_pdSolver = value;
}

void btConvex2dConvex2dAlgorithm_CreateFunc_setSimplexSolver(btConvex2dConvex2dAlgorithm_CreateFunc* obj,
	btVoronoiSimplexSolver* value)
{
	obj->m_simplexSolver = value;
}


btConvex2dConvex2dAlgorithm* btConvex2dConvex2dAlgorithm_new(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap, btVoronoiSimplexSolver* simplexSolver,
	btConvexPenetrationDepthSolver* pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
{
	return new btConvex2dConvex2dAlgorithm(mf, *ci, body0Wrap, body1Wrap, simplexSolver,
		pdSolver, numPerturbationIterations, minimumPointsPerturbationThreshold);
}

const btPersistentManifold* btConvex2dConvex2dAlgorithm_getManifold(btConvex2dConvex2dAlgorithm* obj)
{
	return obj->getManifold();
}

void btConvex2dConvex2dAlgorithm_setLowLevelOfDetail(btConvex2dConvex2dAlgorithm* obj,
	bool useLowLevel)
{
	obj->setLowLevelOfDetail(useLowLevel);
}
