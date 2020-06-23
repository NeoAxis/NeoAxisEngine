#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	//EXPORT bool btMLCPSolverInterface_solveMLCP(btMLCPSolverInterface* obj, const btMatrixX_float* A, const btVectorX_float* b, btVectorX_float* x, const btVectorX_float* lo, const btVectorX_float* hi, const btAlignedObjectArray_int* limitDependency, int numIterations, bool useSparsity);
	EXPORT void btMLCPSolverInterface_delete(btMLCPSolverInterface* obj);
#ifdef __cplusplus
}
#endif
