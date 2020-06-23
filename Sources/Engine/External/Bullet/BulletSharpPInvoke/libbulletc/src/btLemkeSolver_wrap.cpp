// FIXME: weird linker error, unresolved external symbol btLemkeAlgorithm::solve
#if 0
#include <BulletDynamics/MLCPSolvers/btLemkeSolver.h>

#include "btLemkeSolver_wrap.h"

btLemkeSolver* btLemkeSolver_new()
{
	return new btLemkeSolver();
}

int btLemkeSolver_getDebugLevel(btLemkeSolver* obj)
{
	return obj->m_debugLevel;
}

int btLemkeSolver_getMaxLoops(btLemkeSolver* obj)
{
	return obj->m_maxLoops;
}

btScalar btLemkeSolver_getMaxValue(btLemkeSolver* obj)
{
	return obj->m_maxValue;
}

bool btLemkeSolver_getUseLoHighBounds(btLemkeSolver* obj)
{
	return obj->m_useLoHighBounds;
}

void btLemkeSolver_setDebugLevel(btLemkeSolver* obj, int value)
{
	obj->m_debugLevel = value;
}

void btLemkeSolver_setMaxLoops(btLemkeSolver* obj, int value)
{
	obj->m_maxLoops = value;
}

void btLemkeSolver_setMaxValue(btLemkeSolver* obj, btScalar value)
{
	obj->m_maxValue = value;
}

void btLemkeSolver_setUseLoHighBounds(btLemkeSolver* obj, bool value)
{
	obj->m_useLoHighBounds = value;
}
#endif
