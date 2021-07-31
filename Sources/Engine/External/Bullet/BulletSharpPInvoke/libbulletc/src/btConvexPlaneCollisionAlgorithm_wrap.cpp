#include <BulletCollision/CollisionDispatch/btConvexPlaneCollisionAlgorithm.h>

#include "conversion.h"
#include "btConvexPlaneCollisionAlgorithm_wrap.h"

btConvexPlaneCollisionAlgorithm_CreateFunc* btConvexPlaneCollisionAlgorithm_CreateFunc_new()
{
	return new btConvexPlaneCollisionAlgorithm::CreateFunc();
}

int btConvexPlaneCollisionAlgorithm_CreateFunc_getMinimumPointsPerturbationThreshold(
	btConvexPlaneCollisionAlgorithm_CreateFunc* obj)
{
	return obj->m_minimumPointsPerturbationThreshold;
}

int btConvexPlaneCollisionAlgorithm_CreateFunc_getNumPerturbationIterations(btConvexPlaneCollisionAlgorithm_CreateFunc* obj)
{
	return obj->m_numPerturbationIterations;
}

void btConvexPlaneCollisionAlgorithm_CreateFunc_setMinimumPointsPerturbationThreshold(
	btConvexPlaneCollisionAlgorithm_CreateFunc* obj, int value)
{
	obj->m_minimumPointsPerturbationThreshold = value;
}

void btConvexPlaneCollisionAlgorithm_CreateFunc_setNumPerturbationIterations(btConvexPlaneCollisionAlgorithm_CreateFunc* obj,
	int value)
{
	obj->m_numPerturbationIterations = value;
}


btConvexPlaneCollisionAlgorithm* btConvexPlaneCollisionAlgorithm_new(btPersistentManifold* mf,
	const btCollisionAlgorithmConstructionInfo* ci, const btCollisionObjectWrapper* body0Wrap,
	const btCollisionObjectWrapper* body1Wrap, bool isSwapped, int numPerturbationIterations,
	int minimumPointsPerturbationThreshold)
{
	return new btConvexPlaneCollisionAlgorithm(mf, *ci, body0Wrap, body1Wrap, isSwapped,
		numPerturbationIterations, minimumPointsPerturbationThreshold);
}

void btConvexPlaneCollisionAlgorithm_collideSingleContact(btConvexPlaneCollisionAlgorithm* obj,
	const btQuaternion* perturbeRot, const btCollisionObjectWrapper* body0Wrap, const btCollisionObjectWrapper* body1Wrap,
	const btDispatcherInfo* dispatchInfo, btManifoldResult* resultOut)
{
	BTQUATERNION_IN(perturbeRot);
	obj->collideSingleContact(BTQUATERNION_USE(perturbeRot), body0Wrap, body1Wrap,
		*dispatchInfo, resultOut);
}
