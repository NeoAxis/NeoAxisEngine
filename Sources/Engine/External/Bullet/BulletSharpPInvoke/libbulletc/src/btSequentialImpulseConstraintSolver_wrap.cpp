#include <BulletDynamics/ConstraintSolver/btSequentialImpulseConstraintSolver.h>

#include "btSequentialImpulseConstraintSolver_wrap.h"

btSequentialImpulseConstraintSolver* btSequentialImpulseConstraintSolver_new()
{
	return ALIGNED_NEW(btSequentialImpulseConstraintSolver) ();
}

unsigned long btSequentialImpulseConstraintSolver_btRand2(btSequentialImpulseConstraintSolver* obj)
{
	return obj->btRand2();
}

int btSequentialImpulseConstraintSolver_btRandInt2(btSequentialImpulseConstraintSolver* obj,
	int n)
{
	return obj->btRandInt2(n);
}

unsigned long btSequentialImpulseConstraintSolver_getRandSeed(btSequentialImpulseConstraintSolver* obj)
{
	return obj->getRandSeed();
}

void btSequentialImpulseConstraintSolver_setRandSeed(btSequentialImpulseConstraintSolver* obj,
	unsigned long seed)
{
	obj->setRandSeed(seed);
}
