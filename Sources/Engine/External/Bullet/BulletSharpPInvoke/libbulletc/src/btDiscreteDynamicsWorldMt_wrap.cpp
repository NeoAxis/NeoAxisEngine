#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletCollision/CollisionDispatch/btSimulationIslandManager.h>
#include <BulletDynamics/ConstraintSolver/btConstraintSolver.h>
#include <BulletDynamics/Dynamics/btDiscreteDynamicsWorldMt.h>

#include "btDiscreteDynamicsWorldMt_wrap.h"

btConstraintSolverPoolMt* btConstraintSolverPoolMt_new(int numSolvers)
{
	return ALIGNED_NEW(btConstraintSolverPoolMt)(numSolvers);
}

btConstraintSolverPoolMt* btConstraintSolverPoolMt_new2(btConstraintSolver** solvers, int numSolvers)
{
	return ALIGNED_NEW(btConstraintSolverPoolMt)(solvers, numSolvers);
}


btDiscreteDynamicsWorldMt* btDiscreteDynamicsWorldMt_new(btDispatcher* dispatcher, btBroadphaseInterface* pairCache,
	btConstraintSolverPoolMt* constraintSolver, btCollisionConfiguration* collisionConfiguration)
{
	return new btDiscreteDynamicsWorldMt(dispatcher, pairCache, constraintSolver, collisionConfiguration);
}
