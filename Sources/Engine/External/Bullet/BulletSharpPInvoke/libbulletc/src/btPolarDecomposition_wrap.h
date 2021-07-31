#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btPolarDecomposition* btPolarDecomposition_new(btScalar tolerance, unsigned int maxIterations);
	EXPORT unsigned int btPolarDecomposition_decompose(btPolarDecomposition* obj, const btMatrix3x3* a, btMatrix3x3* u, btMatrix3x3* h);
	EXPORT unsigned int btPolarDecomposition_maxIterations(btPolarDecomposition* obj);
	EXPORT void btPolarDecomposition_delete(btPolarDecomposition* obj);
#ifdef __cplusplus
}
#endif
