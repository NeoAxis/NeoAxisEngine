#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btEmptyAlgorithm_CreateFunc* btEmptyAlgorithm_CreateFunc_new();

	EXPORT btEmptyAlgorithm* btEmptyAlgorithm_new(const btCollisionAlgorithmConstructionInfo* ci);
#ifdef __cplusplus
}
#endif
