#include <BulletDynamics/MLCPSolvers/btMLCPSolver.h>

#include "btMLCPSolver_wrap.h"

btMLCPSolver* btMLCPSolver_new(btMLCPSolverInterface* solver)
{
	return new btMLCPSolver(solver);
}

int btMLCPSolver_getNumFallbacks(btMLCPSolver* obj)
{
	return obj->getNumFallbacks();
}

void btMLCPSolver_setMLCPSolver(btMLCPSolver* obj, btMLCPSolverInterface* solver)
{
	obj->setMLCPSolver(solver);
}

void btMLCPSolver_setNumFallbacks(btMLCPSolver* obj, int num)
{
	obj->setNumFallbacks(num);
}
