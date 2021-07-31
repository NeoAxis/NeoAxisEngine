#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMLCPSolver* btMLCPSolver_new(btMLCPSolverInterface* solver);
	EXPORT int btMLCPSolver_getNumFallbacks(btMLCPSolver* obj);
	EXPORT void btMLCPSolver_setMLCPSolver(btMLCPSolver* obj, btMLCPSolverInterface* solver);
	EXPORT void btMLCPSolver_setNumFallbacks(btMLCPSolver* obj, int num);
#ifdef __cplusplus
}
#endif
