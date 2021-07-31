#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT int btITaskScheduler_getMaxNumThreads(btITaskScheduler* obj);
	EXPORT const char* btITaskScheduler_getName(btITaskScheduler* obj);
	EXPORT int btITaskScheduler_getNumThreads(btITaskScheduler* obj);
	EXPORT void btITaskScheduler_setNumThreads(btITaskScheduler* obj, int numThreads);

	EXPORT btITaskScheduler* btThreads_btGetSequentialTaskScheduler();
	EXPORT btITaskScheduler* btThreads_btGetOpenMPTaskScheduler();
	EXPORT btITaskScheduler* btThreads_btGetPPLTaskScheduler();
	EXPORT btITaskScheduler* btThreads_btGetTBBTaskScheduler();
	EXPORT void btThreads_btSetTaskScheduler(btITaskScheduler* ts);
#ifdef __cplusplus
}
#endif
