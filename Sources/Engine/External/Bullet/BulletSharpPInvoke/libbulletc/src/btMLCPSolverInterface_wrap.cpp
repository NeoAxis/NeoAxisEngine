#include <BulletDynamics/MLCPSolvers/btMLCPSolverInterface.h>

#include "btMLCPSolverInterface_wrap.h"
/*
bool btMLCPSolverInterface_solveMLCP(btMLCPSolverInterface* obj, const btMatrixX_float* A,
	const btVectorX_float* b, btVectorX_float* x, const btVectorX_float* lo,
	const btVectorX_float* hi, const btAlignedObjectArray_int* limitDependency,
	int numIterations, bool useSparsity)
{
	return obj->solveMLCP(*A, *b, *x, *lo, *hi, *limitDependency, numIterations,
		useSparsity);
}
*/
void btMLCPSolverInterface_delete(btMLCPSolverInterface* obj)
{
	delete obj;
}
