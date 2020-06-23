#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/CollisionDispatch/btCollisionObject.h>
#include <BulletCollision/NarrowPhaseCollision/btPersistentManifold.h>
#include <BulletDynamics/ConstraintSolver/btConstraintSolver.h>
#include <BulletDynamics/ConstraintSolver/btContactSolverInfo.h>
#include <BulletDynamics/ConstraintSolver/btTypedConstraint.h>
#include <LinearMath/btIDebugDraw.h>

#include "btConstraintSolver_wrap.h"

void btConstraintSolver_allSolved(btConstraintSolver* obj, const btContactSolverInfo* __unnamed0,
	btIDebugDraw* __unnamed1)
{
	obj->allSolved(*__unnamed0, __unnamed1);
}

btConstraintSolverType btConstraintSolver_getSolverType(btConstraintSolver* obj)
{
	return obj->getSolverType();
}

void btConstraintSolver_prepareSolve(btConstraintSolver* obj, int __unnamed0, int __unnamed1)
{
	obj->prepareSolve(__unnamed0, __unnamed1);
}

void btConstraintSolver_reset(btConstraintSolver* obj)
{
	obj->reset();
}

btScalar btConstraintSolver_solveGroup(btConstraintSolver* obj, btCollisionObject** bodies,
	int numBodies, btPersistentManifold** manifold, int numManifolds, btTypedConstraint** constraints,
	int numConstraints, const btContactSolverInfo* info, btIDebugDraw* debugDrawer,
	btDispatcher* dispatcher)
{
	return obj->solveGroup(bodies, numBodies, manifold, numManifolds, constraints,
		numConstraints, *info, debugDrawer, dispatcher);
}

void btConstraintSolver_delete(btConstraintSolver* obj)
{
	ALIGNED_FREE(obj);
}
