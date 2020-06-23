#include <LinearMath/btThreads.h>

#include "btThreads_wrap.h"

int btITaskScheduler_getMaxNumThreads(btITaskScheduler* obj)
{
	return obj->getMaxNumThreads();
}

const char* btITaskScheduler_getName(btITaskScheduler* obj)
{
	return obj->getName();
}

int btITaskScheduler_getNumThreads(btITaskScheduler* obj)
{
	return obj->getNumThreads();
}

void btITaskScheduler_setNumThreads(btITaskScheduler* obj, int numThreads)
{
	obj->setNumThreads(numThreads);
}

btITaskScheduler* btThreads_btGetSequentialTaskScheduler()
{
	return btGetSequentialTaskScheduler();
}

btITaskScheduler* btThreads_btGetOpenMPTaskScheduler()
{
	return btGetOpenMPTaskScheduler();
}

btITaskScheduler* btThreads_btGetPPLTaskScheduler()
{
	return btGetPPLTaskScheduler();
}

btITaskScheduler* btThreads_btGetTBBTaskScheduler()
{
	return btGetTBBTaskScheduler();
}

void btThreads_btSetTaskScheduler(btITaskScheduler* ts)
{
	btSetTaskScheduler(ts);
}
