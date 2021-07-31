#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btConstraintSolverPoolMt* btConstraintSolverPoolMt_new(int numSolvers);
	EXPORT btConstraintSolverPoolMt* btConstraintSolverPoolMt_new2(btConstraintSolver** solvers, int numSolvers);

	EXPORT btDiscreteDynamicsWorldMt* btDiscreteDynamicsWorldMt_new(btDispatcher* dispatcher, btBroadphaseInterface* pairCache, btConstraintSolverPoolMt* constraintSolver, btCollisionConfiguration* collisionConfiguration);
#ifdef __cplusplus
}
#endif
