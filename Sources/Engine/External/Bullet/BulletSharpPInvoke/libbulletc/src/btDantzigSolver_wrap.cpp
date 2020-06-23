#include <BulletDynamics/MLCPSolvers/btDantzigSolver.h>

#include "btDantzigSolver_wrap.h"

btDantzigSolver* btDantzigSolver_new()
{
	return new btDantzigSolver();
}
